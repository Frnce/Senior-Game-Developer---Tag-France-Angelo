using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;

namespace SDI.Managers
{
    public class RelayManager : MonoBehaviour
    {
        public static RelayManager Instance { get; private set; }
        private void Awake()
        {
            Instance = this;
        }
        public async Task<string> CreateRelay()
        {
            try
            {
                Allocation allocation = await RelayService.Instance.CreateAllocationAsync(3);
                string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
                
                RelayServerData relayServerData = new RelayServerData(allocation, "dtls");
                NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

                NetworkManager.Singleton.StartHost();

                return joinCode;
            }
            catch(RelayServiceException ex)
            {
                Debug.Log(ex);
                return null;
            }
        }
        public async void JoinRelay(string joinCode)
        {
            try
            {
                JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);

                RelayServerData relayServerData = new RelayServerData(joinAllocation, "dtls");
                NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

                NetworkManager.Singleton.StartClient();
            }
            catch(RelayServiceException ex)
            {
                Debug.Log(ex);
            }
        }
    }
}