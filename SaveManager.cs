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
    public string GetSlot() { return "_" + SaveSlot.ToString(); }
    public static SaveManager Instance { get; private set; }
    string encodePassword = "hw_multi_mjsdhf457332_";
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

    public static void Save_Game()
    {
        Instance.OnGameSaving?.Invoke();
        Debug.LogWarning("OnGameSaving?.Invoke()");
    }
    public static void Load_Game()
    {
        Debug.LogWarning("OnGameLoading?.Invoke()");
        Instance.OnGameLoading?.Invoke();
    }

    public void Save<T>(string s, T entity)
    {
        SaveGame.Save<T>(s + GetSlot(), entity);
    }

    public void Save<T>(string s, T entity, bool encoding)
    {
        SaveGame.Save<T>(s + GetSlot(), entity, encoding);
    }

    public T Load<T>(string s)
    {
        return SaveGame.Load<T>(s + GetSlot());
    }

    public T Load<T>(string s, T defaultValue)
    {
        return SaveGame.Load<T>(s + GetSlot(), defaultValue);
    }
    public T Load<T>(string s, bool encode)
    {
        return SaveGame.Load<T>(s + GetSlot(), encode, encodePassword);
    }

    public bool Exists(string s)
    {
        return SaveGame.Exists(s + GetSlot());
    }

    public void DeleteKey(string s)
    {
        if (Exists(s + GetSlot())) SaveGame.Delete(s + GetSlot());
    }

    public static void DeleteSlot_0(string s)
    {
        SaveGame.Delete(s + "_0");
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