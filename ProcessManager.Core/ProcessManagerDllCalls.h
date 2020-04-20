#pragma once
#include "ProcessManager.h"
#include "ProcessModel.h"

#define DLL_CALL extern "C" __declspec(dllexport) 


DLL_CALL DWORD RunProcess(const wchar_t* processName, const wchar_t* processArgs, ProcessClosedCallback processClosedCallback, bool visibleOnStartup)
{
    DWORD processID = ProcessManager::RunProcess(processName, processArgs, processClosedCallback, visibleOnStartup);

    return processID;
};


DLL_CALL DWORD RunConsoleProcess(const wchar_t* processName, const wchar_t* processArgs, ProcessClosedCallback processClosedCallback, bool visibleOnStartup)
{
    return ProcessManager::RunConsoleProcess(processName, processArgs, processClosedCallback, visibleOnStartup);
}


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


DLL_CALL bool IsProcessRunning(DWORD processID)
{
    return ProcessManager::IsProcessRunning(processID);
}





// A struct that stores information about a process in-between WNDENUMPROC calls
struct EnumProcParam
{
    // The process' PID
    DWORD ProcessID = NULL;

    // An "out" variable that will contain the process' main window HWND
    HWND HwndOut = NULL;

    // A timing varialbe used to keep track of when the process HWND search has started
    std::chrono::steady_clock::time_point StartTime = std::chrono::steady_clock::now();

    // How long to keep searching for 
    int TimeoutMS = 0;

    // A boolean flag that indicates if the search has timed out
    bool TimedOut = false;
};

// Returns a process' MainWindow handle
static HWND GetProcessHWND(DWORD processID, int msTimeout = 3000)
{
    // Create a WndEnumProcParam struct to hold the data
    EnumProcParam wndEnumProcParam;
    wndEnumProcParam.ProcessID = processID;
    wndEnumProcParam.HwndOut = NULL;
    wndEnumProcParam.StartTime = std::chrono::steady_clock::now();
    wndEnumProcParam.TimeoutMS = msTimeout;


    // Continue iteration while the out HWND variable is null
    while (wndEnumProcParam.HwndOut == NULL)
    {
        // This function iterates through every top-level window,
        EnumWindows([](HWND handle, LPARAM lParam) -> BOOL
        {
            // Only if the current window is visible to the user
            if (IsWindowVisible(handle) == TRUE)
            {
                // Convert the LPARAM to WndEnumProcParam
                EnumProcParam& enumProcParam = *reinterpret_cast<EnumProcParam*>(lParam);

                // Get the current time 
                std::chrono::steady_clock::time_point currentTime = std::chrono::steady_clock::now();

                // Get elapsed time 
                auto elapsedTimeInMS = std::chrono::duration_cast<std::chrono::milliseconds>(currentTime - enumProcParam.StartTime).count();

                // Check if we didn't timeout
                if (elapsedTimeInMS >= enumProcParam.TimeoutMS)
                {
                    enumProcParam.TimedOut = true;
                    return FALSE;
                };

                // Get the process PID
                DWORD currentProcess = 0;
                GetWindowThreadProcessId(handle, &currentProcess);

                // Compare the id's, 
                // if they match
                if (enumProcParam.ProcessID == currentProcess)
                {
                    // Set the HWND out variable 
                    enumProcParam.HwndOut = handle;

                    // Return false(0) to stop the window iteration 
                    return FALSE;
                };
            };

            return TRUE;
        }, reinterpret_cast<LPARAM>(&wndEnumProcParam));

        if (wndEnumProcParam.TimedOut == true)
        {
            return NULL;
        };
    };

    return wndEnumProcParam.HwndOut;
}




// Takes a DWORD error code and returns its string message 
std::string GetLastErrorAsString()
{
    // Stores the error message as a string in memory
    LPSTR buffer = nullptr;

    // Format DWORD error ID to a string 
    FormatMessageA(FORMAT_MESSAGE_ALLOCATE_BUFFER | FORMAT_MESSAGE_FROM_SYSTEM | FORMAT_MESSAGE_IGNORE_INSERTS,
                   NULL,
                   GetLastError(),
                   MAKELANGID(LANG_NEUTRAL, SUBLANG_DEFAULT),
                   (LPSTR)&buffer, 0, NULL);

    // Create std string from buffer
    std::string message(buffer);

    return message;
}

DLL_CALL void Test()
{
    STARTUPINFOW startupInfo = { 0 };
    startupInfo.cb = sizeof(startupInfo);

    PROCESS_INFORMATION processInfo = { 0 };

    wchar_t args[] = L"/C npm run start";

    if (!CreateProcessW(L"C:\\WINDOWS\\system32\\cmd.exe",
        args,
        NULL, NULL,
        FALSE,
        NULL,
        NULL,
        L"C:\\Development\\Npm Test\\client",
        &startupInfo,
        &processInfo))
    {
        auto s = GetLastErrorAsString();
        int ss = 9;
    };

    HWND hwnd = GetProcessHWND(processInfo.dwProcessId, 3000);

    //ShowWindow(hwnd, SW_HIDE);

    CloseHandle(processInfo.hProcess);
    CloseHandle(processInfo.hThread);

};
