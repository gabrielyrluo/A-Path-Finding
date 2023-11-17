using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    public GameObject plane; 

    public int gridSizeX = 20;
    public int gridSizeY = 20;

    private Node[,] grid;
    private float nodeWidth;
    private float nodeHeight;
    private Vector3 gridWorldSize;

    void Start()
    {
        
        CalculateGridSize();
        CreateGrid();
    }

    void Update()
    {
        UpdateGridForObstacles();
    }

    void UpdateGridForObstacles()
    {
        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Node node = grid[x, y];
                Vector3 worldPoint = node.worldPosition;
                bool walkable = !(Physics.CheckSphere(worldPoint, nodeWidth / 2, LayerMask.GetMask("Obstacle")));
                grid[x, y].walkable = walkable;
            }
        }
    }
    public Bounds GetPlaneBounds()
    {
        MeshRenderer renderer = plane.GetComponent<MeshRenderer>();
        return renderer.bounds;
    }

    void CalculateGridSize()
    {
        MeshRenderer planeMeshRenderer = plane.GetComponent<MeshRenderer>();
        gridWorldSize = planeMeshRenderer.bounds.size;
        nodeWidth = gridWorldSize.x / gridSizeX;
        nodeHeight = gridWorldSize.z / gridSizeY;
    }

    void CreateGrid()
    {
        grid = new Node[gridSizeX, gridSizeY];
        Vector3 worldBottomLeft = plane.transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.forward * gridWorldSize.z / 2;

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Vector3 worldPoint = worldBottomLeft 
                                    + Vector3.right * (x * nodeWidth + nodeWidth / 2) 
                                    + Vector3.forward * (y * nodeHeight + nodeHeight / 2);
                grid[x, y] = new Node(true, worldPoint, x, y);
            }
        }
    }

    // Function to draw the grid using Gizmos
    void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, 1, gridWorldSize.z));
        if (grid != null)
        {
            foreach (Node n in grid)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawCube(n.worldPosition, new Vector3(nodeWidth, 0.5f, nodeHeight));
            }
        }
    }

    public Node NodeFromWorldPoint(Vector3 worldPosition)
    {
        int x = Mathf.RoundToInt((worldPosition.x + gridWorldSize.x / 2) / nodeWidth);
        int y = Mathf.RoundToInt((worldPosition.z + gridWorldSize.z / 2) / nodeHeight);
        x = Mathf.Clamp(x, 0, gridSizeX - 1);
        y = Mathf.Clamp(y, 0, gridSizeY - 1);
        return grid[x, y];
    }

    public List<Node> GetNeighbours(Node node)
    {
        List<Node> neighbours = new List<Node>();

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0)
                    continue;

                int checkX = node.gridX + x;
                int checkY = node.gridY + y;

                if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
                {
                    neighbours.Add(grid[checkX, checkY]);
                }
            }
        }

        return neighbours;
    }

    public List<Node> FindPath(Node startNode, Node targetNode)
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

            foreach (Node neighbour in GetNeighbours(currentNode))
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

        return new List<Node>(); 
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
}