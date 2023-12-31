using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MMD_URP
{
    public class PMXRigidbody
    {
		public enum ShapeType
		{
			Sphere,     //白
			Box,        //��
			Capsule,    //カプセル
		}
		public enum OperationType
		{
			Static,                 //ボ�`ン弖��
			Dynamic,                //麗尖處麻
			DynamicAndPosAdjust,    //麗尖處麻(Bone了崔栽せ)
		}
		public string rigidbodyName; // 
		public Collider collider;
		public int rel_bone_index;// �Tデ�`タ�咲v�Bボ�`ン桑催 
		public byte group_index; // �Tデ�`タ�坤哀覃`プ 
		public int ignore_collision_group;
		public ShapeType shape_type;  // 侘彜�坤織ぅ�(0:白、1:�筺�2:カプセル)
		public Vector3 shape_size;
		public Vector3 collider_position;    // 了崔�採志�(x, y, z) 
		public Vector3 collider_rotation;    // 了崔�沙憇�(rad(x), rad(y), rad(z)) 
		public float weight; // �Tデ�`タ�細|楚 // 00 00 80 3F // 1.0
		public float position_dim; // �Tデ�`タ�災����p // 00 00 00 00
		public float rotation_dim; // �Tデ�`タ�沙憇��p // 00 00 00 00
		public float recoil; // �Tデ�`タ�嵯完k薦 // 00 00 00 00
		public float friction; // �Tデ�`タ�債Σ疏� // 00 00 00 00
		public OperationType operation_type; // �Tデ�`タ�坤織ぅ�(0:Bone弖悻�1:麗尖處麻、2:麗尖處麻(Bone了崔栽せ)) // 00 // Bone弖��
	}
}
