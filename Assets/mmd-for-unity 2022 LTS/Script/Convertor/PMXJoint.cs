using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MMD_URP
{
    public class PMXJoint
    {
		public enum OperationType
		{
			Spring6DOF, //���ץ��6DOF
		}
		public string jointName;
		public OperationType operation_type;
		public int rigidbody_a; // �T�ǩ`��������A 
		public int rigidbody_b; // �T�ǩ`��������B 
		public Vector3 position; // �T�ǩ`����λ��(x, y, z) // �T�ǩ`����λ�úϤ��Ǥ��O���� 
		public Vector3 rotation; // �T�ǩ`������ܞ(rad(x), rad(y), rad(z)) 
		public Vector3 constrain_pos_lower; // ���ޣ��Ƅ�1(x, y, z) 
		public Vector3 constrain_pos_upper; // ���ޣ��Ƅ�2(x, y, z) 
		public Vector3 constrain_rot_lower; // ���ޣ���ܞ1(rad(x), rad(y), rad(z)) 
		public Vector3 constrain_rot_upper; // ���ޣ���ܞ2(rad(x), rad(y), rad(z)) 
		public Vector3 spring_position; // �Фͣ��Ƅ�(x, y, z) 
		public Vector3 spring_rotation; // �Фͣ���ܞ(rad(x), rad(y), rad(z)) 
	}
}

