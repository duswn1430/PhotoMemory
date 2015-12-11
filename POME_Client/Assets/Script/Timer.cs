using UnityEngine;
using System.Collections;

public class Timer : MonoBehaviour
{
    public UILabel _label = null;

    public float _fRemainTime;

    private float _fTotalTime = 6.0f;

    float _fHintSetTime;
    float _fHintTimer;

    float _fTimer;
    float _fTimerAfter;
    float _fTimerBefore;

    string _sRemainTime = "";

    bool _bTimerPreccess;
    bool _bHint;

    float _fStageStartTime = 0;

    LTDescr BlinkTween = null;

    public void Init()
    {
        _bTimerPreccess = false;
        _bHint = false;

        _fTimer = 0;
        _fRemainTime = _fTotalTime;

        _label.text = _fTotalTime.ToString("N2");
    }

    void FixedUpdate()
    {
        if (_bTimerPreccess)
        {
            _fTimerAfter = Time.time;

            CalculateRemainTIme();
            Hint();

            _fTimer += _fTimerAfter - _fTimerBefore;

            _fTimerBefore = Time.time;
        }
        else
        {
            _fTimerBefore = Time.time;
        }
    }

    void CalculateRemainTIme()
    {
        _fRemainTime = _fTotalTime - _fTimer;

        if (_fRemainTime <= 0)
        {
            TimerEnd();
        }
        else
        {
            SetTimerText();
        }
    }

    void TimerEnd()
    {
        _bTimerPreccess = false;

        _label.text = "00.00";

        GameManager._Instance.END();
    }

    void SetTimerText()
    {
        _sRemainTime = _fRemainTime.ToString("N2");

        _label.text = _sRemainTime;
    }

    void Hint()
    {
        if (_bHint)
        {
            _fHintTimer = _fHintSetTime - _fRemainTime;

            if (_fHintTimer >= 5)
            {
                _bHint = false;

                GameManager._Instance.ShowHint();
            }
        }
    }

    public void ResetHint()
    {
        _bHint = true;
        _fHintSetTime = _fRemainTime;
    }

    public void BlinkStart()
    {
        BlinkTween = LeanTween.value(gameObject, 1f, 0f, 0.5f).setLoopPingPong().setOnUpdate(
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

    public void TimerStart()
    {
        _bTimerPreccess = true;

        _fStageStartTime = _fTimer;

        ResetHint();
    }

    public void TimerStop()
    {
        _bTimerPreccess = false;
    }

    public float GetStageSpendTime()
    {
        float stageSpendTime = _fTimer - _fStageStartTime;

        return stageSpendTime;
    }

    public void AddTime(float plusTime)
    {
        float Bonus = _fTotalTime + plusTime;

        LeanTween.value(gameObject, _fTotalTime, Bonus, 0.5f).setEase(LeanTweenType.easeOutCirc).setOnUpdate(
            (float value) =>
            {
                _fTotalTime = value;

                _fRemainTime = _fTotalTime - _fTimer;

                SetTimerText();
            }
        );
    }
}
