#pragma once
#include <shlobj_core.h>
#include <winbase.h>
#include <string>
#include <chrono>
#include <comdef.h>
#include <iostream>
#include <atlbase.h>

// Takes a DWORD error code and returns its string message 
static std::wstring GetLastErrorAsStringW()
{
    // Stores the error message as a string in memory
    LPWSTR buffer = nullptr;

    // Format DWORD error ID to a string 
    FormatMessageW(FORMAT_MESSAGE_ALLOCATE_BUFFER | FORMAT_MESSAGE_FROM_SYSTEM | FORMAT_MESSAGE_IGNORE_INSERTS,
                   NULL,
                   GetLastError(),
                   MAKELANGID(LANG_NEUTRAL, SUBLANG_DEFAULT),
                   (LPWSTR)&buffer, 0, NULL);

    // Create std string from buffer
    std::wstring message(buffer);

    return message;
};


// Takes a DWORD error code and returns its string message 
static std::string GetLastErrorAsStringA()
{
    // Stores the error message as a string in memory
    LPSTR buffer = nullptr;

    // Format DWORD error ID to a string 
    FormatMessageA(FORMAT_MESSAGE_ALLOCATE_BUFFER | FORMAT_MESSAGE_FROM_SYSTEM | FORMAT_MESSAGE_IGNORE_INSERTS,
                   NULL,
                   GetLastError(),
                   MAKELANGID(LANG_NEUTRAL, SUBLANG_DEFAULT),
                   (LPSTR)&buffer, 0, NULL);

    // Create std string from buffer
    std::string message(buffer);

    return message;
}


// Simple macro used to "debug" WinApi calls. displays an Ansi MessageBox with error details 
#define WINCALL(wincall) if(!wincall) { auto error = GetLastErrorAsStringA(); MessageBoxA(NULL, error.c_str(), "Error", NULL); throw std::exception(error.c_str()); }



// Calls the windows Directory dialog using COM.
// Returns the path to the opened folder
extern "C" _declspec(dllexport) inline bool OpenDirectoryDialog(wchar_t*& path)
{
    // The file dialog modal window 
    IFileDialog* fileDialog;
    bool result = false;

    // Create an instance of IFileDialog
    if (SUCCEEDED(CoCreateInstance(CLSID_FileOpenDialog, NULL, CLSCTX_INPROC_SERVER, IID_PPV_ARGS(&fileDialog))))
    {
        // Set the file dialog to only browse folders
        fileDialog->SetOptions(FOS_PICKFOLDERS);

        // Show the file dialog
        if (SUCCEEDED(fileDialog->Show(GetActiveWindow())))
        {
            // Get result
            IShellItem* psi;
            if (SUCCEEDED(fileDialog->GetResult(&psi)))
            {
                // Convert result to a readable string
                psi->GetDisplayName(SIGDN_DESKTOPABSOLUTEPARSING, &path);
                psi->Release();

                result = true;
            };
        }
        else
            result = false;

        fileDialog->Release();
    };

    return result;
};


// Opens the IFileDialog from an existing path
extern "C" _declspec(dllexport) bool OpenDirectoryDialogFrom(wchar_t*& path, const wchar_t* openFrom)
{
    // The file dialog modal window 
    CComPtr<IFileDialog> fileDialog;

    // Result of the modal window
    // True, User has chose a folder.
    // False, User has closed the modal without choosing
    bool result = false;

    // Create an instance of IFileDialog
    if (SUCCEEDED(CoCreateInstance(__uuidof(FileOpenDialog), NULL, CLSCTX_INPROC_SERVER, IID_PPV_ARGS(&fileDialog))))
    {
        // Set the file dialog to only browse folders
        fileDialog->SetOptions(FOS_PICKFOLDERS);

        // An option that will store the 'openFrom' value
        CComPtr<IShellItem> openFromOption;

        // Set openFromOption value to 'openFrom'
        if (SUCCEEDED(SHCreateItemFromParsingName(openFrom, NULL, IID_PPV_ARGS(&openFromOption))))
        {
            // Set starting folder
            if (SUCCEEDED(fileDialog->SetFolder(openFromOption)))
            {
                // Show the file dialog
                if (SUCCEEDED(fileDialog->Show(GetActiveWindow())))
                {
                    // Get result
                    CComPtr<IShellItem> psi;
                    if (SUCCEEDED(fileDialog->GetResult(&psi)))
                    {
                        // Convert result to a readable string
                        if (SUCCEEDED(psi->GetDisplayName(SIGDN_DESKTOPABSOLUTEPARSING, &path)))
                        {
                            result = true;
                        };
                    };
                }
                else
                    result = false;
            };
        };
    };

    return result;
}



// A struct that stores information about a process in-between WNDENUMPROC calls
struct EnumProcParam
{
    // The process' PID
    DWORD ProcessID = NULL;

    // An "out" variable that will contain the process' main window HWND
    HWND HwndOut = NULL;

    // A timing varialbe used to keep track of when the process HWND search has started
    std::chrono::steady_clock::time_point StartTime = std::chrono::steady_clock::now();

    // How long to keep searching for 
    int TimeoutMS = 0;

    // A boolean flag that indicates if the search has timed out
    bool TimedOut = false;
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
