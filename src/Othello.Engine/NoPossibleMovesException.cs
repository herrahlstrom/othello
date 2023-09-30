using System;
using System.Linq;

namespace Othello.Engine;
public class NoPossibleMovesException : Exception
{
    public NoPossibleMovesException()
    {
    }
}
