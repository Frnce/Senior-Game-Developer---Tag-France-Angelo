using Cinemachine;
using SDI.Enums;
using SDI.Players;
using SDI.UI;
using SDI.Utils;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Netcode;
using UnityEngine;

namespace SDI.Managers
{
    public class GameManager : NetworkBehaviour
    {
        public static GameManager Instance = null;

        [SerializeField]
        private CinemachineVirtualCamera virtualCamera;
        [Space]
        [SerializeField]
        private List<Transform> spawnLocations = new List<Transform>();
        [Header("Objective")]
        [SerializeField]
        private GameObject keyPrefab;
        [SerializeField]
        private List<Transform> keySpawnLocations = new List<Transform>();
        [Space]
        [SerializeField]
        private GameObject winCardPanel;
        [SerializeField]
        private TextMeshProUGUI winCardText;
        [Header("Win condition Text")]
        [SerializeField]
        private string winText;
        [Space]
        [Header("Player Colors")]
        public List<Color> playerColors = new List<Color>();

        private NetworkVariable<int> joinedPlayers = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone);

        private NetworkVariable<int> maxPlayers = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
        private NetworkVariable<bool> hasAssigned = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
        private NetworkVariable<int> thiefIndex = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
        private List<NetworkClient> playerList;

        [HideInInspector] public NetworkVariable<int> isGameFinished = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

        private void Awake()
        {
            Instance = this;
        }
        private void Start()
        {
            maxPlayers.Value = LobbyInfo.Instance.GetMaxPlayers();

            winCardPanel.SetActive(false);

            if (IsServer)
            {
                playerList = (List<NetworkClient>)NetworkManager.Singleton.ConnectedClientsList;
            }

            NetworkManager.OnClientConnectedCallback += NetworkManager_OnClientConnectedCallback;
            isGameFinished.OnValueChanged += OnGameFinished;
        }

        private void Update()
        {
            if (!IsServer) return;
            joinedPlayers.Value = NetworkManager.Singleton.ConnectedClients.Count;
        }
        [ServerRpc]
        private void AssignRolesServerRpc()
        {
            //thiefIndex.Value = Random.Range(0, playerList.Count);
            for (int i = 0; i < playerList.Count; i++)
            {
                if (i == thiefIndex.Value)
                {
                    playerList[i].PlayerObject.GetComponent<PlayerRole>().roles.Value = Roles.THIEF;
                }
                else
                {
                    playerList[i].PlayerObject.GetComponent<PlayerRole>().roles.Value = Roles.GUARD;
                }
            }
            hasAssigned.Value = true;
        }
        private void NetworkManager_OnClientConnectedCallback(ulong obj)
        {
            if (joinedPlayers.Value >= maxPlayers.Value && !hasAssigned.Value)
            {
                AssignRolesServerRpc();

                SetRoleNamesClientRpc();
                AssignObjectivesClientRpc();

                AssignPlayerToCameraClientRpc();
                AssignPlayerSpawnLocationClientRpc();

                SpawnKeyServerRpc();
            }
        }
        private void OnGameFinished(int prev, int current)
        {
            Debug.Log(current);
        }
        [ClientRpc]
        private void SetRoleNamesClientRpc()
        {
            NetworkManager.Singleton.SpawnManager.GetLocalPlayerObject().GetComponent<PlayerRole>().SetRoleName();
        }
        [ClientRpc]
        private void AssignObjectivesClientRpc()
        {
            ObjectiveManager.Instance.AssignCurrentObjectives(NetworkManager.Singleton.SpawnManager.GetLocalPlayerObject().GetComponent<PlayerRole>().roles.Value);
            ObjectiveManager.Instance.InitializeObjectives();
        }
        [ClientRpc]
        private void AssignPlayerToCameraClientRpc()
        {
            virtualCamera.Follow = NetworkManager.Singleton.SpawnManager.GetLocalPlayerObject().transform;
        }
        [ClientRpc]
        private void AssignPlayerSpawnLocationClientRpc()
        {
            NetworkManager.Singleton.SpawnManager.GetLocalPlayerObject().transform.position = spawnLocations[System.Convert.ToInt32(NetworkManager.Singleton.SpawnManager.GetLocalPlayerObject().OwnerClientId)].position;
        }
        [ServerRpc]
        private void SpawnKeyServerRpc()
        {
            GameObject go = Instantiate(keyPrefab, keySpawnLocations[UnityEngine.Random.Range(0, keySpawnLocations.Count)].position, Quaternion.identity);
            go.GetComponent<NetworkObject>().Spawn();
        }
        public void ShowWinCard(int val)
        {
            if (val == 1)
            {
                winCardPanel.SetActive(true);
                winCardText.text = "Guards " + winText;
            }
            else
            {
                winCardPanel.SetActive(true);
                winCardText.text = "Thief " + winText;
              
            }
        }
    }
}