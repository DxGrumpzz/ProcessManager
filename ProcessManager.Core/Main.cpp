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

    // Show the main window
    ShowWindow(windowHWND, nShowCmd);

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