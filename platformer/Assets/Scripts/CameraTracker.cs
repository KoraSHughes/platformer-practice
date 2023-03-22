using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTracker : MonoBehaviour
{
    GameObject player;
    Vector3 offset;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        if (player) {
            offset = transform.position - player.transform.position;
        }
    }

    private void LateUpdate() {
        if (player) {
            transform.position = player.transform.position + offset;
        }
    }

}
