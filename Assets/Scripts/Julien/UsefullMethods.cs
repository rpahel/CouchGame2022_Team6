using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class UsefullMethods 
{
    public static int GetPlayerIndex(GameObject playerGo)
    {
        var playerNameLastChar = playerGo.name[^1];
        var index = Convert.ToInt32(new string(playerNameLastChar, 1));
        return index;
    }
}
