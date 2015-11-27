using UnityEngine;
using System.Collections;

namespace Define
{
    public enum Type { NONE, RED, YELLOW, BLUE, ORANGE, GREEN, INDIGO, VIOLET };

    public enum STATE { NONE, INIT, IDLE, SELLECT };

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
}
