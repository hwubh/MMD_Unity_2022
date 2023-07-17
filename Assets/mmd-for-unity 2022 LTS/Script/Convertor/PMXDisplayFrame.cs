using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MMD_URP
{
    public class PMXDisplayFrame
    {
        public string displayName;
        public byte special_frame_flag;
        public DisplayElement[] display_element;

        public class DisplayElement
        {
            public byte element_target;
            public int element_target_index;
        }
    }
}
