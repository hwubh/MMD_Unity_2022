using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MMD_URP
{
    public class CustomAnimationClip
    {
        public string name { get; set; }
        public int frameRate { get; set; }

        public float timeLength = 0;

        public Dictionary<string, BoneCurves> bonesCurves = new Dictionary<string, BoneCurves>();

        public struct BoneCurves
        {
            public CustomAnimationCurve[] curves;

            public BoneCurves(CustomAnimationCurve[] curves)
            {
                this.curves = curves;
            }
        }

        public enum CurveType : byte
        {
            PositionX = 0,
            PositionY = 1,
            PositionZ = 2,
            QuaternionX = 3,
            QuaternionY = 4,
            QuaternionZ = 5,
            QuaternionW = 6,
        }

        public void SetCurve(string name, CurveType curveType, CustomAnimationCurve curve){
            if(bonesCurves.TryGetValue(name, out var boneCurves))
            {
                SetCurve(curveType, ref boneCurves, ref curve);
            }
            else
            {
                var tempCurves = new CustomAnimationCurve[7];
                var tempBoenCurves = new BoneCurves(tempCurves);
                SetCurve(curveType, ref tempBoenCurves, ref curve);
                bonesCurves.Add(name, tempBoenCurves);
            }
        }

        internal void SetCurve(CurveType curveType, ref BoneCurves boneCurves, ref CustomAnimationCurve curve)
        {
            switch (curveType)
            {
                case (CurveType)0:
                    {
                        boneCurves.curves[0] = curve;
                        break;
                    }
                case (CurveType)1:
                    {
                        boneCurves.curves[1] = curve;
                        break;
                    }
                case (CurveType)2:
                    {
                        boneCurves.curves[2] = curve;
                        break;
                    }
                case (CurveType)3:
                    {
                        boneCurves.curves[3] = curve;
                        break;
                    }
                case (CurveType)4:
                    {
                        boneCurves.curves[4] = curve;
                        break;
                    }
                case (CurveType)5:
                    {
                        boneCurves.curves[5] = curve;
                        break;
                    }
                case (CurveType)6:
                    {
                        boneCurves.curves[6] = curve;
                        break;
                    }
                default:
                    {
                        Debug.LogError("Unexpected curve added");
                        break;
                    }
            }
            curveLength(curve);
        }

        internal void curveLength(CustomAnimationCurve curve)
        {

                if (timeLength < curve.timeLength)
                    timeLength = curve.timeLength;
        }

        public void EnsureQuaternionContinuity(string boneName)
        {
            if (bonesCurves.TryGetValue(boneName, out var boneCurves))
            {
                var curves = boneCurves.curves;
                int keyCount = curves[4].keys.Count();

                Quaternion last = new Quaternion(curves[3].keys[keyCount - 1].value, curves[4].keys[keyCount - 1].value, curves[5].keys[keyCount - 1].value, curves[6].keys[keyCount - 1].value);
                for(int i = 0; i < keyCount; i++)
                {
                    Quaternion current = new Quaternion(curves[3].keys[i].value, curves[4].keys[i].value, curves[5].keys[i].value, curves[6].keys[i].value);
                    if (Quaternion.Dot(last, current) < 0)
                        current = new Quaternion(-current.x, -current.y, -current.z, -current.w);
                    last = current;
                    {
                        var tempKeyframe = curves[3].keys[i];
                        var tempKeyframe2 = curves[4].keys[i];
                        var tempKeyframe3 = curves[5].keys[i];
                        var tempKeyframe4 = curves[6].keys[i];

                        tempKeyframe.value = current.x;
                        tempKeyframe2.value = current.y;
                        tempKeyframe3.value = current.z;
                        tempKeyframe4.value = current.w;

                        curves[3].keys[i] = tempKeyframe;
                        curves[4].keys[i] = tempKeyframe2;
                        curves[5].keys[i] = tempKeyframe3;
                        curves[6].keys[i] = tempKeyframe4;
                    }
                }

            }
        }
    }
}
