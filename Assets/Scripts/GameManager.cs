using System;
using System.Collections;
using System.Collections.Concurrent;
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
    public Button dragBtn;
    public Button sellBtn;
    public Text currentValueText;
    public Text cashText;
    public Text finalCashText;
    public DuangManager duangManager;
    private List<PlantConfig> plants_config_list;
    private Dictionary<int, List<WordsConfig>> words_config_by_plant_id;
    private Vector3 plantInitPosition;
    public int initCash = 1000;
    private int currentCash = 0;
    
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
    public float btnLongPressTime = 2.0f;
    
    private void Update()
    {
        if (Math.Abs(dragBtnStartTime + 1) > 0.01f)
        {
            var longPressTime = Time.fixedTime - dragBtnStartTime;
            var pressedRatio = longPressTime / btnLongPressTime;
            duangManager.Pulling(pressedRatio);
            if (pressedRatio >= 1.0f)
            {
                OnPlantDrag();
                dragBtnStartTime = -1;
            }
        }
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
        // set cash to init cash
        currentCash = initCash;
        // start game timing
        gameStartTime = Time.fixedTime;
        // instantiate new plant
        InitNewPlant();
    }

    private PlantRunningData _currentPlayingPlant = null;
    
    private void InitNewPlant()
    {
        sellBtn.interactable = false;
        SetTextShow(sellBtn.transform.GetChild(0).GetComponent<Text>(), false);
        dragBtn.interactable = true;
        SetTextShow(dragBtn.transform.GetChild(0).GetComponent<Text>(), true);
        var text = dragBtn.transform.GetChild(0).GetComponent<Text>();
        var color = text.color;
        color.a = 1;
        text.color = color;
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
        currentValueText.text = GetCurrentValueString();
        UpdateCashText();
    }

    private int GetCurrentPlantValue()
    {
        return _currentPlayingPlant.currentDragTime <= 0 ? 0 : _currentPlayingPlant.config.value_list[_currentPlayingPlant.currentDragTime-1];
    }

    private string GetCurrentValueString()
    {
        return "当前价值：￥" + GetCurrentPlantValue();
    }

    public Transform plantMovement;

    private float dragBtnStartTime = -1;

    public void OnDragBtnDown()
    {
        dragBtnStartTime = Time.fixedTime;
    }

    public void OnDragBtnUp()
    {
        dragBtnStartTime = -1;
    }
    
    public void OnDragBtnClicked()
    {
        // //currentCash -= Random.Range(40, 50);
        // currentCash -= 100;
        // UpdateCashText();
        // //plantMovementThreshold += 0.4f;
        // OnPlantDrag();
    }

    private void OnPlantDrag()
    {
        if (_currentPlayingPlant == null)
            return;
        currentCash -= 100;
        UpdateCashText();
        sellBtn.interactable = true;
        SetTextShow(sellBtn.transform.GetChild(0).GetComponent<Text>(), true);
        var valueList = _currentPlayingPlant.config.value_list;
        plantMovement.position += new Vector3(0, _currentPlayingPlant.totalHeight/valueList.Count, 0);
        ++_currentPlayingPlant.currentDragTime;
        currentValueText.text = GetCurrentValueString();
        if (_currentPlayingPlant.currentDragTime < valueList.Count)
        {
            duangManager.Relax();
            return;
        }
        dragBtn.interactable = false;
        SetTextShow(dragBtn.transform.GetChild(0).GetComponent<Text>(), false);
        duangManager.Finish();
    }

    private void SetTextShow(Text text, bool show)
    {
        var color = text.color;
        color.a = show ? 1.0f : 0.0f;
        text.color = color;
    }

    private void UpdateCashText()
    {
        cashText.text = "钱包：￥" + currentCash;
        finalCashText.text = "结余：￥" + currentCash;
    }

    public void OnSellBtnClicked()
    {
        currentCash += GetCurrentPlantValue();
        UpdateCashText();
        InitNewPlant();
    }
    
}
