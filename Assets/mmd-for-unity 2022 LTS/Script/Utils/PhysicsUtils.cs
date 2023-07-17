using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static MMD_URP.PMXRigidbody;

namespace MMD_URP
{
    public class PhysicsUtils
    {
        public static int GetRelBoneIndexFromNearbyRigidbody(int rigidbody_index, PMXRigidbody[] rigidBodies, PMXBone[] bones, PMXJoint[] joints)
        {
            int bone_count = bones.Length;
            //関連ボーンを探す
            int result = rigidBodies[rigidbody_index].rel_bone_index;
            if (result < bone_count)
            {
                //関連ボーンが有れば
                return result;
            }
            //関連ボーンが無ければ
            //ジョイントに接続されている剛体の関連ボーンを探しに行く
            //HACK: 深さ優先探索に為っているけれど、関連ボーンとの類似性を考えれば幅優先探索の方が良いと思われる

            //ジョイントのAを探しに行く(自身はBに接続されている)
            var joint_a_list = joints.Where(x => x.rigidbody_b == rigidbody_index) //自身がBに接続されているジョイントに絞る
                                                                .Where(x => x.rigidbody_a < bone_count) //Aが有効な剛体に縛る
                                                                .Select(x => x.rigidbody_a); //Aを返す
            foreach (var joint_a in joint_a_list)
            {
                result = GetRelBoneIndexFromNearbyRigidbody(joint_a, rigidBodies, bones, joints);
                if (result < bone_count)
                {
                    //関連ボーンが有れば
                    return result;
                }
            }
            //ジョイントのAに無ければ
            //ジョイントのBを探しに行く(自身はAに接続されている)
            var joint_b_list = joints.Where(x => x.rigidbody_a == rigidbody_index) //自身がAに接続されているジョイントに絞る
                                                                .Where(x => x.rigidbody_b < bone_count) //Bが有効な剛体に縛る
                                                                .Select(x => x.rigidbody_b); //Bを返す
            foreach (var joint_b in joint_b_list)
            {
                result = GetRelBoneIndexFromNearbyRigidbody(joint_b, rigidBodies, bones, joints);
                if (result < bone_count)
                {
                    //関連ボーンが有れば
                    return result;
                }
            }
            //それでも無ければ
            //諦める
            result = -1;
            return result;
        }

        public static void UnityRigidbodySetting(PMXRigidbody pmx_rigidbody, GameObject target)
        {
            Rigidbody rigidbody = target.GetComponent<Rigidbody>();
            if (null != rigidbody)
            {
                //既にRigidbodyが付与されているなら
                //質量は合算する
                rigidbody.mass += pmx_rigidbody.weight;
                //減衰値は平均を取る
                rigidbody.drag = (rigidbody.drag + pmx_rigidbody.position_dim) * 0.5f;
                rigidbody.angularDrag = (rigidbody.angularDrag + pmx_rigidbody.rotation_dim) * 0.5f;
            }
            else
            {
                //まだRigidbodyが付与されていないなら
                rigidbody = target.AddComponent<Rigidbody>();
                rigidbody.isKinematic = pmx_rigidbody.operation_type == OperationType.Static;
                rigidbody.mass = Mathf.Max(float.Epsilon, pmx_rigidbody.weight);
                rigidbody.drag = pmx_rigidbody.position_dim;
                rigidbody.angularDrag = pmx_rigidbody.rotation_dim;
            }
        }

        public static void SetMotionAngularLock(PMXJoint joint, ConfigurableJoint conf)
        {
            SoftJointLimit jlim;

            // Motionの固定
            if (joint.constrain_pos_lower.x == 0.0f && joint.constrain_pos_upper.x == 0.0f)
            {
                conf.xMotion = ConfigurableJointMotion.Locked;
            }
            else
            {
                conf.xMotion = ConfigurableJointMotion.Limited;
            }

            if (joint.constrain_pos_lower.y == 0.0f && joint.constrain_pos_upper.y == 0.0f)
            {
                conf.yMotion = ConfigurableJointMotion.Locked;
            }
            else
            {
                conf.yMotion = ConfigurableJointMotion.Limited;
            }

            if (joint.constrain_pos_lower.z == 0.0f && joint.constrain_pos_upper.z == 0.0f)
            {
                conf.zMotion = ConfigurableJointMotion.Locked;
            }
            else
            {
                conf.zMotion = ConfigurableJointMotion.Limited;
            }

            // 角度の固定
            if (joint.constrain_rot_lower.x == 0.0f && joint.constrain_rot_upper.x == 0.0f)
            {
                conf.angularXMotion = ConfigurableJointMotion.Locked;
            }
            else
            {
                conf.angularXMotion = ConfigurableJointMotion.Limited;
                float hlim = Mathf.Max(-joint.constrain_rot_lower.x, -joint.constrain_rot_upper.x); //回転方向が逆なので負数
                float llim = Mathf.Min(-joint.constrain_rot_lower.x, -joint.constrain_rot_upper.x);
                SoftJointLimit jhlim = new SoftJointLimit();
                jhlim.limit = Mathf.Clamp(hlim * Mathf.Rad2Deg, -180.0f, 180.0f);
                conf.highAngularXLimit = jhlim;

                SoftJointLimit jllim = new SoftJointLimit();
                jllim.limit = Mathf.Clamp(llim * Mathf.Rad2Deg, -180.0f, 180.0f);
                conf.lowAngularXLimit = jllim;
            }

            if (joint.constrain_rot_lower.y == 0.0f && joint.constrain_rot_upper.y == 0.0f)
            {
                conf.angularYMotion = ConfigurableJointMotion.Locked;
            }
            else
            {
                // 値がマイナスだとエラーが出るので注意
                conf.angularYMotion = ConfigurableJointMotion.Limited;
                float lim = Mathf.Min(Mathf.Abs(joint.constrain_rot_lower.y), Mathf.Abs(joint.constrain_rot_upper.y));//絶対値の小さい方
                jlim = new SoftJointLimit();
                jlim.limit = lim * Mathf.Clamp(Mathf.Rad2Deg, 0.0f, 180.0f);
                conf.angularYLimit = jlim;
            }

            if (joint.constrain_rot_lower.z == 0f && joint.constrain_rot_upper.z == 0f)
            {
                conf.angularZMotion = ConfigurableJointMotion.Locked;
            }
            else
            {
                conf.angularZMotion = ConfigurableJointMotion.Limited;
                float lim = Mathf.Min(Mathf.Abs(-joint.constrain_rot_lower.z), Mathf.Abs(-joint.constrain_rot_upper.z));//絶対値の小さい方//回転方向が逆なので負数
                jlim = new SoftJointLimit();
                jlim.limit = Mathf.Clamp(lim * Mathf.Rad2Deg, 0.0f, 180.0f);
                conf.angularZLimit = jlim;
            }
        }

        public static void SetDrive(PMXJoint joint, ConfigurableJoint conf)
        {
            JointDrive drive;

            // Position
            if (joint.spring_position.x != 0.0f)
            {
                drive = new JointDrive();
                drive.positionSpring = joint.spring_position.x * 1.0f;
                conf.xDrive = drive;
            }
            if (joint.spring_position.y != 0.0f)
            {
                drive = new JointDrive();
                drive.positionSpring = joint.spring_position.y * 1.0f;
                conf.yDrive = drive;
            }
            if (joint.spring_position.z != 0.0f)
            {
                drive = new JointDrive();
                drive.positionSpring = joint.spring_position.z * 1.0f;
                conf.zDrive = drive;
            }

            // Angular
            if (joint.spring_rotation.x != 0.0f)
            {
                drive = new JointDrive();
                drive.mode = JointDriveMode.PositionAndVelocity;
                drive.positionSpring = joint.spring_rotation.x;
                conf.angularXDrive = drive;
            }
            if (joint.spring_rotation.y != 0.0f || joint.spring_rotation.z != 0.0f)
            {
                drive = new JointDrive();
                drive.mode = JointDriveMode.PositionAndVelocity;
                drive.positionSpring = (joint.spring_rotation.y + joint.spring_rotation.z) * 0.5f;
                conf.angularYZDrive = drive;
            }
        }
    }
}
