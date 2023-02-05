using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using DG.Tweening;

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
    public Text sellBtnText;
    public Text cashText;
    public Text finalCashText;
    public DuangManager duangManager;
    private List<PlantConfig> plants_config_list;
    private Dictionary<int, List<WordsConfig>> words_config_by_plant_id;
    private Vector3 plantInitPosition;
    public int initCash = 1000;
    private int currentCash = 0;
    private int currentMaxDragCount;

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
            if (!words_config_by_plant_id.ContainsKey(config.id))
                words_config_by_plant_id.Add(config.id, new List<WordsConfig>());
            words_config_by_plant_id[config.id].Add(config);
        }
        continueBtn.gameObject.SetActive(false);
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
            duangManager.Restore();
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
        continueBtn.gameObject.SetActive(false);
        duangManager.Restore();

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
        Debug.Log(_currentPlayingPlant.config.key);
        var spriteRenderer = Instantiate(prefab, plantPlaceHolder).GetComponent<SpriteRenderer>();
        _currentPlayingPlant.totalHeight = spriteRenderer.size.y;
        plantMovement.position = plantInitPosition;
        currentValueText.text = GetCurrentValueString();
        sellBtnText.text = "卖(+￥" + GetCurrentPlantValue() + ")";
        UpdateCashText();

        currentMaxDragCount = _currentPlayingPlant.config.value_list.Count;
    }

    private int GetCurrentPlantRealValue()
    {
        return _currentPlayingPlant.config.value_list[currentMaxDragCount - 1];
    }

    private int GetCurrentPlantValue()
    {
        return _currentPlayingPlant.currentDragTime <= 0 ? 0 : _currentPlayingPlant.config.value_list[_currentPlayingPlant.currentDragTime - 1];
    }

    private string GetCurrentValueString()
    {
        return "当前价值：￥" + GetCurrentPlantValue();
    }

    private string GetCurrentPlantWords(int Count)
    {
        return words_config_by_plant_id[_currentPlayingPlant.config.id][Count].words;
    }

    private string[] GetCurrentPlantWordsArray(int count)
    {
        string[] str = new string[count];
        for (int i = 0; i < count; i++)
        {
            str[i] = GetCurrentPlantWords(i);
        }

        return str;
    }

    public Transform plantMovement;

    private float dragBtnStartTime = -1;

    public AudioSource audioSource;
    public AudioClip dragSfx;
    public void OnDragBtnDown()
    {
        duangManager.Restore();
        audioSource.clip = dragSfx;
        audioSource.Play();
        dragBtnStartTime = Time.fixedTime;
    }

    public void OnDragBtnUp()
    {
        if (dragBtnStartTime > 0)
            duangManager.Pulling(0);
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

    public AudioClip relaxSfx;
    private void OnPlantDrag()
    {
        if (_currentPlayingPlant == null)
            return;
        currentCash -= 100;
        UpdateCashText();
        sellBtn.interactable = true;
        SetTextShow(sellBtn.transform.GetChild(0).GetComponent<Text>(), true);
        var valueList = _currentPlayingPlant.config.value_list;
        plantMovement.position += new Vector3(0, _currentPlayingPlant.totalHeight / valueList.Count, 0);
        ++_currentPlayingPlant.currentDragTime;
        currentValueText.text = GetCurrentValueString();
        sellBtnText.text = "卖(+￥" + GetCurrentPlantValue() + ")";
        audioSource.clip = relaxSfx;
        audioSource.Play();
        if (_currentPlayingPlant.currentDragTime < valueList.Count)
        {
            duangManager.ShowSingleComment(GetCurrentPlantWords(_currentPlayingPlant.currentDragTime - 1));
            duangManager.Relax();
            return;
        }
        dragBtn.interactable = false;
        SetTextShow(dragBtn.transform.GetChild(0).GetComponent<Text>(), false);
        sellBtn.interactable = false;
        SetTextShow(sellBtn.transform.GetChild(0).GetComponent<Text>(), false);
        var delay = duangManager.Finish(GetCurrentPlantWordsArray(currentMaxDragCount), GetCurrentPlantRealValue());
        DOTween.To(v => { }, 0, 0, delay).onComplete += () =>
        {
            sellBtn.interactable = true;
            SetTextShow(sellBtn.transform.GetChild(0).GetComponent<Text>(), true);
        };
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

    public Button continueBtn;
    public AudioClip sellSfx;
    public void OnSellBtnClicked()
    {
        audioSource.clip = sellSfx;
        audioSource.Play();
        currentCash += GetCurrentPlantValue();
        duangManager.CoinPS(GetCurrentPlantValue());
        UpdateCashText();

        if (_currentPlayingPlant.currentDragTime < currentMaxDragCount)
        {
            var delay = duangManager.Interrupt(GetCurrentPlantWordsArray(currentMaxDragCount), GetCurrentPlantRealValue(), GetCurrentPlantValue());
            plantMovement.position += new Vector3(0, _currentPlayingPlant.totalHeight, 0);
            dragBtn.interactable = false;
            SetTextShow(dragBtn.transform.GetChild(0).GetComponent<Text>(), false);
            sellBtn.interactable = false;
            SetTextShow(sellBtn.transform.GetChild(0).GetComponent<Text>(), false);
            DOTween.To(v => { }, 0, 0, delay).onComplete += () =>
            {
                continueBtn.gameObject.SetActive(true);
            };
        }
        else
            InitNewPlant();
    }

    public void OnContinueBtnClick()
    {
        InitNewPlant();
    }

}
