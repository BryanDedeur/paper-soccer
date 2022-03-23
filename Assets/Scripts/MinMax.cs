using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinMax
{

    public static int branches;

    public (float, List<Direction>) Solve(Board board, int depth, uint maximizePlayerId)
    {
        // Check if board is in terminal state
        if (board.gameOver || depth == 0)
        {
            if (board.activePlayer != maximizePlayerId)
                return (board.StaticEvaluator(board.nonActivePlayer), new List<Direction>());
            else
                return (board.StaticEvaluator(board.activePlayer), new List<Direction>());
        }

        List<Direction> actions = new List<Direction>();
        float bestValue = 0;
        if (board.activePlayer == maximizePlayerId)
        {
            bestValue = -10000;
            List<List<Direction>> options = board.GetOptionsRecursive(board.activePlayer, 4);
            MonoBehaviour.print(options.Count);
            foreach (List<Direction> setOfMoves in options)
            {
                // Simulate a set of moves on a new board
                Board newBoard = board.FastDeepCopy();
                foreach (Direction dir in setOfMoves)
                {
                    newBoard.MakeMove(dir);
                }
                if (!newBoard.AtOpenNode())
                {
                    continue;
                }
                // Evaluate the board further with minMax
                (float, List<Direction>) value = Solve(newBoard, depth - 1, maximizePlayerId);
                if (value.Item1 > bestValue)
                {
                    bestValue = value.Item1;
                    actions.Clear();
                    foreach (Direction dir in setOfMoves)
                    {
                        actions.Add(dir);
                    }
                }
            }
        } else
        {
            bestValue = 10000;
            List<List<Direction>> options = board.GetOptionsRecursive(board.activePlayer, 4);
            MonoBehaviour.print(options.Count);
            foreach (List<Direction> setOfMoves in options)
            {
                // Simulate a set of moves on a new board
                Board newBoard = board.FastDeepCopy();
                foreach (Direction dir in setOfMoves)
                {
                    newBoard.MakeMove(dir);
                }
                if (!newBoard.AtOpenNode())
                {
                    continue;
                }

                // Evaluate the board further with minMax
                (float, List<Direction>) value = Solve(newBoard, depth - 1, maximizePlayerId);
                if (value.Item1 < bestValue)
                {
                    bestValue = value.Item1;
                    actions.Clear();
                    foreach (Direction dir in setOfMoves)
                    {
                        actions.Add(dir);
                    }
                }                
            }
        }
        return (bestValue, actions);
    }
}
