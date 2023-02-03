using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class Configurator : BaseSingletonDontDestroy<Configurator>
{
    public List<T> GetConfigByKey<T>(string key)
    {
        if (file_text_dic_dictionary == null || file_text_dic_dictionary.Count <= 0)
        {
            Debug.LogError("Json File Not Loaded yet !");
            return null;
        }
        key = key + ".json";
        if (!file_text_dic_dictionary.ContainsKey(key))
            return null;
        string jsonText = file_text_dic_dictionary[key];
        var jsonConfig = JsonUtility.FromJson<ConfigJsonType<T>>(jsonText);
        return jsonConfig.content;
    }

    public double LoadingProgressPercent
    {
        get
        {
            return loading_percent;
        }
    }

    // private bool hasLoadEnvConfifg = false;
    
    public async Task StartASyncLoading(string folder)
    {
        loading_percent = 0.01f;
        var path = FileHelper.GetCrossPlatformConfigPathUrl() + folder + "/";
        var file_list = await AsyncGrabConfigFileList(folder);
        var size = file_list.Count + 1;
        loading_percent += 1.0 / size;
        foreach (var file_name in file_list)
        {
            if (file_name != "localization.json")
            {
                var text = await FileHelper.AsyncLoadFileTextByUrl(path + file_name);
                FileHelper.VerifyKey(text);
                file_text_dic_dictionary.Add(file_name, text);   
            }
            loading_percent += 1.0 / size;
        }
    }

    public async Task ASyncLoadEnvConfig()
    {
        var env_text = await FileHelper.AsyncLoadFileTextByUrl(FileHelper.GetCrossPlatformEnvConfigPath());
        FileHelper.VerifyKey(env_text);
        if (!file_text_dic_dictionary.ContainsKey("env_config.json"))
            file_text_dic_dictionary.Add("env_config.json", env_text);
    }

    public void SyncLoadEnvConfig()
    {
        var env_text = FileHelper.SyncLoadFileTextByUrl(FileHelper.GetCrossPlatformEnvConfigPath());
        FileHelper.VerifyKey(env_text);
        if (!file_text_dic_dictionary.ContainsKey("env_config.json"))
            file_text_dic_dictionary.Add("env_config.json", env_text);
    }

    private double loading_percent = 0;
    
    private async Task<List<string>> AsyncGrabConfigFileList(string folder)
    {
        var meta_file_url = FileHelper.GetCrossPlatformConfigPathUrl() + folder + "/meta.json";
        var file_content = await FileHelper.AsyncLoadFileTextByUrl(meta_file_url);
        var meta_config = JsonUtility.FromJson<ConfigFileMeta>(file_content);
        return meta_config.file_list;
    }
    
    public void SyncLoading(string folder)
    {
        var file_list = SyncGrabConfigFileList(folder);
        var path = FileHelper.GetCrossPlatformConfigPathUrl() + folder + "/";
        foreach (var file_name in file_list)
        {
            var text = FileHelper.SyncLoadFileTextByUrl(path + file_name);
            FileHelper.VerifyKey(text);
            file_text_dic_dictionary.Add(file_name, text);
        }
        loading_percent = 1.0;
    }

    public void Clear()
    {
        loading_percent = 0;
        file_text_dic_dictionary.Clear();
    }

    private List<string> SyncGrabConfigFileList(string folder)
    {
        var meta_file_url = FileHelper.GetCrossPlatformConfigPathUrl() + folder + "/meta.json";
        var file_content = FileHelper.SyncLoadFileTextByUrl(meta_file_url);
        var meta_config = JsonUtility.FromJson<ConfigFileMeta>(file_content);
        return meta_config.file_list;
    }

    private Dictionary<string, string> file_text_dic_dictionary = new Dictionary<string, string>();
}
