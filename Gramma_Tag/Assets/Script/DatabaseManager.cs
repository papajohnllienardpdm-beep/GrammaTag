using SQLite;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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

        string persistentPath = Path.Combine(Application.persistentDataPath, dbName).ToLower();
        string streamingPath = Path.Combine(Application.streamingAssetsPath, dbName);

        Debug.Log("📂 Persistent Path: " + persistentPath);
        Debug.Log("📦 Streaming Path: " + streamingPath);

        // ✅ FRESH INSTALL CHECK
        string installFlagKey = "db_installed_v1";
        bool isFreshInstall = !PlayerPrefs.HasKey(installFlagKey);

        if (isFreshInstall && File.Exists(persistentPath))
        {
            Debug.Log("🆕 FRESH INSTALL DETECTED — Deleting old persistent DB...");

            // ✅ CLOSE FIRST bago i-delete (para sa Unity Editor)
            if (db != null)
            {
                db.Close();
                db = null;
                Debug.Log("🔒 Old DB connection closed before delete.");
            }

            try
            {
                File.Delete(persistentPath);
                Debug.Log("🗑️ Old DB deleted.");
            }
            catch (Exception e)
            {
                Debug.LogWarning("⚠️ Could not delete old DB: " + e.Message + " — Skipping delete, using existing.");
                // ✅ HINDI mag-break — ituloy na lang gamit yung existing DB
                // I-clear lang yung flag para sa susunod na try
                PlayerPrefs.DeleteKey(installFlagKey);
                PlayerPrefs.Save();
            }
        }

#if UNITY_ANDROID && !UNITY_EDITOR
    string wrongCasePath = Path.Combine(Application.persistentDataPath, "GrammaTag.db");

    if (File.Exists(wrongCasePath) && !File.Exists(persistentPath))
    {
        Debug.Log("⚠️ Wrong case DB detected. Renaming...");
        File.Move(wrongCasePath, persistentPath);
        Debug.Log("✅ DB renamed to lowercase.");
    }
#endif

        // ✅ CREATE DATABASE IF NOT EXIST
        if (!File.Exists(persistentPath))
        {
            Debug.Log("🔥 DATABASE NOT FOUND. CREATING...");

#if UNITY_ANDROID && !UNITY_EDITOR
        UnityWebRequest www = UnityWebRequest.Get(streamingPath);

        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("❌ DB COPY FAILED: " + www.error);
            yield break;
        }

        try
        {
            File.WriteAllBytes(persistentPath, www.downloadHandler.data);
            Debug.Log("✅ DATABASE CREATED (ANDROID)");
        }
        catch (Exception e)
        {
            Debug.LogError("❌ WRITE ERROR: " + e.Message);
            yield break;
        }
#else
            try
            {
                File.Copy(streamingPath, persistentPath, true);
                Debug.Log("✅ DATABASE CREATED (EDITOR)");
            }
            catch (Exception e)
            {
                Debug.LogError("❌ COPY ERROR: " + e.Message);
                yield break;
            }
#endif
        }
        else
        {
            Debug.Log("ℹ️ EXISTING DATABASE FOUND");
        }

        // ✅ FINAL FILE CHECK
        if (!File.Exists(persistentPath))
        {
            Debug.LogError("❌ DATABASE STILL MISSING");
            yield break;
        }

        // ✅ OPEN SQLITE DATABASE
        try
        {
            db = new SQLiteConnection(persistentPath);
            Debug.Log("✅ DATABASE OPENED SUCCESSFULLY");
        }
        catch (Exception e)
        {
            Debug.LogError("❌ SQLITE INIT ERROR: " + e.Message);
            yield break;
        }

        // ✅ MARK AS INSTALLED (after successful open)
        PlayerPrefs.SetInt(installFlagKey, 1);
        PlayerPrefs.Save();

        isReady = true;

        Debug.Log("🔥 DATABASE READY");

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
        if (db == null)
        {
            Debug.LogError("❌ DB NULL");
            return;
        }

        lock (dbLock)
        {
            try
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

                    Debug.Log("✅ NEW USER INSERTED");
                }
                else
                {
                    existingUser.FirstName = first;
                    existingUser.LastName = last;
                    existingUser.Age = age;
                    existingUser.Gender = gender;

                    db.Update(existingUser);

                    Debug.Log("✅ EXISTING USER UPDATED");
                }
            }
            catch (Exception e)
            {
                Debug.LogError("❌ INSERT USER ERROR: " + e.Message);
            }
        }
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
            Debug.LogWarning("⚠️ DB NOT READY");
            return false;
        }

        lock (dbLock)
        {
            try
            {
                int count = db.Table<User>().Count();

                Debug.Log("👤 USER COUNT: " + count);

                return count > 0;
            }
            catch (Exception e)
            {
                Debug.LogError("❌ HASUSER ERROR: " + e.Message);
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
                int oldPassed = existing[0].isPassed;

                // ✅ KEEP HIGHEST SCORE
                if (score > oldScore)
                {
                    existing[0].Score = score;
                    existing[0].Stars = stars;
                }

                // ✅ ONCE PASSED = ALWAYS PASSED
                if (isPassed == 1)
                {
                    existing[0].isPassed = 1;
                }

                db.Update(existing[0]);
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
    //dsadjhasgkjdhsajsaldksa
    public void DeductHeartSafe(int amount)
    {
        if (db == null)
        {
            Debug.LogWarning("⚠️ DB not ready, DeductHeart skipped");
            return;
        }

        lock (dbLock)
        {
            var user = db.Table<User>().FirstOrDefault();

            if (user == null) return;

            int newHearts = user.Hearts - amount;

            if (newHearts < 0)
                newHearts = 0;

            user.Hearts = newHearts;
            user.lastHeartTime = DateTime.Now.ToString();

            db.Update(user);

            Debug.Log("❤️ Hearts after deduction: " + user.Hearts);
        }
    }

    // 🪙 Reward system
    public int GiveCoins(int moduleID, int score, int passed)
    {
        lock (dbLock)
        {
            var user = db.Table<User>().FirstOrDefault();

            if (user == null)
                return 0;

            int reward = 0;

            // ✅ CHECK EXISTING PROGRESS
            var existing = db.Query<Progress>(
                "SELECT * FROM Progress WHERE UserID = ? AND ModuleID = ?",
                user.UserID,
                moduleID
            );

            // ✅ FAILED
            if (passed == 0)
            {
                reward = 50;

                Debug.Log("❌ FAILED REWARD");
            }
            else
            {
                bool alreadyPassedBefore = false;

                if (existing.Count > 0)
                {
                    alreadyPassedBefore = existing[0].isPassed == 1;
                }

                // ✅ FIRST TIME PASS
                if (!alreadyPassedBefore)
                {
                    reward = 200;

                    Debug.Log("🎉 FIRST TIME PASS");
                }
                else
                {
                    // ✅ REPEAT PASS
                    reward = 20;

                    Debug.Log("🔁 REPEAT PASS");
                }
            }

            // ✅ DOUBLE COIN
            if (IsPowerUpActive("DoubleCoin"))
            {
                reward *= 2;

                Debug.Log("💰 DOUBLE COIN APPLIED!");

                DeactivatePowerUp("DoubleCoin");
            }

            // ✅ ADD COINS
            user.Coins += reward;

            db.Update(user);

            Debug.Log("🪙 FINAL REWARD: " + reward);

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

