using SDI.Enums;
using SDI.Interfaces;
using SDI.Managers;
using SDI.Utils;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace SDI.Players
{
    [RequireComponent(typeof(Rigidbody2D))][RequireComponent(typeof(PlayerController))]
    public class PlayerNetwork : NetworkBehaviour, IDamageable
    {
        [Header("Charater Stats")]
        [SerializeField]
        private float movementSpeed = 10f;
        [Header("Projectile Setting")]
        [SerializeField]
        private GameObject arrowObject;
        [SerializeField]
        private float arrowSpeed;
        [SerializeField]
        private GameObject arm;
        [SerializeField]
        private Transform bowTip;
        [SerializeField]
        private SpriteRenderer spriteRenderer;

        private PlayerController playerController;
        private Rigidbody2D rb2d;
        private Vector3 playerDir;

        private Vector3 mouse;
        private NetworkVariable<float> mouseAngle = new NetworkVariable<float>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        private NetworkVariable<Vector2> mouseVector = new NetworkVariable<Vector2>(new Vector2(), NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

        private Shader hitShader;
        private Shader defaultShader;

        private bool hasKey;
        private bool hasTreasure;
        public bool HasKey { get { return hasKey; } set { hasKey = value; } }
        public bool HasTreasure { get { return hasTreasure; } set { hasTreasure = value; } }
        void Start()
        {
            playerController = GetComponent<PlayerController>();
            rb2d = GetComponent<Rigidbody2D>();
            hitShader = Shader.Find("GUI/Text Shader");
            defaultShader = Shader.Find("Sprites/Default");
        }
        void Update()
        {
            if (!IsOwner) return;

            playerDir = playerController.Movement;
            Aim();
            if (playerController.AcceptKey)
            {
                ShootServerRpc();
            }
        }
        private void FixedUpdate()
        {
            Move();
        }
        private void Move()
        {
            Vector3 movement = playerDir.normalized * movementSpeed * Time.deltaTime;
            rb2d.MovePosition(transform.position + movement);
        }
        private void Aim()
        {
            mouse = Camera.main.ScreenToWorldPoint(playerController.MousePosition);
            mouse.z = Camera.main.nearClipPlane;

            mouseVector.Value = (mouse - transform.position).normalized;
            float gunAngle = Mathf.Atan2(mouseVector.Value.y, mouseVector.Value.x) * Mathf.Rad2Deg - 90f;
            mouseAngle.Value = gunAngle;
            arm.transform.rotation = Quaternion.AngleAxis(mouseAngle.Value, Vector3.forward);
        }
        [ServerRpc]
        private void ShootServerRpc()
        {
            AmmoScript arrow = Instantiate(arrowObject, bowTip.position, Quaternion.identity).GetComponent<AmmoScript>();

            arrow.GetComponent<NetworkObject>().Spawn();
            arrow.Setup(mouseVector.Value, arrowSpeed, mouseAngle.Value);
        }

        public void OnHit()
        {
            DamageEffectClientRpc();
            DefeatServerRpc();
        }
        [ServerRpc]
        private void DefeatServerRpc()
        {
            GetComponent<NetworkObject>().Despawn();
            Destroy(gameObject);
        }
        [ClientRpc]
        private void DamageEffectClientRpc()
        {
            spriteRenderer.material.shader = hitShader;
            spriteRenderer.material.color = Color.white;
            StartCoroutine(WaitForSpawn());
        }
        IEnumerator WaitForSpawn() // for hitStop and other hit fx
        {
            while (Time.timeScale != 1.0f)
            {
                yield return null;//wait for hit stop to end
            }
            yield return new WaitForSeconds(0.1f);
            spriteRenderer.material.shader = defaultShader;
        }
        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();
            if (GetComponent<PlayerRole>().roles.Value == Enums.Roles.THIEF)
            {
                ObjectiveManager.Instance.Kill("Thief");
            }
        }
        private void OnTriggerStay2D(Collider2D collision)
        {
            if (collision.CompareTag("Interactable"))
            {
                if (playerController.InteractKey)
                {
                    Debug.Log("Interaction Near");
                    collision.GetComponent<IInteractable>().Interact();
                }
            }
        }
    }
}