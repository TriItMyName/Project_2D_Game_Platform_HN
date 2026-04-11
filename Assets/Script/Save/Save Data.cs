using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;

public class SaveData
{
    private static SaveData _instance;
    public static SaveData Instance
    {
        get
        {
            if (_instance == null) _instance = new SaveData();
            return _instance;
        }
    }

    // Map & Scene
    public HashSet<string> sceneNames = new HashSet<string>();

    // Bench Stuff (Điểm hồi sinh)
    public string benchSceneName;
    public Vector2 benchPos;

    // Player Stuff
    public int playerHealth, playerMaxHealth, playerHeartShards;
    public float playerMana;
    public bool playerHalfMana;
    public Vector3 playerPosition;
    public string lastScene;
    public bool playerUnlockedWallJump, playerUnlockedDash, playerUnlockedVarJump, playerUnlockedHeal, playerUnlockedCast;

    // Shade Stuff (Linh hồn)
    public Vector2 shadePos;
    public string sceneWithShade;
    public Quaternion shadeRot;

    public void Initialize()
    {
        if (sceneNames == null) sceneNames = new HashSet<string>();
    }

    #region Bench Stuff (MỚI - Sửa lỗi GlobalController)
    public void SaveBench()
    {
        string path = Application.persistentDataPath + "/save.bench.data";
        try
        {
            using (BinaryWriter writer = new BinaryWriter(File.Open(path, FileMode.Create)))
            {
                writer.Write(benchSceneName ?? "");
                writer.Write(benchPos.x); 
                writer.Write(benchPos.y); 
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("Save Bench thất bại: " + e.Message);
        }
    }

    public void LoadBench()
    {
        string path = Application.persistentDataPath + "/save.bench.data";
        if (!File.Exists(path)) return;

        try
        {
            using (BinaryReader reader = new BinaryReader(File.Open(path, FileMode.Open)))
            {
                benchSceneName = reader.ReadString();
                benchPos = new Vector2(reader.ReadSingle(), reader.ReadSingle());
            }
        }
        catch (System.Exception e) { Debug.LogError("Load Bench thất bại: " + e.Message); }
    }
    #endregion

    #region Player Stuff
    public void SavePlayerData()
    {
        if (PlayerController.Instance == null) return;
        string path = Application.persistentDataPath + "/save.player.data";
        try
        {
            using (BinaryWriter writer = new BinaryWriter(File.Open(path, FileMode.Create)))
            {
                writer.Write(PlayerController.Instance.Health);
                writer.Write(PlayerController.Instance.maxHealth);
                writer.Write(PlayerController.Instance.heartShards);
                writer.Write(PlayerController.Instance.Mana);
                writer.Write(PlayerController.Instance.halfMana);
                writer.Write(PlayerController.Instance.unlockedWallJump);
                writer.Write(PlayerController.Instance.unlockedDash);
                writer.Write(PlayerController.Instance.unlockedVarJump);
                writer.Write(PlayerController.Instance.unlockedHeal);
                writer.Write(PlayerController.Instance.unlockedCastSpell);
                writer.Write(PlayerController.Instance.transform.position.x);
                writer.Write(PlayerController.Instance.transform.position.y);
                writer.Write(PlayerController.Instance.transform.position.z);
                writer.Write(SceneManager.GetActiveScene().name);
            }
        }
        catch (System.Exception e) { Debug.LogError("Save Player thất bại: " + e.Message); }
    }

    public void LoadPlayerData()
    {
        string filePath = Application.persistentDataPath + "/save.player.data";
        if (!File.Exists(filePath)) return;

        try
        {
            using (BinaryReader reader = new BinaryReader(File.Open(filePath, FileMode.Open)))
            {
                int h = reader.ReadInt32();
                int mh = reader.ReadInt32();
                int s = reader.ReadInt32();
                float m = reader.ReadSingle();
                bool hm = reader.ReadBoolean();
                bool uwj = reader.ReadBoolean();
                bool ud = reader.ReadBoolean();
                bool uvj = reader.ReadBoolean();
                bool uh = reader.ReadBoolean();
                bool uc = reader.ReadBoolean();
                Vector3 pos = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                string targetScene = reader.ReadString();

                TempLoadCache.cache = new TempLoadCache.Data()
                {
                    health = h,
                    maxHealth = mh,
                    shards = s,
                    mana = m,
                    halfMana = hm,
                    unlockedWall = uwj,
                    unlockedDash = ud,
                    unlockedVar = uvj,
                    unlockedHeal = uh,
                    unlockedCast = uc,
                    pos = pos
                };

                SceneManager.sceneLoaded -= OnSceneLoadedApplyPlayerData;
                SceneManager.sceneLoaded += OnSceneLoadedApplyPlayerData;
                SceneManager.LoadScene(targetScene);
            }
        }
        catch (System.Exception e) { Debug.LogError("Load Player thất bại: " + e.Message); }
    }

    private void OnSceneLoadedApplyPlayerData(Scene scene, LoadSceneMode mode)
    {
        SceneManager.sceneLoaded -= OnSceneLoadedApplyPlayerData;
        if (PlayerController.Instance != null && TempLoadCache.cache != null)
        {
            var d = TempLoadCache.cache;
            PlayerController.Instance.transform.position = d.pos;
            PlayerController.Instance.Health = d.health;
            PlayerController.Instance.maxHealth = d.maxHealth;
            PlayerController.Instance.heartShards = d.shards;
            PlayerController.Instance.Mana = d.mana;
            PlayerController.Instance.halfMana = d.halfMana;
            PlayerController.Instance.unlockedWallJump = d.unlockedWall;
            PlayerController.Instance.unlockedDash = d.unlockedDash;
            PlayerController.Instance.unlockedVarJump = d.unlockedVar;
            PlayerController.Instance.unlockedHeal = d.unlockedHeal;
            PlayerController.Instance.unlockedCastSpell = d.unlockedCast;
            if (GlobalController.instance != null) GlobalController.instance.LoadPlayerScore();
        }
    }
    #endregion

    #region Shade Stuff
    public void SaveShadeData()
    {
        if (Shade.Instance == null) return;
        string path = Application.persistentDataPath + "/save.shade.data";
        try
        {
            using (BinaryWriter writer = new BinaryWriter(File.Open(path, FileMode.Create)))
            {
                sceneWithShade = SceneManager.GetActiveScene().name;
                shadePos = Shade.Instance.transform.position;
                shadeRot = Shade.Instance.transform.rotation;
                writer.Write(sceneWithShade ?? "");
                writer.Write(shadePos.x);
                writer.Write(shadePos.y);
                writer.Write(shadeRot.x);
                writer.Write(shadeRot.y);
                writer.Write(shadeRot.z);
                writer.Write(shadeRot.w);
            }
        }
        catch (System.Exception e) { Debug.LogError("Save Shade thất bại: " + e.Message); }
    }

    public void LoadShadeData()
    {
        string path = Application.persistentDataPath + "/save.shade.data";
        if (!File.Exists(path)) return;
        try
        {
            using (BinaryReader reader = new BinaryReader(File.Open(path, FileMode.Open)))
            {
                sceneWithShade = reader.ReadString();
                shadePos = new Vector2(reader.ReadSingle(), reader.ReadSingle());
                shadeRot = new Quaternion(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
            }
        }
        catch (System.Exception e) { Debug.LogError("Load Shade thất bại: " + e.Message); }
    }
    #endregion

    public static class TempLoadCache
    {
        public class Data
        {
            public int health, maxHealth, shards;
            public float mana;
            public bool halfMana;
            public bool unlockedWall, unlockedDash, unlockedVar, unlockedHeal, unlockedCast;
            public Vector3 pos;
        }
        public static Data cache;
    }
}