using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MMD_URP
{
    public class VMDShadowKeyFrame
    {
        public int flame_no;
        public byte mode; //00-02
        public float distance;	// 0.1 - (dist * 0.00001)
    }
}
