using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SDI.Players
{
    public class PlayerController : MonoBehaviour
    {
        public Vector2 Movement { get; set; }
        public bool AcceptKey { get; set; }
        public bool CancelKey { get; set; }
        public bool InteractKey { get; set; }
        public Vector3 MousePosition { get; set; }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            Movement = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            AcceptKey = Input.GetButtonDown("Fire1");
            InteractKey = Input.GetButton("Interact");
            MousePosition = Input.mousePosition;
        }
    }
}