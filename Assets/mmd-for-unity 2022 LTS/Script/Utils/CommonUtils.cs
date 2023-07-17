using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace MMD_URP
{
    public class CommonUtils
    {
        public static string GetTransformPath(Transform transform)
        {
            string buf;
            if (transform.parent == null)
                return transform.name;
            else
                buf = GetTransformPath(transform.parent);
            return buf + "/" + transform.name;
        }

        public static void SaveTreeInDictionary(Dictionary<string, GameObject> obj, GameObject root)
        {
            for (int i = 0; i < root.transform.childCount; i++)
            {
                var transf = root.transform.GetChild(i);
                try
                {
                    obj.Add(transf.name, transf.gameObject);
                }
                catch (System.ArgumentException arg)
                {
                    Debug.Log(arg.Message);
                    Debug.Log("An element with the same key already exists in the dictionary. -> " + transf.name);
                }

                if (transf == null) continue;       // ¥¹¥È¥Ã¥Ñ©`
                SaveTreeInDictionary(obj, transf.gameObject);
            }
        }

        public static string GetPathWithoutExtension(string fbxAssetPath, int periodPos)
        {
            if (string.IsNullOrEmpty(fbxAssetPath))
                return (string)null;
            int length = fbxAssetPath.Length;
            return length >= periodPos && fbxAssetPath[length - periodPos] == '.' ? fbxAssetPath.Substring(0, length - periodPos) : Path.Combine(Path.GetDirectoryName(fbxAssetPath), Path.GetFileNameWithoutExtension(fbxAssetPath));
        }
    }
}
