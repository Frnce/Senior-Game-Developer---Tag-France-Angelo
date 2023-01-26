using SDI.Enums;
using SDI.Interfaces;
using SDI.Utils;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace SDI.Players
{
    public class PlayerNetwork : NetworkBehaviour,IDamageable
    {
        [SerializeField]
        private float movementSpeed = 10f;
        [SerializeField]
        private int maxHealth = 5;
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

        private Roles role;
        private PlayerController playerController;
        private Rigidbody2D rb2d;
        private Vector3 playerDir;

        private Vector3 mouse;
        private float mouseAngle;
        private Vector2 mouseVector;

        private int currentHealth;

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

            currentHealth = maxHealth;

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
                Shoot();
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

            mouseVector = (mouse - transform.position).normalized;
            float gunAngle = Mathf.Atan2(mouseVector.y, mouseVector.x) * Mathf.Rad2Deg - 90f;
            mouseAngle = gunAngle;
            arm.transform.rotation = Quaternion.AngleAxis(mouseAngle, Vector3.forward);
        }
        private void Shoot()
        {
            AmmoScript arrow = Instantiate(arrowObject, bowTip.position, Quaternion.identity).GetComponent<AmmoScript>();
            arrow.Setup(mouseVector, mouseAngle, arrowSpeed);
        }

        public void OnHit()
        {
            currentHealth--;
            DamageEffect();
            if (currentHealth <= 0)
            {
                GetComponent<NetworkObject>().Despawn(true);
                Destroy(gameObject);
            }
        }
        private void DamageEffect()
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