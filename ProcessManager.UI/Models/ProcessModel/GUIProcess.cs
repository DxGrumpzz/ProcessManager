﻿namespace ProcessManager.UI
{
    using System;


    /// <summary>
    /// 
    /// </summary>
    public class GUIProcess : ProcessBase
    {
        public override bool IsRunning => ProcessRunning(_processPointer);

        public override event Action<IProcessModel> ProcessClosedCallback;
        public override event Action<IProcessModel> ProcessInitializedCallback;


        /// <summary>
        /// A string contaning the path to the process
        /// </summary>
        public string ProcessPath { get; }

        /// <summary>
        /// A string contaning the process arguments 
        /// </summary>
        public string ProcessArgs { get; }


        public GUIProcess(string processPath, string processArgs = "", bool visibleOnStartup = true)
        {
            ProcessPath = processPath;
            ProcessArgs = processArgs;

            _processClosedCallback = (IntPtr process) =>
            {
                ProcessClosedCallback?.Invoke(this);
            };

            _processInitialziedCallback = (IntPtr process) =>
            {
                ProcessInitializedCallback?.Invoke(this);
            };

            CreateProcessObject(processPath, processArgs, null, null, false, visibleOnStartup, _processClosedCallback, _processInitialziedCallback, ref _processPointer);
        }


        ~GUIProcess()
        {
            DestroyProcessObject(ref _processPointer);
        }

    };
};
