using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal : MonoBehaviour
{
    public GameManager gameManager;

    void OnTriggerEnter(Collider other)
    {
        gameManager = FindObjectOfType<GameManager>();
        if (other.CompareTag("Human"))
        {
            // A human has reached the goal
            gameManager.HumanReachedGoal();
        }
    }
}
