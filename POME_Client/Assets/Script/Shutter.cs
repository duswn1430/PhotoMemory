using UnityEngine;
using System.Collections;

public class Shutter : MonoBehaviour
{
    public Animator _animator = null;

    void Start()
    {
        Invoke("Init", 1);
    }

    void Init()
    {
        transform.localPosition = new Vector3(0, 0, -50);
    }

    public void PlayOpen()
    {
        _animator.Play("FXSemoWB_Open");
    }

    public void PlayClose()
    {
        _animator.Play("FXSemoWB_Close");
    }
}
