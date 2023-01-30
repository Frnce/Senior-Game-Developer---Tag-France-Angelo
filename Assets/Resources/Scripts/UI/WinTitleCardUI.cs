using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace SDI.UI
{
    public class WinTitleCardUI : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI titleText;

        public void Initialize(string text)
        {
            titleText.text = text;
        }
    }
}