using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Effect : MonoBehaviour
{
    public List<EllipsoidParticleEmitter> _listEffect = null;

    public void Play()
    {
        for (int i = 0; i < _listEffect.Count; ++i)
        {
            EllipsoidParticleEmitter effect = _listEffect[i];
            effect.Emit();
        }
    }
}
