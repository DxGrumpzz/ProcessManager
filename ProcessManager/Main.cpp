#include <Windows.h>
#include <cstring>
#include <sstream>
#include <fstream>
#include <vector>
#include <string>
#include <future>
#include <iostream>


// Takes a DWORD error code and returns its string message 
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

// A model class for a process
struct ProcessModel
{
    // The name/path of the process
    std::string ProcessName;

    // The process' arguments
    std::string ProcessArgs;

    ProcessModel(std::string _processName, std::string _processArgs) :
        ProcessName(_processName),
        ProcessArgs(_processArgs)
    {
    };

};


// Returns a list of ProcessModel which contain name and arguments of a process 
std::vector<ProcessModel> GetProcessListFromFile(const char* filename = "Processes.txt")
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


// A list of processes which are currently running
std::vector<PROCESS_INFORMATION> _processList;


// Handles console closing 
BOOL WINAPI CtrlHandler(DWORD fdwCtrlType)
{
    for (const PROCESS_INFORMATION& process : _processList)
    {
        TerminateProcess(process.hProcess, 0);

        CloseHandle(process.hProcess);
        CloseHandle(process.hThread);
    };

    return FALSE;
};


// Creates and runs a process
PROCESS_INFORMATION RunProcess(const ProcessModel& process)
{
    // "Converts" the process arguments from the const char* to a char*
    LPSTR args = const_cast<char*>(process.ProcessArgs.c_str());
    
    // Process information structs
    STARTUPINFOA info = { sizeof(info) };
    PROCESS_INFORMATION processInfo;

    // Try to create the process
    // If process creation failed
    if (!CreateProcessA(process.ProcessName.c_str(), args, NULL, NULL, FALSE, CREATE_NO_WINDOW, NULL, NULL, &info, &processInfo))
    {
        // Get error message
        DWORD errorID = GetLastError();
        std::string errorString = GetErrorString(errorID);

        // display error(s)
        if (errorID == ERROR_FILE_NOT_FOUND)
        {
            std::cout << "File not found: " << process.ProcessName << "\n";
        };

        // Clean process junk
        CloseHandle(processInfo.hProcess);
        CloseHandle(processInfo.hThread);

        return processInfo;
    };

    return processInfo;
};



int main()
{
    // Intercept user exit to clear the processes
    SetConsoleCtrlHandler(CtrlHandler, TRUE);

    // Get processes from file
    std::vector<ProcessModel> processes = GetProcessListFromFile();

    std::cout << "Creating " << processes.size() << " procceses" << "\n";


    // For every process request try to run it
    for (const ProcessModel& process : processes)
    {
        auto result = RunProcess(process);

        // If process creation was successful
        if (result.dwProcessId != 0x0)
        {
            // Add process handle to process list
            _processList.push_back(result);
        };
    };
    

    std::cin.get();
};
