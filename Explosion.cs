using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    //96px

    public Sprite[] explode;
    public SpriteRenderer sprite;

    

    // Start is called before the first frame update
    void Start()
    {
        

        sprite = gameObject.GetComponent<SpriteRenderer>();
        //sprite.sprite. = "Explosion_0";
    }

    public void startThingy(GameObject gameObject)
    {
        StartCoroutine(explodeAnimation(gameObject));
    }

    private IEnumerator explodeAnimation(GameObject toKill)
    {
        for(int i = 0; i < explode.Length; i++)
        {
            sprite.sprite = explode[i];
            yield return new WaitForSeconds(0.1f);
        }
        

        Destroy(toKill);
    }
}
