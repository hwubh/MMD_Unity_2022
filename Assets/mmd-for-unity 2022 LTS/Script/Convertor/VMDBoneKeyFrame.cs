using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MMD_URP
{
    public class VMDBoneKeyFrame
    {
		public uint motion_count;
		public Dictionary<string, List<Motion>> motion;
		public class Motion
		{
			public string bone_name;    // 15byte
			public uint flame_no;
			public Vector3 location;
			public Quaternion rotation;
			public byte[] interpolation;    // [4][4][4], 64byte

			// ¤Ê¤ó¤«²»±ã¤Ë¤Ê¤ê¤½¤¦¤ÊšÝ¤¬¤·¤Æ
			public byte GetInterpolation(int i, int j, int k)
			{
				return this.interpolation[i * 16 + j * 4 + k];
			}

			public void SetInterpolation(byte val, int i, int j, int k)
			{
				this.interpolation[i * 16 + j * 4 + k] = val;
			}
		}
	}
}
