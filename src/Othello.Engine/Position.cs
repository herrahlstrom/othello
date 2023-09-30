namespace Othello.Engine;

public record struct Position(int X, int Y)
{
    public int Index => Y * 8 + X;

    public bool IsCorner =>
        X == 0 && Y == 0 ||
        X == 7 && Y == 0 ||
        X == 0 && Y == 7 ||
        X == 7 && Y == 7;

    public bool IsEdge => X == 0 || Y == 0 || X == 7 || Y == 7;

    public static Position FromIndex(int index) => new Position(index % 8, index / 8);

    public bool TryMove(int deltaX, int deltaY, out Position path)
    {
        int newX = X + deltaX;
        if (newX < 0 || newX > 7)
        {
            path = default;
            return false;
        }

        int newY = Y + deltaY;
        if (newY < 0 || newY > 7)
        {
            path = default;
            return false;
        }

        path = new Position(newX, newY);
        return true;
    }
};
