using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;
using Define;

public class StringData : MonoBehaviour
{
    private static StringData instance = null;
    public static StringData _Instance
    {
        get
        {
            if (instance == null)
            {
                instance = (new GameObject("StringData")).AddComponent<StringData>();
            }
            return instance;
        }
    }

    public static LANGUAGE _LANGUAGE;

    JSONNode _JsonRoot = null;
    string _StringFile = "Data/StringData";

    public class GameText
    {
        public int index;
        public string KR;
        public string EN;
        public string CN;
        public string JP;
    }

    public Dictionary<int, GameText> _dicGameText = new Dictionary<int, GameText>();

    public void SetLanguage()
    {
        SystemLanguage sl = Application.systemLanguage;

        switch(sl)
        {
            case SystemLanguage.Korean:
                {
                    _LANGUAGE = LANGUAGE.KR;
                }
                break;
            case SystemLanguage.Japanese:
                {
                    _LANGUAGE = LANGUAGE.JP;
                }
                break;
            case SystemLanguage.Chinese:
            case SystemLanguage.ChineseSimplified:
            case SystemLanguage.ChineseTraditional:
                {
                    _LANGUAGE = LANGUAGE.CN;
                }
                break;
            default:
                {
                    _LANGUAGE = LANGUAGE.EN;
                }
                break;
        }
    }

    public void LoadStringData()
    {
        TextAsset asset = (TextAsset)Resources.Load(_StringFile);
        _JsonRoot = JSON.Parse(asset.text);

        for (int i = 0; i < _JsonRoot.Count; ++i)
        {
            GameText data = new GameText();
            data.index = _JsonRoot[i]["index"].AsInt;
            data.KR = _JsonRoot[i]["NameKR"];
            data.EN = _JsonRoot[i]["NameEN"];
            data.CN = _JsonRoot[i]["NameCH"];
            data.JP = _JsonRoot[i]["NameJP"];

            _dicGameText.Add(data.index, data);
        }
    }

    public string GetText(int index)
    {
        string text = "";

        switch (_LANGUAGE)
        {
            case LANGUAGE.KR:
                text = _dicGameText[index].KR;
                break;
            case LANGUAGE.EN:
                text = _dicGameText[index].EN;
                break;
            case LANGUAGE.CN:
                text = _dicGameText[index].CN;
                break;
            case LANGUAGE.JP:
                text = _dicGameText[index].JP;
                break;
        }
        return text;
    }
}
