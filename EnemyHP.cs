using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHP : MonoBehaviour
{
    public Sprite HealthBar;
    public SpriteRenderer HealthBarRenderer;

    public GameObject attatchedEnemy;

    public EnemyClass enemyClass;
    public DemonMovement demon;

    float ratio;

    private void Start()
    {
        HealthBarRenderer.enabled = false;
    }

    private void Update()
    {
        

        if (attatchedEnemy.GetComponent<EnemyClass>() != null)
        {
            enemyClass = attatchedEnemy.GetComponent<EnemyClass>();
            ratio = enemyClass.hpRemain / enemyClass.hpMax;

            gameObject.transform.localScale = new Vector3(0.75f, 0.75f, 0.75f);
        }
        else
        {
            demon = attatchedEnemy.GetComponent<DemonMovement>();
            ratio = demon.health / demon.hpMax;
        }

        if (ratio < 1)
        {
            HealthBarRenderer.enabled = true;
        }
        if (ratio <= 0)
        {
            HealthBarRenderer.enabled = false;
        }
        
        

        if(ratio > 0)
        {
            gameObject.transform.localScale = new Vector3(ratio, 1, 1);
        }

        if (ratio >= 0.65)
        {
            HealthBarRenderer.color = Color.green;
        }
        else if (ratio >= 0.3)
        {
            HealthBarRenderer.color = Color.yellow;
        }
        else
        {
            HealthBarRenderer.color = Color.red;
        }
        
    }
}
