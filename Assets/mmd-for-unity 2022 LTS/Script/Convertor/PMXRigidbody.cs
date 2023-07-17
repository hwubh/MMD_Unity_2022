using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MMD_URP
{
    public class PMXRigidbody
    {
		public enum ShapeType
		{
			Sphere,     //��
			Box,        //��
			Capsule,    //���ץ���
		}
		public enum OperationType
		{
			Static,                 //�ܩ`��׷��
			Dynamic,                //��������
			DynamicAndPosAdjust,    //��������(Boneλ�úϤ�)
		}
		public string rigidbodyName; // 
		public Collider collider;
		public int rel_bone_index;// �T�ǩ`�����v�B�ܩ`�󷬺� 
		public byte group_index; // �T�ǩ`��������`�� 
		public int ignore_collision_group;
		public ShapeType shape_type;  // ��״��������(0:��1:�䡢2:���ץ���)
		public Vector3 shape_size;
		public Vector3 collider_position;    // λ�ã�λ��(x, y, z) 
		public Vector3 collider_rotation;    // λ�ã���ܞ(rad(x), rad(y), rad(z)) 
		public float weight; // �T�ǩ`�����|�� // 00 00 80 3F // 1.0
		public float position_dim; // �T�ǩ`�����ƄӜp // 00 00 00 00
		public float rotation_dim; // �T�ǩ`������ܞ�p // 00 00 00 00
		public float recoil; // �T�ǩ`�������k�� // 00 00 00 00
		public float friction; // �T�ǩ`����Ħ���� // 00 00 00 00
		public OperationType operation_type; // �T�ǩ`����������(0:Bone׷����1:�������㡢2:��������(Boneλ�úϤ�)) // 00 // Bone׷��
	}
}
