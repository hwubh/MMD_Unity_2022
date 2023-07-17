using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MMD_URP
{
    public class PMXRigidbody
    {
		public enum ShapeType
		{
			Sphere,     //球
			Box,        //箱
			Capsule,    //カプセル
		}
		public enum OperationType
		{
			Static,                 //ボーン追従
			Dynamic,                //物理演算
			DynamicAndPosAdjust,    //物理演算(Bone位置合せ)
		}
		public string rigidbodyName; // 
		public Collider collider;
		public int rel_bone_index;// 諸データ：関連ボーン番号 
		public byte group_index; // 諸データ：グループ 
		public int ignore_collision_group;
		public ShapeType shape_type;  // 形状：タイプ(0:球、1:箱、2:カプセル)
		public Vector3 shape_size;
		public Vector3 collider_position;    // 位置：位置(x, y, z) 
		public Vector3 collider_rotation;    // 位置：回転(rad(x), rad(y), rad(z)) 
		public float weight; // 諸データ：質量 // 00 00 80 3F // 1.0
		public float position_dim; // 諸データ：移動減 // 00 00 00 00
		public float rotation_dim; // 諸データ：回転減 // 00 00 00 00
		public float recoil; // 諸データ：反発力 // 00 00 00 00
		public float friction; // 諸データ：摩擦力 // 00 00 00 00
		public OperationType operation_type; // 諸データ：タイプ(0:Bone追従、1:物理演算、2:物理演算(Bone位置合せ)) // 00 // Bone追従
	}
}
