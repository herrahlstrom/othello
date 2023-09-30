using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Othello.Engine.AI;
internal class PredictiveAi : AiBase, IAi
{
    IAi _baseAi;
    public PredictiveAi()
    {
        _baseAi = new SimpleAi();
    }

    public Position GetIndex(GameTable table, PlayerColor player)
    {
        // Any possible corners?
        foreach (var pos in Position.All.Where(x => x.IsCorner))
        {
            if (Rules.GetFlippableStones(table, player, pos) > 0)
            {
                return pos;
            }
        }

        var candidates = new List<Candidate>();
        for (int i = 0; i < 64; i++)
        {
            var pos = Position.FromIndex(i);

            var stones = Rules.GetFlippableStones(table, player, pos);
            if (stones == 0) { continue; }

            int points = GetNumberOfStones(stones);

            var workingTable = table.Clone();
            workingTable.PlaceStone(pos, player);
            points -= CalcContraPoints(workingTable, OtherPlayer(player), 1);

            candidates.Add(new Candidate(pos, points));
        }

        if (candidates.Count == 0)
        {
            throw new NoPossibleMovesException();
        }

        return candidates.OrderBy(x => x.Pos.IsCorner ? 0 : 1)
                         .ThenBy(x => x.Pos.IsEdge ? 0 : 1)
                         .OrderByDescending(x => x.Point)
                         .Select(x => x.Pos)
                         .First();
    }

    private int CalcContraPoints(GameTable table, PlayerColor player, int level)
    {
        int maxPoints = 0;

        for (int i = 0; i < 64; i++)
        {
            var pos = Position.FromIndex(i);
            var stones = Rules.GetFlippableStones(table, player, pos);

            if (stones == 0) { continue; }

            int points = GetNumberOfStones(stones);

            // extra points fro corner and edges
            if (pos.IsCorner)
            {
                points += 30;
            }
            else if (pos.IsEdge)
            {
                points += 10;
            }

            if (level > 1)
            {
                var workingTable = table.Clone();
                workingTable.PlaceStone(pos, player);
                points -= CalcContraPoints(workingTable, OtherPlayer(player), level - 1);
            }

            if (points > maxPoints)
            {
                maxPoints = points;
            }
        }

        return maxPoints;
    }

    private record Candidate(Position Pos, int Point);
}
