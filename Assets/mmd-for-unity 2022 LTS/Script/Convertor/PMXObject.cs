using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MMD_URP
{
    public class PMXObject
    {
        /// <summary>Get PMX file header</summary>
        public PMXHeader header { get; internal set; }
        /// <summary>Get number of each list</summary>
        public int[] numberInfo = new int[10];
        /// <summary>Get <see cref="Vertex"/> list</summary>
        public PMXVertex[] VertexList { get; internal set; }

        /// <summary>Get <see cref="Surface"/> list</summary>
        public int[] SurfaceList { get; internal set; }

        /// <summary>Get list of texture file path</summary>
        public string[] TextureList { get; internal set; }

        /// <summary>Get <see cref="Material"/> list</summary>
        public PMXMaterial[] MaterialList { get; internal set; }

        /// <summary>Get <see cref="Bone"/> list</summary>
        public PMXBone[] BoneList { get; internal set; }

        /// <summary>Get <see cref="Morph"/> list</summary>
        public PMXMorph[] MorphList { get; internal set; }

        /// <summary>Get <see cref="DisplayFrame"/> list</summary>
        public PMXDisplayFrame[] DisplayFrameList { get; internal set; }

        /// <summary>Get <see cref="RigidBody"/> list</summary>
        public PMXRigidbody[] RigidbodyList { get; internal set; }

        /// <summary>Get <see cref="Joint"/> list</summary>
        public PMXJoint[] JointList { get; internal set; }

        /// <summary>Get <see cref="SoftBody"/> list</summary>
        public PMXSoftbody[] SoftbodyList { get; internal set; }

    }
    public enum PMXVersion
    {
        /// <summary>PMX Ver 2.0</summary>
        V20 = 20,
        /// <summary>PMX Ver 2.1</summary>
        V21 = 21,
    }
    public enum IndexSize
    {
        Byte1 = 1,
        Byte2 = 2,
        Byte4 = 4,
    }
}

