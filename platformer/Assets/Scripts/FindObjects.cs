using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FindObjects : MonoBehaviour
{
    List<string> nearObjects = new List<string>();
    
    void Update(){
        Debug.Log("Objects: " + string.Join(", ", nearObjects));
    }

    //called when something enters the trigger
    void OnTriggerEnter2D(Collider2D col)
    {
        nearObjects.Add(col.gameObject.tag);
    }
    
    //called when something exits the trigger
    void OnTriggerExit2D(Collider2D col)
    {
        if (nearObjects.Contains(col.gameObject.tag))
        {
            nearObjects.Remove(col.gameObject.tag);
        }
    }

    public List<string> getObjs(){
        return nearObjects;
    }
}
