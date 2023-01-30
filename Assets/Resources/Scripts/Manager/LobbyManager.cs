using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine.SceneManagement;
using Unity.Netcode;
using SDI.UI;

namespace SDI.Managers
{
    public class LobbyManager : MonoBehaviour
    {
        public static LobbyManager Instance { get; private set; }

        public const string KEY_PLAYER_NAME = "PlayerName";
        public const string KEY_START_GAME = "0";

        public event EventHandler OnLeftLobby;

        public event EventHandler<LobbyEventArgs> OnJoinedLobby;
        public event EventHandler<LobbyEventArgs> OnJoinedLobbyUpdate;
        public event EventHandler<LobbyEventArgs> OnKickedFromLobby;

        public event EventHandler OnGameStarted;
        public class LobbyEventArgs : EventArgs
        {
            public Lobby lobby;
        }

        public event EventHandler<OnLobbyListChangedEventArgs> OnLobbyListChanged;
        public class OnLobbyListChangedEventArgs : EventArgs
        {
            public List<Lobby> lobbyList;
        }

        private float heartbeatTimer;
        private float lobbyPollTimer;
        private float refreshLobbyListTimer = 5f;
        private Lobby joinedLobby;
        private string playerName;

        private void Awake()
        {
            Instance = this;
        }
        // Update is called once per frame
        void Update()
        {
            HandleLobbyHeartBeat();
            HandleLobbyPolling();
        }
        public async void Authenticate(string playerName)
        {
            this.playerName = playerName;
            InitializationOptions initializationOptions = new InitializationOptions();
            initializationOptions.SetProfile(playerName);

            await UnityServices.InitializeAsync(initializationOptions);

            AuthenticationService.Instance.SignedIn += () =>
            {
                Debug.Log("Signed in!");
                RefreshLobbyList();
            };

            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }
        private async void HandleLobbyHeartBeat()
        {
            if (IsLobbyHost())
            {
                heartbeatTimer -= Time.deltaTime;
                if (heartbeatTimer < 0f)
                {
                    float maxHeartBeatTimer = 15f;
                    heartbeatTimer = maxHeartBeatTimer;

                    await LobbyService.Instance.SendHeartbeatPingAsync(joinedLobby.Id);
                }
            }
        }
        private async void HandleLobbyPolling()
        {
            if (joinedLobby != null)
            {
                lobbyPollTimer -= Time.deltaTime;
                if (lobbyPollTimer < 0f)
                {
                    float maxLobbyPollTimer = 1.1f;
                    lobbyPollTimer = maxLobbyPollTimer;

                    joinedLobby = await LobbyService.Instance.GetLobbyAsync(joinedLobby.Id);
                    OnJoinedLobbyUpdate?.Invoke(this, new LobbyEventArgs { lobby = joinedLobby });

                    if (!IsPlayerInLobby())
                    {
                        OnKickedFromLobby?.Invoke(this, new LobbyEventArgs { lobby = joinedLobby });
                        joinedLobby = null;
                    }

                    if(joinedLobby.Data[KEY_START_GAME].Value != "0")
                    {
                        if (!IsLobbyHost())
                        {
                            RelayManager.Instance.JoinRelay(joinedLobby.Data[KEY_START_GAME].Value);
                        }
                        joinedLobby = null;

                        OnGameStarted?.Invoke(this, EventArgs.Empty);
                    }
                }
            }
        }
        public Lobby GetJoinedLobby()
        {
            return joinedLobby;
        }
        public bool IsLobbyHost()
        {
            return joinedLobby != null && joinedLobby.HostId == AuthenticationService.Instance.PlayerId;
        }
        private bool IsPlayerInLobby()
        {
            if (joinedLobby != null && joinedLobby.Players != null)
            {
                foreach (Player player in joinedLobby.Players)
                {
                    if (player.Id == AuthenticationService.Instance.PlayerId)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        private Player GetPlayer()
        {
            return new Player(AuthenticationService.Instance.PlayerId, null, new Dictionary<string, PlayerDataObject>
            {
                { KEY_PLAYER_NAME,new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public,playerName) }
            });
        }
        public async void CreateLobby(string lobbyName, int maxPlayers, bool isPrivate)
        {
            Player player = GetPlayer();

            CreateLobbyOptions options = new CreateLobbyOptions
            {
                Player = player,
                IsPrivate = isPrivate,
                Data = new Dictionary<string, DataObject> { { KEY_START_GAME, new DataObject(DataObject.VisibilityOptions.Member, "0") } }
            };
            Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers, options);
            joinedLobby = lobby;
            OnJoinedLobby?.Invoke(this, new LobbyEventArgs { lobby = lobby });
        }
        public async void RefreshLobbyList()
        {
            try
            {
                QueryLobbiesOptions options = new QueryLobbiesOptions();
                options.Count = 25;

                //Filter for open lobbies only
                options.Filters = new List<QueryFilter>
                {
                    new QueryFilter(
                        field:QueryFilter.FieldOptions.AvailableSlots,
                        op: QueryFilter.OpOptions.GT,
                        value: "0")
                };

                //Order by newest Lobby First
                options.Order = new List<QueryOrder>
                {
                    new QueryOrder(
                        asc:false,
                        field:QueryOrder.FieldOptions.Created)
                };

                QueryResponse lobbyListQueryResponse = await Lobbies.Instance.QueryLobbiesAsync();

                OnLobbyListChanged?.Invoke(this, new OnLobbyListChangedEventArgs { lobbyList = lobbyListQueryResponse.Results });
            }
            catch (LobbyServiceException ex)
            {
                Debug.Log(ex);
            }
        }
        public async void JoinLobby(Lobby lobby)
        {
            Player player = GetPlayer();
            joinedLobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobby.Id, new JoinLobbyByIdOptions
            {
                Player = player
            });
        }

        public async void UpdatePlayerName(string playerName)
        {
            this.playerName = playerName;
            if (joinedLobby != null)
            {
                try
                {
                    UpdatePlayerOptions options = new UpdatePlayerOptions();
                    options.Data = new Dictionary<string, PlayerDataObject>()
                    {
                        {
                            KEY_PLAYER_NAME,
                            new PlayerDataObject(
                                visibility:PlayerDataObject.VisibilityOptions.Public,
                                value : playerName)
                        }
                    };

                    string playerId = AuthenticationService.Instance.PlayerId;

                    Lobby lobby = await LobbyService.Instance.UpdatePlayerAsync(joinedLobby.Id, playerId, options);
                    joinedLobby = lobby;

                    OnJoinedLobbyUpdate?.Invoke(this, new LobbyEventArgs { lobby = joinedLobby });
                }
                catch (LobbyServiceException ex)
                {
                    Debug.Log(ex);
                }
            }
        }
        public async void LeaveLobby()
        {
            if (joinedLobby != null)
            {
                try
                {
                    await LobbyService.Instance.RemovePlayerAsync(joinedLobby.Id, AuthenticationService.Instance.PlayerId);

                    joinedLobby = null;

                    OnLeftLobby?.Invoke(this, EventArgs.Empty);
                }
                catch (LobbyServiceException ex)
                {
                    Debug.Log(ex);
                }
            }
        }
        public async void KickPlayer(string playerId)
        {
            if (IsLobbyHost())
            {
                try
                {
                    await LobbyService.Instance.RemovePlayerAsync(joinedLobby.Id, playerId);
                }
                catch (LobbyServiceException ex)
                {
                    Debug.Log(ex);
                }
            }
        }
        public async void StartGame()
        {
            if (IsLobbyHost())
            {
                try
                {
                    ShowLoadScreenClientRpc();
                    string relayCode = await RelayManager.Instance.CreateRelay();

                    Lobby lobby = await Lobbies.Instance.UpdateLobbyAsync(joinedLobby.Id, new UpdateLobbyOptions
                    {
                        Data = new Dictionary<string, DataObject> { { KEY_START_GAME, new DataObject(DataObject.VisibilityOptions.Member, relayCode) } }
                    });

                    LobbyInfo.Instance.maxPlayers.Value = joinedLobby.Players.Count;

                    NetworkManager.Singleton.SceneManager.LoadScene("Game", LoadSceneMode.Single);
                }
                catch (LobbyServiceException ex)
                {
                    Debug.Log(ex);
                }
            }
        }
        [ClientRpc]
        private void ShowLoadScreenClientRpc()
        {
            SimpleLoadingScreenUI.Instance.Show();
        }
        public int GetPlayersList()
        {
            return joinedLobby.Players.Count;
        }
    }
}
