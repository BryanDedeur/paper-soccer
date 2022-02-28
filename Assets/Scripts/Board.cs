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
    private Coordinate curCordinate;
    private Coordinate prevCordinate;

    public Board(uint _rows, uint _cols)
    {
        nodes = new uint[_rows, _cols];
        rows = _rows;
        cols = _cols;
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
            }
        }

        // Set the starting position
        curCordinate = new Coordinate((byte)(rows * 0.5f), (byte)(cols * 0.5f));
    }

    private void MarkDirectionUnavailbale(Directions dir, Coordinate cor)
    {
        nodes[cor.i, cor.j] = nodes[cor.i, cor.j] | ((uint)dir);
    }

    private bool IsTerminalState(Coordinate cor)
    {
        // If goal node
        // if no other moves from current move

        return false;
    }

    /*
     * makes a move towards 
     * returns if terminal state or not
     */
    public bool MakeMove(Directions direction)
    {
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

    public List<Coordinate> GetOptions(Coordinate cor)
    {
        List<Coordinate> options = new List<Coordinate>();
        if (IsBitSet(nodes[cor.i - 1, cor.j], 0)) // N
            options.Add(new Coordinate(cor.i - 1, cor.j));
        if (IsBitSet(nodes[cor.i + 1, cor.j], 1)) // S
            options.Add(new Coordinate(cor.i + 1, cor.j));
        if (IsBitSet(nodes[cor.i, cor.j + 1], 2)) // E
            options.Add(new Coordinate(cor.i, cor.j + 1));
        if (IsBitSet(nodes[cor.i, cor.j - 1], 3)) // W
            options.Add(new Coordinate(cor.i, cor.j - 1));
        if (IsBitSet(nodes[cor.i - 1, cor.j + 1], 4)) // NE
            options.Add(new Coordinate(cor.i - 1, cor.j + 1));
        if (IsBitSet(nodes[cor.i - 1, cor.j - 1], 5)) // NW
            options.Add(new Coordinate(cor.i - 1, cor.j - 1));
        if (IsBitSet(nodes[cor.i + 1, cor.j + 1], 6)) // SE
            options.Add(new Coordinate(cor.i + 1, cor.j + 1));
        if (IsBitSet(nodes[cor.i + 1, cor.j - 1], 7)) // SW
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
        for (int i = 0; i < boardWidth; i++)
        {
            for (int j = 0; j < boardLength; j++)
            {
                nodesBin[i, j] = 0b_0000_0000;

                if (i > 0 && j > 0 && i < boardWidth - 1 && j < boardLength - 1)
                {
                    // Not at a boundary we can add all successors
                    nodesBin[i, j] = nodesBin[i, j] | nodesBin[i - 1, j];
                    nodesBin[i, j] = nodesBin[i, j] | nodesBin[i + 1, j];
                    nodesBin[i, j] = nodesBin[i, j] | nodesBin[i, j - 1];
                    nodesBin[i, j] = nodesBin[i, j] | nodesBin[i, j + 1];
                    nodesBin[i, j] = nodesBin[i, j] | nodesBin[i - 1, j - 1];
                    nodesBin[i, j] = nodesBin[i, j] | nodesBin[i + 1, j - 1];
                    nodesBin[i, j] = nodesBin[i, j] | nodesBin[i - 1, j + 1];
                    nodesBin[i, j] = nodesBin[i, j] | nodesBin[i + 1, j + 1];
                }
                else if (i == 0 && j == 0)
                {
                    nodesBin[i, j] = nodesBin[i + 1, j + 1];
                }
                else if (i == 0 && j == boardLength - 1)
                {
                    nodesBin[i, j] = nodesBin[i + 1, j - 1];
                }
                else if (i == boardWidth - 1 && j == 0)
                {
                    nodesBin[i, j] = nodesBin[i - 1, j + 1];
                }
                else if (i == boardWidth - 1 && j == boardLength - 1)
                {
                    nodesBin[i, j] = nodesBin[i - 1, j - 1];
                }
                else if (i == 0)
                {
                    nodesBin[i, j] = nodesBin[i + 1, j];
                    nodesBin[i, j] = nodesBin[i + 1, j - 1];
                    nodesBin[i, j] = nodesBin[i + 1, j + 1];
                }
                else if (i == boardWidth - 1)
                {
                    nodesBin[i, j] = nodesBin[i - 1, j];
                    nodesBin[i, j] = nodesBin[i - 1, j - 1];
                    nodesBin[i, j] = nodesBin[i - 1, j + 1];
                }
                else if (j == 0)
                {
                    nodesBin[i, j] = nodesBin[i, j + 1];
                    nodesBin[i, j] = nodesBin[i - 1, j + 1];
                    nodesBin[i, j] = nodesBin[i + 1, j + 1];
                }
                else if (j == boardWidth - 1)
                {
                    nodesBin[i, j] = nodesBin[i, j - 1];
                    nodesBin[i, j] = nodesBin[i - 1, j - 1];
                    nodesBin[i, j] = nodesBin[i + 1, j - 1];
                }
            }
        }

        // Add special options around goals
        // Goal 1
        //nodes[0][(int)(nodes[0].Count / 2f) + 1].options.Add(goal1Node);
        //nodes[0][(int)(nodes[0].Count / 2f)].options.Add(goal1Node);
        //nodes[0][(int)(nodes[0].Count / 2f) - 1].options.Add(goal1Node);
        // Add side nodes
        nodesBin[0, (int)(boardLength / 2f) - 1] = nodesBin[0, (int)(boardLength / 2f) - 1] | nodesBin[0, (int)(boardLength / 2f)];
        nodesBin[0, (int)(boardLength / 2f)] = nodesBin[0, (int)(boardLength / 2f)] | nodesBin[0, (int)(boardLength / 2f) + 1];
        nodesBin[0, (int)(boardLength / 2f)] = nodesBin[0, (int)(boardLength / 2f)] | nodesBin[0, (int)(boardLength / 2f) - 1];
        nodesBin[0, (int)(boardLength / 2f) + 1] = nodesBin[0, (int)(boardLength / 2f) + 1] | nodesBin[0, (int)(boardLength / 2f)];

        // Goal 2
        //nodes[nodes.Count - 1][(int)(nodes[0].Count / 2f) - 1].options.Add(goal2Node);
        //nodes[nodes.Count - 1][(int)(nodes[0].Count / 2f)].options.Add(goal2Node);
        //nodes[nodes.Count - 1][(int)(nodes[0].Count / 2f) + 1].options.Add(goal2Node);
        // Add side nodes
        nodesBin[boardWidth - 1, (int)(boardLength / 2f) - 1] = nodesBin[boardWidth - 1, (int)(boardLength / 2f) - 1] | nodesBin[boardWidth - 1, (int)(boardLength / 2f)];
        nodesBin[boardWidth - 1, (int)(boardLength / 2f)] = nodesBin[boardWidth - 1, (int)(boardLength / 2f)] | nodesBin[boardWidth - 1, (int)(boardLength / 2f) + 1];
        nodesBin[boardWidth - 1, (int)(boardLength / 2f)] = nodesBin[boardWidth - 1, (int)(boardLength / 2f)] | nodesBin[boardWidth - 1, (int)(boardLength / 2f) - 1];
        nodesBin[boardWidth - 1, (int)(boardLength / 2f) + 1] = nodesBin[boardWidth - 1, (int)(boardLength / 2f) + 1] | nodesBin[boardWidth - 1, (int)(boardLength / 2f)];
    }*/
}
