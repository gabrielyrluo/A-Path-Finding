using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Linq;

public class ChairMovement : MonoBehaviour
{
    public float speed = 1f; // Slower than human speed
    private Grid grid;
    private Bounds planeBounds;
    private Transform targetHuman; // The human this chair is trying to block

    void Start()
    {
        grid = FindObjectOfType<Grid>(); 
        planeBounds = grid.GetPlaneBounds();
    }

    void Update()
    {
        
        FindNearestHuman();
        MoveToBlockHuman();
        
    }

    void FindNearestHuman()
    {
        // Find the nearest human to this chair
        float closestDistanceSqr = Mathf.Infinity;
        GameObject[] humans = GameObject.FindGameObjectsWithTag("Human");

        foreach (GameObject human in humans)
        {
            Vector3 directionToHuman = human.transform.position - transform.position;
            float dSqrToHuman = directionToHuman.sqrMagnitude;
            if (dSqrToHuman < closestDistanceSqr)
            {
                closestDistanceSqr = dSqrToHuman;
                targetHuman = human.transform;
            }
        }
    }

    void MoveToBlockHuman()
    {
        // Determine the node on the human's path that we want to move to
        Node humanNode = grid.NodeFromWorldPoint(targetHuman.position);
        List<Node> neighbours = grid.GetNeighbours(humanNode);

        Node targetNode = null;
        float closestDistanceSqr = Mathf.Infinity;
        foreach (Node node in neighbours)
        {
            if (!node.walkable || !planeBounds.Contains(node.worldPosition)) continue;

            float distanceSqr = (node.worldPosition - transform.position).sqrMagnitude;
            if (distanceSqr < closestDistanceSqr)
            {
                closestDistanceSqr = distanceSqr;
                targetNode = node;
            }
        }

        if (targetNode != null)
        {
            // Move towards the selected node
            Vector3 dir = (targetNode.worldPosition - transform.position).normalized;
            transform.Translate(dir * speed * Time.deltaTime, Space.World);
        }
    }
}