#pragma once
#include <shlobj_core.h >
#include <winbase.h>
#include <string>


// Simple macro used to "debug" WinApi calls. displays an Ansi MessageBox with error details 
#define WINCALL(wincall) if(!wincall) { auto error = GetLastErrorAsStringA(); MessageBoxA(NULL, error.c_str(), "Error", NULL); throw std::exception(error.c_str()); }


// Takes a DWORD error code and returns its string message 
std::wstring GetLastErrorAsStringW()
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
std::string GetLastErrorAsStringA()
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


// Calls the windows Directory dialog.
// Returns the path to the opened folder
extern "C" _declspec(dllexport) void OpenDirectoryDialog(wchar_t*& path)
{
    // Hold the folder's path 
    path = new wchar_t[MAX_PATH] { 0 };

    // Data that will be sent to SHBrowseForFolderW
    BROWSEINFOW browseInfo = { 0 };

    // The "title" 
    browseInfo.lpszTitle = L"Select project folder";

    browseInfo.ulFlags = BIF_USENEWUI | BIF_STATUSTEXT | BIF_UAHINT;

    // Open the dialog
    LPITEMIDLIST pidl = SHBrowseForFolderW(&browseInfo);
    
    if (pidl == 0)
        return;

    // get the name of the folder and put it in path
    SHGetPathFromIDListW(pidl, path);
};

extern "C" _declspec(dllexport) void DeallocPathPointer(wchar_t* path)
{
    delete[] path;
};