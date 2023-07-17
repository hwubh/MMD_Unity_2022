using UnityEngine;
using UnityEditor;

namespace MMD_URP
{
    [CustomEditor(typeof(CustomAnimator))]
    public class CustomAnimatorEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            CustomAnimator customAnimator = target as CustomAnimator;
            if (GUILayout.Button("Bake"))
            {
                GameSystem.importData.Bake(customAnimator);
            }
            if (GUILayout.Button("Play"))
            {
                GameSystem.importData.Play(customAnimator);
            }
            if (GUILayout.Button("Pause"))
            {
                GameSystem.importData.Pause(customAnimator);
            }

            customAnimator.time = GUILayout.HorizontalSlider(customAnimator.time, 0, customAnimator.animationTime);
        }
    }
}
