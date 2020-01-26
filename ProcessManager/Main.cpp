#include <Windows.h>
#include <processthreadsapi.h >
#include <cstring>
#include <sstream>
#include <fstream>
#include <vector>

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
std::vector<ProcessModel> GetProcessList(const char* filename)
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

    // Iterate through the file by line by line
    for (std::string line; std::getline(file, line);)
    {
        // If current line specifies a process name
        if (line == "[Process]")
        {
            // Get the name of the process
            std::string processName; 
            file >> processName;

            // Add "new" process to the list without arguments
            processes.emplace_back(processName, "");
        }
        // If current line specifies a processes args
        else if (line == "[Args]")
        {
            // Get args 
            std::string processArgs;
            file >> processArgs;

            // Because of the way command arguments are interpreted a space must be inserted in the beggining of the string
            processArgs.insert(processArgs.begin(), ' ');

            // Get last process created, And set it's argument
            auto process= (processes.end() - 1);
            process->ProcessArgs = processArgs;
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
