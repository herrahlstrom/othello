using FakeItEasy;
using FluentAssertions;
using Othello.Engine;
using Othello.Engine.AI;

namespace UnitTest.Othello.Engine;

[TestClass]
public class GameTests
{
    IAi _ai = A.Fake<IAi>();

    [TestMethod]
    public void CanPlaceStone_EmptyTable_ShouldAlwaysBeFalse()
    {
        var game = new Game(_ai);

        for (int i = 0; i < 64; i++)
        {
            var pos = Position.FromIndex(i);
            game.CanPlaceStone(pos).Should().BeFalse();
        }
    }

    [TestMethod]
    public void CanPlaceStone_StartTable_BlackShouldBeAbleToPlaceOnFourPositions()
    {
        var game = new Game(_ai);
        game.InitGame(PlayerColor.Black);

        var expected = new[] {
            new Position(4, 2).Index,
            new Position(5, 3).Index,
            new Position(2, 4).Index,
            new Position(3, 5).Index};

        // act
        var actual = GetAvailablePositions(game);

        // assert
        actual.Should().BeEquivalentTo(expected);
    }

    [TestMethod]
    public void CanPlaceStone_StartTable_WhiteShouldBeAbleToPlaceOnFourPositions()
    {
        var game = new Game(_ai);
        game.InitGame(PlayerColor.White);

        var expected = new[] {
            new Position(2, 3).Index,
            new Position(3, 2).Index,
            new Position(4, 5).Index,
            new Position(5, 4).Index};


        // act
        var actual = GetAvailablePositions(game);

        // assert
        actual.Should().BeEquivalentTo(expected);
    }

    private IEnumerable<int> GetAvailablePositions(Game game)
    {
        for (int i = 0; i < 64; i++)
        {
            var pos = Position.FromIndex(i);
            if (game.CanPlaceStone(pos))
            {
                yield return i;
            }
        }
    }
}
