using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace MMD_URP
{
    public class MaterialUtils
    {
        private static Texture2D tempTexture = null;
        public static IEnumerator  GetTextureHTTP(Material material, string path, string textureName)
        {
#if UNITY_EDITOR
            Debug.Log(path);
#endif
            using (UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(path))
            {
                yield return uwr.SendWebRequest();

                if (uwr.result == UnityWebRequest.Result.ConnectionError || uwr.result == UnityWebRequest.Result.ProtocolError)
                {
                    Debug.Log(uwr.error);
                }
                else
                {
                    // Get downloaded asset bundle
                    tempTexture = DownloadHandlerTexture.GetContent(uwr);
                    material.SetTexture(textureName, tempTexture);
                    //material.mainTexture = tempTexture;
                }
            }
        }
    }

}
