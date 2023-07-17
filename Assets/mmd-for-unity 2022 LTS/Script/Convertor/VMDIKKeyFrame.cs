using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MMD_URP
{
    public class VMDIKKeyFrame
    {
        public int flame_no;
        public bool isShow;
        public int boneCount;
        public IKbone[] ikBones;
        public class IKbone
        {
            public string boneName;
            public bool isIKOn;
        }

    }
}
