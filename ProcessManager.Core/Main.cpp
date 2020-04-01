#include "ProcessManager.h"
#include "ProcessModel.h"
#include <commctrl.h>

#pragma comment(lib, "comctl32.lib")

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

#define WM_ICON_CALLBACK 42069


LRESULT Subclassproc(HWND hWnd, UINT uMsg, WPARAM wParam, LPARAM lParam, UINT_PTR uIdSubclass, DWORD_PTR dwRefData)
{
    switch (uMsg)
    {
        case WM_MOUSEMOVE:
        {
            int s = 0;
            break;
        };

        case WM_ICON_CALLBACK:
        {
            int s = 0;
        };

        default:
            break;
    }

    return DefSubclassProc(hWnd, uMsg, wParam, lParam);
};

#include <shellapi.h>

DLL_CALL NOTIFYICONDATAW* CreateSystemTrayIcon(HWND mainWindowHandle)
{
    // Add a Subclass to the main window so we can handle NotifyIcon events
    SetWindowSubclass(mainWindowHandle, Subclassproc, 42071, NULL);


    NOTIFYICONDATAW* notifyIconData = new NOTIFYICONDATAW();
    HANDLE icon = LoadImageW(NULL, L"C:\\Users\\yosi1\\Desktop\\New folder\\Icon.ico", IMAGE_ICON, 0, 0, LR_LOADFROMFILE | LR_DEFAULTSIZE);

    if (!icon)
    {
        std::wstring errorString = GetErrorStringW(GetLastError());
        DebugBreak();

        return nullptr;
    };


    notifyIconData->cbSize = sizeof(notifyIconData);
    notifyIconData->hWnd = mainWindowHandle;
    notifyIconData->uID = 100;
    notifyIconData->uCallbackMessage = WM_ICON_CALLBACK;
    notifyIconData->uVersion = NOTIFYICON_VERSION_4;
    notifyIconData->dwState = NIS_SHAREDICON;

    wcscpy_s(notifyIconData->szTip, 128, L"ProcessManager.UI");
    wcscpy_s(notifyIconData->szInfo, 200, L"ProcessManager.UI");
    wcscpy_s(notifyIconData->szInfoTitle, 64, L"szInfoTitle");

    notifyIconData->uFlags = NIF_MESSAGE | NIF_ICON | NIF_TIP;
    notifyIconData->dwInfoFlags = NIIF_INFO;
    notifyIconData->hIcon = (HICON)icon;

    BOOL notifyIcon = Shell_NotifyIconW(NIM_ADD, notifyIconData);

    if (notifyIcon == FALSE)
    {
        std::wstring errorString = GetErrorStringW(GetLastError());
        DebugBreak();

        return nullptr;
    };
    
    return notifyIconData;
};


DLL_CALL void RemoveSystemTrayIcon(NOTIFYICONDATAW* icon)
{
    BOOL notifyIcon = Shell_NotifyIconW(NIM_DELETE, icon);


    if (notifyIcon == FALSE)
    {
        std::wstring errorString = GetErrorStringW(GetLastError());
        DebugBreak();
    };

    delete icon;
}