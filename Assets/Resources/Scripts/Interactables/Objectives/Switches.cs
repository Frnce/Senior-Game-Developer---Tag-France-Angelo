using SDI.Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SDI.Objectives
{
    public class Switches : MonoBehaviour,IInteractable
    {
        private bool hasSwitched = false;
        private HiddenSwitchesObjective hiddenSwitcheObjective;

        private void Start()
        {
            hiddenSwitcheObjective = GetComponentInParent<HiddenSwitchesObjective>();
        }
        public void Interact()
        {
            if (hasSwitched == true) return;

            hasSwitched = true;
            hiddenSwitcheObjective.SwitchesOpened++;
        }
    }
}
