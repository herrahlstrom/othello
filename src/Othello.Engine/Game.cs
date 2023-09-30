using System.Collections;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Throw;

namespace Othello.Engine;

public class Game
{
    public readonly PlayerColor?[] _table;
    private readonly IAi _ai;

    public Game(IAi ai)
    {
        _ai = ai;
        _table = new PlayerColor?[64];
    }

    public int BlackPoints => _table.Count(x => x == PlayerColor.Black);

    public PlayerColor CurrentPlayer { get; private set; }

    public IReadOnlyList<PlayerColor?> Table => _table;

    public int WhitePoints => _table.Count(x => x == PlayerColor.White);

    public bool CanPlaceStone(int index) => Rules.CanPlaceStone(_table, CurrentPlayer, index);

    public void InitGame(PlayerColor? startPlayer = null)
    {
        for (int i = 0; i < _table.Length; i++)
        {
            _table[i] = null;
        }

        _table[new Position(3, 3).Index] = PlayerColor.Black;
        _table[new Position(4, 4).Index] = PlayerColor.Black;

        _table[new Position(4, 3).Index] = PlayerColor.White;
        _table[new Position(3, 4).Index] = PlayerColor.White;

        CurrentPlayer = startPlayer is { } sp
            ? sp
            : Random.Shared.Next(0, 2) == 0
                ? PlayerColor.White
                : PlayerColor.Black;
    }

    public void LoadGame(string data)
    {
        var bytes = Convert.FromBase64String(data);
        var json = Encoding.UTF8.GetString(bytes);
        var model = JsonSerializer.Deserialize<SerializeGame>(json)
            ?? throw new JsonException("Invalid json");

        CurrentPlayer = model.Current == "W"
            ? PlayerColor.White
            : PlayerColor.Black;

        for (int i = 0; i < _table.Length; i++)
        {
            ulong value = (ulong)1 << i;
            if ((model.White & value) == value)
            {
                _table[i] = PlayerColor.White;
            }
            else if ((model.Black & value) == value)
            {
                _table[i] = PlayerColor.Black;
            }
        }
    }

    public void PlaceStone(int index)
    {
        index.Throw().IfOutOfRange(0, 63);

        var stonesToFlip = Rules.GetFlippableStones(_table, CurrentPlayer, index);
        _table[index] = CurrentPlayer;
        for (int i = 0; stonesToFlip > 0; i++)
        {
            if ((stonesToFlip & 1) == 1)
            {
                _table[i] = CurrentPlayer;
            }
            stonesToFlip >>= 1;
        }

        CurrentPlayer = CurrentPlayer.Equals(PlayerColor.White) ? PlayerColor.Black : PlayerColor.White;
    }

    public void PlaceStoneWithAi()
    {
        var index = _ai.GetIndex(_table, CurrentPlayer);
        PlaceStone(index);
    }

    public string Serialize()
    {
        ulong white = 0;
        ulong black = 0;

        for (int i = 0; i < 64; i++)
        {
            if (_table[i] == PlayerColor.White)
            {
                white |= (ulong)1 << i;
            }
            else if (_table[i] == PlayerColor.Black)
            {
                black |= (ulong)1 << i;
            }
        }

        var model = new SerializeGame(
            Current: CurrentPlayer == PlayerColor.White ? "W" : "B",
            White: white,
            Black: black);

        var json = JsonSerializer.Serialize(model);
        var bytes = Encoding.UTF8.GetBytes(json);
        return Convert.ToBase64String(bytes);
    }

    private record SerializeGame(
        [property: JsonPropertyName("C")] string Current,
        [property: JsonPropertyName("W")] ulong White,
        [property: JsonPropertyName("B")] ulong Black);
}
