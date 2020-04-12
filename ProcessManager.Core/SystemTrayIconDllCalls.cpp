#include <windows.h>
#include <string>
#include <array>
#include <exception>
#include <vector>

#include "SystemTrayIconData.h"
#include "WindowsHelpers.h"
#include "SystemTrayIconData.h"
#include "TrayIconMenuHelpers.h"

#include <commctrl.h>
#pragma comment(lib, "comctl32.lib")


#define DLL_CALL extern "C" __declspec(dllexport) 

#define WM_ICON_CALLBACK 42069



LRESULT CALLBACK Subclassproc(HWND hwnd, UINT uMsg, WPARAM wParam, LPARAM lParam, UINT_PTR uIdSubclass, DWORD_PTR dwRefData)
{
    switch (uMsg)
    {
        // The callback ID for the TrayIcon
        case WM_ICON_CALLBACK:
        {
            switch (lParam)
            {
                // If user double clicked the icon
                case WM_LBUTTONDBLCLK:
                {
                    // Focus the app
                    SetForegroundWindow(hwnd);
                    return DefSubclassProc(hwnd, uMsg, wParam, lParam);
                };
            };

            // How the user interacted with the TrayIcon (Mouse click, keyboard focus, etc...) 
            switch (LOWORD(lParam))
            {
                // User click on the icon with his right mouse button
                case WM_RBUTTONUP:
                {
                    std::vector<SystemTrayIconData*>* systemTrayIconData = reinterpret_cast<std::vector<SystemTrayIconData*>*>(dwRefData);


                    // Create the main context menu
                    HMENU menu = CreateTrayIconMenu(systemTrayIconData);

                    // Show menu and get result
                    //TrayIconMenuResult menuItemIndex = ShowTrayIconMenu(hwnd, menu);

                    SetForegroundWindow(hwnd);

                    // Get cursor position
                    POINT cursorPoint;
                    GetCursorPos(&cursorPoint);

                    // Show the menu
                    BOOL result = TrackPopupMenu(menu, TPM_RETURNCMD, cursorPoint.x, cursorPoint.y, NULL, hwnd, NULL);

                    if (result == 0)
                        break;
                    
                    for (int a = 0; a < result; a++)
                    {
                        // Get inner menu
                        HMENU sub = GetSubMenu(menu, a);
              
                        // Get current item
                        MENUITEMINFOW menuItem = { 0 };
                        menuItem.cbSize = sizeof(menuItem);

                        menuItem.fMask = MIIM_STRING | MIIM_ID | MIIM_DATA;

                        GetMenuItemInfoW(sub, result, FALSE, &menuItem);

                        if (menuItem.wID == result)
                        {
                            SystemTrayIconData* data = reinterpret_cast<SystemTrayIconData*>(menuItem.dwItemData);
                                
                            // Run project
                            if (result % 2 == 0)
                            {
                                data->RunProjectCallBack(data->Data);
                            }
                            // Close project
                            else
                            {
                                data->CloseProjectCallBack(data->Data);
                            };
                        };
                    };



                    // Hanlde result from menu
                    //HanldeMenuResult(menu, menuItemIndex);

                    break;
                };
            };

            break;
        };
    };

    return DefSubclassProc(hwnd, uMsg, wParam, lParam);
};



std::vector<SystemTrayIconData*>* CopyTrayIconDataToHeap(SystemTrayIconData* systemTrayIconData, int count)
{
    std::vector<SystemTrayIconData*>* heapData = new std::vector<SystemTrayIconData*>();
    heapData->reserve(count);

    // Copy the systemTrayIconData data to the Heap so it won't be lost when we call SubclassprocS
    for (int a = 0; a < count; a++)
    {
        SystemTrayIconData* current = new SystemTrayIconData();
        heapData->push_back(current);

        size_t projectNameLength = wcslen(systemTrayIconData[a].ProjectName) + 1;

        current->CloseProjectCallBack = systemTrayIconData[a].CloseProjectCallBack;
        current->RunProjectCallBack = systemTrayIconData[a].RunProjectCallBack;

        current->Data = systemTrayIconData[a].Data;
        current->ProjectName = new wchar_t[projectNameLength];

        wcscpy_s(current->ProjectName, projectNameLength, systemTrayIconData[a].ProjectName);
    };

    return heapData;
};


DLL_CALL NOTIFYICONDATAW* CreateSystemTrayIcon(HWND mainWindowHandle, const wchar_t* iconPath, SystemTrayIconData* systemTrayIconData, int numberOfProjects)
{
    auto heapData = CopyTrayIconDataToHeap(systemTrayIconData, numberOfProjects);

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