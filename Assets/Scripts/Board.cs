using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Coordinate
{
    public Coordinate(uint _i, uint _j)
    {
        i = _i;
        j = _j;
    }

    public uint i;
    public uint j;
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
    public uint moves = 0;
    public uint[] playerMoves;

    public bool gameOver = false;

    public Board(uint _rows, uint _cols)
    {
        nodes = new uint[_rows, _cols];
        rows = _rows;
        cols = _cols;
        playerMoves = new uint[2];
        Reset();
    }

    public void Reset()
    {
        // Reset every node back to default values
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                nodes[i, j] = 0b_0000_0000;

                if (i > 0 && j > 0 && i < rows - 1 && j < cols - 1)
                {
                    // if middle node do nothing
                }
                else if (i == 0 && j == 0)
                {
                    // Upper left corner node can only move south east
                    nodes[i, j] = 0b_1111_1111 ^ (uint)Directions.SE;
                }
                else if (i == 0 && j == cols - 1)
                {
                    // Upper right corner node can only move south west
                    nodes[i, j] = 0b_1111_1111 ^ (uint)Directions.SW;
                }
                else if (i == rows - 1 && j == 0)
                {
                    // Lower left corner node can only move north east
                    nodes[i, j] = 0b_1111_1111 ^ (uint)Directions.NE;
                }
                else if (i == rows - 1 && j == cols - 1)
                {
                    // Lower right corner node can only move north west
                    nodes[i, j] = 0b_1111_1111 ^ (uint)Directions.NW;
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
                else if (j == 0)
                {
                    // Left wall
                    nodes[i, j] = 0b_1111_1111 ^ ((uint)Directions.NE | (uint)Directions.E | (uint)Directions.SE);
                }
                else if (j == cols - 1)
                {
                    // Right Wall
                    nodes[i, j] = 0b_1111_1111 ^ ((uint)Directions.NW | (uint)Directions.W | (uint)Directions.SW);
                }
            }
        }

        // Set the starting position
        curCordinate = new Coordinate((byte)(rows * 0.5f), (byte)(cols * 0.5f));
        gameOver = false;
        moves = 0;
        playerMoves[0] = 0;
        playerMoves[1] = 0;
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

    private bool IsTerminalState(Coordinate cor)
    {
        // If goal node
        // if no other moves from current move
        if (BitCount(nodes[curCordinate.i, curCordinate.j]) == 8)
        {
            return true;
        }
        return false;
    }

    /*
     * makes a move towards 
     * returns if terminal state or not
     */
    public bool MakeMove(uint playerId, Directions direction)
    {
        moves++;
        playerMoves[playerId]++;

        Coordinate next = GetCoordinateInDirection(curCordinate, direction);

        MarkDirectionUnavailbale(direction, curCordinate);
        MarkDirectionUnavailbale(GetOppositeDirection(direction), next);

        Coordinate temp = curCordinate;
        curCordinate = next;
        prevCordinate = temp;

        return IsTerminalState(curCordinate);
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

    public List<Coordinate> GetOptions(Coordinate cor)
    {
        // TODO consider perimiter
        List<Coordinate> options = new List<Coordinate>();
        if (!IsBitSet(nodes[cor.i, cor.j], 0)) // N
            options.Add(new Coordinate(cor.i - 1, cor.j));
        if (!IsBitSet(nodes[cor.i, cor.j], 1)) // S
            options.Add(new Coordinate(cor.i + 1, cor.j));
        if (!IsBitSet(nodes[cor.i, cor.j], 2)) // E
            options.Add(new Coordinate(cor.i, cor.j + 1));
        if (!IsBitSet(nodes[cor.i, cor.j], 3)) // W
            options.Add(new Coordinate(cor.i, cor.j - 1));
        if (!IsBitSet(nodes[cor.i, cor.j], 4)) // NE
            options.Add(new Coordinate(cor.i - 1, cor.j + 1));
        if (!IsBitSet(nodes[cor.i, cor.j], 5)) // NW
            options.Add(new Coordinate(cor.i - 1, cor.j - 1));
        if (!IsBitSet(nodes[cor.i, cor.j], 6)) // SE
            options.Add(new Coordinate(cor.i + 1, cor.j + 1));
        if (!IsBitSet(nodes[cor.i, cor.j], 7)) // SW
            options.Add(new Coordinate(cor.i + 1, cor.j - 1));
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
/*

    private void ResetAllNodeOptions()
    {
        *//*
        This will update every nodes list of neighbors for quick access later. 

            N.W   N   N.E
              \   |   /
               \  |  /
            W----Node----E
                / | \
              /   |  \
            S.W   S   S.E

        A --> North       (i-1, j)
        B --> South       (i+1, j)
        C --> East        (i, j+1)
        D --> West        (i, j-1)
        E --> North-East  (i-1, j+1)
        F --> North-West  (i-1, j-1)
        G --> South-East  (i+1, j+1)
        H --> South-West  (i+1, j-1)*//*
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                nodes[i, j] = 0b_0000_0000;

                if (i > 0 && j > 0 && i < rows - 1 && j < cols - 1)
                {
                    // Not at a boundary we can add all successors
                    nodes[i, j] = nodes[i, j] | nodes[i - 1, j];
                    nodes[i, j] = nodes[i, j] | nodes[i + 1, j];
                    nodes[i, j] = nodes[i, j] | nodes[i, j - 1];
                    nodes[i, j] = nodes[i, j] | nodes[i, j + 1];
                    nodes[i, j] = nodes[i, j] | nodes[i - 1, j - 1];
                    nodes[i, j] = nodes[i, j] | nodes[i + 1, j - 1];
                    nodes[i, j] = nodes[i, j] | nodes[i - 1, j + 1];
                    nodes[i, j] = nodes[i, j] | nodes[i + 1, j + 1];
                }
                else if (i == 0 && j == 0)
                {
                    nodes[i, j] = nodes[i + 1, j + 1];
                }
                else if (i == 0 && j == cols - 1)
                {
                    nodes[i, j] = nodes[i + 1, j - 1];
                }
                else if (i == rows - 1 && j == 0)
                {
                    nodes[i, j] = nodes[i - 1, j + 1];
                }
                else if (i == rows - 1 && j == cols - 1)
                {
                    nodes[i, j] = nodes[i - 1, j - 1];
                }
                else if (i == 0)
                {
                    nodes[i, j] = nodes[i + 1, j];
                    nodes[i, j] = nodes[i + 1, j - 1];
                    nodes[i, j] = nodes[i + 1, j + 1];
                }
                else if (i == rows - 1)
                {
                    nodes[i, j] = nodes[i - 1, j];
                    nodes[i, j] = nodes[i - 1, j - 1];
                    nodes[i, j] = nodes[i - 1, j + 1];
                }
                else if (j == 0)
                {
                    nodes[i, j] = nodes[i, j + 1];
                    nodes[i, j] = nodes[i - 1, j + 1];
                    nodes[i, j] = nodes[i + 1, j + 1];
                }
                else if (j == rows - 1)
                {
                    nodes[i, j] = nodes[i, j - 1];
                    nodes[i, j] = nodes[i - 1, j - 1];
                    nodes[i, j] = nodes[i + 1, j - 1];
                }
            }
        }

        // Add special options around goals
        // Goal 1
        //nodes[0][(int)(nodes[0].Count / 2f) + 1].options.Add(goal1Node);
        //nodes[0][(int)(nodes[0].Count / 2f)].options.Add(goal1Node);
        //nodes[0][(int)(nodes[0].Count / 2f) - 1].options.Add(goal1Node);
        // Add side nodes
        nodes[0, (int)(cols / 2f) - 1] = nodes[0, (int)(cols / 2f) - 1] | nodes[0, (int)(cols / 2f)];
        nodes[0, (int)(cols / 2f)] = nodes[0, (int)(cols / 2f)] | nodes[0, (int)(cols / 2f) + 1];
        nodes[0, (int)(cols / 2f)] = nodes[0, (int)(cols / 2f)] | nodes[0, (int)(cols / 2f) - 1];
        nodes[0, (int)(cols / 2f) + 1] = nodes[0, (int)(cols / 2f) + 1] | nodes[0, (int)(cols / 2f)];

        // Goal 2
        //nodes[nodes.Count - 1][(int)(nodes[0].Count / 2f) - 1].options.Add(goal2Node);
        //nodes[nodes.Count - 1][(int)(nodes[0].Count / 2f)].options.Add(goal2Node);
        //nodes[nodes.Count - 1][(int)(nodes[0].Count / 2f) + 1].options.Add(goal2Node);
        // Add side nodes
        nodes[rows - 1, (int)(cols / 2f) - 1] = nodes[rows - 1, (int)(cols / 2f) - 1] | nodes[rows - 1, (int)(cols / 2f)];
        nodes[rows - 1, (int)(cols / 2f)] = nodes[rows - 1, (int)(cols / 2f)] | nodes[rows - 1, (int)(cols / 2f) + 1];
        nodes[rows - 1, (int)(cols / 2f)] = nodes[rows - 1, (int)(cols / 2f)] | nodes[rows - 1, (int)(cols / 2f) - 1];
        nodes[rows - 1, (int)(cols / 2f) + 1] = nodes[rows - 1, (int)(cols / 2f) + 1] | nodes[rows - 1, (int)(cols / 2f)];
    }*/
}
