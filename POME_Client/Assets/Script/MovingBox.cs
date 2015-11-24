using UnityEngine;
using System.Collections;

public class MovingBox : MonoBehaviour
{
    public Camera _MainCamera = null;

    public Renderer _Renderer = null;

    public Light _Light = null;

    private bool _bMove = false;

    private Vector3 MovePos;
    private Vector3 initMousePos;
    private float m_ZoomSpeed;

    Transform _MyTransform = null;

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
                Vector3 worldPoint = Input.mousePosition;
                worldPoint = _MainCamera.ScreenToWorldPoint(worldPoint);

                Vector3 diffPos = worldPoint - initMousePos;

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
            Color color = objrender.material.color;

            _Renderer.material.color = color;
            _Light.color = color;

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
