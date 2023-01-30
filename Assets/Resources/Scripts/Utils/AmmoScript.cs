using SDI.Interfaces;
using SDI.Managers;
using SDI.Players;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace SDI.Utils
{
    public class AmmoScript : NetworkBehaviour
    {
        private bool isHit;
        [SerializeField]
        private bool hasTempPermanence;

        public struct AmmoData : INetworkSerializable
        {
            public Vector2 direction;
            public float angle;
            public float speed;

            public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
            {
                serializer.SerializeValue(ref direction);
                serializer.SerializeValue(ref angle);
                serializer.SerializeValue(ref speed);
            }
        }

        public NetworkVariable<AmmoData> ammoData = new NetworkVariable<AmmoData>(new AmmoData { speed = 10f, angle = 0f, direction = new Vector2(0, 0) }, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

        public void Setup(Vector2 direction, float speed, float angle)
        {
            ammoData.Value = new AmmoData
            {
                direction = direction,
                speed = speed,
                angle = angle
            };
        }
        // Start is called before the first frame update
        void Start()
        {
            Quaternion quat = Quaternion.identity;
            quat.eulerAngles = new Vector3(0, 0, ammoData.Value.angle);
            transform.localRotation = quat;
        }

        // Update is called once per frame
        void Update()
        {
            if (!isHit)
            {
                MoveClientRpc();
            }
        }
        [ClientRpc]
        private void MoveClientRpc()
        {
            Vector2 tempPos = transform.position;
            tempPos += ammoData.Value.direction * ammoData.Value.speed * Time.deltaTime;
            transform.position = tempPos;
        }
        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                collision.gameObject.GetComponentInParent<IDamageable>().OnHit();
                Debug.Log("hit");
                isHit = true;
                DespawnServerRpc();
            }
        }
        [ServerRpc]
        private void DespawnServerRpc()
        {
            GetComponent<NetworkObject>().Despawn();
            Destroy(gameObject);
        }
        public override void OnNetworkSpawn()
        {
            ammoData.OnValueChanged += (AmmoData prevValue, AmmoData newValue) =>
            {
                Debug.Log("New Value " + newValue.angle + " / " + newValue.speed+ " / "+newValue.direction);
            };
        }
    }

}