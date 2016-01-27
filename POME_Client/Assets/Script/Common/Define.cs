using UnityEngine;
using System.Collections;

namespace Define
{
    //public enum Type { NONE, RED, YELLOW, BLUE, ORANGE, GREEN, INDIGO, VIOLET };
    public enum BOX_TYPE { NONE, RED, BLUE, YELLOW, GREEN, PINK, WHITE, ORANGE };

    public enum BOX_STATE { NONE, INIT, IDLE, SELLECT };

    public enum BACK_STEP { NONE, QUIT, MAIN, GAME, PAUSE, RESULT, AD, HELP };

    public enum LANGUAGE { KR, EN, CN, JP };

    public enum HELP_TYPE { TUTORIAL, MAIN, GAME };

    public enum TOAST_TYPE { EXIT, ORIGIN };

    public class BoxMapData
    {
        public int idx;
        public int iRow;
        public int iCol;
        public int iColorType;
        public int iColorVolume;
        public int iBonusTerms;
        public int iBonusTime;
    }

    public class Hint
    {
        public LTDescr tween;
        public GameObject obj;
        public Vector3 pos;
    }
}
