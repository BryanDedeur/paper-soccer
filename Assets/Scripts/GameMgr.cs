using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;

public class GameMgr : MonoBehaviour
{
    public static GameMgr instance;
    public Material lineMaterial;

    public GameObject nodePrefab;
    public List<Player> players;

    public GameObject ball;

    // Events
    public StringEvent GameOver;
    public UnityEvent PlayersChanged;
    public UnityEvent BallMoved;

    private GameObject nodeContainer;

    private Board board;

    private const int boardWidth = 9;
    private const int boardLength = 11;

    private Node[,] nodes;
    private Node[] goalNodes;

    private List<Node> activeNodes;

    private static int nodeCount;
    private MinMax minMax;

    private Queue<IEnumerator> moveQueue = new Queue<IEnumerator>();

    private bool actionsEnabled = true;

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
        activeNodes = new List<Node>();
        goalNodes = new Node[2];
        board = new Board(boardWidth, boardLength);
        CreateNodes(boardWidth, boardLength);
        DrawBoardLines();
        RandomizePlayer();
        ball.transform.position = nodes[board.curCordinate.i, board.curCordinate.j].transform.position;
        ShowOptions();
    }

    private void Start()
    {
        StartCoroutine(RunCoroutineMoveQueue());
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
                if ((z == 0 || z == dimZ - 1) && x != (int)(dimX / 2f))
                {
                    nodes[x, z].gameObject.SetActive(false);
                }            
            }
        }

        // Creates extra nodes for the goals
        goalNodes[0] = nodes[(int)(dimX / 2f), 0];
        goalNodes[1] = nodes[(int)(dimX / 2f), dimZ - 1];
    }

    void DrawBoardLines()
    {
        Vector3 upperLeftCorner = new Vector3(-boardWidth, 0, -boardLength + 2);
        Vector3 upperRightCorner = new Vector3(-boardWidth, 0, boardLength - 2);
        Vector3 lowerLeftCorner = new Vector3(boardWidth, 0, -boardLength + 2);
        Vector3 lowerRightCorner = new Vector3(boardWidth, 0, boardLength - 2);

        LineMgr.instance.CreateLine(upperLeftCorner, upperRightCorner, lineMaterial);
        LineMgr.instance.CreateLine(upperRightCorner, lowerRightCorner, lineMaterial);
        LineMgr.instance.CreateLine(lowerLeftCorner, lowerRightCorner, lineMaterial);
        LineMgr.instance.CreateLine(lowerLeftCorner, upperLeftCorner, lineMaterial);

    }

    public void Move(Direction direction)
    {
        if (!board.gameOver)
        {
            if (board.IsValidMove(direction))
            {
                Player player = players[(int) board.activePlayer];
                int gameOverReached = board.MakeMove(direction);
                player.IncrementMoveCounter();
                Vector3 startPos = nodes[board.prevCordinate.i, board.prevCordinate.j].transform.position;
                Material prevPlayer = player.material;
                if (gameOverReached == -1)
                {
                    // If player makes a move and game is continuing
                    Vector3 endPos = nodes[board.curCordinate.i, board.curCordinate.j].transform.position;
                    moveQueue.Enqueue(RollBall(endPos, player));
                    if (!board.AtOpenNode()) {
                        // Player bounced the ball their turn continues
                        player.IncrementBounceCounter();
                    }
                }
                else if (gameOverReached == 2)
                {
                    // No winner but game is over
                    Vector3 endPos = nodes[board.curCordinate.i, board.curCordinate.j].transform.position;
                    moveQueue.Enqueue(RollBall(endPos, player));
                    GameOver.Invoke("Winner: None");
                    players[(int)board.activePlayer].SetPlacing(false);
                }
                else
                {
                    print(gameOverReached);
                    players[gameOverReached].IncrementWinCounter();
                    Vector3 endPos = goalNodes[gameOverReached].transform.position;
                    moveQueue.Enqueue(RollBall(endPos, player));
                    if (players[(int)board.activePlayer].isAI)
                    {
                        GameOver.Invoke("Winner: Player" + ((int)board.activePlayer + 1).ToString() + " (AI)");
                    } else
                    {
                        GameOver.Invoke("Winner: Player" + ((int)board.activePlayer + 1).ToString() + " (Human)");
                    }
                    players[(int)board.activePlayer].SetPlacing(false) ;
                }

                // Evaluate the board
                players[0].EvaluationUpdate(board.StaticEvaluator(players[0].id));
                players[1].EvaluationUpdate(board.StaticEvaluator(players[1].id));

                if (player.id != board.activePlayer)
                {
                    players[(int)board.activePlayer].SetPlacing(true);
                    players[(int)player.id].SetPlacing(false);

                    PlayersChanged.Invoke();

                }
            }
        }
    }

    public void ResetBoard()
    {
        LineMgr.instance.ClearAll();
        board = new Board(boardWidth, boardLength);
        DrawBoardLines();
        RandomizePlayer();
        ball.transform.position = nodes[board.curCordinate.i, board.curCordinate.j].transform.position;
        ShowOptions();

        players[0].Reset();
        players[1].Reset();
    }

    public void MakeMoves(List<Direction> moves)
    {
        while(moves.Count > 0)
        {
            Move(moves[0]);
            moves.RemoveAt(0);
        }
    }

    IEnumerator RunCoroutineMoveQueue()
    {
        while (true)
        {
            while (moveQueue.Count > 0)
                yield return StartCoroutine(moveQueue.Dequeue());
            yield return null;
        }
    }

    IEnumerator RollBall(Vector3 targetPos, Player who)
    {
        actionsEnabled = false;

        HideOptions();
        Vector3 startPos = ball.transform.position;
        LineMgr.instance.CreateLine(startPos, targetPos, who.material);
        Vector3 diff = (targetPos - ball.transform.position);
        Vector3 dir = diff.normalized;
        float distance = diff.magnitude;
        float scale = 0;
        float speed = 4f;
        while (scale <= 1)
        {
            scale += speed * Time.deltaTime;
            ball.transform.position = startPos + diff * (-Mathf.Cos(scale * Mathf.PI) + 1)/2.0f;
            yield return null;
        }
        ball.transform.position = targetPos;
        if (!players[(int) board.activePlayer].isAI)
        {
            ShowOptions();
        }
        actionsEnabled = true;
    }

    private void HideOptions()
    {
        DirectionIndicator.instance.HideAll();
        foreach(Node node in activeNodes)
        {
            node.interactable.active = false;
        }
        activeNodes.Clear();

    }

    private void ShowOptions()
    {
        foreach (Direction dir in board.GetOptions(board.curCordinate))
        {
            DirectionIndicator.instance.ShowDirection(dir);
            Coordinate cor = board.GetCoordinateInDirection(board.curCordinate, dir);
            nodes[cor.i, cor.j].interactable.active = true;
            nodes[cor.i, cor.j].interactable.associatedDirection = dir;
            activeNodes.Add(nodes[cor.i, cor.j]);
            
        }
        DirectionIndicator.instance.transform.position = ball.transform.position;
    }

    private void ShowAllMoves()
    {
        //board.GetOptionsRecursive(board, board.activePlayer);
    }

    private void Update()
    {
        if (actionsEnabled)
        {
            if (players[(int)board.activePlayer].isAI)
            {
                (float, Direction) action = minMax.Solve(board, players[(int)board.activePlayer].searchDepth, float.NegativeInfinity, float.PositiveInfinity, board.activePlayer);
                //MakeMoves(action.Item2);
                Move(action.Item2);

            } else
            {
                if (Input.GetKeyDown(KeyCode.Keypad8))
                {
                    Move(Direction.N);
                }
                if (Input.GetKeyDown(KeyCode.Keypad2))
                {
                    Move(Direction.S);
                }
                if (Input.GetKeyDown(KeyCode.Keypad6))
                {
                    Move(Direction.E);
                }
                if (Input.GetKeyDown(KeyCode.Keypad4))
                {
                    Move(Direction.W);
                }
                if (Input.GetKeyDown(KeyCode.Keypad7))
                {
                    Move(Direction.NW);
                }
                if (Input.GetKeyDown(KeyCode.Keypad9))
                {
                    Move(Direction.NE);
                }
                if (Input.GetKeyDown(KeyCode.Keypad1))
                {
                    Move(Direction.SW);
                }
                if (Input.GetKeyDown(KeyCode.Keypad3))
                {
                    Move(Direction.SE);
                }
            }
        }

    }
}
