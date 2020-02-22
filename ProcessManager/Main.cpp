#include <windows.h>
#include <string>
#include <vector>
#include <fstream>
#include <CommCtrl.h>

#include "ProcessManager.h"


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


const wchar_t* windowTitle = L"Process manager";
const wchar_t* windowClassName = L"DesktopApp";

HINSTANCE hInstance;

HWND button;
HWND button2;

std::vector<HWND> _processesLabels;





LRESULT CALLBACK WindowProcedure(HWND hWnd, UINT message, WPARAM wParam, LPARAM lParam)
{
    switch (message)
    {
        case WM_CLOSE:
        {
            PostQuitMessage(0);
            for (ProcessModel& process : ProcessManager::ProcessList)
            {
                ProcessManager::CloseProcess(process);
            };
            return 0;
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
                        for (ProcessModel& process : ProcessManager::ProcessList)
                        {
                            ProcessManager::RunProcess(process);
                        };

                        return TRUE;
                    };

                    // Abort process button
                    case 2:
                    {
                        for (ProcessModel& process : ProcessManager::ProcessList)
                        {
                            ProcessManager::CloseProcess(process);
                        };

                        return TRUE;
                    };


                    default:
                        return DefWindowProcW(hWnd, message, wParam, lParam);
                };
            };

            return 0;
        };


        case WM_SIZE:
        {
            const int newWidth = LOWORD(lParam);

            int counter = 0;
            for (const HWND& _hwnd : _processesLabels)
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

            return 1;
        }


        default:
            return DefWindowProcW(hWnd, message, wParam, lParam);
    };
};


HWND CreateMainWindow(const HINSTANCE& hInstance)
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

        return NULL;
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



    return windowHWND;
}

void CreateButtons(const HWND& windowHWND, const HINSTANCE& hInstance)
{
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

    // Set button cursor
    SetClassLongPtrW(button, GCLP_HCURSOR, (LONG_PTR)LoadCursorW(NULL, IDC_HAND));

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


    ShowWindow(button, SW_SHOW);
    ShowWindow(button2, SW_SHOW);
}

void CreateProcessLabels(const HWND& windowHWND, const HINSTANCE& hInstance)
{
    int longestProcessName = 0;

    for (const ProcessModel& process : ProcessManager::ProcessList)
    {
        if (process.ProcessName.size() > longestProcessName)
            longestProcessName = process.ProcessName.size();
    };

    const int  CHAR_MULTIPLIER = 8;

    const int TEXT_WIDTH = longestProcessName * CHAR_MULTIPLIER;


    int index = 0;
    for (const ProcessModel& process : ProcessManager::ProcessList)
    {
        int TEXT_HEIGHT = (int)(std::count(process.ProcessName.begin(), process.ProcessName.end(), L'\n') + 1) * 20;

        const int TEXT_X_POSITION = abs(500 - TEXT_WIDTH) - 15;
        const int TEXT_Y_POSITION = TEXT_HEIGHT * index;
        const HMENU id = (HMENU)(index + 3);

        HWND textBlock = CreateWindowExW(NULL,
                                         L"STATIC",
                                         process.ProcessName.c_str(),
                                         WS_CHILD | SS_CENTER | SS_NOTIFY,
                                         TEXT_X_POSITION, TEXT_Y_POSITION + (index * 4),
                                         TEXT_WIDTH, TEXT_HEIGHT,
                                         windowHWND,
                                         id,
                                         hInstance,
                                         NULL);


        _processesLabels.push_back(textBlock);

        SetClassLongPtrW(textBlock, -12, (LONG_PTR)LoadCursorW(NULL, IDC_HAND));

        ShowWindow(textBlock, SW_SHOW);

        index++;
    };
}



// _In_opt_ nad _In_ are something called SAL annotations, They mean that a parameter maybe be passed as NULL
int WINAPI wWinMain(_In_ HINSTANCE hInstance, _In_opt_ HINSTANCE hPrevInstance, _In_ LPWSTR lpCmdLine, _In_ int nShowCmd)
{

    HWND windowHWND = CreateMainWindow(hInstance);

    if (windowHWND == 0)
    {
        std::wstring error = GetErrorStringW(GetLastError());
        error.insert(0, L"An error occured while creating window.\n");

        MessageBoxW(NULL, error.c_str(), NULL, NULL);

        return 1;
    };


    CreateButtons(windowHWND, hInstance);

    ProcessManager::GetProcessListFromFile();

    CreateProcessLabels(windowHWND, hInstance);


    ShowWindow(windowHWND, nShowCmd);
    UpdateWindow(windowHWND);

    MSG message;

    while (1)
    {
        while (PeekMessageW(&message, NULL, 0, 0, PM_REMOVE))
        {
            TranslateMessage(&message);
            DispatchMessageW(&message);
        };

        if (message.message == WM_QUIT)
            break;

        for (ProcessModel& process : ProcessManager::ProcessList)
        {
            ProcessManager::HideProcess(process, windowHWND);
        };

        if (message.message == WM_QUIT)
            break;

        Sleep(1);
    };


    return (int)message.wParam;
};