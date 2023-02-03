using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;

public class PrefabManager : BaseSingletonDontDestroy<PrefabManager>
{
    private StringBuilder _stringBuilder = new StringBuilder();

    public GameObject GetPrefab(string name)
    {
        if (!_prefabPathByKey.ContainsKey(name))
            return null;
        _stringBuilder.Clear();
        _stringBuilder.Append(_prefabPathByKey[name]);
        _stringBuilder.Append("/");
        _stringBuilder.Append(name);
        return Resources.Load(_stringBuilder.ToString()) as GameObject;
    }

#if UNITY_EDITOR
    public GameObject GetEditorPrefab(string name)
    {
        return EditorGUIUtility.Load($"Assets/Prefabs/Editor/{name}") as GameObject;
    }
#endif

    public Sprite GetResourceIconSprite(string key)
    {
        _stringBuilder.Clear();
        _stringBuilder.Append("Icons/ResourceIcons/");
        _stringBuilder.Append("resource_icon_");
        _stringBuilder.Append(key);
        return Resources.Load<Sprite>(_stringBuilder.ToString());
    }
    
    public Sprite GetLabItemIconSprite(string key){
        _stringBuilder.Clear();
        _stringBuilder.Append("Icons/LabItemIcons/");
        _stringBuilder.Append("lab_item_icon_");
        _stringBuilder.Append(key);
        var icon = Resources.Load<Sprite>(_stringBuilder.ToString());
        if(icon != null)
            return icon;
        else
            return GetLabItemIconSprite("default");
    }
    
    public Sprite GetCommonIconSprite(string key)
    {
        _stringBuilder.Clear();
        _stringBuilder.Append("Icons/Common/");
        _stringBuilder.Append(key);
        return Resources.Load<Sprite>(_stringBuilder.ToString());
    }

    private bool loaded = false;
    public void LoadNecessaryResources()
    {
        if (IsLoaded())
            return;
        // 获取所有Prefab的路径
        var fileContent = Resources.Load<TextAsset>(PrefabPathFileInfo.configFileName);
        var data = JsonUtility.FromJson<PrefabPathFileInfo.PrefabPathData>(fileContent.text);
        foreach (var pair in data.content)
            _prefabPathByKey.Add(pair.prefabName, pair.path);
        
        // 先加载一遍固定需要的
        loaded = true;
    }

    public void UnloadUnusedResources()
    {
        loaded = false;
        _prefabPathByKey.Clear();
        Resources.UnloadUnusedAssets();
    }

    public bool IsLoaded()
    {
        return loaded;
    }

    private Dictionary<string, string> _prefabPathByKey = new Dictionary<string, string>();

    private StringBuilder _sb = new StringBuilder();
}
