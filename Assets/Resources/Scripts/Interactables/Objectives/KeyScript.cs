using SDI.Interfaces;
using SDI.Managers;
using SDI.Players;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SDI.Objectives
{
    public class KeyScript : MonoBehaviour, IInteractable
    {
        private const string key = "Key";
        public void Interact()
        {
            ObjectiveManager.Instance.Collect(key);
            FindObjectOfType<PlayerNetwork>().HasKey = true;
            gameObject.SetActive(false);
        }
    }
}
