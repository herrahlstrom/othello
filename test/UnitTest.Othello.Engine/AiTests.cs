using Othello.Engine;
using Othello.Engine.AI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTest.Othello.Engine;

[TestClass]
public class AiTests
{
    [TestMethod]
    public void AI_Start_Someting()
    {
        var ai = new PredictiveAi();

        var table = new Game(ai);
        table.InitGame(PlayerColor.White);

        var index = ai.GetIndex(table.Table, table.CurrentPlayer);
    }
}
