using UnityEngine;
using System.Collections;
using Define;

public class Timer : MonoBehaviour
{
    public GameManager _GameManager = null;

    //public UISprite _sprite = null;
    public UILabel _label = null;

    public float _fRemainTime = 0;

    private float _fTotalTime = 30f;

    float _fEndTime = 0;
    float _fCurTime = 0;
    float _fPauseTime = 0;
    float _fCutTime = 0;

    float _fStageStartTime = 0;
    float _fHintTime = 0;

    float _fAmount = 0;
    string _sRemainTime = "";
    
    LTDescr BlinkTween = null;

    bool _bTimerStart = false;
    bool _bHint = false;

    void Update()
    {
        if (_bTimerStart)
        {
            SepndTime();
            Hint();
        }
    }

    public void Init()
    {
        //_sprite.fillAmount = 1;
        _label.text = _fTotalTime.ToString("N2");

        _fRemainTime = _fTotalTime;
    }

    public void AddTime(BoxMapData mapdata, float waitT)
    {
        float stageSpendTime = Time.time - _fStageStartTime - _fCutTime;

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

    public void TimerStart()
    {
        _bTimerStart = true;
        _bHint = true;

        _fEndTime = Time.time + _fRemainTime;
        _fStageStartTime = Time.time;

        _fHintTime = _fRemainTime;

        _fCutTime = 0;
    }

    public void TimerStop()
    {
        _bTimerStart = false;
    }

    public void TimerPause()
    {
        _bTimerStart = false;

        _fPauseTime = Time.time;
    }

    public void TimerResume()
    {
        _bTimerStart = true;

        _fPauseTime = Time.time - _fPauseTime;

        _fCutTime += _fPauseTime;
    }

    void SepndTime()
    {
        _fCurTime = Time.time - _fCutTime;

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

        //_sprite.fillAmount = _fAmount;

        _sRemainTime = _fRemainTime.ToString("N2");
        //_sRemainTime = ((int)_fRemainTime).ToString();

        _label.text = _sRemainTime;
    }

    void TimerEnd()
    {
        TimerStop();

        //_sprite.fillAmount = 0;

        _label.text = "00.00";

        _GameManager.END();
    }

    void Hint()
    {
        if (_bHint)
        {
            float time = _fHintTime - _fRemainTime;

            if (time >= 5)
            {
                _bHint = false;

                _GameManager.ShowHint();
            }
        }
    }

    public void ResetHint()
    {
        _bHint = true;
        _fHintTime = _fRemainTime;
    }

    public void BlinkStart()
    {
        BlinkTween = LeanTween.value(_label.gameObject, 1f, 0f, 0.5f).setLoopPingPong().setOnUpdate(
            (float value) =>
            {
                _label.alpha = value;
            });
    }

    public void BlinkStop()
    {
        if (BlinkTween != null)
        {
            LeanTween.cancel(BlinkTween.uniqueId);

            _label.alpha = 1;
        }
    }
}
