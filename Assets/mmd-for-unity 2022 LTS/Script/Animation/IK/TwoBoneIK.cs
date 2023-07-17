using UnityEngine;

namespace MMD_URP
{
    public class TwoBoneIK : MonoBehaviour
    {
        public Transform Root;
        public Transform Middle;
        public Transform Effector;
        public Transform Target;
        public Transform Pole;

        public Transform[] boneChains = new Transform[3];

        private Vector3 length = Vector3.zero;

        private void Update()
        {
            Vector3 a = Root.position;
            Vector3 b = Middle.position;
            Vector3 c = Effector.position;
            Vector3 target = Target.position;
            //Vector3 pole = Pole.position;

            float lenAB = Vector3.Distance(Root.position, Middle.position);
            float lenBC = Vector3.Distance(Middle.position, Effector.position);
            float lenAT = Mathf.Clamp(Vector3.Distance(a, target), 0.01f, lenAB + lenBC - 0.01f);

            float angleAB_AC0 = Vector3.Angle(Vector3.Normalize(c - a), Vector3.Normalize(b - a));
            float angleAB_BC0 = Vector3.Angle(Vector3.Normalize(a - b), Vector3.Normalize(c - b));

            // the cosine rule
            float angleAB_AC1 = Mathf.Acos(Mathf.Clamp((lenBC * lenBC - lenAB * lenAB - lenAT * lenAT) / (-2.0f * lenAB * lenAT), -1, 1)) * Mathf.Rad2Deg;
            float angleAB_BC1 = Mathf.Acos(Mathf.Clamp((lenAT * lenAT - lenAB * lenAB - lenBC * lenBC) / (-2.0f * lenAB * lenBC), -1, 1)) * Mathf.Rad2Deg;

            Vector3 axis0 = Vector3.Normalize(Vector3.Cross(c - a, b - a));

            //use ccdik to fix
            if (axis0.magnitude == 0)
            {
                boneChains[0] = Root;
                boneChains[1] = Middle;
                boneChains[2] = Effector;
                CCDIK.Solve(Target, 0, boneChains);
            }

            Quaternion rotation0 = Quaternion.AngleAxis(angleAB_AC1 - angleAB_AC0, Quaternion.Inverse(Root.rotation) * axis0);
            Quaternion rotation1 = Quaternion.AngleAxis(angleAB_BC1 - angleAB_BC0, Quaternion.Inverse(Middle.rotation) * axis0);

            Root.localRotation *= rotation0;
            Middle.localRotation *= rotation1;

            float angleAC_AT1 = Vector3.Angle(Vector3.Normalize(c - a), Vector3.Normalize(target - a));
            Vector3 axis1 = Vector3.Normalize(Vector3.Cross(c - a, target - a));
            Quaternion rotation2 = Quaternion.AngleAxis(angleAC_AT1, Quaternion.Inverse(Root.rotation) * axis1);
            Root.localRotation *= rotation2;
        }
    }
}