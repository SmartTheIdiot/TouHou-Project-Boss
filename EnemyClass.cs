
using System.Collections;
using System.Collections.Generic;

using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class EnemyClass : MonoBehaviour
{
        public int StartHP = 1;
        public int speed = 10;

    public int sweeps = 0;
    public GameObject[] LOOT;

        public movement moveChoice;
        public projectile projectileChoice;

    public GameObject exploder;
    public GameObject dangerZone;
    public GameObject enemyShot;

    public float hpMax = 0;
    public float hpRemain = 5;


    SpriteRenderer spriteRenderer;

        public new Rigidbody2D rigidbody;

    public GameObject player;

    private bool bankai = false;
    public bool hit = false;


    public enum projectile
        {
            none,
            bullet,
            rocket,
            laser
        }

    public enum movement
    {
        charge,
        sweep,
        strike,
        hover
    }

    public void Start()
    {
    //transform.position = Vector3.zero;

        player = GameObject.FindWithTag("Player");
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();

    //rigidbody = gameObject.GetComponent<Rigidbody2D>();



        switch (moveChoice)
        {
            case movement.charge:
            gameObject.transform.position = new Vector2(Random.Range(-6, 6), 13);
                bankai = true;
                StartCoroutine(StartWithDelay(Charge(), Random.Range(0, 1f)));
                hpMax = 3;
            //InvokeRepeating("Charge", 0, 0.1f);

                break;

            case movement.sweep:
                StartCoroutine(Sweep());
                hpMax = 4;

                break;

            case movement.strike:
                StartCoroutine(Strike());
                gameObject.transform.position = new Vector2(Random.Range(-6, 6), 13);
                hpMax = 4;

            break;

            case movement.hover:
                Hover();

                break;
        }

        hpRemain = hpMax;
    }

    public void Update()
    {
        if(hpRemain <= 0) {
            explode();
            hit = true;
        }
    }

    /*private void OnCollisionEnter2D(Collision2D collision)
    {
        //.Log("hit");
            
            //Debug.Log("bankai");
            if (!collision.gameObject.CompareTag("Player"))
            {
            ////Debug.Log("Testing1"); // Player is meant to live 
                hit = true;

                //explode();
                
                //StartCoroutine(killDelay(collision));
               
            }
            else
            {

                Debug.Log("Testing2"); /// Testing 2 is confirmed where a player is meant to die 
                hit = true;
            DeathAnimation.Instance.StartCoroutine(DeathAnimation.Instance.Death());

            explode();
            }

        collision.gameObject.GetComponentInParent<PlayerShots>().ENEMIES.Remove(gameObject);
        
    }*/

    public IEnumerator spawnShot(GameObject toShoot, float angle = 0, int amount = 1, float timeMod = 0, float spreadMod = 1)    ///// where enemy bullets spawn / kill player
    {
        for(int i = 0; i < amount; i++)
        {
            GameObject shot = Instantiate(toShoot, gameObject.transform);

            shot.transform.eulerAngles += new Vector3(0, 0, angle + Random.Range(-5 * spreadMod, 5 * spreadMod));
            shot.GetComponent<Rigidbody2D>().AddForce(Vector3.down * 5);

            //amount--;
            yield return new WaitForSeconds(Random.Range(0.2f + (1 * timeMod), 0.34f + (1 * timeMod)));
        }        
    }

    public IEnumerator killDelay (Collision2D collision)
    {
        yield return new WaitForSeconds(0.1f);
        Destroy(collision.gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

            if (collision.gameObject.CompareTag("Shot"))
            {
                if (collision.gameObject.GetComponent<PlayerShots>() != null && collision.gameObject.GetComponent<PlayerShots>().bulletChoice != PlayerShots.shots.lightning)
                {
                    hpRemain -= collision.gameObject.GetComponent<PlayerShots>().Dmg;

                    if (collision.gameObject.GetComponent<PlayerShots>().bulletChoice == PlayerShots.shots.shot)
                    {
                        Destroy(collision.gameObject);
                    }
                }
                else
                {
                    //DeathAnimation.Instance.StartCoroutine(DeathAnimation.Instance.Death());
                    hpRemain -= 5;
                }



            }
        
        //else 
        //{//
        //    hit = true;
        //Debug.Log("player");
        //explode

        //    explode();
        //}


        //collision.gameObject.GetComponentInParent<PlayerShots>().ENEMIES.Remove(gameObject);

    }

    public void explode()
    {
        transform.position = transform.position;
        rigidbody.velocity = Vector3.zero;
        rigidbody.angularVelocity = 0f;
        //if (rigidbody != null)
        //{
            //rigidbody.gameObject.SetActive(false);
        //}
        
        transform.position = transform.position;
        exploder.SetActive(true);
        exploder.GetComponent<Explosion>().startThingy(this.gameObject);
        if (dangerZone != null)
        {
            dangerZone.SetActive(false);
        }
        ////////////////////////////////////////////////////////////////    ????
        ///











        
        /////Debug.Log(LOOT[Random.Range(0, LOOT.Length)] + " This is what was instantiated");
        SpawnLoot(this.gameObject);


        gameObject.GetComponent<SpriteRenderer>().enabled = false;
        rigidbody.angularVelocity = 0f;
    }

    void SpawnLoot(GameObject parentEnemy)
    {
        /// RANDOM.RANGE(0-100), IF # is 10< then LOOT3, if # <10, >60 spawn LOOT 2 else spawn loot 3
        ///
        ///
        int Number = Random.Range(0, 100);
        if (Number <= 10)
        {
            Instantiate(LOOT[0], parentEnemy.transform.position, Quaternion.identity);
        }
        else if (Number > 10 && Number <= 60)
        {
            Instantiate(LOOT[1], parentEnemy.transform.position, Quaternion.identity);
        }
        else
        {
            Instantiate(LOOT[2], parentEnemy.transform.position, Quaternion.identity);
        }
        ///Instantiate(LOOT[Random.Range(0, LOOT.Length)], parentEnemy.transform.position, Quaternion.identity); // 2ND PARAMETER transforms it to where we want 
       
    }


    public IEnumerator StartWithDelay(IEnumerator coroutine, float delay)
    {
        yield return new WaitForSeconds(delay);
        StartCoroutine(coroutine);
    }
 

     public IEnumerator Charge()    //// the red ones 
     {

        if (!hit)
        {
            float pX = player.transform.position.x; //player Y value
            float cX = gameObject.transform.position.x; //this X value

            float cY = gameObject.transform.position.y; //this Y value

            if(cY > -12)
            {

                if (pX > cX)
                {
                    rigidbody.AddForce(Vector2.right * 100);
                }
                else
                {
                    rigidbody.AddForce(Vector2.left * 100);
                }

                rigidbody.AddForce(Vector2.down * 250);
            }
            else
            {
               
                DestroyImmediate(gameObject);
            }
        }

        //Debug.Log("Charging");



        yield return new WaitForSeconds(0.1f);
        yield return StartCoroutine(Charge());

     } //good

     public IEnumerator Sweep()
        {
            Dictionary<float, Vector3[]> paths = 
                new Dictionary<float, Vector3[]>()
                {
                    {1, new Vector3[] { //left to right short
                        new Vector3(-5,13,0),//spawn
                        new Vector3(-5, 10, 0),//start curve
                        new Vector3(-2.5f, 8.5f, 0), //curve depth
                        new Vector3(0, 10, 0), //end curve
                        new Vector3(0, 13, 0) } //kill
                    },
                    {2, new Vector3[] { //left to right deep
                        new Vector3(-5,13,0),//spawn
                        new Vector3(-5, 6.5f, 0),//start curve
                        new Vector3(-2.5f, 5f, 0), //curve depth
                        new Vector3(0, 6.5f, 0), //end curve
                        new Vector3(0, 13, 0) } //kill
                    },
                    {3, new Vector3[] { //right to left short
                        new Vector3(5,13,0),//spawn
                        new Vector3(5, 10, 0),//start curve
                        new Vector3(2.5f, 8.5f, 0), //curve depth
                        new Vector3(0, 10, 0), //end curve
                        new Vector3(0, 13, 0) } //kill
                    },
                    {4, new Vector3[] { //right to left deep
                        new Vector3(5,13,0),//spawn
                        new Vector3(5, 6.5f, 0),//start curve
                        new Vector3(2.5f, 5, 0), //curve depth
                        new Vector3(0, 6.5f, 0), //end curve
                        new Vector3(0, 13, 0) } //kill
                    }
                };

            Vector3[] choice = paths[Random.Range(1, 4)];
            
            transform.position = choice[0];

            rigidbody.AddRelativeForce(Vector2.down * speed);

            yield return new WaitUntil(() => transform.position.y <= choice[1].y);

            rigidbody.velocity = Vector3.zero;
            transform.position = choice[1];

        transform.eulerAngles += new Vector3(0, 0, -45 * (choice[0].x / System.Math.Abs(choice[0].x)));

        yield return new WaitForSeconds(0.1f);
            rigidbody.AddRelativeForce(Vector2.down * speed);


            yield return new WaitUntil(() => (transform.position.x <= choice[2].x + 0.05f) && (transform.position.x >= choice[2].x - 0.05f));

        rigidbody.velocity = Vector3.zero;
        transform.position = transform.position;

        //spawnShot(enemyShot, 180, 5, 1.2f, 2f);
        StartCoroutine(spawnShot(enemyShot, Random.Range(150, 211), 5));

        transform.eulerAngles = new Vector3(0, 0, 0);

        transform.eulerAngles += new Vector3(0, 0, 45 * (choice[1].x / System.Math.Abs(choice[1].x)));

        yield return new WaitForSeconds(0.1f);
        rigidbody.AddRelativeForce(Vector2.up * speed);

        yield return new WaitUntil(() => (transform.position.x <= choice[3].x + 0.05f) && (transform.position.x >= choice[3].x - 0.05f));

        rigidbody.velocity = Vector3.zero; 
        transform.position = transform.position;

        

        transform.eulerAngles = new Vector3(0, 0, 0);

        yield return new WaitUntil(() => (transform.position.x <= 0.05f) && (transform.position.x >= -0.05f));

        rigidbody.AddRelativeForce(Vector3.up * speed);

        yield return new WaitUntil(() => ((transform.position.y < -13.05f) || (transform.position.y > 13.05f)) || ((transform.position.x > 8) || (transform.position.x < -8 )));

        //yield return StartCoroutine(Sweep());
        yield return new WaitForSeconds(3);
        Destroy(gameObject);

        } //good

    public IEnumerator Strike()
    {

        rigidbody.velocity = Vector2.down * 5;
        
        
        yield return new WaitUntil(() => (gameObject.transform.position.y <= player.transform.position.y + 2) && (gameObject.transform.position.y >= player.transform.position.y));

        
        rigidbody.velocity = Vector3.zero;
        transform.transform.position = transform.position;
        //rigidbody.Sleep();

        yield return new WaitForSeconds(0.1f);

        rigidbody.velocity += Vector2.up * 5;

        yield return new WaitUntil(() => (gameObject.transform.position.y >= player.transform.position.y + 5));

        rigidbody.velocity = Vector3.zero;
        transform.transform.position = transform.position;
        //rigidbody.Sleep();

        StartCoroutine(spawnShot(enemyShot, 180, 3));

        rigidbody.velocity += Vector2.up * 5;

        yield return new WaitUntil(() => (gameObject.transform.position.y >= 7));

        //sweeps++;

        //if(sweeps < 3)
        rigidbody.velocity = Vector2.down * 5;

        //{
        yield return StartCoroutine(Strike());
        //}
        //else
        //{
        //    spriteRenderer.color = Color.red;
        //    Charge();
        //}
        

    } //good

    public void Hover()
    {
        rigidbody.velocity = Vector3.right * speed;
    }   
}
