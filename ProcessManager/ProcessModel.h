#pragma once
#include <string>
#include <processthreadsapi.h>
#include <vector>
#include <msxml.h>


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


    static std::vector<ProcessModel> ProcessList;


private:


    static void CALLBACK WinEventHookCallback(HWINEVENTHOOK hWinEventHook, DWORD event, HWND hwnd, LONG idObject, LONG idChild, DWORD idEventThread, DWORD dwmsEventTime)
    {
        DWORD processId;
        GetWindowThreadProcessId(hwnd, &processId);

        for (ProcessModel& process : ProcessList)
        {
            if (process.ProcessInfo.dwProcessId == processId)
            {
                process.Creating = true;
                process.handles.push_back(hwnd);
            };
        };
    };

public:

    ProcessModel(std::wstring processName, std::wstring processArgs) :
        ProcessName(processName),
        ProcessArgs(processArgs)
    {
        info.cb = sizeof(STARTUPINFOW);
        info.dwFlags = STARTF_USESHOWWINDOW;
        info.wShowWindow = SW_HIDE;
    };


public:

    BOOL RunProcess()
    {
        NormalizeArgs();

        if (!CreateProcessW(ProcessName.c_str(), const_cast<wchar_t*>(ProcessArgs.c_str()), NULL, NULL, FALSE, CREATE_SUSPENDED | CREATE_NO_WINDOW, NULL, NULL, &info, &ProcessInfo))
        {
            CloseHandle(ProcessInfo.hProcess);
            CloseHandle(ProcessInfo.hThread);

            return FALSE;
        };

        Hook = SetWinEventHook(EVENT_OBJECT_CREATE, EVENT_OBJECT_CREATE, NULL, WinEventHookCallback, ProcessInfo.dwProcessId, 0, WINEVENT_OUTOFCONTEXT);

        if (!ResumeThread(ProcessInfo.hThread))
        {
            CloseHandle(ProcessInfo.hProcess);
            CloseHandle(ProcessInfo.hThread);

            return FALSE;
        };

        return TRUE;
    };

    BOOL CloseProcess()
    {
        DWORD exitCode;
        GetExitCodeProcess(ProcessInfo.hProcess, &exitCode);

        if (exitCode == STILL_ACTIVE)
        {
            TerminateProcess(ProcessInfo.hProcess, 0);

            if (!CloseHandle(ProcessInfo.hProcess))
                return FALSE;

            if (!CloseHandle(ProcessInfo.hThread))
                return FALSE;
        }

        return TRUE;
    }

private:

    void NormalizeArgs()
    {
        if (ProcessArgs[0] != L' ')
        {
            ProcessArgs.insert(0, L" ");
        };
    }

};
