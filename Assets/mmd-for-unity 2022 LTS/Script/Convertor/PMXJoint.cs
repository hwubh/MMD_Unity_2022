using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MMD_URP
{
    public class PMXJoint
    {
		public enum OperationType
		{
			Spring6DOF, //スプリング6DOF
		}
		public string jointName;
		public OperationType operation_type;
		public int rigidbody_a; // 諸データ：剛体A 
		public int rigidbody_b; // 諸データ：剛体B 
		public Vector3 position; // 諸データ：位置(x, y, z) // 諸データ：位置合せでも設定可 
		public Vector3 rotation; // 諸データ：回転(rad(x), rad(y), rad(z)) 
		public Vector3 constrain_pos_lower; // 制限：移動1(x, y, z) 
		public Vector3 constrain_pos_upper; // 制限：移動2(x, y, z) 
		public Vector3 constrain_rot_lower; // 制限：回転1(rad(x), rad(y), rad(z)) 
		public Vector3 constrain_rot_upper; // 制限：回転2(rad(x), rad(y), rad(z)) 
		public Vector3 spring_position; // ばね：移動(x, y, z) 
		public Vector3 spring_rotation; // ばね：回転(rad(x), rad(y), rad(z)) 
	}
}

