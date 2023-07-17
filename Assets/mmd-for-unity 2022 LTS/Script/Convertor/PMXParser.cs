using System;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.Rendering;
using static MMD_URP.PMXVertex;

namespace MMD_URP
{
    public class PMXParser
    {
        private static BinaryReader binaryReader;
        private static Encoding encoding;

        /// <summary>Parse PMX data from specified file. This method is thread-safe.</summary>
        /// <param name="fileName">file name of PMX data</param>
        /// <returns>PMX object</returns>
        public static PMXObject Parse(string ModelPath)
        {
            if (!File.Exists(ModelPath)) { throw new FileNotFoundException("The file is not found (模型未找到)", ModelPath); }
            using (var stream = new FileStream(ModelPath, FileMode.Open, FileAccess.Read))
            {
                if (stream is null) { throw new ArgumentNullException(nameof(stream)); }
                using (binaryReader = new BinaryReader(stream))
                {
                    if (binaryReader is null) { throw new ArgumentNullException(nameof(binaryReader)); }
                    try
                    {
                        var obj = new PMXObject();
                        ParseHeader(obj);
                        ParseVertex(obj);
                        ParseSurface(obj);
                        ParseTexture(obj);
                        ParseMaterial(obj);
                        ParseBone(obj);
                        ParseMorph(obj);
                        ParseDisplayFrame(obj);
                        ParseRigidbody(obj);
                        ParseJoint(obj);
                        if (obj.header.Version < PMXVersion.V21)
                            obj.SoftbodyList = null;
                        else
                            ParseSoftbody(obj);
                        return obj;
                    }
                    finally
                    {
                        stream.Dispose();
                    }
                }
            }
        }
        //Functions to parse *.pmx into several parts
        #region Parse Functions
        //Get Header of PMX file
        private static void ParseHeader(PMXObject obj)
        {

            obj.header = new PMXHeader();
            var header = obj.header;
            var format = binaryReader.ReadBytes(4);
            if (Encoding.ASCII.GetString(format) != "PMX ")
            {
                throw new System.FormatException("NOT A *.pmx FILE(���벻��һ��*.pmx�ļ�)");
            }
            header.Version = (PMXVersion)(int)(binaryReader.ReadSingle() * 10);
            binaryReader.ReadByte();
            header.Encoding = binaryReader.ReadByte() switch
            {
                0 => Encoding.Unicode,
                1 => Encoding.UTF8,
                _ => throw new Exception("Invalid Transformation Format, can't get model information(δ֪�Ľ����ʽ, �޷���ȷ��ȡ)"),
            };
            encoding = header.Encoding;
            header.AdditionalUVCount = binaryReader.ReadByte();
            header.VertexIndexSize = binaryReader.ReadByte();
            header.TextureIndexSize = binaryReader.ReadByte();
            header.MaterialIndexSize = binaryReader.ReadByte();
            header.BoneIndexSize = binaryReader.ReadByte();
            header.MorphIndexSize = binaryReader.ReadByte();
            header.RigidBodyIndexSize = binaryReader.ReadByte();
            header.Name = string.Format("{0} ({1})", ReadString(), ReadString());
            header.Comment = ReadString();
            header.CommentEnglish = ReadString();
        }

        //Get vertex data of PMX file
        private static void ParseVertex(PMXObject obj)
        {
            int vertexCount = (int)binaryReader.ReadUInt32();
            obj.numberInfo[0] = vertexCount;
            obj.VertexList = new PMXVertex[vertexCount];
            var vertexList = obj.VertexList;
            var additionUVCount = obj.header.AdditionalUVCount;
            var boneIndexSize = obj.header.BoneIndexSize;
            for (int i = 0; i < vertexCount; ++i)
            {
                vertexList[i] = new PMXVertex();
                ReadVertex(vertexList[i], additionUVCount, boneIndexSize);
            }
        }
        //Read single Vertex Datum
        private static void ReadVertex(PMXVertex vertex, int additionUVCount, byte boneIndexSize)
        {

            vertex.pos = ReadSinglesToVector3(binaryReader);
            vertex.normal = ReadSinglesToVector3(binaryReader);
            vertex.uv = ReadSinglesToVector2(binaryReader);
            //reverse y axis
            vertex.uv.y = 1 - vertex.uv.y;
            vertex.additionUV = new Vector4[additionUVCount];
            for (int i = 0; i < additionUVCount; i++)
            {
                vertex.additionUV[i] = ReadSinglesToVector4(binaryReader);
            }
            var weightMethod = (PMXVertex.WeightMethod)binaryReader.ReadByte();
            var boneWeight = new PMXBoneWeight();
            switch (weightMethod)
            {
                case PMXVertex.WeightMethod.BDEF1:
                    ReadBoneWeightBDEF1(ref boneWeight, boneIndexSize);
                    break;
                case PMXVertex.WeightMethod.BDEF2:
                    ReadBoneWeightBDEF2(ref boneWeight, boneIndexSize);
                    break;
                case PMXVertex.WeightMethod.BDEF4:
                    ReadBoneWeightBDEF4(ref boneWeight, boneIndexSize);
                    break;
                case PMXVertex.WeightMethod.SDEF:
                    ReadBoneWeightSDEF(ref boneWeight, boneIndexSize);
                    break;
                case PMXVertex.WeightMethod.QDEF:
                    ReadBoneWeightQDEF(ref boneWeight, boneIndexSize);
                    break;
                default:
                    throw new System.FormatException("Invalid weight transform type(����Ĺ���Ȩ������)");
            }
            vertex.boneWeight = boneWeight;
            vertex.edgeScale = binaryReader.ReadSingle();
        }
        //Read triangles of meshes
        private static void ParseSurface(PMXObject obj)
        {

            int surfaceCount = (int)binaryReader.ReadUInt32();
            obj.numberInfo[1] = surfaceCount;
            obj.SurfaceList = new int[surfaceCount];
            var surfaceList = obj.SurfaceList;
            var vertexIndexSize = obj.header.VertexIndexSize;
            for (int i = 0; i < surfaceCount; ++i)
            {
                surfaceList[i] = CastIntRead(vertexIndexSize);
            }
        }
        // Read relative path of textures
        private static void ParseTexture(PMXObject obj)
        {
            int textureCount = (int)binaryReader.ReadUInt32();
            obj.TextureList = new string[textureCount];
            var textureList = obj.TextureList;
            for (int i = 0; i < textureCount; ++i)
            {
                textureList[i] = ReadString();
                //Skip "./" and replace "/" with "\\"
                if (('.' == textureList[i][0]) && (1 == textureList[i].IndexOfAny(new[] { '/', '\\' }, 1, 1)))
                {
                    textureList[i] = textureList[i].Substring(2);
                }
            }
        }
        //Read material properties
        private static void ParseMaterial(PMXObject obj)
        {
            int materialCount = (int)binaryReader.ReadUInt32();
            obj.MaterialList = new PMXMaterial[materialCount];
            var materialList = obj.MaterialList;
            Shader shader = null;
            if (GraphicsSettings.defaultRenderPipeline == null)
                shader = Shader.Find("Unlit/Texture");
            else
                shader = Shader.Find("Universal Render Pipeline/Unlit");
            for (int i = 0; i < materialCount; ++i)
            {
                materialList[i] = new PMXMaterial();
                ReadMaterial(materialList[i], shader, obj.header);
            }
        }
        //Create PMXMaterial
        private static void ReadMaterial(PMXMaterial PMXmaterial, Shader shader, PMXHeader header)
        {
            Material material = new Material(shader);
            PMXmaterial.material = material;
            var name = ReadString();
            var EnglishName = ReadString();
            material.name = string.Format("{0} ({1})", name, EnglishName);
            material.SetFloat("_OutlineWidth", 10.0f);
            material.SetColor("_OutlineColor", Color.black);
            material.SetFloat("_UseAlphaClipping", 1.0f);
            material.SetFloat("_Cutoff", 1.0f);
            Color tempColor = new Color(0f, 0f, 0f, 0f);
            material.color = ReadSinglesToColor(); // Diffuse Color
            material.SetColor("_Color", material.color);
            ReadSinglesToColor(tempColor, 1); // specular color
            material.SetColor("_SpecColor", tempColor);
            float specularStrength = binaryReader.ReadSingle();//Specular strength
            material.SetFloat("_Shininess", specularStrength);
            ReadSinglesToColor(tempColor, 1); // ambient color
            material.SetColor("_Ambient", tempColor);
            PMXmaterial.flag = (PMXMaterial.Flag)binaryReader.ReadByte();
            ReadSinglesToColor(tempColor); // Rim color
            material.SetColor("_EdgeColor", tempColor);
            float edgeSize = binaryReader.ReadSingle();// Edge size
            material.SetFloat("_EdgeSize", edgeSize);
            var materialIndexSize = header.MaterialIndexSize;
            PMXmaterial.diffuseTextureIndex = CastIntRead(materialIndexSize);// Diffuse texuture index
            PMXmaterial.environmentTextureIndex = CastIntRead(materialIndexSize);// Environment texuture index
            PMXmaterial.blendMode = (PMXMaterial.EnvironmentBlendMode)binaryReader.ReadByte();//Environment Blend Mode
            PMXmaterial.toonTexture = binaryReader.ReadByte();//Toon texture reference
            byte textureIndexSize = ((PMXmaterial.toonTexture == 0) ? header.TextureIndexSize : (byte)1);
            PMXmaterial.toonTextureIndex = CastIntRead(textureIndexSize);
            PMXmaterial.memo = ReadString();
            PMXmaterial.surfaceCount = (int)binaryReader.ReadUInt32(); // Number of surface
        }
        //Read bonelist
        private static void ParseBone(PMXObject obj)
        {

            int boneCount = (int)binaryReader.ReadUInt32();
            obj.BoneList = new PMXBone[boneCount];
            var boneList = obj.BoneList;
            var boneIndexSize = obj.header.BoneIndexSize;
            for (int i = 0; i < boneCount; ++i)
            {
                boneList[i] = new PMXBone();
                ReadBone(boneList[i], boneIndexSize);
            }
        }
        //Create PMXBone
        private static void ReadBone(PMXBone bone, byte boneIndexSize)
        {
            //bone.boneName = string.Format("{0} ({1})", ReadString(), ReadString());
            bone.boneName = ReadString();
            ReadString();
            bone.bone_position = ReadSinglesToVector3(binaryReader);
            bone.parent_bone_index = CastIntRead(boneIndexSize);
            bone.transform_level = (int)binaryReader.ReadUInt32();
            bone.bone_flag = (PMXBone.Flag)binaryReader.ReadUInt16();

            if ((bone.bone_flag & PMXBone.Flag.Connection) == 0)
            {
                // ���˥��ե��åȤ�ָ��
                bone.position_offset = ReadSinglesToVector3(binaryReader);
            }
            else
            {
                // �ܩ`���ָ��
                bone.connection_index = CastIntRead(boneIndexSize);
            }
            if ((bone.bone_flag & (PMXBone.Flag.AddRotation | PMXBone.Flag.AddMove)) != 0)
            {
                bone.additional_parent_index = CastIntRead(boneIndexSize);
                bone.additional_rate = binaryReader.ReadSingle();
            }
            if ((bone.bone_flag & PMXBone.Flag.FixedAxis) != 0)
            {
                bone.axis_vector = ReadSinglesToVector3(binaryReader);
            }
            if ((bone.bone_flag & PMXBone.Flag.LocalAxis) != 0)
            {
                bone.x_axis_vector = ReadSinglesToVector3(binaryReader);
                bone.z_axis_vector = ReadSinglesToVector3(binaryReader);
            }
            if ((bone.bone_flag & PMXBone.Flag.ExternalParentTransform) != 0)
            {
                bone.key_value = (int)(int)binaryReader.ReadUInt32();
            }
            if ((bone.bone_flag & PMXBone.Flag.IkFlag) != 0)
            {
                bone.ik_data = new PMXBone.IK_Data();
                ReadIkData(bone.ik_data, boneIndexSize);
            }
        }
        //Read Morphs
        private static void ParseMorph(PMXObject obj)
        {
            int morphCount = (int)binaryReader.ReadUInt32();
            obj.MorphList = new PMXMorph[morphCount];
            var morphList = obj.MorphList;
            for (int i = 0; i < morphCount; ++i)
            {
                morphList[i] = new PMXMorph();
                ReadMorphData(morphList[i], obj.header);
            }
        }
        //Create PMXMorph
        private static void ReadMorphData(PMXMorph morph, PMXHeader header)
        {
            morph.morphName = string.Format("{0} ({1})", ReadString(), ReadString());
            morph.handle_panel = (PMXMorph.Panel)binaryReader.ReadByte();
            morph.morph_type = (PMXMorph.MorphType)binaryReader.ReadByte();
            uint morph_offset_count = binaryReader.ReadUInt32();
            morph.morph_offset = new PMXMorph.MorphOffset[morph_offset_count];
            var morphOffsets = morph.morph_offset;
            for (int i = 0; i < morph_offset_count; ++i)
            {
                switch (morph.morph_type)
                {
                    case PMXMorph.MorphType.Group:
                    case PMXMorph.MorphType.Flip:
                        morphOffsets[i] = new PMXMorph.GroupMorphOffset();
                        ReadGroupMorphOffset((PMXMorph.GroupMorphOffset)morphOffsets[i], header.MorphIndexSize);
                        break;
                    case PMXMorph.MorphType.Vertex:
                        morphOffsets[i] = new PMXMorph.VertexMorphOffset();
                        ReadVertexMorphOffset((PMXMorph.VertexMorphOffset)morphOffsets[i], header.VertexIndexSize);
                        break;
                    case PMXMorph.MorphType.Bone:
                        morphOffsets[i] = new PMXMorph.BoneMorphOffset();
                        ReadBoneMorphOffset((PMXMorph.BoneMorphOffset)morphOffsets[i], header.BoneIndexSize);
                        break;
                    case PMXMorph.MorphType.Uv:
                    case PMXMorph.MorphType.Adduv1:
                    case PMXMorph.MorphType.Adduv2:
                    case PMXMorph.MorphType.Adduv3:
                    case PMXMorph.MorphType.Adduv4:
                        morphOffsets[i] = new PMXMorph.UVMorphOffset();
                        ReadUVMorphOffset((PMXMorph.UVMorphOffset)morphOffsets[i], header.VertexIndexSize);
                        break;
                    case PMXMorph.MorphType.Material:
                        morphOffsets[i] = new PMXMorph.MaterialMorphOffset();
                        ReadMaterialMorphOffset((PMXMorph.MaterialMorphOffset)morphOffsets[i], header.MaterialIndexSize);
                        break;
                    case PMXMorph.MorphType.Impulse:
                        morphOffsets[i] = new PMXMorph.ImpulseMorphOffset();
                        ReadImpulseMorphOffset((PMXMorph.ImpulseMorphOffset)morphOffsets[i], header.MorphIndexSize);
                        break;
                    default:
                        throw new System.FormatException("Invalid morph type(δ֪�ı�������)");
                }
            }
        }
        //Read DisplayFrame
        private static void ParseDisplayFrame(PMXObject obj)
        {
            int display_frame_count = (int)binaryReader.ReadUInt32();
            obj.DisplayFrameList = new PMXDisplayFrame[display_frame_count];
            var displayFrameList = obj.DisplayFrameList;
            for (int i = 0; i < display_frame_count; ++i)
            {
                displayFrameList[i] = new PMXDisplayFrame();
                ReadDisplayFrame(displayFrameList[i], obj.header.BoneIndexSize, obj.header.MorphIndexSize);
            }
        }
        //Create PMXDisplayFrame
        private static void ReadDisplayFrame(PMXDisplayFrame displayFrame, byte BoneIndexSize, byte MorphIndexSize)
        {
            displayFrame.displayName = string.Format("{0} ({1})", ReadString(), ReadString());
            displayFrame.special_frame_flag = binaryReader.ReadByte();
            int display_element_count = (int)binaryReader.ReadUInt32();
            displayFrame.display_element = new PMXDisplayFrame.DisplayElement[display_element_count];
            var displayElement = displayFrame.display_element;
            for (int i = 0; i < display_element_count; ++i)
            {
                displayElement[i] = new PMXDisplayFrame.DisplayElement();
                ReadDisplayElement(displayElement[i], BoneIndexSize, MorphIndexSize);
            }
        }
        //Create Display Element
        private static void ReadDisplayElement(PMXDisplayFrame.DisplayElement displayElement, byte BoneIndexSize, byte MorphIndexSize)
        {
            displayElement.element_target = binaryReader.ReadByte();
            byte element_target_index_size = ((displayElement.element_target == 0) ? BoneIndexSize : MorphIndexSize);
            displayElement.element_target_index = CastIntRead(element_target_index_size);
        }
        //Parse rigidbodies
        private static void ParseRigidbody(PMXObject obj)
        {		
            int rigidbody_count = (int)binaryReader.ReadUInt32();
            obj.RigidbodyList = new PMXRigidbody[rigidbody_count];
            var rigidbodyList = obj.RigidbodyList;
		    for (int i = 0; i < rigidbody_count; ++i) {
                rigidbodyList[i] = new PMXRigidbody();
                ReadRigidbody(rigidbodyList[i], obj.header.BoneIndexSize);
            }
        }
        //Create PMXRigidbody
        private static void ReadRigidbody(PMXRigidbody rigidbody, byte boneIndexSize)
        {
            rigidbody.rigidbodyName = string.Format("r_{0} ({1})", ReadString(), ReadString());
            rigidbody.rel_bone_index = CastIntRead(boneIndexSize);
            rigidbody.group_index = binaryReader.ReadByte();
            rigidbody.ignore_collision_group = (int) binaryReader.ReadUInt16();
            rigidbody.shape_type = (PMXRigidbody.ShapeType)binaryReader.ReadByte();
            rigidbody.shape_size = ReadSinglesToVector3(binaryReader);
            rigidbody.collider_position = ReadSinglesToVector3(binaryReader);
            rigidbody.collider_rotation = ReadSinglesToVector3(binaryReader);
            rigidbody.weight = binaryReader.ReadSingle();
            rigidbody.position_dim = binaryReader.ReadSingle();
            rigidbody.rotation_dim = binaryReader.ReadSingle();
            rigidbody.recoil = binaryReader.ReadSingle();
            rigidbody.friction = binaryReader.ReadSingle();
            rigidbody.operation_type = (PMXRigidbody.OperationType)binaryReader.ReadByte();
        }
        //Parse Joint
        private static void ParseJoint(PMXObject obj)
        {
            int jointCount = (int)binaryReader.ReadUInt32();
            obj.JointList = new PMXJoint[jointCount];
            var jointList = obj.JointList;
            for (int i = 0; i < jointCount; ++i)
            {
                jointList[i] = new PMXJoint();
                ReadJoint(jointList[i], obj.header.RigidBodyIndexSize);
            }
        }
        //Create PMXJoint
        private static void ReadJoint(PMXJoint joint, byte rigidbodyIndexSize)
        {

            joint.jointName = string.Format("{0} ({1})", ReadString(), ReadString());
            switch ((PMXJoint.OperationType)binaryReader.ReadByte())
            {
                case PMXJoint.OperationType.Spring6DOF:
                    joint.rigidbody_a = CastIntRead(rigidbodyIndexSize);
                    joint.rigidbody_b = CastIntRead(rigidbodyIndexSize);
                    joint.position = ReadSinglesToVector3(binaryReader);
                    joint.rotation = ReadSinglesToVector3(binaryReader);
                    joint.constrain_pos_lower = ReadSinglesToVector3(binaryReader);
                    joint.constrain_pos_upper = ReadSinglesToVector3(binaryReader);
                    joint.constrain_rot_lower = ReadSinglesToVector3(binaryReader);
                    joint.constrain_rot_upper = ReadSinglesToVector3(binaryReader);
                    joint.spring_position = ReadSinglesToVector3(binaryReader);
                    joint.spring_rotation = ReadSinglesToVector3(binaryReader);
                    break;
                default:
                    //empty.
                    break;
            }
        }
        //Parse softbodies
        private static void ParseSoftbody(PMXObject obj)
        {
            var softbodyCount = (int)binaryReader.ReadUInt32();
            obj.SoftbodyList = new PMXSoftbody[softbodyCount];
            var softbodyList = obj.SoftbodyList;
            var materialIndexSize = obj.header.MaterialIndexSize;
            var vertexIndexSize = obj.header.VertexIndexSize;
            var rigidBodyIndexSize  = obj.header.RigidBodyIndexSize;
            for (int i = 0; i < softbodyCount; i++)
            {
                softbodyList[i] = new PMXSoftbody();
                var softbody = softbodyList[i];
                softbody.softbodyName = string.Format("{0} ({1})", ReadString(), ReadString());
                softbody.Shape = (PMXSoftbody.SoftBodyShape)binaryReader.ReadByte();
                softbody.TargetMaterial = CastIntRead(materialIndexSize);
                softbody.Group = binaryReader.ReadByte();
                softbody.GroupTarget = binaryReader.ReadUInt16();
                softbody.Mode = (PMXSoftbody.SoftBodyModeFlag)binaryReader.ReadByte();
                softbody.BLinkDistance = (int) binaryReader.ReadUInt32();
                softbody.ClusterCount = (int) binaryReader.ReadUInt32();
                softbody.TotalMass = binaryReader.ReadSingle();
                softbody.CollisionMargin = binaryReader.ReadSingle();
                softbody.AeroModel = (PMXSoftbody.SoftBodyAeroModel)(int) binaryReader.ReadUInt32();

                softbody.Config = new PMXSoftbody.SoftBodyConfig()
                {
                    VCF = binaryReader.ReadSingle(),
                    DP = binaryReader.ReadSingle(),
                    DG = binaryReader.ReadSingle(),
                    LF = binaryReader.ReadSingle(),
                    PR = binaryReader.ReadSingle(),
                    VC = binaryReader.ReadSingle(),
                    DF = binaryReader.ReadSingle(),
                    MT = binaryReader.ReadSingle(),
                    CHR = binaryReader.ReadSingle(),
                    KHR = binaryReader.ReadSingle(),
                    SHR = binaryReader.ReadSingle(),
                    AHR = binaryReader.ReadSingle(),
                };

                softbody.Cluster = new PMXSoftbody.SoftBodyCluster()
                {
                    SRHR_CL = binaryReader.ReadSingle(),
                    SKHR_CL = binaryReader.ReadSingle(),
                    SSHR_CL = binaryReader.ReadSingle(),
                    SR_SPLT_CL = binaryReader.ReadSingle(),
                    SK_SPLT_CL = binaryReader.ReadSingle(),
                    SS_SPLT_CL = binaryReader.ReadSingle(),
                };
                softbody.Iteration = new PMXSoftbody.SoftBodyIteration()
                {
                    V_IT = (int) binaryReader.ReadUInt32(),
                    P_IT = (int) binaryReader.ReadUInt32(),
                    D_IT = (int) binaryReader.ReadUInt32(),
                    C_IT = (int) binaryReader.ReadUInt32(),
                };
                softbody.Material = new PMXSoftbody.SoftBodyMaterial()
                {
                    LST = binaryReader.ReadSingle(),
                    AST = binaryReader.ReadSingle(),
                    VST = binaryReader.ReadSingle(),
                };

                var anchorRigidBodyCount = (int) binaryReader.ReadUInt32();
                var anchors = new PMXSoftbody.AnchorRigidBody[anchorRigidBodyCount];
                softbody.AnchorRigidBodies = anchors;
                for (int j = 0; j < anchors.Length; j++)
                {
                    anchors[j].RigidBody = CastIntRead(rigidBodyIndexSize);
                    anchors[j].Vertex = CastIntRead(vertexIndexSize);
                    anchors[j].IsNearMode = binaryReader.ReadByte() != 0;
                }

                var pinnedVertexCount = (int) binaryReader.ReadUInt32();
                var pinnedVertex = new int[pinnedVertexCount];
                softbody.PinnedVertex = pinnedVertex;
                for (int j = 0; j < pinnedVertex.Length; j++)
                {
                    pinnedVertex[j] = CastIntRead(vertexIndexSize);
                }
            }
        }
        #endregion
        //Helper functions
        #region ToolFucntions
        //Read string
        private static string ReadString()
        {
            string result = null;
            int stringLength = binaryReader.ReadInt32();
            byte[] buf = binaryReader.ReadBytes(stringLength);
            if(Encoding.Unicode == encoding)
                result = Encoding.Unicode.GetString(buf);
            if (Encoding.UTF8 == encoding)
                result = Encoding.UTF8.GetString(buf);
            if(result == null)
                throw new System.InvalidOperationException("Invalid string encoding format(δ֪���ַ��������ʽ)");
            return result;
        }
        private static Vector3 ReadSinglesToVector3(BinaryReader binaryReader)
        {
            var vector3 = new Vector3(0f, 0f, 0f);
            float value = binaryReader.ReadSingle();
            if (!float.IsNaN(value))
                vector3.x = value;
            value = binaryReader.ReadSingle();
            if (!float.IsNaN(value))
                vector3.y = value;
            value = binaryReader.ReadSingle();
            if (!float.IsNaN(value))
                vector3.z = value;
            return vector3;
        }

        private static Vector2 ReadSinglesToVector2(BinaryReader binaryReader)
        {
            var vector2 = new Vector2(0f, 0f);
            float value = binaryReader.ReadSingle();
            if (!float.IsNaN(value))
                vector2.x = value;
            value = binaryReader.ReadSingle();
            if (!float.IsNaN(value))
                vector2.y = value;
            return vector2;
        }

        private static Vector4 ReadSinglesToVector4(BinaryReader binaryReader)
        {
            var vector4 = new Vector4(0f, 0f, 0f, 0f);
            float value = binaryReader.ReadSingle();
            if (!float.IsNaN(value))
                vector4.x = value;
            value = binaryReader.ReadSingle();
            if (!float.IsNaN(value))
                vector4.y = value;
            value = binaryReader.ReadSingle();
            if (!float.IsNaN(value))
                vector4.z = value;
            value = binaryReader.ReadSingle();
            if (!float.IsNaN(value))
                vector4.w = value;
            return vector4;
        }

        private static Quaternion ReadSinglesToQuaternion(BinaryReader binaryReader)
        {
            var quaternion = new Quaternion(0f, 0f, 0f, 0f);
            float value = binaryReader.ReadSingle();
            if (!float.IsNaN(value))
                quaternion.x = value;
            value = binaryReader.ReadSingle();
            if (!float.IsNaN(value))
                quaternion.y = value;
            value = binaryReader.ReadSingle();
            if (!float.IsNaN(value))
                quaternion.z = value;
            value = binaryReader.ReadSingle();
            if (!float.IsNaN(value))
                quaternion.w = value;
            return quaternion;
        }

        private static void ReadBoneWeightBDEF1(ref PMXBoneWeight PMXboneWeight, byte boneIndexSize)
        {
            PMXboneWeight.boneWeights = new BoneWeight1[1];
            var boneWeights = PMXboneWeight.boneWeights;
            PMXboneWeight.boneCount = 1;
            boneWeights[0] = new BoneWeight1();
            boneWeights[0].boneIndex = CastIntRead(boneIndexSize);
            boneWeights[0].weight = 1;
        }
        private static void ReadBoneWeightBDEF2(ref PMXBoneWeight PMXboneWeight, byte boneIndexSize)
        {
            PMXboneWeight.boneWeights = new BoneWeight1[2];
            var boneWeights = PMXboneWeight.boneWeights;
            PMXboneWeight.boneCount = 2;
            boneWeights[0] = new BoneWeight1();
            boneWeights[1] = new BoneWeight1();
            boneWeights[0].boneIndex = CastIntRead(boneIndexSize);
            boneWeights[1].boneIndex = CastIntRead(boneIndexSize);
            boneWeights[0].weight = binaryReader.ReadSingle();
            boneWeights[1].weight = 1 - boneWeights[0].weight;
        }
        private static void ReadBoneWeightBDEF4(ref PMXBoneWeight PMXboneWeight, byte boneIndexSize)
        {
            PMXboneWeight.boneWeights = new BoneWeight1[4];
            var boneWeights = PMXboneWeight.boneWeights;
            PMXboneWeight.boneCount = 4;
            boneWeights[0] = new BoneWeight1();
            boneWeights[1] = new BoneWeight1();
            boneWeights[2] = new BoneWeight1();
            boneWeights[3] = new BoneWeight1();
            boneWeights[0].boneIndex = CastIntRead(boneIndexSize);
            boneWeights[1].boneIndex = CastIntRead(boneIndexSize);
            boneWeights[2].boneIndex = CastIntRead(boneIndexSize);
            boneWeights[3].boneIndex = CastIntRead(boneIndexSize);
            boneWeights[0].weight = binaryReader.ReadSingle();
            boneWeights[1].weight = binaryReader.ReadSingle();
            boneWeights[2].weight = binaryReader.ReadSingle();
            boneWeights[3].weight = binaryReader.ReadSingle();
        }
        private static void ReadBoneWeightSDEF(ref PMXBoneWeight PMXboneWeight, byte boneIndexSize)
        {
            PMXboneWeight.boneWeights = new BoneWeight1[2];
            var boneWeights = PMXboneWeight.boneWeights;
            PMXboneWeight.boneCount = 2;
            boneWeights[0] = new BoneWeight1();
            boneWeights[1] = new BoneWeight1();
            boneWeights[0].boneIndex = CastIntRead(boneIndexSize);
            boneWeights[1].boneIndex = CastIntRead(boneIndexSize);
            boneWeights[0].weight = binaryReader.ReadSingle();
            boneWeights[1].weight = 1 - boneWeights[0].weight;
            PMXboneWeight.c_value = ReadSinglesToVector3(binaryReader);
            PMXboneWeight.r0_value = ReadSinglesToVector3(binaryReader);
            PMXboneWeight.r1_value = ReadSinglesToVector3(binaryReader);
        }
        private static void ReadBoneWeightQDEF(ref PMXBoneWeight PMXboneWeight, byte boneIndexSize)
        {
            PMXboneWeight.boneWeights = new BoneWeight1[4];
            var boneWeights = PMXboneWeight.boneWeights;
            PMXboneWeight.boneCount = 4;
            boneWeights[0] = new BoneWeight1();
            boneWeights[1] = new BoneWeight1();
            boneWeights[2] = new BoneWeight1();
            boneWeights[3] = new BoneWeight1();
            boneWeights[0].boneIndex = CastIntRead(boneIndexSize);
            boneWeights[1].boneIndex = CastIntRead(boneIndexSize);
            boneWeights[2].boneIndex = CastIntRead(boneIndexSize);
            boneWeights[3].boneIndex = CastIntRead(boneIndexSize);
            boneWeights[0].weight = binaryReader.ReadSingle();
            boneWeights[1].weight = binaryReader.ReadSingle();
            boneWeights[2].weight = binaryReader.ReadSingle();
            boneWeights[3].weight = binaryReader.ReadSingle();
        }
        private static int CastIntRead(byte index_size)
        {
            int result = 0;
            switch (index_size)
            {
                case (byte) 1 :
                    result = binaryReader.ReadByte();
                    if (byte.MaxValue == result)
                    {
                        result = -1;
                    }
                    break;
                case (byte) 2:
                    result = binaryReader.ReadUInt16();
                    if (ushort.MaxValue == result)
                    {
                        result = -1;
                    }
                    break;
                case (byte) 4:
                    result = (int) binaryReader.ReadUInt32();
                    break;
                default:
                    throw new System.ArgumentOutOfRangeException();
            }
            return result;
        }

        private static Color ReadSinglesToColor()
        {
            var color = new Color(0f, 0f, 0f, 0f);
            float value = binaryReader.ReadSingle();
            if (!float.IsNaN(value))
                color.r = value;
            value = binaryReader.ReadSingle();
            if (!float.IsNaN(value))
                color.g = value;
            value = binaryReader.ReadSingle();
            if (!float.IsNaN(value))
                color.b = value;
            value = binaryReader.ReadSingle();
            if (!float.IsNaN(value))
                color.a = value;
            return color;
        }

        private static void ReadSinglesToColor(Color color)
        {
            float value = binaryReader.ReadSingle();
            if (!float.IsNaN(value))
                color.r = value;
            value = binaryReader.ReadSingle();
            if (!float.IsNaN(value))
                color.g = value;
            value = binaryReader.ReadSingle();
            if (!float.IsNaN(value))
                color.b = value;
            value = binaryReader.ReadSingle();
            if (!float.IsNaN(value))
                color.a = value;
        }
        private static Color ReadSinglesToColor(float fix_alpha)
        {
            var color = new Color(0f, 0f, 0f, fix_alpha);
            float value = binaryReader.ReadSingle();
            if (!float.IsNaN(value))
                color.r = value;
            value = binaryReader.ReadSingle();
            if (!float.IsNaN(value))
                color.g = value;
            value = binaryReader.ReadSingle();
            if (!float.IsNaN(value))
                color.b = value;
            return color;
        }
        private static void ReadSinglesToColor(Color color, float fix_alpha)
        {
            float value = binaryReader.ReadSingle();
            if (!float.IsNaN(value))
                color.r = value;
            value = binaryReader.ReadSingle();
            if (!float.IsNaN(value))
                color.g = value;
            value = binaryReader.ReadSingle();
            if (!float.IsNaN(value))
                color.b = value;
            color.a = fix_alpha;
        }

        private static void ReadIkData(PMXBone.IK_Data ik_Data, byte boneIndexSize)
        {
            ik_Data.ik_bone_index = CastIntRead(boneIndexSize);
            ik_Data.iterations = (int) binaryReader.ReadUInt32();
            ik_Data.limit_angle = binaryReader.ReadSingle();
            int ik_link_count = (int) binaryReader.ReadUInt32();
            ik_Data.ik_link = new PMXBone.IK_Link[ik_link_count];
            var ikLinks = ik_Data.ik_link;
            for (uint i = 0; i < ik_link_count; ++i)
            {
                ikLinks[i] = new PMXBone.IK_Link();
                ReadIkLink(ikLinks[i], boneIndexSize);
            }
        }
        private static void ReadIkLink(PMXBone.IK_Link ik_Link, byte boneIndexSize)
        {
            ik_Link.target_bone_index = CastIntRead(boneIndexSize);
            ik_Link.angle_limit = binaryReader.ReadByte();
            if (ik_Link.angle_limit == 1)
            {
                ik_Link.lower_limit = ReadSinglesToVector3(binaryReader);
                ik_Link.upper_limit = ReadSinglesToVector3(binaryReader);
            }
        }

        private static void ReadGroupMorphOffset(PMXMorph.GroupMorphOffset morphOffset, byte morphIndexSize)
        {
            morphOffset.morph_index = CastIntRead(morphIndexSize);
            morphOffset.morph_rate = binaryReader.ReadSingle();
        }
        private static void ReadVertexMorphOffset(PMXMorph.VertexMorphOffset morphOffset, byte vertexIndexSize)
        {
            morphOffset.vertex_index = CastIntRead(vertexIndexSize);
            morphOffset.position_offset = ReadSinglesToVector3(binaryReader);
        }
        private static void ReadBoneMorphOffset(PMXMorph.BoneMorphOffset morphOffset, byte boneIndexSize)
        {
            morphOffset.bone_index = CastIntRead(boneIndexSize);
            morphOffset.move_value = ReadSinglesToVector3(binaryReader);
            morphOffset.rotate_value = ReadSinglesToQuaternion(binaryReader);
        }
        private static void ReadUVMorphOffset(PMXMorph.UVMorphOffset morphOffset, byte vertexIndexSize)
        {
            morphOffset.vertex_index = CastIntRead(vertexIndexSize);
            morphOffset.uv_offset = ReadSinglesToVector4(binaryReader);
        }
        private static void ReadMaterialMorphOffset(PMXMorph.MaterialMorphOffset morphOffset, byte materialIndexSize)
        {
            morphOffset.material_index = CastIntRead(materialIndexSize);
            morphOffset.offset_method = (PMXMorph.MaterialMorphOffset.OffsetMethod) binaryReader.ReadByte();
            morphOffset.diffuse = ReadSinglesToColor();
            morphOffset.specular = ReadSinglesToColor(1);
            morphOffset.specularity = binaryReader.ReadSingle();
            morphOffset.ambient = ReadSinglesToColor(1);
            morphOffset.edge_color = ReadSinglesToColor();
            morphOffset.edge_size = binaryReader.ReadSingle();
            morphOffset.texture_coefficient = ReadSinglesToColor();
            morphOffset.sphere_texture_coefficient = ReadSinglesToColor();
            morphOffset.toon_texture_coefficient = ReadSinglesToColor();
        }
        private static void ReadImpulseMorphOffset(PMXMorph.ImpulseMorphOffset morphOffset, byte morphIndexSize)
        {
            morphOffset.rigidbody_index = CastIntRead(morphIndexSize);
            morphOffset.local_flag = binaryReader.ReadByte();
            morphOffset.move_velocity = ReadSinglesToVector3(binaryReader);
            morphOffset.rotation_torque = ReadSinglesToVector3(binaryReader);
        }
        #endregion
    }
}
