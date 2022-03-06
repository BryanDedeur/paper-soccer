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

public enum Directions
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
                    nodes[i, j] = 0b_1111_1111 ^ (uint)Directions.SE;
                }
                else if (i == 0 && j == cols - 2)
                {
                    // Upper right corner node can only move south west
                    nodes[i, j] = 0b_1111_1111 ^ (uint)Directions.SW;
                }
                else if (i == rows - 1 && j == 1)
                {
                    // Lower left corner node can only move north east
                    nodes[i, j] = 0b_1111_1111 ^ (uint)Directions.NE;
                }
                else if (i == rows - 1 && j == cols - 2)
                {
                    // Lower right corner node can only move north west
                    nodes[i, j] = 0b_1111_1111 ^ (uint)Directions.NW;
                } else if (j == 0 || j == cols - 1)
                {
                    nodes[i, j] = 0b_1111_1111;
                }
                else if (i == 0)
                {
                    // Top Wall
                    nodes[i, j] = 0b_1111_1111 ^ ((uint)Directions.SE | (uint)Directions.S | (uint)Directions.SW);
                }
                else if (i == rows - 1)
                {
                    // Lower Wall
                    nodes[i, j] = 0b_1111_1111 ^ ((uint)Directions.NE | (uint)Directions.N | (uint)Directions.NW);
                }
                else if (j == 1)
                {
                    // Left wall
                    nodes[i, j] = 0b_1111_1111 ^ ((uint)Directions.NE | (uint)Directions.E | (uint)Directions.SE);
                }
                else if (j == cols - 2)
                {
                    // Right Wall
                    nodes[i, j] = 0b_1111_1111 ^ ((uint)Directions.NW | (uint)Directions.W | (uint)Directions.SW);
                } 
                

            }
        }

        // Set the starting position
        curCordinate = new Coordinate((byte)(rows * 0.5f), (byte)(cols * 0.5f));
        gameOver = false;
        moves[0] = 0;
        moves[1] = 0;

        // Add left side node directions
        nodes[(int)(rows / 2f) - 1, 1] = nodes[(int)(rows / 2f) - 1, 1] ^ (uint) Directions.S;
        nodes[(int)(rows / 2f) - 1, 1] = nodes[(int)(rows / 2f) - 1, 1] ^ (uint) Directions.SW;
        nodes[(int)(rows / 2f), 1] = 0b_0000_0000;
        nodes[(int)(rows / 2f) + 1, 1] = nodes[(int)(rows / 2f) + 1, 1] ^ (uint) Directions.N;
        nodes[(int)(rows / 2f) + 1, 1] = nodes[(int)(rows / 2f) + 1, 1] ^ (uint) Directions.NW;

        // Add right side node directions
        nodes[(int)(rows / 2f) - 1, cols - 2] = nodes[(int)(rows / 2f) - 1, cols - 2] ^ (uint)Directions.S;
        nodes[(int)(rows / 2f) - 1, cols - 2] = nodes[(int)(rows / 2f) - 1, cols - 2] ^ (uint)Directions.SE;
        nodes[(int)(rows / 2f), 1] = 0b_0000_0000;
        nodes[(int)(rows / 2f) + 1, cols - 2] = nodes[(int)(rows / 2f) + 1, cols - 2] ^ (uint)Directions.N;
        nodes[(int)(rows / 2f) + 1, cols - 2] = nodes[(int)(rows / 2f) + 1, cols - 2] ^ (uint)Directions.NE;

    }

    public float StaticEvaluator(uint playerId)
    {
        float score = 0;
        // If Deadend then significantly punish the player
        if (GetOptions(curCordinate).Count == 0)
        {
            score += -100;
        }
        // If goal node == nonplayergoal 100 points
        if (curCordinate.i == (int)(rows / 2f))
        {
            if (playerId == 1)
            {
                if (curCordinate.j == cols - 1)
                {
                    score += 200;
                }
            }
            else
            {
                if (curCordinate.j == cols)
                {
                    score += 200;
                }
            }
        }


        // Number of bounces nodes available
        // Distance from nearest victory nodes
        // Distance from opposite goal
        if (playerId == 1)
        {
            score += (new Vector2(curCordinate.i, curCordinate.j) - new Vector2((int)(rows / 2f), 0)).magnitude;
        }
        else
        {
            score += (new Vector2(curCordinate.i, curCordinate.j) - new Vector2((int)(rows / 2f), cols - 1)).magnitude;
        }
        // Number of current bounces
        //score += moves[playerId];
        return score;
    }

    private void MarkDirectionUnavailbale(Directions dir, Coordinate cor)
    {
        nodes[cor.i, cor.j] = nodes[cor.i, cor.j] | ((uint)dir);
    }

    public bool AtOpenNode()
    {
        return BitCount(nodes[curCordinate.i, curCordinate.j]) == 1;
    }

    public bool IsValidMove(Directions dir)
    {
        return !IsBitSet(nodes[curCordinate.i, curCordinate.j], dir);
    }

    private int IsTerminalState(Coordinate cor)
    {
        // If goal node
        if (cor.j < 0)
        {
            gameOver = true;
            winner = 0;
            return 0;
        }
        if (cor.j >= cols)
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
    public int MakeMove(Directions direction)
    {
        moves[activePlayer]++;

        Coordinate next = GetCoordinateInDirection(curCordinate, direction);
        if (!(next.j < 0 || next.j >= cols))
        {
            MarkDirectionUnavailbale(direction, curCordinate);
            MarkDirectionUnavailbale(GetOppositeDirection(direction), next);
        }

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
    }

    bool IsBitSet(uint b, int pos)
    {
        return (b & (1 << pos)) != 0;
    }

    bool IsBitSet(uint b, Directions dir)
    {
        bool state = false;
        switch (dir)
        {
            case Directions.N:
                state = IsBitSet(b, 0);
                break;
            case Directions.S:
                state = IsBitSet(b, 1);
                break;
            case Directions.E:
                state = IsBitSet(b, 2);
                break;
            case Directions.W:
                state = IsBitSet(b, 3);
                break;
            case Directions.NE:
                state = IsBitSet(b, 4);
                break;
            case Directions.NW:
                state = IsBitSet(b, 5);
                break;
            case Directions.SE:
                state = IsBitSet(b, 6);
                break;
            case Directions.SW:
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

    public Dictionary<Directions, Coordinate> GetOptions(Coordinate cor)
    {
        // TODO consider perimiter
        Dictionary<Directions, Coordinate> options = new Dictionary<Directions, Coordinate>();
        if (!IsBitSet(nodes[cor.i, cor.j], 0)) // N
            options.Add(Directions.N, new Coordinate(cor.i - 1, cor.j));
        if (!IsBitSet(nodes[cor.i, cor.j], 1)) // S
            options.Add(Directions.S, new Coordinate(cor.i + 1, cor.j));
        if (!IsBitSet(nodes[cor.i, cor.j], 2)) // E
            options.Add(Directions.E, new Coordinate(cor.i, cor.j + 1));
        if (!IsBitSet(nodes[cor.i, cor.j], 3)) // W
            options.Add(Directions.W, new Coordinate(cor.i, cor.j - 1));
        if (!IsBitSet(nodes[cor.i, cor.j], 4)) // NE
            options.Add(Directions.NE, new Coordinate(cor.i - 1, cor.j + 1));
        if (!IsBitSet(nodes[cor.i, cor.j], 5)) // NW
            options.Add(Directions.NW, new Coordinate(cor.i - 1, cor.j - 1));
        if (!IsBitSet(nodes[cor.i, cor.j], 6)) // SE
            options.Add(Directions.SE, new Coordinate(cor.i + 1, cor.j + 1));
        if (!IsBitSet(nodes[cor.i, cor.j], 7)) // SW
            options.Add(Directions.SW, new Coordinate(cor.i + 1, cor.j - 1));
        return options;
    }

    private Coordinate GetCoordinateInDirection(Coordinate cor, Directions dir)
    {
        Coordinate otherCor = new Coordinate();
        switch (dir)
        {
            case Directions.N:
                otherCor = new Coordinate(cor.i - 1, cor.j);
                break;
            case Directions.S:
                otherCor = new Coordinate(cor.i + 1, cor.j);
                break;
            case Directions.E:
                otherCor = new Coordinate(cor.i, cor.j + 1);
                break;
            case Directions.W:
                otherCor = new Coordinate(cor.i, cor.j - 1);
                break;
            case Directions.NE:
                otherCor = new Coordinate(cor.i - 1, cor.j + 1);
                break;
            case Directions.NW:
                otherCor = new Coordinate(cor.i - 1, cor.j - 1);
                break;
            case Directions.SE:
                otherCor = new Coordinate(cor.i + 1, cor.j + 1);
                break;
            case Directions.SW:
                otherCor = new Coordinate(cor.i + 1, cor.j - 1);
                break;
            default:
                break;
        }

        return otherCor;
    }

    private Directions GetOppositeDirection(Directions dir)
    {
        Directions opposite = Directions.N;
        switch (dir)
        {
            case Directions.N:
                opposite = Directions.S;
                break;
            case Directions.S:
                opposite = Directions.N;
                break;
            case Directions.E:
                opposite = Directions.W;
                break;
            case Directions.W:
                opposite = Directions.E;
                break;
            case Directions.NE:
                opposite = Directions.SW;
                break;
            case Directions.NW:
                opposite = Directions.SE;
                break;
            case Directions.SE:
                opposite = Directions.NW;
                break;
            case Directions.SW:
                opposite = Directions.NE;
                break;
            default:
                break;
        }

        return opposite;
    }
}
