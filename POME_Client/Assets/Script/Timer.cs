using UnityEngine;
using System.Collections;

public class Timer : MonoBehaviour
{
    public GameManager _GameManager = null;

    public UISprite _sprite = null;
    public UILabel _label = null;

    private float _fTotalTime = 20f;

    float _fStartTime = 0;
    float _fEndTime = 0;
    float _fCurTime = 0;
    float _fRemainTime = 0;

    float _fAmount = 0;
    string _sRemainTime = "";

    void Update()
    {
        if (GameManager._Step == GameManager.STEP.PLAY)
        {
            SepndTime();
        }
    }

    public void Init()
    {
        _sprite.fillAmount = 1;
        _label.text = _fTotalTime.ToString("N2");

        _fRemainTime = _fTotalTime;
    }

    public void AddTime(float time)
    {
        _fEndTime += time;
    }

    public void Start()
    {
        _fEndTime = Time.time + _fRemainTime;
    }

    //public void StopTimer()
    //{
    //    _bTimer = false;
    //}

    void SepndTime()
    {
        _fCurTime = Time.time;

        _fRemainTime = _fEndTime - _fCurTime;

        if (_fRemainTime <= 0)
        {
            TimerEnd();
        }
        else
        {
            ShowTimer();
        }
    }

    void ShowTimer()
    {
        _fAmount = (_fRemainTime / _fTotalTime);

        _sprite.fillAmount = _fAmount;

        _sRemainTime = _fRemainTime.ToString("N2");

        _label.text = _sRemainTime;
    }

    void TimerEnd()
    {
        _GameManager.END();

        _sprite.fillAmount = 0;

        _label.text = "00.00";

    }
}
