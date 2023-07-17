using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static MMD_URP.CustomAnimationClip;
using static MMD_URP.ImportData;

namespace MMD_URP
{
    public class CustomAnimator : MonoBehaviour
    {
        [NonSerialized] public float animationTime, time = 0f;
        [NonSerialized] public float deltaTime = 1f / 24f;
        [NonSerialized] public BoneManager skeleton;

        private Vector3 tempVector3 = Vector3.zero;
        private Quaternion tempQuaternion = Quaternion.identity;
        //private List<CustomAnimationClip> clips = new List<CustomAnimationClip>();
        private CustomAnimationClip clip = null;
        private bool isPlay = false;
        private bool isBaked = false;
        private Dictionary<Transform, CurveDataBaked[]> DataBaked = new Dictionary<Transform, CurveDataBaked[]>();
        private CustomAnimationCurve[] curveShown;
        private Transform TransformSelected;

        public void Update()
        {
            if (isPlay && Time.time > animationTime)
            {
                animationTime = Time.time + deltaTime;
                SetTransform(skeleton, clip);
            }
        }

        //public void PlayImmediately(BoneManager bones, MonoBehaviour instance)
        //{
        //    Application.targetFrameRate = 24;
        //    foreach(var clip in clips)
        //        instance.StartCoroutine(SetTransform(bones, clip));
        //}

        public void PlayImmediately(BoneManager bones)
        {
            skeleton = bones;
            isPlay = true;
        }

        public void PlayImmediately()
        {
            isPlay = true;
        }

        public void Bake(BoneManager bones)
        {
            skeleton = bones;

            var NumFrame = (int) Math.Round((clip.timeLength / deltaTime), 0);
            var bonesCurves = clip.bonesCurves;
            foreach (var boneName in clip.bonesCurves.Keys)
            {
                if (skeleton.boneTransforms.TryGetValue(boneName, out TransformSelected))
                {
                    var curveData = new CurveDataBaked[NumFrame];

                    var boneCurves = bonesCurves.GetValueOrDefault(boneName);
                    
                    for(int i = 0; i < NumFrame; i++)
                    {
                        
                        var tempVector3 = new Vector3();
                        var tempQuaternion = new Quaternion();

                        var curveTX = boneCurves.curves[0];
                        var keyframesTX = curveTX.keys;
                        tempVector3.x = LinearInterpolation(keyframesTX, ref curveTX.startFrameNum, ref curveTX.endFrameNum, i * (deltaTime));

                        var curveTY = boneCurves.curves[1];
                        var keyframesTY = curveTY.keys;
                        tempVector3.y = LinearInterpolation(keyframesTY, ref curveTY.startFrameNum, ref curveTY.endFrameNum, i * (deltaTime));

                        var curveTZ = boneCurves.curves[2];
                        var keyframesTZ = curveTZ.keys;
                        tempVector3.z = LinearInterpolation(keyframesTZ, ref curveTZ.startFrameNum, ref curveTZ.endFrameNum, i * (deltaTime));

                        var curveQX = boneCurves.curves[3];
                        var keyframesQX = curveQX.keys;
                        tempQuaternion.x = LinearInterpolation(keyframesQX, ref curveQX.startFrameNum, ref curveQX.endFrameNum, i * (deltaTime));

                        var curveQY = boneCurves.curves[4];
                        var keyframesQY = curveQY.keys;
                        tempQuaternion.y = LinearInterpolation(keyframesQY, ref curveQY.startFrameNum, ref curveQY.endFrameNum, i * (deltaTime));

                        var curveQZ = boneCurves.curves[5];
                        var keyframesQZ = curveQZ.keys;
                        tempQuaternion.z = LinearInterpolation(keyframesQZ, ref curveQZ.startFrameNum, ref curveQZ.endFrameNum, i * (deltaTime));

                        var curveQW = boneCurves.curves[6];
                        var keyframesQW = curveQW.keys;
                        tempQuaternion.w = LinearInterpolation(keyframesQW, ref curveQW.startFrameNum, ref curveQW.endFrameNum, i * (deltaTime));

                        var tempCurveDataBaked = new CurveDataBaked(tempVector3, tempQuaternion);
                        curveData[i] = tempCurveDataBaked;
                    }
                    for(int i = 0; i < 6; i++)
                        boneCurves.curves[i].Reset();

                    DataBaked.Add(TransformSelected, curveData);

                    isBaked = true;
                }
            }

        }

        public void Bake()
        {

            var NumFrame = (int)Math.Round((clip.timeLength / deltaTime), 0);
            var bonesCurves = clip.bonesCurves;
            foreach (var boneName in clip.bonesCurves.Keys)
            {
                if (skeleton.boneTransforms.TryGetValue(boneName, out TransformSelected))
                {
                    var curveData = new CurveDataBaked[NumFrame];

                    var boneCurves = bonesCurves.GetValueOrDefault(boneName);

                    for (int i = 0; i < NumFrame; i++)
                    {

                        var tempVector3 = new Vector3();
                        var tempQuaternion = new Quaternion();

                        var curveTX = boneCurves.curves[0];
                        var keyframesTX = curveTX.keys;
                        tempVector3.x = LinearInterpolation(keyframesTX, ref curveTX.startFrameNum, ref curveTX.endFrameNum, i * (deltaTime));

                        var curveTY = boneCurves.curves[1];
                        var keyframesTY = curveTY.keys;
                        tempVector3.y = LinearInterpolation(keyframesTY, ref curveTY.startFrameNum, ref curveTY.endFrameNum, i * (deltaTime));

                        var curveTZ = boneCurves.curves[2];
                        var keyframesTZ = curveTZ.keys;
                        tempVector3.z = LinearInterpolation(keyframesTZ, ref curveTZ.startFrameNum, ref curveTZ.endFrameNum, i * (deltaTime));

                        var curveQX = boneCurves.curves[3];
                        var keyframesQX = curveQX.keys;
                        tempQuaternion.x = LinearInterpolation(keyframesQX, ref curveQX.startFrameNum, ref curveQX.endFrameNum, i * (deltaTime));

                        var curveQY = boneCurves.curves[4];
                        var keyframesQY = curveQY.keys;
                        tempQuaternion.y = LinearInterpolation(keyframesQY, ref curveQY.startFrameNum, ref curveQY.endFrameNum, i * (deltaTime));

                        var curveQZ = boneCurves.curves[5];
                        var keyframesQZ = curveQZ.keys;
                        tempQuaternion.z = LinearInterpolation(keyframesQZ, ref curveQZ.startFrameNum, ref curveQZ.endFrameNum, i * (deltaTime));

                        var curveQW = boneCurves.curves[6];
                        var keyframesQW = curveQW.keys;
                        tempQuaternion.w = LinearInterpolation(keyframesQW, ref curveQW.startFrameNum, ref curveQW.endFrameNum, i * (deltaTime));

                        var tempCurveDataBaked = new CurveDataBaked(tempVector3, tempQuaternion);
                        curveData[i] = tempCurveDataBaked;
                    }
                    for (int i = 0; i < 6; i++)
                        boneCurves.curves[i].Reset();

                    DataBaked.Add(TransformSelected, curveData);

                    isBaked = true;
                }
            }

        }

        public void Rebake(CustomAnimationCurve[] curves, Transform transform)
        {
            var NumFrame = (int) Math.Round((clip.timeLength / deltaTime), 0);
            var curveData = DataBaked[transform];

            for (int i = 0; i < NumFrame; i++)
            {

                var tempVector3 = new Vector3();
                var tempQuaternion = new Quaternion();

                var curveTX = curves[0];
                var keyframesTX = curveTX.keys;
                tempVector3.x = LinearInterpolation(keyframesTX, ref curveTX.startFrameNum, ref curveTX.endFrameNum, i * (deltaTime));

                var curveTY = curves[1];
                var keyframesTY = curveTY.keys;
                tempVector3.y = LinearInterpolation(keyframesTY, ref curveTY.startFrameNum, ref curveTY.endFrameNum, i * (deltaTime));

                var curveTZ = curves[2];
                var keyframesTZ = curveTZ.keys;
                tempVector3.z = LinearInterpolation(keyframesTZ, ref curveTZ.startFrameNum, ref curveTZ.endFrameNum, i * (deltaTime));

                var curveQX = curves[3];
                var keyframesQX = curveQX.keys;
                tempQuaternion.x = LinearInterpolation(keyframesQX, ref curveQX.startFrameNum, ref curveQX.endFrameNum, i * (deltaTime));

                var curveQY = curves[4];
                var keyframesQY = curveQY.keys;
                tempQuaternion.y = LinearInterpolation(keyframesQY, ref curveQY.startFrameNum, ref curveQY.endFrameNum, i * (deltaTime));

                var curveQZ = curves[5];
                var keyframesQZ = curveQZ.keys;
                tempQuaternion.z = LinearInterpolation(keyframesQZ, ref curveQZ.startFrameNum, ref curveQZ.endFrameNum, i * (deltaTime));

                var curveQW = curves[6];
                var keyframesQW = curveQW.keys;
                tempQuaternion.w = LinearInterpolation(keyframesQW, ref curveQW.startFrameNum, ref curveQW.endFrameNum, i * (deltaTime));

                var tempCurveDataBaked = new CurveDataBaked(tempVector3, tempQuaternion);
                curveData[i] = tempCurveDataBaked;
            }
            for (int i = 0; i < 6; i++)
                curves[i].Reset();

            DataBaked[transform] = curveData;
        }

        public void Pause()
        {
            isPlay = false;
        }

        public void SetTime(float targetTime)
        {
            time = targetTime;
        }

        public bool GetCurve(string boneName, out CustomAnimationCurve[] Curves) {
            if (clip.bonesCurves.TryGetValue(boneName, out var boneCurves))
            {
                skeleton.boneTransforms.TryGetValue(boneName, out TransformSelected);
                Curves = curveShown = boneCurves.curves;
                return true;
            }
            Curves = null;
            return false;
        }

        public float GetValue(int index, string boneNme, float time)
        {
            if(skeleton.boneTransforms.TryGetValue(boneNme, out var transforms))
                if (DataBaked.TryGetValue(transforms, out var CurveDataBaked))
                    return GetCurveValue(index, ref CurveDataBaked[GetFrameByTime(time)].position, ref CurveDataBaked[GetFrameByTime(time)].rotation);

            return 0f;
        }

        private float GetCurveValue(int index, ref Vector3 vector3, ref Quaternion quaternion)
        {
            switch (index)
            {
                case 0:
                    return vector3.x;
                case 1:
                    return vector3.y;
                case 2:
                    return vector3.z;
                case 3:
                    return quaternion.x;
                case 4:
                    return quaternion.y;
                case 5:
                    return quaternion.z;
                case 6:
                    return quaternion.w;
            }
            return 0f;
        }

        private int GetFrameByTime(float time)
        {
            return (int) Math.Round((time / deltaTime), 0);
        }

        public void SetTransform(BoneManager bones, CustomAnimationClip clip)
        {
            var bonesCurves = clip.bonesCurves;
            foreach (var boneName in clip.bonesCurves.Keys)
            {
                if (bones.boneTransforms.TryGetValue(boneName, out var transforms))
                {
                    if(!isBaked)
                        LinearInterpolation(bonesCurves.GetValueOrDefault(boneName), transforms);
                    else
                    {
                        transforms.localPosition = DataBaked[transforms][(int) Math.Round((time / deltaTime), 0)].position;
                        transforms.localRotation = DataBaked[transforms][(int) Math.Round((time / deltaTime), 0)].rotation;
                    }
                }
            }
            time += deltaTime;
            if (time > clip.timeLength)
            {
                isPlay = false;
            }
        }

        internal void LinearInterpolation(BoneCurves boneCurves, Transform transforms)
        {
            var curveTX = boneCurves.curves[0];
            var keyframesTX = curveTX.keys;
            tempVector3.x = LinearInterpolation(keyframesTX, ref curveTX.startFrameNum, ref curveTX.endFrameNum, time);
            //Debug.Log("Time is: " + time + ",  PositionX is:" + tempVector3.x) ;

            var curveTY = boneCurves.curves[1];
            var keyframesTY = boneCurves.curves[1].keys;
            tempVector3.y = LinearInterpolation(keyframesTY, ref curveTY.startFrameNum, ref curveTY.endFrameNum, time);

            var curveTZ = boneCurves.curves[2];
            var keyframesTZ = curveTZ.keys;
            tempVector3.z = LinearInterpolation(keyframesTZ, ref curveTZ.startFrameNum, ref curveTZ.endFrameNum, time);

            var curveQX = boneCurves.curves[3];
            var keyframesQX = curveQX.keys;
            tempQuaternion.x = LinearInterpolation(keyframesQX, ref curveQX.startFrameNum, ref curveQX.endFrameNum, time);

            var curveQY = boneCurves.curves[4];
            var keyframesQY = curveQY.keys;
            tempQuaternion.y = LinearInterpolation(keyframesQY, ref curveQY.startFrameNum, ref curveQY.endFrameNum, time);

            var curveQZ = boneCurves.curves[5];
            var keyframesQZ = curveQZ.keys;
            tempQuaternion.z = LinearInterpolation(keyframesQZ, ref curveQZ.startFrameNum, ref curveQZ.endFrameNum, time);

            var curveQW = boneCurves.curves[6];
            var keyframesQW = curveQW.keys;
            tempQuaternion.w = LinearInterpolation(keyframesQW, ref curveQW.startFrameNum, ref curveQW.endFrameNum, time);

            transforms.localPosition = tempVector3;
            transforms.localRotation = tempQuaternion;
        }

        internal float LinearInterpolation(List<Keyframe> keyframes, ref int startFrameNum, ref int endFrameNum, float time)
        {
            var startFrame = keyframes[startFrameNum];
            var endFrame = keyframes[endFrameNum];
            //while (time < startFrame.time)
            //{
            //    startFrame = keyframes[--startFrameNum];
            //    endFrame = keyframes[--endFrameNum];
            //}


            while (time > endFrame.time)
            {
                if (endFrameNum == keyframes.Count() - 1)
                    return endFrame.value;

                startFrame = keyframes[++startFrameNum];
                endFrame = keyframes[++endFrameNum];
            }
            return Mathf.Lerp(startFrame.value, endFrame.value, GetInterpolationValue(startFrame.time, endFrame.time, time));
        }

        internal float GetInterpolationValue(float a, float b, float c)
        {
            return (c - a) / (b - a);
        }

        public void SetClip(CustomAnimationClip clip)
        {
            this.clip = clip;
        }

        //public void modifyKeyframe(int curveValueIndex, int index, float value, GameSystem.KeyframeActionState KeyframeAction)
        //{
        //    switch (KeyframeAction)
        //    {
        //        case GameSystem.KeyframeActionState.Add:
        //            {
        //                if(!curveShown[curveValueIndex].keysIndex.TryGetValue(index, out var keyframeIndex))
        //                {
        //                    curveShown[curveValueIndex].keys.Insert(IndexToInsert(curveShown[curveValueIndex].keys, index),new Keyframe(index * deltaTime, value));
        //                    curveShown[curveValueIndex].KeysIndexDicUpdate(curveShown[curveValueIndex].keys);
        //                    Rebake(curveShown, TransformSelected);
        //                    GameSystem.boneList._curveView.DrawCurve();
        //                }
        //                break;
        //            }
        //        case GameSystem.KeyframeActionState.Remove:
        //            {
        //                if (curveShown[curveValueIndex].keysIndex.TryGetValue(index, out var keyframeIndex))
        //                {
        //                    curveShown[curveValueIndex].keys.RemoveAt(keyframeIndex);
        //                    curveShown[curveValueIndex].KeysIndexDicUpdate(curveShown[curveValueIndex].keys);
        //                    Rebake(curveShown, TransformSelected);
        //                    GameSystem.boneList._curveView.DrawCurve();
        //                }
        //                break;
        //            }
        //        case GameSystem.KeyframeActionState.Modify:
        //            {
        //                if (curveShown[curveValueIndex].keysIndex.TryGetValue(index, out var keyframeIndex))
        //                {
        //                    var tempKeyframe = curveShown[curveValueIndex].keys[keyframeIndex];
        //                    tempKeyframe.value = value;
        //                    curveShown[curveValueIndex].keys[keyframeIndex] = tempKeyframe;
        //                    curveShown[curveValueIndex].KeysIndexDicUpdate(curveShown[curveValueIndex].keys);
        //                    Rebake(curveShown, TransformSelected);
        //                    GameSystem.boneList._curveView.DrawCurve();
        //                }
        //                break;
        //            }
        //    }
        //}

        private int IndexToInsert(List<Keyframe> keys, int index)
        {
            for (int i = keys.Count() - 1; i >= 0; i--)
                if ((int) Math.Round((keys[i].time / deltaTime), 0) <= index)
                    return i + 1;
            return keys.Count() + 1;
        }

        public struct CurveDataBaked
        {
            public Vector3 position;
            public Quaternion rotation;

            public CurveDataBaked(Vector3 position, Quaternion rotation)
            {
                this.position = position;
                this.rotation = rotation;
            }
        }
    }
}
