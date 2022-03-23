using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinMax
{

    public static int branches;

    public (float, Direction) Solve(Board board, int depth, float alpha, float beta, uint maximizePlayerId)
    {
        // Check if board is in terminal state
        if (board.gameOver || depth == 0)
        {
            if (board.activePlayer != maximizePlayerId)
                return (board.StaticEvaluator(board.nonActivePlayer), Direction.N);
            else
                return (board.StaticEvaluator(board.activePlayer), Direction.N);
        }

        Direction action = new Direction();
        float bestValue = 0;
        if (board.activePlayer == maximizePlayerId)
        {
            bestValue = -10000;
            List<Direction> options = board.GetOptions(board.curCordinate);
            MonoBehaviour.print(options.Count);
            foreach (Direction dir in options)
            {
                // Simulate a set of moves on a new board
                Board newBoard = board.FastDeepCopy();
                newBoard.MakeMove(dir);

                // Evaluate the board further with minMax
                (float, Direction) value = Solve(newBoard, depth - 1, alpha, beta, maximizePlayerId);

                if (value.Item1 > bestValue)
                {
                    bestValue = value.Item1;
                    action = dir;
                }

                // Alpha beta pruning
                alpha = Mathf.Max(alpha, value.Item1);
                if (beta <= alpha)
                {
                    break;
                }
            }
        } else
        {
            bestValue = 10000;
            List<Direction> options = board.GetOptions(board.curCordinate);
            MonoBehaviour.print(options.Count);
            foreach (Direction dir in options)
            {
                // Simulate a set of moves on a new board
                Board newBoard = board.FastDeepCopy();
                newBoard.MakeMove(dir);

                // Evaluate the board further with minMax
                (float, Direction) value = Solve(newBoard, depth - 1, alpha, beta, maximizePlayerId);

                if (value.Item1 < bestValue)
                {
                    bestValue = value.Item1;
                    action = dir;
                }

                // Alpha beta pruning
                beta = Mathf.Min(beta, value.Item1);
                if (beta <= alpha)
                {
                    break;
                }
            }
        }
        return (bestValue, action);
    }
}
