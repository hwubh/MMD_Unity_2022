using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Networking;
using UnityEngine.Playables;
using UnityEngine.Rendering;
using static MMD_URP.PMXMaterial;
using static MMD_URP.PMXRigidbody;

namespace MMD_URP
{
    public class Convertor:MonoBehaviour
    {
        #region Properties
        internal static Texture2D tempTexture = null;
        internal static ImportData.ModelData tempModel;
        #endregion
        public void Import(string ModelPath, string MotionPath, string TexturesPath = null)
        {
            #region PMX
            //Parse *.pmx file
            PMXObject obj = PMXParser.Parse(ModelPath);
            //Create Model Object
            GameObject model;
            tempModel = new ImportData.ModelData();
            tempModel.characterRoot = model = new GameObject(obj.header.Name);
            //Read Comment
            //TextAsset ReadMe = new TextAsset(obj.Comment.ToString());

            //Mesh triangles
            int[] triangles = obj.SurfaceList;

            //Create Meshes
            var materialList = obj.MaterialList;
            var materialCount = materialList.Length;
            var vertexList = obj.VertexList;
            var vertexCount = vertexList.Length;
            Mesh mesh = null;
            MeshCreationInfo result = new MeshCreationInfo();
            //Only create one mesh renderer
            //Assign vertices to each material(submesh)
            result.value = meshCreationInfoPacks(materialCount, materialList, triangles);
            var vertices = new int[vertexCount];
            //result[0].all_vertices = Enumerable.Range(0, vertexCount).Select(x => x).ToArray();
            var reassign_dictionary = new Dictionary<int, int>(vertexCount);
            for (int i = 0; i < vertexCount; ++i)
            {
                vertices[i] = i;
                reassign_dictionary[i] = i;
            }
            result.all_vertices = vertices;
            result.reassign_dictionary = reassign_dictionary;

            mesh = CreateMesh(result, vertexList);

            //Create Materials
            var texturePathList = obj.TextureList;
            var texturePathListLength = texturePathList.Length;
            var materials = obj.MaterialList;

            //Assign textures to materials
            string diffuseKeyword = null;
            if (GraphicsSettings.defaultRenderPipeline == null)
                diffuseKeyword = "_MainTex";
            else
                diffuseKeyword = "_BaseMap";
            for (int i = 0; i < materialCount; i++)
            {
                var material = materials[i];
                if (material.diffuseTextureIndex < texturePathListLength && material.diffuseTextureIndex != -1)
                {
                    StartCoroutine(MaterialUtils.GetTextureHTTP(material.material, Path.Combine("file://", Path.GetDirectoryName(ModelPath), texturePathList[material.diffuseTextureIndex]), diffuseKeyword));
                }
                if (material.blendMode != EnvironmentBlendMode.Null)
                {
                    if (material.environmentTextureIndex < texturePathListLength && material.environmentTextureIndex != -1)
                        StartCoroutine(MaterialUtils.GetTextureHTTP(material.material, Path.Combine("file://", Path.GetDirectoryName(ModelPath), texturePathList[material.environmentTextureIndex]), "_GlossyTex"));
                }
                if (material.toonTexture == 0)
                {
                    if (material.toonTextureIndex < texturePathListLength && material.toonTextureIndex != -1)
                        StartCoroutine(MaterialUtils.GetTextureHTTP(material.material, Path.Combine("file://", Path.GetDirectoryName(ModelPath), texturePathList[material.toonTextureIndex]), "_ToonTex"));
                }
            }

            //Create Bones
            var boneList = obj.BoneList;
            int boneSize = boneList.Length;

            Transform model_root_transform = (new GameObject("BoneRoot")).transform;
            model_root_transform.parent = model.transform;

            GameObject[] bones = new GameObject[boneSize];
            Matrix4x4[] bindposes = new Matrix4x4[boneSize];
            Transform[] bones_transform = new Transform[boneSize];
            var boneManager = new BoneManager();
            for (int i = 0; i < boneSize; i++)
            {
                GameObject game_object = new GameObject(boneList[i].boneName);
                game_object.transform.position = boneList[i].bone_position * 1.0f;
                bones[i] = game_object;

                int parent_bone_index = boneList[i].parent_bone_index;
                if (parent_bone_index < bones.Length && parent_bone_index != -1)
                    bones[i].transform.parent = bones[parent_bone_index].transform;
                else
                    bones[i].transform.parent = model_root_transform;

                bindposes[i] = bones[i].transform.worldToLocalMatrix;
                bones_transform[i] = bones[i].transform;
                try { boneManager.boneTransforms.Add(bones[i].name, bones_transform[i]); }
                catch { Debug.LogError("Duplicate Name of bones!(重复的骨骼名)"); }
            }
            tempModel.boneManager = boneManager;

            Transform mesh_root_transform = (new GameObject("Mesh")).transform;
            mesh_root_transform.parent = model.transform;
            SkinnedMeshRenderer meshRenderers = new SkinnedMeshRenderer();
            Transform mesh_transform = (new GameObject("Mesh Renderer")).transform;
            mesh_transform.parent = mesh_root_transform.transform;
            SkinnedMeshRenderer smr = mesh_transform.gameObject.AddComponent<SkinnedMeshRenderer>();
            mesh.bindposes = bindposes;
            smr.sharedMesh = mesh;
            smr.sharedMesh.name = mesh_root_transform.parent.name;
            smr.bones = bones_transform;
            smr.materials = result.value.Select(x => materials[x.material_index].material).ToArray();
            smr.rootBone = model_root_transform;
            smr.receiveShadows = false;

            meshRenderers = smr;

            //Create Morphes(BlendShapes)
            var morphList = obj.MorphList;
            foreach (var morph in morphList)
            {
                switch (morph.morph_type)
                {
                    case PMXMorph.MorphType.Vertex:
                        {
                            var shapeName = morph.morphName;
                            var frameWeight = 1f;
                            var deltaVertices = new Vector3[vertexCount];
                            var verticesChanged = new Dictionary<int, Vector3>();
                            foreach (PMXMorph.VertexMorphOffset offset in morph.morph_offset)
                                verticesChanged.Add(offset.vertex_index, offset.position_offset);
                            for (int i = 0; i < vertexCount; i++)
                            {
                                if (verticesChanged.ContainsKey(i))
                                    deltaVertices[i] = new Vector3(verticesChanged[i].x, verticesChanged[i].y, verticesChanged[i].z) * 1.0f;
                                else
                                    deltaVertices[i] = new Vector3(0f, 0f, 0f);
                            }
                            mesh.AddBlendShapeFrame(shapeName, frameWeight, deltaVertices, null, null);
                            break;
                        }
                    case PMXMorph.MorphType.Uv:
                        {
                            var shapeName = morph.morphName;
                            var frameWeight = 1f;
                            var uvsChanged = new Dictionary<int, Vector2>();
                            foreach (PMXMorph.UVMorphOffset offset in morph.morph_offset)
                                uvsChanged.Add(offset.vertex_index, new Vector2(offset.uv_offset.x, 1 - offset.uv_offset.y));
                            for (int i = 0; i < vertexCount; i++)
                            {
                                if (uvsChanged.ContainsKey(i))
                                    mesh.uv[i] += uvsChanged[i];
                            }
                            break;
                        }
                    case PMXMorph.MorphType.Bone:
                        {
                            var shapeName = morph.morphName;
                            var frameWeight = 1f;
                            var bonesChanged = new Dictionary<int, Tuple<Vector3, Quaternion>>();
                            foreach (PMXMorph.BoneMorphOffset offset in morph.morph_offset)
                                bonesChanged.Add(offset.bone_index, new Tuple<Vector3, Quaternion>(offset.move_value, offset.rotate_value));
                            for (int i = 0; i < boneSize; i++)
                            {
                                if (bonesChanged.ContainsKey(i))
                                {
                                    bones[i].transform.position += bonesChanged[i].Item1 * 1.0f;
                                    bones[i].transform.rotation *= bonesChanged[i].Item2;
                                }
                            }
                            break;
                        }
                    case PMXMorph.MorphType.Material:
                        {
                            //var shapeName = morph.morphName;
                            //var frameWeight = 1f;
                            //var materialsChanged = new Dictionary<int, PMXMorph.MaterialMorphOffset>();
                            //foreach (PMXMorph.MaterialMorphOffset offset in morph.morph_offset)
                            //    materialsChanged.Add(offset.material_index, offset);
                            //for (int i = 0; i < materialCount; i++)
                            //{
                            //    if (materialsChanged.ContainsKey(i))
                            //    {
                            //        if (materialsChanged[i].offset_method == MMD_URP.PMXMorph.MaterialMorphOffset.OffsetMethod.Mul)
                            //        {

                            //            meshRenderers.materials[i].SetColor("_Color", materialList[i].)
                            //        }
                            //        else if(materialsChanged[i].offset_method == MMD_URP.PMXMorph.MaterialMorphOffset.OffsetMethod.Add)
                            //        {

                            //        }
                            //    }
                            //}
                            break;
                        }
                    case PMXMorph.MorphType.Group:
                        {
                            //var shapeName = morph.morphName;
                            //var frameWeight = 1f;
                            //var bonesChanged = new Dictionary<int, Tuple<Vector3, Quaternion>>();
                            //foreach (PMXMorph.BoneMorphOffset offset in morph.morph_offset)
                            //    bonesChanged.Add(offset.bone_index, new Tuple<Vector3, Quaternion>(offset.move_value, offset.rotate_value));
                            //for (int i = 0; i < boneSize; i++)
                            //{
                            //    if (bonesChanged.ContainsKey(i))
                            //    {
                            //        bones[i].transform.position += bonesChanged[i].Item1;
                            //        bones[i].transform.rotation *= bonesChanged[i].Item2;
                            //    }
                            //}
                            break;
                        }
                    case PMXMorph.MorphType.Impulse:
                        {
                            break;
                        }
                    default:
                        break;
                }
            }

            //use ik solver
            MMDEngine engine = model.AddComponent<MMDEngine>(); //MMDEngine追加
            //スケール・エッジ幅
            engine.scale = 1.0f;
            engine.outline_width = 1.0f;
            engine.material_outline_widths = obj.MaterialList.Select(x => 0.1f).ToArray();
            engine.enable_render_queue = false; //初期値無効
            engine.render_queue_value = 3000;
            {
                engine.bone_controllers = EntryBoneController(bones, obj);
                engine.ik_list = engine.bone_controllers.Where(x => null != x.ik_solver)
                                                        .Select(x => x.ik_solver)
                                                        .ToArray();
            }

            //Create rigidbody
            var rigidbodyList = obj.RigidbodyList;
            var jointList = obj.JointList;
            //Create gameobjects with colliders
            GameObject[] rigids = rigidbodyList.Select(x =>
            {
                GameObject rigidBody = new GameObject(x.rigidbodyName);

                //cost cg here
                rigidBody.transform.position = x.collider_position * 1.0f;
                rigidBody.transform.rotation = Quaternion.Euler(x.collider_rotation * Mathf.Rad2Deg);

                // Create collider
                switch (x.shape_type)
                {
                    case PMXRigidbody.ShapeType.Sphere:
                        SphereCollider colliderS = rigidBody.AddComponent<SphereCollider>();
                        colliderS.radius = x.shape_size.x * 1.0f;
                        break;
                    case PMXRigidbody.ShapeType.Box:
                        BoxCollider colliderB = rigidBody.AddComponent<BoxCollider>();
                        colliderB.size = new Vector3(x.shape_size.x, x.shape_size.y, x.shape_size.z) * 2.0f * 1.0f;
                        break;
                    case PMXRigidbody.ShapeType.Capsule:
                        CapsuleCollider colliderC = rigidBody.AddComponent<CapsuleCollider>();
                        colliderC.radius = x.shape_size.x * 1.0f;
                        colliderC.height = (x.shape_size.y + x.shape_size.x * 2.0f) * 1.0f;
                        break;
                    default:
                        throw new System.ArgumentException();
                }
                return rigidBody;
            }).ToArray();

            var rigidCount = rigids.Length;
            // create PhysicMaterial
            for (int i = 0; i < rigidCount; ++i)
            {
                PhysicMaterial material = new PhysicMaterial(string.Format("{0}_r ({1})", model.name, rigidbodyList[i].rigidbodyName));
                material.bounciness = rigidbodyList[i].recoil;
                material.staticFriction = rigidbodyList[i].friction;
                material.dynamicFriction = rigidbodyList[i].friction;
                rigids[i].GetComponent<Collider>().material = material;
            }
            // assign PhysicMaterial to objects
            Transform physics_root_transform = (new GameObject("Physics", typeof(PhysicsManager))).transform;
            physics_root_transform.parent = model.transform;

            // connect rigidbodies
            for (int i = 0; i < rigidCount; ++i)
            {
                int rel_bone_index = PhysicsUtils.GetRelBoneIndexFromNearbyRigidbody(i, rigidbodyList, boneList, jointList);
                if (rel_bone_index < bones.Length && rel_bone_index != -1)
                    rigids[i].transform.parent = bones[rel_bone_index].transform;
                else
                    rigids[i].transform.parent = physics_root_transform;
            }

            int bone_count = boneList.Length;
            for (int i = 0; i < rigidCount; ++i)
            {
                GameObject target;
                if (rigidbodyList[i].rel_bone_index != -1)
                    target = bones[rigidbodyList[i].rel_bone_index];
                else
                    target = rigids[i];
                //assign rigidbody properties to unity rigidbody
                PhysicsUtils.UnityRigidbodySetting(rigidbodyList[i], target);
            }

            List<GameObject> result_list = new List<GameObject>();
            foreach (var joint in jointList)
            {
                //相互接続する剛体の取得
                Transform transform_a = rigids[joint.rigidbody_a].transform;
                Rigidbody rigidbody_a = transform_a.GetComponent<Rigidbody>();
                if (null == rigidbody_a)
                {
                    rigidbody_a = transform_a.parent.GetComponent<Rigidbody>();
                }
                Transform transform_b = rigids[joint.rigidbody_b].transform;
                Rigidbody rigidbody_b = transform_b.GetComponent<Rigidbody>();
                if (null == rigidbody_b)
                {
                    rigidbody_b = transform_b.parent.GetComponent<Rigidbody>();
                }
                if (rigidbody_a != rigidbody_b)
                {
                    //接続する剛体が同じ剛体を指さないなら
                    //(本来ならPMDの設定が間違っていない限り同一を指す事は無い)
                    //ジョイント設定
                    ConfigurableJoint config_joint = rigidbody_b.gameObject.AddComponent<ConfigurableJoint>();
                    config_joint.connectedBody = rigidbody_a;
                    PhysicsUtils.SetMotionAngularLock(joint, config_joint);
                    PhysicsUtils.SetDrive(joint, config_joint);

                    result_list.Add(config_joint.gameObject);
                }
            }
            GameObject[] joints = result_list.ToArray();

            PhysicsManager physics_manager = physics_root_transform.gameObject.GetComponent<PhysicsManager>();

            if ((null != joints) && (0 < joints.Length))
            {
                // PhysicsManagerに移動前の状態を覚えさせる(幾つか重複しているので重複は削除)
                physics_manager.connect_bone_list = joints.Select(x => x.gameObject)
                                                            .Distinct()
                                                            .Select(x => new PhysicsManager.ConnectBone(x, x.transform.parent.gameObject))
                                                            .ToArray();

                //isKinematicで無くConfigurableJointを持つ場合はグローバル座標化
                //foreach (ConfigurableJoint joint in joints.Where(x => !x.GetComponent<Rigidbody>().isKinematic)
                //                                            .Select(x => x.GetComponent<ConfigurableJoint>()))
                //{
                //    joint.transform.parent = physics_root_transform;
                //}
            }

            const int MaxGroup = 16;    // グループの最大数
            List<int>[] ignoreGroups = new List<int>[MaxGroup];
            for (int i = 0, i_max = MaxGroup; i < i_max; ++i)
            {
                ignoreGroups[i] = new List<int>();
            }

            // それぞれの剛体が所属している非衝突グループを追加していく
            for (int i = 0, i_max = rigids.Length; i < i_max; ++i)
            {
                int ignoreIndex = rigidbodyList[i].group_index;
                if (ignoreIndex < MaxGroup)
                    ignoreGroups[ignoreIndex].Add(i);
            }

            for (int i = 0; i < rigids.Length; i++)
            {
                for (int shift = 0; shift < 16; shift++)
                {
                    // フラグチェック
                    if ((rigidbodyList[i].ignore_collision_group & (1 << shift)) == 0)
                    {
                        for (int j = 0; j < ignoreGroups[shift].Count; j++)
                        {
                            int ignoreIndex = ignoreGroups[shift][j];
                            if (i == ignoreIndex) continue;
                            Physics.IgnoreCollision(rigids[i].GetComponent<Collider>(), rigids[ignoreIndex].GetComponent<Collider>(), true);
                        }
                    }
                }
            }
            #endregion
            #region VMD
            Animation animation = model.AddComponent<Animation>();
            //Animator animator = model.AddComponent<Animator>();
            VMDObject vmdObj = VMDParser.Parse(MotionPath);
            CustomAnimator animator = model.AddComponent<CustomAnimator>();
            CustomAnimationClip clip = new CustomAnimationClip();
            clip.name = string.Format(model.name, "_", vmdObj.name);

            Dictionary<string, string> bone_path = new Dictionary<string, string>();
            Dictionary<string, GameObject> gameobj = new Dictionary<string, GameObject>();
            CommonUtils.SaveTreeInDictionary(gameobj, model);
            FullSearchBonePath(model.transform, bone_path);
            //clip.frameRate = 24;
            //if (clip.legacy)
            //{
            //    clip.wrapMode = WrapMode.Once;
            //}
            FullEntryBoneAnimation(vmdObj, clip, bone_path, gameobj, 1);
            //#if UNITY_EDITOR
            //            clip.legacy = false;
            //            UnityEditor.AssetDatabase.CreateAsset(clip, "Assets/TestModel/ganyu/Animation/test.anim");
            //#endif
            //string output = JsonConvert.SerializeObject(clip);
            //print(output);

            //Old animation system
            {
                //PlayableGraph playableGraph;
                //AnimationClipPlayable animationClipPlayable;

                //playableGraph = PlayableGraph.Create("PlayableGraph");
                //var animationOutputPlayable = AnimationPlayableOutput.Create(playableGraph, "AnimationOutput", animator);

                //animationClipPlayable = AnimationClipPlayable.Create(playableGraph, clip); //往graph添加playable
                //animationOutputPlayable.SetSourcePlayable(animationClipPlayable, 0);

                //tempModel.animationClipPlayable = animationClipPlayable;
                //tempModel.graph = playableGraph;
                //GameSystem.importData.models.Add(tempModel);

                //playableGraph.Play();
                //animationClipPlayable.SetTime(0);
                //animationClipPlayable.Pause();
            }

            //Latios aniamtion system
            {            //SingleClipAuthoring animationAuthoring = model.AddComponent(typeof(SingleClipAuthoring)) as SingleClipAuthoring;
                         //animationAuthoring.clip = clip;
            }

            //Custom animtion system
            {
                animator.SetClip(clip);
                tempModel.animator = animator;
                GameSystem.importData.models.Add(tempModel);
            }


            #endregion
        }
        #region ToolFunctions
        MeshCreationInfo.Pack[] meshCreationInfoPacks(int materialCount, PMXMaterial[] materialList, int[] triangles)
        {
            int plane_start = 0;
            var packs = new MeshCreationInfo.Pack[materialCount];
            for(int i = 0; i < materialCount; i++)
            {
                MeshCreationInfo.Pack pack = new MeshCreationInfo.Pack();
                pack.material_index = i;
                int plane_count = materialList[i].surfaceCount;
                pack.plane_indices = (new ArraySegment<int>(triangles, plane_start, plane_count)).ToArray();
                pack.vertices = pack.plane_indices.Distinct().ToArray();
                plane_start += plane_count;
                packs[i] = pack;
            }
            return packs;
        }
        Mesh CreateMesh(MeshCreationInfo result, PMXVertex[] vertexList)
        {
                Mesh mesh = new Mesh();
                mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
                var vertices = result.all_vertices;
                var vertexCount = vertices.Length;
                var tempVertices = new Vector3[vertexCount];
                var tempNormals = new Vector3[vertexCount];
                var tempUVs = new Vector2[vertexCount];
                var tempColors = new Color[vertexCount];
                NativeList<BoneWeight1> bonesWeight = new NativeList<BoneWeight1>(0, AllocatorManager.Temp);
                NativeList<byte> bonesPerVertex = new NativeList<byte>(0, AllocatorManager.Temp);
                Color color = new Color(0f, 0f, 0f, 0f);
                for (int j = 0; j < vertexCount; j++)
                {
                    tempVertices[j] = new Vector3();
                    tempVertices[j] = vertexList[vertices[j]].pos * 1.0f;
                    tempNormals[j] = new Vector3();
                    tempNormals[j] = vertexList[vertices[j]].normal;
                    tempUVs[j] = new Vector2();
                    tempUVs[j] = vertexList[vertices[j]].uv;
                    tempColors[j] = new Color();
                    color.a = vertexList[vertices[j]].edgeScale * 0.25f;
                    tempColors[j] = color;
                    var PMXboneWeight = vertexList[vertices[j]].boneWeight;
                    var boneCount = PMXboneWeight.boneCount;
                    bonesPerVertex.Add(boneCount);
                    for (int a = 0; a < boneCount; a++)
                    {
                        var boneWeights = PMXboneWeight.boneWeights;
                        bonesWeight.Add(boneWeights[a]);
                    }
                };
                mesh.vertices = tempVertices;
                mesh.normals = tempNormals;
                mesh.uv = tempUVs;
                mesh.colors = tempColors;
                mesh.SetBoneWeights(bonesPerVertex, bonesWeight);
                var materialCount = result.value.Length;
                mesh.subMeshCount = materialCount;
                for (int j = 0; j < materialCount; ++j)
                {
                    int[] indices = result.value[j].plane_indices.Select(x => result.reassign_dictionary[x]).ToArray();
                    mesh.SetTriangles(indices, j);
                }
            return mesh;
        }
        //IEnumerator GetText(Material material, string path, string textureName)
        //{
        //    using (UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(path))
        //    {
        //        yield return uwr.SendWebRequest();

        //        if (uwr.result == UnityWebRequest.Result.ConnectionError || uwr.result == UnityWebRequest.Result.ProtocolError)
        //        {
        //            Debug.Log(uwr.error);
        //        }
        //        else
        //        {
        //            // Get downloaded asset bundle
        //            tempTexture = DownloadHandlerTexture.GetContent(uwr);
        //            material.SetTexture(textureName, tempTexture);
        //            material.mainTexture = tempTexture;
        //        }
        //    }
        //}
        //int GetRelBoneIndexFromNearbyRigidbody(int rigidbody_index, PMXRigidbody[] rigidBodies, PMXBone[] bones, PMXJoint[] joints)
        //{
        //    int bone_count = bones.Length;
        //    //関連ボーンを探す
        //    int result = rigidBodies[rigidbody_index].rel_bone_index;
        //    if (result < bone_count)
        //    {
        //        //関連ボーンが有れば
        //        return result;
        //    }
        //    //関連ボーンが無ければ
        //    //ジョイントに接続されている剛体の関連ボーンを探しに行く
        //    //HACK: 深さ優先探索に為っているけれど、関連ボーンとの類似性を考えれば幅優先探索の方が良いと思われる

        //    //ジョイントのAを探しに行く(自身はBに接続されている)
        //    var joint_a_list = joints.Where(x => x.rigidbody_b == rigidbody_index) //自身がBに接続されているジョイントに絞る
        //                                                        .Where(x => x.rigidbody_a < bone_count) //Aが有効な剛体に縛る
        //                                                        .Select(x => x.rigidbody_a); //Aを返す
        //    foreach (var joint_a in joint_a_list)
        //    {
        //        result = GetRelBoneIndexFromNearbyRigidbody(joint_a, rigidBodies, bones, joints);
        //        if (result < bone_count)
        //        {
        //            //関連ボーンが有れば
        //            return result;
        //        }
        //    }
        //    //ジョイントのAに無ければ
        //    //ジョイントのBを探しに行く(自身はAに接続されている)
        //    var joint_b_list = joints.Where(x => x.rigidbody_a == rigidbody_index) //自身がAに接続されているジョイントに絞る
        //                                                        .Where(x => x.rigidbody_b < bone_count) //Bが有効な剛体に縛る
        //                                                        .Select(x => x.rigidbody_b); //Bを返す
        //    foreach (var joint_b in joint_b_list)
        //    {
        //        result = GetRelBoneIndexFromNearbyRigidbody(joint_b, rigidBodies, bones, joints);
        //        if (result < bone_count)
        //        {
        //            //関連ボーンが有れば
        //            return result;
        //        }
        //    }
        //    //それでも無ければ
        //    //諦める
        //    result = -1;
        //    return result;
        //}
        //void UnityRigidbodySetting(PMXRigidbody pmx_rigidbody, GameObject target)
        //{
        //    Rigidbody rigidbody = target.GetComponent<Rigidbody>();
        //    if (null != rigidbody)
        //    {
        //        //既にRigidbodyが付与されているなら
        //        //質量は合算する
        //        rigidbody.mass += pmx_rigidbody.weight;
        //        //減衰値は平均を取る
        //        rigidbody.drag = (rigidbody.drag + pmx_rigidbody.position_dim) * 0.5f;
        //        rigidbody.angularDrag = (rigidbody.angularDrag + pmx_rigidbody.rotation_dim) * 0.5f;
        //    }
        //    else
        //    {
        //        //まだRigidbodyが付与されていないなら
        //        rigidbody = target.AddComponent<Rigidbody>();
        //        rigidbody.isKinematic = pmx_rigidbody.operation_type == OperationType.Static;
        //        rigidbody.mass = Mathf.Max(float.Epsilon, pmx_rigidbody.weight);
        //        rigidbody.drag = pmx_rigidbody.position_dim;
        //        rigidbody.angularDrag = pmx_rigidbody.rotation_dim;
        //    }
        //}
        //void SetMotionAngularLock(PMXJoint joint, ConfigurableJoint conf)
        //{
        //    SoftJointLimit jlim;

        //    // Motionの固定
        //    if (joint.constrain_pos_lower.x == 0.0f && joint.constrain_pos_upper.x == 0.0f)
        //    {
        //        conf.xMotion = ConfigurableJointMotion.Locked;
        //    }
        //    else
        //    {
        //        conf.xMotion = ConfigurableJointMotion.Limited;
        //    }

        //    if (joint.constrain_pos_lower.y == 0.0f && joint.constrain_pos_upper.y == 0.0f)
        //    {
        //        conf.yMotion = ConfigurableJointMotion.Locked;
        //    }
        //    else
        //    {
        //        conf.yMotion = ConfigurableJointMotion.Limited;
        //    }

        //    if (joint.constrain_pos_lower.z == 0.0f && joint.constrain_pos_upper.z == 0.0f)
        //    {
        //        conf.zMotion = ConfigurableJointMotion.Locked;
        //    }
        //    else
        //    {
        //        conf.zMotion = ConfigurableJointMotion.Limited;
        //    }

        //    // 角度の固定
        //    if (joint.constrain_rot_lower.x == 0.0f && joint.constrain_rot_upper.x == 0.0f)
        //    {
        //        conf.angularXMotion = ConfigurableJointMotion.Locked;
        //    }
        //    else
        //    {
        //        conf.angularXMotion = ConfigurableJointMotion.Limited;
        //        float hlim = Mathf.Max(-joint.constrain_rot_lower.x, -joint.constrain_rot_upper.x); //回転方向が逆なので負数
        //        float llim = Mathf.Min(-joint.constrain_rot_lower.x, -joint.constrain_rot_upper.x);
        //        SoftJointLimit jhlim = new SoftJointLimit();
        //        jhlim.limit = Mathf.Clamp(hlim * Mathf.Rad2Deg, -180.0f, 180.0f);
        //        conf.highAngularXLimit = jhlim;

        //        SoftJointLimit jllim = new SoftJointLimit();
        //        jllim.limit = Mathf.Clamp(llim * Mathf.Rad2Deg, -180.0f, 180.0f);
        //        conf.lowAngularXLimit = jllim;
        //    }

        //    if (joint.constrain_rot_lower.y == 0.0f && joint.constrain_rot_upper.y == 0.0f)
        //    {
        //        conf.angularYMotion = ConfigurableJointMotion.Locked;
        //    }
        //    else
        //    {
        //        // 値がマイナスだとエラーが出るので注意
        //        conf.angularYMotion = ConfigurableJointMotion.Limited;
        //        float lim = Mathf.Min(Mathf.Abs(joint.constrain_rot_lower.y), Mathf.Abs(joint.constrain_rot_upper.y));//絶対値の小さい方
        //        jlim = new SoftJointLimit();
        //        jlim.limit = lim * Mathf.Clamp(Mathf.Rad2Deg, 0.0f, 180.0f);
        //        conf.angularYLimit = jlim;
        //    }

        //    if (joint.constrain_rot_lower.z == 0f && joint.constrain_rot_upper.z == 0f)
        //    {
        //        conf.angularZMotion = ConfigurableJointMotion.Locked;
        //    }
        //    else
        //    {
        //        conf.angularZMotion = ConfigurableJointMotion.Limited;
        //        float lim = Mathf.Min(Mathf.Abs(-joint.constrain_rot_lower.z), Mathf.Abs(-joint.constrain_rot_upper.z));//絶対値の小さい方//回転方向が逆なので負数
        //        jlim = new SoftJointLimit();
        //        jlim.limit = Mathf.Clamp(lim * Mathf.Rad2Deg, 0.0f, 180.0f);
        //        conf.angularZLimit = jlim;
        //    }
        //}
        //void SetDrive(PMXJoint joint, ConfigurableJoint conf)
        //{
        //    JointDrive drive;

        //    // Position
        //    if (joint.spring_position.x != 0.0f)
        //    {
        //        drive = new JointDrive();
        //        drive.positionSpring = joint.spring_position.x * 1.0f;
        //        conf.xDrive = drive;
        //    }
        //    if (joint.spring_position.y != 0.0f)
        //    {
        //        drive = new JointDrive();
        //        drive.positionSpring = joint.spring_position.y * 1.0f;
        //        conf.yDrive = drive;
        //    }
        //    if (joint.spring_position.z != 0.0f)
        //    {
        //        drive = new JointDrive();
        //        drive.positionSpring = joint.spring_position.z * 1.0f;
        //        conf.zDrive = drive;
        //    }

        //    // Angular
        //    if (joint.spring_rotation.x != 0.0f)
        //    {
        //        drive = new JointDrive();
        //        drive.mode = JointDriveMode.PositionAndVelocity;
        //        drive.positionSpring = joint.spring_rotation.x;
        //        conf.angularXDrive = drive;
        //    }
        //    if (joint.spring_rotation.y != 0.0f || joint.spring_rotation.z != 0.0f)
        //    {
        //        drive = new JointDrive();
        //        drive.mode = JointDriveMode.PositionAndVelocity;
        //        drive.positionSpring = (joint.spring_rotation.y + joint.spring_rotation.z) * 0.5f;
        //        conf.angularYZDrive = drive;
        //    }
        //}
        class MeshCreationInfo
        {
            public class Pack
            {
                public int material_index; //マテリアル
                public int[] plane_indices;    //面
                public int[] vertices;     //頂点
            }
            public Pack[] value;
            public int[] all_vertices;         //総頂点
            public Dictionary<int, int> reassign_dictionary;  //頂点リアサインインデックス用辞書
        }
        //void GetGameObjects(Dictionary<string, GameObject> obj, GameObject assign_pmd)
        //{
        //    for (int i = 0; i < assign_pmd.transform.childCount; i++)
        //    {
        //        var transf = assign_pmd.transform.GetChild(i);
        //        try
        //        {
        //            obj.Add(transf.name, transf.gameObject);
        //        }
        //        catch (System.ArgumentException arg)
        //        {
        //            Debug.Log(arg.Message);
        //            Debug.Log("An element with the same key already exists in the dictionary. -> " + transf.name);
        //        }

        //        if (transf == null) continue;       // ストッパー
        //        GetGameObjects(obj, transf.gameObject);
        //    }
        //}
        void FullSearchBonePath(Transform transform, Dictionary<string, string> dic)
        {
            int count = transform.childCount;
            for (int i = 0; i < count; i++)
            {
                Transform t = transform.GetChild(i);
                FullSearchBonePath(t, dic);
            }

            // オブジェクト名が足されてしまうので抜く
            string buf = "";
            string[] spl = CommonUtils.GetTransformPath(transform).Split('/');
            for (int i = 1; i < spl.Length - 1; i++)
                buf += spl[i] + "/";
            buf += spl[spl.Length - 1];

            try
            {
                dic.Add(transform.name, buf);
            }
            catch (System.ArgumentException arg)
            {
                Debug.Log(arg.Message);
                Debug.Log("An element with the same key already exists in the dictionary. -> " + transform.name);
            }

            // dicには全てのボーンの名前, ボーンのパス名が入る
        }
        //string GetBonePath(Transform transform)
        //{
        //    string buf;
        //    if (transform.parent == null)
        //        return transform.name;
        //    else
        //        buf = GetBonePath(transform.parent);
        //    return buf + "/" + transform.name;
        //}
        int GetKeyframeCount(List<VMDBoneKeyFrame.Motion> mlist, int type, int interpolationQuality)
        {
            int count = 0;
            for (int i = 0; i < mlist.Count; i++)
            {
                if (i > 0 && !IsLinear(mlist[i].interpolation, type))
                {
                    count += interpolationQuality;//Interpolation Keyframes
                }
                else
                {
                    count += 1;//Keyframe
                }
            }
            return count;
        }
        static bool IsLinear(byte[] interpolation, int type)
        {
            byte ax = interpolation[0 * 8 + type];
            byte ay = interpolation[0 * 8 + 4 + type];
            byte bx = interpolation[1 * 8 + type];
            byte by = interpolation[1 * 8 + 4 + type];
            return (ax == ay) && (bx == by);
        }
        void FullEntryBoneAnimation(VMDObject format, CustomAnimationClip clip, Dictionary<string, string> dic, Dictionary<string, GameObject> obj, int interpolationQuality)
        {
            foreach (KeyValuePair<string, string> p in dic) // keyはtransformの名前, valueはパス
            {
                // 互いに名前の一致する場合にRigidbodyが存在するか調べたい
                GameObject current_obj = null;
                if (obj.ContainsKey(p.Key))
                {
                    current_obj = obj[p.Key];

                    // Rigidbodyがある場合はキーフレの登録を無視する
                    var rigid = current_obj.GetComponent<Rigidbody>();
                    //if (rigid != null || rigid.isKinematic)
                    //{
                    //    continue;
                    //}
                }

                // キーフレの登録
                if (p.Key.Contains("D") || p.Key.Contains("EX"))
                    continue;
                CreateKeysForLocation(format, clip, p.Key, p.Value, interpolationQuality, current_obj);
                CreateKeysForRotation(format, clip, p.Key, p.Value, interpolationQuality);
            }
        }
        void CreateKeysForLocation(VMDObject format, CustomAnimationClip clip, string current_bone, string bone_path, int interpolationQuality, GameObject current_obj = null)
        {
            try
            {
                Vector3 default_position = Vector3.zero;
                if (current_obj != null)
                    default_position = current_obj.transform.localPosition;

                List<VMDBoneKeyFrame.Motion> mlist = format.boneKeyFrameList.motion[current_bone];

                int keyframeCountX = GetKeyframeCount(mlist, 0, interpolationQuality);
                int keyframeCountY = GetKeyframeCount(mlist, 1, interpolationQuality);
                int keyframeCountZ = GetKeyframeCount(mlist, 2, interpolationQuality);

                List<Keyframe> keysX = new List<Keyframe>(keyframeCountX);
                List<Keyframe> keysY = new List<Keyframe>(keyframeCountY);
                List<Keyframe> keysZ = new List<Keyframe>(keyframeCountZ);
                Keyframe lx_prev_key = new Keyframe(-1, 0);
                Keyframe ly_prev_key = new Keyframe(-1, 0);
                Keyframe lz_prev_key = new Keyframe(-1, 0);
                int ix = 0;
                int iy = 0;
                int iz = 0;
                for (int i = 0; i < mlist.Count; i++)
                {
                    const float tick_time = 1.0f / 24.0f;

                    float tick = mlist[i].flame_no * tick_time;

                    Keyframe lx_cur_key = new Keyframe(tick, mlist[i].location.x * 1.0f + default_position.x);
                    Keyframe ly_cur_key = new Keyframe(tick, mlist[i].location.y * 1.0f + default_position.y);
                    Keyframe lz_cur_key = new Keyframe(tick, mlist[i].location.z * 1.0f + default_position.z);

                    AddBezierKeyframes(mlist[i].interpolation, 0, lx_prev_key, lx_cur_key, interpolationQuality, ref keysX, ref ix);
                    AddBezierKeyframes(mlist[i].interpolation, 0, ly_prev_key, ly_cur_key, interpolationQuality, ref keysY, ref iy);
                    AddBezierKeyframes(mlist[i].interpolation, 0, lz_prev_key, lz_cur_key, interpolationQuality, ref keysZ, ref iz);

                    lx_prev_key = lx_cur_key;
                    ly_prev_key = ly_cur_key;
                    lz_prev_key = lz_cur_key;
                }

                for (int i = 0; i < keysX.Count(); i++)
                {
                    //線形補間する
                    var tempKeyframeX = keysX[i];
                    tempKeyframeX.weightedMode = WeightedMode.Both;
                    if (i > 0)
                    {
                        float t = (keysX[i].value - keysX[i - 1].value) / (keysX[i].time - keysX[i - 1].time);
                        var tempKeyframeX2 = keysX[i - 1];
                        tempKeyframeX2.outTangent = t;
                        tempKeyframeX.inTangent = t;
                        tempKeyframeX2.inWeight = 0.5f;
                        tempKeyframeX.outWeight = 0.5f;
                        keysX[i - 1] = tempKeyframeX2;
                    }
                    keysX[i] = tempKeyframeX;
                }
                for (int i = 0; i < keysY.Count(); i++)
                {
                    //線形補間する
                    var tempKeyframeY = keysY[i];
                    tempKeyframeY.weightedMode = WeightedMode.Both;
                    if (i > 0)
                    {
                        float t = (keysY[i].value - keysY[i - 1].value) / (keysY[i].time - keysY[i - 1].time);
                        var tempKeyframeY2 = keysY[i - 1];
                        tempKeyframeY2.outTangent = t;
                        tempKeyframeY.inTangent = t;
                        tempKeyframeY2.inWeight = 0.5f;
                        tempKeyframeY.outWeight = 0.5f;
                        keysY[i - 1] = tempKeyframeY2;
                    }
                    keysY[i] = tempKeyframeY;
                }
                for (int i = 0; i < keysZ.Count(); i++)
                {
                    //線形補間する
                    var tempKeyframeZ = keysZ[i];
                    tempKeyframeZ.weightedMode = WeightedMode.Both;
                    if (i > 0)
                    {
                        float t = (keysZ[i].value - keysZ[i - 1].value) / (keysZ[i].time - keysZ[i - 1].time);
                        var tempKeyframeZ2 = keysZ[i - 1];
                        tempKeyframeZ2.outTangent = t;
                        tempKeyframeZ.inTangent = t;
                        tempKeyframeZ2.inWeight = 0.5f;
                        tempKeyframeZ.outWeight = 0.5f;
                        keysZ[i - 1] = tempKeyframeZ2;
                    }
                    keysZ[i] = tempKeyframeZ;
                }
                AddDummyKeyframe(ref keysX);
                AddDummyKeyframe(ref keysY);
                AddDummyKeyframe(ref keysZ);

                if (mlist.Count != 0)
                {
                    CustomAnimationCurve curve_x = new CustomAnimationCurve(keysX);
                    CustomAnimationCurve curve_y = new CustomAnimationCurve(keysY);
                    CustomAnimationCurve curve_z = new CustomAnimationCurve(keysZ);

                    clip.SetCurve(current_bone, CustomAnimationClip.CurveType.PositionX, curve_x);
                    clip.SetCurve(current_bone, CustomAnimationClip.CurveType.PositionY, curve_y);
                    clip.SetCurve(current_bone, CustomAnimationClip.CurveType.PositionZ, curve_z);

                }


            }
            catch (KeyNotFoundException)
            {
                //Debug.LogError("互換性のないボーンが読み込まれました:" + current_bone);
            }
        }
        void CreateKeysForRotation(VMDObject format, CustomAnimationClip clip, string current_bone, string bone_path, int interpolationQuality)
        {
            try
            {
                List<VMDBoneKeyFrame.Motion> mlist = format.boneKeyFrameList.motion[current_bone];
                int keyframeCount = GetKeyframeCount(mlist, 3, interpolationQuality);

                List<Keyframe> rx_keys = new List<Keyframe>(keyframeCount);
                List<Keyframe> ry_keys = new List<Keyframe>(keyframeCount);
                List<Keyframe> rz_keys = new List<Keyframe>(keyframeCount);
                List<Keyframe> rw_keys = new List<Keyframe>(keyframeCount);
                Keyframe rx_prev_key = new Keyframe(-1, 0);
                Keyframe ry_prev_key = new Keyframe(-1, 0);
                Keyframe rz_prev_key = new Keyframe(-1, 0);
                Keyframe rw_prev_key = new Keyframe(-1, 0);
                int irx = 0;
                int iry = 0;
                int irz = 0;
                int irw = 0;
                for (int i = 0; i < mlist.Count; i++)
                {
                    const float tick_time = 1.0f / 24.0f;
                    float tick = mlist[i].flame_no * tick_time;

                    Quaternion rotation = mlist[i].rotation;
                    Keyframe rx_cur_key = new Keyframe(tick, rotation.x);
                    Keyframe ry_cur_key = new Keyframe(tick, rotation.y);
                    Keyframe rz_cur_key = new Keyframe(tick, rotation.z);
                    Keyframe rw_cur_key = new Keyframe(tick, rotation.w);

                    AddBezierKeyframes(mlist[i].interpolation, 3, rx_prev_key, rx_cur_key, interpolationQuality, ref rx_keys, ref irx);
                    AddBezierKeyframes(mlist[i].interpolation, 3, ry_prev_key, ry_cur_key, interpolationQuality, ref ry_keys, ref iry);
                    AddBezierKeyframes(mlist[i].interpolation, 3, rz_prev_key, rz_cur_key, interpolationQuality, ref rz_keys, ref irz);
                    AddBezierKeyframes(mlist[i].interpolation, 3, rw_prev_key, rw_cur_key, interpolationQuality, ref rw_keys, ref irw);
                    rx_prev_key = rx_cur_key;
                    ry_prev_key = ry_cur_key;
                    rz_prev_key = rz_cur_key;
                    rw_prev_key = rw_cur_key;
                }

                for (int i = 0; i < keyframeCount; i++)
                {
                    //線形補間する
                    var tempKeyframeX = rx_keys[i];
                    var tempKeyframeY = ry_keys[i];
                    var tempKeyframeZ = rz_keys[i];
                    var tempKeyframeW = rw_keys[i];

                    tempKeyframeX.weightedMode = WeightedMode.Both;
                    tempKeyframeY.weightedMode = WeightedMode.Both;
                    tempKeyframeZ.weightedMode = WeightedMode.Both;
                    tempKeyframeW.weightedMode = WeightedMode.Both;
                    if (i > 0)
                    {
                        float tx = GetLinearTangentForRotation(rx_keys[i - 1], rx_keys[i]);
                        float ty = GetLinearTangentForRotation(ry_keys[i - 1], ry_keys[i]);
                        float tz = GetLinearTangentForRotation(rz_keys[i - 1], rz_keys[i]);
                        float tw = GetLinearTangentForRotation(rw_keys[i - 1], rw_keys[i]);

                        var tempKeyframeX2 = rx_keys[i - 1];
                        var tempKeyframeY2 = ry_keys[i - 1];
                        var tempKeyframeZ2 = rz_keys[i - 1];
                        var tempKeyframeW2 = rw_keys[i - 1];

                        tempKeyframeX2.outTangent = tx;
                        tempKeyframeX.inTangent = tx;
                        tempKeyframeX2.inWeight = 0;
                        tempKeyframeX.outWeight = 0f;
                        tempKeyframeY2.outTangent = ty;
                        tempKeyframeY.inTangent = ty;
                        tempKeyframeY2.inWeight = 0;
                        tempKeyframeY.outWeight = 0f;
                        tempKeyframeZ2.outTangent = tz;
                        tempKeyframeZ.inTangent = tz;
                        tempKeyframeZ2.inWeight = 0;
                        tempKeyframeZ.outWeight = 0f;
                        tempKeyframeW2.outTangent = tw;
                        tempKeyframeW.inTangent = tw;
                        tempKeyframeW2.inWeight = 0;
                        tempKeyframeW.outWeight = 0f;

                        rx_keys[i - 1] = tempKeyframeX2;
                        ry_keys[i - 1] = tempKeyframeY2;
                        rz_keys[i - 1] = tempKeyframeZ2;
                        rw_keys[i - 1] = tempKeyframeW2;
                    }
                    rx_keys[i] = tempKeyframeX;
                    ry_keys[i] = tempKeyframeY;
                    rz_keys[i] = tempKeyframeZ;
                    rw_keys[i] = tempKeyframeW;
                }
                AddDummyKeyframe(ref rx_keys);
                AddDummyKeyframe(ref ry_keys);
                AddDummyKeyframe(ref rz_keys);
                AddDummyKeyframe(ref rw_keys);

                CustomAnimationCurve curve_x = new CustomAnimationCurve(rx_keys);
                CustomAnimationCurve curve_y = new CustomAnimationCurve(ry_keys);
                CustomAnimationCurve curve_z = new CustomAnimationCurve(rz_keys);
                CustomAnimationCurve curve_w = new CustomAnimationCurve(rw_keys);

                //for(int i = 0; i < curve_x.length; i++)
                //    curve_x.SmoothTangents(i, 1);
                //for (int i = 0; i < curve_y.length; i++)
                //    curve_y.SmoothTangents(i, 1);
                //for (int i = 0; i < curve_z.length; i++)
                //    curve_z.SmoothTangents(i, 1);
                //for (int i = 0; i < curve_w.length; i++)
                //    curve_w.SmoothTangents(i, 1);

                clip.SetCurve(current_bone, CustomAnimationClip.CurveType.QuaternionX, curve_x);
                clip.SetCurve(current_bone, CustomAnimationClip.CurveType.QuaternionY, curve_y);
                clip.SetCurve(current_bone, CustomAnimationClip.CurveType.QuaternionZ, curve_z);
                clip.SetCurve(current_bone, CustomAnimationClip.CurveType.QuaternionW, curve_w);
                clip.EnsureQuaternionContinuity(current_bone);

                //#if UNITY_EDITOR
                //                Quaternion quaternion = new Quaternion(curve_x.Evaluate(19 * 1.0f / 24.0f), curve_y.Evaluate(19 * 1.0f / 24.0f), curve_z.Evaluate(19 * 1.0f / 24.0f), curve_w.Evaluate(19 * 1.0f / 24.0f));
                //                Debug.Log(current_bone + ": " + mlist[3].rotation.eulerAngles.y + " and q.y is: " + mlist[3].rotation.y);
                //                quaternion = new Quaternion(curve_x.Evaluate(20 * 1.0f / 24.0f), curve_y.Evaluate(20 * 1.0f / 24.0f), curve_z.Evaluate(20 * 1.0f / 24.0f), curve_w.Evaluate(20 * 1.0f / 24.0f));
                //                Debug.Log(current_bone + ": " + quaternion.eulerAngles.y + " and q.y is: " + quaternion.y);
                //                quaternion = new Quaternion(curve_x.Evaluate(21 * 1.0f / 24.0f), curve_y.Evaluate(21 * 1.0f / 24.0f), curve_z.Evaluate(21 * 1.0f / 24.0f), curve_w.Evaluate(21 * 1.0f / 24.0f));
                //                Debug.Log(current_bone + ": " + quaternion.eulerAngles.y + " and q.y is: " + quaternion.y);
                //                quaternion = new Quaternion(curve_x.Evaluate(22 * 1.0f / 24.0f), curve_y.Evaluate(22 * 1.0f / 24.0f), curve_z.Evaluate(22 * 1.0f / 24.0f), curve_w.Evaluate(22 * 1.0f / 24.0f));
                //                Debug.Log(current_bone + ": " + quaternion.eulerAngles.y + " and q.y is: " + quaternion.y);
                //                quaternion = new Quaternion(curve_x.Evaluate(23 * 1.0f / 24.0f), curve_y.Evaluate(23 * 1.0f / 24.0f), curve_z.Evaluate(23 * 1.0f / 24.0f), curve_w.Evaluate(23 * 1.0f / 24.0f));
                //                Debug.Log(current_bone + ": " + quaternion.eulerAngles.y + " and q.y is: " + quaternion.y);
                //                quaternion = new Quaternion(curve_x.Evaluate(24 * 1.0f / 24.0f), curve_y.Evaluate(24 * 1.0f / 24.0f), curve_z.Evaluate(24 * 1.0f / 24.0f), curve_w.Evaluate(24 * 1.0f / 24.0f));
                //                Debug.Log(current_bone + ": " + quaternion.eulerAngles.y + " and q.y is: " + quaternion.y);
                //                quaternion = new Quaternion(curve_x.Evaluate(25 * 1.0f / 24.0f), curve_y.Evaluate(25 * 1.0f / 24.0f), curve_z.Evaluate(25 * 1.0f / 24.0f), curve_w.Evaluate(25 * 1.0f / 24.0f));
                //                Debug.Log(current_bone + ": " + quaternion.eulerAngles.y + " and q.y is: " + quaternion.y);
                //                quaternion = new Quaternion(curve_x.Evaluate(26 * 1.0f / 24.0f), curve_y.Evaluate(26 * 1.0f / 24.0f), curve_z.Evaluate(26 * 1.0f / 24.0f), curve_w.Evaluate(26 * 1.0f / 24.0f));
                //                Debug.Log(current_bone + ": " + mlist[4].rotation.eulerAngles.y + " and q.y is: " + mlist[4].rotation.y);
                //#endif

            }
            catch (KeyNotFoundException)
            {
                //Debug.LogError("互換性のないボーンが読み込まれました:" + bone_path);
            }
        }
        public static void AddBezierKeyframes(byte[] interpolation, int type,
            Keyframe prev_keyframe, Keyframe cur_keyframe, int interpolationQuality,
            ref List<Keyframe> keyframes, ref int index)
        {
            if (prev_keyframe.time < 0 || IsLinear(interpolation, type))
            {
                keyframes.Add(cur_keyframe);
            }
            else
            {
                Vector2 bezierHandleA = GetBezierHandle(interpolation, type, 0);
                Vector2 bezierHandleB = GetBezierHandle(interpolation, type, 1);
                int sampleCount = interpolationQuality;
                for (int j = 0; j < sampleCount; j++)
                {
                    float t = (j + 1) / (float)sampleCount;
                    Vector2 sample = SampleBezier(bezierHandleA, bezierHandleB, t);
                    keyframes.Add(Lerp(prev_keyframe, cur_keyframe, sample));
                }
            }
        }
        static Vector2 GetBezierHandle(byte[] interpolation, int type, int ab)
        {
            // 0=X, 1=Y, 2=Z, 3=R
            // abはa?かb?のどちらを使いたいか
            Vector2 bezierHandle = new Vector2((float)interpolation[ab * 8 + type], (float)interpolation[ab * 8 + 4 + type]);
            return bezierHandle / 127f;
        }
        static Vector2 SampleBezier(Vector2 bezierHandleA, Vector2 bezierHandleB, float t)
        {
            Vector2 p0 = Vector2.zero;
            Vector2 p1 = bezierHandleA;
            Vector2 p2 = bezierHandleB;
            Vector2 p3 = new Vector2(1f, 1f);

            Vector2 q0 = Vector2.Lerp(p0, p1, t);
            Vector2 q1 = Vector2.Lerp(p1, p2, t);
            Vector2 q2 = Vector2.Lerp(p2, p3, t);

            Vector2 r0 = Vector2.Lerp(q0, q1, t);
            Vector2 r1 = Vector2.Lerp(q1, q2, t);

            Vector2 s0 = Vector2.Lerp(r0, r1, t);
            return s0;
        }
        public static Keyframe Lerp(Keyframe from, Keyframe to, Vector2 t)
        {
            return new Keyframe(
                Mathf.Lerp(from.time, to.time, t.x),
                Mathf.Lerp(from.value, to.value, t.y)
            );
        }
        void AddDummyKeyframe(ref List<Keyframe> keyframes)
        {
            if (keyframes[0].time > 0)
            {
                keyframes.Insert(0, new Keyframe(0, keyframes[0].value));

                //var tempKeyframe = new Keyframe(0, keyframes[0].value);
                //List<Keyframe> newKeyframes = new List<Keyframe>(keyframes.Count() + 1);
                //newKeyframes[0] = tempKeyframe;
                //keyframes.CopyTo(keyframes, 0, newKeyframes, 1, keyframes.Count());
                //keyframes = newKeyframes;
            }

            if (keyframes.Count() == 1)
            {
                keyframes.Insert(1, new Keyframe(keyframes[0].time + 0.001f / 24f, keyframes[0].value, 0f, keyframes[0].outTangent));
                var tempKeyframe = keyframes[0];
                tempKeyframe.outTangent = 0f;
                keyframes[0] = tempKeyframe;

                //Keyframe[] newKeyframes = new Keyframe[2];
                //newKeyframes[0] = keyframes[0];
                //newKeyframes[1] = keyframes[0];
                //newKeyframes[1].time += 0.001f / 24f;//1[ms]
                //newKeyframes[0].outTangent = 0f;
                //newKeyframes[1].inTangent = 0f;
                //keyframes = newKeyframes;
            }
        }
        float GetLinearTangentForRotation(Keyframe from_keyframe, Keyframe to_keyframe)
        {
            float tv = to_keyframe.value;
            float fv = from_keyframe.value;
            float delta_value = tv - fv;
            return delta_value / (to_keyframe.time - from_keyframe.time);
            ////180度を越える場合は逆回転
            //if (delta_value < 180f)
            //{
            //    return delta_value / (to_keyframe.time - from_keyframe.time);
            //}
            //else
            //{
            //    return (delta_value - 360f) / (to_keyframe.time - from_keyframe.time);
            //}
        }
        float Mod360(float angle)
        {
            //剰余演算の代わりに加算にする
            return (angle < 0) ? (angle + 360f) : (angle);
        }

        float clamp360(float angle)
        {
            //剰余演算の代わりに加算にする
            if (angle > 180)
                return angle - 360;// (angle > 180) ? (angle - 360) : (angle);
            else
                return angle;
        }

        BoneController[] EntryBoneController(GameObject[] bones, PMXObject obj)
        {
            //BoneControllerが他のBoneControllerを参照するので先に全ボーンに付与
            foreach (var bone in bones)
            {
                bone.AddComponent<BoneController>();
            }
            BoneController[] result = Enumerable.Range(0, obj.BoneList.Length)
                .OrderBy(x => (int)(PMXBone.Flag.PhysicsTransform & obj.BoneList[x].bone_flag)) //物理後変形を後方へ
                .ThenBy(x => obj.BoneList[x].transform_level) //変形階層で安定ソート
                .Select(x => ConvertBoneController(obj.BoneList[x], x, bones, obj)) //ConvertIk()を呼び出す
                .ToArray();
            return result;
        }

        BoneController ConvertBoneController(PMXBone bone, int bone_index, GameObject[] bones, PMXObject obj)
        {
            BoneController result = bones[bone_index].GetComponent<BoneController>();
            if (0.0f != bone.additional_rate)
            {
                //付与親が有るなら
                result.additive_parent = bones[bone.additional_parent_index].GetComponent<BoneController>();
                result.additive_rate = bone.additional_rate;
                result.add_local = (0 != (PMXBone.Flag.AddLocal & bone.bone_flag));
                result.add_move = (0 != (PMXBone.Flag.AddMove & bone.bone_flag));
                result.add_rotate = (0 != (PMXBone.Flag.AddRotation & bone.bone_flag));
            }
            if (true)
            {
                //IKを使用するなら
                if (0 != (PMXBone.Flag.IkFlag & bone.bone_flag))
                {
                    //IKが有るなら
                    result.ik_solver = bones[bone_index].AddComponent<CCDIKSolver>();
                    result.ik_solver.target = bones[bone.ik_data.ik_bone_index].transform;
                    result.ik_solver.controll_weight = bone.ik_data.limit_angle / 4; //HACK: CCDIKSolver側で4倍している様なので此処で逆補正
                    result.ik_solver.iterations = (int)bone.ik_data.iterations;
                    result.ik_solver.chains = bone.ik_data.ik_link.Select(x => x.target_bone_index).Select(x => bones[x].transform).ToArray();
                    //IK制御下のBoneController登録
                    result.ik_solver_targets = Enumerable.Repeat(result.ik_solver.target, 1)
                                                        .Concat(result.ik_solver.chains)
                                                        .Select(x => x.GetComponent<BoneController>())
                                                        .ToArray();

                    //IK制御先のボーンについて、物理演算の挙動を調べる
                    var operation_types = Enumerable.Repeat(bone.ik_data.ik_bone_index, 1) //IK対象先をEnumerable化
                                                    .Concat(bone.ik_data.ik_link.Select(x => x.target_bone_index)) //IK制御下を追加
                                                    .Join(obj.RigidbodyList, x => x, y => y.rel_bone_index, (x, y) => y.operation_type); //剛体リストから関連ボーンにIK対象先・IK制御下と同じボーンを持つ物を列挙し、剛体タイプを返す
                    foreach (var operation_type in operation_types)
                    {
                        if (PMXRigidbody.OperationType.Static != operation_type)
                        {
                            //ボーン追従で無い(≒物理演算)なら
                            //IK制御の無効化
                            result.ik_solver.enabled = false;
                            break;
                        }
                    }
                }
            }
            return result;
        }
        #endregion
    }
}
