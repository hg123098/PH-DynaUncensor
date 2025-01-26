using RootMotion.FinalIK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using UniRx.Triggers;
using Unity.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using static MapData;
using static RootMotion.FinalIK.Grounding;

namespace PH_DynaUncensor
{
    internal class DynaFemale : MonoBehaviour
    {
        internal static string OreoPath = Application.dataPath.Substring(0, Application.dataPath.LastIndexOf('/')) + "/BepInEx/plugins/HG/PH_DynaUncensor/Oreo" + DynaUncensor.OreoVer.ToString();
        internal static string OreoSoftAB = "[oreo]all-in-one_body.unity3d";
        internal static string OreoSoftName = "[oreo]all-in-one_body";
        internal static string OreoHardAB = "cf_base.unity3d";
        internal static string OreoHardName = "p_cf_body_00";
        

        private readonly HashSet<string> AdditionalBones = new HashSet<string>();

        private Female female;
        private GameObject AnimBoneRoot;
        private GameObject WearsRoot;
        private SkinnedMeshRenderer NudeBody;
        private Transform Pelvis;
        private Transform Kokan;
        private Transform VaginaIK;
        private Transform DynaVaginaPos;

        private SkinnedMeshRenderer OreoSoft;
        private SkinnedMeshRenderer OreoHard;
        private SkinnedMeshRenderer Nail;
        private SkinnedMeshRenderer TikubiL;
        private SkinnedMeshRenderer TikubiR;
        private SkinnedMeshRenderer Underhair;

        private GameObject UncensorRoot;
        private HashSet<DynamicBone> SoftDynamicsBones = new HashSet<DynamicBone>();
        private HashSet<DynamicBone> VaginaDynamicBones = new HashSet<DynamicBone>();

        

        private void Awake()
        {
            female = this.GetComponent<Female>();
            if (female == null)
            {
                Destroy(this);
                return;
            }

            AnimBoneRoot = this.transform.Find("p_cf_anim/cf_J_Root").gameObject;
            WearsRoot = this.transform.Find("Wears").gameObject;
            Pelvis = Transform_Utility.FindTransform(AnimBoneRoot.transform, "cf_J_Kosi02_s");
            Kokan = Transform_Utility.FindTransform(Pelvis, "cf_J_Kokan");
            VaginaIK = Transform_Utility.FindTransform(Kokan, "k_f_kokan_00");

            DynaVaginaPos = new GameObject("DynaVaginaPos").transform;
            DynaVaginaPos.gameObject.layer = 8;
            DynaVaginaPos.SetParent(Pelvis, false);
            DynaVaginaPos.localPosition = new Vector3(0f, -0.07f, -0.01f);

            NudeBody = Transform_Utility.FindTransform(this.transform, "cf_O_body_00").GetComponent<SkinnedMeshRenderer>();
            Nail = Transform_Utility.FindTransform(this.transform, "cf_O_nail").GetComponent<SkinnedMeshRenderer>();
            TikubiL = Transform_Utility.FindTransform(this.transform, "cf_O_tikubiL_00").GetComponent<SkinnedMeshRenderer>();
            TikubiR = Transform_Utility.FindTransform(this.transform, "cf_O_tikubiR_00").GetComponent<SkinnedMeshRenderer>();
            Underhair = Transform_Utility.FindTransform(this.transform, "cf_O_mnpk").GetComponent<SkinnedMeshRenderer>();


            AssetBundleController assetBundleController = AssetBundleController.New_OpenFromFile(OreoPath, OreoSoftAB);

            GameObject OreoRoot = assetBundleController.LoadAndInstantiate<GameObject>(OreoSoftName);
            if (OreoRoot == null)
            {
                assetBundleController.Close(true);
                Destroy(OreoRoot);
                Destroy(this);
                return;
            }

            TextAsset ta = assetBundleController.LoadAsset<TextAsset>("additional_bones");
            if (ta != null)
            {
                string[] lines = ta.text.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string line in lines)
                {
                    string[] cells = line.Split('\t');
                    if (!cells[0].Equals(OreoSoftName))
                        continue;

                    for (int i = 1; i < cells.Length; i++)
                        AdditionalBones.Add(cells[i]);
                    break;
                }
            }

             assetBundleController.Close(false);


            if (AdditionalBones.Count != 0)
            {
                Tools.RecurseTransforms(OreoRoot.transform, bone =>
                {
                    if (AdditionalBones.Contains(bone.name))
                    {
                        Transform parent = Transform_Utility.FindTransform(AnimBoneRoot.transform, bone.parent.name);
                        Vector3 localPos = bone.localPosition;
                        Quaternion localRot = bone.localRotation;
                        Vector3 localScale = bone.localScale;
                        bone.SetParent(parent);
                        bone.localPosition = localPos;
                        bone.localRotation = localRot;
                        bone.localScale = localScale;
                        return true;
                    }
                    return false;
                });
            }

            OreoRoot.transform.SetParent(WearsRoot.transform, false);
            GameObject OreoMeshRoot = Transform_Utility.FindTransform(OreoRoot.transform, "cf_N_O_root").gameObject;
            if (OreoMeshRoot == null)
            {
                Destroy(OreoRoot);
                Destroy(this);
                return;
            }


            GameObject OreoHardRoot = AssetBundleLoader.LoadAndInstantiate<GameObject>(OreoPath, OreoHardAB, OreoHardName);
            Transform HardBody = Transform_Utility.FindTransform(OreoHardRoot.transform, "cf_O_body_00");
            if (HardBody != null) HardBody.SetParent(OreoMeshRoot.transform, false);
            else Destroy(OreoHardRoot);


            AttachBoneWeight.Attach(AnimBoneRoot, OreoMeshRoot, true);


            UncensorRoot = new GameObject("OreoBody");
            UncensorRoot.transform.SetParent(OreoMeshRoot.transform, false);

            SkinnedMeshRenderer[] OreoMeshRenderer = OreoMeshRoot.GetComponentsInChildren<SkinnedMeshRenderer>(true);
            foreach (SkinnedMeshRenderer renderer in OreoMeshRenderer)
            {
                renderer.rootBone = NudeBody.rootBone;
                renderer.reflectionProbeUsage = ReflectionProbeUsage.Off;
                renderer.updateWhenOffscreen = true;
                renderer.gameObject.layer = 8;
                renderer.gameObject.tag = "";   
                renderer.gameObject.SetActive(false);
                if (renderer.name == "cf_O_body_a" || renderer.name == "cf_O_unc_a" || renderer.name == "cf_O_body_00" || renderer.name == "cf_O_unc_a.01" || renderer.name == "cf_O_body_b" || renderer.name == "cf_O_unc_b")
                {
                    renderer.sharedMaterials = NudeBody.sharedMaterials;
                    renderer.gameObject.SetActive(true);
                    NudeBody.enabled = false;
                    if (renderer.name == "cf_O_body_a")
                    {
                        OreoSoft = renderer;
                        renderer.transform.SetParent(UncensorRoot.transform, false);
                    }
                    else if (renderer.name == "cf_O_body_00")
                    {
                        OreoHard = renderer;
                        renderer.transform.SetParent(UncensorRoot.transform, false);
                    }
                    else if (renderer.name == "cf_O_body_b")
                    {
                        OreoHard = renderer;
                        renderer.transform.SetParent(UncensorRoot.transform, false);
                    }
                }
                else if (renderer.name == "cf_O_nail_a_custom")                                  //발가락 모드 대응 네일
                {
                    Nail.bones = renderer.bones;
                    Nail.sharedMesh = renderer.sharedMesh;
                    Destroy(renderer.gameObject);
                }
                else if (renderer.name == "N_tikubi_L_custom")                                   //블렌드셰이프 대응 젖꼭지L
                {
                    TikubiL.bones = renderer.bones;
                    TikubiL.sharedMesh = renderer.sharedMesh;
                    Destroy(renderer.gameObject);
                }
                else if (renderer.name == "N_tikubi_R_custom")                                   //블렌드셰이프 대응 젖꼭지R
                {
                    TikubiR.bones = renderer.bones;
                    TikubiR.sharedMesh = renderer.sharedMesh;
                    Destroy(renderer.gameObject);
                }
                else if (renderer.name == "cf_O_mnpk_0_custom")                                  //블렌드셰이프 대응 음모; PH/HS1과 메시 모두 다름
                {
                    Underhair.bones = renderer.bones;
                    Underhair.sharedMesh = renderer.sharedMesh;
                    Destroy(renderer.gameObject);
                }
                else if (renderer.name == "Phair_AM_04" || renderer.name == "Phair_AM_26" || renderer.name == "Phair_AM_26_part_2" || renderer.name == "Phair_AM_27" || renderer.name == "Phair_AM_28")
                {
                    Transform Phair = Transform_Utility.FindTransform(Underhair.transform.parent, "Phair" + DynaUncensor.OreoVer.ToString());
                    if (Phair == null)
                    {
                        Phair = new GameObject("Phair" + DynaUncensor.OreoVer.ToString()).transform;
                        Phair.SetParent(Underhair.transform.parent, false);
                    }
                    Phair.gameObject.SetActive(false);

                    renderer.transform.SetParent(Phair, false);
                    renderer.gameObject.SetActive(true);
                }
            }
            female.body.ChangeUnderHair();
            female.wears.CheckBodyShow(true);

            UncensorRoot.transform.SetParent(this.transform, false);
            Destroy(OreoRoot);
            Destroy(OreoHardRoot);

            DynamicBone[] dynamibones = AnimBoneRoot.GetComponentsInChildren<DynamicBone>();
            foreach(DynamicBone dynamibone in dynamibones)
            {
                if (dynamibone.name.Contains("soft")) SoftDynamicsBones.Add(dynamibone);
                else if (dynamibone.name.Contains("dyna_puss")) VaginaDynamicBones.Add(dynamibone);
            }

            Transform[] transforms = this.GetComponentsInChildren<Transform>(true);
            foreach (Transform child in transforms)
            {
                if (child.name == "cf_J_Root" && child.parent.name != "p_cf_anim") Destroy(child.gameObject);
            }


            HardBody.gameObject.SetActive(false);
        }


        internal void LateUpdate()              //항상 fbik의 LateUpdate 이후에 호출되어 오직 item에만 적용되는 듯함. Harmony 사용보다 가벼움.
        {
            VaginaIK.position += DynaVaginaPos.position - Kokan.position;
        }


        internal void SyncBodySkin(Renderer nudebody)
        {
            Renderer [] BodyRenderer = { OreoSoft, OreoHard };

            foreach (Renderer renderer in BodyRenderer)
            {
                if (renderer != null) renderer.sharedMaterials = nudebody.sharedMaterials;
            }
        }


        internal void SetupBodyDynamicBones()       //남자 혹은 딜도 다이나믹본콜라이더 추가 시 호출되어 콜라이더 등록
        {
            DynaMale.DynaCollMale = new HashSet<DynamicBoneCollider>(DynaMale.DynaCollMale.Where(item => item != null));
            DynaToy.DynaCollToy = new HashSet<DynamicBoneCollider>(DynaToy.DynaCollToy.Where(item => item != null));

            foreach (DynamicBone dynamicBone in SoftDynamicsBones)
            {
                dynamicBone.m_Colliders.Clear();
                dynamicBone.m_Colliders.AddRange(DynaMale.DynaCollMale);
                dynamicBone.m_Colliders.AddRange(DynaToy.DynaCollToy);
            }
            foreach (DynamicBone dynamicBone in VaginaDynamicBones)
            {
                dynamicBone.m_Colliders.Clear();
                dynamicBone.m_Colliders.AddRange(DynaMale.DynaCollMale);
                dynamicBone.m_Colliders.AddRange(DynaToy.DynaCollToy);
            }
            
            foreach (DynamicBone_Ver02 dynamicBone_Ver02 in AnimBoneRoot.GetComponentsInParent<DynamicBone_Ver02>())
            {
                dynamicBone_Ver02.Colliders.Clear();
                dynamicBone_Ver02.Colliders.AddRange(DynaMale.DynaCollMale);
                dynamicBone_Ver02.Colliders.AddRange(DynaToy.DynaCollToy);
            }
        }

        public void ShowFemale(bool show)
        {
            if(UncensorRoot != null) UncensorRoot.SetActive(show);
        }


        public void EnableSoftBody()
        {
            foreach(DynamicBone dynamicBone in SoftDynamicsBones)
            {
                dynamicBone.enabled = true;
            }
        }

        public void DisableSoftBody()
        {
            foreach (DynamicBone dynamicBone in SoftDynamicsBones)
            {
                dynamicBone.enabled = false;
            }
        }

        public void EnableDynaVagina()
        {
            foreach (DynamicBone dynamicBone in VaginaDynamicBones)
            {
                dynamicBone.enabled = true;
            }
        }

        public void DisableDynaVagina()
        {
            foreach (DynamicBone dynamicBone in VaginaDynamicBones)
            {
                dynamicBone.enabled = false;
            }
        }

    }   
}
