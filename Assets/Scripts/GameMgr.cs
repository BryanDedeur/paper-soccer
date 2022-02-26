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

    private GameObject nodeContainer;

    private const int boardWidth = 9;
    private const int boardLength = 11;

    private List<List<Node>> nodes;
    private Node goal1Node;
    private Node goal2Node;

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
        CreateNodes(boardWidth,boardLength);
        ResetAllNodeOptions();
    }

    private void Start()
    {
        ResetBoard();
        StartRound();
    }

    private Node CreateNode(Vector3 pos)
    {
        // Creates a node game object with a node component
        GameObject newNode = Instantiate(nodePrefab);
        newNode.transform.position = pos;
        newNode.transform.parent = nodeContainer.transform;
        Node node = newNode.GetComponent<Node>();
        node.name = "Node" + nodeCount.ToString();
        nodeCount++;
        return node;
    }

    private void CreateNodes(int dimX, int dimZ, float spacing = 2)
    {
        // Creates the 2d grid of nodes
        Vector3 startPos = new Vector3(-8, 0, -10);
        nodeContainer = new GameObject();
        nodeContainer.name = "Nodes";

        nodes = new List<List<Node>>();
        for (int z = 0; z < dimZ; z++)
        {
            nodes.Add(new List<Node>());
            for (int x = 0; x < dimX; x++)
            {
                Node node = CreateNode(new Vector3(startPos.x + x * spacing, 0.01f, startPos.z + z * spacing));
                nodes[z].Add(node);
            }
        }

        // Creates extra nodes for the goals
        goal1Node = CreateNode(new Vector3(startPos.x + ((int)(dimX / 2)) * spacing, 0.01f, startPos.z + (-1 * spacing)));

        goal2Node = CreateNode(new Vector3(startPos.x + ((int)(dimX / 2)) * spacing, 0.01f, startPos.z + (dimZ * spacing)));
    }

    public void MoveBallToNode(Node node)
    {
        // Remove the possibility of doing the same move
        if (ballNode != null)
        {
            ballNode.options.Remove(node);
            LineMgr.instance.CreateLine(node.transform.position, ballNode.transform.position);
        }
        node.options.Remove(ballNode);

        ballNode = node;
        ball.transform.position = node.transform.position;
        HighlightMgr.instance.DeselectAll();
        ShowOptions();
    }

    private void ResetBoard()
    {
        // Resets all the data necessary to restart a new round
        ResetAllNodeOptions();

        // Moves the ball to the center
        Node middleNode = nodes[Mathf.RoundToInt(boardLength / 2)][Mathf.RoundToInt(boardWidth / 2)];
        MoveBallToNode(middleNode);
    }

    private void StartRound()
    {
        int startPlayer = Random.Range(0, 2);
        if (startPlayer == 0)
        {
            activePlayer = player1;
        } else
        {
            activePlayer = player2;
        }
    }

    private void ShowOptions()
    {
        foreach (Node neighbor in ballNode.options)
        {
            neighbor.highlight.SetState(true);
        }
    }

    private void Update()
    {
        
    }

    private void ResetAllNodeOptions()
    {
        /*
        This will update every nodes list of neighbors for quick access later. 

            N.W   N   N.E
              \   |   /
               \  |  /
            W----Cell----E
                / | \
              /   |  \
            S.W   S   S.E

        Cell-->Current Cell (i, j)
        N -->  North       (i-1, j)
        S -->  South       (i+1, j)
        E -->  East        (i, j+1)
        W -->  West           (i, j-1)
        A--> North-East  (i-1, j+1)
        B--> North-West  (i-1, j-1)
        C--> South-East  (i+1, j+1)
        D--> South-West  (i+1, j-1)*/
        for (int i = 0; i < nodes.Count; i++)
        {
            for (int j = 0; j < nodes[i].Count; j++)
            {
                nodes[i][j].options.Clear();
                if (i > 0 && j > 0 && i < nodes.Count - 1 && j < nodes[i].Count - 1)
                {
                    // Not at a boundary we can add all successors
                    nodes[i][j].options.Add(nodes[i - 1][j]);
                    nodes[i][j].options.Add(nodes[i + 1][j]);
                    nodes[i][j].options.Add(nodes[i][j - 1]);
                    nodes[i][j].options.Add(nodes[i][j + 1]);
                    nodes[i][j].options.Add(nodes[i - 1][j - 1]);
                    nodes[i][j].options.Add(nodes[i + 1][j - 1]);
                    nodes[i][j].options.Add(nodes[i - 1][j + 1]);
                    nodes[i][j].options.Add(nodes[i + 1][j + 1]);
                }
                else if (i == 0 && j == 0)
                {
                    //nodes[i][j].options.Add(nodes[i + 1][j]);
                    //nodes[i][j].options.Add(nodes[i][j + 1]);
                    nodes[i][j].options.Add(nodes[i + 1][j + 1]);
                }
                else if (i == 0 && j == nodes[i].Count - 1)
                {
                    //nodes[i][j].options.Add(nodes[i + 1][j]);
                    //nodes[i][j].options.Add(nodes[i][j - 1]);
                    nodes[i][j].options.Add(nodes[i + 1][j - 1]);
                }
                else if (i == nodes.Count - 1 && j == 0)
                {
                    //nodes[i][j].options.Add(nodes[i - 1][j]);
                    //nodes[i][j].options.Add(nodes[i][j + 1]);
                    nodes[i][j].options.Add(nodes[i - 1][j + 1]);
                }
                else if (i == nodes.Count - 1 && j == nodes[i].Count - 1)
                {
                    //nodes[i][j].options.Add(nodes[i - 1][j]);
                    //nodes[i][j].options.Add(nodes[i][j - 1]);
                    nodes[i][j].options.Add(nodes[i - 1][j - 1]);
                }
                else if (i == 0)
                {
                    // no i -
                    nodes[i][j].options.Add(nodes[i + 1][j]);
                    //nodes[i][j].options.Add(nodes[i][j - 1]);
                    //nodes[i][j].options.Add(nodes[i][j + 1]);
                    nodes[i][j].options.Add(nodes[i + 1][j - 1]);
                    nodes[i][j].options.Add(nodes[i + 1][j + 1]);
                }
                else if (i == nodes.Count - 1)
                {
                    // no i +
                    nodes[i][j].options.Add(nodes[i - 1][j]);
                    //nodes[i][j].options.Add(nodes[i][j - 1]);
                    //nodes[i][j].options.Add(nodes[i][j + 1]);
                    nodes[i][j].options.Add(nodes[i - 1][j - 1]);
                    nodes[i][j].options.Add(nodes[i - 1][j + 1]);
                }
                else if (j == 0)
                {
                    // no j -
                    //nodes[i][j].options.Add(nodes[i - 1][j]);
                    //nodes[i][j].options.Add(nodes[i + 1][j]);
                    nodes[i][j].options.Add(nodes[i][j + 1]);
                    nodes[i][j].options.Add(nodes[i - 1][j + 1]);
                    nodes[i][j].options.Add(nodes[i + 1][j + 1]);
                }
                else if (j == nodes[i].Count - 1)
                {
                    // no j +
                    //nodes[i][j].options.Add(nodes[i - 1][j]);
                    //nodes[i][j].options.Add(nodes[i + 1][j]);
                    nodes[i][j].options.Add(nodes[i][j - 1]);
                    nodes[i][j].options.Add(nodes[i - 1][j - 1]);
                    nodes[i][j].options.Add(nodes[i + 1][j - 1]);
                }
            }
        }

        // Add special options around goals
        // Goal 1
        nodes[0][(int)(nodes[0].Count / 2f) + 1].options.Add(goal1Node);
        nodes[0][(int)(nodes[0].Count / 2f)].options.Add(goal1Node);
        nodes[0][(int)(nodes[0].Count / 2f) - 1].options.Add(goal1Node);
        // Add side nodes
        nodes[0][(int)(nodes[0].Count / 2f) - 1].options.Add(nodes[0][(int)(nodes[0].Count / 2f)]);
        nodes[0][(int)(nodes[0].Count / 2f)].options.Add(nodes[0][(int)(nodes[0].Count / 2f) + 1]);
        nodes[0][(int)(nodes[0].Count / 2f)].options.Add(nodes[0][(int)(nodes[0].Count / 2f) - 1]);
        nodes[0][(int)(nodes[0].Count / 2f) + 1].options.Add(nodes[0][(int)(nodes[0].Count / 2f)]);

        // Goal 2
        nodes[nodes.Count - 1][(int)(nodes[0].Count / 2f) - 1].options.Add(goal2Node);
        nodes[nodes.Count - 1][(int)(nodes[0].Count / 2f)].options.Add(goal2Node);
        nodes[nodes.Count - 1][(int)(nodes[0].Count / 2f) + 1].options.Add(goal2Node);
        // Add side nodes
        nodes[nodes.Count - 1][(int)(nodes[0].Count / 2f) - 1].options.Add(nodes[nodes.Count - 1][(int)(nodes[0].Count / 2f)]);
        nodes[nodes.Count - 1][(int)(nodes[0].Count / 2f)].options.Add(nodes[nodes.Count - 1][(int)(nodes[0].Count / 2f) + 1]);
        nodes[nodes.Count - 1][(int)(nodes[0].Count / 2f)].options.Add(nodes[nodes.Count - 1][(int)(nodes[0].Count / 2f) - 1]);
        nodes[nodes.Count - 1][(int)(nodes[0].Count / 2f) + 1].options.Add(nodes[nodes.Count - 1][(int)(nodes[0].Count / 2f)]);
    }
}
