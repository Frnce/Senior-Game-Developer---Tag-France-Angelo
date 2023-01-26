using SDI.Managers;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Authentication;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

namespace SDI.UI
{
    public class LobbyUI : MonoBehaviour
    {
        public static LobbyUI Instance { get; private set; }
        [SerializeField]
        private Transform playerSingletemplate;
        [SerializeField]
        private Transform container;
        [SerializeField]
        private TextMeshProUGUI lobbyNameText;
        [SerializeField]
        private TextMeshProUGUI playerCountText;
        [SerializeField]
        private Button leaveLobbyButton;
        [SerializeField]
        private Button startGameButton;

        private LobbyManager lobbyManager;

        private void Awake()
        {
            Instance = this;

            playerSingletemplate.gameObject.SetActive(false);

            leaveLobbyButton.onClick.AddListener(() =>
            {
                LobbyManager.Instance.LeaveLobby();
            });
            startGameButton.onClick.AddListener(() =>
            {
                LobbyManager.Instance.StartGame();
            });
        }
        private void Start()
        {
            lobbyManager = LobbyManager.Instance;

            lobbyManager.OnJoinedLobby += UpdateLobby_Event;
            lobbyManager.OnJoinedLobbyUpdate += UpdateLobby_Event;
            lobbyManager.OnLeftLobby += LobbyManager_OnLeftLobby;
            lobbyManager.OnKickedFromLobby += LobbyManager_OnLeftLobby;

            Hide();
        }
        private void LobbyManager_OnLeftLobby(object sender, System.EventArgs e)
        {
            ClearLobby();
            Hide();
        }

        private void UpdateLobby_Event(object sender, LobbyManager.LobbyEventArgs e)
        {
            UpdateLobby(lobbyManager.GetJoinedLobby());
        }
        private void UpdateLobby(Lobby lobby)
        {
            ClearLobby();

            foreach (Player player in lobby.Players)
            {
                Transform playerSingleTransform = Instantiate(playerSingletemplate, container);
                playerSingleTransform.gameObject.SetActive(true);
                LobbyPlayerSingleUI lobbyPlayerSingleUI = playerSingleTransform.GetComponent<LobbyPlayerSingleUI>();

                lobbyPlayerSingleUI.SetKickPlayerButtonVisible(
                    lobbyManager.IsLobbyHost() && player.Id != AuthenticationService.Instance.PlayerId);

                lobbyPlayerSingleUI.UpdatePlayer(player);
            }

            lobbyNameText.text = lobby.Name;
            playerCountText.text = lobby.Players.Count + " / " + lobby.MaxPlayers;

            Show();
        }
        private void ClearLobby()
        {
            foreach (Transform child in container)
            {
                if (child == playerSingletemplate) continue;
                Destroy(child.gameObject);
            }
        }

        private void Hide()
        {
            gameObject.SetActive(false);
        }
        private void Show()
        {
            gameObject.SetActive(true);
        }
    }
}