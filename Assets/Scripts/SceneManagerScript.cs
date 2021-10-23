using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagerScript : MonoBehaviour
{
    bool sceneLoaded;
    public float delayNextSceneLoad;
    float timer;
    private void Start()
    {
            
    }
    private void Update()
    {
        if (timer < 0.1f)
        {
            timer += Time.deltaTime;

            if (!sceneLoaded)
            {
                if (TrailerPipeline.useTrailerSettingsImSerious)
                    delayNextSceneLoad = 5f;
                StartCoroutine(SceneLoadDelayed(delayNextSceneLoad));
                sceneLoaded = true;
            }
        }
    }
    IEnumerator SceneLoadDelayed(float t)
    {
        yield return new WaitForSecondsRealtime(t);
        SceneManager.LoadSceneAsync(1);
    }
}
