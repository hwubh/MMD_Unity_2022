using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MMD_URP
{
    public class PMXBone
    {
		[Flags]
		public enum Flag
		{
			Connection = 0x0001, //�ӾA��(PMD�ӥܩ`��ָ��)��ʾ����(ON:�ܩ`���ָ����OFF:���˥��ե��åȤ�ָ��)
			Rotatable = 0x0002, //��ܞ����
			Movable = 0x0004, //�Ƅӿ���
			DisplayFlag = 0x0008, //��ʾ
			CanOperate = 0x0010, //������
			IkFlag = 0x0020, //IK
			AddLocal = 0x0080, //��`���븶�� | ���댝��(ON:�H�Υ�`�����������OFF:��`���`���΂���IK��󥯣����ظ���)
			AddRotation = 0x0100, //��ܞ����
			AddMove = 0x0200, //�ƄӸ���
			FixedAxis = 0x0400, //�S�̶�
			LocalAxis = 0x0800, //��`�����S
			PhysicsTransform = 0x1000, //���������
			ExternalParentTransform = 0x2000, //�ⲿ�H����
		}
		public string boneName; // Name of Bone
		public Vector3 bone_position;
		public int parent_bone_index; // �H�ܩ`�󷬺�(�ʤ����Ϥ�uint.MaxValue)
		public int transform_level;
		public Flag bone_flag;
		public Vector3 position_offset;
		public int connection_index;
		public int additional_parent_index;
		public float additional_rate;
		public Vector3 axis_vector;
		public Vector3 x_axis_vector;
		public Vector3 z_axis_vector;
		public int key_value;
		public IK_Data ik_data;
		public class IK_Data
		{
			public int ik_bone_index; // IK�ܩ`�󷬺�
			public int iterations; // �َ�������� // IK��1
			public float limit_angle;
			public IK_Link[] ik_link;
		}
		public class IK_Link
		{
			public int target_bone_index;
			public byte angle_limit;
			public Vector3 lower_limit;
			public Vector3 upper_limit;
		}
	}
}
