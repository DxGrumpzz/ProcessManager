#include <windows.h>
#include <string>
#include <vector>
#include <fstream>


#define RBG_UNIFORM(uniformColour) RGB(uniformColour,uniformColour,uniformColour) 


// Takes a DWORD error code and returns its string message 
std::wstring GetErrorStringW(DWORD error)
{

    // Stores the error message as a string in memory
    LPWSTR buffer = nullptr;

    // Format DWORD error ID to a string 
    FormatMessageW(FORMAT_MESSAGE_ALLOCATE_BUFFER | FORMAT_MESSAGE_FROM_SYSTEM | FORMAT_MESSAGE_IGNORE_INSERTS,
                   NULL,
                   error,
                   MAKELANGID(LANG_NEUTRAL, SUBLANG_DEFAULT),
                   (LPWSTR)&buffer, 0, NULL);

    // Create std string from buffer
    std::wstring message(buffer);

    return message;
};


const wchar_t* windowTitle = L"Window title";
const wchar_t* windowClassName = L"DesktopApp";

HWINEVENTHOOK _hook;


HWND button;
HWND button2;


std::vector<HWND> ProcessesLabels;


class ProcessModel;

std::vector<ProcessModel> _processList;

void CALLBACK WinEventHookCallback(HWINEVENTHOOK hWinEventHook, DWORD event, HWND hwnd, LONG idObject, LONG idChild, DWORD idEventThread, DWORD dwmsEventTime);


class ProcessModel
{
public:
    std::wstring ProcessName;
    std::wstring ProcessArgs;

    PROCESS_INFORMATION ProcessInfo = { 0 };
    STARTUPINFOW info = { 0 };

    std::vector<HWND> handles;

    bool Creating = false;


public:

    ProcessModel(std::wstring processName, std::wstring processArgs) :
        ProcessName(processName),
        ProcessArgs(processArgs)
    {
        info.cb = sizeof(STARTUPINFOW);
        info.dwFlags = STARTF_USESHOWWINDOW;
        info.wShowWindow = SW_HIDE;
    };


public:

    BOOL RunProcess()
    {
        if (!CreateProcessW(ProcessName.c_str(), NULL, NULL, NULL, FALSE, CREATE_SUSPENDED | CREATE_NO_WINDOW, NULL, NULL, &info, &ProcessInfo))
        {
            CloseHandle(ProcessInfo.hProcess);
            CloseHandle(ProcessInfo.hThread);

            return FALSE;
        };

        _hook = SetWinEventHook(EVENT_OBJECT_CREATE, EVENT_OBJECT_CREATE, NULL, WinEventHookCallback, ProcessInfo.dwProcessId, 0, WINEVENT_OUTOFCONTEXT);

        if (!ResumeThread(ProcessInfo.hThread))
        {
            CloseHandle(ProcessInfo.hProcess);
            CloseHandle(ProcessInfo.hThread);

            return FALSE;
        };

        return TRUE;
    };

    BOOL CloseProcess()
    {
        DWORD exitCode;
        GetExitCodeProcess(ProcessInfo.hProcess, &exitCode);

        if (exitCode == STILL_ACTIVE)
        {
            TerminateProcess(ProcessInfo.hProcess, 0);

            if (!CloseHandle(ProcessInfo.hProcess))
                return FALSE;

            if (!CloseHandle(ProcessInfo.hThread))
                return FALSE;
        }

        return TRUE;
    }

};


void CALLBACK WinEventHookCallback(HWINEVENTHOOK hWinEventHook, DWORD event, HWND hwnd, LONG idObject, LONG idChild, DWORD idEventThread, DWORD dwmsEventTime)
{
    DWORD processId;
    GetWindowThreadProcessId(hwnd, &processId);

    for (ProcessModel& process : _processList)
    {
        if (process.ProcessInfo.dwProcessId == processId)
        {
            process.Creating = true;
            process.handles.push_back(hwnd);
        };
    };
};




LRESULT CALLBACK WindowProcedure(HWND hWnd, UINT message, WPARAM wParam, LPARAM lParam)
{
    switch (message)
    {
        case WM_CLOSE:
        {
            PostQuitMessage(0);

            DWORD exitCode;
            for (ProcessModel& process : _processList)
            {
                process.CloseProcess();
            };

            return TRUE;
        };

        case  WM_COMMAND:
        {

            if (HIWORD(wParam) == BN_CLICKED)
            {
                WORD controlID = LOWORD(wParam);

                switch (controlID)
                {
                    // Create process button
                    case 1:
                    {
                        for (ProcessModel& process : _processList)
                        {
                            process.RunProcess();
                        };

                        return TRUE;
                    };

                    // Abort process button
                    case 2:
                    {
                        for (ProcessModel& process : _processList)
                        {
                            process.CloseProcess();
                        };

                        return TRUE;
                    };


                    default:
                        return DefWindowProcW(hWnd, message, wParam, lParam);
                };
            };

            return TRUE;
        };

        case WM_SIZE:
        {
            const int newWidth = LOWORD(lParam);

            int counter = 0;
            for (const HWND& _hwnd : ProcessesLabels)
            {
                RECT windowRect;
                GetWindowRect(_hwnd, &windowRect);

                const int windowWidth = windowRect.right - windowRect.left;
                const int windowHeight = windowRect.bottom - windowRect.top;

                const int newX = (newWidth - windowWidth) - 10;
                const int newY = ((windowHeight + 4) * counter) + 10;


                SetWindowPos(_hwnd, NULL,
                             newX, newY,
                             0, 0,
                             SWP_NOSIZE | SWP_NOZORDER);

                counter++;
            };

            return TRUE;
        }

        case WM_DESTROY:
        {
            for (ProcessModel& process : _processList)
            {
                process.CloseProcess();
            };

            return TRUE;
        };


        default:
            return DefWindowProcW(hWnd, message, wParam, lParam);
    };
};



// Returns a list of ProcessModel which contain name and arguments of a process 
std::vector<ProcessModel> GetProcessListFromFile(const wchar_t* filename = L"Processes.txt")
{
    // Stores the list of processes as a ProcessModel struct
    std::vector<ProcessModel> processes;

    // The processes file
    std::wifstream file(filename);

    // If file is invalid
    if (!file)
    {
        std::wstring error = L"File error. \nCould not open: ";
        error.append(filename);

        size_t outputSize = error.size() + 1;

        char* outputString = new char[outputSize];

        size_t charsConverted = 0;

        const wchar_t* inputW = error.c_str();

        wcstombs_s(&charsConverted, outputString, outputSize, inputW, error.size());

        throw std::exception(outputString);
        delete[] outputString;
    };


    // This is absolute aids. 
    // This will improve


    // Iterate through the file line by line
    // Store current read line 
    std::wstring line;
    while (std::getline(file, line))
    {
        // If current line is the process name
        if (line == L"[Process]")
        {
            // Read process name into current line
            std::getline(file, line);

            // Add the process to the list
            processes.emplace_back(line, L"");
        }
        // If current line is the process' arguments
        else if (line == L"[Args]")
        {
            // Read next line
            std::getline(file, line);

            // Because of the way command arguments are interpreted a space must be inserted in the beggining of the string
            line.insert(line.begin(), ' ');

            // Set the process' arguments
            auto process = (processes.end() - 1);
            process->ProcessArgs = line;
        };
    };

    return processes;
};



// _In_opt_ nad _In_ are something called SAL annotations, They mean that a parameter maybe be passed as NULL
int WINAPI wWinMain(_In_ HINSTANCE hInstance, _In_opt_ HINSTANCE hPrevInstance, _In_ LPWSTR lpCmdLine, _In_ int nShowCmd)
{
    WNDCLASSEXW windowClass = { 0 };
    windowClass.cbSize = sizeof(WNDCLASSEXW);
    windowClass.style = CS_HREDRAW | CS_VREDRAW;
    windowClass.hInstance = hInstance;
    windowClass.lpszClassName = windowClassName;
    windowClass.lpfnWndProc = WindowProcedure;
    windowClass.hbrBackground = CreateSolidBrush(RBG_UNIFORM(0xE1));
    windowClass.hCursor = LoadCursorW(NULL, IDC_ARROW);


    ATOM registerClassResult = RegisterClassExW(&windowClass);

    if (registerClassResult == 0)
    {
        std::wstring error = GetErrorStringW(GetLastError());
        error.insert(0, L"An error occured while creating window.\n");

        MessageBoxW(NULL, error.c_str(), NULL, NULL);

        return 1;
    };




    HWND windowHWND = CreateWindowExW(NULL,
                                      windowClassName,
                                      windowTitle,
                                      WS_CAPTION | WS_BORDER | WS_SYSMENU | WS_MAXIMIZEBOX | WS_MINIMIZEBOX | WS_SIZEBOX,
                                      0, 0,
                                      800, 350,
                                      NULL,
                                      NULL,
                                      hInstance,
                                      NULL);


    if (windowHWND == 0)
    {
        std::wstring error = GetErrorStringW(GetLastError());
        error.insert(0, L"An error occured while creating window.\n");

        MessageBoxW(NULL, error.c_str(), NULL, NULL);

        return 1;
    };

    button = CreateWindowExW(NULL,
                             L"BUTTON",
                             L"Run processes",
                             WS_BORDER | WS_CHILD,
                             10,
                             10,
                             120,
                             30,
                             windowHWND,
                             (HMENU)1,
                             hInstance,
                             NULL);


    button2 = CreateWindowExW(NULL,
                              L"BUTTON",
                              L"Stop processes",
                              WS_BORDER | WS_CHILD,
                              10,
                              45,
                              120,
                              30,
                              windowHWND,
                              (HMENU)2,
                              hInstance,
                              NULL);

    // Set button cursor
    SetClassLongPtrW(button, -12, (LONG_PTR)LoadCursorW(NULL, IDC_HAND));


    _processList = GetProcessListFromFile();

    int longestProcessName = 0;

    for (const ProcessModel& process : _processList)
    {
        if (process.ProcessName.size() > longestProcessName)
            longestProcessName = process.ProcessName.size();
    };

    const int  CHAR_MULTIPLIER = 8;

    const int TEXT_WIDTH = longestProcessName * CHAR_MULTIPLIER;


    int index = 0;
    for (const ProcessModel& process : _processList)
    {
        int TEXT_HEIGHT = (int)(std::count(process.ProcessName.begin(), process.ProcessName.end(), L'\n') + 1) * 20;

        const int TEXT_X_POSITION = abs(500 - TEXT_WIDTH) - 15;
        const int TEXT_Y_POSITION = TEXT_HEIGHT * index;


        HWND textBlock = CreateWindowExW(NULL,
                                         L"STATIC",
                                         process.ProcessName.c_str(),
                                         WS_CHILD | SS_CENTER | SS_NOTIFY,
                                         TEXT_X_POSITION, TEXT_Y_POSITION + (index * 4),
                                         TEXT_WIDTH, TEXT_HEIGHT,
                                         windowHWND,
                                         (HMENU)index + 3,
                                         hInstance,
                                         NULL);

        ProcessesLabels.push_back(textBlock);

        SetClassLongPtrW(textBlock, -12, (LONG_PTR)LoadCursorW(NULL, IDC_HAND));

        ShowWindow(textBlock, SW_SHOW);

        index++;
    };


    ShowWindow(windowHWND, nShowCmd);
    UpdateWindow(windowHWND);

    ShowWindow(button, SW_SHOW);
    ShowWindow(button2, SW_SHOW);


    MSG message;
    while (GetMessageW(&message, NULL, 0, 0) > 0)
    {
        for (const ProcessModel& process : _processList)
        {
            if (process.Creating == true)
            {
                for (const HWND& hwnd : process.handles)
                {
                    ShowWindowAsync(hwnd, SW_HIDE);
                };
            };
        };

        TranslateMessage(&message);
        DispatchMessageW(&message);
    };

    return (int)message.wParam;
};