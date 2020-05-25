using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeactivateObjects : MonoBehaviour
{
    public GameObject[] gameObjects;

    private void Awake()
    {
        for (int i = 0; i < gameObjects.Length; i++)
        {
            if (gameObjects[i] != null)
                gameObjects[i].SetActive(false);
        }
    }
}
