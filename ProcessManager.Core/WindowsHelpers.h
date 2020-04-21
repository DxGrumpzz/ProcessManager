#pragma once
#include <shlobj_core.h>
#include <winbase.h>
#include <string>



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


// Simple macro used to "debug" WinApi calls. displays an Ansi MessageBox with error details 
#define WINCALL(wincall) if(!wincall) { auto error = GetLastErrorAsStringA(); MessageBoxA(NULL, error.c_str(), "Error", NULL); throw std::exception(error.c_str()); }



// Calls the windows Directory dialog using COM.
// Returns the path to the opened folder
extern "C" _declspec(dllexport) void OpenDirectoryDialog(wchar_t*& path)
{
    // The file dialog modal window 
    IFileDialog* fileDialog;

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
            };
        };
        fileDialog->Release();
    };
};