using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public float total_game_length = 60;
    public ProgressBar gameProgressBar;
    private List<PlantConfig> plants_config_list;
    private Dictionary<int, List<WordsConfig>> words_config_by_plant_id;
    
    private void Awake()
    {
        startGamePage.SetActive(true);
        endGamePage.SetActive(false);
        // load all configs.
        plants_config_list = Configurator.Instance.GetConfigByKey<PlantConfig>("plants_config");
        var words_config_list = Configurator.Instance.GetConfigByKey<WordsConfig>("words");
        // assemble configs.
        words_config_by_plant_id = new Dictionary<int, List<WordsConfig>>();
        foreach (var config in words_config_list)
        {
            if(!words_config_by_plant_id.ContainsKey(config.id))
                words_config_by_plant_id.Add(config.id, new List<WordsConfig>());
            words_config_by_plant_id[config.id].Add(config);
        }
    }

    private float gameStartTime = 0;
    
    
    private void Update()
    {
        var usedTime = Time.fixedTime - gameStartTime;
        var ratio = usedTime / total_game_length;
        if (ratio >= 1.0f)
        {
            endGamePage.SetActive(true);
            return;
        }
        if (ratio < 0) return;
        gameProgressBar.SetProgress(ratio);
    }


    public GameObject startGamePage;
    public GameObject endGamePage;
    public void OnStartGameBtnClicked()
    {
        startGamePage.SetActive(false);
        endGamePage.SetActive(false);
        // start game timing!
        gameStartTime = Time.fixedTime;
    }
}
