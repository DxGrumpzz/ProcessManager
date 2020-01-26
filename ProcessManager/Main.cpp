#include <Windows.h>
#include <processthreadsapi.h >
#include <cstring>
#include <sstream>
#include <sstream>

std::string GetErrorString(DWORD error)
{
    // Stores the error message as a string in memory
    LPSTR buffer = nullptr;

    // Format DWORD error ID to a string 
    FormatMessageA(FORMAT_MESSAGE_ALLOCATE_BUFFER | FORMAT_MESSAGE_FROM_SYSTEM | FORMAT_MESSAGE_IGNORE_INSERTS,
        NULL, error, MAKELANGID(LANG_NEUTRAL, SUBLANG_DEFAULT), (LPSTR)&buffer, 0, NULL);

    // Create std string from buffer
    std::string message(buffer);

    return message;
};


int main()
{
    const char* path = "C:\\WINDOWS\\system32\\notepad.exe";
    
    LPSTR args = const_cast<char*>(" C:\\Users\\yosi1\\Desktop\\Wrists.txt");

    STARTUPINFOA info = { sizeof(info) };
    PROCESS_INFORMATION processInfo = { 0 };

    
    if (CreateProcessA(path, args, NULL, NULL, TRUE, 0, NULL, NULL, &info, &processInfo))
    {
        // Block current thread until process exists
        WaitForSingleObject(processInfo.hProcess, INFINITE);

        // Get process exit code after exiting
        LPDWORD exitCode = { 0 };
        BOOL s = GetExitCodeProcess(processInfo.hProcess, (LPDWORD)&exitCode);

        // Close/free process handles
        CloseHandle(processInfo.hProcess);
        CloseHandle(processInfo.hThread);
    }
    else
    {
        std::string errorString = GetErrorString(GetLastError());
    };

};
