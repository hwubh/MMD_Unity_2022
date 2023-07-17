using UnityEngine;

namespace MMD_URP
{
    public class CCDIK
    {
		//// Target
		//public Transform target;
		//// Loop count
		//public int iterations;
		//// rad limit
		//public float controll_weight;
		//public Transform[] chains;
		//public bool drawRay = false;


        public static void Solve(Transform target, float ControlWeight, Transform[] boneChains, bool drawRay = true, int iteration = 40)
		{
			for (int i = 0; i < iteration; i++)
			{
				for (int j = 0; j < boneChains.Length; j++)
				{
					var bone = boneChains[j];
					var bonePos = bone.position;

					var effectorPos = boneChains[0].position;
					var effectorDirection = effectorPos - bonePos;

					var targetDirection = (target.position - bonePos);

					if (drawRay)
					{
						Debug.DrawRay(bonePos, effectorDirection, Color.green);
						Debug.DrawRay(bonePos, targetDirection, Color.red);
					}

					// ÄÚ·e
					effectorDirection = effectorDirection.normalized;
					targetDirection = targetDirection.normalized;

					var rotate = Quaternion.FromToRotation(effectorDirection, targetDirection); 
					bone.rotation = rotate * bone.rotation;

					//limitter(bone);
				}
			}
		}
	}
}