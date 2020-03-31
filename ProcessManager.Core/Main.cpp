#include "ProcessManager.h"
#include "ProcessModel.h"



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



DLL_CALL unsigned long RunProcess(const wchar_t* processName, const wchar_t* processArgs, ProcessClosedCallback processClosedCallback, bool visibleOnStartup)
{
    DWORD processID = ProcessManager::RunProcess(processName, processArgs, processClosedCallback, visibleOnStartup);

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


DLL_CALL void CloseProcessTree(DWORD processID)
{
    for (ProcessModel& process : ProcessManager::ProcessList)
    {
        if (process.ProcessInfo.dwProcessId == processID)
        {
            ProcessManager::CloseProcessTree(process);
        };
    };
};


DLL_CALL bool ShowProcess(DWORD processID)
{
    ProcessModel* process = ProcessManager::GetProcess(processID);

    if (process == nullptr)
        return false;

    ShowWindow(process->MainWindowHandle, SW_SHOW);

    return true;
};



DLL_CALL bool HideProcess(DWORD processID)
{
    ProcessModel* process = ProcessManager::GetProcess(processID);

    if (process == nullptr)
        return false;

    ShowWindow(process->MainWindowHandle, SW_HIDE);

    return true;
};



DLL_CALL DWORD RunConsoleProcess(const wchar_t* processName, const wchar_t* processArgs, ProcessClosedCallback processClosedCallback, bool visibleOnStartup)
{
    return ProcessManager::RunConsoleProcess(processName, processArgs, processClosedCallback, visibleOnStartup);
}


DLL_CALL BOOL IsProcessRunning(DWORD processID)
{
    return ProcessManager::IsProcessRunning(processID);
}

/*
#define RBG_UNIFORM(uniformColour) RGB(uniformColour, uniformColour, uniformColour)


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



//
//struct ProcessData
//{
//    const wchar_t* Path;
//    const wchar_t* Args;
//
//    HWND MainWindowHandle;
//    HWND* Handles;
//
//    PROCESS_INFORMATION ProcessInfo;
//};
//
//
//
//// A struct that stores information about a process in-between WNDENUMPROC calls
//struct WndEnumProcParam
//{
//    // The process' PID
//    DWORD ProcessID;
//
//    // An "out" variable that will contain the process' main window HWND
//    HWND HwndOut;
//};
//
//// Returns a process' MainWindow handle
//HWND GetProcessHWND(DWORD processID)
//{
//    // Create a WndEnumProcParam struct to hold the data
//    WndEnumProcParam wndEnumProcParam;
//    wndEnumProcParam.ProcessID = processID;
//    wndEnumProcParam.HwndOut = NULL;
//
//    // Continue iteration while the out HWND variable is null
//    while (wndEnumProcParam.HwndOut == NULL)
//    {
//        // This function iterates through every top-level window,
//        EnumWindows(
//            [](HWND handle, LPARAM lParam) -> BOOL
//        {
//            // Only if the current window is visible to the user
//            if (IsWindowVisible(handle) == TRUE)
//            {
//                // Convert the LPARAM to WndEnumProcParam
//                WndEnumProcParam& param_data = *reinterpret_cast<WndEnumProcParam*>(lParam);
//
//                // Get the process PID
//                DWORD currentProcess = 0;
//                GetWindowThreadProcessId(handle, &currentProcess);
//
//                // Compare the id's, 
//                // if they match
//                if (param_data.ProcessID == currentProcess)
//                {
//                    // Set the HWND out variable 
//                    param_data.HwndOut = handle;
//
//                    // Return false(0) to stop the window iteration 
//                    return FALSE;
//                };
//            };
//
//            return TRUE;
//        }, reinterpret_cast<LPARAM>(&wndEnumProcParam));
//    };
//
//    return wndEnumProcParam.HwndOut;
//}
//
//std::vector<HWND> GetProcessHWNDs(DWORD processID)
//{
//    std::pair<std::vector<HWND>, DWORD> param = std::make_pair(std::vector<HWND>(), processID);
//
//    EnumWindows([](HWND handle, LPARAM lParam) -> BOOL
//    {
//        std::pair<std::vector<HWND>, DWORD>& param = *reinterpret_cast<std::pair<std::vector<HWND>, DWORD>*>(lParam);
//
//        DWORD currentProcess = 0;
//        GetWindowThreadProcessId(handle, &currentProcess);
//
//        if (param.second == currentProcess)
//        {
//            param.first.push_back(handle);
//        };
//
//        return TRUE;
//
//    }, reinterpret_cast<LPARAM>(&param));
//
//    return param.first;
//}
//
//
//
//DLL_CALL bool Test_RunProcess(ProcessData& processData)
//{
//    STARTUPINFOW startupInfo = { 0 };
//    std::wstring error;
//
//    if (!CreateProcessW(processData.Path,
//        const_cast<wchar_t*>(processData.Args),
//        NULL, NULL,
//        FALSE,
//        NULL,
//        NULL,
//        NULL,
//        &startupInfo,
//        &processData.ProcessInfo))
//    {
//        error = GetErrorStringW(GetLastError());
//
//        CloseHandle(processData.ProcessInfo.hProcess);
//        CloseHandle(processData.ProcessInfo.hThread);
//
//        return false;
//    };
//
//    processData.MainWindowHandle = GetProcessHWND(processData.ProcessInfo.dwProcessId);
//
//    return true;
//};
//
//
//DLL_CALL int Test_GetProcessHandlesCount(DWORD processID)
//{
//    auto processHandles = GetProcessHWNDs(processID);
//
//    return processHandles.size();
//};
//
//
//
//DLL_CALL void Test_GetProcessHandles(DWORD processID, HWND* handles, int handleCount)
//{
//    auto processHandles = GetProcessHWNDs(processID);
//
//    for (int a = 0; a < handleCount; a++)
//    {
//        handles[a] = processHandles[a];
//    };
//
//};
//
//
//DLL_CALL void Test_CloseProcess(DWORD processID)
//{
//    HANDLE processHandle = OpenProcess(PROCESS_ALL_ACCESS, FALSE, processID);
//
//    TerminateProcess(processHandle, 0);
//
//    CloseHandle(processHandle);
//};
//

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
*/