namespace Othello.Engine.AI;
internal class SimpleAi : AiBase, IAi
{
    public Position GetIndex(GameTable table, PlayerColor player)
    {
        var candidates = new List<CandidateToPlace>();
        for (int i = 0; i < 64; i++)
        {
            var pos = Position.FromIndex(i);

            //if(!Rules.CanPlaceStone(table, player, pos))
            //{
            //    continue;
            //}

            var stones = Rules.GetFlippableStones(table, player, pos);
            if (stones == 0) { continue; }

            candidates.Add(new CandidateToPlace(pos, GetNumberOfStones(stones)));
        }

        if (candidates.Count == 0)
        {
            throw new NoPossibleMovesException();
        }

        return candidates.OrderBy(x => x.Pos.IsCorner ? 0 : 1)
                         .ThenBy(x => x.Pos.IsEdge ? 0 : 1)
                         .OrderByDescending(x => x.FlippedStones)
                         .Select(x => x.Pos)
                         .First();
    }

    private record CandidateToPlace(Position Pos, int FlippedStones);
}
