using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SDI.Managers
{
    public class GameManager : MonoBehaviour
    {
        private static GameManager instance = null;
        public static GameManager Instance
        {
            get
            {
                if (instance != null)
                {
                    return instance;
                }
                return null;
            }
        }
        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            if (instance != this)
            {
                Destroy(gameObject);
                return;
            }
            DontDestroyOnLoad(gameObject);
        }
        private void Start()
        {
           
        }
    }
}