using System;
using System.Linq;

namespace Othello.Engine.AI;
internal class PredictiveAi : AiBase, IAi
{
    public Position GetIndex(GameTable table, PlayerColor player)
    {
        var candidates = new List<Candidate>();
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));

        GetPoints(table, player, 4, candidates.Add, cts.Token);

        if (candidates.Count == 0)
        {
            throw new NoPossibleMovesException();
        }

        return candidates.OrderBy(x => x.Pos.IsCorner ? 0 : 1)
                         .ThenBy(x => x.Pos.IsEdge ? 0 : 1)
                         .ThenBy(x => x.Pos.IsDangerZone ? 1 : 0)
                         .OrderByDescending(x => x.Point)
                         .Select(x => x.Pos)
                         .First();
    }

    private void GetPoints(GameTable table, PlayerColor player, int level, Action<Candidate> callback, CancellationToken cancellationToken)
    {
        for (int i = 0; i < 64; i++)
        {
            if (table[i] != null)
            {
                continue;
            }

            cancellationToken.ThrowIfCancellationRequested();
            var pos = Position.FromIndex(i);

            var stones = Rules.GetFlippableStones(table, player, pos);
            if (stones == 0)
            {
                continue;
            }

            int points = GetNumberOfStones(stones);

            if(pos.IsCorner)
            {
                points += 30;
            }
            else if(pos.IsEdge)
            {
                points += 10;
            }
            else if(pos.IsDangerZone)
            {
                points -= 5;
            }

            if (level > 1)
            {
                var workingTable = table.Copy();
                workingTable.PlaceStone(pos, player);

                int contraPoins = -1000;
                GetPoints(workingTable, player, level - 1, candidate =>
                {
                    if (candidate.Point > contraPoins)
                    {
                        contraPoins = candidate.Point;
                    }
                }, cancellationToken);
                points -= contraPoins;
            }

            callback.Invoke(new Candidate(pos, points));
        }
    }

    private record struct Candidate(Position Pos, int Point);
}
