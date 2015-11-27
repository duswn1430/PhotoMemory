using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Define;
using System.Linq;

public class BoxMapManager : MonoBehaviour
{
    public CameraControl _stCameraControl = null;

    public GameManager _GameManager = null;

    public GameObject _BoxPrefab = null;
    public GameObject _DustPrefab = null;

    public UISprite _UIShutter = null;

    BoxMapData _stMapData = null;

    List<Box> _BoxList = null;
    List<Transform> _BoxTrans = null;

    //public int _iRow;
    public List<int> _listCol;

    Transform _MyTransform = null;

    private bool _bTouchLock = false;
    private float _fSpwanTime = 0.2f;

    Hint _Hint1 = null;
    Hint _Hint2 = null;
    
    void Awake()
    {
        Init();
    }

    public void Init()
    {
        _BoxList = new List<Box>();
        _BoxTrans = new List<Transform>();
        _listCol = new List<int>();

        _Hint1 = new Hint();
        _Hint2 = new Hint();

        _MyTransform = transform;

        //_iRow = 0;
        _listCol.Clear();

    }

    public IEnumerator SetBoxMap(BoxMapData mapdata)
    {
        if (_bTouchLock) yield break;

        MapSetting(mapdata);

        yield return StartCoroutine(SpwanBox());

        SettingTartgets();
    }

    // 맵 셋팅.
    void MapSetting(BoxMapData mapdata)
    {
        _stMapData = mapdata;

        for (int row = 0; row < mapdata.iRow; row++)
        {
            if (_listCol.Count < row + 1)
            {
                _listCol.Add(0);
            }

            for (int col = _listCol[row]; col < mapdata.iCol; col++)
            {
                Box box = new Box();
                box._Idx = _BoxList.Count;
                box._X = row;
                box._Y = col;

                _BoxList.Add(box);
            }
            _listCol[row] = mapdata.iCol;
        }
    }

    // 박스 추가.
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

        SetBoxPrefab(obj, box);

        return obj;

    }

    // 박스 오브젝트 셋팅.
    void SetBoxPrefab(Transform obj, Box box)
    {
        obj.name = box._Idx.ToString();
        obj.parent = _MyTransform;
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

        int colorNum = _stMapData.iColorType;
        int colorCnt = _stMapData.iColorVolume;

        List<Box> listBox = _BoxList.ToList();

        for (int i = 0; i < listBox.Count; ++i)
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

        for (int i = 0; i < _BoxList.Count; ++i)
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
        for (int i = 0; i < _BoxList.Count; ++i)
        {
            Box box = _BoxList[i];

            if (box._OriginTpye != box._CurType)
                return false;
        }
        return true;
    }

    // 박스 체인지.
    public void BoxChange(Transform obj1, Transform obj2)
    {
        int idx1 = _BoxTrans.FindIndex(row => row == obj1);
        int idx2 = _BoxTrans.FindIndex(row => row == obj2);

        Box box1 = _BoxList[idx1];
        Box box2 = _BoxList[idx2];

        Type tmpType = box1._CurType;
        box1._CurType = box2._CurType;
        box2._CurType = tmpType;

        AmiscGame.SetColor(obj1, box1._CurType);
        AmiscGame.SetColor(obj2, box2._CurType);

        LeanTween.moveLocalY(obj1.gameObject, 0, 0.2f).setFrom(-6).setEase(LeanTweenType.easeSpring);
        LeanTween.moveLocalY(obj2.gameObject, 0, 0.2f).setFrom(6).setEase(LeanTweenType.easeSpring);

        Transform obj = Instantiate(_DustPrefab).transform;
        obj.position = obj2.position;

        DismissHint();

        if (IsComplete())
        {
            _GameManager.COMPLETE(_stMapData);
        }
        else
        {
            _GameManager.resetHint();
        }            
    }


    // 박스맵 리셋.
    public void ClearBoxMap()
    {
        if (_bTouchLock) return;

        for (int i = 0; i < _BoxTrans.Count; ++i)
        {
            Destroy(_BoxTrans[i].gameObject);
        }

        _BoxTrans.Clear();
        _BoxList.Clear();

        //_iRow = 0;
        _listCol.Clear();

        _stCameraControl.TargetsClear();
    }

    // 정답 보여주기.
    public void ShowOriginBox()
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

    public void ShowHint()
    {
        List<Box> listColorBox = _BoxList.Where(row => row._OriginTpye != Type.NONE && row._OriginTpye != row._CurType).ToList();

        int colorRnd = Random.Range(0, listColorBox.Count);

        Box cBox = listColorBox[colorRnd];


        List<Box> listNoneBox = _BoxList.Where(row => row._CurType == cBox._OriginTpye && row._OriginTpye != row._CurType).ToList();

        int noneRnd = Random.Range(0, listNoneBox.Count);

        Box nBox = listNoneBox[noneRnd];

        _Hint1.obj = _BoxTrans[cBox._Idx].gameObject;
        _Hint1.pos = _BoxTrans[cBox._Idx].position;
        _Hint1.tween = LeanTween.moveY(_Hint1.obj, 0.5f, 0.5f).setEase(LeanTweenType.punch).setLoopClamp();

        _Hint2.obj = _BoxTrans[nBox._Idx].gameObject;
        _Hint2.pos = _BoxTrans[nBox._Idx].position;
        _Hint2.tween = LeanTween.moveY(_Hint2.obj, 0.5f, 0.5f).setEase(LeanTweenType.punch).setLoopClamp();
    }

    public void DismissHint()
    {
        if (_Hint1.tween != null)
        {
            LeanTween.cancel(_Hint1.obj, _Hint1.tween.uniqueId);
            _Hint1.obj.transform.position = _Hint1.pos;

        }

        if (_Hint1.tween != null)
        {
            LeanTween.cancel(_Hint2.obj, _Hint2.tween.uniqueId);
            _Hint2.obj.transform.position = _Hint2.pos;
        }
    }

    public void ShowOrigin()
    {
        for (int i = 0; i < _BoxList.Count; ++i)
        {
            Box box = _BoxList[i];
            Transform obj = _BoxTrans[box._Idx];

            AmiscGame.SetColor(obj, box._OriginTpye);
        }
    }

    public void ShowCur()
    {
        for (int i = 0; i < _BoxList.Count; ++i)
        {
            Box box = _BoxList[i];
            Transform obj = _BoxTrans[box._Idx];

            AmiscGame.SetColor(obj, box._CurType);
        }
    }
}
