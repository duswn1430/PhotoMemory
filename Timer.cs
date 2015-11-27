using UnityEngine;
using System.Collections;
using Define;

public class Timer : MonoBehaviour
{
    public GameManager _GameManager = null;

    public UISprite _sprite = null;
    public UILabel _label = null;

    public float _fRemainTime = 0;
    
    private float _fTotalTime = 60f;

    //float _fStartTime = 0;
    float _fEndTime = 0;
    float _fCurTime = 0;
    float _fStageStartTime = 0;

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

    public void AddTime(BoxMapData mapdata, float waitT)
    {
        float stageSpendTime = Time.time - _fStageStartTime;

        if (stageSpendTime <= mapdata.iBonusTerms)
        {
            _fCurTime = Time.time - waitT;

            float Bonus = _fEndTime + mapdata.iBonusTime;

            LeanTween.value(gameObject, _fEndTime, Bonus, 0.5f).setEase(LeanTweenType.easeOutCirc).setOnUpdate(
                (float value) =>
                {
                    _fEndTime = value;

                    _fRemainTime = _fEndTime - _fCurTime;

                    ShowTimer();
                }
            );
        }
    }

    public void GameStart()
    {
        _fEndTime = Time.time + _fRemainTime;
        _fStageStartTime = Time.time;
    }

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
        //_sRemainTime = ((int)_fRemainTime).ToString();

        _label.text = _sRemainTime;
    }

    void TimerEnd()
    {
        _GameManager.END();

        _sprite.fillAmount = 0;

        _label.text = "00.00";
    }
}
