#pragma once
#include <windows.h>
#include <string>
#include <chrono>
#include <mutex>
#include <filesystem>

#include "WindowsHelpers.h"

typedef void(*ProcessClosedCallBack)(class ProcessModel*);


// A class that represents a process that can be interacted with
class ProcessModel
{

private:


    // A handle used in RegisterWaitForSingleObject function 
    HANDLE _registerCallbackHandle;


    // A boolean flag that indicates that this process is initializing
    bool _isInitializing = false;

    // A boolean flag that indicates that this process is closing
    bool _isClosing = false;

    // A mutex that is shared between the close and run calls
    std::mutex _processInitializationCloseMutex;

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

    // A boolean flag that indicates if this process will be visible when it's initialized
    bool VisibleOnStartup;


    // The console script to run
    std::wstring ConsoleScript;

    // A directory to run the console script from
    std::wstring StartupDirectory;


    // A callback function that will be called when this process terminates
    ProcessClosedCallBack ProcessClosedCallback;

    // A callback function that will be called when his process initialized
    ProcessClosedCallBack ProcessInitializedCallback;


public:

    ProcessModel() :
        _registerCallbackHandle(NULL),

        ProcessInfo({ 0 }),
        StartupInfo({ 0 }),

        MainWindowHandle(NULL),

        ProcessPath(L""),
        ProcessArgs(L""),

        RunAsConsole(false),

        VisibleOnStartup(true),

        ConsoleScript(L""),
        StartupDirectory(L""),

        ProcessClosedCallback(NULL),
        ProcessInitializedCallback(NULL)
    {
        StartupInfo.cb = sizeof(StartupInfo);
    };


public:


    // Run the process and return boolean result
    bool RunProcess()
    {
        std::lock_guard< std::mutex>lock(_processInitializationCloseMutex);

        if (_isInitializing == true || _isClosing == true)
            return false;

        _isInitializing = true;

        // In Windows, process initialization is different depending if the "process" is a cmd script or a GUI process
        // So we run different functions depending on the "type" of process

        bool result = false;

        // Try to run the process
        if (RunAsConsole == true)
            result = RunProcessAsConsole();
        else
            result = RunProcessAsGUI();

        // If process initialized succesfully invoke the initialzied event
        if (result == true)
        {
            // Hide process window if request
            if (VisibleOnStartup == false)
                HideProcessWindow();

            ProcessInitializedCallback(this);
        };

        _isInitializing = false;
        return result;
    };

    // Closes the process
    bool CloseProcess()
    {
        std::lock_guard< std::mutex>lock(_processInitializationCloseMutex);

        if (_isInitializing == true || _isClosing == true)
            return false;


        _isClosing = true;

        if (IsRunning() == true)
        {
            // Send a close message to the process window and wait for response
            SendMessageW(MainWindowHandle, WM_CLOSE, NULL, NULL);

            // Close process handles
            CleanProcessHandles();

            _isClosing = false;
            return true;
        }
        else
        {
            _isClosing = false;
            return false;
        };

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
        // Why am I returning the inverse '!' of ShowWindow function ?
        //https://docs.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-showwindow#return-value
        // "If the window was previously hidden, the return value is zero."
        BOOL result = ShowWindow(MainWindowHandle, SW_SHOW);

        return !result;
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
        MainWindowHandle = GetProcessHWND(ProcessInfo.dwProcessId);

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
            // Create suspended so we can attach a closed callback,
            // Create new console beause I'm using a console logger and I don't want std handles to use the logger window to output their messages
            CREATE_SUSPENDED | CREATE_NEW_CONSOLE,
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
        MainWindowHandle = GetProcessHWND(ProcessInfo.dwProcessId);

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