using UnityEngine;
using System.Collections;

public class CameraControl : MonoBehaviour
{
    public float m_DampTime = 0.2f;
    public float m_ScreenEdgeBuffer = 4f;
    public float m_MinSize = 6.5f;

    public Transform _Floor = null;

    [HideInInspector]
    public Transform[] m_Targets;

    private Camera m_Camera;

    private Vector3 m_DesiredPosition;

    Transform _MyTransform = null;

    private void Awake()
    {
        m_Camera = GetComponentInChildren<Camera>();
        _MyTransform = transform;
    }

    public void FindAveragePosition()
    {
        Vector3 averagePos = new Vector3();
        int numTargets = 0;

        for (int i = 0; i < m_Targets.Length; i++)
        {
            if (!m_Targets[i].gameObject.activeSelf)
                continue;

            averagePos += m_Targets[i].position;
            numTargets++;
        }

        if (numTargets > 0)
            averagePos /= numTargets;

        averagePos.y = _MyTransform.position.y;

        m_DesiredPosition = averagePos;

        LeanTween.value(gameObject, _MyTransform.position, averagePos, 0.2f).setEase(LeanTweenType.easeOutCirc).setOnUpdate(
            (Vector3 value) =>
            {
                _MyTransform.position = value;
                _Floor.localPosition = new Vector3(value.x, 0, value.z + 1);
            }
        );
    }


    public void FindRequiredSize()
    {
        Vector3 desiredLocalPos = _MyTransform.InverseTransformPoint(m_DesiredPosition);

        float size = 0f;

        for (int i = 0; i < m_Targets.Length; i++)
        {
            if (!m_Targets[i].gameObject.activeSelf)
                continue;

            Vector3 targetLocalPos = _MyTransform.InverseTransformPoint(m_Targets[i].position);

            Vector3 desiredPosToTarget = targetLocalPos - desiredLocalPos;

            size = Mathf.Max(size, Mathf.Abs(desiredPosToTarget.y));

            size = Mathf.Max(size, Mathf.Abs(desiredPosToTarget.x) / m_Camera.aspect);
        }

        size += m_ScreenEdgeBuffer;

        size = Mathf.Max(size, m_MinSize);

        LeanTween.value(gameObject, m_Camera.orthographicSize, size, 0.3f).setEase(LeanTweenType.easeOutCubic).setOnUpdate(
            (float value) =>
            {
                m_Camera.orthographicSize = value;
            }
        );
    }

    public void TargetsClear()
    {
        m_Targets = null;
    }
}