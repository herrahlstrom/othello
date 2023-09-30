using FluentAssertions;
using Othello.Engine;

namespace UnitTest.Othello.Engine;

[TestClass]
public class GameTests
{
    [TestMethod]
    public void CanPlaceStone_EmptyTable_ShouldAlwaysBeFalse()
    {
        var game = new Game();

        for (int i = 0; i < 64; i++)
        {
            game.CanPlaceStone(i).Should().BeFalse();
        }
    }

    [TestMethod]
    public void CanPlaceStone_StartTable_BlackShouldBeAbleToPlaceOnFourPositions()
    {
        var game = new Game();
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
        var game = new Game();
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
            if (game.CanPlaceStone(i))
            {
                yield return i;
            }
        }
    }
}
