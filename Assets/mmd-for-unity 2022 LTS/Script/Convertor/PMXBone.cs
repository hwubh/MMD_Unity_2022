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
			Connection = 0x0001, //接続先(PMD子ボーン指定)表示方法(ON:ボーンで指定、OFF:座標オフセットで指定)
			Rotatable = 0x0002, //回転可能
			Movable = 0x0004, //移動可能
			DisplayFlag = 0x0008, //表示
			CanOperate = 0x0010, //操作可
			IkFlag = 0x0020, //IK
			AddLocal = 0x0080, //ローカル付与 | 付与対象(ON:親のローカル変形量、OFF:ユーザー変形値／IKリンク／多重付与)
			AddRotation = 0x0100, //回転付与
			AddMove = 0x0200, //移動付与
			FixedAxis = 0x0400, //軸固定
			LocalAxis = 0x0800, //ローカル軸
			PhysicsTransform = 0x1000, //物理後変形
			ExternalParentTransform = 0x2000, //外部親変形
		}
		public string boneName; // Name of Bone
		public Vector3 bone_position;
		public int parent_bone_index; // 親ボーン番号(ない場合はuint.MaxValue)
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
			public int ik_bone_index; // IKボーン番号
			public int iterations; // 再帰演算回数 // IK値1
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
