using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace MMD_URP
{
    public class ImportData
    {
        public List<ModelData> models = new List<ModelData>();

        public struct ModelData
        {
            public GameObject characterRoot;
            public BoneManager boneManager;
            public CustomAnimator animator;

            public string PMXPath;
            public string VMDPath;
            public string TexPath;
        }

        //public class ModelData
        //{
        //    public GameObject characterRoot;
        //    public PlayableGraph graph;
        //    public AnimationClipPlayable animationClipPlayable;

        //    public string PMXPath;
        //    public string VMDPath;
        //    public string TexPath;
        //}

        public void Play()
        {
            try
            {
                models.ForEach((model) => { model.animator.PlayImmediately(model.boneManager); });
            }
            catch
            {
                Debug.LogError("Not such model or motion to Play");
            }
        }

        public void Play(CustomAnimator animator)
        {
            try
            {
                models.ForEach((model) => { if(animator == model.animator) model.animator.PlayImmediately(model.boneManager); });
            }
            catch
            {
                Debug.LogError("Not such model or motion to Play");
            }
        }

        public void Pause()
        {
            try
            {
                models.ForEach((model) => { model.animator.Pause(); });
            }
            catch
            {
                Debug.LogError("Not such model or motion to Pause");
            }
        }

        public void Pause(CustomAnimator animator)
        {
            try
            {
                models.ForEach((model) => { if (animator == model.animator) model.animator.Pause(); });
            }
            catch
            {
                Debug.LogError("Not such model or motion to Pause");
            }
        }

        public void Bake(CustomAnimator animator)
        {
            try
            {
                models.ForEach((model) => { if (animator == model.animator) model.animator.Bake(model.boneManager); });
            }
            catch
            {
                Debug.LogError("Not such model or motion to Bake");
            }
        }

        public void Bake()
        {
            try
            {
                models.ForEach((model) => { model.animator.Bake(model.boneManager); });
            }
            catch
            {
                Debug.LogError("Not such model or motion to Bake");
            }
        }

        public void Restart()
        {
            try
            {
                models.ForEach((model) => { model.animator.SetTime(0f); });
            }
            catch
            {
                Debug.LogError("No model or motion in the scene");
            }
        }

        public bool IsModelExist(GameObject gameObject, out BoneManager BoneManagerSelected)
        {
            foreach (var model in models)
            {
                if (model.characterRoot == gameObject)
                {
                    BoneManagerSelected = model.boneManager;
                    return true;
                }
            }
            BoneManagerSelected = null;
            return false;
        }

        public bool IsModelExist(GameObject gameObject, out CustomAnimator CustomAnimatorSelected)
        {
            foreach (var model in models)
            {
                if (model.characterRoot == gameObject)
                {
                    CustomAnimatorSelected = model.animator;
                    return true;
                }
            }
            CustomAnimatorSelected = null;
            return false;
        }

        //public bool IsModelExist(GameObject gameObject, out BoneManager BoneManagerSelected)
        //{
        //    foreach(var model in models)
        //    {
        //        if (model.characterRoot == gameObject)
        //        {
        //            BoneManagerSelected = model.boneManager;
        //            return true;
        //        }
        //    }
        //    BoneManagerSelected = null;
        //    return false;
        //}

        //public bool IsModelExist(GameObject gameObject, out CustomAnimator CustomAnimatorSelected)
        //{
        //    foreach (var model in models)
        //    {
        //        if (model.characterRoot == gameObject)
        //        {
        //            CustomAnimatorSelected = model.animator;
        //            return true;
        //        }
        //    }
        //    CustomAnimatorSelected = null;
        //    return false;
        //}

        //public void DestroyAnimations()
        //{
        //    try
        //    {
        //        models.ForEach((model) =>
        //        {
        //            model.animationClipPlayable.Destroy();
        //            model.graph.Destroy();
        //        });
        //    }
        //    catch
        //    {
        //        Debug.LogError("No such motion data in the scene");
        //    }
        //}

        //public void DestroyModels()
        //{
        //    try
        //    {
        //        models.ForEach((model) =>
        //        {
        //            GameObject.Destroy(model.characterRoot);
        //            model.animationClipPlayable.Destroy();
        //            model.graph.Destroy();
        //            models.Clear();
        //        });
        //    }
        //    catch
        //    {
        //        Debug.LogError("No such data in the scene");
        //    }
        //}

        public string PMXPath;
        public string VMDPath;
        public string TexPath;
    }
}
