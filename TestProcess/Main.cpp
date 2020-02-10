#include <windows.h>
#include <string>
#include <vector>
#include <tlhelp32.h>
#include <wbemcli.h>
#include <msinkaut.h>

#define RBG_UNIFORM(uniformColour) RGB(uniformColour,uniformColour,uniformColour) 


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



const wchar_t* windowTitle = L"Window title";
const wchar_t* windowClassName = L"DesktopApp";



std::vector<HWND> handles;
PROCESS_INFORMATION _processInfo = { 0 };



bool _doEvent = false;
void CALLBACK WinEventHookCallback(HWINEVENTHOOK hWinEventHook, DWORD event, HWND hwnd, LONG idObject, LONG idChild, DWORD idEventThread, DWORD dwmsEventTime)
{
    _doEvent = true;
    handles.push_back(hwnd);
};


BOOL CloseProcess(PROCESS_INFORMATION process)
{
    if (!TerminateProcess(process.hProcess, 0))
        return FALSE;

    if(!CloseHandle(process.hProcess))
        return FALSE;

    if(!CloseHandle(process.hThread))
        return FALSE;
    
    return TRUE;
}

PROCESS_INFORMATION RunProcess(const wchar_t* processName)
{
    PROCESS_INFORMATION processInfo = { 0 };

    STARTUPINFOW info = { 0 };
    info.cb = sizeof(STARTUPINFOW);
    info.dwFlags = STARTF_USESHOWWINDOW;
    info.wShowWindow = SW_HIDE;

    BOOL result = CreateProcessW(processName, NULL, NULL, NULL, FALSE, CREATE_SUSPENDED | CREATE_NO_WINDOW, NULL, NULL, &info, &processInfo);

    HWINEVENTHOOK hook = SetWinEventHook(EVENT_OBJECT_CREATE, EVENT_OBJECT_CREATE, NULL, WinEventHookCallback, _processInfo.dwProcessId, 0, WINEVENT_OUTOFCONTEXT);


    if (!ResumeThread(_processInfo.hThread))
    {
        CloseProcess(processInfo);
    };

    if (!hook)
    {
        CloseProcess(processInfo);
    };

    return processInfo;
}


HWND button;
HWND button2;

LRESULT CALLBACK WindowProcedure(HWND hWnd, UINT message, WPARAM wParam, LPARAM lParam)
{
    switch (message)
    {
        case WM_CLOSE:
        {
            PostQuitMessage(0);

            CloseProcess(_processInfo);

            return TRUE;
        };


        case  WM_COMMAND:
        {

            if (HIWORD(wParam) == BN_CLICKED)
            {
                WORD buttonId = LOWORD(wParam);

                switch (buttonId)
                {
                    // Create process button
                    case 0:
                    {
                        /*
                        STARTUPINFOW info = { 0 };
                        info.cb = sizeof(STARTUPINFOW);
                        info.dwFlags = STARTF_USESHOWWINDOW;
                        info.wShowWindow = SW_HIDE;

                        BOOL result = CreateProcessW(L"C:\\Software\\IL Spy\\ILSpy.exe", NULL, NULL, NULL, FALSE, CREATE_SUSPENDED | CREATE_NO_WINDOW, NULL, NULL, &info, &_processInfo);
                        //BOOL result = CreateProcessW(L"C:\\Users\\yosi1\\Desktop\\AnyDesk.exe", NULL, NULL, NULL, FALSE, CREATE_SUSPENDED | CREATE_NO_WINDOW, NULL, NULL, &info, &processInfo);
                        //BOOL result = CreateProcessW(L"C:\\Software\\Microsoft VS Code\\Code.exe", NULL, NULL, NULL, FALSE, CREATE_SUSPENDED | CREATE_NO_WINDOW, NULL, NULL, &info, &processInfo);

                        HWINEVENTHOOK hook = SetWinEventHook(EVENT_OBJECT_CREATE, EVENT_OBJECT_CREATE, NULL, WinEventHookCallback, _processInfo.dwProcessId, 0, WINEVENT_OUTOFCONTEXT);


                        if (!ResumeThread(_processInfo.hThread))
                        {
                            TerminateProcess(_processInfo.hProcess, 0);

                            CloseHandle(_processInfo.hProcess);
                            CloseHandle(_processInfo.hThread);

                            MessageBoxW(hWnd, GetErrorStringW(GetLastError()).insert(0, L"An error occured\n").c_str(), L"error", 0);
                        };


                        if (!hook)
                        {
                            TerminateProcess(_processInfo.hProcess, 0);

                            CloseHandle(_processInfo.hProcess);
                            CloseHandle(_processInfo.hThread);

                            MessageBoxW(hWnd, GetErrorStringW(GetLastError()).insert(0, L"An error occured\n").c_str(), L"error", 0);
                        };
                        */

                        _processInfo = RunProcess(L"C:\\Software\\IL Spy\\ILSpy.exe");

                        return TRUE;
                    };

                    // Abort process button
                    case 1:
                    {
                        CloseProcess(_processInfo);

                        return TRUE;
                    };


                    default:
                        return DefWindowProcW(hWnd, message, wParam, lParam);
                };
            };

            return TRUE;
        };


        case WM_DESTROY:
        {
            CloseProcess(_processInfo);

            return TRUE;
        };

        default:
            return DefWindowProcW(hWnd, message, wParam, lParam);
    };
};



// _In_opt_ nad _In_ are something called SAL annotations, They mean that a parameter maybe be passed as NULL
int WINAPI wWinMain(_In_ HINSTANCE hInstance, _In_opt_ HINSTANCE hPrevInstance, _In_ LPWSTR lpCmdLine, _In_ int nShowCmd)
{

    WNDCLASSEXW windowClass = { 0 };
    windowClass.cbSize = sizeof(WNDCLASSEXW);
    windowClass.style = CS_HREDRAW | CS_VREDRAW;
    windowClass.hInstance = hInstance;
    windowClass.lpszClassName = windowClassName;
    windowClass.lpfnWndProc = WindowProcedure;
    windowClass.hbrBackground = CreateSolidBrush(RBG_UNIFORM(0xE1));
    windowClass.hCursor = LoadCursorW(NULL, IDC_ARROW);


    ATOM registerClassResult = RegisterClassExW(&windowClass);

    if (registerClassResult == 0)
    {
        std::wstring error = GetErrorStringW(GetLastError());
        error.insert(0, L"An error occured while creating window.\n");

        MessageBoxW(NULL, error.c_str(), NULL, NULL);

        return 1;
    };




    HWND windowHWND = CreateWindowExW(NULL,
                                      windowClassName,
                                      windowTitle,
                                      WS_CAPTION | WS_BORDER | WS_SYSMENU | WS_MAXIMIZEBOX | WS_MINIMIZEBOX | WS_SIZEBOX,
                                      0, 0,
                                      500, 350,
                                      NULL,
                                      NULL,
                                      hInstance,
                                      NULL);


    if (windowHWND == 0)
    {
        std::wstring error = GetErrorStringW(GetLastError());
        error.insert(0, L"An error occured while creating window.\n");

        MessageBoxW(NULL, error.c_str(), NULL, NULL);

        return 1;
    };

    button = CreateWindowExW(NULL,
                             L"BUTTON",
                             L"Create process",
                             WS_BORDER | WS_CHILD,
                             10,
                             10,
                             120,
                             30,
                             windowHWND,
                             (HMENU)0,
                             hInstance,
                             NULL);


    button2 = CreateWindowExW(NULL,
                              L"BUTTON",
                              L"Stop process",
                              WS_BORDER | WS_CHILD,
                              10,
                              45,
                              120,
                              30,
                              windowHWND,
                              (HMENU)1,
                              hInstance,
                              NULL);




    // Set button cursor
    SetClassLongPtrW(button, -12, (LONG_PTR)LoadCursorW(NULL, IDC_HAND));


    const int  CHAR_MULTIPLIER = 8;
    std::wstring processName = L"Process1";

    int TEXT_HEIGHT = (int)(std::count(processName.begin(), processName.end(), L'\n') + 1) * 15;

    const int TEXT_WIDTH = processName.size() * CHAR_MULTIPLIER;
    
    const int TEXT_X_POSITION = abs(500 - TEXT_WIDTH) - 15;


    HWND textBlock = CreateWindowExW(NULL,
                                     L"STATIC",
                                     processName.c_str(),
                                     WS_CHILD,
                                     TEXT_X_POSITION, 0,
                                     TEXT_WIDTH, TEXT_HEIGHT,
                                     windowHWND,
                                     NULL,
                                     hInstance,
                                     NULL);


    ShowWindow(windowHWND, nShowCmd);
    UpdateWindow(windowHWND);

    ShowWindow(button, SW_SHOW);
    ShowWindow(button2, SW_SHOW);
    ShowWindow(textBlock, SW_SHOW);
    


    MSG message;
    while (GetMessageW(&message, NULL, 0, 0) > 0)
    {
        if (_doEvent == true)
        {
            _doEvent = false;

            for (const HWND& hwnd : handles)
            {
                ShowWindow(hwnd, SW_HIDE);
            };
        };

        TranslateMessage(&message);
        DispatchMessageW(&message);
    };

    return (int)message.wParam;
};