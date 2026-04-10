using System;
using System.IO;
using UnityEngine;
using SQLite;
using UnityEngine.Networking;
using System.Collections;

public class DatabaseManager : MonoBehaviour
{
    public static DatabaseManager Instance;

    private SQLiteConnection db;
    private object dbLock = new object();

    IEnumerator Start()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            yield return StartCoroutine(SetupDatabase());
        }
        else
        {
            Destroy(gameObject);
        }
    }

    IEnumerator SetupDatabase()
    {
        string dbName = "grammatag.db";

        string persistentPath = Path.Combine(Application.persistentDataPath, dbName);
        string streamingPath = Path.Combine(Application.streamingAssetsPath, dbName);

        if (!File.Exists(persistentPath))
        {
#if UNITY_ANDROID && !UNITY_EDITOR
        using (var www = UnityEngine.Networking.UnityWebRequest.Get(streamingPath))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityEngine.Networking.UnityWebRequest.Result.Success)
            {
                Debug.LogError("DB load failed: " + www.error);
            }
            else
            {
                File.WriteAllBytes(persistentPath, www.downloadHandler.data);
                Debug.Log("DB copied!");
            }
        }
#else
            File.Copy(streamingPath, persistentPath);
#endif
        }

        db = new SQLiteConnection(persistentPath);


        Debug.Log("DB Ready: " + persistentPath);

        yield return null; // 🔥 IMPORTANT
    }

    void OnApplicationQuit()
    {
        CloseDatabase();
    }

    void OnDestroy()
    {
        CloseDatabase();
    }

    void CloseDatabase()
    {
        if (db != null)
        {
            db.Close();
            db = null;
            Debug.Log("Database closed.");
        }
    }

    public void InsertUser(string first, string last, int age, string gender)
    {
        lock (dbLock)
        {
            var existingUser = db.Table<User>().FirstOrDefault();

            if (existingUser == null)
            {
                User user = new User
                {
                    FirstName = first,
                    LastName = last,
                    Age = age,
                    Gender = gender,
                    Coins = 0,
                    Hearts = 5,
                    lastHeartTime = DateTime.Now.ToString()
                };

                db.Insert(user);
            }
            else
            {
                existingUser.FirstName = first;
                existingUser.LastName = last;
                existingUser.Age = age;
                existingUser.Gender = gender;

                db.Update(existingUser);
            }
        }

        Debug.Log("User saved!");
    }

    public string GetPlayerName()
    {
        lock (dbLock)
        {
            var user = db.Table<User>().FirstOrDefault();
            return user != null ? user.FirstName : "Player";
        }
    }

    public int GetCoins()
    {
        lock (dbLock)
        {
            var user = db.Table<User>().FirstOrDefault();
            return user != null ? user.Coins : 0;
        }
    }

    public int GetHearts()
    {
        lock (dbLock)
        {
            var user = db.Table<User>().FirstOrDefault();
            return user != null ? user.Hearts : 5;
        }
    }

    public bool HasUser()
    {
        if (db == null)
        {
            Debug.LogWarning("DB not ready yet");
            return false;
        }

        lock (dbLock)
        {
            return db.Table<User>().FirstOrDefault() != null;
        }
    }


    public string GetUserGender()
    {
        if (db == null)
        {
            Debug.LogError("DB not ready!");
            return "Girl";
        }

        lock (dbLock)
        {
            try
            {
                var user = db.Table<User>().FirstOrDefault();

                if (user == null)
                {
                    Debug.LogWarning("No user found in DB!");
                    return "Girl";
                }

                return user.Gender;
            }
            catch (System.Exception e)
            {
                Debug.LogError("GetUserGender ERROR: " + e.Message);
                return "Girl";
            }
        }
    }

    public bool IsDatabaseReady()
    {
        return db != null;
    }


    public DateTime GetLastHeartTime()
    {
        lock (dbLock)
        {
            var user = db.Table<User>().FirstOrDefault();
            if (user != null && !string.IsNullOrEmpty(user.lastHeartTime))
            {
                return DateTime.Parse(user.lastHeartTime);
            }
            return DateTime.Now;
        }
    }

    public void UpdateHearts(int hearts, DateTime lastTime)
    {
        lock (dbLock)
        {
            var user = db.Table<User>().FirstOrDefault();

            if (user != null)
            {
                user.Hearts = hearts;
                user.lastHeartTime = lastTime.ToString();
                db.Update(user);
            }
        }
    }


    public void UpdateCoins(int coins)
    {
        lock (dbLock)
        {
            var user = db.Table<User>().FirstOrDefault();

            if (user != null)
            {
                user.Coins = coins;
                db.Update(user);
            }
        }

        Debug.Log("Coins updated: " + coins);
    }



}