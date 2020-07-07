using System.Collections;
using System.Collections.Generic;
using System.Xml.Schema;
using TMPro;
using UnityEngine;

public class Player : MonoBehaviour 
{
    public int lvl;
    public bool[] lvlsWon;
    public bool[] lvlsWonFullHp;
    public bool[] lvlsWonZeroDmg;

    public bool unlockAllSilver;
    public bool unlockAllGold;
    public bool unlockAllDiamond;

    public void UnlockAll()
    {
        UnlockAllTrophies(unlockAllSilver, lvlsWon);
        UnlockAllTrophies(unlockAllGold, lvlsWonFullHp);
        UnlockAllTrophies(unlockAllDiamond, lvlsWonZeroDmg);
    }

    private void UnlockAllTrophies(bool unlockStatus, bool[] achievementData)
    {
        for (int i = 0; i < achievementData.Length; i++)
        {
            achievementData[i] = unlockStatus;
        }
    }

    public static bool savedGameAvailable;
    public void SavePlayer()
    {
        SaveSystem.SavePlayer(this);
    }
    public void LoadPlayer()
    {
        PlayerData data = SaveSystem.LoadPlayer();
        if (data == null)
        {
            savedGameAvailable = false;
        }
        else
        {
            savedGameAvailable = true;
        }
        lvl = data.lvl;
        lvlsWon = data.lvlsWon;
        lvlsWonFullHp = data.lvlsWonFullHp;
        lvlsWonZeroDmg = data.lvlsWonZeroDmg;
    }
    public void NewGame()
    {
        bool deleted = SaveSystem.DeleteSaveFile();
        //print("saved game deleted:" + deleted);
    }
}
