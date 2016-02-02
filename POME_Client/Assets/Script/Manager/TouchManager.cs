using UnityEngine;
using System.Collections;

public class TouchManager : MonoBehaviour
{
    public Camera _MainCamera = null;

    public BoxMapManager _BoxMapManager = null;

    private Transform _SellectBox1 = null;
    private Transform _SellectBox2 = null;

    public MovingBox _MovingBox = null;
    public GameObject _goMark = null;
    public Transform _transMark = null;

    public void Init()
    {
        _goMark.SetActive(false);
        _MovingBox.Visible(null, false);

        if (_SellectBox1 != null)
        {
            BoxVisible(_SellectBox1, true);
            _SellectBox1 = null;
        }

        if (_SellectBox2 != null)
        {
            BoxVisible(_SellectBox2, true);
            _SellectBox2 = null;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager._Step == GameManager.STEP.PLAY && GameManager._bPause == false)
            UpdateTouch();
    }

    // 터치 상황별 처리.
    void UpdateTouch()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = _MainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, _MainCamera.farClipPlane, _MainCamera.cullingMask))
            {
                if (hit.transform.CompareTag("Box"))
                {
                    Transform box = hit.transform;

                    OnTouchDown(box);
                }
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            Ray ray = _MainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, _MainCamera.farClipPlane, _MainCamera.cullingMask))
            {
                OnTouchUp(hit);
            }
        }
        else if (Input.GetMouseButton(0))
        {
            Ray ray = _MainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, _MainCamera.farClipPlane, _MainCamera.cullingMask))
            {
                if (hit.transform.CompareTag("Box"))
                {
                    Transform box = hit.transform;

                    OnTouchMove(box);
                }
            }
        }
    }

    // Down일때 박스1 & 무빙 박스 셋팅.
    void OnTouchDown(Transform obj)
    {
        _SellectBox1 = obj;
        BoxVisible(_SellectBox1, false);

        _goMark.SetActive(true);
        _transMark.position = obj.position;

        _MovingBox.Visible(obj, true);

        Sound._Instance.BoxUp();
    }

    // Up일때 교체 처리.
    void OnTouchUp(RaycastHit hit)
    {
        if (_SellectBox1 != null)
        {
            if (hit.transform.CompareTag("Box"))
            {
                _SellectBox2 = hit.transform;

                BoxChange();
            }
            else if (_SellectBox2 != null)
            {
                BoxChange();
            }
        }

        _goMark.SetActive(false);
        _MovingBox.Visible(null, false);
    }

    // Move일때 마크 포지션 셋팅.
    void OnTouchMove(Transform obj)
    {
        if (_SellectBox1 != null)
        {
            _SellectBox2 = obj;
            _transMark.position = obj.position;
        }
    }

    // 무빙박스가 나타날 동안 선택한 박스는 투명 처리.
    void BoxVisible(Transform obj, bool visible)
    {
        if (obj != null)
        {
            MeshRenderer renderer = obj.GetComponent<MeshRenderer>();

            renderer.enabled = visible;
        }
    }

    // 박스1 박스2 교체.
    void BoxChange()
    {
        if (_SellectBox1 == _SellectBox2)
        {
            BoxVisible(_SellectBox1, true);

            _SellectBox1 = null;
            _SellectBox2 = null;

            _MovingBox.Visible(null, false);

            return;
        }

        BoxVisible(_SellectBox1, true);
        _BoxMapManager.BoxChange(_SellectBox1, _SellectBox2);

        _SellectBox1 = null;
        _SellectBox2 = null;
    }
}
