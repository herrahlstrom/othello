using System.Text.Json;
using System.Text;
using System.Text.Json.Serialization;

namespace Othello.Engine;

public class GameTable : ISerialiable
{
    public readonly PlayerColor?[] _cells;

    public GameTable()
    {
        _cells = new PlayerColor?[64];
    }

    private GameTable(GameTable source)
        : this()
    {
        for (int i = 0; i < 64; i++)
        {
            _cells[i] = source._cells[i];
        }
    }

    public PlayerColor? this[int index] => _cells[index];
    public PlayerColor? this[Position pos] => _cells[pos.Index];

    public GameTable Copy() => new GameTable(this);

    public void InitNewGame()
    {
        for (int i = 0; i < 64; i++)
        {
            _cells[i] = null;
        }

        _cells[new Position(3, 3).Index] = PlayerColor.Black;
        _cells[new Position(4, 4).Index] = PlayerColor.Black;

        _cells[new Position(4, 3).Index] = PlayerColor.White;
        _cells[new Position(3, 4).Index] = PlayerColor.White;
    }
    public void Load(string data)
    {
        var bytes = Convert.FromBase64String(data);
        var json = Encoding.UTF8.GetString(bytes);
        var model = JsonSerializer.Deserialize<SerializeGameTable>(json)
            ?? throw new JsonException("Invalid json");

        for (int i = 0; i < _cells.Length; i++)
        {
            ulong value = (ulong)1 << i;
            if ((model.White & value) == value)
            {
                _cells[i] = PlayerColor.White;
            }
            else if ((model.Black & value) == value)
            {
                _cells[i] = PlayerColor.Black;
            }
        }
    }

    public int NumberOf(PlayerColor color) => _cells.Count(x => x == color);

    public void PlaceStone(Position pos, PlayerColor color)
    {
        if (_cells[pos.Index] != null)
        {
            throw new InvalidMoveException($"Position {pos} can't be placed, due its occupied!");
        }

        var stonesToFlip = Rules.GetFlippableStones(this, color, pos);
        _cells[pos.Index] = color;
        for (int i = 0; stonesToFlip > 0; i++)
        {
            if ((stonesToFlip & 1) == 1)
            {
                _cells[i] = color;
            }
            stonesToFlip >>= 1;
        }
    }

    public string Serialize()
    {
        ulong white = 0;
        ulong black = 0;

        for (int i = 0; i < 64; i++)
        {
            if (_cells[i] == PlayerColor.White)
            {
                white |= (ulong)1 << i;
            }
            else if (_cells[i] == PlayerColor.Black)
            {
                black |= (ulong)1 << i;
            }
        }

        var model = new SerializeGameTable(
            White: white,
            Black: black);

        var json = JsonSerializer.Serialize(model);
        var bytes = Encoding.UTF8.GetBytes(json);
        return Convert.ToBase64String(bytes);
    }

    private record SerializeGameTable(
        [property: JsonPropertyName("W")] ulong White,
        [property: JsonPropertyName("B")] ulong Black);
}
