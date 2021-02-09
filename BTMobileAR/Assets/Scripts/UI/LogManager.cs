using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class LogManager : Singleton<LogManager>
{
    private System.Text.StringBuilder sb;
    private const string LogFilename = "Logfile.txt";

    public delegate void OnShowDebugInfo(StringBuilder sb);
    public static event OnShowDebugInfo ShowDebugInfo;

    // guarantee this will be always a singleton only - can't use the constructor!
    // Use this for initialization
    protected LogManager()
    {
        sb = new System.Text.StringBuilder(100);
    }

    public void AddLog(string slog)
    {
        sb.AppendFormat("{0: H:mm:ss}-{1}", DateTime.Now, slog);
        sb.AppendLine();
        if (ShowDebugInfo != null) // if event is used
            ShowDebugInfo(sb);
    }

    public void ClearLog()
    {
        sb = new System.Text.StringBuilder(100);
        if (ShowDebugInfo != null) // if event is used
            ShowDebugInfo(sb);
    }

    public void SaveLog()
    {
        try
        {
            string sfilename = string.Format("{0}//{1}", Application.persistentDataPath, LogFilename);
            FileInfo f = new FileInfo(sfilename);
            if (f.Exists)
                f.Delete();
            File.WriteAllText(sfilename, sb.ToString());
            AddLog(string.Format("Success Save - {0}", sfilename));
        }
        catch (Exception ex)
        {
            AddLog(string.Format("Error saving file - (0}", ex.Message));
        }
    }
}
