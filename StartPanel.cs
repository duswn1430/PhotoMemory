﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class StartPanel : MonoBehaviour
{   
    [Serializable]
    public class Cube
    {
        public GameObject _Obj;
        public float _Time;
        public float _Distance;
        public LTDescr _Tween;
    }

    enum LAUNCH_STEP {NONE, LOGO, TITLE, CUBE, BUTTON };

    LAUNCH_STEP _Step = LAUNCH_STEP.NONE;

    bool _bDone = false;
    
    public GameObject _Logo = null;
    public GameObject _Title = null;
    public GameObject _CubeParent = null;

    public GameObject _btnStart = null;
    public GameObject _btnHelp = null;
    public GameObject _btnSound = null;
    public GameObject _btnRank = null;

    public List<Cube> _listCube;

    // Use this for initialization
    void Start()
    {
        TweenAlpha.Begin(_btnStart, 1f, 0);
        TweenAlpha.Begin(_btnHelp, 1f, 0);
        TweenAlpha.Begin(_btnSound, 1f, 0);
        TweenAlpha.Begin(_btnRank, 1f, 0);

    }

    public void MainLaunch()
    {
        _Step = LAUNCH_STEP.LOGO;
        
        StartCoroutine(LaunchProcess());
    }

    IEnumerator LaunchProcess()
    {
        while(_bDone == false)
        {
            switch(_Step)
            {
                case LAUNCH_STEP.LOGO:
                    {
                        TweenAlpha.Begin(_Logo, 1f, 0);
                        TweenAlpha.Begin(_Title, 1f, 1);

                        yield return new WaitForSeconds(2f);

                        _Step = LAUNCH_STEP.TITLE;
                    }
                    break;
                case LAUNCH_STEP.TITLE:
                    {
                        LeanTween.moveLocalY(_Title, 510.0F, 1.5f).setEase(LeanTweenType.easeOutCubic);
                        LeanTween.moveLocalY(_CubeParent, 0.0f, 3.0f).setEase(LeanTweenType.easeOutCubic);

                        yield return new WaitForSeconds(3.5f);

                        _Step = LAUNCH_STEP.CUBE;
                    }
                    break;
                case LAUNCH_STEP.CUBE:
                    {
                        SetCubesMove();

                        yield return new WaitForSeconds(0.5f);

                        _Step = LAUNCH_STEP.BUTTON;
                    }
                    break;
                case LAUNCH_STEP.BUTTON:
                    {
                        TweenAlpha.Begin(_btnStart, 1f, 1);
                        TweenAlpha.Begin(_btnHelp, 1f, 1);
                        TweenAlpha.Begin(_btnSound, 1f, 1);
                        TweenAlpha.Begin(_btnRank, 1f, 1);

#if !UNITY_EDITOR && !NO_AD
        GoogleAds._Instance.ShowBanner();
#endif
                        UIManager._BackStep = Define.BACK_STEP.MAIN;

                        _Step = LAUNCH_STEP.NONE;
                        _bDone = true;
                    }
                    break;
            }

            yield return new WaitForEndOfFrame();
        }
    }

    void SetCubesMove()
    {
        for (int i = 0; i < _listCube.Count; ++i)
        {
            Cube cube = _listCube[i];

            if (cube != null)
            {
                float to = cube._Obj.transform.localPosition.y + cube._Distance;

                cube._Tween = LeanTween.moveLocalY(cube._Obj, to, cube._Time).setLoopPingPong();
            }
        }
    }

    void ReleaseCubesMove()
    {
        for (int i = 0; i < _listCube.Count; ++i)
        {
            Cube cube = _listCube[i];

            if (cube != null)
            {
                LeanTween.cancel(cube._Tween.uniqueId);
            }
        }
    }
}
