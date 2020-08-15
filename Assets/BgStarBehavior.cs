using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BgStarBehavior : MonoBehaviour
{
    public Animator starAnimAnim;
    public SpriteRenderer starAnimSpriteSimple;
    //public SpriteRenderer starAnimSpriteVertical;
    //public SpriteRenderer starAnimSpriteHorizontal;
    //public SpriteRenderer starIdleSprite;
    //public Animator starIdleAnim;

    private void Awake()
    {
        //starIdleSprite.color = new Color(1, 1, 1, 0);
    }

    //public void ApplyColorToIdle(Color c)
    //{
    //    starIdleSprite.color = new Color(c.r, c.g, c.b, starIdleSprite.color.a);
    //}
    public void ApplyColorToAnim(Color color)
    {
        starAnimSpriteSimple.color = color;
        //starAnimSpriteVertical.color = starAnimSpriteHorizontal.color = color;
    }
}
