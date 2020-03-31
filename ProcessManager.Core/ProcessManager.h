#pragma once
#include <chrono>
#include <vector>
#include <msxml.h>
#include <fstream>
#include <algorithm>
#include <tlhelp32.h>

#include "ProcessModel.h"


// A class that is responsible for interaction with the ProcessModel
class ProcessManager
{

private:

 

public:

    // The list of processes that will be ran
    static std::vector<ProcessModel> ProcessList;


public:

    // A struct that stores information about a process in-between WNDENUMPROC calls
    struct EnumProcParam
    {
        // The process' PID
        DWORD ProcessID;

        // An "out" variable that will contain the process' main window HWND
        HWND HwndOut;

        // A timing varialbe used to keep track of when the process HWND search has started
        std::chrono::steady_clock::time_point StartTime;

        // How long to keep searching for 
        int TimeoutMS;

        // A boolean flag that indicates if the search has timed out
        bool TimedOut;
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

    static std::vector<HWND> GetProcessHWNDs(DWORD processID)
    {
        std::pair<std::vector<HWND>, DWORD> param = std::make_pair(std::vector<HWND>(), processID);

        EnumWindows([](HWND handle, LPARAM lParam) -> BOOL
        {
            std::pair<std::vector<HWND>, DWORD>& param = *reinterpret_cast<std::pair<std::vector<HWND>, DWORD>*>(lParam);

            DWORD currentProcess = 0;
            GetWindowThreadProcessId(handle, &currentProcess);

            if (param.second == currentProcess)
            {
                param.first.push_back(handle);
            };

            return TRUE;

        }, reinterpret_cast<LPARAM>(&param));

        return param.first;
    }


    // A callback function that is invoke when a process is terminated
    static void CALLBACK ProcessTerminatedCallback(PVOID lpParameter, BOOLEAN TimerOrWaitFired)
    {
        // Convert lpParam to the process ID
        DWORD processID = reinterpret_cast<DWORD>(lpParameter);

        // Find the closed process 
        ProcessModel* process = ProcessManager::GetProcess(processID);

        // Call the actuall callback
        process->ProcessClosedCallback();


        // Remove the process from the list
        if (ProcessManager::ProcessList.size() == 1)
        {
            ProcessManager::ProcessList.clear();
        }
        else
        {
            ProcessManager::ProcessList.erase(
                std::remove_if(ProcessManager::ProcessList.begin(), ProcessManager::ProcessList.end(),
                [&](const ProcessModel& currentProcess)
            {
                return process->ProcessInfo.dwProcessId == currentProcess.ProcessInfo.dwProcessId;
            }));
        };
    }

    // Runs a single process
    static BOOL RunProcess(ProcessModel& process)
    {
        // Fix process arguments string if needed
        NormalizeArgs(process);

        auto result = CreateProcessW(process.ProcessName.c_str(),
                                     // Because the argument string will be appended to the default path argument 
                                     // a const cast must be used to convert the LPCWSTR to a LPWSTR
                                     const_cast<wchar_t*>(process.ProcessArgs.c_str()),
                                     NULL, NULL,
                                     TRUE,
                                     NULL,
                                     NULL,
                                     NULL,
                                     &process.StartupInfo,
                                     &process.ProcessInfo);
        // Create/run the process
        if (!result)
        {
            // If process creation failed.

            // Close the process handles
            CleanupProcessHandles(process);

            return FALSE;
        };

        // If prcess creation was successful, 

        // Get the hanlde for the process' main window
        HWND hwnd = GetProcessHWND(process.GetPID());

        if (hwnd == NULL)
        {
            return FALSE;
        };

        process.MainWindowHandle = hwnd;

        // Get the rest of the handles
        process.handles = GetProcessHWNDs(process.GetPID());


        auto processIDPointer = reinterpret_cast<PVOID>(process.ProcessInfo.dwProcessId);

        HANDLE hNewHandle;
        RegisterWaitForSingleObject(&hNewHandle, process.ProcessInfo.hProcess, ProcessTerminatedCallback, processIDPointer, INFINITE, WT_EXECUTEONLYONCE);

        return TRUE;
    };



    // Runs a single process
    static DWORD RunProcess(const wchar_t* processName, const wchar_t* processArgs, ProcessClosedCallback processClosedCallback, bool visibleOnStartup)
    {
        ProcessModel process(processName, processArgs);
        BOOL result = ProcessManager::RunProcess(process);


        if (result == FALSE)
        {
            return 0;
        }
        else
        {
            process.ProcessClosedCallback = processClosedCallback;

            ProcessManager::ProcessList.push_back(process);

            if (visibleOnStartup == false)
                ShowWindow(process.MainWindowHandle, SW_HIDE);

            return process.ProcessInfo.dwProcessId;
        };
    };

    // An overload that runs a console process
    static DWORD RunConsoleProcess(const wchar_t* processName, const wchar_t* processArgs, ProcessClosedCallback processClosedCallback, bool visibleOnStartup)
    {
        // Create the process 
        ProcessModel process(processName, processArgs);

        // Normalize the process arguments
        NormalizeArgs(process);

        // Combine the process path and arguments into a single string 
        std::wstring cmdLet = std::wstring(process.ProcessName) + std::wstring(process.ProcessArgs);

        // Convert the arguments to a non-const string
        wchar_t* cmdlines = const_cast<wchar_t*>(cmdLet.c_str());

        // Run the process
        if (!CreateProcessW(NULL, cmdlines,
            NULL, NULL,
            FALSE,
            NULL,
            FALSE,
            FALSE,
            &process.StartupInfo,
            &process.ProcessInfo))
        {
            CloseHandle(process.ProcessInfo.hThread);
            CloseHandle(process.ProcessInfo.hProcess);

            return 0;
        };

        HWND hwnd = GetProcessHWND(process.GetPID(), 3000);

        process.MainWindowHandle = hwnd;
        process.ProcessClosedCallback = processClosedCallback;

        HANDLE hNewHandle;
        RegisterWaitForSingleObject(&hNewHandle, process.ProcessInfo.hProcess, ProcessTerminatedCallback, reinterpret_cast<PVOID>(process.ProcessInfo.dwProcessId), INFINITE, WT_EXECUTEONLYONCE);

        ProcessList.push_back(process);

        return process.GetPID();
    }


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
            //UnhookWinEvent(process.Hook);

                // Close the process
            if (!TerminateProcess(process.ProcessInfo.hProcess, 0))
                return FALSE;

            if (!CleanupProcessHandles(process))
                return FALSE;

        }
        // Don't do anything if the process is closed
        else
        {
            return FALSE;
        };

        return TRUE;
    }

    static void CloseProcessTree(ProcessModel& process)
    {
        PROCESSENTRY32W processEntry = { 0 };
        processEntry.dwSize = sizeof(processEntry);

        HANDLE hSnap = CreateToolhelp32Snapshot(TH32CS_SNAPPROCESS, 0);

        if (Process32FirstW(hSnap, &processEntry))
        {
            BOOL bContinue = TRUE;

            while (bContinue)
            {
                if (processEntry.th32ParentProcessID == process.GetPID())
                {
                    HANDLE hChildProc = OpenProcess(PROCESS_ALL_ACCESS, FALSE, processEntry.th32ProcessID);

                    if (hChildProc)
                    {
                        BOOL termResult = TerminateProcess(hChildProc, 0);
                        
                        CloseHandle(hChildProc);
                    };
                };

                bContinue = Process32NextW(hSnap, &processEntry);
            };

            CloseProcess(process);
        };

        CloseHandle(hSnap);
    };


    // Returns a reference to a running process
    static ProcessModel* GetProcess(DWORD processID)
    {
        for (ProcessModel& process : ProcessManager::ProcessList)
        {
            if (process.ProcessInfo.dwProcessId == processID)
            {
                return &process;
            };
        };

        return nullptr;
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

        return TRUE;
    }

};