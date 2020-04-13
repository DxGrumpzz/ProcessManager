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
MENUITEMINFOW CreateMenuItem(UINT menuItemID, const wchar_t* menuItemText, const SystemTrayIconData* data)
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
    menuItem.dwTypeData = const_cast<wchar_t*>(menuItemText);
    menuItem.cch = static_cast<UINT>(wcslen(menuItemText) + 1);

    menuItem.dwItemData = reinterpret_cast<ULONG_PTR>(data);

    return menuItem;
}

// Adds a sub-menu to an existing menu
MENUITEMINFOW AddMenuItemSub(HMENU subMenu, UINT menuItemID, const wchar_t* menuItemText, const SystemTrayIconData* data)
{
    // Create the menu item without inserting
    MENUITEMINFOW menuItem = CreateMenuItem(menuItemID, menuItemText, data);

    // Set the menu to be a part of a sub menu
    menuItem.fMask |= MIIM_SUBMENU;

    // Set the associated submenu
    menuItem.hSubMenu = subMenu;

    return menuItem;
};




// Create the TrayIcon menu along with it's sub menus
HMENU CreateTrayIconMenu(std::vector<SystemTrayIconData*>* menuData)
{
    // DEMAND a context menu from windows
    HMENU menu = CreatePopupMenu();


    // For every project create a menu item and a sub menu option(s)

    // Because for every project there will be an inner menu that runs/closes the project
    // simply getting the clicked menu' ID isn't enough.
    // So we some number magic like this:
    // the number 1***** will contain the menu item's index, the menu item's position in the top menu item, and if the option is to run or close the project.
    // The number in the ten thousands spot will be set to 1 or 0 to indicate if to run or close the project
    // The numbers in the thousands and hundreds spot(s) will contain the menu item position
    // The numbers in the tens and ones spot(s) will be used as an accumulator to assure no duplicates
    int index = 100000;
    
    for (SystemTrayIconData* project : *menuData)
    {
        HMENU innerMenu = CreatePopupMenu();
   
        // Add the close and run menu options
        MENUITEMINFOW closeProjectMenuItem = CreateMenuItem(++index, L"Close", project);
        WINCALL(InsertMenuItemW(innerMenu, 68, FALSE, &closeProjectMenuItem));
       
        // Add a 1 in the ten thousands place and remove after creating the menu
        MENUITEMINFOW runProjectMenuItem = CreateMenuItem(++(index += 10000), L"Run", project);
        WINCALL(InsertMenuItemW(innerMenu, 69, FALSE, &runProjectMenuItem));
        index -= 10000;

        // Create the main menu item 
        MENUITEMINFOW projectMenu = AddMenuItemSub(innerMenu, MENUITEMID + (index - 1), project->ProjectName, project);
        WINCALL(InsertMenuItemW(menu, 69, FALSE, &projectMenu));

        // Increment the index to the next project menu item sub menu
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
inline SystemTrayIconData* GetMenuItemData(HMENU menu, UINT menuItemID)
{
    // The menu item's data 
    MENUITEMINFOW menuItem = { 0 };
    menuItem.cbSize = sizeof(menuItem);
    menuItem.fMask = MIIM_ID | MIIM_DATA;

    // Get the menu item's ifno
    GetMenuItemInfoW(menu, menuItemID, FALSE, &menuItem);

    // Cast dwItemData into a use-able type
    //T* data = reinterpret_cast<T*>(menuItem.dwItemData);
    SystemTrayIconData* data = reinterpret_cast<SystemTrayIconData*>(menuItem.dwItemData);
    
    return data;
}


// Hanldes a returned result from TrackPopupMenu function
void HanldeMenuResult(HMENU menu, int result)
{
    if (result == 0)
        return;

    // Get the index for which menu item the selected option belongs to
    int index = (result / 100) % 100;

    // Get the bool result for running or closing the project
    bool runProject = (result / 10000) % 10;

    // Get the sub menu from index
    HMENU sub = GetSubMenu(menu, index);
    
    // Get data associated with the menu item
    SystemTrayIconData* data = GetMenuItemData(sub, result);

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