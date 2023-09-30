using Microsoft.AspNetCore.Components;
using Othello.Engine;
using System.Timers;
using System.Web;

namespace Othello.Pages;

public class Player
{
    private bool _ai;
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
    public string Name { get; }
    public PlayerColor Color { get; }

    public Player(PlayerColor color, string name)
    {
        Color = color;
        Name = name;
    }

    public event EventHandler AiActivated;
}

public partial class Game
{
    private const string TokenQueryName = "token";
    private System.Timers.Timer _aiDelay;
    HashSet<int> _errorCells = new();

    public Player WhitePlayer { get; }
    public Player BlackPlayer { get; }

    public Player CurrentPlayer => OthelloGame.CurrentPlayer == PlayerColor.White ? WhitePlayer : BlackPlayer;

    public Game()
    {
        _aiDelay = new(TimeSpan.FromSeconds(1)) { AutoReset = false };
        _aiDelay.Elapsed += AiDelayTimerElapsed;

        WhitePlayer = new Player(PlayerColor.White, "Vit");
        WhitePlayer.AiActivated += Player_AiActivated;

        BlackPlayer = new Player(PlayerColor.Black, "Svart");        
        BlackPlayer.AiActivated += Player_AiActivated;
    }

    private void Player_AiActivated(object? sender, EventArgs e)
    {
        if(sender == CurrentPlayer)
        {
            _aiDelay.Start();
        }
    }

    [Parameter, SupplyParameterFromQuery(Name = TokenQueryName)]
    public string? Token { get; set; }

    protected override void OnInitialized()
    {
        if (string.IsNullOrEmpty(Token))
        {
            OthelloGame.InitGame();
        }
        else
        {
            OthelloGame.LoadGame(Token);
        }
    }

    private void AiDelayTimerElapsed(object? sender, ElapsedEventArgs e)
    {
        OthelloGame.PlaceStoneWithAi();

        StateHasChanged();
    }

    private void PlaceStone(int index)
    {
        if (OthelloGame.CanPlaceStone(index))
        {
            _errorCells.Clear();
            OthelloGame.PlaceStone(index);

            SaveGameState();

            if (CurrentPlayer.Ai)
            {
                _aiDelay.Start();
            }
        }
        else
        {
            _errorCells.Add(index);
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

        _errorCells.Clear();
    }
}
