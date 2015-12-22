using UnityEngine;
using System.Collections;

public class Effect : MonoBehaviour
{
    public EllipsoidParticleEmitter _emSmoke = null;
    public EllipsoidParticleEmitter _emCicle = null;

    public void Play()
    {
        _emSmoke.Emit();
        _emCicle.Emit();
    }
}
