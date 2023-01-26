using SDI.Managers;
using System.Collections;
using System.Collections.Generic;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

namespace SDI.UI
{
    public class LobbyListUI : MonoBehaviour
    {
        public static LobbyListUI Instance { get; set; }
        [SerializeField]
        private Transform lobbySingleTemplate;
        [SerializeField]
        private Transform container;
        [SerializeField]
        private Button refreshButton;
        [SerializeField]
        private Button createLobbyButton;

        private LobbyManager lobbyManager;

        private void Awake()
        {
            Instance = this;

            lobbySingleTemplate.gameObject.SetActive(false);

            refreshButton.onClick.AddListener(RefreshButtonClick);
            createLobbyButton.onClick.AddListener(CreateLobbyButtonClick);

            Hide();
        }
        // Start is called before the first frame update
        void Start()
        {
            lobbyManager = LobbyManager.Instance;
            lobbyManager.OnLobbyListChanged += LobbyManager_OnLobbyListChanged;
            lobbyManager.OnJoinedLobby += LobbyManager_OnJoinedLobby;
            lobbyManager.OnLeftLobby += LobbyManager_OnLeftLobby;
            lobbyManager.OnKickedFromLobby += LobbyManager_OnKickedFromLobby;
        }

        private void LobbyManager_OnKickedFromLobby(object sender, LobbyManager.LobbyEventArgs e)
        {
            Show();
        }

        private void LobbyManager_OnLeftLobby(object sender, System.EventArgs e)
        {
            Show();
        }

        private void LobbyManager_OnJoinedLobby(object sender, LobbyManager.LobbyEventArgs e)
        {
            Hide();
        }

        private void LobbyManager_OnLobbyListChanged(object sender, LobbyManager.OnLobbyListChangedEventArgs e)
        {
            UpdateLobbyList(e.lobbyList);
        }

        private void UpdateLobbyList(List<Lobby> lobbyList)
        {
            foreach (Transform child in container)
            {
                if (child == lobbySingleTemplate) continue;
                Destroy(child.gameObject);
            }
            foreach (Lobby lobby in lobbyList)
            {
                Transform lobbySingleTransform = Instantiate(lobbySingleTemplate, container);
                lobbySingleTransform.gameObject.SetActive(true);
                LobbyListSingleUI lobbyListSingleUI = lobbySingleTransform.GetComponent<LobbyListSingleUI>();
                lobbyListSingleUI.UpdateLobby(lobby);
            }
        }
        private void RefreshButtonClick()
        {
            lobbyManager.RefreshLobbyList();
        }
        private void CreateLobbyButtonClick()
        {
            LobbyCreateUI.Instance.Show();
        }
        private void Hide()
        {
            gameObject.SetActive(false);
        }
        public void Show()
        {
            gameObject.SetActive(true);
        }
    }
}