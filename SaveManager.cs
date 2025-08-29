using BayatGames.SaveGameFree;
using UnityEngine;
using System.IO;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

[DefaultExecutionOrder(-90)]
public class SaveManager : MonoBehaviour
{
    [SerializeField, TextArea(0, 5)] string savePaths;
    [Range(0, 7)] public int SaveSlot;
    readonly string encodePassword = "randm_";
    public string GetSlot() { return "_" + SaveSlot.ToString(); }
    public static SaveManager Instance { get; private set; }
    public Action OnGameSaving;
    public Action OnGameLoading;


    protected void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(gameObject);
        }

        SaveGame.EncodePassword = encodePassword;
    }

    public void Save_Game()
    {
        OnGameSaving?.Invoke();
        Debug.LogWarning("OnGameSaving?.Invoke()");
    }
    public void Load_Game()
    {
        Debug.LogWarning("OnGameLoading?.Invoke()");
        OnGameLoading?.Invoke();
    }

    public static void Save<T>(string saveKey, T entity, bool encoding = false)
    {
        SaveGame.Save<T>(saveKey + Instance.GetSlot(), entity, encoding);
    }

    public static T Load<T>(string saveKey)
    {
        return SaveGame.Load<T>(saveKey + Instance.GetSlot());
    }

    public static T Load<T>(string saveKey, T defaultValue)
    {
        return SaveGame.Load<T>(saveKey + Instance.GetSlot(), defaultValue);
    }
    public static T Load<T>(string saveKey, bool encode)
    {
        return SaveGame.Load<T>(saveKey + Instance.GetSlot(), encode, Instance.encodePassword);
    }

    public static bool Exists(string saveKey)
    {
        return SaveGame.Exists(saveKey + Instance.GetSlot());
    }

    public static void DeleteKey(string saveKey)
    {
        if (Exists(saveKey + Instance.GetSlot())) SaveGame.Delete(saveKey + Instance.GetSlot());
    }

    public static void DeleteSlot_0(string saveKey)
    {
        SaveGame.Delete(saveKey + "_0");
    }

    public void DeleteAllKeys()
    {
        SaveGame.Clear();
    }

    public void GetPath()
    {
        FileInfo[] fileInfos = SaveGame.GetFiles();
        savePaths = "";
        if (fileInfos.Length <= 0)
        {
            Debug.Log("<color=red>SaveGame not found</color>");
            savePaths = "SaveGame not found";
            return;
        }
        foreach (var f in fileInfos)
        {
            Debug.Log("<color=yellow>" + f.DirectoryName + " </color>" + "<color=black>" + f.Name + "</color>");
            savePaths += f.FullName + "\n";
        }
        System.Diagnostics.Process.Start("explorer.exe", $"/select,\"{fileInfos[0].FullName}\"");
    }
}


#if UNITY_EDITOR

[CustomEditor(typeof(SaveManager))]
public class SaveManagerEditor : Editor
{
    SaveManager myComponent;
    public void OnEnable()
    {
        myComponent = target as SaveManager;
    }
    public override void OnInspectorGUI()
    {
        if (GUILayout.Button("Delete All Keys"))
        {
            myComponent.DeleteAllKeys();
        }

        if (GUILayout.Button("Get File Informations"))
        {
            myComponent.GetPath();
        }

        DrawDefaultInspector();
    }
}
#endif
