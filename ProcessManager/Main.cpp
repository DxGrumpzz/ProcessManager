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

// The button that runs the processes
HWND _createProcessesButton;

// The button that closes the processes
HWND _closeProcessesButton;

// A list of labels that contain the names of the processes
std::vector<HWND> _processesLabels;


// The MainWindow function procedure
LRESULT CALLBACK WindowProcedure(HWND hWnd, UINT message, WPARAM wParam, LPARAM lParam)
{
    switch (message)
    {
        case WM_CLOSE:
        {
            // Send a quit message to this window 
            PostQuitMessage(0);

            // Close processes
            ProcessManager::CloseEveryProcess();

            return 0;
        };


        case  WM_COMMAND:
        {
            // If the commnad is a mouse button click
            if (HIWORD(wParam) == BN_CLICKED)
            {
                // Get window ID
                WORD controlID = LOWORD(wParam);

                
                switch (controlID)
                {
                    // Create process button
                    case 1:
                    {
                        // Run the processes
                        ProcessManager::RunEveryProcess();

                        EnableWindow(_createProcessesButton, FALSE);
                        EnableWindow(_closeProcessesButton, TRUE);

                        return TRUE;
                    };

                    // Abort process button
                    case 2:
                    {
                        // Close every process
                        ProcessManager::CloseEveryProcess();

                        EnableWindow(_createProcessesButton, TRUE);
                        EnableWindow(_closeProcessesButton, FALSE);

                        return TRUE;
                    };
                };
            };

            return 0;
        };


        case WM_SIZE:
        {
            // Get the window's new size
            const int newWidth = LOWORD(lParam);

            // Because the labels are vertically stacked, 
            // A counter is needed to calculate the correct Y position relative to the label's height
            int counter = 0;

            for (const HWND& _hwnd : _processesLabels)
            {
                // Get the label's window rect
                RECT windowRect;
                GetWindowRect(_hwnd, &windowRect);

                // Calculate the label's width, height, and positions
                const int windowWidth = windowRect.right - windowRect.left;
                const int windowHeight = windowRect.bottom - windowRect.top;

                const int newX = (newWidth - windowWidth) - 10;
                const int newY = ((windowHeight + 4) * counter) + 10;

                // "Append" the label to the right of the screen
                SetWindowPos(_hwnd, NULL,
                             newX, newY,
                             0, 0,
                             SWP_NOSIZE | SWP_NOZORDER);

                counter++;
            };

            return 0;
        }


        default:
            return DefWindowProcW(hWnd, message, wParam, lParam);
    };
};


// Create the app's main window
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

    // Register the window
    ATOM registerClassResult = RegisterClassExW(&windowClass);

    if (registerClassResult == 0)
    {
        std::wstring error = GetErrorStringW(GetLastError());
        error.insert(0, L"An error occured while creating window.\n");

        MessageBoxW(NULL, error.c_str(), NULL, NULL);

        return NULL;
    };


    // Create the actuall window
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

// Creates the buttons that run and close  the processes
void CreateButtons(const HWND& windowHWND, const HINSTANCE& hInstance)
{
    _createProcessesButton = CreateWindowExW(NULL,
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
    SetClassLongPtrW(_createProcessesButton, GCLP_HCURSOR, (LONG_PTR)LoadCursorW(NULL, IDC_HAND));

    _closeProcessesButton = CreateWindowExW(NULL,
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


    // Show the buttons
    ShowWindow(_createProcessesButton, SW_SHOW);
    ShowWindow(_closeProcessesButton, SW_SHOW);
}

// Create the Process labels 
void CreateProcessLabels(const HWND& windowHWND, const HINSTANCE& hInstance)
{
    int longestProcessName = 0;

    // Find the process with the longest name and set "longestProcessName" to it's string size
    for (const ProcessModel& process : ProcessManager::ProcessList)
    {
        if (process.ProcessName.size() > longestProcessName)
            longestProcessName = (int)process.ProcessName.size();
    };

    // An arbitrary character multipler, used to calculate the label's window width
    constexpr int CHAR_MULTIPLIER = 8;

    // The width of the text
    const int TEXT_WIDTH = longestProcessName * CHAR_MULTIPLIER;

    // Because the labels are vertically stacked, 
    // A counter is needed to calculate the correct Y position relative to the label's height
    int counter = 0;
    
    for (const ProcessModel& process : ProcessManager::ProcessList)
    {
        // Find the window height by checking if there is a newline character and multiple it by some value (20)
        int TEXT_HEIGHT = (int)(std::count(process.ProcessName.begin(), process.ProcessName.end(), L'\n') + 1) * 20;

        // Set an ID for evey label
        const long long id = (long long)counter + 3L;

        HWND textBlock = CreateWindowExW(NULL,
                                         L"STATIC",
                                         process.ProcessName.c_str(),
                                         WS_CHILD | SS_CENTER | SS_NOTIFY,
                                         // When the window is created the OS sends a WM_SIZE message that will be handled,
                                         // So there is no need to set X and Y
                                         0, 0,
                                         TEXT_WIDTH, TEXT_HEIGHT,
                                         windowHWND,
                                         (HMENU)id,
                                         hInstance,
                                         NULL);


        // Add the label to the list
        _processesLabels.push_back(textBlock);

        // Show the label
        ShowWindow(textBlock, SW_SHOW);

        counter++;
    };
}


// _In_opt_ nad _In_ are something called SAL annotations, They mean that a parameter maybe be passed as NULL
int WINAPI wWinMain(_In_ HINSTANCE hInstance, _In_opt_ HINSTANCE hPrevInstance, _In_ LPWSTR lpCmdLine, _In_ int nShowCmd)
{
    // Create the main window
    HWND windowHWND = CreateMainWindow(hInstance);

    // Handle window creation errors
    if (windowHWND == 0)
    {
        std::wstring error = GetErrorStringW(GetLastError());
        error.insert(0, L"An error occured while creating window.\n");

        MessageBoxW(NULL, error.c_str(), NULL, NULL);

        return 1;
    };

    // Create process buttons
    CreateButtons(windowHWND, hInstance);

    // Setup ProcessManager 
    ProcessManager::GetProcessListFromFile();

    // Create the process labels
    CreateProcessLabels(windowHWND, hInstance);

    // Show the main window
    ShowWindow(windowHWND, nShowCmd);
    EnableWindow(_closeProcessesButton, FALSE);


    // Windows message loop
    MSG message;
    
    // Continuously try and get message
    while (1)
    {
        // Peek message returns 1 if there a message is available, 
        // If there are none it will return 0.
        // So we continually loop as long as there are messages in queue
        while (PeekMessageW(&message, NULL, 0, 0, PM_REMOVE))
        {
            // If the message is a keystroke get the key's character value
            TranslateMessage(&message);

            // Send the message to the Window procedure function
            DispatchMessageW(&message);
        };

        // To exit the infinite loop check if the current message was a quit message
        if (message.message == WM_QUIT)
            break;

        // Hide the processes. 
        // Why is this here ? read ProcessManager::RunProcess doc
        for (ProcessModel& process : ProcessManager::ProcessList)
        {
            ProcessManager::HideProcess(process);
        };

        // Because this is an infinite loop a 1ms thread delay is a must
        Sleep(1);
    };


    return (int)message.wParam;
};