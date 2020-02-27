#pragma once
#include <string>
#include <processthreadsapi.h>
#include <vector>
#include <msxml.h>

#include "ProcessManager.h"

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
    STARTUPINFOW info = { 0 };

    // Because a single process can have multiple HWND's associated with it stores the process HWND's in a list
    std::vector<HWND> handles;

    // A boolean flag used to indicate if this process is still being created
    bool Creating = false;

    // A WinEventHook function used to get the the process HWND's
    HWINEVENTHOOK Hook;



public:

    ProcessModel(std::wstring processName, std::wstring processArgs) :
        ProcessName(processName),
        ProcessArgs(processArgs)
    {
        info.cb = sizeof(STARTUPINFOW);
        info.dwFlags = STARTF_USESHOWWINDOW;
        info.wShowWindow = SW_SHOWMINNOACTIVE;
    };

    ProcessModel(const wchar_t* processName, const wchar_t* processArgs) :
        ProcessModel(std::wstring(processName), 
                     std::wstring(processArgs == nullptr ?
                     L"" :
                     processArgs))
    {
    };

};
