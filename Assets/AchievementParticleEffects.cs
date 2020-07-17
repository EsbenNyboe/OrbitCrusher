using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AchievementParticleEffects : MonoBehaviour
{
    public Achievements achievements;
    ParticleSystem[] achievementParticle;

    // Start is called before the first frame update
    void Start()
    {
        CreateArray();
    }

    private void CreateArray()
    {
        achievementParticle = new ParticleSystem[achievements.levelCount];
        int i = 0;
        foreach (Transform child in transform)
        {
            achievementParticle[i] = child.GetComponent<ParticleSystem>();
            i++;
        }
    }

    public void NewAchievement(int level)
    {
        if (gameObject.activeInHierarchy)
            StartCoroutine(DelayParticles(level));
    }

    IEnumerator DelayParticles(int level)
    {
        yield return new WaitForSeconds(2f);
        achievementParticle[level].Play();
        //achievementParticle[level].GetComponentInChildren<ParticleSystem>().Play();

    }





    public float space;
    public void ApplySpacing()
    {
        CreateArray();
        for (int i = 0; i < achievementParticle.Length; i++)
        {
            Vector3 p = achievementParticle[i].transform.localPosition;
            achievementParticle[i].transform.localPosition = new Vector3(i * space, p.y);
        }
    }
}
