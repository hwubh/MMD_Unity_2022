using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace MMD_URP {
    public class CustomAnimationCurve
    {
        public List<Keyframe> keys = new List<Keyframe>();
        public Dictionary<int, int> keysIndex = new Dictionary<int, int>();
        public float timeLength = 0;
        public float minValue = 0;
        public float maxValue = 0;

        public int startFrameNum = 0;
        public int endFrameNum = 1;

        public CustomAnimationCurve(List<Keyframe> keys)
        {
            curveLength(keys);
            this.keys = keys;
            KeysIndexDicUpdate(keys);
        }

        public CustomAnimationCurve()
        {
            this.keys = null;
        }

        private void curveLength(List<Keyframe> keys)
        {
            for(int i = 0; i < keys.Count(); i++)
            {
                if (timeLength < keys[i].time)
                    timeLength = keys[i].time;

                if (minValue > keys[i].value)
                    minValue = keys[i].value;

                if (maxValue < keys[i].value)
                    maxValue = keys[i].value;
            }
        }

        public void KeysIndexDicUpdate(List<Keyframe> keys)
        {
            keysIndex.Clear();
            for (int i = 0; i < keys.Count(); i++)
            {
                    keysIndex.Add((int) Math.Round(keys[i].time / (1f / 24f), 0), i);
            }
        }
        public void Reset()
        {
            startFrameNum = 0;
            endFrameNum = 1;
        }

        //public void ModifyKeyframe()
        //{

        //}
    }
}
