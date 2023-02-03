using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class PlantRunningData
{
    public PlantConfig config;
    public float totalHeight;
    public int currentDragTime;
}


public class GameManager : MonoBehaviour
{
    public float total_game_length = 60;
    public ProgressBar gameProgressBar;
    public Transform plantPlaceHolder;
    private List<PlantConfig> plants_config_list;
    private Dictionary<int, List<WordsConfig>> words_config_by_plant_id;
    private Vector3 plantInitPosition;
    
    private void Awake()
    {
        plantInitPosition = plantMovement.transform.position;
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
    private float plantMovementThreshold = 0;
    
    
    private void Update()
    {
        plantMovementThreshold -= 0.7f * Time.deltaTime;
        plantMovementThreshold = plantMovementThreshold < 0 ? 0 : plantMovementThreshold;
        Debug.Log(plantMovementThreshold);
        
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
        // instantiate new plant
        InitNewPlant();
    }

    private PlantRunningData _currentPlayingPlant = null;
    
    private void InitNewPlant()
    {
        if (plantPlaceHolder.childCount > 0)
            Destroy(plantPlaceHolder.GetChild(0).gameObject);
        var plantIndex = Random.Range(0, plants_config_list.Count);
        _currentPlayingPlant = new PlantRunningData();
        _currentPlayingPlant.currentDragTime = 0;
        _currentPlayingPlant.config = plants_config_list[plantIndex];
        var prefab = PrefabManager.Instance.GetPrefab(_currentPlayingPlant.config.key);
        var spriteRenderer = Instantiate(prefab, plantPlaceHolder).GetComponent<SpriteRenderer>();
        _currentPlayingPlant.totalHeight = spriteRenderer.size.y;
        plantMovement.position = plantInitPosition;
    }

    public Transform plantMovement;
    
    public void OnDragBtnClicked()
    {
        plantMovementThreshold += 0.4f;
        if (!(plantMovementThreshold >= 1.0f)) return;
        plantMovementThreshold = 0;
        OnPlantDrag();
    }

    private void OnPlantDrag()
    {
        if (_currentPlayingPlant == null)
            return;
        var valueList = _currentPlayingPlant.config.value_list;
        if (_currentPlayingPlant.currentDragTime < valueList.Count)
        {
            ++_currentPlayingPlant.currentDragTime;
            plantMovement.position += new Vector3(0, _currentPlayingPlant.totalHeight/valueList.Count, 0);
        }
        else
            InitNewPlant();
    }

    public void OnSellBtnClicked()
    {
        
    }
    
}
