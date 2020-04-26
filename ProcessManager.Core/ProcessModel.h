#pragma once
#include <windows.h>
#include <string>
#include <chrono>
#include <mutex>

typedef void(*ProcessClosedCallBack)(class ProcessModel*);


// A class that represents a process that can be interacted with
class ProcessModel
{

private:

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

    // A handle used in RegisterWaitForSingleObject function 
    HANDLE _registerCallbackHandle;


public:

    // A struct that contains information about this process
    PROCESS_INFORMATION ProcessInfo;

    // A struct that contains info about a process' startup
    STARTUPINFOW StartupInfo;

    // A handle to the process' main window
    HWND MainWindowHandle;

    // The process path 
    std::wstring ProcessPath;

    // The process' argument "list"
    std::wstring ProcessArgs;


    // A boolean flag that indicates if this process is to run as a console script
    bool RunAsConsole;


    // The console script to run
    std::wstring ConsoleScript;

    // A directory to run the console script from
    std::wstring StartupDirectory;


    // A callback function that will be called when this process terminates
    ProcessClosedCallBack ProcessClosedCallback;


public:

    ProcessModel() :
        ProcessInfo({ 0 }),
        StartupInfo({ 0 }),

        MainWindowHandle(NULL),

        ProcessPath(L""),
        ProcessArgs(L""),

        RunAsConsole(false),

        ConsoleScript(L""),
        StartupDirectory(L""),

        ProcessClosedCallback(NULL),
        _registerCallbackHandle(NULL)

    {
        StartupInfo.cb = sizeof(StartupInfo);
    };


public:

    // Run the process and return boolean result
    bool RunProcess()
    {
        // In windows, process initialization is different depending if the "process" is a cmd script or a GUI process
        // So we run different functions depending on the "type" of process

        if (RunAsConsole == true)
            return RunProcessAsConsole();
        else
            return RunProcessAsGUI();
    };

    // Closes the process
    bool CloseProcess()
    {
        // Send a close message to the process window and wait for response
        SendMessageW(MainWindowHandle, WM_CLOSE, NULL, NULL);

        // Close process handles
        CleanProcessHandles();

        return true;
    };


    void ForceCloseProcess()
    {
        TerminateProcess(ProcessInfo.hThread, 0);

        CleanProcessHandles();
    };


    // Check if the process is currently running
    bool IsRunning()
    {
        // Get process exit code
        DWORD exitCode = 0;
        GetExitCodeProcess(ProcessInfo.hProcess, &exitCode);

        // If code equal to STILL_ACTIVE
        if (exitCode == STILL_ACTIVE)
            // The process is still active
            return true;
        else
            return false;
    };


    // Shows this process' main window
    bool ShowProcessWindow()
    {
        return ShowWindow(MainWindowHandle, SW_SHOW);
    };

    // Hides this process' main window
    bool HideProcessWindow()
    {
        return ShowWindow(MainWindowHandle, SW_HIDE);
    };



private:

    // Runs this process as a GUI app
    bool RunProcessAsGUI()
    {
        // In windows, to correctly pass a set of arguments to a process
        // A space is required in the beggining of the arguments string.
        // This "normalizes" the argument string if needed.

        // Get a normalized string if neccesarry
        std::wstring normalizedArgs;
        NormalizedArgs(normalizedArgs);

        // Run the process
        if (!CreateProcessW(ProcessPath.c_str(),
            const_cast<wchar_t*>(normalizedArgs.c_str()),
            NULL, NULL,
            FALSE,
            // Because we register a callback function when the process closes we start the process in a suspended state
            CREATE_SUSPENDED,
            NULL,
            NULL,
            &StartupInfo,
            &ProcessInfo))
        {
            CleanProcessHandles();
            return false;
        };

        // Register the callback for the created process
        RegisterClosedCallback();

        // Allow process to initialize
        ResumeProcess();

        // Find the process main WindowHandle
        MainWindowHandle = GetProcessHWND();

        return true;
    };

    // Runs this process from the windows CMD
    bool RunProcessAsConsole()
    {
        // To keep the process running after creation we append a /C switch in the beggining of the console script
        std::wstring scriptAsWString(ConsoleScript);
        scriptAsWString.insert(0, L"/C ");

        // Convert the std::wstring to a wchar_t* so we won't lose the new string when we exit scope
        wchar_t* cmdlines = const_cast<wchar_t*>(scriptAsWString.c_str());

        // Create the process
        if (!CreateProcessW(
            // Run a windows cmd
            L"C:\\WINDOWS\\system32\\cmd.exe",
            // Give the cmd the script to run
            cmdlines,
            NULL, NULL,
            FALSE,
            CREATE_SUSPENDED,
            NULL,
            // Start the cmd from this directory
            StartupDirectory.c_str(),
            &StartupInfo,
            &ProcessInfo))
        {
            CleanProcessHandles();
            return false;
        };

        // Register a process closed callback
        RegisterClosedCallback();

        // Resume process initialization
        ResumeProcess();

        // Find main window HWND
        MainWindowHandle = GetProcessHWND();

        return true;
    };


    // Register a callback when the process closes
    bool RegisterClosedCallback()
    {
        return RegisterWaitForSingleObject(&_registerCallbackHandle, ProcessInfo.hProcess, ProcessHandleClosed, this, INFINITE, WT_EXECUTEONLYONCE);
    };

    // Resumes process thread execution after creation with CREATE_SUSPENDED flag
    bool ResumeProcess()
    {
        return ResumeThread(ProcessInfo.hThread);
    };

    // Cleans the process hThread, and hProcess hanldes
    void CleanProcessHandles()
    {
        CloseHandle(ProcessInfo.hProcess);
        CloseHandle(ProcessInfo.hThread);
    };

    // Get a normalized string to send to the process args
    bool NormalizedArgs(std::wstring& normalizedArgs)
    {
        if (ProcessArgs[0] != L' ')
        {
            normalizedArgs = std::wstring(ProcessArgs);

            normalizedArgs.insert(0, L" ");

            return true;
        };

        return false;
    };


    // Returns a process' MainWindow handle
    HWND GetProcessHWND(int msTimeout = 3000)
    {
        // Create a WndEnumProcParam struct to hold the data
        EnumProcParam wndEnumProcParam;
        wndEnumProcParam.ProcessID = ProcessInfo.dwProcessId;
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


    // A function that will be called when the process closes
    static void __stdcall ProcessHandleClosed(PVOID lpParameter, BOOLEAN timerOrWaitFired)
    {
        // Get closing process
        ProcessModel* process = reinterpret_cast<ProcessModel*>(lpParameter);

        // Unregister this callback function
        UnregisterWait(process->_registerCallbackHandle);

        // Call process closed function
        process->ProcessClosedCallback(process);

    };

};