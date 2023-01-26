using SDI.Managers;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SDI.UI
{
    public class LobbyCreateUI : MonoBehaviour
    {
        public static LobbyCreateUI Instance { get; private set; }

        [SerializeField]
        private Button createButton;
        [SerializeField]
        private Button lobbyNameButton;
        [SerializeField]
        private Button publicPrivateButton;
        [SerializeField]
        private Button maxPlayersButton;
        [SerializeField]
        private TextMeshProUGUI lobbyNameText;
        [SerializeField]
        private TextMeshProUGUI publicPrivateText;
        [SerializeField]
        private TextMeshProUGUI maxPlayersText;

        private string lobbyName;
        private bool isPrivate;
        private int maxPlayers;
        private void Awake()
        {
            Instance = this;

            createButton.onClick.AddListener(() =>
            {
                LobbyManager.Instance.CreateLobby(lobbyName, maxPlayers, isPrivate);
                Hide();
            });

            lobbyNameButton.onClick.AddListener(() =>
            {
                UI_InputWindow.Show_Static("Lobby Name", lobbyName, "abcdefghijklmnopqrstuvxywzABCDEFGHIJKLMNOPQRSTUVXYWZ .,-", 20, () => { /* Cancel */ }, (string lobbyName) =>
                 {
                     this.lobbyName = lobbyName;
                     UpdateText();
                 });
            });
            Hide();
        }
        private void UpdateText()
        {
            lobbyNameText.text = lobbyName;
            publicPrivateText.text = isPrivate ? "Private" : "Public";
            maxPlayersText.text = maxPlayers.ToString();
        }
        private void Hide()
        {
            gameObject.SetActive(false);
        }
        public void Show()
        {
            gameObject.SetActive(true);

            lobbyName = "My Lobby";
            isPrivate = false;
            maxPlayers = 4;
            UpdateText();
        }
    }
}