using System;
using System.IO;
using UnityEngine;
using SQLite;

public class DatabaseManager : MonoBehaviour
{
    public static DatabaseManager Instance;

    private SQLiteConnection db;
    private object dbLock = new object();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SetupDatabase();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void SetupDatabase()
    {
        string dbName = "grammatag.db";

        string persistentPath = Path.Combine(Application.persistentDataPath, dbName);
        string streamingPath = Path.Combine(Application.streamingAssetsPath, dbName);

        // 🔥 COPY DB FIRST TIME ONLY
        if (!File.Exists(persistentPath))
        {
            Debug.Log("Copying database from StreamingAssets...");

            File.Copy(streamingPath, persistentPath);
        }

        // 🔥 OPEN DB
        db = new SQLiteConnection(persistentPath);

        db.CreateTable<LearnerProfile>();

        Debug.Log("DB Ready: " + persistentPath);
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
        int retries = 3;

        while (retries > 0)
        {
            try
            {
                lock (dbLock)
                {
                    var existingUser = db.Table<LearnerProfile>().FirstOrDefault();

                    if (existingUser == null)
                    {
                        LearnerProfile user = new LearnerProfile
                        {
                            firstName = first,
                            lastName = last,
                            age = age,
                            gender = gender,
                            coins = 0,
                            hearts = 5,
                            lastHeartTime = DateTime.Now.ToString()
                        };

                        db.Insert(user);
                    }
                    else
                    {
                        existingUser.firstName = first;
                        existingUser.lastName = last;
                        existingUser.age = age;
                        existingUser.gender = gender;

                        db.Update(existingUser);
                    }
                }

                Debug.Log("User saved!");
                return;
            }
            catch (SQLiteException e)
            {
                if (e.Message.Contains("locked"))
                {
                    Debug.LogWarning("DB locked, retrying...");
                    System.Threading.Thread.Sleep(100);
                    retries--;
                }
                else
                {
                    throw;
                }
            }
        }

        Debug.LogError("Failed to write to DB after retries.");
    }

    public string GetPlayerName()
    {
        lock (dbLock)
        {
            var user = db.Table<LearnerProfile>().FirstOrDefault();
            return user != null ? user.firstName : "Player";
        }
    }

    public int GetCoins()
    {
        lock (dbLock)
        {
            var user = db.Table<LearnerProfile>().FirstOrDefault();
            return user != null ? user.coins : 0;
        }
    }

    public int GetHearts()
    {
        lock (dbLock)
        {
            var user = db.Table<LearnerProfile>().FirstOrDefault();
            return user != null ? user.hearts : 5;
        }
    }
}