using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace MMD_URP
{
    public class PMXSoftbody
    {
        public string softbodyName { get; internal set; } = string.Empty;
        public SoftBodyShape Shape { get; internal set; }
        public int TargetMaterial { get; internal set; }
        public byte Group { get; internal set; }
        public ushort GroupTarget { get; internal set; }
        public SoftBodyModeFlag Mode { get; internal set; }
        public int BLinkDistance { get; internal set; }
        public int ClusterCount { get; internal set; }
        public float TotalMass { get; internal set; }
        public float CollisionMargin { get; internal set; }
        public SoftBodyAeroModel AeroModel { get; internal set; }
        public SoftBodyConfig Config { get; internal set; } = null!;
        public SoftBodyCluster Cluster { get; internal set; } = null!;
        public SoftBodyIteration Iteration { get; internal set; } = null!;
        public SoftBodyMaterial Material { get; internal set; } = null!;
        public ReadOnlyMemory<AnchorRigidBody> AnchorRigidBodies { get; internal set; }
        public ReadOnlyMemory<int> PinnedVertex { get; internal set; }
        public enum SoftBodyShape : byte
        {
            TriBesh,
            Rope,
        }
        [Flags]
        public enum SoftBodyModeFlag : byte
        {
            CreateBLink = 0x01,
            CreateCluster = 0x02,
            CrossedLink = 0x04,
        }
        public class SoftBodyConfig
        {
            public float VCF { get; internal set; }
            public float DP { get; internal set; }
            public float DG { get; internal set; }
            public float LF { get; internal set; }
            public float PR { get; internal set; }
            public float VC { get; internal set; }
            public float DF { get; internal set; }
            public float MT { get; internal set; }
            public float CHR { get; internal set; }
            public float KHR { get; internal set; }
            public float SHR { get; internal set; }
            public float AHR { get; internal set; }
        }
        public enum SoftBodyAeroModel : int
        {
            VPoint = 0,
            VTwoSided = 1,
            VOneSided = 2,
            FTwoSided = 3,
            FOneSided = 4,
        }
        public class SoftBodyCluster
        {
            public float SRHR_CL { get; internal set; }
            public float SKHR_CL { get; internal set; }
            public float SSHR_CL { get; internal set; }
            public float SR_SPLT_CL { get; internal set; }
            public float SK_SPLT_CL { get; internal set; }
            public float SS_SPLT_CL { get; internal set; }
        }

        public class SoftBodyIteration
        {
            public int V_IT { get; internal set; }
            public int P_IT { get; internal set; }
            public int D_IT { get; internal set; }
            public int C_IT { get; internal set; }
        }

        public class SoftBodyMaterial
        {
            public float LST { get; internal set; }
            public float AST { get; internal set; }
            public float VST { get; internal set; }
        }
        [DebuggerDisplay("AnchorRigidBody (RigidBody={RigidBody}, Vertex={Vertex}, IsNearMode={IsNearMode})")]
        public struct AnchorRigidBody : IEquatable<AnchorRigidBody>
        {
            public int RigidBody { get; internal set; }
            public int Vertex { get; internal set; }
            public bool IsNearMode { get; internal set; }

            public override bool Equals(object? obj)
            {
                return obj is AnchorRigidBody body && Equals(body);
            }

            public bool Equals(AnchorRigidBody other)
            {
                return RigidBody == other.RigidBody &&
                       Vertex == other.Vertex &&
                       IsNearMode == other.IsNearMode;
            }

            public override int GetHashCode() => HashCode.Combine(RigidBody, Vertex, IsNearMode);

            public static bool operator ==(AnchorRigidBody left, AnchorRigidBody right)
            {
                return left.Equals(right);
            }

            public static bool operator !=(AnchorRigidBody left, AnchorRigidBody right)
            {
                return !(left == right);
            }
        }
    }
}
