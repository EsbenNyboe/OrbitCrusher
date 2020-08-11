using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CometColor : MonoBehaviour
{
    public Color cOutOfOrbit, cInOrbit, cEnraged;
    [Range(0, 1)]
    public float fadeToWhiteKey;

    public ParticleSystem trailOutOfOrbit;
    public ParticleSystem trailInOrbit;
    public ParticleSystem trailEnraged;
    public ParticleSystem trailBetweenPuzzles;

    void Start()
    {
        SetCometColors(trailOutOfOrbit, cOutOfOrbit);
        SetCometColors(trailInOrbit, cInOrbit);
        Color_OutOfOrbit();
    }

    public void Color_BetweenPuzzles()
    {
        trailBetweenPuzzles.Play();
        trailOutOfOrbit.Stop();
        trailInOrbit.Stop();
        trailEnraged.Stop();
    }
    public void Color_OutOfOrbit()
    {
        trailBetweenPuzzles.Stop();
        trailOutOfOrbit.Play();
        trailInOrbit.Stop();
        trailEnraged.Stop();
    }
    public void Color_InOrbit()
    {
        trailBetweenPuzzles.Stop();
        trailOutOfOrbit.Stop();
        trailInOrbit.Play();
        trailEnraged.Stop();
    }
    public void Color_Enraged()
    {
        if (!GameManager.inTutorial)
            trailEnraged.Play();
    }

    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Alpha1))
        //{
        //    Color_OutOfOrbit();
        //}
        //if (Input.GetKeyDown(KeyCode.Alpha2))
        //{
        //    Color_InOrbit();
        //}
        //if (Input.GetKeyDown(KeyCode.Alpha3))
        //{
        //    Color_Enraged();
        //}
    }






    private void SetCometColors(ParticleSystem ps, Color c)
    {
        var col = ps.colorOverLifetime;
        col.enabled = true;

        Gradient grad = new Gradient();
        grad.SetKeys(new GradientColorKey[] { new GradientColorKey(c, 0.0f), new GradientColorKey(Color.white, fadeToWhiteKey) }, new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(0.0f, 1.0f) });

        col.color = grad;
    }

    public void ApplyColors(Color inOrbit, Color outOfOrbit, Color enraged)
    {
        cInOrbit = inOrbit;
        cOutOfOrbit = outOfOrbit;
        cEnraged = enraged;
        SetCometColors(trailOutOfOrbit, cInOrbit);
        SetCometColors(trailInOrbit, cOutOfOrbit);
        SetCometColors(trailEnraged, cEnraged);
    }
}