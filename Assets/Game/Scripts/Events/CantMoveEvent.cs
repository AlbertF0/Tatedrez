using System;

namespace Events
{
    public class CantMoveEvent
    {
        public GameController.Team PlayerWithoutMoves;
        public GameController.Team NextPlayer;
        public Action<GameController.Team, GameController.Team> Callback;
    }
}
