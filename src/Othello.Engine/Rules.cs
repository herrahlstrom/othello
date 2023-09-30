namespace Othello.Engine;

internal class Rules
{
    private static Direction[] Directions = new Direction[]
    {
        new(-1, 0),
        new(+1, 0),
        new(0, -1),
        new(0, +1),
        new(-1, -1),
        new(-1, +1),
        new(+1, -1),
        new(+1, +1)
    };

    public static bool CanPlaceStone(GameTable table, PlayerColor player, Position pos)
    {
        return table[pos.Index] == null && Directions.Any(dir => GetFlippableStones(table, pos, player, dir) > 0);
    }

    public static ulong GetFlippableStones(GameTable table, PlayerColor player, Position pos)
    {
        return Directions.Aggregate(
            (ulong)0,
            (seed, dir) => seed | GetFlippableStones(table, pos, player, dir));
    }

    private static ulong GetFlippableStones(GameTable table, Position pos, PlayerColor player, Direction dir)
    {
        ulong flippedStones = 0;
        Position next = pos;

        while (next.TryMove(dir.X, dir.Y, out Position candidateForNext) && table[candidateForNext.Index] is not null)
        {
            if (table[candidateForNext.Index] == player)
            {
                return flippedStones;
            }
            else
            {
                flippedStones |= (ulong)1 << candidateForNext.Index;
                next = candidateForNext;
                continue;
            }
        }

        return 0;
    }

    private record struct Direction(int X, int Y);
}
