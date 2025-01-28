using System;
using BepInEx;
using BepInEx.Configuration;
using H;
using Character;
using HarmonyLib;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System.Linq;
using RootMotion.FinalIK;
using UniRx.Triggers;

namespace PH_DynaUncensor
{
    [BepInProcess("PlayHome32bit")]
    [BepInProcess("PlayHome64bit")]
    [BepInProcess("PlayHomeStudio32bit")]
    [BepInProcess("PlayHomeStudio64bit")]
    //[BepInProcess("VR GEDOU")]
    [BepInPlugin(GUID, PluginName, VERSION)]
    public class DynaUncensor : BaseUnityPlugin
    {
        internal const string GUID = "HG.DynaUncensor";
        internal const string PluginName = "PH DynaUncensor";
        internal const string VERSION = "1.1";

        internal const int OreoVer = 495;


        private void Awake()
        {
            var harmony = new Harmony("com.HG.DynaUncensor");
            harmony.PatchAll(typeof(Hooks));
            harmony.PatchAll(typeof(UnderHairMod));
            harmony.PatchAll(typeof(WearMod));
            harmony.PatchAll(typeof(HAnimIKMod));
            harmony.PatchAll(typeof(HumanLoadFix));
        }

        private class Hooks
        {
            [HarmonyPostfix, HarmonyPatch(typeof(Female), nameof(Female.Apply))]
            private static void AddDynaFemale(Female __instance)
            {
                if (__instance.GetComponent<DynaFemale>() == null)
                {
                    DynaFemale script = __instance.gameObject.AddComponent<DynaFemale>();
                }
            }

            [HarmonyPostfix, HarmonyPatch(typeof(Wears), nameof(Wears.ChangeBodyMaterial))]
            private static void SyncBodySkin(Renderer bodySkin, Human ___human)
            {
                if (___human == null) return;                                                       //의상카드 생성 시에는 ___human이 NULL

                DynaFemale UIfemale = ___human.GetComponent<DynaFemale>();
                if (UIfemale != null) UIfemale.SyncBodySkin(bodySkin);
            }

            [HarmonyPostfix, HarmonyPatch(typeof(Male), nameof(Male.Apply))]
            private static void AddDynaMale(Male __instance)
            {
                if (__instance.GetComponent<DynaMale>() == null)
                {
                    DynaMale script = __instance.gameObject.AddComponent<DynaMale>();
                }
            }

            [HarmonyPostfix, HarmonyPatch(typeof(H_Item), nameof(H_Item.SetTarget))]
            private static void AddDynaToy(Transform target, H_Item __instance)
            {
                if (target.name == "k_f_kokan_00" && __instance.GetComponent<DynaToy>() == null)
                {
                    DynaToy script = __instance.gameObject.AddComponent<DynaToy>();
                }
            }

            [HarmonyPostfix, HarmonyPatch(typeof(CutAct_CharaShow), nameof(CutAct_CharaShow.Action))]           //ADV에는 여자 숨김 상황이 있음
            private static void CutSceneFix_FemaleShow(CutScene ___cutScene, string ___chara, string ___show)
            {
                Human human = ___cutScene.GetHuman(___chara);

                if (___show.Length > 0)
                {
                    bool flag = true;
                    if (___show.Equals("hide", StringComparison.OrdinalIgnoreCase) || ___show.Equals("false", StringComparison.OrdinalIgnoreCase) || ___show.Equals("off", StringComparison.OrdinalIgnoreCase))
                    {
                        flag = false;
                    }
                    if (human.sex == SEX.FEMALE)
                    {
                        DynaFemale script = human.GetComponent<DynaFemale>();
                        if (script != null) script.ShowFemale(flag);
                    }
                }
            }

        }

        private class UnderHairMod
        {
            [HarmonyPrefix, HarmonyPatch(typeof(Body), nameof(Body.ChangeUnderHair))]
            private static void UnderHairExtend(Human ___human, Renderer ___rend_underhair)
            {
                if (___human == null || ___human.sex != SEX.FEMALE) return;

                BodyParameter body = ___human.customParam.body;
                UnderhairData underhair = CustomDataManager.GetUnderhair(body.underhairID);

                Transform Phair = Transform_Utility.FindTransform(___rend_underhair.transform.parent, "Phair" + OreoVer.ToString());
                if (Phair == null) return;
                
                if (underhair.sub == OreoVer)
                {
                    AssetBundleController assetBundleController = AssetBundleController.New_OpenFromFile(GlobalData.assetBundlePath, underhair.assetbundleName);
                    Renderer[] componentsInChildren = Phair.GetComponentsInChildren<Renderer>(true);
                    foreach (Renderer renderer in componentsInChildren)
                    {
                        string text = renderer.name.Substring(0, 11);
                        renderer.sharedMaterial = assetBundleController.LoadAsset<Material>(text);
                    }
                    Phair.gameObject.SetActive(true);
                    assetBundleController.Close(false);
                }
                else Phair.gameObject.SetActive(false);
            }

            [HarmonyPostfix, HarmonyPatch(typeof(Body), nameof(Body.ChangeUnderHairColor))]
            private static void ChangeUnderHairColorExtend(Human ___human, Renderer ___rend_underhair)
            {
                if (___human == null) return;

                BodyParameter body = ___human.customParam.body;
                Transform Phair = Transform_Utility.FindTransform(___rend_underhair.transform.parent, "Phair" + OreoVer.ToString());
                if (Phair != null && Phair.gameObject.activeSelf)
                {
                    Renderer[] componentsInChildren = Phair.GetComponentsInChildren<Renderer>(true);
                    foreach (Renderer renderer in componentsInChildren)
                    {
                        body.underhairColor.SetToMaterial(renderer.material);
                    }
                }
            }

            [HarmonyPostfix, HarmonyPatch(typeof(Body), nameof(Body.ShowUnderHair3D))]
            private static void ShowUnderHair3DExtend(bool show, Human ___human, Renderer ___rend_underhair)
            {
                if (___human == null || ___human.sex == SEX.MALE) return;

                BodyParameter body = ___human.customParam.body;
                UnderhairData underhair = CustomDataManager.GetUnderhair(body.underhairID);

                Transform Phair = Transform_Utility.FindTransform(___rend_underhair.transform.parent, "Phair" + OreoVer.ToString());
                if (Phair == null) return;

                if (underhair.sub == OreoVer) Phair.gameObject.SetActive(show);
                else Phair.gameObject.SetActive(false);
            }
        }

        private class WearMod
        {
            [HarmonyPrefix, HarmonyPatch(typeof(WearCustomEdit), "ChangeOnWear")]
            private static bool ChangeOnWearOneTypeOnly(WEAR_TYPE wear, int id, WearCustomEdit __instance, Human ___human)
            {
                if (___human == null) return true;

                ___human.customParam.wear.wears[(int)wear].id = id;
                ___human.customParam.wear.wears[(int)wear].color = null;
                MaterialCustomData.GetWear(wear, ___human.customParam.wear.wears[(int)wear]);
                ___human.ApplyCoordinate(wear);
                AccessTools.Method(typeof(WearCustomEdit), "Update_Tab").Invoke(__instance, new object[] { });
                HWearAcceChangeUI hwearAcceChangeUI = UnityEngine.Object.FindObjectOfType<HWearAcceChangeUI>();
                if (hwearAcceChangeUI != null)
                {
                    hwearAcceChangeUI.CheckShowUI();
                }
                return false;
            }

            [HarmonyPostfix, HarmonyPatch(typeof(Wears), nameof(Wears.WearInstantiate))]
            private static void DisableTopBodyGameObject(WEAR_TYPE type, Wears __instance, Human ___human)
            {
                if (___human == null || type != WEAR_TYPE.TOP || ___human.sex != SEX.FEMALE) return;        //의상카드 생성 시에는 ___human이 NULL

                WearObj TopWearObj = __instance.GetWearObj(Character.WEAR_TYPE.TOP);
                if (TopWearObj == null) return;

                GameObject TopRoot = TopWearObj.obj;
                SkinnedMeshRenderer TopBodyA = FindTopBodyMeshObject(TopRoot, "N_top_a");
                if (TopBodyA != null) TopBodyA.gameObject.SetActive(false);
                SkinnedMeshRenderer TopBodyB = FindTopBodyMeshObject(TopRoot, "N_top_b");
                if (TopBodyB != null) TopBodyB.gameObject.SetActive(false);
            }

            private static SkinnedMeshRenderer FindTopBodyMeshObject(GameObject TopRoot, string name)
            {
                Transform TopMesh = Transform_Utility.FindTransform(TopRoot.transform, name);
                if (TopMesh != null)
                {
                    return Transform_Utility.FindTransform_Partial(TopMesh, "body").GetComponentInChildren<SkinnedMeshRenderer>();
                }
                return null;
            }
        }

        private class DynamicBoneMod
        {
            [HarmonyPrefix, HarmonyPatch(typeof(Female), nameof(Female.SetupDynamicBones))]
            private static bool AddDynaColliderMod(Female __instace)
            {
                DynamicBone[] componentsInChildren = __instace.hairs.HirsParent.GetComponentsInChildren<DynamicBone>(true);
                DynamicBone_Ver01[] componentsInChildren2 = __instace.hairs.HirsParent.GetComponentsInChildren<DynamicBone_Ver01>(true);
                DynamicBone_Ver02[] componentsInChildren3 = __instace.hairs.HirsParent.GetComponentsInChildren<DynamicBone_Ver02>(true);
                DynamicBoneCollider[] componentsInChildren4 = __instace.body.Obj.transform.GetComponentsInChildren<DynamicBoneCollider>(true);

                DynaMale.DynaCollMale = new HashSet<DynamicBoneCollider>(DynaMale.DynaCollMale.Where(item => item != null));

                foreach (DynamicBone dynamicBone in componentsInChildren)
                {
                    dynamicBone.m_Colliders.Clear();
                    foreach (DynamicBoneCollider dynamicBoneCollider in componentsInChildren4)
                    {
                        dynamicBone.m_Colliders.Add(dynamicBoneCollider);
                    }
                    dynamicBone.m_Colliders.AddRange(DynaMale.DynaCollMale);
                }
                foreach (DynamicBone_Ver01 dynamicBone_Ver in componentsInChildren2)
                {
                    dynamicBone_Ver.m_Colliders.Clear();
                    foreach (DynamicBoneCollider dynamicBoneCollider2 in componentsInChildren4)
                    {
                        dynamicBone_Ver.m_Colliders.Add(dynamicBoneCollider2);
                    }
                    dynamicBone_Ver.m_Colliders.AddRange(DynaMale.DynaCollMale);
                }
                foreach (DynamicBone_Ver02 dynamicBone_Ver2 in componentsInChildren3)
                {
                    dynamicBone_Ver2.Colliders.Clear();
                    foreach (DynamicBoneCollider dynamicBoneCollider3 in componentsInChildren4)
                    {
                        dynamicBone_Ver2.Colliders.Add(dynamicBoneCollider3);
                    }
                    dynamicBone_Ver2.Colliders.AddRange(DynaMale.DynaCollMale);
                }


                return false;
            }
        }

        private class HAnimIKMod
        {
            [HarmonyPrefix, HarmonyPatch(typeof(H_Members), nameof(H_Members.SetIK), new Type[] { typeof(IK_DataList) })]
            private static void EditPoseIKList(IK_DataList dataList)
            {
                if (dataList != null)
                {
                    if (dataList.ikDatas.Exists(data => data.targetPart == "k_f_ana_00"))
                    {
                        dataList.ikDatas.RemoveAll(data => data.targetPart == "k_f_kokan_00");
                    }
                }
            }

            private static void DestroyDynaPenetration(FullBodyBipedIK fbik)
            {
                DynaPenetration script = fbik.GetComponent<DynaPenetration>();
                if (script != null)
                {
                    script.OnDisable();
                    Destroy(script);
                }
            }

            [HarmonyPrefix, HarmonyPatch(typeof(IK_Control), nameof(IK_Control.SetIK))]
            private static bool AddDynaPenetration(IK_Data.PART part, Transform target, FullBodyBipedIK ___fbik)
            {
                if (target.name != "k_f_kokan_00") return true;
                if (part != IK_Data.PART.TIN /*&& target.IsChildOf(___fbik.transform)*/) return true;

                DestroyDynaPenetration(___fbik);
                DynaPenetration script = ___fbik.gameObject.AddComponent<DynaPenetration>();
                script.Init(target);
                return false;
            }

            [HarmonyPostfix, HarmonyPatch(typeof(IK_Control), nameof(IK_Control.ClearIK), new Type[] { })]
            private static void ReleaseIK(FullBodyBipedIK ___fbik)
            {
                DestroyDynaPenetration(___fbik);
            }

            [HarmonyPostfix, HarmonyPatch(typeof(IK_Control), nameof(IK_Control.ClearIK), new Type[] { typeof(IK_Data.PART) })]
            private static void ReleaseIKADV(IK_Data.PART part, FullBodyBipedIK ___fbik)
            {
                if(part == IK_Data.PART.TIN) DestroyDynaPenetration(___fbik);
            }
        }

        private class HumanLoadFix
        {
            private class HUMANLOADNOAPPLY : MonoBehaviour
            {
                private void Start()
                {
                    Destroy(this);
                }
            }

            [HarmonyPostfix, HarmonyPatch(typeof(Human), nameof(Human.Load), new Type[] { typeof(CustomParameter), typeof(bool) })]
            private static void AddHLNA(bool _apply, Human __instance)
            {
                if (_apply) __instance.gameObject.AddComponent<HUMANLOADNOAPPLY>();
            }

            [HarmonyPrefix, HarmonyPatch(typeof(Female), nameof(Female.Apply))]
            private static bool DuplicatedFemaleApply(Human __instance)
            {
                HUMANLOADNOAPPLY script = __instance.GetComponent<HUMANLOADNOAPPLY>();
                if (script != null) return false;
                else return true;
            }

            [HarmonyPrefix, HarmonyPatch(typeof(Male), nameof(Male.Apply))]
            private static bool DuplicatedMaleApply(Human __instance)
            {
                HUMANLOADNOAPPLY script = __instance.GetComponent<HUMANLOADNOAPPLY>();
                if (script != null) return false;
                else return true;
            }
        }


    }
}