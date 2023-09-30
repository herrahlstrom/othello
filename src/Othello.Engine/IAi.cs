using System;
using System.Linq;

namespace Othello.Engine;

public interface IAi
{
    int GetIndex(GameTable table, PlayerColor player);
}
