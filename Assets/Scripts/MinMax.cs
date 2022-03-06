using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinMax
{
    public (float, Directions) Solve(Board board, uint depth, uint playerDepth, uint maximizePlayerId)
    {
        if (board.activePlayer != maximizePlayerId)
        {
            playerDepth--;
        }

        depth--;

        // Check if board is in terminal state
        if (board.gameOver || playerDepth <= 0 || depth <= 0)
        {
            if (board.activePlayer != maximizePlayerId)
                return (board.StaticEvaluator(board.nonActivePlayer), Directions.N);
            else
                return (board.StaticEvaluator(board.activePlayer), Directions.N);
        }

        Directions action = Directions.N;
        float bestValue = 0;
        if (board.activePlayer == maximizePlayerId)
        {
            bestValue = -10000;
            foreach (KeyValuePair<Directions, Coordinate> cord in board.GetOptions(board.curCordinate))
            {
                Board newBoard = board.FastDeepCopy();
                newBoard.MakeMove(cord.Key);
                (float, Directions) value = Solve(newBoard, depth, playerDepth, maximizePlayerId);
                if (value.Item1 > bestValue)
                {
                    bestValue = value.Item1;
                    action = cord.Key;
                }
            }
        } else
        {
            bestValue = 10000;
            foreach (KeyValuePair<Directions, Coordinate> cord in board.GetOptions(board.curCordinate))
            {
                Board newBoard = board.FastDeepCopy();
                newBoard.MakeMove(cord.Key);
                (float, Directions) value = Solve(newBoard, depth, playerDepth, maximizePlayerId);
                if (value.Item1 < bestValue)
                {
                    bestValue = value.Item1;
                    action = cord.Key;
                }                
                // TODO update how we do this best value stuff
            }
        }
        return (bestValue, action);
    }
}
