using Character;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace PH_DynaUncensor
{
    internal static class Extensions
    {
        internal static void ApplyCoordinate(this Human human, WEAR_TYPE wearType)
        {
            human.wears.WearInstantiate(wearType, human.body.SkinMaterial, human.body.CustomHighlightMat_Skin);
            human.wears.CheckShow(true);
            if (human.sex == SEX.FEMALE) (human as Female).OnShapeApplied();
            for (int i = 0; i < 10; i++)
            {
                human.accessories.AccessoryInstantiate(human.customParam.acce, i, false, null);
            }
            Resources.UnloadUnusedAssets();
            if (human.sex == SEX.FEMALE) (human as Female).SetupDynamicBones();
            if (human.sex == SEX.MALE) (human as Male).ChangeMaleShow((human as Male).MaleShow);
        }
    }
}
