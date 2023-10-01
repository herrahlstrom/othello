using Othello.Engine;

namespace Othello.ViewModels;

public class Player
{
    public Player(PlayerColor color, string name)
    {
        Color = color;
        Name = name;
    }

    public bool Ai { get; set; }
    public PlayerColor Color { get; }
    public string Name { get; }
    public int Points { get; set; }
}
