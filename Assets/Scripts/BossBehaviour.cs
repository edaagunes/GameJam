using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BossBehaviour : MonoBehaviour
{
    #region Public Variables

    public Transform rayCast;
    public LayerMask raycastMask;
    public float raycastLength;
    public float attackDistance; //minimum distance for attack
    public float moveSpeed;
    public float timer; //timer for cooldown between attacks

    public int maxHealth = 100; 
    
    #endregion

    #region Private Variables

    private RaycastHit2D hit;
    private GameObject target;
    private Animator anim;
    private float distance; //Store the distance b/w enemy and player
    private bool attackMode;
    private bool inRange; //Check if player is in range
    private bool cooling; //Check if enemy is cooling after attack
    private float intTimer;

    private int currentHealth;
    
    #endregion

    private void Awake()
    {
        intTimer = timer; //Store the inital value of timer
        anim = GetComponent<Animator>();

        currentHealth = maxHealth;

    }

    void Update()
    {
        if (inRange)
        {
            hit = Physics2D.Raycast(rayCast.position, Vector2.left, raycastLength, raycastMask);
            RaycastDebugger();

            //karakterin damage sıklığı ayarlanacak bu haliyle direkt ölüyor
            TakeDamage(20);

        }
        
        //When Player is Detected
        if (hit.collider != null)
        {
            EnemyLogic();
        }
        else if (hit.collider == null)
        {
            inRange = false;
        }

        if (inRange==false)
        {
            anim.SetBool("canWalk",false);
            StopAttack();
        }
        
    }

    private void TakeDamage(int damage)
    {
        currentHealth -= damage;

        //Hurt 
        anim.SetTrigger("Hurt");
        
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        anim.SetTrigger("Death");
        
        Destroy(gameObject,3f);
    }

    private void EnemyLogic()
    {
        distance = Vector2.Distance(transform.position, target.transform.position);

        if (distance > attackDistance)
        {
            Move();
            StopAttack();
        }
        else if (attackDistance>=distance && cooling == false)
        {
            Attack();
        }

        if (cooling)
        {
            CoolDown();
            anim.SetBool("Attack",false);
        }
    }

    void Move()
    {
        anim.SetBool("canWalk",true);
        
        if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
        {
            Vector2 targetPosition = new Vector2(target.transform.position.x, transform.position.y);
            transform.position = Vector2.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
        }
    }

    void Attack()
    {
        timer = intTimer; // Reset time when Player enter Attack Range
        attackMode = true; // To check if Enemy can still attack or not
        
        anim.SetBool("canWalk",false);
        anim.SetBool("Attack",true);
        
    }

    void CoolDown()
    {
        timer -= Time.deltaTime;

        if (timer<=0 && cooling && attackMode)
        {
            cooling = false;
            timer = intTimer;
        }
    }

    void StopAttack()
    {
        cooling = false;
        attackMode = false;
        anim.SetBool("Attack",false);
    }
    
    private void RaycastDebugger()
    {
        if (distance >attackDistance)
        {
            Debug.DrawRay(rayCast.position,Vector2.left * raycastLength,Color.red);
        }

        else if(attackDistance>distance)
        {
            Debug.DrawRay(rayCast.position,Vector2.left * raycastLength,Color.green);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            target = other.gameObject;
            inRange = true;
        }
    }

    public void TriggerCooling()
    {
        cooling = true;
        
    }
}
