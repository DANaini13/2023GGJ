using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;


[Serializable]
class ConfigJsonType<T>
{
    public string key;
    public List<T> content;
}

[Serializable]
class ConfigFileMeta
{
    public List<string> file_list;
}


public static class FileHelper
{
    public static async Task<string> AsyncLoadFileTextByUrl(string file_url)
    {
        if(Application.platform != RuntimePlatform.Android)
            file_url = "file://" + file_url;
        var webRequest = UnityWebRequest.Get(file_url);
        await webRequest.SendWebRequest();
        return webRequest.downloadHandler.text;
    }
    
    
    public static string SyncLoadFileTextByUrl(string file_url)
    {
        return File.ReadAllText(file_url);
    }
    
    
    public static string GetCrossPlatformConfigPathUrl()
    {
        return Application.streamingAssetsPath + "/Config/";
    }

    public static string GetCrossPlatformEnvConfigPath()
    {
        return Application.streamingAssetsPath + "/env_config.json";
    }

    public static void VerifyKey(string text)
    {
        
    }
}