using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;
using Define;

public class StringManager : MonoBehaviour
{
    public static LANGUAGE _LANGUAGE;

    public class GameText
    {
        public int index;
        public string KR;
        public string EN;
        public string CN;
        public string JP;
    }

    public Dictionary<int, GameText> _dicGameText = new Dictionary<int, GameText>();

    public List<UILabel> _ListFont = null;

    public Font _fontGothic = null;
    public Font _fontLibreCaslon = null;
    public Font _fontOSeongandHanEum = null;

    JSONNode _JsonRoot = null;
    string _StringFile = "Data/StringData";

    public void Init()
    {
        LoadStringData();
        SetLanguage();
        SetLabelsFont();
    }

    void SetLanguage()
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

    void LoadStringData()
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

    public void SetLabelsFont()
    {
        for (int i = 0; i < _ListFont.Count; ++i)
        {
            if (_LANGUAGE == LANGUAGE.CN || _LANGUAGE == LANGUAGE.JP)
            {
                _ListFont[i].trueTypeFont = _fontGothic;
            }
            else if (_LANGUAGE == LANGUAGE.EN)
            {
                _ListFont[i].trueTypeFont = _fontLibreCaslon;
            }
            else if (_LANGUAGE == LANGUAGE.KR)
            {
                _ListFont[i].trueTypeFont = _fontOSeongandHanEum;
            }
        }    
    }

    public void SetLabelsText()
    {
        _ListFont[0].text = GetText(1001); // 시작.
        _ListFont[1].text = GetText(1002); // 도움말.
        _ListFont[2].text = "Clear"; // Clear.
        _ListFont[3].text = GetText(3002); // 최고 점수.
        _ListFont[4].text = GetText(3003); // 점수.
        _ListFont[5].text = GetText(2002); // 이어 하기(+15초)
        _ListFont[6].text = GetText(2003); // 메인 메뉴.
        _ListFont[7].text = GetText(3001); // 결과.
        _ListFont[8].text = GetText(2001); // 일시 정지.
        _ListFont[9].text = "Continue";
        _ListFont[10].text = GetText(1002);// 도움말.
        _ListFont[11].text = GetText(2003);// 메인 메뉴.
    }
}
