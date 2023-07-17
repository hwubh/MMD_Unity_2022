using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MMD_URP
{
    public class VMDObject
    {
		public string name;
		public string path;
		public string folder;

		public VMDHeader header;
		public VMDBoneKeyFrame boneKeyFrameList;
		public VMDMorphKeyFrame[] morphKeyFrameList;
		public VMDCameraKeyFrame[] cameraKeyFrameList;
		public VMDLightKeyFrame[] lightKeyFrameList;
		public VMDShadowKeyFrame[] shadowKeyFrameList;
		public VMDIKKeyFrame[] ikKeyFrameList;
	}
}
