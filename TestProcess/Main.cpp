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
PROCESS_INFORMATION processInfo = { 0 };


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


bool _doEvent = false;
void CALLBACK WinEventHookCallback(HWINEVENTHOOK hWinEventHook, DWORD event, HWND hwnd, LONG idObject, LONG idChild, DWORD idEventThread, DWORD dwmsEventTime)
{
    //ShowWindowAsync(hwnd, SW_HIDE);
    _doEvent = true;
    handles.push_back(hwnd);
};



HWND button;
HWND button2;

LRESULT CALLBACK WindowProcedure(HWND hWnd, UINT message, WPARAM wParam, LPARAM lParam)
{
    switch (message)
    {
        case WM_CLOSE:
        {
            PostQuitMessage(0);

            TerminateProcess(processInfo.hProcess, 0);

            CloseHandle(processInfo.hProcess);
            CloseHandle(processInfo.hThread);

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

                        STARTUPINFOW info = { 0 };
                        info.cb = sizeof(STARTUPINFOW);
                        info.dwFlags = STARTF_USESHOWWINDOW;
                        info.wShowWindow = SW_HIDE;

                        //BOOL result = CreateProcessW(L"C:\\Software\\IL Spy\\ILSpy.exe", NULL, NULL, NULL, FALSE, CREATE_SUSPENDED | CREATE_NO_WINDOW, NULL, NULL, &info, &processInfo);
                        //BOOL result = CreateProcessW(L"C:\\Users\\yosi1\\Desktop\\AnyDesk.exe", NULL, NULL, NULL, FALSE, CREATE_SUSPENDED | CREATE_NO_WINDOW, NULL, NULL, &info, &processInfo);
                        BOOL result = CreateProcessW(L"C:\\Software\\Microsoft VS Code\\Code.exe", NULL, NULL, NULL, FALSE, CREATE_SUSPENDED | CREATE_NO_WINDOW, NULL, NULL, &info, &processInfo);

                        HWINEVENTHOOK hook = SetWinEventHook(EVENT_OBJECT_CREATE, EVENT_OBJECT_CREATE, NULL, WinEventHookCallback, processInfo.dwProcessId, 0, WINEVENT_OUTOFCONTEXT);


                        if (!ResumeThread(processInfo.hThread))
                        {
                            TerminateProcess(processInfo.hProcess, 0);

                            CloseHandle(processInfo.hProcess);
                            CloseHandle(processInfo.hThread);

                            MessageBoxW(hWnd, GetErrorStringW(GetLastError()).insert(0, L"An error occured\n").c_str(), L"error", 0);
                        };


                        if (!hook)
                        {
                            TerminateProcess(processInfo.hProcess, 0);

                            CloseHandle(processInfo.hProcess);
                            CloseHandle(processInfo.hThread);

                            MessageBoxW(hWnd, GetErrorStringW(GetLastError()).insert(0, L"An error occured\n").c_str(), L"error", 0);
                        };


                        return TRUE;
                    };

                    // Abort process button
                    case 1:
                    {
                        TerminateProcess(processInfo.hProcess, 0);

                        CloseHandle(processInfo.hProcess);
                        CloseHandle(processInfo.hThread);


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
            TerminateProcess(processInfo.hProcess, 0);

            CloseHandle(processInfo.hProcess);
            CloseHandle(processInfo.hThread);

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


    ShowWindow(windowHWND, nShowCmd);
    UpdateWindow(windowHWND);

    ShowWindow(button, SW_SHOW);
    ShowWindow(button2, SW_SHOW);

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