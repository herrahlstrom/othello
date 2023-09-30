namespace Othello.Engine;

internal class SimpleAi : IAi
{
    public int GetIndex(GameTable table, PlayerColor player)
    {
        var candidates = new List<CandidateToPlace>();
        for(int i = 0; i < 64; i++)
        {
            if(!Rules.CanPlaceStone(table, player, i))
            {
                continue;
            }

            var stones = Rules.GetFlippableStones(table, player, i);

            candidates.Add(new CandidateToPlace(Position.FromIndex(i), GetPositions(stones).ToList()));
        }

        if(candidates.Count == 0)
        {
            throw new NoPossibleMovesException();
        }

        return candidates.OrderBy(x => x.Pos.IsCorner ? 0 : 1)
                         .ThenBy(x => x.Pos.IsEdge ? 0 : 1)
                         .OrderByDescending(x => x.FlippedStones.Count)
                         .Select(x => x.Pos.Index)
                         .FirstOrDefault();
    }

    private static IEnumerable<Position> GetPositions(ulong stones)
    {
        for (int index = 0; stones > 0; index++)
        {
            if ((stones & 1) == 1)
            {
                yield return Position.FromIndex(index);
            }
            stones >>= 1;
        }
    }

    private record CandidateToPlace(Position Pos, IReadOnlyCollection<Position> FlippedStones);
}
