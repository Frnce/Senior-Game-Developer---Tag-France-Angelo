using SDI.Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SDI.Utils
{
    public class AmmoScript : MonoBehaviour
    {
        private Vector2 direction;
        private float angle;
        private float speed;
        private bool isHit;
        [SerializeField]
        private bool hasTempPermanence;
        
        public void Setup(Vector3 direction, float angle, float speed)
        {
            this.direction = direction;
            this.angle = angle;
            this.speed = speed;
        }
        // Start is called before the first frame update
        void Start()
        {
            Quaternion quat = Quaternion.identity;
            quat.eulerAngles = new Vector3(0, 0, angle);
            transform.localRotation = quat;
        }

        // Update is called once per frame
        void Update()
        {
            if (!isHit)
            {
                Move();
            }
        }
        private void Move()
        {
            Vector2 tempPos = transform.position;
            tempPos += direction * speed * Time.deltaTime;
            transform.position = tempPos;
        }
        private void OnTriggerEnter2D(Collider2D collision)
        {
            collision.GetComponentInParent<IDamageable>().OnHit();
        }
    }

}