#include <Windows.h>
#include <processthreadsapi.h >
#include <cstring>
#include <sstream>
#include <fstream>
#include <vector>
#include <string>

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


struct ProcessModel
{

    std::string ProcessName;
    std::string ProcessArgs;
    

    ProcessModel(std::string _processName, std::string _processArgs) :
        ProcessName(_processName),
        ProcessArgs(_processArgs)
    {
    };

};
        

// Returns a list of ProcessModel which contain name and arguments of a process 
std::vector<ProcessModel> GetProcessList(const char* filename = "Processes.txt")
{
    // Stores the list of processes as a ProcessModel struct
    std::vector<ProcessModel> processes;

    // The processes file
    std::ifstream file(filename);

    // If file is invalid
    if (!file)
    {
        std::string error = "File error ";
        error.append(filename);

        throw new std::runtime_error(error);
    };

    // This is absolute aids. 
    // This will improve


    // Iterate through the file line by line
    // Store current read line 
    std::string line;
    while (std::getline(file, line))
    {
        // If current line is the process name
        if (line == "[Process]")
        {
            // Read process name into current line
            std::getline(file, line);

            // Add the process to the list
            processes.emplace_back(line, "");
        }
        // If current line is the process' arguments
        else if (line == "[Args]")
        {
            // Read next line
            std::getline(file, line);

            // Because of the way command arguments are interpreted a space must be inserted in the beggining of the string
            line.insert(line.begin(), ' ');

            // Set the process' arguments
            auto process = (processes.end() - 1);
            process->ProcessArgs = line;
        };
        };


    return processes;
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
