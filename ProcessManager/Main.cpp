#include <Windows.h>
#include <fstream>
#include <vector>
#include <string>
#include <iostream>
#include <thread>
#include <mutex>
#include <set>
#include <psapi.h>

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

std::string GetLastErrorString()
{
    return GetErrorString(GetLastError());
};

// A model class for a process
struct ProcessModel
{
public:
    // The name/path of the process
    std::string ProcessName;

    // The process' arguments
    std::string ProcessArgs;

    PROCESS_INFORMATION ProcessInfo = { 0 };

    std::vector<HWND> Handles;


public:

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
        std::string error = "File error. \nCould not open: ";
        error.append(filename);

        throw std::exception(error.c_str());
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


// Creates and runs a process
PROCESS_INFORMATION RunProcess(const ProcessModel& process)
{
    // "Converts" the process arguments from the const char* to a char*
    LPSTR args = const_cast<char*>(process.ProcessArgs.c_str());

    // Process information structs
    STARTUPINFOA info = { 0 };
    info.wShowWindow = FALSE;
    info.cb = sizeof(STARTUPINFO);

    PROCESS_INFORMATION processInfo = { 0 };


    // Try to create the process
    // If process creation failed
    if (!CreateProcessA(process.ProcessName.c_str(), args, NULL, NULL, TRUE, CREATE_NO_WINDOW, NULL, NULL, &info, &processInfo))
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

    // Wait for process to initialize
    WaitForInputIdle(processInfo.hProcess, INFINITE);

    return processInfo;
};


// Takes a string and spilts it into several other strings
std::vector<std::string> SplitString(std::string stringToSplit, char delimiter)
{
    // Store the split strings
    std::vector<std::string> strings;

    // The previous index where the delimiter was located
    int previousSpaceIndex = 0;

    for (size_t a = 0; a < stringToSplit.size(); a++)
    {
        const char& currentChar = stringToSplit[a];

        // If the currenct character matches the delimiter
        if (currentChar == delimiter)
        {
            // Use substring function to take the string in between the previous index and the current index - previous
            strings.emplace_back(stringToSplit.substr(previousSpaceIndex, a - previousSpaceIndex));

            previousSpaceIndex = a + 1;
        };
    };

    // Add last string
    strings.emplace_back(stringToSplit.substr(previousSpaceIndex, stringToSplit.size() - 1));

    return strings;
};



std::vector<HWND> handles;

BOOL CALLBACK EnumWindowsCallback(HWND hwnd, LPARAM lParam)
{
    DWORD lpdwProcessId;
    GetWindowThreadProcessId(hwnd, &lpdwProcessId);

    if (lpdwProcessId == lParam)
    {
        handles.push_back(hwnd);
    };

    return TRUE;
}


void GetHwnds(DWORD processID)
{

    int previousHandleSize = 0;

    do
    {
        previousHandleSize = handles.size();
        EnumWindows(EnumWindowsCallback, processID);
    }
    while (previousHandleSize < 1);

}

int main()
{
    STARTUPINFOW info = { 0 };
    info.cb = sizeof(STARTUPINFOW);
    
    PROCESS_INFORMATION processInfo = { 0 };

    std::cout << "Initializing process\n";
    
    BOOL result = CreateProcessW(L"C:\\Software\\IL Spy\\ILSpy.exe", NULL, NULL, NULL, FALSE, CREATE_SUSPENDED, NULL, NULL, &info, &processInfo);

    std::cout << "Getting process data\n";
    

    DWORD exitCode = { 0 };
    ResumeThread(processInfo.hThread);
    
    


    do
    {
        GetExitCodeProcess(processInfo.hProcess, &exitCode);
    }
    while (exitCode == 0);


    GetHwnds(processInfo.dwProcessId);

    // Validate HWNDS
    int index = 0;
    int size = handles.size();

    for (; index < size; index++)
    {
        HWND hwnd = handles[0];
        BOOL result = IsWindow(hwnd);

        if (result <= 0)
        {
            index = 0;

            GetHwnds(processInfo.dwProcessId);
                
            size = handles.size();
        };
    };



    std::cout << "Found " << handles.size() << " handles\n";

    for (const HWND& hwnd : handles)
    {
        ShowWindowAsync(hwnd, SW_HIDE);
        ShowWindowAsync(hwnd, SW_HIDE);
    };
    

    std::cout << "Press enter key to terminate process...";
    std::cin.get();

    TerminateProcess(processInfo.hProcess, 0);

    CloseHandle(processInfo.hProcess);
    CloseHandle(processInfo.hThread);


    /*
    STARTUPINFOW info = { 0 };
    info.wShowWindow = FALSE;
    info.cb = sizeof(STARTUPINFOW);
    info.dwFlags = STARTF_USESHOWWINDOW;
    info.wShowWindow = SW_HIDE;

    PROCESS_INFORMATION processInfo = { 0 };

    BOOL result = CreateProcessW(L"C:\\Software\\IL Spy\\ILSpy.exe", NULL, NULL, NULL, FALSE, CREATE_SUSPENDED, NULL, NULL, &info, &processInfo);

    HWINEVENTHOOK hook = SetWinEventHook(EVENT_OBJECT_CREATE, EVENT_OBJECT_CREATE, NULL, Wineventproc, processInfo.dwProcessId, 0, WINEVENT_OUTOFCONTEXT);

    ResumeThread(processInfo.hThread);


    MSG msg;
    BOOL result1 = GetMessageW(&msg, NULL, 0, 0);

    while (result1 > 0)
    {
        TranslateMessage(&msg);
        DispatchMessage(&msg);
    };

    for (const HWND& handle : handles)
    {
        ShowWindow(handle, SW_HIDE);
    };

    TerminateProcess(processInfo.hProcess, 0);
    CloseHandle(processInfo.hProcess);
    CloseHandle(processInfo.hThread);
    */


    /*
    STARTUPINFOW info = { 0 };
    info.wShowWindow = FALSE;
    info.cb = sizeof(STARTUPINFOW);
    info.dwFlags = STARTF_USESHOWWINDOW;
    info.wShowWindow = SW_HIDE;

    PROCESS_INFORMATION processInfo = { 0 };

    // BOOL result = CreateProcessW(L"C:\\WINDOWS\\system32\\notepad.exe", const_cast<wchar_t*>(L" C:\\Users\\yosi1\\Desktop\\Wrists.txt"), NULL, NULL, FALSE, CREATE_NO_WINDOW, NULL, NULL, &info, &processInfo);
    // BOOL result = CreateProcessW(L"C:\\Development\\Projects\\ProcessManager\\Debug\\TestProcess.exe", NULL, NULL, NULL, FALSE, NULL, NULL, NULL, &info, &processInfo);
    // BOOL result = CreateProcessW(L"C:\\Users\\yosi1\\Desktop\\AnyDesk.exe", NULL, NULL, NULL, FALSE, NULL, NULL, NULL, &info, &processInfo);
    // BOOL result = CreateProcessW(L"C:\\Users\\yosi1\\Desktop\\AnyDesk.exe", NULL, NULL, NULL, FALSE, NULL, NULL, NULL, &info, &processInfo);

    BOOL result = CreateProcessW(L"C:\\Software\\IL Spy\\ILSpy.exe", NULL, NULL, NULL, TRUE, CREATE_NO_WINDOW, NULL, NULL, &info, &processInfo);


    std::chrono::steady_clock::time_point begin = std::chrono::steady_clock::now();

    // Wait for process to initialize
    WaitForInputIdle(processInfo.hProcess, INFINITE);

    std::chrono::steady_clock::time_point end = std::chrono::steady_clock::now();

    auto seconds = std::chrono::duration_cast<std::chrono::seconds>(end - begin);

    std::cout << "waiting for " << seconds.count() << " seconds" << "\n";

    using namespace std::chrono_literals;
    std::this_thread::sleep_for(5s);

    if (!result)
    {
        DWORD errorID = GetLastError();
        std::string errorString = GetErrorString(errorID);
    };

    EnumWindows(EnumWindowsCallback, processInfo.dwProcessId);

    std::cout << "handles: " << handles.size() << "\n";

    for (const HWND& handle : handles)
    {
        SetFocus(handle);
        ShowWindow(handle, SW_HIDE);
    };

    TerminateProcess(processInfo.hProcess, 0);
    CloseHandle(processInfo.hProcess);
    CloseHandle(processInfo.hThread);
    */


    /*
    // Get processes from file
    std::vector<ProcessModel> processes = GetProcessListFromFile();

    std::cout << "Creating " << processes.size() << " procceses" << "\n";

    // For every process request try to run it
    int counter = 0;
    for (ProcessModel& process : processes)
    {
        auto result = RunProcess(process);

        // If process creation was successful
        if (result.dwProcessId)
        {
            std::cout << process.ProcessName << " created succesfully [" << counter << "]" << "\n";

            process.ProcessInfo = result;

            counter++;
        };
    };


    // Get process HWNDs
    for (ProcessModel& process : processes)
    {
        EnumWindows(EnumWindowsCallback, process.ProcessInfo.dwProcessId);

        for(const auto& hwnd : handles)
        {
            process.Handles.push_back(hwnd);
        };

        handles.empty();
    };


    // Hide process
    for (const ProcessModel& process : processes)
    {
        for (size_t a = 0; a < sizeof(process.Handles) / sizeof(process.Handles[0]); a++)
        {
            SetFocus(process.Handles[a]);
            ShowWindow(process.Handles[a], SW_HIDE);
        };
    };

    // Close processes
    for (const ProcessModel& process : processes)
    {
        TerminateProcess(process.ProcessInfo.hProcess, 0);
        CloseHandle(process.ProcessInfo.hProcess);
        CloseHandle(process.ProcessInfo.hThread);
    }

    //EnumWindows(EnumWindowsCallback, processInfo.dwProcessId);


    //for (const HWND& handle : handles)
    //{
    //    SetFocus(handle);
    //    ShowWindow(handle, SW_HIDE);
    //};

    //TerminateProcess(processInfo.hProcess, 0);
    //CloseHandle(processInfo.hProcess);
    //CloseHandle(processInfo.hThread);

    return 0;


    */


    //// Get processes from file
    //std::vector<ProcessModel> processes = GetProcessListFromFile();

    //std::cout << "Creating " << processes.size() << " procceses" << "\n";
    //
    //// For every process request try to run it
    //int counter = 0;
    //for (const ProcessModel& process : processes)
    //{
    //    auto result = RunProcess(process);

    //    // If process creation was successful
    //    if (result.dwProcessId)
    //    {
    //        std::cout << process.ProcessName << " created succesfully [" << counter << "]" << "\n";

    //        // Add process handle to process list
    //        _processList.push_back(result);
    //        counter++;
    //    };
    //};


    //std::string command;
    //std::vector<std::string> splitComamnd;
    //
    //while (1)
    //{
    //    std::getline(std::cin, command);
    //    splitComamnd = SplitString(command, ' ');

    //    if (splitComamnd[0] == "kill")
    //    {
    //        if (splitComamnd[1] == "all")
    //        {
    //            for (const PROCESS_INFORMATION& process : _processList)
    //            {
    //                TerminateProcess(process.hProcess, 0);

    //                CloseHandle(process.hProcess);
    //                CloseHandle(process.hThread);
    //            };
    //        }
    //        else
    //        {
    //            try
    //            {
    //                int processIndex = std::stoi(splitComamnd[1]);

    // TerminateProcess(_processList[processIndex].hProcess, 0);

    //                CloseHandle(_processList[processIndex].hProcess);
    //                CloseHandle(_processList[processIndex].hThread);

    //                std::cout << "Succesfully terminated" << "\n";
    //            }
    //            catch (std::invalid_argument)
    //            {
    //                std::cout << "Invalid argument: " << splitComamnd[1] << "\n";
    //                continue;
    //            };
    //        };
    //    }
    //    else
    //    {
    //        std::cout << "Uknown command " << command << "\n";
    //    };
    //};


    std::cin.get();
};