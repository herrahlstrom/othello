﻿using System.Collections;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Othello.Engine.AI;
using Throw;

namespace Othello.Engine;

public class Game : ISerialiable
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

    public void Load(string data)
    {
        var bytes = Convert.FromBase64String(data);
        var json = Encoding.UTF8.GetString(bytes);
        var model = JsonSerializer.Deserialize<SerializeGame>(json)
            ?? throw new JsonException("Invalid json");

        CurrentPlayer = model.Current == "W"
            ? PlayerColor.White
            : PlayerColor.Black;

        Table.Load(model.Table);
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

    public string Serialize()
    {
        ulong white = 0;
        ulong black = 0;

        for (int i = 0; i < 64; i++)
        {
            if (Table[i] == PlayerColor.White)
            {
                white |= (ulong)1 << i;
            }
            else if (Table[i] == PlayerColor.Black)
            {
                black |= (ulong)1 << i;
            }
        }

        var model = new SerializeGame(
            Current: CurrentPlayer == PlayerColor.White ? "W" : "B",
            Table: Table.Serialize());

        var json = JsonSerializer.Serialize(model);
        var bytes = Encoding.UTF8.GetBytes(json);
        return Convert.ToBase64String(bytes);
    }

    private record SerializeGame(
        [property: JsonPropertyName("C")] string Current,
        [property: JsonPropertyName("T")] string Table);
}
