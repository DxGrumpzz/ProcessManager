#pragma once

#include <WinBase.h>
#include <WinUser.h>
#include <windef.h>
#include <string>
#include <vector>

#include "SystemTrayIconData.h"
#include "WindowsHelpers.h"

#define MENUID  7186

#define MENUITEMID  120


// Adds a Menu item to a menu
MENUITEMINFOW AddMenuItem(HMENU menu, UINT menuItemID, const std::wstring& menuItemText, const SystemTrayIconData* data, bool insertMenuItem = true)
{
    // Create the menu
    MENUITEMINFOW menuItem = { 0 };
    menuItem.cbSize = sizeof(menuItem);

    // Create the menu item..
    menuItem.fMask =
        // With a visible text
        MIIM_STRING |
        // With associated ID 
        MIIM_ID |
        // With data that we can retrieve later
        MIIM_DATA;

    // A menu type that shows text
    menuItem.fType = MFT_STRING;

    menuItem.wID = menuItemID;

    // Set the menu item's text
    menuItem.dwTypeData = const_cast<wchar_t*>(menuItemText.c_str());
    menuItem.cch = static_cast<UINT>(menuItemText.size());

    menuItem.dwItemData = reinterpret_cast<ULONG_PTR>(data);

    // neccessary bool check for AddMenuItemSub, 
    // Because we don't want to create a sub menu and insert it immediately before finishing it's initilization
    if (insertMenuItem == true)
        WINCALL(InsertMenuItemW(menu, menuItem.wID, FALSE, &menuItem));

    return menuItem;
}


// Adds a sub-menu to an existing menu
MENUITEMINFOW AddMenuItemSub(HMENU menu, HMENU subMenu, UINT menuItemID, const std::wstring& menuItemText, const SystemTrayIconData* data)
{
    // Create the menu item without inserting
    MENUITEMINFOW menuItem = AddMenuItem(menu, menuItemID, menuItemText, data, false);

    // Set the menu to be a part of a sub menu
    menuItem.fMask |= MIIM_SUBMENU;

    // Set the associated submenu
    menuItem.hSubMenu = subMenu;

    // Insert the menuitem into the parent menu
    WINCALL(InsertMenuItemW(menu, menuItem.wID, TRUE, &menuItem));

    return menuItem;
};


MENUITEMINFOW CreateProjectMenuItem(HMENU menu, HMENU subMenu, UINT menuItemID, const std::wstring& menuItemText)
{

    MENUITEMINFOW projectMenuItem = { 0 };
    projectMenuItem.cbSize = sizeof(projectMenuItem);

    projectMenuItem.fMask = MIIM_STRING | MIIM_SUBMENU | MIIM_ID;
    projectMenuItem.hSubMenu = subMenu;
    projectMenuItem.wID = menuItemID;

    projectMenuItem.dwTypeData = const_cast<wchar_t*>(menuItemText.c_str());
    projectMenuItem.cch = static_cast<UINT>(menuItemText.size());

    WINCALL(InsertMenuItemW(menu, projectMenuItem.wID, TRUE, &projectMenuItem));

    return projectMenuItem;
};



// Create the TrayIcon menu along with it's sub menus
HMENU CreateTrayIconMenu(std::vector<SystemTrayIconData*>* menuData)
{
    // DEMAND a context menu from windows
    HMENU menu = CreatePopupMenu();

    // For every project create a menu item and a sub menu option(s)
    int index = 100000;
    for (SystemTrayIconData* project : *menuData)
    {
        HMENU innerMenu = CreatePopupMenu();

        {
            MENUITEMINFOW menuItem = { 0 };
            menuItem.cbSize = sizeof(menuItem);

            menuItem.fMask = MIIM_STRING | MIIM_ID | MIIM_DATA;

            menuItem.fType = MFT_STRING;

            menuItem.wID = ++index;
            
            menuItem.dwTypeData = const_cast<wchar_t*>(L"Close");
            menuItem.cch = static_cast<UINT>(6);

            menuItem.dwItemData = reinterpret_cast<ULONG_PTR>(project);

            WINCALL(InsertMenuItemW(innerMenu, 68, FALSE, &menuItem));
        };


        {
            MENUITEMINFOW menuItem = { 0 };
            menuItem.cbSize = sizeof(menuItem);

            menuItem.fMask = MIIM_STRING | MIIM_ID | MIIM_DATA;

            menuItem.fType = MFT_STRING;

            // Increment the index by 1 and turn the number in the hundred's to a 1 
            menuItem.wID = ++(index += 10000);
            index -= 10000;

            menuItem.dwTypeData = const_cast<wchar_t*>(L"Run");
            menuItem.cch = static_cast<UINT>(4);

            menuItem.dwItemData = reinterpret_cast<ULONG_PTR>(project);

            WINCALL(InsertMenuItemW(innerMenu, 69, FALSE, &menuItem));
        };


        {
            MENUITEMINFOW menuItem = { 0 };
            menuItem.cbSize = sizeof(menuItem);

            menuItem.fMask = MIIM_STRING | MIIM_ID | MIIM_SUBMENU;

            menuItem.fType = MFT_STRING;

            menuItem.hSubMenu = innerMenu;

            menuItem.wID = static_cast<UINT>(MENUITEMID + (index - 1));

            menuItem.dwTypeData = const_cast<wchar_t*>(project->ProjectName);
            menuItem.cch = static_cast<UINT>(wcslen(project->ProjectName) + 1);

            WINCALL(InsertMenuItemW(menu, menuItem.wID, FALSE, &menuItem));
        };

        index += 100;
    };

    return menu;
}


// Show the menu "under" where the user's cursor is
int ShowTrayIconMenu(HWND hwnd, HMENU menu)
{
    // Windows being windows.
    // This function is a must if we want to close the menu when we click outside of it
    SetForegroundWindow(hwnd);

    // Get cursor position
    POINT cursorPoint;
    GetCursorPos(&cursorPoint);

    // Show the menu
    return TrackPopupMenu(menu, TPM_RETURNCMD | TPM_BOTTOMALIGN | TPM_LEFTALIGN, cursorPoint.x, cursorPoint.y, NULL, hwnd, NULL);
};


// Retrieves a menu items dwItemData as some type
template<class T>
T* GetMenuItemData(HMENU menu, UINT menuItemID)
{
    // The menu item's data 
    MENUITEMINFOW menuItem = { 0 };
    menuItem.cbSize = sizeof(menuItem);
    menuItem.fMask = MIIM_ID | MIIM_DATA;

    // Get the menu item's ifno
    GetMenuItemInfoW(menu, menuItemID, FALSE, &menuItem);

    // Cast dwItemData into a use-able type
    T* data = reinterpret_cast<T*>(menuItem.dwItemData);

    return data;
}


// Hanldes a returned result from TrackPopupMenu function
void HanldeMenuResult(HMENU menu, int result)
{
    if (result == 0)
        return;

    int index = (result / 100) % 100;
    bool runProject = (result / 10000) % 10;

    HMENU sub = GetSubMenu(menu, index);

    MENUITEMINFOW menuItem = { 0 };
    menuItem.cbSize = sizeof(menuItem);

    menuItem.fMask = MIIM_STRING | MIIM_ID | MIIM_DATA;
    GetMenuItemInfoW(sub, result, FALSE, &menuItem);

    SystemTrayIconData* data = reinterpret_cast<SystemTrayIconData*>(menuItem.dwItemData);

    // Run project
    if (runProject == true)
    {
        data->RunProjectCallBack(data->Data);
    }
    // Close project
    else
    {
        data->CloseProjectCallBack(data->Data);
    };

};