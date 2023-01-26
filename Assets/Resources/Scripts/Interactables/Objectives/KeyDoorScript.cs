using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyDoorScript : MonoBehaviour
{
    private bool isOpen = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void OpenDoor()
    {
        if (isOpen) return;
        gameObject.SetActive(false);
        Debug.Log("A door has opened Somewhere");
    }
}
