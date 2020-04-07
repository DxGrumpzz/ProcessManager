#pragma once

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