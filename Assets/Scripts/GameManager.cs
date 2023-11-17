using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Spawner spawner;
    public float resetTime = 10f; // Time in seconds before the goal resets if not reached

    private float timer;
    private bool isGoalActive;

    void Start()
    {
        // Set the goal to active and start the timer
        spawner = FindObjectOfType<Spawner>();
        isGoalActive = true;
        timer = resetTime;
    }

    void Update()
    {
        if (isGoalActive)
        {
            // Countdown the timer
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                ResetGoal();
            }
        }
    }

    public void HumanReachedGoal()
    {
        isGoalActive = false;
        StopAllHumans();
         StartCoroutine(ResetGoalAfterDelay());
    }

    void StopAllHumans()
    {
        // Find all humans and stop them
        foreach (GameObject human in spawner.GetHumans())
        {
            human.GetComponent<HumanMovement>().StopMoving();
        }
    }

    void ResetGoal()
    {
        if (spawner.currentGoal != null)
        {
            Destroy(spawner.currentGoal);
        }
        
        spawner.SpawnGoal();

        // Reset the timer and set the goal to active
        timer = resetTime;
        isGoalActive = true;

    }

    private IEnumerator ResetGoalAfterDelay()
    {
        ResetGoal(); // Immediately reset the goal
        yield return new WaitForSeconds(0.5f); // Wait for 0.5 seconds
        RestartAllHumans(); // Now let all humans start moving again
    }

    void RestartAllHumans()
    {
        // Find all humans and restart their movement
        foreach (GameObject human in spawner.GetHumans())
        {
            human.GetComponent<HumanMovement>().StartMoving();
        }
    }
}