using SDI.Interfaces;
using SDI.Players;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SDI.Objectives
{
    public class TreasureScript : MonoBehaviour, IInteractable
    {
        private bool isOpened = false;
        public void Interact()
        {
            if (isOpened) return;

            FindObjectOfType<PlayerNetwork>().HasTreasure = true;
            isOpened = true;
            //Change Sprite to open
        }

    }
}