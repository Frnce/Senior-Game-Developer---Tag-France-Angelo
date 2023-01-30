using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace SDI.UI
{
    public class RolesUI : MonoBehaviour
    {
        public static RolesUI Instance;

        [SerializeField]
        private TextMeshProUGUI rolesText;

        private void Awake()
        {
            Instance = this;
        }
        public void SetRoleText(string role)
        {
            rolesText.text = role;
        }
    }
}