using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Player : MonoBehaviour 
{
    public int lvl;

    public void SavePlayer()
    {
        SaveSystem.SavePlayer(this);
        print("save " + lvl);
    }
    public void LoadPlayer()
    {
        PlayerData data = SaveSystem.LoadPlayer();
        if (data.lvl < 1)
        {
            lvl = 0;
        }
        else
        {
            lvl = data.lvl;
        }

        print("load " + lvl);
    }
    
}
