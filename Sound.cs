using UnityEngine;
using System.Collections;

public class Sound : MonoBehaviour
{
    public static Sound _Instance = null;

    public AudioSource[] _audio = null;

    public AudioClip[] _Clips;

    void Awake()
    {
        _Instance = this;
    }

    void Start()
    {
        _audio = transform.GetComponents<AudioSource>();
    }

    public void Play(string clip)
    {
        for (int i = 0; i < _Clips.Length; ++i)
        {
            if (_Clips[i].name.Equals(clip))
            {
                SoundPlay(_Clips[i]);
            }

        }
    }

    void SoundPlay(AudioClip clip)
    {
        for (int i = 0; i < _audio.Length; ++i)
        {
            if (_audio[i].isPlaying)
                continue;

            _audio[i].clip = clip;
            _audio[i].Play();

            break;
        }
    }
}
