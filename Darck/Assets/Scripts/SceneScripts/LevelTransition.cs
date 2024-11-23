using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelTransition : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public string nextSceneName; // Public variable to set in the Inspector

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) // Check for the Player tag
        {
            Debug.Log("Checkpoint activated!");
            SceneManager.LoadScene(int.Parse(nextSceneName)); // Load the specified scene
            AudioManager.instance.lvlbgmsc.Play();
        }
    }
}
