using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace PH_DynaUncensor
{
    internal class DynaMale : MonoBehaviour
    {
        internal static HashSet<DynamicBoneCollider> DynaCollMale = new HashSet<DynamicBoneCollider>();

        private GameObject AnimBoneRoot;

        private DynamicBoneCollider DynaCollTin;
        private HashSet<DynamicBoneCollider> DynaCollTang = new HashSet<DynamicBoneCollider>();
        private HashSet<DynamicBoneCollider> DynaCollHand = new HashSet<DynamicBoneCollider>();


        private void Awake()
        {
            AnimBoneRoot = this.transform.Find("p_cm_anim/cm_J_Root").gameObject;

            foreach (DynamicBoneCollider collider in AnimBoneRoot.GetComponentsInChildren<DynamicBoneCollider>())
            {
                DynaCollMale.Add(collider);
            }

            AddDynaCollTin();
            AddDynaCollTang();
            AddDynaCollHand();


            DynaFemale[] females = FindObjectsOfType<DynaFemale>();
            foreach(DynaFemale female in females)
            {
                female.SetupBodyDynamicBones();
            }


            Transform[] transforms = this.GetComponentsInChildren<Transform>(true);
            foreach (Transform child in transforms)
            {
                if (child.name == "cm_J_Root" && child.parent.name != "p_cm_anim") Destroy(child.gameObject);
            }
        }

        private void AddDynaCollTin()
        {
            Transform TinTip = Transform_Utility.FindTransform(AnimBoneRoot.transform, "cm_J_dan109_00");
            if (TinTip != null) DynaCollTin = TinTip.gameObject.AddComponent<DynamicBoneCollider>();
            DynaCollTin.m_Bound = DynamicBoneCollider.Bound.Outside;
            DynaCollTin.m_Direction = DynamicBoneCollider.Direction.Z;
            DynaCollTin.m_Center = new Vector3(0f,0f,0f);
            DynaCollTin.m_Radius = 0.0321f;
            DynaCollTin.m_Height = 0.07f;

            DynaCollMale.Add(DynaCollTin);
        }

        private void AddDynaCollTang()
        {
            Transform Tang = Transform_Utility.FindTransform(AnimBoneRoot.transform, "cm_J_Tang_S_04");
            if (Tang != null)
            {
                DynamicBoneCollider DynaCollTang04 = Tang.gameObject.AddComponent<DynamicBoneCollider>();
                DynaCollTang04.m_Bound = DynamicBoneCollider.Bound.Outside;
                DynaCollTang04.m_Direction = DynamicBoneCollider.Direction.Z;
                DynaCollTang04.m_Center = new Vector3(0f, 0f, 0f);
                DynaCollTang04.m_Radius = 0.016f;
                DynaCollTang04.m_Height = 0f;
                DynaCollTang.Add(DynaCollTang04);
                DynaCollMale.Add(DynaCollTang04);

                Transform TangMid = Transform_Utility.FindTransform(Tang.transform, "cm_J_Tang_S_06");
                if (TangMid != null)
                {
                    DynamicBoneCollider DynaCollTang06 = TangMid.gameObject.AddComponent<DynamicBoneCollider>();
                    DynaCollTang06.m_Bound = DynamicBoneCollider.Bound.Outside;
                    DynaCollTang06.m_Direction = DynamicBoneCollider.Direction.Z;
                    DynaCollTang06.m_Center = new Vector3(0f, 0f, 0f);
                    DynaCollTang06.m_Radius = 0.014f;
                    DynaCollTang06.m_Height = 0f;
                    DynaCollTang.Add(DynaCollTang06);
                    DynaCollMale.Add(DynaCollTang06);

                    Transform TangTip = Transform_Utility.FindTransform(TangMid.transform, "cm_J_Tang_S_08");
                    if (TangTip != null)
                    {
                        DynamicBoneCollider DynaCollTang08 = TangTip.gameObject.AddComponent<DynamicBoneCollider>();
                        DynaCollTang08.m_Bound = DynamicBoneCollider.Bound.Outside;
                        DynaCollTang08.m_Direction = DynamicBoneCollider.Direction.Z;
                        DynaCollTang08.m_Center = new Vector3(0f, 0f, 0f);
                        DynaCollTang08.m_Radius = 0.012f;
                        DynaCollTang08.m_Height = 0f;
                        DynaCollTang.Add(DynaCollTang08);
                        DynaCollMale.Add(DynaCollTang08);
                    }
                }
            }

        }

        private void AddDynaCollHand()
        {
            Transform HandL = Transform_Utility.FindTransform(AnimBoneRoot.transform, "cm_J_Hand_L");
            if (HandL != null)
            {
                SetDynaCollHand(Transform_Utility.FindTransform(HandL, "cm_J_Hand_Index02_L").gameObject.AddComponent<DynamicBoneCollider>());
                SetDynaCollHand(Transform_Utility.FindTransform(HandL, "cm_J_Hand_Index03_L").gameObject.AddComponent<DynamicBoneCollider>());
                SetDynaCollHand(Transform_Utility.FindTransform(HandL, "cm_J_Hand_Middle02_L").gameObject.AddComponent<DynamicBoneCollider>());
                SetDynaCollHand(Transform_Utility.FindTransform(HandL, "cm_J_Hand_Middle03_L").gameObject.AddComponent<DynamicBoneCollider>());
                SetDynaCollHand(Transform_Utility.FindTransform(HandL, "cm_J_Hand_Little02_L").gameObject.AddComponent<DynamicBoneCollider>());
                SetDynaCollHand(Transform_Utility.FindTransform(HandL, "cm_J_Hand_Little03_L").gameObject.AddComponent<DynamicBoneCollider>());
            }

            Transform HandR = Transform_Utility.FindTransform(AnimBoneRoot.transform, "cm_J_Hand_R");
            if (HandR != null)
            {
                SetDynaCollHand(Transform_Utility.FindTransform(HandR, "cm_J_Hand_Index02_R").gameObject.AddComponent<DynamicBoneCollider>());
                SetDynaCollHand(Transform_Utility.FindTransform(HandR, "cm_J_Hand_Index03_R").gameObject.AddComponent<DynamicBoneCollider>());
                SetDynaCollHand(Transform_Utility.FindTransform(HandR, "cm_J_Hand_Middle02_R").gameObject.AddComponent<DynamicBoneCollider>());
                SetDynaCollHand(Transform_Utility.FindTransform(HandR, "cm_J_Hand_Middle03_R").gameObject.AddComponent<DynamicBoneCollider>());
                SetDynaCollHand(Transform_Utility.FindTransform(HandR, "cm_J_Hand_Little02_R").gameObject.AddComponent<DynamicBoneCollider>());
                SetDynaCollHand(Transform_Utility.FindTransform(HandR, "cm_J_Hand_Little03_R").gameObject.AddComponent<DynamicBoneCollider>());
            }
        }

        private void SetDynaCollHand(DynamicBoneCollider collider)
        {
            collider.m_Bound = DynamicBoneCollider.Bound.Outside;
            collider.m_Direction = DynamicBoneCollider.Direction.X;
            collider.m_Center = new Vector3(0.02f, 0f, 0f);
            collider.m_Radius = 0.02f;
            collider.m_Height = 0f;

            DynaCollHand.Add(collider);
            DynaCollMale.Add(collider);
        }
    }
}
