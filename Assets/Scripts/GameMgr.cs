using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
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

    // Events
    public UnityEvent GameOver;
    public UnityEvent PlayersChanged;
    public UnityEvent BallMoved;

    private GameObject nodeContainer;

    private Board board;

    private const int boardWidth = 9;
    private const int boardLength = 11;

    private Node[,] nodes;

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

        board = new Board(boardWidth, boardLength);
        CreateNodes(boardWidth, boardLength);
        RandomizePlayer();
        ball.transform.position = nodes[board.curCordinate.i, board.curCordinate.j].transform.position;

/*        nodesBin = new uint[boardWidth, boardLength];
        print(BitCount(N));

        // Board creation stuff
        ResetAllNodeOptions();*/
    }

    private void Start()
    {

    }

    private void RandomizePlayer()
    {
        int startPlayer = Random.Range(0, 2);
        if (startPlayer == 0)
        {
            activePlayer = player1;
        }
        else
        {
            activePlayer = player2;
        }
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

    public void Move(Directions direction)
    {
        if (!board.gameOver)
        {
            if (board.IsValidMove(direction))
            {
                HideOptions();
                bool gameOverReached = board.MakeMove(activePlayer.id, direction);
                LineMgr.instance.CreateLine(
                    nodes[board.prevCordinate.i, board.prevCordinate.j].transform.position,
                    nodes[board.curCordinate.i, board.curCordinate.j].transform.position,
                    activePlayer.material);
                ball.transform.position = nodes[board.curCordinate.i, board.curCordinate.j].transform.position;

                if (gameOverReached)
                {
                    GameOver.Invoke();
                } else
                {
                    if (board.AtOpenNode())
                    {
                        SwitchPlayers();
                    }
                    ShowOptions();
                }

            }
        }

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
        PlayersChanged.Invoke();
    }

    private void ResetBoard()
    {
        // Resets all the data necessary to restart a new round
        //ResetAllNodeOptions();

        // Moves the ball to the center
/*        int i = (int)(boardWidth / 2);
        int j = (int)(boardLength / 2);
        MoveToNode(i,j);*/
    }

    private void HideOptions()
    {
        foreach (Coordinate cord in board.GetOptions(board.curCordinate))
        {
            nodes[cord.i, cord.j].highlight.SetState(false);
            nodes[cord.i, cord.j].interactable.SetState(false);
        }
    }

    private void ShowOptions()
    {
        foreach (Coordinate cord in board.GetOptions(board.curCordinate))
        {
            nodes[cord.i, cord.j].highlight.SetState(true);
            nodes[cord.i, cord.j].interactable.SetState(true);
        }
        /*        foreach (Node neighbor in ballNode.options)
                {
                    neighbor.highlight.SetState(true);
                    neighbor.interactable.SetState(true);
                }*/
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Keypad8))
        {
            Move(Directions.N);
        }
        if (Input.GetKeyDown(KeyCode.Keypad2))
        {
            Move(Directions.S);
        }
        if (Input.GetKeyDown(KeyCode.Keypad6))
        {
            Move(Directions.E);
        }
        if (Input.GetKeyDown(KeyCode.Keypad4))
        {
            Move(Directions.W);
        }
        if (Input.GetKeyDown(KeyCode.Keypad7))
        {
            Move(Directions.NW);
        }
        if (Input.GetKeyDown(KeyCode.Keypad9))
        {
            Move(Directions.NE);
        }
        if (Input.GetKeyDown(KeyCode.Keypad1))
        {
            Move(Directions.SW);
        }
        if (Input.GetKeyDown(KeyCode.Keypad3))
        {
            Move(Directions.SE);
        }
    }
}
