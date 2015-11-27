using UnityEngine;
using System.Collections;
using Define;

public class Timer : MonoBehaviour
{
    public GameManager _GameManager = null;

    public UISprite _sprPause = null;
    public UISprite _sprite = null;
    public UILabel _label = null;

    public float _fRemainTime = 0;

    private float _fTotalTime = 60f;

    float _fEndTime = 0;
    float _fCurTime = 0;
    float _fStageStartTime = 0;

    float _fAmount = 0;
    string _sRemainTime = "";

    LTDescr tmpTween = null;

    bool _bTimerStart = false;

    void Update()
    {
        if (_bTimerStart)
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

    public void TimerStart()
    {
        _bTimerStart = true;

        _fEndTime = Time.time + _fRemainTime;
        _fStageStartTime = Time.time;

        _sprPause.gameObject.SetActive(false);

        if (tmpTween != null)
        {
            LeanTween.cancel(tmpTween.uniqueId);

            _sprPause.alpha = 1;
            _label.alpha = 1;
        }

    }

    public void TimerStop()
    {
        _bTimerStart = false;

        TimeBlink();
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
        TimerStop();

        _sprite.fillAmount = 0;

        _label.text = "00.00";

        if (tmpTween != null)
        {
            LeanTween.cancel(tmpTween.uniqueId);

            _sprPause.alpha = 1;
            _label.alpha = 1;
        }

        _GameManager.END();
    }

    void TimeBlink()
    {
        _sprPause.gameObject.SetActive(true);

        tmpTween = LeanTween.value(_sprPause.gameObject, 1f, 0f, 0.5f).setLoopPingPong().setOnUpdate(
            (float value) =>
            {
                _sprPause.alpha = value;
                _label.alpha = value;
            });
    }
}
