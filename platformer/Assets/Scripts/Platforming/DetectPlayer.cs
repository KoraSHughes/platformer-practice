using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectPlayer : MonoBehaviour
{
    GameObject player;
    public float xthreshold = 0.7f;
    public float ythreshold = 2.7f;

    void Start(){
        player = GameObject.FindWithTag("Player");
    }

    public int wallToPlayer(){
        float xdisplacement = transform.position.x - player.transform.position.x;
        float ydisplacement = transform.position.y - player.transform.position.y;
        if ((ydisplacement < 0 && ydisplacement > -ythreshold) || (ydisplacement > 0 && ydisplacement < ythreshold)){
            // only if we are within this wall's vertical range
            if (xdisplacement > 0 && xdisplacement < xthreshold){
                return 1;
            }
            else if (xdisplacement < 0 && xdisplacement > -xthreshold){
                return -1;
            }
        }
        return 0;
    }
}
