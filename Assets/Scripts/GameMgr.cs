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

    public List<Vector2Int> moves;

    private GameObject nodeContainer;

    private const int boardWidth = 9;
    private const int boardLength = 11;

    private Node[,] nodes;
    private uint[,] nodesBin;

    // binary node uint node = 0b_0000_0000 NSEWABCD

    const uint N = 0b_0000_0001;
    const uint S = 0b_0000_0010;
    const uint E = 0b_0000_0100;
    const uint W = 0b_0000_1000;
    const uint A = 0b_0001_0000;
    const uint B = 0b_0010_0000;
    const uint C = 0b_0100_0000;
    const uint D = 0b_1000_0000;

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

        nodesBin = new uint[boardWidth, boardLength];
        print(BitCount(N));

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

        nodes = new Node[dimX, dimZ];
        for (int x = 0; x < dimX; x++)
        {
            for (int z = 0; z < dimZ; z++)
            {
                nodes[x,z] = CreateNode(new Vector3(startPos.x + x * spacing, 0.01f, startPos.z + z * spacing));
            }
        }

        // Creates extra nodes for the goals
        goal1Node = CreateNode(new Vector3(startPos.x + ((int)(dimX / 2)) * spacing, 0.01f, startPos.z + (-1 * spacing)));
        goal2Node = CreateNode(new Vector3(startPos.x + ((int)(dimX / 2)) * spacing, 0.01f, startPos.z + (dimZ * spacing)));
    }
    int BitCount(uint n)
    {
        int count = 0;
        while(n > 0) {
            count++;
            n = n & (n - 1);
        }
        return count;
    }

    public void MoveToNode(int i, int j)
    {
        Vector2Int currentPos = moves[moves.Count - 1];

        // Find the direction
        if (i == currentPos.x && j > currentPos.y)
        {

        } else if (i == currentPos.x && j < currentPos.y)
        {

        }
        else if (i == currentPos.x && j > currentPos.y)
        {

        }
        else if ()
        {

        }
        else if ()
        {

        }
        nodesBin[currentPos.x, currentPos.y] = nodesBin[currentPos.x, currentPos.y] | 


        moves.Add(new Vector2Int(i, j));

/*
        Node nextNode = nodes[i,j];

        // If we are not on the first move, modify the current node
        if (moves.Count > 0)
        {
            //ballNode.options.Remove(node);

            //Vector2 ballNode = moves[moves.Count - 1];
            //nodesBin[ballNode.x, ballNode.y] = 

            //LineMgr.instance.CreateLine(node.transform.position, ballNode.transform.position, activePlayer.material);
            // Hide the options
            while (InteractableMgr.instance.active.Count > 0)
            {
                InteractableMgr.instance.active[0].SetState(false);
            }
        }

        // If the new node does not have any bounce options then we swap players
        if (BitCount(nodesBin[i,j]) == 8)
        {
            SwitchPlayers();
        }

        //node.options.Remove(ballNode);
        //ballNode = node;
        ball.transform.position = nextNode.transform.position;
        HighlightMgr.instance.DeselectAll();
        ShowOptions();*/
    }

    private void SwitchPlayers()
    {
        if (activePlayer == player2)
        {
            activePlayer = player1;
        }
        else
        {
            activePlayer = player2;
        }
    }

    private void ResetBoard()
    {
        // Resets all the data necessary to restart a new round
        ResetAllNodeOptions();

        // Moves the ball to the center
        int i = (int)(boardWidth / 2);
        int j = (int)(boardLength / 2);
        MoveToNode(i,j);
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
/*        foreach (Node neighbor in ballNode.options)
        {
            neighbor.highlight.SetState(true);
            neighbor.interactable.SetState(true);
        }*/
    }

    private void Update()
    {
        
    }
}
