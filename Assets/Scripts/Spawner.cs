using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject plane;
    public GameObject chairPrefab;
    public GameObject humanPrefab;
    public GameObject goalPrefab; // Updated to goalPrefab
    public int numberOfChairs = 10;
    public int numberOfHumans = 5;

    public GameObject currentGoal; // To keep track of the current goal
    public List<GameObject> humans;

    void Start()
    {
        Bounds spawnBounds = GetPlaneBounds(plane);

        for (int i = 0; i < numberOfChairs; i++)
        {
            SpawnObject(chairPrefab, spawnBounds, chairPrefab.transform.localScale);
        }

        humans = new List<GameObject>();
        for (int i = 0; i < numberOfHumans; i++)
        {
            GameObject human = SpawnObject(humanPrefab, spawnBounds, humanPrefab.transform.localScale);
            humans.Add(human);
        }

        currentGoal = SpawnObject(goalPrefab, spawnBounds, goalPrefab.transform.localScale);
        
    }

      GameObject SpawnObject(GameObject prefab, Bounds bounds, Vector3 size)
    {
        bool placed = false;
        while (!placed)
        {
            Vector3 position = GetRandomPositionWithinBounds(bounds, size);
            if (prefab == chairPrefab)
            {
                // Use a box check for the chairs
                placed = !Physics.CheckBox(position, size * 0.5f);
            }
            else if (prefab == humanPrefab)
            {
                // Use a sphere check for the humans
                placed = !Physics.CheckSphere(position, size.y * 0.5f);
            }

            else // For goalPrefab
            {
                // Increase the check radius to ensure there's space around the goal
                float checkRadius = Mathf.Max(size.x, size.y, size.z);
                placed = !Physics.CheckSphere(position, checkRadius, LayerMask.GetMask("Obstacle"));

                //add more space around the goal by a certain factor
                placed = placed && !Physics.CheckSphere(position, checkRadius * 1.5f, LayerMask.GetMask("Obstacle"));
            }

            if (placed)
            {
                return Instantiate(prefab, position, Quaternion.identity);
            }
        }
        return null;
    }

    Bounds GetPlaneBounds(GameObject planeObject)
    {
        MeshRenderer renderer = planeObject.GetComponent<MeshRenderer>();
        return renderer.bounds;
    }

    Vector3 GetRandomPositionWithinBounds(Bounds bounds, Vector3 size)
    {
        // Calculate a margin to prevent spawning at the very edges
        float marginX = size.x / 2;
        float marginZ = size.z / 2;

        // Shrink the bounds by the margin
        float minX = bounds.min.x + marginX;
        float maxX = bounds.max.x - marginX;
        float minZ = bounds.min.z + marginZ;
        float maxZ = bounds.max.z - marginZ;
        
        return new Vector3(
            Random.Range(minX, maxX),
            bounds.min.y + size.y / 2, 
            Random.Range(minZ, maxZ)
        );
    }

    public void SpawnGoal()
    {
        // Spawns the goal at a random location and assigns it to currentGoal
        currentGoal = SpawnObject(goalPrefab, GetPlaneBounds(plane), goalPrefab.transform.localScale);
    }

    public List<GameObject> GetHumans()
    {
        // Returns a list of all human GameObjects in the scene
        return humans;
    }
}