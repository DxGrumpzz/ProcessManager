#pragma once
#include <string>
#include <processthreadsapi.h>
#include <vector>
#include <msxml.h>
#include "ProcessManager.h"

class ProcessModel
{

public:
    std::wstring ProcessName;
    std::wstring ProcessArgs;

    PROCESS_INFORMATION ProcessInfo = { 0 };
    STARTUPINFOW info = { 0 };

    std::vector<HWND> handles;

    bool Creating = false;

    HWINEVENTHOOK Hook;
    


public:

    ProcessModel(std::wstring processName, std::wstring processArgs) :
        ProcessName(processName),
        ProcessArgs(processArgs)
    {
        info.cb = sizeof(STARTUPINFOW);
        info.dwFlags = STARTF_USESHOWWINDOW;
        info.wShowWindow = SW_HIDE;
    };


};
