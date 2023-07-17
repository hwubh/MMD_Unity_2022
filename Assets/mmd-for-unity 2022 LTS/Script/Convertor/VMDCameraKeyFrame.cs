using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MMD_URP
{
    public class VMDCameraKeyFrame
    {
		public int flame_no;
		public float length;
		public Vector3 location;
		public Vector3 rotation;    // ������`��, X�S�Ϸ��Ť���ܞ���Ƥ���
		public byte[] interpolation;    // [6][4], 24byte(δ���^)
		public int viewing_angle;
		public byte perspective;    // 0:on 1:off

		public byte GetInterpolation(int i, int j)
		{
			return this.interpolation[i * 6 + j];
		}

		public void SetInterpolation(byte val, int i, int j)
		{
			this.interpolation[i * 6 + j] = val;
		}
	}
}
