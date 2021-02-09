using SQLite4Unity3d;
using System.Collections.Generic;
using UnityEngine;
#if !UNITY_EDITOR
using System.Collections;
using System.IO;
#endif

namespace Assets.Scripts.BT
{
    /// <summary>
    /// This class handles operations with SQLite database
    /// </summary>
    public class DataService
    {
        private SQLiteConnection _connection;
        public DataService(string DatabaseName)
        {

#if UNITY_EDITOR
            var dbPath = string.Format(@"Assets/StreamingAssets/{0}", DatabaseName);
#else
        // check if file exists in Application.persistentDataPath
        var filepath = string.Format("{0}/{1}", Application.persistentDataPath, DatabaseName);

        if (!File.Exists(filepath))
        {
            Debug.Log("Database not in Persistent path");
            // if it doesn't ->
            // open StreamingAssets directory and load the db ->

#if UNITY_ANDROID 
            var loadDb = new WWW("jar:file://" + Application.dataPath + "!/assets/" + DatabaseName);  // this is the path to your StreamingAssets in android
            while (!loadDb.isDone) { }  // CAREFUL here, for safety reasons you shouldn't let this while loop unattended, place a timer and error check
            // then save to Application.persistentDataPath
            File.WriteAllBytes(filepath, loadDb.bytes);
#elif UNITY_IOS
                 var loadDb = Application.dataPath + "/Raw/" + DatabaseName;  // this is the path to your StreamingAssets in iOS
                // then save to Application.persistentDataPath
                File.Copy(loadDb, filepath);
#elif UNITY_WP8
                var loadDb = Application.dataPath + "/StreamingAssets/" + DatabaseName;  // this is the path to your StreamingAssets in iOS
                // then save to Application.persistentDataPath
                File.Copy(loadDb, filepath);

#elif UNITY_WINRT
		var loadDb = Application.dataPath + "/StreamingAssets/" + DatabaseName;  // this is the path to your StreamingAssets in iOS
		// then save to Application.persistentDataPath
		File.Copy(loadDb, filepath);
		
#elif UNITY_STANDALONE_OSX
		var loadDb = Application.dataPath + "/Resources/Data/StreamingAssets/" + DatabaseName;  // this is the path to your StreamingAssets in iOS
		// then save to Application.persistentDataPath
		File.Copy(loadDb, filepath);
#else
	var loadDb = Application.dataPath + "/StreamingAssets/" + DatabaseName;  // this is the path to your StreamingAssets in iOS
	// then save to Application.persistentDataPath
	File.Copy(loadDb, filepath);

#endif

            Debug.Log("Database written");
        }

        var dbPath = filepath;
#endif
            _connection = new SQLiteConnection(dbPath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create);
            Debug.Log("Final PATH: " + dbPath);

        }


        public IEnumerable<FuzzyParts> GetFuzzyParts()
        {
            return _connection.Table<FuzzyParts>();
        }

        public IEnumerable<FuzzyValues> GetFuzzyValues()
        {
            return _connection.Table<FuzzyValues>();
        }

        public IEnumerable<FuzzyRules> GetFuzzyRules()
        {
            return _connection.Table<FuzzyRules>();
        }

        public IEnumerable<POIAddress> GetPOIAddress()
        {
            return _connection.Table<POIAddress>();
        }

        public POIAddress GetPoiAddressByID(int ID)
        {
            return _connection.Table<POIAddress>().Where(x => x.ID == ID).FirstOrDefault();
        }

        public void InsertPoiAddress(POIAddress adr)
        {
          var id =  _connection.Insert(adr);
        }
    }
}
