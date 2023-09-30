namespace Othello.Engine.AI;
internal abstract class AiBase
{
    protected static int GetNumberOfStones(ulong stones)
    {
        int count = 0;
        for (int index = 0; stones > 0; index++)
        {
            if ((stones & 1) == 1)
            {
                count++;
            }
            stones >>= 1;
        }
        return count;
    }

    protected static PlayerColor OtherPlayer(PlayerColor color) => color.Equals(PlayerColor.White) 
        ? PlayerColor.Black
        : PlayerColor.White;
}
