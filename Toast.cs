using UnityEngine;
using System.Collections;
using Define;
using System;

public class Toast : MonoBehaviour
{
    public static Toast _Instance = null;

    public StringManager _StringManager = null;

    public GameObject _Parent = null;

    public UILabel _Text = null;

    bool _bShow;

    void Awake()
    {
        _Instance = this;
    }

    // Use this for initialization
    void Start()
    {
        _Parent.transform.localScale = Vector3.zero;

        _Text.text = "";

        _bShow = false;
    }

    public void Show(TOAST_TYPE type)
    {
        if (_bShow == false)
        {
            _bShow = true;

            _Text.text = GetText(type);

            LeanTween.scale(_Parent, Vector3.one, 0.15f).setOnComplete(Dismiss);
        }
    }

    void Dismiss()
    {
        LeanTween.scale(_Parent, Vector3.zero, 0.15f).setDelay(3.0f).setOnComplete(new Action(() => { _bShow = false; }));
    }

    string GetText(TOAST_TYPE type)
    {
        string txt = "";

        switch(type)
        {
            case TOAST_TYPE.EXIT:
                {
                    txt = _StringManager.GetText(5001);
                }
                break;
            case TOAST_TYPE.ORIGIN:
                {
                    txt = _StringManager.GetText(5002);
                }
                break;
        }

        return txt;
    }
}
