using System;
using System.Linq;

namespace Othello.Engine.AI;

public interface IAi
{
    Position GetIndex(GameTable table, PlayerColor player);
}
