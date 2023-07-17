using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MMD_URP
{
    public class PMXMaterial
    {
		[Flags]
		public enum Flag
		{
			Reversible = 0x00000001, //�I���軭
			CastShadow = 0x00000010, //����Ӱ
			CastSelfShadow = 0x00000100, //����ե���ɥ��ޥåפؤ��軭
			ReceiveSelfShadow = 0x00001000, //����ե���ɥ����軭
			Edge = 0x00010000, //���å��軭
		}
		public enum EnvironmentBlendMode
		{
			Null,       //�o��
			MulSphere,  //�\�㥹�ե���
			AddSphere,  //���㥹�ե���
			SubTexture, //���֥ƥ�������
		}
		public Material material;
		public Flag flag;
		public int diffuseTextureIndex;
		public int environmentTextureIndex;
		public EnvironmentBlendMode blendMode;
		public byte toonTexture;
		public int toonTextureIndex;
		public string memo;
		public int surfaceCount; // ��픵��� // ����ǥå����ˉ�Q������Ϥϡ����|0����혤˼���
	}
}
