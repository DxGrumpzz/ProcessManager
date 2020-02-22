#pragma once
#include <vector>
#include <msxml.h>

#include "ProcessModel.h"


class ProcessManager
{

public:

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

    static BOOL RunProcess(ProcessModel& process)
    {
        NormalizeArgs(process);

        if (!CreateProcessW(process.ProcessName.c_str(), const_cast<wchar_t*>(process.ProcessArgs.c_str()), NULL, NULL, FALSE, CREATE_SUSPENDED | CREATE_NO_WINDOW, NULL, NULL, &process.info, &process.ProcessInfo))
        {
            CloseHandle(process.ProcessInfo.hProcess);
            CloseHandle(process.ProcessInfo.hThread);

            return FALSE;
        };

        process.Hook = SetWinEventHook(EVENT_OBJECT_CREATE, EVENT_OBJECT_CREATE, NULL, WinEventHookCallback, process.ProcessInfo.dwProcessId, 0, WINEVENT_OUTOFCONTEXT);

        if (!ResumeThread(process.ProcessInfo.hThread))
        {
            CloseHandle(process.ProcessInfo.hProcess);
            CloseHandle(process.ProcessInfo.hThread);

            return FALSE;
        };

        return TRUE;
    };


    static BOOL CloseProcess(ProcessModel& process)
    {
        DWORD exitCode;
        GetExitCodeProcess(process.ProcessInfo.hProcess, &exitCode);

        if (exitCode == STILL_ACTIVE)
        {
            TerminateProcess(process.ProcessInfo.hProcess, 0);

            if (!CloseHandle(process.ProcessInfo.hProcess))
                return FALSE;

            if (!CloseHandle(process.ProcessInfo.hThread))
                return FALSE;
        }

        return TRUE;
    }


    static void HideProcess(ProcessModel& process, HWND mainWindow)
    {
        if (process.Creating == true)
        {
            for (const HWND& hwnd : process.handles)
            {
                ShowWindowAsync(hwnd, SW_HIDE);
            };

            process.Creating = false;
        };
    };

    static void CreateProcessModel(std::wstring processName, std::wstring processArgs)
    {
        ProcessManager::ProcessList.emplace_back(processName, processArgs);
    };


private:

    static void NormalizeArgs(ProcessModel& process)
    {
        if (process.ProcessArgs[0] != L' ')
        {
            process.ProcessArgs.insert(0, L" ");
        };
    }

};