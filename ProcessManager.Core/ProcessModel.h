#pragma once
#include <string>
#include <processthreadsapi.h>
#include <vector>
#include <msxml.h>

#include "ProcessManager.h"

typedef void(*ProcessClosedCallback)(void);

// Encapsulates a process information
class ProcessModel
{

public:
    // The name (path) of the process
    std::wstring ProcessName;

    // The process arguments
    std::wstring ProcessArgs;

    // A process info struct that will be filled by CreateProcessW function
    PROCESS_INFORMATION ProcessInfo = { 0 };

    // A set of arguments that can be passed to CreateProcessW
    STARTUPINFOW StartupInfo = { 0 };


    HWND MainWindowHandle;

    // Because a single process can have multiple HWND's associated with it stores the process HWND's in a list
    std::vector<HWND> handles;

    // A boolean flag used to indicate if this process is still being created
    bool Creating = false;

    // A WinEventHook function used to get the the process HWND's
    HWINEVENTHOOK Hook;


    bool RunAsConsole;

    // Directory path to run a console script from. Used only in console processes
    std::wstring StartInDirectory;

    // A script to be ran in the console
    std::wstring ConsoleScript;


    ProcessClosedCallback ProcessClosedCallback;


public:

    ProcessModel(std::wstring processName, std::wstring processArgs) :
        ProcessName(processName),
        ProcessArgs(processArgs)
    {
        StartupInfo.cb = sizeof(STARTUPINFOW);
        StartupInfo.dwFlags = STARTF_USESHOWWINDOW;
        StartupInfo.wShowWindow = SW_SHOWMINNOACTIVE;
    };

    ProcessModel(const wchar_t* processName, const wchar_t* processArgs) :
        ProcessModel(std::wstring(processName),
                     std::wstring(processArgs == nullptr ?
                     L"" :
                     processArgs))
    {
    };

    ProcessModel()
    {
    };

public:

    // Retrieve the process PID
    inline DWORD GetPID() const
    {
        return ProcessInfo.dwProcessId;
    };


public:

    // Compares process ID's 
    bool operator == (const ProcessModel& rhs) const
    {
        return this->GetPID() == rhs.GetPID();
    }

};
