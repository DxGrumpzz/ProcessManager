#pragma once
#include <vector>
#include <msxml.h>
#include <fstream>

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
            if(!TerminateProcess(process.ProcessInfo.hProcess, 0))
                return FALSE;

            if (!CloseHandle(process.ProcessInfo.hProcess) &&
                !CloseHandle(process.ProcessInfo.hThread))
                return FALSE;

            UnhookWinEvent(process.Hook);
        }
        else
        {
                return FALSE;
        };

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


    // Returns a list of ProcessModel which contain name and arguments of a process 
    static void GetProcessListFromFile(const wchar_t* filename = L"Processes.txt")
    {
        // The processes file
        std::wifstream file(filename);

        // If file is invalid
        if (!file)
        {
            std::wstring error = L"File error. \nCould not open: ";
            error.append(filename);

            size_t outputSize = error.size() + 1;

            char* outputString = new char[outputSize];

            size_t charsConverted = 0;

            const wchar_t* inputW = error.c_str();

            wcstombs_s(&charsConverted, outputString, outputSize, inputW, error.size());

            throw std::exception(outputString);
            delete[] outputString;
        };


        // This is absolute aids. 
        // This will improve


        // Iterate through the file line by line
        // Store current read line 
        std::wstring line;
        while (std::getline(file, line))
        {
            // If current line is the process name
            if (line == L"[Process]")
            {
                // Read process name into current line
                std::getline(file, line);

                // Add the process to the list
                ProcessManager::ProcessList.emplace_back(line, L"");
            }
            // If current line is the process' arguments
            else if (line == L"[Args]")
            {
                // Read next line
                std::getline(file, line);

                // Because of the way command arguments are interpreted a space must be inserted in the beggining of the string
                line.insert(line.begin(), ' ');

                // Set the process' arguments
                auto process = (ProcessManager::ProcessList.end() - 1);
                process->ProcessArgs = line;
            };
        };
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