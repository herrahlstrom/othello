using Microsoft.AspNetCore.Components;
using Othello.Engine;
using Othello.ViewModels;
using System;
using System.Timers;
using System.Web;

namespace Othello.Pages;

public partial class Game
{
    private readonly System.Timers.Timer _aiDelay;
    private readonly HashSet<int> _errorCells = new();
    private Exception? _exception = null;

    public Game()
    {
        _aiDelay = new(TimeSpan.FromSeconds(1)) { AutoReset = false };
        _aiDelay.Elapsed += AiDelayTimerElapsed;

        WhitePlayer = new Player(PlayerColor.White, "Vit");
        BlackPlayer = new Player(PlayerColor.Black, "Svart");
    }

    public Player BlackPlayer { get; }

    public Player CurrentPlayer => Players.First(x => x.Color == Model.CurrentPlayer);

    [Parameter]
    public Engine.Game Model { get; set; } = default!;

    [Parameter]
    public GameOptions Options { get; set; } = default!;

    public IEnumerable<Player> Players
    {
        get
        {
            yield return WhitePlayer;
            yield return BlackPlayer;
        }
    }

    public Player WhitePlayer { get; }

    protected override void OnInitialized()
    {
        AfterStonePlaced();
    }

    protected override void OnParametersSet()
    {
        WhitePlayer.Ai = Options.Ai == nameof(PlayerColor.White);
        BlackPlayer.Ai = Options.Ai == nameof(PlayerColor.Black);
    }

    private void AfterStonePlaced()
    {
        _errorCells.Clear();

        WhitePlayer.Points = Model.Table.NumberOf(WhitePlayer.Color);
        BlackPlayer.Points = Model.Table.NumberOf(BlackPlayer.Color);

        if (CurrentPlayer.Ai)
        {
            _aiDelay.Start();
        }

        StateHasChanged();
    }

    private void AiDelayTimerElapsed(object? sender, ElapsedEventArgs e)
    {
        try
        {
            if (sender == null)
            {
                throw new ArgumentNullException(nameof(sender), $"{nameof(sender)} is null.");
            }

            if (e == null)
            {
                throw new ArgumentNullException(nameof(e), $"{nameof(e)} is null.");
            }

            if (!CurrentPlayer.Ai) { return; }

            Model.PlaceStoneWithAi();

            AfterStonePlaced();
        }
        catch (Exception ex)
        {
            _exception = ex;
        }
        finally
        {
            StateHasChanged();
        }
    }

    private void PlaceStone(int index)
    {
        try
        {
            var pos = Position.FromIndex(index);
            if (Model.CanPlaceStone(pos))
            {
                Model.PlaceStone(pos);

                AfterStonePlaced();
            }
            else
            {
                _errorCells.Add(index);
            }
        }
        catch (Exception ex)
        {
            _exception = ex;
        }
        finally
        {
            StateHasChanged();
        }
    }

    private void Player_AiActivated(object? sender, EventArgs e)
    {
        if (sender == CurrentPlayer)
        {
            _aiDelay.Start();
        }
    }
}
