using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.IO;
using System;

public class Settings
{
    public bool DebugMode { get; set; }
    public int GPSRadius { get; set; }
    public bool SaveReversePOIToSQLite { get; set; }
    public bool FuzzyLogicFromSQLite { get; set; }

    public bool CreateWithoutPredefined { get; set; }


    public Settings()
    {
        GPSRadius = 15; // meters default value
        DebugMode = false;
        SaveReversePOIToSQLite = true;
        FuzzyLogicFromSQLite = true;
        CreateWithoutPredefined = true;
    }

    public static Settings ReadFromFile(string sfilename)
    {
        if (File.Exists(sfilename) == false)
            return new Settings();
        try
        {
            XmlDocument xDoc = new XmlDocument();
            string sdata = File.ReadAllText(sfilename);
            xDoc.LoadXml(sdata); // load XML to document
            Settings set = new Settings();
            string sdebug = ExtractValue(xDoc, "Settings/DebugMode");
            if (!string.IsNullOrEmpty(sdebug))
                set.DebugMode = bool.Parse(sdebug);
            string sradious = ExtractValue(xDoc, "Settings/Radius");
            if (!string.IsNullOrEmpty(sdebug))
                set.GPSRadius = int.Parse(sradious);
            string srev = ExtractValue(xDoc, "Settings/SaveReversePOI");
            if (!string.IsNullOrEmpty(srev))
                set.SaveReversePOIToSQLite = bool.Parse(srev);
            string spref = ExtractValue(xDoc, "Settings/CreateWithoutPredefined");
            if (!string.IsNullOrEmpty(spref))
                set.CreateWithoutPredefined = bool.Parse(spref);
            string sfuzzy = ExtractValue(xDoc, "Settings/FuzzyLogicFromSQLite");
            if (!string.IsNullOrEmpty(sfuzzy))
                set.FuzzyLogicFromSQLite = bool.Parse(sfuzzy);

            return set;
        }
        catch (Exception ex)
        {
            LogManager.Instance.AddLog(string.Format("Error read settings! {0}", ex.Message));
            return new Settings();
        }
    }


    private static string ExtractValue(XmlDocument xDoc, string xPath)
    {
        XmlNode xnode = xDoc.SelectSingleNode(xPath);
        if (xnode == null) return null;
        return xnode.InnerXml;
    }

    public void SaveSettingsToFile(string sfilename)
    {
        try
        {
            XmlDocument xmlDoc = new XmlDocument();
            XmlElement root = xmlDoc.CreateElement("Settings");
            xmlDoc.AppendChild(root);
            // DebugMode
            XmlElement xdebug = xmlDoc.CreateElement("DebugMode");
            xdebug.InnerText = DebugMode.ToString();
            root.AppendChild(xdebug);

            XmlElement radious = xmlDoc.CreateElement("Radius");
            radious.InnerText = GPSRadius.ToString();
            root.AppendChild(radious);

            XmlElement saveRev = xmlDoc.CreateElement("SaveReversePOI");
            saveRev.InnerText = SaveReversePOIToSQLite.ToString();
            root.AppendChild(saveRev);

            XmlElement fuzzy = xmlDoc.CreateElement("FuzzyLogicFromSQLite");
            fuzzy.InnerText = FuzzyLogicFromSQLite.ToString();
            root.AppendChild(fuzzy);

            XmlElement spref = xmlDoc.CreateElement("CreateWithoutPredefined");
            spref.InnerText = CreateWithoutPredefined.ToString();
            root.AppendChild(spref);

            // now save the XML to file
            string sxml = xmlDoc.InnerXml;
            FileInfo f = new FileInfo(sfilename);
            if (f.Exists)
                f.Delete();
            File.WriteAllText(sfilename, sxml);
        }
        catch (Exception ex)
        {
            LogManager.Instance.AddLog(string.Format("Error save settings! {0}", ex.Message));
            return;
        }
    }
}
