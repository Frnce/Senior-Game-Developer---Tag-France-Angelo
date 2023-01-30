using SDI.Interfaces;
using SDI.Players;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SDI.Objectives
{
    public class TreasureDoorScript : MonoBehaviour, IInteractable
    {
        public void Interact()
        {
            if (FindObjectOfType<PlayerNetwork>().HasKey)
            {
                gameObject.SetActive(false) ;
            }
        }
    }
}