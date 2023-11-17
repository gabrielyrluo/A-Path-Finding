using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanMovement : MonoBehaviour
{
    public Transform goal;
    public float speed = 3f;
    private Grid grid;
    private List<Node> path = new List<Node>();
    private int targetIndex;
    private bool isMoving;
    private Bounds planeBounds;

    void Start()
    {
        grid = FindObjectOfType<Grid>(); 
        planeBounds = grid.GetPlaneBounds();
    }

    void Update()
    {
        if (path.Count > 0 && isMoving)
        {
            if (!planeBounds.Contains(path[targetIndex].worldPosition))
            {
                StopMoving();
                MoveToGoal();
            }
            else {
            Vector3 dir = path[targetIndex].worldPosition - transform.position;
            transform.Translate(dir.normalized * speed * Time.deltaTime, Space.World);

            if (Vector3.Distance(transform.position, path[targetIndex].worldPosition) < 0.4f)
            {
                targetIndex++;
            }
            }
        }
        goal = GameObject.FindWithTag("Finish").transform;
        MoveToGoal();

    }

    public void MoveToGoal()
    {
        if (!isMoving) return; // Do not calculate path if not moving
        Node startNode = grid.NodeFromWorldPoint(transform.position);
        Node targetNode = grid.NodeFromWorldPoint(goal.position);
        path = FindPath(startNode, targetNode);
        targetIndex = 0;
    }

    public void StopMoving()
    {
        isMoving = false;
        path.Clear();
    }

    public void StartMoving()
    {
        isMoving = true;
    }

    List<Node> FindPath(Node startNode, Node targetNode)
    {
        List<Node> openSet = new List<Node>();
        HashSet<Node> closedSet = new HashSet<Node>();
        openSet.Add(startNode);

        while (openSet.Count > 0)
        {
            Node currentNode = openSet[0];
            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].fCost < currentNode.fCost || openSet[i].fCost == currentNode.fCost && openSet[i].hCost < currentNode.hCost)
                {
                    currentNode = openSet[i];
                }
            }

            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            if (currentNode == targetNode)
            {
                return RetracePath(startNode, targetNode);
            }

            foreach (Node neighbour in grid.GetNeighbours(currentNode))
            {
                if (!neighbour.walkable || closedSet.Contains(neighbour))
                {
                    continue;
                }

                int newCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);
                if (newCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                {
                    neighbour.gCost = newCostToNeighbour;
                    neighbour.hCost = GetDistance(neighbour, targetNode);
                    neighbour.parent = currentNode;

                    if (!openSet.Contains(neighbour))
                        openSet.Add(neighbour);
                }
            }
        }

        return new List<Node>(); // Return an empty path if no path is found
    }

    List<Node> RetracePath(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }
        path.Reverse();

        return path;
    }

    int GetDistance(Node nodeA, Node nodeB)
    {
        int dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int dstY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

        if (dstX > dstY)
            return 14 * dstY + 10 * (dstX - dstY);
        return 14 * dstX + 10 * (dstY - dstX);
    }

    public List<Node> GetCurrentPath()
    {
        return new List<Node>(path); // Returns a copy of the current path
    }
}