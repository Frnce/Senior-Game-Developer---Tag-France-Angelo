using SDI.Players;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SDI.Objectives
{
    public class EscapeScript : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if(collision.CompareTag("Player") && FindObjectOfType<PlayerNetwork>().HasTreasure)
            {
                Debug.Log("THIEF WINS");
            }
        }
    }

}