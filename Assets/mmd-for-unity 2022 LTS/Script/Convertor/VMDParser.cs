using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace MMD_URP
{
    public class VMDParser
    {
        private static BinaryReader binaryReader;
        private static Encoding encoding;
        public static VMDObject Parse(string MotionPath)
		{
            if (!File.Exists(MotionPath)) { throw new FileNotFoundException("The file is not found (文件不存在)", MotionPath); }
            using (var stream = new FileStream(MotionPath, FileMode.Open, FileAccess.Read))
            {
                if (stream is null) { throw new ArgumentNullException(nameof(stream)); }
                using (binaryReader = new BinaryReader(stream))
                {
                    if (binaryReader is null) { throw new ArgumentNullException(nameof(binaryReader)); }
                    int read_count = 0;
                    var obj = new VMDObject();
                    try
                    {
                        obj.name = Path.GetFileNameWithoutExtension(MotionPath);
                        ParseHeader(obj); read_count++;
                        ParseBoneKeyFrame(obj); read_count++;
                        ParseMorphKeyFrame(obj);read_count++;
                        ParseCameraKeyFrame(obj);read_count++;
                        ParseLightKeyFrame(obj);read_count++;
                        ParseShadowKeyFrame(obj);read_count++;
                        ParseIKKeyFrame(obj); read_count++;
                        return obj;
                    }
                    catch (EndOfStreamException e)
                    {
                        Debug.Log(e.Message);
                        if (read_count <= 0)
                            obj.header = null;
                        if (read_count <= 1 || obj.boneKeyFrameList.motion_count <= 0)
                            obj.boneKeyFrameList = null;
                        if (read_count <= 2 || obj.morphKeyFrameList.Length <= 0)
                            obj.morphKeyFrameList = null;
                        if (read_count <= 3 || obj.cameraKeyFrameList.Length <= 0)
                            obj.cameraKeyFrameList = null;
                        if (read_count <= 4 || obj.lightKeyFrameList.Length <= 0)
                            obj.lightKeyFrameList = null;
                        if (read_count <= 5 || obj.shadowKeyFrameList.Length <= 0)
                            obj.shadowKeyFrameList = null;
                        if (read_count <= 6 || obj.ikKeyFrameList.Length <= 0)
                            obj.ikKeyFrameList = null;
                        return obj;
                    }
                    finally
                    {
                        stream.Dispose();
                    }
                }
            }
        }
        #region Parse Functions
        //Get Header of VMD File
        private static void ParseHeader(VMDObject obj)
        {
            var result = new VMDHeader();
            result.vmd_header = ConvertByteToString(binaryReader.ReadBytes(30), "");
            result.vmd_model_name = ConvertByteToString(binaryReader.ReadBytes(20), "");
            obj.header = result;
        }
        private static void ParseBoneKeyFrame(VMDObject obj)
        {
            VMDBoneKeyFrame result = new VMDBoneKeyFrame();
            result.motion_count = binaryReader.ReadUInt32();
            result.motion = new Dictionary<string, List<VMDBoneKeyFrame.Motion>>();

            // 一度バッファに貯めてソートする
            VMDBoneKeyFrame.Motion[] buf = new VMDBoneKeyFrame.Motion[result.motion_count];
            for (int i = 0; i < result.motion_count; i++)
            {
                buf[i] = new VMDBoneKeyFrame.Motion();
                ReadBoneKeyFrame(buf[i]);
            }
            Array.Sort(buf, (x, y) => ((int)x.flame_no - (int)y.flame_no));

            // モーションの数だけnewされないよね？
            for (int i = 0; i < result.motion_count; i++)
            {
                try { result.motion.Add(buf[i].bone_name, new List<VMDBoneKeyFrame.Motion>()); }
                catch { }
            }

            // dictionaryにどんどん登録
            for (int i = 0; i < result.motion_count; i++)
            {
                result.motion[buf[i].bone_name].Add(buf[i]);
            }
            obj.boneKeyFrameList = result;
        }
        private static void ReadBoneKeyFrame(VMDBoneKeyFrame.Motion frame)
        {
            frame.bone_name = ConvertByteToString(binaryReader.ReadBytes(15), "");
            frame.flame_no = binaryReader.ReadUInt32();
            frame.location = ReadSinglesToVector3(binaryReader);
            frame.rotation = ReadSinglesToQuaternion(binaryReader);
            frame.interpolation = binaryReader.ReadBytes(64);
        }
        private static void ParseMorphKeyFrame(VMDObject obj)
        {
            var frameCount = binaryReader.ReadUInt32();
            VMDMorphKeyFrame[] result = new VMDMorphKeyFrame[frameCount];

            // 一度バッファに貯めてソートする
            
            for (int i = 0; i < frameCount; i++)
            {
                result[i] = new VMDMorphKeyFrame();
                ReadSkinData(result[i]);
            }
            Array.Sort(result, (x, y) => ((int)x.flame_no - (int)y.flame_no));
            obj.morphKeyFrameList = result;
        }
        private static void ReadSkinData(VMDMorphKeyFrame frame)
        {
            frame.skin_name = ConvertByteToString(binaryReader.ReadBytes(15), "");
            frame.flame_no = (int) binaryReader.ReadUInt32();
            frame.weight = binaryReader.ReadSingle();
        }
        private static void ParseCameraKeyFrame(VMDObject obj)
        {
            var frameCount = binaryReader.ReadUInt32();
            VMDCameraKeyFrame[] result = new VMDCameraKeyFrame[frameCount];
            for (int i = 0; i < frameCount; i++)
            {
                result[i] = new VMDCameraKeyFrame();
                ReadCameraData(result[i]);
            }
            Array.Sort(result, (x, y) => (x.flame_no - y.flame_no));
            obj.cameraKeyFrameList = result;
        }
        private static void ReadCameraData(VMDCameraKeyFrame frame)
        {
            frame.flame_no = (int) binaryReader.ReadUInt32();
            frame.length = binaryReader.ReadSingle();
            frame.location = ReadSinglesToVector3(binaryReader);
            frame.rotation = ReadSinglesToVector3(binaryReader);
            frame.interpolation = binaryReader.ReadBytes(24);
            frame.viewing_angle = (int) binaryReader.ReadUInt32();
            frame.perspective = binaryReader.ReadByte();
        }
        private static void ParseLightKeyFrame(VMDObject obj)
        {
            var frameCount = binaryReader.ReadUInt32();
            VMDLightKeyFrame[] result = new VMDLightKeyFrame[frameCount];
            for (int i = 0; i < frameCount; i++)
            {
                result[i] = new VMDLightKeyFrame();
                ReadLightData(result[i]);
            }

            Array.Sort(result, (x, y) => (x.flame_no - y.flame_no));
            obj.lightKeyFrameList = result;
        }
        private static void ReadLightData(VMDLightKeyFrame frame)
        {
            frame.flame_no = (int) binaryReader.ReadUInt32();
            frame.rgb = ReadSinglesToColor(1);
            frame.location = ReadSinglesToVector3(binaryReader);
        }
        private static void ParseShadowKeyFrame(VMDObject obj)
        {
            var frameCount = binaryReader.ReadUInt32();
            VMDShadowKeyFrame[] result = new VMDShadowKeyFrame[frameCount];
            for (int i = 0; i < frameCount; i++)
            {
                result[i] = new VMDShadowKeyFrame();
                ReadSelfShadowData(result[i]);
            }

            Array.Sort(result, (x, y) => (x.flame_no - y.flame_no));
            obj.shadowKeyFrameList = result;
        }
        private static void ReadSelfShadowData(VMDShadowKeyFrame frame)
        {
            frame.flame_no = (int) binaryReader.ReadUInt32();
            frame.mode = binaryReader.ReadByte();
            frame.distance = binaryReader.ReadSingle();
        }
        private static void ParseIKKeyFrame(VMDObject obj)
        {
            var frameCount = binaryReader.ReadUInt32();
            VMDIKKeyFrame[] result = new VMDIKKeyFrame[frameCount];
            for (int i = 0; i < frameCount; i++)
            {
                result[i] = new VMDIKKeyFrame();
                ReadIKBoneData(result[i]);
            }

            Array.Sort(result, (x, y) => (x.flame_no - y.flame_no));
            obj.ikKeyFrameList = result;
        }
        private static void ReadIKBoneData(VMDIKKeyFrame frame)
        {
            frame.flame_no = (int)binaryReader.ReadUInt32();
            frame.isShow = binaryReader.ReadBoolean();
            frame.boneCount = (int)binaryReader.ReadUInt32();
            var boneData = new VMDIKKeyFrame.IKbone[frame.boneCount];
            for(int i = 0; i < frame.boneCount; i++)
            {
                boneData[i] = new VMDIKKeyFrame.IKbone();
                boneData[i].boneName = ConvertByteToString(binaryReader.ReadBytes(20), "");
                boneData[i].isIKOn = binaryReader.ReadBoolean();
            }
            frame.ikBones = boneData;
        }

        #endregion
        #region ToolFucntions
        private static string ConvertByteToString(byte[] bytes, string line_feed_code = null)
        {
            // パディングの消去, 文字を詰める
            if (bytes[0] == 0) return "";
            int count;
            for (count = 0; count < bytes.Length; count++) if (bytes[count] == 0) break;
            byte[] buf = new byte[count];       // NULL文字を含めるとうまく行かない
            for (int i = 0; i < count; i++)
            {
                buf[i] = bytes[i];
            }

            buf = Encoding.Convert(Encoding.GetEncoding(932), Encoding.UTF8, buf);
            string result = Encoding.UTF8.GetString(buf);
            if (null != line_feed_code)
            {
                //改行コード統一(もしくは除去)
                result = result.Replace("\r\n", "\n").Replace('\r', '\n').Replace("\n", line_feed_code);
            }
            return result;
        }
        //Read string
        private static string ReadString()
        {
            string result = null;
            int stringLength = binaryReader.ReadInt32();
            byte[] buf = binaryReader.ReadBytes(stringLength);
            if (Encoding.Unicode == encoding)
                result = Encoding.Unicode.GetString(buf);
            if (Encoding.UTF8 == encoding)
                result = Encoding.UTF8.GetString(buf);
            if (result == null)
                throw new System.InvalidOperationException("Invalid string encoding format(未知的字符串解码格式)");
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

        private static int CastIntRead(byte index_size)
        {
            int result = 0;
            switch (index_size)
            {
                case (byte)1:
                    result = binaryReader.ReadByte();
                    if (byte.MaxValue == result)
                    {
                        result = -1;
                    }
                    break;
                case (byte)2:
                    result = binaryReader.ReadUInt16();
                    if (ushort.MaxValue == result)
                    {
                        result = -1;
                    }
                    break;
                case (byte)4:
                    result = (int)binaryReader.ReadUInt32();
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
            ik_Data.iterations = (int)binaryReader.ReadUInt32();
            ik_Data.limit_angle = binaryReader.ReadSingle();
            int ik_link_count = (int)binaryReader.ReadUInt32();
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
            morphOffset.offset_method = (PMXMorph.MaterialMorphOffset.OffsetMethod)binaryReader.ReadByte();
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
