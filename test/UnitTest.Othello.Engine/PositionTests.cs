using FluentAssertions;
using Othello.Engine;

namespace UnitTest.Othello.Engine;


[TestClass]
public class PositionTests
{

    [TestMethod]
    public void FromIndex_ShouldBeCorrect()
    {
        for (int y = 0; y < 8; y++)
        {
            for (int x = 0; x < 8; x++)
            {
                var index = y * 8 + x;
                var pos = Position.FromIndex(index);

                pos.X.Should().Be(x);
                pos.Y.Should().Be(y);
            }
        }
    }
    [TestMethod]
    public void Index_ShouldBeCorrect()
    {
        for (int y = 0; y < 8; y++)
        {
            for (int x = 0; x < 8; x++)
            {
                var expected = y * 8 + x;

                var pos = new Position(x, y);
                pos.Index.Should().Be(expected);
            }
        }
    }

}