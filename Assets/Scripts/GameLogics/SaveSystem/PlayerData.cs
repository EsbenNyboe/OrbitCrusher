using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public int lvl;
    public bool easyMode;
    public bool[] lvlsWonEasy;
    public bool[] lvlsWon;
    public bool[] lvlsWonFullHp;
    public bool[] lvlsWonZeroDmg;

    public PlayerData(Player player)
    {
        lvl = player.lvl;
        easyMode = player.easyMode;
        lvlsWonEasy = player.lvlsWonEasy;
        lvlsWon = player.lvlsWon;
        lvlsWonFullHp = player.lvlsWonFullHp;
        lvlsWonZeroDmg = player.lvlsWonZeroDmg;
    }
}

