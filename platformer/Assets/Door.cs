using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Door : MonoBehaviour
{

    int ranInt;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other) {
        // if player collides with door, load next room?
        if (other.CompareTag("Player")) {
            Debug.Log("collision");
            // ranInt = SceneManager.GetActiveScene().buildIndex + 1;
            // SceneManager.LoadScene(ranInt);
        }
    }


}
