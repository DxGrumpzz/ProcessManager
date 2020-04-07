#pragma once


struct SystemTrayIconData
{
    wchar_t* ProjectName;
    void* Data;

    void (*CloseProjectCallBack)(void* data);
    void (*RunProjectCallBack)(void* data);
};
