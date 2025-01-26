using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace PH_DynaUncensor
{
    internal static class Tools
    {
        internal static void RecurseTransforms(Transform t, Func<Transform, bool> onBone)
        {
            for (int i = 0; i < t.childCount; ++i)
            {
                Transform child = t.GetChild(i);
                if (onBone(child))
                    --i;
                else
                    RecurseTransforms(child, onBone);
            }
        }

        internal static T[] ReplaceWithNullIfCondition<T>(T[] array, Func<T, bool> condition)
        {
            // 새로운 배열 생성
            T[] newArray = new T[array.Length];

            // 기존 배열을 순회하며 조건을 만족하면 null로 설정
            for (int i = 0; i < array.Length; i++)
            {
                newArray[i] = condition(array[i]) ? default(T) : array[i];
            }

            return newArray;
        }

        internal static void ComputeBoneMatrices(SkinnedMeshRenderer sMesh)
        {
            Transform meshTransform = sMesh.transform;
            Matrix4x4 meshTransformDivisor = meshTransform.localToWorldMatrix.inverse;
            Mesh mesh = sMesh.sharedMesh;
            List<Matrix4x4> m_BindPose = new List<Matrix4x4>();
            for (int i = 0; i < sMesh.bones.Length; i++)
            {
                Transform boneFrame = sMesh.bones[i];
                if (boneFrame != null)
                {
                    Matrix4x4 m = meshTransform.localToWorldMatrix * meshTransformDivisor;
                    m = m.inverse;
                    m_BindPose.Add(Matrix4x4.Transpose(m));
                }
            }
            mesh.bindposes = m_BindPose.ToArray();
        }

    }


}
