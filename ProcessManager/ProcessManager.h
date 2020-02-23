#pragma once
#include <vector>
#include <msxml.h>
#include <fstream>

#include "ProcessModel.h"

// A class that is responsible for interaction with the ProcessModel
class ProcessManager
{

public:

    // The list of processes that will be ran
    static std::vector<ProcessModel> ProcessList;


private:

    // A WinEventHook function used to get a process HWND's
    static void CALLBACK WinEventHookCallback(HWINEVENTHOOK hWinEventHook, DWORD event, HWND hwnd, LONG idObject, LONG idChild, DWORD idEventThread, DWORD dwmsEventTime)
    {
        // Get the current process ID
        DWORD processId;
        GetWindowThreadProcessId(hwnd, &processId);

        for (ProcessModel& process : ProcessList)
        {
            // Match the ID with the created process
            if (process.ProcessInfo.dwProcessId == processId)
            {
                // Add the process HWND to the process handles list
                process.handles.push_back(hwnd);

                // Set creating flag
                process.Creating = true;
            };
        };
    };



public:

    // Runs a single process
    static BOOL RunProcess(ProcessModel& process)
    {
        // Fix process arguments string if needed
        NormalizeArgs(process);

        // Create/run the process
        if (!CreateProcessW(process.ProcessName.c_str(),
            // Because the argument string will be appended to the default path argument 
            // a const cast must be used to convert the LPCWSTR to a LPWSTR
            const_cast<wchar_t*>(process.ProcessArgs.c_str()), 
            NULL,NULL, 
            FALSE,
            // Create a suspended process, A process that will start paused until ResumeThread is called
            // Run the process without a window, Only works on Console app for some reason 
            CREATE_SUSPENDED | CREATE_NO_WINDOW, 
            NULL,
            NULL, 
            &process.info, 
            &process.ProcessInfo))
        {
            // If process creation failed.

            // Close the process handles
            CleanupProcessHandles(process);

            return FALSE;
        };
        
        // Setup the hook function so it will only call the function when the process has finished initialization
        process.Hook = SetWinEventHook(EVENT_OBJECT_CREATE, EVENT_OBJECT_CREATE, NULL, WinEventHookCallback, process.ProcessInfo.dwProcessId, 0, WINEVENT_OUTOFCONTEXT);

        // After hooking the process resume normal execution
        if (!ResumeThread(process.ProcessInfo.hThread))
        {
            CleanupProcessHandles(process);

            return FALSE;
        };

        return TRUE;
    };

    // Closes a single process
    static BOOL CloseProcess(ProcessModel& process)
    {
        // Get process exit code
        DWORD exitCode;
        GetExitCodeProcess(process.ProcessInfo.hProcess, &exitCode);

        // If the process is running
        if (exitCode == STILL_ACTIVE)
        {
            // Unhook the WinEvent from the process
            UnhookWinEvent(process.Hook);
           
            // Close the process
            if (!TerminateProcess(process.ProcessInfo.hProcess, 0))
                return FALSE;

            if(!CleanupProcessHandles(process));
                return FALSE;
        }
        // Don't do anything if the process is closed
        else
        {
            return FALSE;
        };

        return TRUE;
    }


    // Creates and runs every process in the list
    static void RunEveryProcess()
    {
        for (ProcessModel& process : ProcessManager::ProcessList)
        {
            ProcessManager::RunProcess(process);
        };
    };

    // Closes every process in the list
    static void CloseEveryProcess()
    {
        for (ProcessModel& process : ProcessManager::ProcessList)
        {
            ProcessManager::CloseProcess(process);
        };
    };


    // Hides a process (Only the visible window, nothing sketchy)
    static void HideProcess(ProcessModel& process)
    {
        // If the process is in "creation" mode
        if (process.Creating == true)
        {
            // Go through every handle, and run ShowWindow with SW_HIDE
            for (const HWND& hwnd : process.handles)
            {
                ShowWindow(hwnd, SW_HIDE);
            };

            // Set creating flag
            process.Creating = false;
        };
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

    // Normalizes the argument string, Just in case
    static void NormalizeArgs(ProcessModel& process)
    {
        // In windows to pass an agument string to a process 
        // The string MUST contain a space in the beggining.
        // This is because by default windows passes a path argument to every running process

        if (process.ProcessArgs[0] != L' ')
        {
            process.ProcessArgs.insert(0, L" ");
        };
    }

    // Closes an opened process' handles
    static BOOL CleanupProcessHandles(ProcessModel& process)
    {
        if (!CloseHandle(process.ProcessInfo.hProcess) &&
            !CloseHandle(process.ProcessInfo.hThread))
            return FALSE;
    }

};