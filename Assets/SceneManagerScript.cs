using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagerScript : MonoBehaviour
{
    bool sceneLoaded;
    private void Update()
    {
        if (!sceneLoaded)
        {
            StartCoroutine(SceneLoadDelayed(0.1f));
            sceneLoaded = true;
        }
    }
    IEnumerator SceneLoadDelayed(float t)
    {
        yield return new WaitForSeconds(t);
        SceneManager.LoadSceneAsync(1);
    }
}
