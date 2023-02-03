using UnityEngine;
using System;
using System.Collections.Generic;

#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

static class PrefabPathSerializer
{
    public static void ReserializePrefabs()
    {
        var prefabPathByKey = new Dictionary<string, string>();
        DirectoryInfo folder = new DirectoryInfo(Application.dataPath + "/Resources/Prefabs");
        RecursivelyGetPrefabPaths(folder, prefabPathByKey);
        var data = new PrefabPathFileInfo.PrefabPathData();
        data.content = new List<PrefabPathFileInfo.PrefabPathUnit>();
        foreach (var pair in prefabPathByKey)
        {
            data.content.Add(new PrefabPathFileInfo.PrefabPathUnit()
            {
                prefabName = pair.Key,
                path = pair.Value
            });
        }

        File.WriteAllText(PrefabPathFileInfo.configPath + PrefabPathFileInfo.configFileName + ".txt", JsonUtility.ToJson(data));
        Debug.Log("Prefab路径序列化完成");
    }
    
    private static void RecursivelyGetPrefabPaths(DirectoryInfo dirInfo, Dictionary<string, string> pathDic)
    {
        var subDirList = dirInfo.GetDirectories();
        foreach (var fileInfo in dirInfo.GetFiles())
        {
            var prefabName = fileInfo.Name;
            if(prefabName.Contains(".meta"))
                continue;
            prefabName = prefabName.Remove(prefabName.Length - 7, 7);
            if (fileInfo.Directory == null)
                continue;
            var path = fileInfo.Directory.FullName;
            string key = Application.platform == RuntimePlatform.WindowsEditor ? "\\Resources\\" : "/Resources/";
            int index = path.LastIndexOf(key, StringComparison.Ordinal);
            path = path.Remove(0, index + 11);
            if (pathDic.ContainsKey(prefabName))
            {
                Debug.LogError("存在同名Prefab，请检查：" + fileInfo.FullName);
                continue;
            }
            pathDic.Add(prefabName, path);
        }
        foreach (var subDir in subDirList)
        {
            RecursivelyGetPrefabPaths(subDir, pathDic);
        }
    }
}

public class PrefabPathPostProcessor : AssetPostprocessor, IPreprocessBuildWithReport
{
    public void OnPostprocessPrefab(GameObject g)
    {
        PrefabPathSerializer.ReserializePrefabs();
    }

    public int callbackOrder { get; set; }
    public void OnPreprocessBuild(BuildReport report)
    {
        PrefabPathSerializer.ReserializePrefabs();
    }
}

[InitializeOnLoadAttribute]
public static class PlayModeStateChangedExample
{
    static PlayModeStateChangedExample()
    {
        PrefabPathSerializer.ReserializePrefabs();
    }
}
#endif

static class PrefabPathFileInfo
{
    public static string configPath = Application.dataPath + "/Resources/";
    public static string configFileName = "prefab_paths";
    
    [Serializable]
    public struct PrefabPathUnit
    {
        public string prefabName;
        public string path;
    }

    [Serializable]
    public struct PrefabPathData
    {
        public List<PrefabPathUnit> content;
    }
}