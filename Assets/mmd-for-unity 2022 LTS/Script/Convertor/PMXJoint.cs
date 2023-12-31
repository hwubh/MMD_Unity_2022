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
		public int rigidbody_a; // �Tデ�`タ����悶A 
		public int rigidbody_b; // �Tデ�`タ����悶B 
		public Vector3 position; // �Tデ�`タ�採志�(x, y, z) // �Tデ�`タ�採志炭呂擦任瞑O協辛 
		public Vector3 rotation; // �Tデ�`タ�沙憇�(rad(x), rad(y), rad(z)) 
		public Vector3 constrain_pos_lower; // 崙�泯災���1(x, y, z) 
		public Vector3 constrain_pos_upper; // 崙�泯災���2(x, y, z) 
		public Vector3 constrain_rot_lower; // 崙�泯沙憇�1(rad(x), rad(y), rad(z)) 
		public Vector3 constrain_rot_upper; // 崙�泯沙憇�2(rad(x), rad(y), rad(z)) 
		public Vector3 spring_position; // ばね�災���(x, y, z) 
		public Vector3 spring_rotation; // ばね�沙憇�(rad(x), rad(y), rad(z)) 
	}
}

