using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    private Settings msettings;
    private string SettingsFilename;
    private int Counter = 1500;
    protected GameManager()
    {
        SettingsFilename = null;
        msettings = null;
    }

    // public read only property
    public Settings Setting
    {
        get
        {
            if (msettings == null)
            {
                if (SettingsFilename == null)
                    SettingsFilename = string.Format("{0}//{1}", Application.persistentDataPath, "Settings.xml");
                msettings = Settings.ReadFromFile(SettingsFilename);
            }
            return msettings;
        }
    }

    public void SaveSettings()
    {
        Setting.SaveSettingsToFile(SettingsFilename);
    }

    public int GetNextCounterValue()
    {
        return ++Counter;
    }
}
