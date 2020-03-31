#include "ProcessManager.h"
#include "ProcessModel.h"



#define DLL_CALL extern "C" __declspec(dllexport) 


// Takes a DWORD error code and returns its string message 
std::wstring GetErrorStringW(DWORD error)
{
    // Stores the error message as a string in memory
    LPWSTR buffer = nullptr;

    // Format DWORD error ID to a string 
    FormatMessageW(FORMAT_MESSAGE_ALLOCATE_BUFFER | FORMAT_MESSAGE_FROM_SYSTEM | FORMAT_MESSAGE_IGNORE_INSERTS,
                   NULL,
                   error,
                   MAKELANGID(LANG_NEUTRAL, SUBLANG_DEFAULT),
                   (LPWSTR)&buffer, 0, NULL);

    // Create std string from buffer
    std::wstring message(buffer);

    return message;
};



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