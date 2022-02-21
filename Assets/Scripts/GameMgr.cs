using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMgr : MonoBehaviour
{
    public static GameMgr instance;

    public GameObject nodePrefab;
    public Player player1;
    public Player player2;
    public Player activePlayer;

    public GameObject ball;
    public Node ballNode;

    private List<Node> nodes;
    private static int nodeCount;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        } else
        {
            Destroy(this);
        }

        // Board creation stuff
        CreateNodes();
        FindNeighbors();
    }

    private void Start()
    {
        StartRound();
    }

    private void CreateNode(Vector3 pos)
    {
        GameObject newNode = Instantiate(nodePrefab);
        newNode.transform.position = pos;
        Node node = newNode.GetComponent<Node>();
        nodes.Add(node);
        node.name = "Node" + nodeCount.ToString();
        nodeCount++;
    }

    private void CreateNodes()
    {
        Vector3 startPos = new Vector3(-8, 0, -10);
        float stepSize = 2;

        int length = 11;
        int width = 9;

        nodes = new List<Node>();
        nodeCount = 0;
        for (int z = 0; z < length; z++)
        {
            for (int x = 0; x < width; x++)
            {
                CreateNode(new Vector3(startPos.x + x * stepSize, 0.01f, startPos.z + z * stepSize));
            }
        }
        CreateNode(new Vector3(startPos.x + ((int)(width / 2)) * stepSize, 0.01f, startPos.z + (-1 * stepSize)));
        CreateNode(new Vector3(startPos.x + ((int)(width / 2)) * stepSize, 0.01f, startPos.z + (length * stepSize)));

    }

    private void FindNeighbors()
    {
        foreach (Node node in nodes)
        {
            foreach (Node otherNode in nodes)
            {
                if (otherNode != node)
                {
                    if (Mathf.Abs(node.transform.position.x - otherNode.transform.position.x) < 2.5f)
                    {
                        if (Mathf.Abs(node.transform.position.z - otherNode.transform.position.z) < 2.5f)
                        {
                            node.neighbors.Add(otherNode);
                        }
                    }
                }
            }
        }
    }

    public void SetBallNode(Node node)
    {
        ballNode = node;
        ball.transform.position = node.transform.position;
    }

    private void StartRound()
    {
        SetBallNode(nodes[49]);
        int startPlayer = Random.Range(0, 2);
        if (startPlayer == 0)
        {
            activePlayer = player1;
        } else
        {
            activePlayer = player2;
        }
        ShowOptions();
    }

    public void ShowOptions()
    {
        foreach (Node neighbor in ballNode.neighbors)
        {
            neighbor.SetSelector(true);
        }
    }

    public void SubmitMoves()
    {

    }

    private void Update()
    {
        
    }
}
