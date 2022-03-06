using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;

public class GameMgr : MonoBehaviour
{
    public static GameMgr instance;

    public GameObject nodePrefab;
    public List<Player> players;

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

    private Node[] goalNodes;

    private static int nodeCount;
    private MinMax minMax;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        } else
        {
            Destroy(this);
        }
        minMax = new MinMax();

        goalNodes = new Node[2];
        board = new Board(boardWidth, boardLength);
        CreateNodes(boardWidth, boardLength);
        RandomizePlayer();
        ball.transform.position = nodes[board.curCordinate.i, board.curCordinate.j].transform.position;
        ShowOptions();
    }

    private void Start()
    {

    }

    private void RandomizePlayer()
    {
        uint startPlayer = (uint) Random.Range(0, 2);
        board.activePlayer = startPlayer;
        if (startPlayer == 0)
        {
            board.nonActivePlayer = 1;
        } else
        {
            board.nonActivePlayer = 0;
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
        goalNodes[1] = CreateNode(new Vector3(startPos.x + ((int)(dimX / 2)) * spacing, 0.01f, startPos.z + (dimZ * spacing)));
        goalNodes[0] = CreateNode(new Vector3(startPos.x + ((int)(dimX / 2)) * spacing, 0.01f, startPos.z + (-1 * spacing)));
    }

    public void Move(Directions direction)
    {
        if (!board.gameOver)
        {
            if (board.IsValidMove(direction))
            {
                HideOptions();
                Player player = players[(int) board.activePlayer];
                int gameOverReached = board.MakeMove(direction);
                player.IncrementMoveCounter();

                Vector3 startPos = nodes[board.prevCordinate.i, board.prevCordinate.j].transform.position;
                Material prevPlayer = player.material;
                if (gameOverReached == -1)
                {
                    // If player makes a move and game is continuing
                    Vector3 endPos = nodes[board.curCordinate.i, board.curCordinate.j].transform.position;
                    LineMgr.instance.CreateLine(startPos, endPos, prevPlayer);
                    ball.transform.position = endPos;
                    if (!board.AtOpenNode()) {
                        // Player bounced the ball their turn continues
                        player.IncrementBounceCounter();
                    }
                    ShowOptions();
                }
                else if (gameOverReached == 2)
                {
                    // No winner but game is over
                    Vector3 endPos = nodes[board.curCordinate.i, board.curCordinate.j].transform.position;
                    LineMgr.instance.CreateLine(startPos, endPos, prevPlayer);
                    ball.transform.position = endPos;
                    player.placing = false;
                }
                else
                {
                    print(gameOverReached);
                    players[gameOverReached].IncrementWinCounter();
                    Vector3 endPos = goalNodes[gameOverReached].transform.position;
                    LineMgr.instance.CreateLine(startPos, endPos, prevPlayer);
                    ball.transform.position = endPos;
                    GameOver.Invoke();
                }

                // Evaluate the board
                players[0].EvaluationUpdate(board.StaticEvaluator(players[0].id));
                players[1].EvaluationUpdate(board.StaticEvaluator(players[1].id));

                if (player.id != board.activePlayer)
                {
                    player.placing = false;
                    PlayersChanged.Invoke();
                    players[(int) board.activePlayer].placing = true;
                }
            }
        }

    }

    IEnumerator RollBall()
    {

        yield return null;
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
        foreach (KeyValuePair<Directions, Coordinate> cord in board.GetOptions(board.curCordinate))
        {
            //nodes[cord.Value.i, cord.Value.j].highlight.SetState(false);
            //nodes[cord.Value.i, cord.Value.j].interactable.SetState(false);
        }
        DirectionIndicator.instance.HideAll();
    }

    private void ShowOptions()
    {
        foreach (KeyValuePair<Directions, Coordinate> cord in board.GetOptions(board.curCordinate))
        {
            //nodes[cord.Value.i, cord.Value.j].highlight.SetState(true);
            //nodes[cord.Value.i, cord.Value.j].interactable.SetState(true);
            DirectionIndicator.instance.ShowDirection(cord.Key);
        }
        DirectionIndicator.instance.transform.position = ball.transform.position;
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
        if(players[(int) board.activePlayer].isAI)
        {
            (float, Directions) action = minMax.Solve(board, 3, 5, board.activePlayer);
            print(action);
            Move(action.Item2);
        }
    }
}
