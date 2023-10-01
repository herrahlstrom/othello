using System.Collections;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Othello.Engine.AI;
using Throw;

namespace Othello.Engine;

public class Game
{
    private readonly IAi _ai;

    public Game(IAi ai)
    {
        _ai = ai;
        Table = new GameTable();
    }

    public PlayerColor CurrentPlayer { get; private set; }

    public GameTable Table { get; }

    public bool CanPlaceStone(Position pos) => Rules.CanPlaceStone(Table, CurrentPlayer, pos);

    public void InitGame(PlayerColor? startPlayer = null)
    {
        Table.InitNewGame();

        CurrentPlayer = startPlayer is { } sp
            ? sp
            : Random.Shared.Next(0, 2) == 0
                ? PlayerColor.White
                : PlayerColor.Black;
    }

    public void PlaceStone(Position pos)
    {
        Table.PlaceStone(pos, CurrentPlayer);

        CurrentPlayer = CurrentPlayer.Equals(PlayerColor.White) ? PlayerColor.Black : PlayerColor.White;
    }

    public void PlaceStoneWithAi()
    {
        var pos = _ai.GetIndex(Table, CurrentPlayer);
        PlaceStone(pos);
    }
}
