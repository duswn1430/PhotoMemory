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

    public BOX_TYPE _OriginTpye;
    public BOX_TYPE _CurType;

    public BOX_STATE _State = BOX_STATE.NONE;

    public void Init()
    {
        _State = BOX_STATE.INIT;

        _OriginTpye = BOX_TYPE.NONE;
        _CurType = BOX_TYPE.NONE;

        _Pos = new Vector3((_Size + _Dist) * _X, 0.0f, (_Size + _Dist) * _Y);
    }

    public void Setting(BOX_TYPE type)
    {
        _State = BOX_STATE.IDLE;

        _OriginTpye = type;
        _CurType = type;
    }
}
