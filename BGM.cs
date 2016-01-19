using UnityEngine;
using System.Collections;

public class BGM : MonoBehaviour
{
    public static BGM _Instance = null;

    public AudioSource _audio = null;

    public AudioClip[] _Clips;

    enum TRACK { NONE, T2, T5 };

    TRACK _Track;

    void Awake()
    {
        _Instance = this;

        _Track = TRACK.NONE;
    }

    void Play()
    {
        switch (_Track)
        {
            case TRACK.T2:
                _audio.clip = _Clips[0];
                break;
            case TRACK.T5:
                _audio.clip = _Clips[1];
                break;
        }

        _audio.Play();
    }

    public void PlayT2()
    {
        if (_Track == TRACK.T2) return;

        _Track = TRACK.T2;
        Play();
    }

    public void PlayT5()
    {
        if (_Track == TRACK.T5) return;

        _Track = TRACK.T5;
        Play();
    }

}
