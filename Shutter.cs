using UnityEngine;
using System.Collections;

public class Shutter : MonoBehaviour
{
    public Animator _animator = null;

    public void PlayOpen()
    {
        _animator.Play("FXSemo_Open");
    }

    public void PlayClose()
    {
        _animator.Play("FXSemo_Close");
    }
}
