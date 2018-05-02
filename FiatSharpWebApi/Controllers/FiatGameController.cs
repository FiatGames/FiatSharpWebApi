using FiatSharp;
using FiatSharp.Controllers;
using FiatSharp.Examples.TicTacToe;

namespace FiatSharpWebApi.Controllers
{
    //todo fix up these types to be for your game
    public class FiatGameController : FiatGameController<Settings,State,Move,Settings,State>
    {
        public override IFiatGame<Settings, State, Move, Settings, State> GetFiatGame() => new FiatGame();
    }
}
