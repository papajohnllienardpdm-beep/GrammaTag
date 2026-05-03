using SQLite;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

public class DatabaseManager : MonoBehaviour
{
    public static DatabaseManager Instance;

    private SQLiteConnection db;
    private object dbLock = new object();
    private bool isReady = false;


    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    IEnumerator Start()
    {
        yield return StartCoroutine(SetupDatabase());
    }




    IEnumerator SetupDatabase()
    {
        string dbName = "grammatag.db";

        string persistentPath = Path.Combine(Application.persistentDataPath, dbName);
        string streamingPath = Application.streamingAssetsPath + "/" + dbName;

        Debug.Log("📂 Persistent Path: " + persistentPath);
        Debug.Log("📦 Streaming Path: " + streamingPath);


        /////////////dsadsagdujhgsadgkjsagdkjgasdkjgsakjdg
        ///djhsakjgdkjsadkjasgkdjasgd
        ///dhsadhsahdkas
        ///dsaldhlsahdlksa
        ///dksajhdhjlsadlk
        ///dpksajdpohjsajhd


        // 🔥 FIRST INSTALL CHECK
        bool isFirstRun = PlayerPrefs.GetInt("DB_INITIALIZED", 0) == 0;

        if (isFirstRun)
        {
            Debug.Log("🔥 FIRST RUN DETECTED");

#if UNITY_ANDROID && !UNITY_EDITOR
    // 🔥 SAFE LANG SA ANDROID
    if (File.Exists(persistentPath))
    {
        try
        {
            File.Delete(persistentPath);
            Debug.Log("🧹 OLD DB DELETED (ANDROID)");
        }
        catch (Exception e)
        {
            Debug.LogError("❌ DELETE FAILED: " + e.Message);
        }
    }
#else
            // ❗ UNITY EDITOR → HUWAG MAG DELETE (CAUSE NG ERROR MO)
            Debug.Log("⚠️ UNITY EDITOR → SKIP DELETE");
#endif

            PlayerPrefs.SetInt("DB_INITIALIZED", 1);
            PlayerPrefs.Save();
        }

        // 🔥 COPY DB ONLY IF NOT EXISTS
        if (!File.Exists(persistentPath))
        {
#if UNITY_ANDROID && !UNITY_EDITOR
        UnityWebRequest www = UnityWebRequest.Get(streamingPath);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("❌ DB load failed: " + www.error);
            yield break;
        }

        File.WriteAllBytes(persistentPath, www.downloadHandler.data);
        Debug.Log("✅ DB copied (Android)");
#else
            if (!File.Exists(persistentPath))
            {
                File.Copy(streamingPath, persistentPath);
                Debug.Log("✅ DB copied (Editor)");
            }
            else
            {
                Debug.Log("ℹ️ DB already exists (Editor)");
            }
            Debug.Log("✅ DB copied (Editor)");
#endif
        }
        else
        {
            Debug.Log("ℹ️ DB already exists, skipping copy");
        }

        // 🔥 OPEN DB
        try
        {
            db = new SQLiteConnection(persistentPath);
            Debug.Log("✅ DB OPENED");
        }
        catch (Exception e)
        {
            Debug.LogError("❌ DB INIT ERROR: " + e.Message);
            yield break;
        }

        isReady = true;
        Debug.Log("🔥 DB READY FLAG SET");

        yield return null;
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

        // 🔥 IMPORTANT
        PlayerPrefs.SetInt("HAS_LOGGED_IN", 1);
        PlayerPrefs.Save();

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
            var user = db.Table<User>().FirstOrDefault();

            if (user != null)
            {
                Debug.Log("👤 USER FOUND: " + user.FirstName);
                return true;
            }
            else
            {
                Debug.Log("❌ NO USER FOUND");
                return false;
            }
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
        return isReady && db != null;
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
        if (db == null)
        {
            Debug.LogWarning("⚠️ UpdateHearts skipped: DB is null");
            return;
        }

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


    public bool HasPowerUp(string type)
    {
        lock (dbLock)
        {
            var result = db.Query<UserPowerUps>(
                "SELECT * FROM UserPowerUps WHERE PowerUpType = ?", type);

            return result.Count > 0;
        }
    }

    public bool IsPowerUpActive(string type)
    {
        lock (dbLock)
        {
            var result = db.Query<UserPowerUps>(
                "SELECT * FROM UserPowerUps WHERE PowerUpType = ?", type);

            if (result.Count > 0)
            {
                return result[0].isActive == 1;
            }

            return false;
        }
    }

    public void ActivatePowerUp(string type)
    {
        lock (dbLock)
        {
            var result = db.Query<UserPowerUps>(
                "SELECT * FROM UserPowerUps WHERE PowerUpType = ?", type);

            if (result.Count == 0)
            {
                // FIRST TIME → INSERT
                db.Execute(
                    "INSERT INTO UserPowerUps (UserID, PowerUpType, isActive) VALUES (?, ?, ?)",
                    1, type, 1
                );
            }
            else
            {
                // REUSE → UPDATE
                db.Execute(
                    "UPDATE UserPowerUps SET isActive = 1 WHERE PowerUpType = ?",
                    type
                );
            }
        }

        Debug.Log("PowerUp Activated: " + type);
    }

    public void DeactivatePowerUp(string type)
    {
        lock (dbLock)
        {
            db.Execute(
                "UPDATE UserPowerUps SET isActive = 0 WHERE PowerUpType = ?",
                type
            );
        }
    }




    public List<DBQuestion> GetQuestionsByModule(int moduleID)
    {
        lock (dbLock)
        {
            return db.Query<DBQuestion>(
                "SELECT * FROM AssessmentItems WHERE ModuleID = ?", moduleID
            );
        }
    }



    public void SaveProgressBetter(int userID, int moduleID, int score, int isPassed, int stars)
    {
        if (db == null)
        {
            Debug.LogError("❌ DB NOT READY! SaveProgress skipped.");
            return;
        }

        lock (dbLock)
        {
            var existing = db.Query<Progress>(
                "SELECT * FROM Progress WHERE UserID = ? AND ModuleID = ?",
                userID, moduleID
            );

            if (existing.Count > 0)
            {
                int oldScore = existing[0].Score;

                if (score > oldScore)
                {
                    db.Execute(
                        "UPDATE Progress SET Score=?, isPassed=?, Stars=? WHERE UserID=? AND ModuleID=?",
                        score, isPassed, stars, userID, moduleID
                    );
                }
            }
            else
            {
                db.Execute(
                    "INSERT INTO Progress (UserID, ModuleID, Score, isPassed, Stars) VALUES (?, ?, ?, ?, ?)",
                    userID, moduleID, score, isPassed, stars
                );
            }
        }

        // 🔥 AUTO UNLOCK
        if (isPassed == 1)
        {
            int nextModule = moduleID + 1;

            if (PlayerPrefsManager.Instance != null)
            {
                PlayerPrefsManager.Instance.UnlockModule(nextModule);
            }

            int realUserID = GetUserID();
            SaveAchievement(realUserID, moduleID);
        }
    }

    //--------------------------------------------------------------------------------------------------

    public int GetUserID()
    {
        lock (dbLock)
        {
            var user = db.Table<User>().FirstOrDefault();
            return user != null ? user.UserID : 1;
        }
    }

    // ❤️ Bawas heart pag pasok sa game
    public void DeductHeart()
    {
        lock (dbLock)
        {
            var user = db.Table<User>().FirstOrDefault();

            if (user != null && user.Hearts > 0)
            {
                user.Hearts -= 1;
                user.lastHeartTime = DateTime.Now.ToString();
                db.Update(user);

                Debug.Log("Heart deducted! Remaining: " + user.Hearts);
            }
            else
            {
                Debug.Log("No hearts left!");
            }
        }
    }

    // 🪙 Reward system
    public int GiveCoins(int moduleID, int score, int passed)
    {
        lock (dbLock)
        {
            var user = db.Table<User>().FirstOrDefault();

            if (user == null) return 0;

            var existing = db.Query<Progress>(
                "SELECT * FROM Progress WHERE UserID = ? AND ModuleID = ?",
                user.UserID, moduleID
            );

            int reward = 0;

            if (passed == 1)
            {
                if (existing.Count == 0)
                {
                    reward = 200;
                }
                else
                {
                    reward = 50;
                }
            }
            else
            {
                reward = 20;
            }

            // 🔥 DOUBLE COIN HERE
            if (IsPowerUpActive("DoubleCoin"))
            {
                reward *= 2;
                Debug.Log("💰 DOUBLE COIN APPLIED!");

                DeactivatePowerUp("DoubleCoin");
            }

            user.Coins += reward;
            db.Update(user);

            Debug.Log("Coins Earned: " + reward);
            return reward;
        }
    }


    public bool IsSubtopicPassed(int moduleID)
    {
        lock (dbLock)
        {
            var result = db.Query<Progress>(
                "SELECT * FROM Progress WHERE ModuleID = ? AND isPassed = 1",
                moduleID
            );

            return result.Count > 0;
        }
    }


    public Modules GetModuleData(int moduleID)
    {
        lock (dbLock)
        {
            return db.Table<Modules>()
                     .Where(m => m.ModuleID == moduleID)
                     .FirstOrDefault();
        }
    }


    public string GetModuleOverview(int moduleID)
    {
        lock (dbLock)
        {
            var module = db.Table<Modules>()
                           .Where(m => m.ModuleID == moduleID)
                           .FirstOrDefault();

            return module != null ? module.Overview : "No Overview Available";
        }
    }

    public void SaveAchievement(int userID, int moduleID)
    {
        lock (dbLock)
        {
            // 🔍 CHECK kung meron na (avoid duplicate)
            var existing = db.Query<Achievement>(
                "SELECT * FROM Achievement WHERE UserID = ? AND ModuleID = ?",
                userID, moduleID
            );

            if (existing.Count == 0)
            {
                Achievement newAch = new Achievement
                {
                    UserID = userID,
                    ModuleID = moduleID,
                    dateEarned = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                };

                db.Insert(newAch);

                Debug.Log("🏆 Achievement SAVED → Module " + moduleID);
            }
            else
            {
                Debug.Log("⚠️ Achievement already exists → skip");
            }
        }
    }

    public List<CertificateData> GetCertificateData(int userID)
    {
        lock (dbLock)
        {
            return db.Query<CertificateData>(
                @"SELECT m.ModuleID, m.ModuleName, 
                     IFNULL(p.Score, 0) as Score, 
                     IFNULL(p.Stars, 0) as Stars
              FROM Modules m
              LEFT JOIN Progress p 
              ON m.ModuleID = p.ModuleID AND p.UserID = ?
              ORDER BY m.ModuleID ASC",
                userID
            );
        }
    }


    public User GetUserData()
    {
        lock (dbLock)
        {
            return db.Table<User>().FirstOrDefault();
        }
    }

    public Achievement GetAchievement(int userID, int moduleID)
    {
        lock (dbLock)
        {
            return db.Query<Achievement>(
                "SELECT * FROM Achievement WHERE UserID = ? AND ModuleID = ?",
                userID, moduleID
            ).FirstOrDefault();
        }
    }

}

