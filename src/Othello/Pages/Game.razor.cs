using Microsoft.AspNetCore.Components;
using Othello.Engine;
using System;
using System.Timers;
using System.Web;

namespace Othello.Pages;

public partial class Game
{
    private const string TokenQueryName = "token";
    private System.Timers.Timer _aiDelay;
    HashSet<int> _errorCells = new();
    Exception? _exception = null;

    public Game()
    {
        _aiDelay = new(TimeSpan.FromSeconds(1)) { AutoReset = false };
        _aiDelay.Elapsed += AiDelayTimerElapsed;

        WhitePlayer = new Player(PlayerColor.White, "Vit");
        WhitePlayer.AiActivated += Player_AiActivated;

        BlackPlayer = new Player(PlayerColor.Black, "Svart");
        BlackPlayer.AiActivated += Player_AiActivated;
    }

    public Player BlackPlayer { get; }

    public Player CurrentPlayer => Players.First(x => x.Color == OthelloGame.CurrentPlayer);

    public IEnumerable<Player> Players
    {
        get
        {
            yield return WhitePlayer;
            yield return BlackPlayer;
        }
    }

    [Parameter, SupplyParameterFromQuery(Name = TokenQueryName)]
    public string? Token { get; set; }

    public Player WhitePlayer { get; }

    protected override void OnInitialized()
    {
        if (string.IsNullOrEmpty(Token))
        {
            OthelloGame.InitGame();
        }
        else
        {
            OthelloGame.Load(Token);
        }

        AfterStonePlaced();
    }

    private void AfterStonePlaced()
    {
        _errorCells.Clear();

        SaveGameState();

        WhitePlayer.Points = OthelloGame.Table.NumberOf(WhitePlayer.Color);
        BlackPlayer.Points = OthelloGame.Table.NumberOf(BlackPlayer.Color);

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

            OthelloGame.PlaceStoneWithAi();

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
            if (OthelloGame.CanPlaceStone(pos))
            {
                OthelloGame.PlaceStone(pos);

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

    private void SaveGameState()
    {
        Token = OthelloGame.Serialize();

        var uri = NavMan.ToAbsoluteUri(NavMan.Uri);
        var queryCollection = HttpUtility.ParseQueryString(uri.Query);
        queryCollection[TokenQueryName] = Token;

        var uriBuilder = new UriBuilder(uri);
        uriBuilder.Query = queryCollection.ToString();

        NavMan.NavigateTo(uriBuilder.Uri.AbsoluteUri, new NavigationOptions() { ForceLoad = false, ReplaceHistoryEntry = true });
    }

    private void StartNewGame()
    {
        OthelloGame.InitGame();

        AfterStonePlaced();
    }
}

public class Player
{
    private bool _ai;

    public Player(PlayerColor color, string name)
    {
        Color = color;
        Name = name;
    }

    public event EventHandler AiActivated;

    public bool Ai
    {
        get => _ai; set
        {
            _ai = value;

            if (value)
            {
                AiActivated?.Invoke(this, EventArgs.Empty);
            }
        }
    }
    public PlayerColor Color { get; }
    public string Name { get; }
    public int Points { get; set; }
}
