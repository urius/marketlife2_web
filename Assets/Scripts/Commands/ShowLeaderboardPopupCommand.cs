using Events;
using GamePush;
using Infra.CommandExecutor;
using UnityEngine;

namespace Commands
{
    public class ShowLeaderboardPopupCommand : ICommand<UILeaderboardButtonClickedEvent>
    {
        public void Execute(UILeaderboardButtonClickedEvent e)
        {
            Debug.Log("execute ShowLeaderboardPopupCommand");
            
            GP_Leaderboard.Open(orderBy: "score", limit: 10, withMe: WithMe.first, displayFields: "score");
        }
    }
}