using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwimmingStatePlayer : IStatePlayer
{
    public void OnEnter(Player player)
    {
        player.StartSwimmingState();
    }

    public void OnExecute(Player player)
    {
        player.UpdateSwimmingState();
    }

    public void OnExit(Player player)
    {
        player.ExitSwimmingState();
    }
}
