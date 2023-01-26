using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SDI.Objectives
{
    public class HiddenSwitchesObjective : MonoBehaviour
    {
        [SerializeField]
        private Switches[] switches;
        [SerializeField]
        private KeyDoorScript keyDoor;

        private int switchesOpened;
        public int SwitchesOpened { get { return switchesOpened; } set { switchesOpened = value; } }

        private void Update()
        {
            if (switchesOpened >= switches.Length)
            {
                keyDoor.OpenDoor();
            }
        }
    }

}