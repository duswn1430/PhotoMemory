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

    // Update is called once per frame
    void Update()
    {
        UpdateTouch();
    }

    void UpdateTouch()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = _MainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, _MainCamera.farClipPlane, _MainCamera.cullingMask))
            {
                if(hit.transform.CompareTag("Box"))
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
                if (hit.transform.CompareTag("Box"))
                {
                    Transform box = hit.transform;

                    OnTouchUp(box);
                }
                else
                {
                    BoxVisible(_SellectBox1, true);

                    _SellectBox1 = null;
                    _SellectBox2 = null;

                    _goMark.SetActive(false);
                    _MovingBox.Visible(null, false);
                }
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

    void OnTouchDown(Transform obj)
    {
        if (GameManager._Step == GameManager.STEP.PLAY)
        {
            _SellectBox1 = obj;
            BoxVisible(_SellectBox1, false);

            _goMark.SetActive(true);
            _transMark.position = obj.position;

            _MovingBox.Visible(obj, true);

            Sound._Instance.Play("block_up");
        }
    }

    void OnTouchUp(Transform obj)
    {
        if(_SellectBox1 != null)
        {
            _SellectBox2 = obj;

            BoxChange();
        }
        else
        {
            if (_SellectBox1 != null)
                _SellectBox1 = null;

            if (_SellectBox2 != null)
                _SellectBox2 = null;
        }

        _goMark.SetActive(false);

        _MovingBox.Visible(null, false);
    }

    void OnTouchMove(Transform obj)
    {
        if (_SellectBox1 != null)
            _transMark.position = obj.position;
    }

    void BoxVisible(Transform obj, bool visible)
    {
        if (obj != null)
        {
            MeshRenderer renderer = obj.GetComponent<MeshRenderer>();

            renderer.enabled = visible;
        }
    }

    void BoxChange()
    {
        if(_SellectBox1 == _SellectBox2)
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
