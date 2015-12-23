using UnityEngine;
using System.Collections;

public class Shutter : MonoBehaviour
{
    public Animator _animator = null;

    MeshRenderer[] renderers;

    void Start()
    {
        renderers = transform.GetComponentsInChildren<MeshRenderer>();

        Invoke("Init", 1);
    }

    void Init()
    {
        Invisible();

        transform.localPosition = new Vector3(0, 0, -500);
    }

    // 에니메이션에서 trigger로 호출.
    void Visible()
    {
        for (int i = 0; i < renderers.Length; ++i)
        {
            renderers[i].enabled = true;
        }
    }

    // 에니메이션에서 trigger로 호출.
    void Invisible()
    {
        for (int i = 0; i < renderers.Length; ++i)
        {
            renderers[i].enabled = false;
        }
    }

    public void PlayOpen()
    {
        _animator.Play("Shutter_Open");
    }

    public void PlayClose()
    {
        _animator.Play("Shutter_Close");
    }
}
