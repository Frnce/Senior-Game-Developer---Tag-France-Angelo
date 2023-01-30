using SDI.Enums;
using SDI.UI;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace SDI.Players
{
    public class PlayerRole : NetworkBehaviour
    {
        [SerializeField]
        private SpriteRenderer playerIdentifier;

        public NetworkVariable<Roles> roles = new NetworkVariable<Roles>(Enums.Roles.GUARD,NetworkVariableReadPermission.Everyone,NetworkVariableWritePermission.Owner);
        public void SetRoleName()
        {
            RolesUI.Instance.SetRoleText(roles.Value.ToString());
        }
        public void SetPlayerIdentifier(Color color)
        {
            playerIdentifier.color = color;
        }
    }
}