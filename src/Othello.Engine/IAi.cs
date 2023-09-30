using System;
using System.Linq;

namespace Othello.Engine;

public interface IAi
{
    int GetIndex(IReadOnlyList<PlayerColor?> table, PlayerColor currentPlayer);
}
