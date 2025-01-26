using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Valve.VR.InteractionSystem;

namespace PH_DynaUncensor
{
    internal class DynaToy : MonoBehaviour
    {
        internal static HashSet<DynamicBoneCollider> DynaCollToy = new HashSet<DynamicBoneCollider>();

        private GameObject AnimBoneRoot;

        private HashSet<DynamicBoneCollider> DynaCollVibe = new HashSet<DynamicBoneCollider>();

        private void Start()
        {
            AnimBoneRoot = Transform_Utility.FindTransform(this.transform, "J_vibe_root").gameObject;
            if (AnimBoneRoot == null)
            {
                Destroy(this);
                return;
            }

            AddDynaCollVibe();
            DynaFemale[] females = FindObjectsOfType<DynaFemale>();
            foreach (DynaFemale female in females)
            {
                female.SetupBodyDynamicBones();
            }
        }

        private void AddDynaCollVibe()
        {
            Transform Vibe07 = Transform_Utility.FindTransform(AnimBoneRoot.transform, "J_vibe_07");
            DynamicBoneCollider DynaCollVibe07 = Vibe07.gameObject.AddComponent<DynamicBoneCollider>();
            DynaCollVibe07.m_Bound = DynamicBoneCollider.Bound.Outside;
            DynaCollVibe07.m_Direction = DynamicBoneCollider.Direction.Y;
            DynaCollVibe07.m_Center = new Vector3(0f, -0.01f, 0f);
            DynaCollVibe07.m_Radius = 0.015f;
            DynaCollVibe07.m_Height = 0.05f;
            DynaCollVibe.Add(DynaCollVibe07);
            DynaCollToy.Add(DynaCollVibe07);

            Transform Vibe06 = Transform_Utility.FindTransform(AnimBoneRoot.transform, "J_vibe_06");
            DynamicBoneCollider DynaCollVibe06 = Vibe06.gameObject.AddComponent<DynamicBoneCollider>();
            DynaCollVibe06.m_Bound = DynamicBoneCollider.Bound.Outside;
            DynaCollVibe06.m_Direction = DynamicBoneCollider.Direction.Y;
            DynaCollVibe06.m_Center = new Vector3(0f, 0f, 0f);
            DynaCollVibe06.m_Radius = 0.01f;
            DynaCollVibe06.m_Height = 0f;
            DynaCollVibe.Add(DynaCollVibe06);
            DynaCollToy.Add(DynaCollVibe06);

            SetDynaCollVibeMid(Transform_Utility.FindTransform(AnimBoneRoot.transform, "J_vibe_01").gameObject.AddComponent<DynamicBoneCollider>());
            SetDynaCollVibeMid(Transform_Utility.FindTransform(AnimBoneRoot.transform, "J_vibe_02").gameObject.AddComponent<DynamicBoneCollider>());
            SetDynaCollVibeMid(Transform_Utility.FindTransform(AnimBoneRoot.transform, "J_vibe_03").gameObject.AddComponent<DynamicBoneCollider>());
            SetDynaCollVibeMid(Transform_Utility.FindTransform(AnimBoneRoot.transform, "J_vibe_04").gameObject.AddComponent<DynamicBoneCollider>());
            SetDynaCollVibeMid(Transform_Utility.FindTransform(AnimBoneRoot.transform, "J_vibe_05").gameObject.AddComponent<DynamicBoneCollider>());
        }

        private void SetDynaCollVibeMid(DynamicBoneCollider collider)
        {
            collider.m_Bound = DynamicBoneCollider.Bound.Outside;
            collider.m_Direction = DynamicBoneCollider.Direction.Y;
            collider.m_Center = new Vector3(0f, 0f, 0f);
            collider.m_Radius = 0.03f;
            collider.m_Height = 0f;

            DynaCollVibe.Add(collider);
            DynaCollToy.Add(collider);
        }
    }
}
