
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class PlayerShots : MonoBehaviour
{
    //public 
    public shots bulletChoice;

    public CircleCollider2D lethalRocketRange;

    public new CapsuleCollider2D collider;

    public GameObject tendril;
    public GameObject zapSegment;

    public SpriteRenderer spriteRenderer;
    public Rigidbody2D rb;
    GameObject target;

    GameObject player;


    GameObject thisShot;

    GameObject closest;
    float distance = 0;

    Sprite[] zapies;

    public float Dmg;

    bool hit = false;

    public List<Sprite> sprites = new List<Sprite>()
    {
    };

    public List<Sprite> blast = new List<Sprite>()
    {
    };

    public List<Sprite> soul = new List<Sprite>() { };


    public List<GameObject> ENEMIES = new List<GameObject>()
    {

    };

    public List<GameObject> ZAPs = new();

    bool sigtard = true;
    public enum shots
    {
        shot,
        rocket,
        soulBlast,
        lightning,
        nuke
    }

    // Start is called before the first frame update
    void Start()
    {
        thisShot = gameObject;

        player = GameObject.FindGameObjectWithTag("Player");
        //tendril.SetActive(false);
        rb = GetComponent<Rigidbody2D>();

        if(bulletChoice == shots.rocket)
        {
            lethalRocketRange.enabled = true;
            spriteRenderer.sprite = sprites[1];
            Dmg = 4;
        }
        if(bulletChoice == shots.soulBlast)
        {
            collider.enabled = false;
            StartCoroutine(animateSoulShot());
            StartCoroutine(SoulShot());
            rb.AddRelativeForce(Vector3.up * 400);
            Dmg = 1;
        }
        if(bulletChoice == shots.shot)
        {
            rb.AddRelativeForce(Vector3.up * 500);
            Dmg = 1;
        }
        if(bulletChoice == shots.nuke)
        {
            gameObject.transform.localScale = new Vector3(3,3,3);
            spriteRenderer.sprite = sprites[7];
            rb.AddRelativeForce(Vector3.up * 500);
            lethalRocketRange.enabled = true;
            Dmg = 100;
        }
        if(bulletChoice == shots.lightning)
        {
            spriteRenderer.enabled = false;
            StartCoroutine(ChainLightning());
            Dmg = 10;
            //rb.AddRelativeForce(Vector3.up * 0);
        }
    }

    // Update is called once per frame
    void Update()
    {
        switch (bulletChoice)
        {
            case shots.shot:
                //shot();
                spriteRenderer.sprite = sprites[0];
                break;
            case shots.rocket:
                rocket();
                
                break;

            case shots.soulBlast:
                
                break;
            case shots.nuke:
                break;

        }

        foreach (GameObject go in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            if (!ENEMIES.Contains(go))
            {
                ENEMIES.Add(go);
            }
        }

        if(target == null && ENEMIES.Count > 0)
        {

                target = ENEMIES[Random.Range(0, ENEMIES.Count)];
            
        }

        
        foreach (GameObject enemy in ENEMIES)
        {
            if(FindDistance(enemy, gameObject) > distance)
            {
                distance = FindDistance(enemy, gameObject);
                closest = enemy;
            }
        }

        if(lethalRocketRange.enabled == true && CheckClose(closest, 2))
        {
            StartCoroutine(Blast());
        }
    }


    public IEnumerator Blast()
    {
        lethalRocketRange.enabled = true;

        rb.velocity = Vector2.zero;
        rb.freezeRotation = true;
        rb.constraints = 0;
        rb.Sleep();
        spriteRenderer.color = Color.magenta;

        if(bulletChoice != shots.lightning)
        {
            gameObject.transform.localScale = gameObject.transform.localScale * 1;
        }
        if(bulletChoice == shots.nuke)
        {
            for(int i = 0; i< 100; i++)
            {
                gameObject.transform.localScale = gameObject.transform.localScale * 2;
            }
            
        }       
        if (bulletChoice == shots.rocket && sigtard)
        {
            gameObject.transform.localScale = gameObject.transform.localScale * 2;
            sigtard = false;
        }


        for(int i = 0;i < blast.Count; i++)
        {
            spriteRenderer.sprite = blast[i];
            yield return new WaitForSeconds(0.1f);
        }

        Destroy(gameObject);
    }

    //void shot()
    //{
        
    //}

     void rocket()
    {
        if (!hit)
        {
            rb.AddRelativeForce(Vector3.up * 10);
        }
        for(int i = 0; i < ENEMIES.Count; i++)
        {
            if (CheckClose(ENEMIES[i], 2))
            {
                StartCoroutine(Blast());
            }
        }


    }

    private IEnumerator animateSoulShot()
    {
        for (int i = 0; i <  soul.Count ; i++)
        {
            spriteRenderer.sprite = soul[i];
            yield return new WaitForSeconds(0.1f);
        }

        yield return StartCoroutine(animateSoulShot());
    }

    IEnumerator SoulShot()
    {


        foreach(GameObject enemy in ENEMIES)
        {
            if(CheckClose(enemy, 3))
            {
                GameObject Tent = Instantiate(tendril, thisShot.gameObject.transform);

                List<GameObject> Tents = new();

                Tents.Add(Tent);

                for(float i = 0; i < 5; i += 0.5f) 
                {
                    Tent.transform.localEulerAngles = new Vector3(0, 0, AngleToPoint(enemy, thisShot));
                    Tent.transform.localScale = new Vector3(0.5f, FindDistance(thisShot, enemy), 0.5f);
                    yield return new WaitForEndOfFrame();
                }
                Destroy(Tent);
            }
        }

        yield return new WaitForSeconds(0.01f);
        yield return StartCoroutine(SoulShot());
    }

    IEnumerator ChainLightning()
    {
        Rigidbody2D rigid = gameObject.GetComponent<Rigidbody2D>();

        rigid.constraints = RigidbodyConstraints2D.FreezeAll;


        if(ENEMIES != null && target != null){
            target = ENEMIES[Random.Range(0, ENEMIES.Count)];
        }

        foreach(Collider2D collider in gameObject.GetComponents<Collider2D>())
        {
            collider.enabled = false;
        }
        

        GameObject ZAP = Instantiate(zapSegment, gameObject.transform);

        GameObject lastZap;
        GameObject kachow;
        zapies = Resources.LoadAll<Sprite>("Assets/Resources/Zaps");

        ZAPs.Add(ZAP);

        for(int i = 1;i < 30; i++)
        {
            lastZap = ZAPs[i - 1];
            kachow = Instantiate(zapSegment, lastZap.transform);
            ZAPs.Add(kachow );

            gameObject.transform.eulerAngles = new Vector3(0, 0, 90);

            kachow.transform.localScale = new Vector3(1,1,1);
            
            kachow.transform.localPosition = new Vector3(0.3f, 0, 0);

            if (i % 3 == 0)
            {
                kachow.transform.localEulerAngles = new Vector3(0, 0, AngleToPoint(target, kachow));
                //kachow.GetComponent<SpriteRenderer>().color = Color.red;
            }
            else
            {
                kachow.transform.localEulerAngles = new Vector3(0, 0, Random.Range(-45, 46));
            }


            Debug.Log(zapies);
            //kachow.GetComponent<SpriteRenderer>().sprite = zapies[Random.Range(0,zapies.Length)];

            //if(i > 3)
            //{
            //    lastZap.GetComponent<SpriteRenderer>().enabled = false;
            //}

            kachow = null;

            yield return new WaitForSeconds(0.01f);
        }

        foreach(GameObject bolt in ZAPs) {
            bolt.GetComponent<SpriteRenderer>().enabled = false;
            yield return new WaitForSeconds(0.01f);
        }

        Destroy(gameObject);

        ZAPs = new List<GameObject>();
    }

    bool CheckClose(GameObject enemy, float range)
    {
        if (enemy != null)
        {
            if ((enemy.transform.position.x <= thisShot.transform.position.x + range &&
            enemy.transform.position.x >= thisShot.transform.position.x - range) &&
            (enemy.transform.position.y <= thisShot.transform.position.y + range &&
            enemy.transform.position.y >= thisShot.transform.position.y - range))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
         else { return false; }
    }

    float AngleToPoint(GameObject toPoint, GameObject fromPoint)
    {
        float angle;
        float yDis = 0;
        float xDis;

        if (toPoint != null && player != null && thisShot != null)
        {
            yDis = toPoint.transform.position.x - (/*fromPoint.GetComponentInParent<Transform>().transform.position.x +*/ fromPoint.transform.position.x);
            xDis = toPoint.transform.position.y - (/*player.transform.position.y +*/ fromPoint.transform.position.y);

            angle = (float)(Mathf.Rad2Deg * (Mathf.Atan(yDis/xDis)));
        }
        else
        {
            angle = 0;
        }


        //if(yDis < 0) //if below
        //{
        //    angle += 180f;
        //}
        //if(toPoint.transform.position.x > (player.transform.position.x + thisShot.transform.position.x))
        //{
        //    angle *= -1;
        //}

        //Debug.Log(angle);

        if(yDis < 0)
        {
            return angle + 90;
        }
        else
        {
            return angle - 90;
        }
    }

    
    float FindDistance(GameObject from, GameObject to)
    {
        float Dis = 0;

        if(from != null && to != null)
        {
            float yDis = to.transform.position.y - from.transform.position.y;
            float xDis = to.transform.position.x - from.transform.position.x;

            Dis = (float)Mathf.Sqrt( ((int)yDis ^ 2) + ((int)xDis ^ 2) );
            //Debug.Log(Dis);
        }

        return Dis;
    }
    
}

