using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SDI.UI
{
    public class SimpleLoadingScreenUI : MonoBehaviour
    {
        public static SimpleLoadingScreenUI Instance { get; private set; }
        private void Awake()
        {
            Instance = this;
        }
        private void Start()
        {
            Hide();
        }
        public void Show()
        {
            gameObject.SetActive(true);
        }
        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}