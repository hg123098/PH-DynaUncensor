using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace PH_DynaUncensor
{
    internal class DynaPenetration : MonoBehaviour
    {
        internal Transform Pelvis;

        internal void Init(Transform TargetVag)
        {
            Transform transform = TargetVag.parent.parent;
            if (transform.name == "cf_J_Kosi02_s") Pelvis = transform;
            OnEnable();
        }

        internal void OnEnable()
        {
            if(Pelvis != null)
            {
                Vector3 currentRotation = Pelvis.localEulerAngles;
                Pelvis.localEulerAngles = new Vector3(-15f, currentRotation.y, currentRotation.z);
            }
        }

        internal void OnDisable()
        {
            if (Pelvis != null)
            {
                Vector3 currentRotation = Pelvis.localEulerAngles;
                Pelvis.localEulerAngles = new Vector3(0f, currentRotation.y, currentRotation.z);
            }
        }

    }
}
