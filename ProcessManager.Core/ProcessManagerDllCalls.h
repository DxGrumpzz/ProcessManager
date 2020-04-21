#pragma once
#include "ProcessManager.h"
#include "ProcessModel.h"


#define DLL_CALL extern "C" __declspec(dllexport) 


DLL_CALL DWORD RunProcess(const wchar_t* processName, const wchar_t* processArgs, ProcessClosedCallback processClosedCallback, bool visibleOnStartup)
{
    DWORD processID = ProcessManager::RunProcess(processName, processArgs, processClosedCallback, visibleOnStartup);

    return processID;
};


DLL_CALL DWORD RunConsoleProcess(const wchar_t* processName, const wchar_t* processArgs, ProcessClosedCallback processClosedCallback, bool visibleOnStartup)
{
    return ProcessManager::RunConsoleProcess(processName, processArgs, processClosedCallback, visibleOnStartup);
}


DLL_CALL void CloseProcess(DWORD processID)
{
    for (ProcessModel& process : ProcessManager::ProcessList)
    {
        if (process.ProcessInfo.dwProcessId == processID)
        {
            ProcessManager::CloseProcess(process);
        };
    };
};


DLL_CALL void CloseProcessTree(DWORD processID)
{
    for (ProcessModel& process : ProcessManager::ProcessList)
    {
        if (process.ProcessInfo.dwProcessId == processID)
        {
            ProcessManager::CloseProcessTree(process);
        };
    };
};


DLL_CALL bool ShowProcess(DWORD processID)
{
    ProcessModel* process = ProcessManager::GetProcess(processID);

    if (process == nullptr)
        return false;

    ShowWindow(process->MainWindowHandle, SW_SHOW);

    return true;
};


DLL_CALL bool HideProcess(DWORD processID)
{
    ProcessModel* process = ProcessManager::GetProcess(processID);

    if (process == nullptr)
        return false;

    ShowWindow(process->MainWindowHandle, SW_HIDE);

    return true;
};


DLL_CALL bool IsProcessRunning(DWORD processID)
{
    return ProcessManager::IsProcessRunning(processID);
}





// Takes a DWORD error code and returns its string message 
//std::string GetLastErrorAsString()
//{
//    // Stores the error message as a string in memory
//    LPSTR buffer = nullptr;
//
//    // Format DWORD error ID to a string 
//    FormatMessageA(FORMAT_MESSAGE_ALLOCATE_BUFFER | FORMAT_MESSAGE_FROM_SYSTEM | FORMAT_MESSAGE_IGNORE_INSERTS,
//                   NULL,
//                   GetLastError(),
//                   MAKELANGID(LANG_NEUTRAL, SUBLANG_DEFAULT),
//                   (LPSTR)&buffer, 0, NULL);
//
//    // Create std string from buffer
//    std::string message(buffer);
//
//    return message;
//}


DLL_CALL bool Test(const wchar_t* runFromDirectory, wchar_t* script, ProcessClosedCallback processClosedCallback, bool visibleOnStartup, DWORD& pid)
{
    STARTUPINFOW startupInfo = { 0 };
    startupInfo.cb = sizeof(startupInfo);

    PROCESS_INFORMATION processInfo = { 0 };

    ProcessModel process(L"", L"");
    process.StartInDirectory = runFromDirectory;
    process.ConsoleScript = script;

    std::wstring scriptAsWString(script);
    scriptAsWString.insert(0, L"/C ");

    wchar_t* cmdlines = const_cast<wchar_t*>(scriptAsWString.c_str());

    if (!CreateProcessW(L"C:\\WINDOWS\\system32\\cmd.exe", cmdlines,
        NULL, NULL,
        FALSE,
        NULL,
        NULL,
        runFromDirectory,
        &startupInfo,
        &processInfo))
    {
        CloseHandle(processInfo.hProcess);
        CloseHandle(processInfo.hThread);

        return false;
    };

    process.ProcessInfo = processInfo;
    process.StartupInfo = startupInfo;

    
    HWND hwnd = GetProcessHWND(processInfo.dwProcessId, 3000);

    process.MainWindowHandle = hwnd;
    process.ProcessClosedCallback = processClosedCallback;

    HANDLE hNewHandle;
    RegisterWaitForSingleObject(&hNewHandle, process.ProcessInfo.hProcess, ProcessManager::ProcessTerminatedCallback, reinterpret_cast<PVOID>(process.GetPID()), INFINITE, WT_EXECUTEONLYONCE);

    ProcessManager::ProcessList.push_back(process);


    if (visibleOnStartup == false)
        ShowWindow(process.MainWindowHandle, SW_HIDE);


    pid = process.GetPID();

    return true;
};
