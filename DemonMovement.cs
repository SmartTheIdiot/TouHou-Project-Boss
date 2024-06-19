using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

public class DemonMovement : MonoBehaviour
{

    public Rigidbody2D demon;
    public shardSpawn shardspawn;
    public GameObject beamTime;
    public int maxSpeed = 10;
    public float health = 100;
    public float hpMax = 100;
    public bool running = false;
    public bool start = false;
    public bool stop = false;

    public bool teleporting = false;

    public GameObject tpSpot;

    public List<Sprite> deathExplosions = new();
    public List<Sprite> tpSprites = new();

    public Sprite deathBeams;
    public GameObject DeadBoom;

    public GameObject sheild;

    // Start is called before the first frame update
    void Start()
    {
        demon = GetComponent<Rigidbody2D>();
        beamTime.SetActive(false);
        shardspawn = GetComponent<shardSpawn>();
        StartCoroutine(AttackPattern());
    }

    // Update is called once per frame
    void Update()
    {
        if(health <= 0 && !stop)
        {
            gameObject.GetComponent<SpriteRenderer>().enabled = (false);

            for (int i = 0; i < 30; i++)
            {
               StartCoroutine(DeathThroes());
            }

        }
    }

    //141->227


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.gameObject.CompareTag("Player"))
        {
            if(collision.gameObject.GetComponentInParent<PlayerShots>().bulletChoice != PlayerShots.shots.lightning)
            {
                health -= collision.gameObject.GetComponent<PlayerShots>().Dmg;

                if (collision.gameObject.GetComponent<PlayerShots>().bulletChoice == PlayerShots.shots.shot)
                {
                    Destroy(collision.gameObject);
                }
            }
            else
            {
                health -= 5;
            }



        }
    }

    private IEnumerator AttackPattern()
    {
        if (health >= 1.1 && !teleporting)
        {
            if (!start)
            {
                StartCoroutine(DemonSpawn());
                start = true;
            }


            yield return new WaitUntil(() => !running);

            //Debug.Log("donerun");
            //gameObject.transform.position = new Vector3(0, 4, 0);

            //Debug.Log("Attack");
            if (Random.Range(0, 10) > 4)
            {
                //Debug.Log("shards");

                float vary = Random.Range(-30, 31);

                for (int l = 0; l < 5; l++)
                {
                    
                    StartCoroutine(shardspawn.spray(5, 10, 210 + vary));
                    StartCoroutine(shardspawn.spray(5, 10, 150 + vary));
                    shardspawn.shotgun(10, 20, 180 + vary);

                }
            }
            else
            {
                //Debug.Log("beam");
                beamTime.SetActive(true);
                yield return new WaitForSeconds(2f); /*Until(()=> beamTime.GetComponent<Beam>().run)*/
                beamTime.SetActive(false);
            }
            yield return new WaitForSeconds(1f);

            if(Random.Range(0, 10) > 2)
            {
                StartCoroutine(Teleport());
            }

            if(Random.Range(0, 10) > 7)
            {
                sheild.SetActive(true);
            }

            yield return StartCoroutine(AttackPattern());
        }
    }

    private IEnumerator Teleport()
    {
        teleporting = true;

        gameObject.GetComponent<SpriteRenderer>().enabled = false;

        Vector2 dest = new(Random.Range(-5, 5), Random.Range(4, 7));

        GameObject tpOut = Instantiate(tpSpot, dest, transform.rotation);
        GameObject tpIn = Instantiate(tpSpot, transform.position, transform.rotation);

        SpriteRenderer InRenderer = tpIn.GetComponent<SpriteRenderer>();
        SpriteRenderer OutRenderer = tpOut.GetComponent<SpriteRenderer>();

        for (int i = 0; i < 5; i++)
        {
            InRenderer.sprite = tpSprites[i];
            InRenderer.color = Random.ColorHSV();
            OutRenderer.sprite = tpSprites[4 - i];
            OutRenderer.color = Random.ColorHSV();
            yield return new WaitForSeconds(0.1f);
        }

        Destroy(tpIn);
        Destroy(tpOut);

        gameObject.transform.position = dest;
        gameObject.GetComponent<SpriteRenderer>().enabled = true;
        teleporting = false;
        StartCoroutine(AttackPattern());
    }

    private IEnumerator DemonSpawn()
    {
        running = true;

        gameObject.transform.position = new Vector3(-4, 13, 0);

        yield return new WaitForSeconds(1);


        demon.AddRelativeForce(new Vector2(0f, -50f));

        yield return new WaitUntil(() => gameObject.transform.position.y <= 4f);

        demon.velocity = new Vector2(0f, 0f);
        gameObject.transform.position = new Vector3(-4, 4, 0);

        yield return new WaitForSeconds(0.3f);

        demon.AddRelativeForce(new Vector2(50f, 0f));

        yield return new WaitUntil(() => gameObject.transform.position.x >= 0f);

        demon.velocity = new Vector2(0f, 0f);
        gameObject.transform.position = new Vector3(0, 4, 0);

        running = false;
    }

    private IEnumerator DeathThroes()
    {
        stop = true;

        GameObject kablooey = Instantiate(DeadBoom, new Vector3(gameObject.transform.position.x + Random.Range(-3, 3), gameObject.transform.position.y + Random.Range(-3, 3)), Quaternion.Euler(gameObject.transform.eulerAngles + new Vector3(0,0,Random.Range(-90, 90))));

        
        
        SpriteRenderer spriteRenderer = kablooey.GetComponent<SpriteRenderer>();
        
        kablooey.transform.localScale *= (Random.Range(1, 5));

        for (int z = 0; z < 7 ; z++)
        {
            spriteRenderer.sprite = deathExplosions[z];
            spriteRenderer.color = Random.ColorHSV();
            yield return new WaitForSeconds(0.1f);
        }

        kablooey.SetActive(false);

    }
}
