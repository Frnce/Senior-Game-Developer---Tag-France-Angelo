using SDI.Managers;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SDI.UI
{
    public class EditPlayerName : MonoBehaviour
    {
        public static EditPlayerName Instance { get; set; }
        public event EventHandler OnNameChanged;
        [SerializeField]
        private TextMeshProUGUI playerNameText;
        private string playerName = "Player Name";

        private void Awake()
        {
            Instance = this;
            GetComponent<Button>().onClick.AddListener(() =>
            {
                ShowInputField();
            });

            playerNameText.text = playerName;
        }
        // Start is called before the first frame update
        void Start()
        {
            ShowInputField();
        }

        private void ShowInputField()
        {
            UI_InputWindow.Show_Static("Player Name", playerName, "abcdefghijklmnopqrstuvxywzABCDEFGHIJKLMNOPQRSTUVXYWZ .,-", 20, () => { }, (string newName) =>
            {
                playerName = newName;
                playerNameText.text = playerName;
                OnNameChanged?.Invoke(this, EventArgs.Empty);
            });
        }

        private void EditPlayerName_OnNameChanged(object sender, EventArgs e)
        {
            LobbyManager.Instance.UpdatePlayerName(GetPlayerName());
        }
        public string GetPlayerName()
        {
            return playerName;
        }
    }
}