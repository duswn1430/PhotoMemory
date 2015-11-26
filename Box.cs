using UnityEngine;
using System.Collections;
using Define;

public class Box
{
    static float _Size = 2.0f;
    static float _Dist = 0.0f;
    
    public int _Idx;

    public int _X;
    public int _Y;

    public Vector3 _Pos;

    public Type _OriginTpye;
    public Type _CurType;

    public STATE _State = STATE.NONE;

    public void Init()
    {
        _State = STATE.INIT;

        _OriginTpye = Type.NONE;
        _CurType = Type.NONE;

        _Pos = new Vector3((_Size + _Dist) * _X, 0.0f, (_Size + _Dist) * _Y);
    }

    public void Setting(Type type)
    {
        _State = STATE.IDLE;

        _OriginTpye = type;
        _CurType = type;
    }
}
