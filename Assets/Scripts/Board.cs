using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Coordinate
{
    public Coordinate(int _i, int _j)
    {
        i = _i;
        j = _j;
    }

    public int i;
    public int j;
}

public enum Direction
{
    N = 0b_0000_0001, // 1      --> North       (i-1    , j     )
    S = 0b_0000_0010, // 2      --> South       (i+1    , j     )
    E = 0b_0000_0100, // 4      --> East        (i      , j+1   )
    W = 0b_0000_1000, // 8      --> West        (i      , j-1   )
    NE = 0b_0001_0000, // 16     --> North-East  (i-1    , j+1   )
    NW = 0b_0010_0000, // 32     --> North-West  (i-1    , j-1   )
    SE = 0b_0100_0000, // 64     --> South-East  (i+1    , j+1   )
    SW = 0b_1000_0000  // 128    --> South-West  (i+1    , j-1   )
}

public class Board
{
    /*
    
    The board contains a snapshot of moves that led up to the current state. The board is made up of nodes.
    
    Each node contains information about the traversal direction options. 

    The nodes are stored in 8 bit binary indicating which directions have and have not been previous traversed.

    0 indicates the direction has NOT been traversed
    1 indicates the direction has been traversed

    Some nodes like the nodes around the perimiter do not have all the directions available.

        N.W   N   N.E
          \   |   /
           \  |  /
        W----Node----E
            / | \
          /   |  \
        S.W   S   S.E

    */

    private uint rows;
    private uint cols;

    private uint[,] nodes;
    public Coordinate curCordinate;
    public Coordinate prevCordinate;

    // stats
    public int turns;
    public uint[] moves;

    public uint activePlayer;
    public uint nonActivePlayer;
    public bool gameOver = false;
    public int winner = -1;

    public Board() {}

    public Board(uint _rows, uint _cols)
    {
        nodes = new uint[_rows, _cols];
        rows = _rows;
        cols = _cols;
        moves = new uint[2];
        turns = 1;
        Reset();
    }

    public Board FastDeepCopy()
    {
        Board newBoard = new Board();
        newBoard.nodes = new uint[this.rows, this.cols];
        newBoard.rows = this.rows;
        newBoard.cols = this.cols;
        newBoard.moves = new uint[2];
        newBoard.moves[0] = this.moves[0];
        newBoard.moves[1] = this.moves[1];
        newBoard.gameOver = this.gameOver;
        newBoard.curCordinate = new Coordinate(this.curCordinate.i, this.curCordinate.j);
        newBoard.prevCordinate = new Coordinate(this.prevCordinate.i, this.prevCordinate.j);
        newBoard.winner = this.winner;
        newBoard.activePlayer = this.activePlayer;
        newBoard.nonActivePlayer = this.nonActivePlayer;
        newBoard.turns = this.turns;

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                newBoard.nodes[i, j] = this.nodes[i, j];
            }
        }

        return newBoard;
    }

    public void Reset()
    {
        // Reset every node back to default values
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                nodes[i, j] = 0b_0000_0000;

                if (i > 0 && j > 1 && i < rows - 1 && j < cols - 2)
                {
                    // if middle node do nothing
                }
                else if (i == 0 && j == 1)
                {
                    // Upper left corner node can only move south east
                    nodes[i, j] = 0b_1111_1111 ^ (uint)Direction.SE;
                }
                else if (i == 0 && j == cols - 2)
                {
                    // Upper right corner node can only move south west
                    nodes[i, j] = 0b_1111_1111 ^ (uint)Direction.SW;
                }
                else if (i == rows - 1 && j == 1)
                {
                    // Lower left corner node can only move north east
                    nodes[i, j] = 0b_1111_1111 ^ (uint)Direction.NE;
                }
                else if (i == rows - 1 && j == cols - 2)
                {
                    // Lower right corner node can only move north west
                    nodes[i, j] = 0b_1111_1111 ^ (uint)Direction.NW;
                } else if (j == 0 || j == cols - 1)
                {
                    nodes[i, j] = 0b_1111_1111;
                }
                else if (i == 0)
                {
                    // Top Wall
                    nodes[i, j] = 0b_1111_1111 ^ ((uint)Direction.SE | (uint)Direction.S | (uint)Direction.SW);
                }
                else if (i == rows - 1)
                {
                    // Lower Wall
                    nodes[i, j] = 0b_1111_1111 ^ ((uint)Direction.NE | (uint)Direction.N | (uint)Direction.NW);
                }
                else if (j == 1)
                {
                    // Left wall
                    nodes[i, j] = 0b_1111_1111 ^ ((uint)Direction.NE | (uint)Direction.E | (uint)Direction.SE);
                }
                else if (j == cols - 2)
                {
                    // Right Wall
                    nodes[i, j] = 0b_1111_1111 ^ ((uint)Direction.NW | (uint)Direction.W | (uint)Direction.SW);
                } 
                

            }
        }

        // Set the starting position
        curCordinate = new Coordinate((byte)(rows * 0.5f), (byte)(cols * 0.5f));
        gameOver = false;
        moves[0] = 0;
        moves[1] = 0;

        // Add left side node directions
        nodes[(int)(rows / 2f) - 1, 1] = nodes[(int)(rows / 2f) - 1, 1] ^ (uint) Direction.S;
        nodes[(int)(rows / 2f) - 1, 1] = nodes[(int)(rows / 2f) - 1, 1] ^ (uint) Direction.SW;
        nodes[(int)(rows / 2f), 1] = nodes[(int)(rows / 2f), 1] ^ (uint)Direction.N;
        nodes[(int)(rows / 2f), 1] = nodes[(int)(rows / 2f), 1] ^ (uint)Direction.W;
        nodes[(int)(rows / 2f), 1] = nodes[(int)(rows / 2f), 1] ^ (uint)Direction.S;

        nodes[(int)(rows / 2f) + 1, 1] = nodes[(int)(rows / 2f) + 1, 1] ^ (uint) Direction.N;
        nodes[(int)(rows / 2f) + 1, 1] = nodes[(int)(rows / 2f) + 1, 1] ^ (uint) Direction.NW;

        // Add right side node directions
        nodes[(int)(rows / 2f) - 1, cols - 2] = nodes[(int)(rows / 2f) - 1, cols - 2] ^ (uint)Direction.S;
        nodes[(int)(rows / 2f) - 1, cols - 2] = nodes[(int)(rows / 2f) - 1, cols - 2] ^ (uint)Direction.SE;
        nodes[(int)(rows / 2f), cols - 2] = nodes[(int)(rows / 2f), cols - 2] ^ (uint)Direction.N;
        nodes[(int)(rows / 2f), cols - 2] = nodes[(int)(rows / 2f), cols - 2] ^ (uint)Direction.E;
        nodes[(int)(rows / 2f), cols - 2] = nodes[(int)(rows / 2f), cols - 2] ^ (uint)Direction.S;
        nodes[(int)(rows / 2f) + 1, cols - 2] = nodes[(int)(rows / 2f) + 1, cols - 2] ^ (uint)Direction.N;
        nodes[(int)(rows / 2f) + 1, cols - 2] = nodes[(int)(rows / 2f) + 1, cols - 2] ^ (uint)Direction.NE;

    }

    private float DistanceFromSelfGoal(uint playerId)
    {
        if (playerId == 1)
        {
            return (new Vector2(curCordinate.i, curCordinate.j) - new Vector2((int)(rows / 2f), 0)).magnitude;
        }
        return (new Vector2(curCordinate.i, curCordinate.j) - new Vector2((int)(rows / 2f), cols - 1)).magnitude;
    }

    private int IsAtGoal(uint playerId)
    {
        if (curCordinate.i == (int)(rows / 2f))
        {
            if (playerId == 1)
            {
                if (curCordinate.j == cols - 1)
                {
                    return 1;
                }
            }
            else
            {
                if (curCordinate.j == 0)
                {
                    return 1;
                }
            }
        }
        return 0;
    }

    private int IsAtDeadEnd(uint playerId)
    {
        if (GetOptions(curCordinate).Count == 0)
        {
            return 1;
        }
        return 0;
    }

    public float StaticEvaluator(uint playerId)
    {
        float selfScore = 0;

        // If deadend reduce several points;
        selfScore += IsAtDeadEnd(playerId) * -100.0f;

        // If at goal increase score
        selfScore += IsAtGoal(playerId) * 200.0f;

        // If far away from self goal then increase score
        selfScore += DistanceFromSelfGoal(playerId);

        selfScore -= moves[playerId];

        return selfScore;
    }

/*    private void MarkDirectionAvailbale(Direction dir, Coordinate cor)
    {
        nodes[cor.i, cor.j] = nodes[cor.i, cor.j] | ((uint)dir);
    }*/

    private void MarkDirectionUnavailbale(Direction dir, Coordinate cor)
    {
        nodes[cor.i, cor.j] = nodes[cor.i, cor.j] | ((uint)dir);
    }

    public bool AtOpenNode()
    {
        return BitCount(nodes[curCordinate.i, curCordinate.j]) == 1;
    }

    public bool IsValidMove(Direction dir)
    {
        return !IsBitSet(nodes[curCordinate.i, curCordinate.j], dir);
    }

    private int IsTerminalState(Coordinate cor)
    {
        // If goal node
        if (cor.j == 0)
        {
            gameOver = true;
            winner = 0;
            return 0;
        }
        if (cor.j == cols - 1)
        {
            gameOver = true;
            winner = 1;
            return 1;
        }
        // if no other moves from current move
        if (BitCount(nodes[curCordinate.i, curCordinate.j]) == 8)
        {
            gameOver = true;
            winner = 2;
            return 2;
        }
        return -1;
    }

    /*
     * makes a move towards 
     * returns if terminal state or not
     */
    public int MakeMove(Direction direction)
    {
        moves[activePlayer]++;

        Coordinate next = GetCoordinateInDirection(curCordinate, direction);
        MarkDirectionUnavailbale(direction, curCordinate);
        MarkDirectionUnavailbale(GetOppositeDirection(direction), next);

        Coordinate temp = curCordinate;
        curCordinate = next;
        prevCordinate = temp;

        int state = IsTerminalState(curCordinate);

        if (state == -1)
        {
            if (AtOpenNode())
            {
                SwitchPlayers();
            }
        }

        return state;
    }

    public void SwitchPlayers()
    {
        if (activePlayer == 0)
        {
            activePlayer = 1;
            nonActivePlayer = 0;
        }
        else
        {
            activePlayer = 0;
            nonActivePlayer = 1;
        }
        turns++;
    }

    bool IsBitSet(uint b, int pos)
    {
        return (b & (1 << pos)) != 0;
    }

    bool IsBitSet(uint b, Direction dir)
    {
        bool state = false;
        switch (dir)
        {
            case Direction.N:
                state = IsBitSet(b, 0);
                break;
            case Direction.S:
                state = IsBitSet(b, 1);
                break;
            case Direction.E:
                state = IsBitSet(b, 2);
                break;
            case Direction.W:
                state = IsBitSet(b, 3);
                break;
            case Direction.NE:
                state = IsBitSet(b, 4);
                break;
            case Direction.NW:
                state = IsBitSet(b, 5);
                break;
            case Direction.SE:
                state = IsBitSet(b, 6);
                break;
            case Direction.SW:
                state = IsBitSet(b, 7);
                break;
            default:
                break;
        }
        return state;
    }

    int BitCount(uint n)
    {
        int count = 0;
        while (n > 0)
        {
            count++;
            n = n & (n - 1);
        }
        return count;
    }

    public List<Direction> GetOptions(Coordinate cor)
    {
        List<Direction> options = new List<Direction>();
        if (!IsBitSet(nodes[cor.i, cor.j], 0)) // N
            options.Add(Direction.N);
        if (!IsBitSet(nodes[cor.i, cor.j], 1)) // S
            options.Add(Direction.S);
        if (!IsBitSet(nodes[cor.i, cor.j], 2)) // E
            options.Add(Direction.E);
        if (!IsBitSet(nodes[cor.i, cor.j], 3)) // W
            options.Add(Direction.W);
        if (!IsBitSet(nodes[cor.i, cor.j], 4)) // NE
            options.Add(Direction.NE);
        if (!IsBitSet(nodes[cor.i, cor.j], 5)) // NW
            options.Add(Direction.NW);
        if (!IsBitSet(nodes[cor.i, cor.j], 6)) // SE
            options.Add(Direction.SE);
        if (!IsBitSet(nodes[cor.i, cor.j], 7)) // SW
            options.Add(Direction.SW);
        return options;
    }

    public List<List<Direction>> GetOptionsRecursive(uint focusPlayer, int maxDepth = -1)
    {
        List<List<Direction>> options = new List<List<Direction>>();
        if (this.gameOver || focusPlayer != this.activePlayer || maxDepth == 0)
        {
            return options;
        }

        // For each possible move at current state
        foreach (Direction dir in this.GetOptions(this.curCordinate))
        {
            // Make move on hypothetical board
            Board tempBoard = this.FastDeepCopy();
            tempBoard.MakeMove(dir);

            // check if subMoves leads to an open node
            if (tempBoard.AtOpenNode())
            {
                options.Add(new List<Direction>());
                options[options.Count - 1].Add(dir);
            }
            else
            {
                // Get all possible options from new state
                List<List<Direction>> subOptions = tempBoard.GetOptionsRecursive(focusPlayer, maxDepth - 1);

                MonoBehaviour.print(subOptions.Count);
                foreach (List<Direction> subMoves in subOptions)
                {
                    options.Add(new List<Direction>());
                    options[options.Count - 1].Add(dir);
                    foreach (Direction subMove in subMoves)
                    {
                        options[options.Count - 1].Add(subMove);
                    }
                }
            }
        }

        return options;
    }

    public Coordinate GetCoordinateInDirection(Coordinate cor, Direction dir)
    {
        Coordinate otherCor = new Coordinate();
        switch (dir)
        {
            case Direction.N:
                otherCor = new Coordinate(cor.i - 1, cor.j);
                break;
            case Direction.S:
                otherCor = new Coordinate(cor.i + 1, cor.j);
                break;
            case Direction.E:
                otherCor = new Coordinate(cor.i, cor.j + 1);
                break;
            case Direction.W:
                otherCor = new Coordinate(cor.i, cor.j - 1);
                break;
            case Direction.NE:
                otherCor = new Coordinate(cor.i - 1, cor.j + 1);
                break;
            case Direction.NW:
                otherCor = new Coordinate(cor.i - 1, cor.j - 1);
                break;
            case Direction.SE:
                otherCor = new Coordinate(cor.i + 1, cor.j + 1);
                break;
            case Direction.SW:
                otherCor = new Coordinate(cor.i + 1, cor.j - 1);
                break;
            default:
                break;
        }

        return otherCor;
    }

    private Direction GetOppositeDirection(Direction dir)
    {
        Direction opposite = Direction.N;
        switch (dir)
        {
            case Direction.N:
                opposite = Direction.S;
                break;
            case Direction.S:
                opposite = Direction.N;
                break;
            case Direction.E:
                opposite = Direction.W;
                break;
            case Direction.W:
                opposite = Direction.E;
                break;
            case Direction.NE:
                opposite = Direction.SW;
                break;
            case Direction.NW:
                opposite = Direction.SE;
                break;
            case Direction.SE:
                opposite = Direction.NW;
                break;
            case Direction.SW:
                opposite = Direction.NE;
                break;
            default:
                break;
        }

        return opposite;
    }
}
