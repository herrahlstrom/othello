using System.Collections;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Throw;

namespace Othello.Engine;

public class Game
{
    public readonly PlayerColor?[] _table;

    public Game()
    {
        _table = new PlayerColor?[64];
    }

    public PlayerColor CurrentPlayer { get; private set; }

    public IReadOnlyList<PlayerColor?> Table => _table;

    public bool CanPlaceStone(int index)
    {
        index.Throw().IfOutOfRange(0, 63);

        return _table[index] == null && GetFlippableStones(index) > 0;
    }

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

        var stonesToFlip = GetFlippableStones(index);
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

    private ulong GetFlippableStones(int index)
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

            while (next.TryMove(deltaX, deltaY, out Position candidateForNext) && _table[candidateForNext.Index] is not null)
            {
                if (_table[candidateForNext.Index] == CurrentPlayer)
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

    private record SerializeGame(
        [property: JsonPropertyName("C")] string Current,
        [property: JsonPropertyName("W")] ulong White,
        [property: JsonPropertyName("B")] ulong Black);
}
