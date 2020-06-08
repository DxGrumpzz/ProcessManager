#pragma once
#include "ProcessModel.h"


#define DLL_CALL extern "C" __declspec(dllexport) 


// Creates an instance of ProcessModels and 'Outs' it's pointer
DLL_CALL void CreateProcessObject(const wchar_t* processPath, const wchar_t* processArgs,
                                  const wchar_t* consoleScript, const wchar_t* startupDirectory,
                                  ProcessModel::ProcessTypeEnum processType,
                                  bool visibleOnStartup,
                                  ProcessClosedCallBack processClosedCallback,
                                  ProcessClosedCallBack processInitialziedCallback,
                                  ProcessModel*& process)
{
    process = new ProcessModel();

    process->ProcessType = processType;
    process->VisibleOnStartup = visibleOnStartup;


    switch (process->ProcessType)
    {
        case ProcessModel::ProcessTypeEnum::ConsoleProcess:
        {
            process->ConsoleScript = consoleScript;
            process->StartupDirectory = startupDirectory;
            break;
        };

        case ProcessModel::ProcessTypeEnum::GUIProcess:
        {
            process->ProcessPath = processPath;
            process->ProcessArgs = processArgs;
        };
    };

    process->ProcessInitializedCallback = processInitialziedCallback;
    process->ProcessClosedCallback = processClosedCallback;
};




// Frees memory used by a created ProcessModel
DLL_CALL void DestroyProcessObject(ProcessModel*& process)
{
    delete process;
    process = nullptr;
};



DLL_CALL bool RunProcess(ProcessModel* process)
{
    return process->RunProcess();
};

DLL_CALL bool CloseProcess(ProcessModel* process)
{
    bool result = process->CloseProcess();

    return result;
};


DLL_CALL bool ProcessRunning(ProcessModel* process)
{
    return process->IsRunning();
};


DLL_CALL bool ShowProcessWindow(ProcessModel* process)
{
    return process->ShowProcessWindow();
};

DLL_CALL bool HideProcessWindow(ProcessModel* process)
{
    return process->HideProcessWindow();
};