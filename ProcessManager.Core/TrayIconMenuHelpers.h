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


// A result that is returned from a user's interaction with the Tray icon menu
enum class TrayIconMenuResult : UINT
{
    // The user exited the menu
    MenuClosed = 0,

    // The user decided to close the current project
    CloseProject = MENUITEMID,

    // The user decided to run the current project
    RunProject = MENUITEMID + 1,
};

// Adds a Menu item to a menu
MENUITEMINFOW AddMenuItem(HMENU menu, UINT menuItemID, const std::wstring& menuItemText, const SystemTrayIconData* data, bool insertMenuItem = true)
{
    // Create the menu
    MENUITEMINFOW menuItem;
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
    WINCALL(InsertMenuItemW(menu, menuItem.wID, FALSE, &menuItem));

    return menuItem;
}


// Create the TrayIcon menu along with it's sub menus
HMENU CreateTrayIconMenu(std::vector<SystemTrayIconData*>* menuData)
{
    // DEMAND a context menu from windows
    HMENU menu = CreatePopupMenu();

    // For every project create a menu item and a sub menu option(s)
    int index = 0;
    for (SystemTrayIconData* project : *menuData)
    {
        HMENU innerMenu = CreatePopupMenu();

        // Close project menu item
        AddMenuItem(innerMenu, static_cast<UINT>(TrayIconMenuResult::CloseProject), L"Close project", project);

        // Run project menu item
        AddMenuItem(innerMenu, static_cast<UINT>(TrayIconMenuResult::RunProject), L"Run project", project);

        // Create the main menu
        AddMenuItemSub(menu, innerMenu, MENUITEMID + index, project->ProjectName, project);

        index++;
    };

    return menu;
}


// Show the menu "under" where the user's cursor is
TrayIconMenuResult ShowTrayIconMenu(HWND hwnd, HMENU menu)
{
    // Windows being windows.
    // This function is a must if we want to close the menu when we click outside of it
    SetForegroundWindow(hwnd);

    // Get cursor position
    POINT cursorPoint;
    GetCursorPos(&cursorPoint);

    // Show the menu
    return static_cast<TrayIconMenuResult>(TrackPopupMenu(menu, TPM_RETURNCMD | TPM_BOTTOMALIGN | TPM_LEFTALIGN, cursorPoint.x, cursorPoint.y, NULL, hwnd, NULL));
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
void HanldeMenuResult(HMENU menu, TrayIconMenuResult result)
{
    // Don't do anything if user closed the menu
    if (result != TrayIconMenuResult::MenuClosed)
    {
        // The clicked menu item's ID
        UINT menuItemID = static_cast<UINT>(result);

        // If user chose to run the project
        if (result == TrayIconMenuResult::RunProject)
        {
            // Get project data from menu item
            SystemTrayIconData* project = GetMenuItemData<SystemTrayIconData>(menu, menuItemID);

            // Run the project
            project->RunProjectCallBack(project->Data);
        }
        else if (result == TrayIconMenuResult::CloseProject)
        {
            // Get project data from menu item
            SystemTrayIconData* project = GetMenuItemData<SystemTrayIconData>(menu, menuItemID);// menuItem.dwItemData);

            // Close the project
            project->CloseProjectCallBack(project->Data);
        };
    };
};


