using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace SDI.UI
{
    public class LobbyInfo : NetworkBehaviour
    {
        public static LobbyInfo Instance;

        public NetworkVariable<int> maxPlayers = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
        private void Awake()
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public int GetMaxPlayers()
        {
            return maxPlayers.Value;
        }
    }
}
