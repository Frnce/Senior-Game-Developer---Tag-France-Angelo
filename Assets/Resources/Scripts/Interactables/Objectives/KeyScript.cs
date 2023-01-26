using SDI.Interfaces;
using SDI.Players;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SDI.Objectives
{
    public class KeyScript : MonoBehaviour, IInteractable
    {
        public void Interact()
        {
            FindObjectOfType<PlayerNetwork>().HasKey = true;
            gameObject.SetActive(false);
        }
    }
}
