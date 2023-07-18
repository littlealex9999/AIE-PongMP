using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class AppHelper
{
    Process proc;
    public bool isRunning { get { return !proc.HasExited; } }
    public bool hasExited { get { return proc.HasExited; } }
    public int getExitCode { get { return proc.ExitCode; } }

    public AppHelper(string path)
    {
        proc = Process.Start(path);
    }

    /// <summary>
    /// Kills the application and returns its exit code
    /// </summary>
    /// <returns></returns>
    public int KillApp()
    {
        proc.Kill();
        return proc.ExitCode;
    }
}
