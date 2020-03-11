#include <windows.h>
#include <string>
#include <vector>
#include <fstream>
#include <CommCtrl.h>
#include <thread>

#include "ProcessManager.h"
#include "ProcessModel.h"


#define RBG_UNIFORM(uniformColour) RGB(uniformColour, uniformColour, uniformColour)

#define DLL_CALL extern "C" __declspec(dllexport) 


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
LRESULT CALLBACK WindowProcedure(HWND hwnd, UINT message, WPARAM wParam, LPARAM lParam)
{
            return DefWindowProcW(hwnd, message, wParam, lParam);
};


// Create the app's main window
HWND CreateMainWindow(const HINSTANCE& hInstance)
{
    WNDCLASSEXW windowClass = { 0 };
    windowClass.cbSize = sizeof(WNDCLASSEXW);
    windowClass.hInstance = hInstance;
    windowClass.lpszClassName = windowClassName;
    windowClass.lpfnWndProc = WindowProcedure;

    // Register the window
    ATOM registerClassResult = RegisterClassExW(&windowClass);

    if (registerClassResult == 0)
    {
        std::wstring error = GetErrorStringW(GetLastError());
        error.insert(0, L"An error occured while creating window.\n");

        MessageBoxW(NULL, error.c_str(), NULL, NULL);

        return NULL;
    };

    //const HWND hDesktop = GetDesktopWindow();
    //RECT desktopRECT;

    //GetWindowRect(hDesktop, &desktopRECT);

    //const int monitorWidth = desktopRECT.right - desktopRECT.left;
    //const int monitorHeight = desktopRECT.bottom - desktopRECT.top;

    //constexpr int WINDOW_WIDTH = 800;
    //constexpr int WINDOW_HEIGHT = 350;

    // Create the actuall window
    HWND windowHWND = CreateWindowExW(NULL,
                                      windowClassName,
                                      L" ",
                                      NULL,//WS_CAPTION | WS_BORDER | WS_SYSMENU | WS_MAXIMIZEBOX | WS_MINIMIZEBOX | WS_SIZEBOX,
                                      0, 0,//(monitorWidth / 2) - (WINDOW_WIDTH / 2), (monitorHeight / 2) - (WINDOW_HEIGHT / 2),
                                      0, 0,//800, 350,
                                      NULL,
                                      NULL,
                                      hInstance,
                                      NULL);


    return windowHWND;
};


DLL_CALL void Initialize()
{
    std::thread([]()
    {
        HINSTANCE hInstance = GetModuleHandleW(NULL);


        HWND windowHWND = CreateMainWindow(hInstance);

        // Handle window creation errors
        if (windowHWND == 0)
        {
            std::wstring error = GetErrorStringW(GetLastError());
            error.insert(0, L"An error occured while creating window.\n");

            MessageBoxW(NULL, error.c_str(), NULL, NULL);
            return;
        };


        // Show the main window
        ShowWindow(windowHWND, SW_HIDE);

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

            //// Hide the processes. 
            //// Why is this here ? read ProcessManager::RunProcess doc
            //for (ProcessModel& process : ProcessManager::ProcessList)
            //{
            //    if (process.HideProcess == true)
            //    {
            //        // Go through every handle, and run ShowWindow with SW_HIDE
            //        for (const HWND& hwnd : process.handles)
            //        {
            //            ShowWindowAsync(hwnd, SW_HIDE);
            //        };

            //        process.IsProcessShown = false;
            //    }
            //    else
            //    {
            //        if (process.IsProcessShown == false)
            //        {
            //            // Go through every handle, and run ShowWindow with SW_HIDE
            //            for (const HWND& hwnd : process.handles)
            //            {
            //                ShowWindowAsync(hwnd, SW_SHOW);
            //            };

            //            process.IsProcessShown = true;
            //        };
            //    };
            //    //ProcessManager::HideProcess(process);
            //};

            // Because this is an infinite loop a 1ms thread delay is a must
            Sleep(1);
        };
    })
    .detach();
};


DLL_CALL unsigned long RunProcess(const wchar_t* processName, const wchar_t* processArgs)
{
    DWORD processID = ProcessManager::RunProcess(processName, processArgs);

    return processID;
};

DLL_CALL void CloseProcess(DWORD processID)
{
    for (ProcessModel& process : ProcessManager::ProcessList)
    {
        if (process.ProcessInfo.dwProcessId == processID)
        {
            ProcessManager::CloseProcess(process);
        };
    };
};
