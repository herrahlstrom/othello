using System;
using System.Linq;

namespace Othello.Engine;

public class InvalidMoveException : OthelloException
{
    public InvalidMoveException(string message) : base(message)
    {
    }
}

public class NoPossibleMovesException : OthelloException
{
    public NoPossibleMovesException() : base("Current user has new possible moves")
    {
    }
}
public abstract class OthelloException : Exception
{
    public OthelloException(string message) : base(message)
    {
    }
}
