using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace MMD_URP
{
    public class PMXHeader
    {
            public PMXVersion Version { get; internal set; }
            public Encoding Encoding { get; internal set; }
            public int AdditionalUVCount { get; internal set; }
            public byte VertexIndexSize { get; internal set; }
            public byte TextureIndexSize { get; internal set; }
            public byte MaterialIndexSize { get; internal set; }
            public byte BoneIndexSize { get; internal set; }
            public byte MorphIndexSize { get; internal set; }
            public byte RigidBodyIndexSize { get; internal set; }
        
        /// <summary>Get name of pmx data</summary>
        public string Name { get; internal set; } = string.Empty;
        public string Comment { get; internal set; } = string.Empty;
        /// <summary>Get English comment of pmx data</summary>
        public string CommentEnglish { get; internal set; } = string.Empty;
        public enum StringCode
        {
            Utf16le,
            Utf8,
        }
    }
}
