#include <windows.h>
#include <string>
#include <array>
#include <exception>

#include <commctrl.h>
#include <vector>
#pragma comment(lib, "comctl32.lib")

#define DLL_CALL extern "C" __declspec(dllexport) 

#define WM_ICON_CALLBACK 42069


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


#define WINCALL(wincall) if(!wincall) { auto error = GetLastErrorAsStringA(); MessageBoxA(NULL, error.c_str(), "Error", NULL); throw std::exception(error.c_str()); }



struct SystemTrayIconData
{
    const wchar_t* ProjectName;
    void* Data;
    void (*Callback)(void* data);
};

LRESULT Subclassproc(HWND hwnd, UINT uMsg, WPARAM wParam, LPARAM lParam, UINT_PTR uIdSubclass, DWORD_PTR dwRefData)
{
    switch (uMsg)
    {
        case WM_MENUCOMMAND:
        {
            HMENU menu = (HMENU)lParam;
            int menuItemIndex = (int)wParam;

            MENUITEMINFOW menuItem = { 0 };
            menuItem.cbSize = sizeof(menuItem);

            GetMenuItemInfoW(menu, menuItemIndex, TRUE, &menuItem);

            return DefSubclassProc(hwnd, uMsg, wParam, lParam);
        };

        case WM_ICON_CALLBACK:
        {
            WORD loWord = LOWORD(lParam);
            WORD hiWord = LOWORD(lParam);

            switch (lParam)
            {
                case WM_LBUTTONDBLCLK:
                {
                    SetForegroundWindow(hwnd);
                    return DefSubclassProc(hwnd, uMsg, wParam, lParam);
                };
            };

            switch (loWord)
            {
                case WM_RBUTTONUP:
                {
                    POINT cursorPoint;
                    GetCursorPos(&cursorPoint);

                    std::vector<const wchar_t*>& systemTrayIconData = *reinterpret_cast<std::vector<const wchar_t*>*>(dwRefData);


                    HMENU menu = CreatePopupMenu();

                    int index = 2;
                    for (const wchar_t* project : systemTrayIconData)
                    {
                        int stringLength = wcslen(project) + 1;

                        MENUITEMINFOW menuItem = { 0 };
                        menuItem.cbSize = sizeof(MENUITEMINFO);
                        wchar_t s[] = L"asdf";

                       /* 
                        menuItem.fMask = MIIM_DATA | MIIM_ID | MIIM_STATE | MIIM_STRING | MIIM_TYPE;
                        menuItem.fType = MFT_STRING;
                        menuItem.wID = index + 1;
                        menuItem.dwItemData = reinterpret_cast<ULONG_PTR>(project);
                        menuItem.dwTypeData = const_cast<wchar_t*>(project);
                        menuItem.cch = stringLength;*/
                        menuItem.fMask = MIIM_STRING | MIIM_ID;
                        menuItem.fType = MFT_STRING;
                        menuItem.wID = 0;
                        menuItem.dwTypeData = s;
                        menuItem.cch = 5;


                        WINCALL(InsertMenuItemW(menu, 0, TRUE, &menuItem));

                        index++;
                    };

                    //int index = 0;
                    //for (const wchar_t* project : systemTrayIconData)
                    //{
                    //    InsertMenuW(menu, index, MF_BYPOSITION, index + 1, project);
                    //    index++;
                    //};


                    MENUINFO menuInfo = { 0 };
                    menuInfo.cbSize = sizeof(menuInfo);
                    menuInfo.fMask = MIM_MAXHEIGHT | MIM_BACKGROUND | MIM_HELPID | MIM_MENUDATA | MIM_STYLE | MIM_APPLYTOSUBMENUS;
                    menuInfo.dwStyle = MNS_NOTIFYBYPOS;


                    //SetMenuInfo(menu, &menuInfo);

                    SetForegroundWindow(hwnd);

                    WINCALL(TrackPopupMenu(menu, TPM_BOTTOMALIGN | TPM_LEFTALIGN, cursorPoint.x - 5, cursorPoint.y + 5, NULL, hwnd, NULL));

                    break;
                };
            };

            break;
        };

        default:
            break;
    }

    return DefSubclassProc(hwnd, uMsg, wParam, lParam);
};



DLL_CALL NOTIFYICONDATAW* CreateSystemTrayIcon(HWND mainWindowHandle, const wchar_t* iconPath, const wchar_t** systemTrayIconData, int numberOfProjects)
{
    std::vector<wchar_t*>* heapData = new std::vector<wchar_t*>();
    heapData->reserve(numberOfProjects);

    for (size_t a = 0; a < numberOfProjects; a++)
    {
        size_t length = wcslen(systemTrayIconData[a]) + 1;

        heapData->emplace_back(new wchar_t[length]);

        wcscpy_s((*heapData)[a], length, systemTrayIconData[a]);
    };

    // Add a Subclass to the main window so we can handle NotifyIcon events
    SetWindowSubclass(mainWindowHandle, Subclassproc, 42071, reinterpret_cast<DWORD_PTR>(heapData));

    NOTIFYICONDATAW* notifyIconData = new NOTIFYICONDATAW();
    HANDLE icon = LoadImageW(GetModuleHandle(NULL), iconPath, IMAGE_ICON, 0, 0, LR_LOADFROMFILE | LR_DEFAULTSIZE);

    if (!icon)
    {
        std::wstring errorString = GetLastErrorAsStringW();
        DebugBreak();

        return nullptr;
    };


    notifyIconData->cbSize = sizeof(notifyIconData);
    notifyIconData->hWnd = mainWindowHandle;
    notifyIconData->uCallbackMessage = WM_ICON_CALLBACK;
    notifyIconData->uVersion = NOTIFYICON_VERSION_4;
    notifyIconData->dwState = NIS_SHAREDICON;

    wcscpy_s(notifyIconData->szTip, 128, L"ProcessManager.UI");
    wcscpy_s(notifyIconData->szInfo, 200, L"ProcessManager.UI");
    wcscpy_s(notifyIconData->szInfoTitle, 64, L"szInfoTitle");

    notifyIconData->uFlags = NIF_MESSAGE | NIF_ICON | NIF_TIP;
    notifyIconData->dwInfoFlags = NIIF_INFO;
    notifyIconData->hIcon = (HICON)icon;

    return notifyIconData;
};


DLL_CALL void ShowSystemTrayIcon(NOTIFYICONDATAW* icon)
{
    BOOL notifyIcon = Shell_NotifyIconW(NIM_ADD, icon);

    if (notifyIcon == FALSE)
    {
        std::string errorString = GetLastErrorAsStringA();
        errorString.insert(0, "Unable to create icon. \nError: ");
    };
}

DLL_CALL void RemoveSystemTrayIcon(NOTIFYICONDATAW* icon)
{
    BOOL notifyIcon = Shell_NotifyIconW(NIM_DELETE, icon);


    if (notifyIcon == FALSE)
    {
        std::wstring errorString = GetLastErrorAsStringW();
        DebugBreak();
    };

    delete icon;
}