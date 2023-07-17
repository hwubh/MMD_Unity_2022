using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MMD_URP
{
    public class PMXVertex
    {
		public enum WeightMethod
		{
			BDEF1,
			BDEF2,
			BDEF4,
			SDEF,
			QDEF,
		}
		public struct PMXBoneWeight
		{
			public WeightMethod method;
			public BoneWeight1[] boneWeights;
			public Vector3 c_value;
			public Vector3 r0_value;
			public Vector3 r1_value;
			public byte boneCount;
		}
		public Vector3 pos; // x, y, z // ����
		public Vector3 normal; // nx, ny, nz // �����٥��ȥ�
		public Vector2 uv; // u, v // UV���� // MMD��픵�UV
		public Vector4[] additionUV; // x,y,z,w
		public PMXBoneWeight boneWeight;
		public float edgeScale;
	}
}
