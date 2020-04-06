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
    wchar_t* ProjectName;
    void* Data;

    void (*CloseProjectCallBack)(void* data);
    void (*RunProjectCallBack)(void* data);
};


#define MENUID  7186

#define MENUITEMID  120

#define MENUITEM_RUN_PROJECT   MENUITEMID + 1
#define MENUITEM_CLOSE_PROJECT MENUITEMID 


LRESULT Subclassproc(HWND hwnd, UINT uMsg, WPARAM wParam, LPARAM lParam, UINT_PTR uIdSubclass, DWORD_PTR dwRefData)
{
    switch (uMsg)
    {
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

                    std::vector<SystemTrayIconData*>& systemTrayIconData = *reinterpret_cast<std::vector<SystemTrayIconData*>*>(dwRefData);


                    HMENU menu = CreatePopupMenu();

                    SetForegroundWindow(hwnd);

                    int index = 0;
                    for (SystemTrayIconData* project : systemTrayIconData)
                    {
                        int stringLength = wcslen(project->ProjectName) + 1;

                        HMENU innerMenu = CreatePopupMenu();

                        // Close project menu item
                        {
                            wchar_t closeProjectText[] = L"Close project";
                            size_t closeProjectTextLength = wcslen(closeProjectText) + 1;

                            MENUITEMINFOW menuItem;
                            menuItem.cbSize = sizeof(menuItem);

                            menuItem.fMask = MIIM_STRING | MIIM_ID | MIIM_DATA;
                            menuItem.fType = MFT_STRING;

                            menuItem.wID = MENUITEM_CLOSE_PROJECT;

                            menuItem.dwTypeData = closeProjectText;
                            menuItem.cch = closeProjectTextLength;

                            menuItem.dwItemData = reinterpret_cast<ULONG_PTR>(project);

                            WINCALL(InsertMenuItemW(innerMenu, menuItem.wID, MENUITEMID, &menuItem));
                        };


                        // Run project menu item
                        {
                            wchar_t runProjectText[] = L"Run project";
                            size_t runProjectTextLength = wcslen(runProjectText) + 1;

                            MENUITEMINFOW menuItem;
                            menuItem.cbSize = sizeof(menuItem);

                            menuItem.fMask = MIIM_STRING | MIIM_ID | MIIM_DATA;
                            menuItem.fType = MFT_STRING;

                            menuItem.wID = MENUITEM_RUN_PROJECT;

                            menuItem.dwTypeData = runProjectText;
                            menuItem.cch = runProjectTextLength;

                            menuItem.dwItemData = reinterpret_cast<ULONG_PTR>(project);


                            WINCALL(InsertMenuItemW(innerMenu, menuItem.wID, MENUITEMID, &menuItem));
                        };


                        MENUITEMINFOW menuItem;
                        menuItem.cbSize = sizeof(menuItem);

                        menuItem.fMask = MIIM_STRING | MIIM_ID | MIIM_DATA | MIIM_SUBMENU;
                        menuItem.fType = MFT_STRING;
                        menuItem.wID = MENUITEMID + index;
                        menuItem.dwTypeData = const_cast<wchar_t*>(project->ProjectName);
                        menuItem.cch = stringLength;
                        menuItem.dwItemData = reinterpret_cast<ULONG_PTR>(project);
                        menuItem.hSubMenu = innerMenu;

                        WINCALL(InsertMenuItemW(menu, menuItem.wID, MENUITEMID, &menuItem));

                        index++;
                    };


                    MENUINFO menuInfo = { 0 };
                    menuInfo.cbSize = sizeof(menuInfo);
                    menuInfo.fMask = MIM_HELPID | MIM_MENUDATA | MIM_STYLE;
                    menuInfo.dwContextHelpID = MENUID;
                    menuInfo.dwMenuData = reinterpret_cast<ULONG_PTR>(&systemTrayIconData);

                    SetMenuInfo(menu, &menuInfo);



                    BOOL menuItemIndex = TrackPopupMenu(menu, TPM_RETURNCMD | TPM_BOTTOMALIGN | TPM_LEFTALIGN, cursorPoint.x - 5, cursorPoint.y + 5, NULL, hwnd, NULL);

                    if (menuItemIndex != NULL)
                    {
                        MENUITEMINFOW menuItem = { 0 };
                        menuItem.cbSize = sizeof(menuItem);
                        menuItem.fMask = MIIM_ID | MIIM_DATA;


                        if (menuItemIndex == MENUITEM_RUN_PROJECT)
                        {
                            GetMenuItemInfoW(menu, menuItemIndex, FALSE, &menuItem);

                            SystemTrayIconData* project = reinterpret_cast<SystemTrayIconData*>(menuItem.dwItemData);

                            project->RunProjectCallBack(project->Data);
                        }
                        else if (menuItemIndex == MENUITEM_CLOSE_PROJECT)
                        {
                            GetMenuItemInfoW(menu, menuItemIndex, FALSE, &menuItem);

                            SystemTrayIconData* project = reinterpret_cast<SystemTrayIconData*>(menuItem.dwItemData);

                            project->CloseProjectCallBack(project->Data);
                        };
                    };

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



DLL_CALL NOTIFYICONDATAW* CreateSystemTrayIcon(HWND mainWindowHandle, const wchar_t* iconPath, SystemTrayIconData* systemTrayIconData, int numberOfProjects)
{
    std::vector<SystemTrayIconData*>* heapData = new std::vector<SystemTrayIconData*>();
    heapData->resize(numberOfProjects, new SystemTrayIconData());

    // Copy the systemTrayIconData data to the Heap so it won't be lost when we call SubclassprocS
    for (size_t a = 0; a < numberOfProjects; a++)
    {
        SystemTrayIconData* current = heapData->at(a);

        size_t projectNameLength = wcslen(systemTrayIconData[a].ProjectName) + 1;

        current->CloseProjectCallBack = systemTrayIconData[a].CloseProjectCallBack;
        current->RunProjectCallBack = systemTrayIconData[a].RunProjectCallBack;

        current->Data = systemTrayIconData[a].Data;
        current->ProjectName = new wchar_t[projectNameLength];

        wcscpy_s(current->ProjectName, projectNameLength, systemTrayIconData[a].ProjectName);
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