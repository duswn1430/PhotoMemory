using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Define;
using SimpleJSON;


public class GameManager : MonoBehaviour
{
    public enum STEP { INIT, PLAY, COMPLETE };

    public static STEP _Step;

    public BoxMapManager _BoxMapManager = null;

    public UILabel _UIText = null;
    public UILabel _UIScore = null;

    public GameObject _btnStart = null;

    List<BoxMapData> _listMapData;

    JSONNode _jsonRoot = null;
    string _sMapFile = "MapData";

    int _iStage;
    int _iCountTime = 3;

    void Awake()
    {
        _listMapData = new List<BoxMapData>();
    }

    // Use this for initialization
    void Start()
    {
        LoadMapData();
    }

    void LoadMapData()
    {
        TextAsset asset = (TextAsset)Resources.Load(_sMapFile);
        _jsonRoot = JSON.Parse(asset.text);

        for (int i = 0; i < _jsonRoot.Count; ++i)
        {
            BoxMapData data = new BoxMapData();
            data.idx = _jsonRoot[i]["Index"].AsInt;
            data.iRow = _jsonRoot[i]["BoxW"].AsInt;
            data.iCol = _jsonRoot[i]["BoxH"].AsInt;
            data.iColorType = _jsonRoot[i]["ColorTypes"].AsInt;
            data.iColorVolume = _jsonRoot[i]["ColorVolume"].AsInt;
            data.iBonusTerms = _jsonRoot[i]["BonusTerms"].AsInt;
            data.iBonusTime = _jsonRoot[i]["BonusTime"].AsInt;

            _listMapData.Add(data);
        }
    }

    void Init()
    {
        _Step = STEP.INIT;

        _iStage = 0;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void START()
    {
        _btnStart.SetActive(false);

        StartCoroutine(GameStart());
    }

    public void RESTART()
    {

    }

    public void BACK()
    {

    }

    public void COMPLETE()
    {
        _Step = STEP.COMPLETE;

        SetText(_Step.ToString(), 200);

        StartCoroutine(NextStage());
    }

    IEnumerator GameStart()
    {
        yield return StartCoroutine(_BoxMapManager.SetBoxMap(_listMapData[_iStage]));

        yield return new WaitForSeconds(0.5f);

        _BoxMapManager.BoxColoring();

        yield return new WaitForSeconds(0.5f);

        yield return StartCoroutine(Counting());

        yield return StartCoroutine(_BoxMapManager.BoxShuffling());

        _Step = STEP.PLAY;

        SetText(_Step.ToString(), 200);
    }

    IEnumerator Counting()
    {
        int time = _iCountTime;

        while (time > 0)
        {
            SetText(time.ToString(), 250);

            yield return new WaitForSeconds(1f);

            time--;
        }

        SetText("", 0);
    }

    IEnumerator NextStage()
    {
        yield return new WaitForSeconds(2f);

        _iStage++;

        if (_listMapData.Count > _iStage)
            StartCoroutine(GameStart());
        else
            Debug.Log("STAGE END");

    }

    void SetText(string text, int size)
    {
        _UIText.fontSize = size;
        _UIText.text = text;
    }
}
