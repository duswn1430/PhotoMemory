using UnityEngine;
using System.Collections;

public class MovingBox : MonoBehaviour
{
    public Camera _MainCamera = null;

    public Renderer _Renderer = null;

    private bool _bMove = false;

    private Vector3 MovePos;
    private Vector3 initMousePos;
    private float m_ZoomSpeed;

    Transform _MyTransform = null;

    Vector3 worldPoint;
    Vector3 diffPos;

    void Start()
    {
        _MyTransform = transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (_bMove)
        {
            if (Input.GetMouseButton(0))
            {
                worldPoint = Input.mousePosition;
                worldPoint = _MainCamera.ScreenToWorldPoint(worldPoint);

                diffPos = worldPoint - initMousePos;

                initMousePos = Input.mousePosition;
                initMousePos = _MainCamera.ScreenToWorldPoint(initMousePos);

                MovePos = _MyTransform.position + diffPos;
                MovePos.y = Mathf.SmoothDamp(_MyTransform.position.y, 4, ref m_ZoomSpeed, 0.1f);

                _MyTransform.position = MovePos;
            }
        }
    }

    public void Visible(Transform obj, bool visible)
    {
        if (visible)
        {
            gameObject.SetActive(true);
            _MyTransform.position = obj.position;

            MeshRenderer objrender = obj.GetComponent<MeshRenderer>();
            Color diffuse = objrender.material.GetColor("_Diffuse");
            Color emission = objrender.material.GetColor("_Emission");

            _Renderer.material.SetColor("_Diffuse", diffuse);
            _Renderer.material.SetColor("_Emission", emission);

            initMousePos = _MainCamera.ScreenToWorldPoint(Input.mousePosition);

            _bMove = true;
        }
        else
        {
            _bMove = false;
            gameObject.SetActive(false);
        }
    }
}
