using UnityEngine;
using System.Collections;

public class Sound : MonoBehaviour
{
    public static Sound _Instance = null;

    public AudioSource _audio = null;

    public AudioClip[] _Clips;

    void Awake()
    {
        _Instance = this;
    }

    public void Play(string clip)
    {
        for (int i = 0; i < _Clips.Length; ++i)
        {
            if (_Clips[i].name.Equals(clip))
            {
                _audio.clip = _Clips[i];
                _audio.Play();
            }

        }
    }
}
