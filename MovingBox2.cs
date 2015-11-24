using UnityEngine;
using System.Collections;

public class MovingBox2 : MonoBehaviour
{
    private bool _bMove = false;

    private Vector3 MovePos;
    private Vector3 initMousePos;
    private float m_ZoomSpeed;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            initMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
        else if (Input.GetMouseButton(0))
        {
            Vector3 worldPoint = Input.mousePosition;
            worldPoint = Camera.main.ScreenToWorldPoint(worldPoint);

            Vector3 diffPos = worldPoint - initMousePos;

            initMousePos = Input.mousePosition;
            initMousePos = Camera.main.ScreenToWorldPoint(initMousePos);

            MovePos = transform.position + diffPos;
            MovePos.y = Mathf.SmoothDamp(transform.position.y, 4, ref m_ZoomSpeed, 0.1f);

            transform.position = MovePos;
        }
    }
}
