using UnityEngine;
using System.Collections;

public class StartPanel : MonoBehaviour
{
    enum LAUNCH_STEP {NONE, TITLE, CUBE, BUTTON };

    LAUNCH_STEP _Step = LAUNCH_STEP.NONE;

    bool _bDone = false;

    public GameObject _Title = null;
    public GameObject _Cube = null;
    public UISprite _btnStart = null;
    public UISprite _btnHelp = null;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void MainLaunch()
    {
        _Step = LAUNCH_STEP.TITLE;

        _btnStart.alpha = 0;
        _btnHelp.alpha = 0;

        StartCoroutine(LaunchProcess());
    }

    IEnumerator LaunchProcess()
    {
        while(_bDone == false)
        {
            switch(_Step)
            {
                case LAUNCH_STEP.TITLE:
                    {
                        LeanTween.moveLocalY(_Title, 512.5F, 2.0f).setEase(LeanTweenType.easeOutCubic);

                        yield return new WaitForSeconds(2.0f);

                        _Step = LAUNCH_STEP.CUBE;
                    }
                    break;
                case LAUNCH_STEP.CUBE:
                    {
                        LeanTween.moveLocalY(_Cube, 126.0f, 2.0f).setEase(LeanTweenType.easeOutCubic);

                        yield return new WaitForSeconds(2.0f);

                        _Step = LAUNCH_STEP.BUTTON;

                    }
                    break;
                case LAUNCH_STEP.BUTTON:
                    {
                        _btnStart.transform.localPosition = new Vector3(_btnStart.transform.localPosition.x, 255.5f, _btnStart.transform.localPosition.z);
                        _btnHelp.transform.localPosition = new Vector3(_btnHelp.transform.localPosition.x, 255.5f, _btnHelp.transform.localPosition.z);

                        yield return StartCoroutine(FadeInButtons());

                        _Step = LAUNCH_STEP.NONE;
                        _bDone = true;
                    }
                    break;
            }

            yield return new WaitForEndOfFrame();
        }
    }

    IEnumerator FadeInButtons()
    {
        float alpha = 0;

        while(alpha < 1)
        {
            _btnStart.alpha = alpha;
            _btnHelp.alpha = alpha;

            yield return new WaitForSeconds(0.05f);

            alpha += 0.1f;
        }

        _btnStart.alpha = 1;
        _btnHelp.alpha = 1;
    }
}
