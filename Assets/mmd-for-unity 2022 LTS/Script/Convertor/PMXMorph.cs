using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MMD_URP
{
    public class PMXMorph
    {
		public enum Panel
		{
			Base,
			EyeBrow,
			Eye,
			Lip,
			Other,
		}
		public enum MorphType
		{
			Group,
			Vertex,
			Bone,
			Uv,
			Adduv1,
			Adduv2,
			Adduv3,
			Adduv4,
			Material,

			Flip,
			Impulse,
		}
		public string morphName; //°°±Ì«È√˚
		public Panel handle_panel;
		public MorphType morph_type;
		public MorphOffset[] morph_offset;
		public interface MorphOffset { };

		public class VertexMorphOffset : MorphOffset
		{
			public int vertex_index;
			public Vector3 position_offset;
		}
		public class UVMorphOffset : MorphOffset
		{
			public int vertex_index;
			public Vector4 uv_offset;
		}
		public class BoneMorphOffset : MorphOffset
		{
			public int bone_index;
			public Vector3 move_value;
			public Quaternion rotate_value;
		}
		public class MaterialMorphOffset : MorphOffset
		{
			public enum OffsetMethod
			{
				Mul,
				Add,
			}
			public int material_index;
			public OffsetMethod offset_method;
			public Color diffuse;
			public Color specular;
			public float specularity;
			public Color ambient;
			public Color edge_color;
			public float edge_size;
			public Color texture_coefficient;
			public Color sphere_texture_coefficient;
			public Color toon_texture_coefficient;
		}
		public class GroupMorphOffset : MorphOffset
		{
			public int morph_index;
			public float morph_rate;
		}

		public class ImpulseMorphOffset : MorphOffset
		{
			public int rigidbody_index;
			public byte local_flag;
			public Vector3 move_velocity;
			public Vector3 rotation_torque;
		}
	}
}
