using SDI.Managers;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

namespace SDI.UI
{
    public class LobbyListSingleUI : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI lobbyNameText;
        [SerializeField]
        private TextMeshProUGUI playersText;

        private Lobby lobby;
        private void Awake()
        {
            GetComponent<Button>().onClick.AddListener(() =>
            {
                LobbyManager.Instance.JoinLobby(lobby);
            });
        }
        public void UpdateLobby(Lobby lobby)
        {
            this.lobby = lobby;

            lobbyNameText.text = lobby.Name;
            playersText.text = lobby.Players.Count + " / " + lobby.MaxPlayers;
        }
    }
}