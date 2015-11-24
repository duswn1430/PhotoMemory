using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Define;
using System.Linq;

public class BoxMapManager : MonoBehaviour
{
    public GameManager _GameManager = null;

    public CameraControl _stCameraControl = null;
        
    public List<Box> _BoxList = null;
    public List<Transform> _BoxTrans = null;
    public GameObject _BoxPrefab = null;

    public int _iLevel = 1;
    public int _iRow;
    public int _iCol;
    int _iMaxSize = 8;
    int _iMaxColorCnt = 7;
    int _iBoxCount = 0;

    private float _fSpwanTime = 0.2f;
    private float _fMoveTime = 0.2f;

    private bool _bTouchLock = false;

    public Color[] _Colors = null;

    //public UISprite _sprShutter = null;
    public UISprite _UIShutter = null;

    // Use this for initialization
    void Awake()
    {
        _BoxList = new List<Box>();
        _BoxTrans = new List<Transform>();

        //StartCoroutine(InitBoxMap());
    }

    public IEnumerator InitBoxMap()
    {
        AddBoxMap();
        AddBoxMap();
        AddBoxMap();
        AddBoxMap();

        yield return StartCoroutine(SpwanBox());

        SettingTartgets();
    }

    public IEnumerator GenerateBoxMap()
    {
        if (_bTouchLock) yield break;

        AddBoxMap();
        yield return StartCoroutine(SpwanBox());
        SettingTartgets();
    }

    // 맵 추가.
    void AddBoxMap()
    {
        if (_iRow < _iMaxSize || _iCol < _iMaxSize)
        {
            if (_iRow == _iCol)
            {
                for (int col = 0; col < _iCol; ++col)
                {
                    Box box = new Box();
                    box._Parent = transform;
                    //box._Name = string.Format("{0}({1},{2})", _iBoxCount++, _iRow, col);
                    box._Idx = _iBoxCount++;
                    box._X = _iRow;
                    box._Y = col;

                    _BoxList.Add(box);
                }

                _iRow++;
            }
            else if (_iRow > _iCol)
            {
                for (int row = 0; row < _iRow; ++row)
                {
                    Box box = new Box();
                    box._Parent = transform;
                    //box._Name = string.Format("{0}({1},{2})", _iBoxCount++, row, _iCol);
                    box._Idx = _iBoxCount++;
                    box._X = row;
                    box._Y = _iCol;

                    _BoxList.Add(box);
                }

                _iCol++;
            }
        }
        else
        {
            Debug.Log("SIZE FULL");
        }
    }

    // 맵 오브젝트 추가.
    IEnumerator SpwanBox()
    {
        _bTouchLock = true;

        List<Box> spawnList = _BoxList.Where(row => row._State == STATE.NONE).ToList();

        float rate = _fSpwanTime / spawnList.Count;

        for (int i = 0; i < spawnList.Count; ++i)
        {
            yield return new WaitForSeconds(rate);

            spawnList[i].Init();
            Transform boxTrans = CreateBox(spawnList[i]);

            _BoxTrans.Add(boxTrans);
        }

        _bTouchLock = false;
    }

    // 박스 오브젝트 생성.
    Transform CreateBox(Box box)
    {
        Transform obj = Instantiate(_BoxPrefab).transform;

        obj.name = box._Idx.ToString();
        obj.parent = box._Parent;
        obj.position = box._Pos;

        SetBoxPrefab(obj, box);

        return obj.transform;

    }

    // 박스 오브젝트 셋팅.
    void SetBoxPrefab(Transform obj, Box box)
    {
        obj.name = box._Idx.ToString();
        obj.parent = box._Parent;
        obj.position = box._Pos;
        AmiscGame.SetColor(obj, box._CurType);
    }

    // 카메라 타겟팅 잡아주기.
    void SettingTartgets()
    {
        Transform[] arrTransform = _BoxTrans.ToArray();

        _stCameraControl.m_Targets = arrTransform;
        _stCameraControl.FindAveragePosition();
        _stCameraControl.FindRequiredSize();
    }

    // 박스 컬러링.
    public void BoxColoring()
    {
        if (_bTouchLock) return;

        int colorNum = Mathf.Min(_iRow, _Colors.Length - 1);
        int colorCnt = Mathf.Min(_iCol - 1, _iMaxColorCnt);

        List<Box> listBox = _BoxList.ToList();

        for(int i=0; i< listBox.Count; ++i)
        {
            listBox[i].Init();
        }

        System.Random rd = new System.Random();
        for (int i = 1; i <= colorNum; ++i)
        {
            for (int k = 0; k < colorCnt; ++k)
            {
                int rnd = rd.Next(listBox.Count);
                Box box = listBox[rnd];
                box.Setting((Type)i);

                listBox.Remove(box);
            }
        }

        for(int i=0; i<_BoxList.Count; ++i)
        {
            Box box = _BoxList[i];
            Transform obj = _BoxTrans[box._Idx];

            AmiscGame.SetColor(obj, box._CurType);

            if (box._OriginTpye != Type.NONE)
                LeanTween.moveY(obj.gameObject, 1f, 1.0f).setEase(LeanTweenType.punch);
        }
    }

    // 박스 셔플링.
    public IEnumerator BoxShuffling()
    {
        if (_bTouchLock) yield break;

        _bTouchLock = true;

        yield return StartCoroutine(SetShutter(true));

        yield return StartCoroutine(Shuffle(_BoxList));

        for (int i = 0; i < _BoxList.Count; ++i)
        {
            Box box = _BoxList[i];
            Transform obj = _BoxTrans[box._Idx];

            AmiscGame.SetColor(obj, box._CurType);
        }

        yield return StartCoroutine(SetShutter(false));
        _bTouchLock = false;
    }

    // 리스트 섞기.
    IEnumerator Shuffle(List<Box> list)
    {
        int n = list.Count;
        System.Random rnd = new System.Random();

        while (n > 1)
        {
            int k = (rnd.Next(0, n) % n);
            n--;
            Type vType = list[k]._CurType;
            list[k]._CurType = list[n]._CurType;
            list[n]._CurType = vType;
        }

        if (IsComplete())
        {
            Debug.Log("안섞임 다시 셋팅.");
            StartCoroutine(Shuffle(list));
        }

        yield return new WaitForEndOfFrame();
    }

    // 셔터(추후 수정).
    IEnumerator SetShutter(bool fill)
    {
        int length = AmiscGame.arrShutterName.Length;
        string shutterName = "";

        if (fill)
        {
            _UIShutter.gameObject.SetActive(true);

            int cnt = 0;

            while (cnt < length)
            {
                shutterName = AmiscGame.arrShutterName[cnt];
                _UIShutter.spriteName = shutterName;

                yield return new WaitForSeconds(0.02f);

                cnt++;
            }
        }
        else
        {
            int cnt = length - 1;

            while (cnt > 0)
            {
                shutterName = AmiscGame.arrShutterName[cnt];
                _UIShutter.spriteName = shutterName;

                yield return new WaitForSeconds(0.02f);

                cnt--;
            }
            _UIShutter.gameObject.SetActive(false);
        }

    }

    // 완료 여부.
    public bool IsComplete()
    {
        for(int i=0; i<_BoxList.Count; ++i)
        {
            Box box = _BoxList[i];

            if (box._OriginTpye != box._CurType)
                return false;
        }

        return true;
    }

    // 박스 체인지.
    public void BoxChange(Transform inObj, Transform outObj)
    {
        int inIdx = _BoxTrans.FindIndex(row => row == inObj);
        int outIdx = _BoxTrans.FindIndex(row => row == outObj);

        Box inBox = _BoxList[inIdx];
        Box outBox = _BoxList[outIdx];

        Type tmpType = inBox._CurType;
        inBox._CurType = outBox._CurType;
        outBox._CurType = tmpType;

        AmiscGame.SetColor(inObj, inBox._CurType);
        AmiscGame.SetColor(outObj, outBox._CurType);

        LeanTween.moveLocalY(inObj.gameObject, 0, 0.2f).setFrom(-6).setEase(LeanTweenType.easeSpring);
        LeanTween.moveLocalY(outObj.gameObject, 0, 0.2f).setFrom(6).setEase(LeanTweenType.easeSpring);

        if (IsComplete())
            _GameManager.COMPLETE();
    }


    // 박스맵 리셋.
    public void ClearBoxMap()
    {
        if (_bTouchLock) return;

        for(int i=0; i<_BoxTrans.Count; ++i)
        {
            Destroy(_BoxTrans[i].gameObject);
        }

        _BoxTrans.Clear();
        _BoxList.Clear();

        _iRow = 0;
        _iCol = 0;
        _iBoxCount = 0;

        _stCameraControl.TargetsClear();
    }

    public void RefreshBoxMap()
    {
        for (int i = 0; i < _BoxList.Count; ++i)
        {
            Box box = _BoxList[i];
            Transform obj = _BoxTrans[box._Idx];

            AmiscGame.SetColor(obj, box._OriginTpye);

            if (box._OriginTpye != Type.NONE)
                LeanTween.moveY(obj.gameObject, 1f, 1.0f).setEase(LeanTweenType.punch);
        }

    }
}
