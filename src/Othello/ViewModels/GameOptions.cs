using Othello.Engine;

namespace Othello.ViewModels;

public class GameOptions
{
    public PlayerColor StartPlayer { get; set; }

    public string Ai { get; set; } = "";
}