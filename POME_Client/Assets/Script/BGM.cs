using UnityEngine;
using System.Collections;

public class BGM : MonoBehaviour
{
    public static BGM _Instance = null;

    public AudioSource _audio = null;

    public AudioClip[] _Clips;

    enum SORT { FANTASY, RHYTHMICAL };

    SORT _Sort;

    void Awake()
    {
        _Instance = this;

        _Sort = SORT.FANTASY;
    }

    public void Play()
    {
        switch (_Sort)
        {
            case SORT.FANTASY:
                _audio.clip = _Clips[0];
                break;
            case SORT.RHYTHMICAL:
                _audio.clip = _Clips[1];
                break;
        }

        _audio.Play();
    }

    public void BGMChange()
    {
        _Sort = _Sort == SORT.FANTASY ? SORT.RHYTHMICAL : SORT.FANTASY;

        Play();
    }

}
