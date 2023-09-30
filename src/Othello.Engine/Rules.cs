using Throw;

namespace Othello.Engine;

internal class Rules
{
    public static bool CanPlaceStone(GameTable table, PlayerColor player, int index)
    {
        index.Throw().IfOutOfRange(0, 63);

        return table[index] == null && GetFlippableStones(table, player, index) > 0;
    }

    public static ulong GetFlippableStones(GameTable table, PlayerColor player, int index)
    {
        index.Throw().IfOutOfRange(0, 63);

        Position pos = Position.FromIndex(index);

        return
            GetFlippableStones(-1, 0) |
            GetFlippableStones(+1, 0) |
            GetFlippableStones(0, -1) |
            GetFlippableStones(0, +1) |
            GetFlippableStones(-1, -1) |
            GetFlippableStones(-1, +1) |
            GetFlippableStones(+1, -1) |
            GetFlippableStones(+1, +1);

        ulong GetFlippableStones(int deltaX, int deltaY)
        {
            ulong flippedStones = 0;
            Position next = pos;

            while (next.TryMove(deltaX, deltaY, out Position candidateForNext) && table[candidateForNext.Index] is not null)
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
    }
}
