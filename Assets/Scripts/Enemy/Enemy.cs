using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;
using Vector2 = UnityEngine.Vector2;

public class Enemy : MonoBehaviour
{
    private Animator enemyAnim;
    private SpriteRenderer enemyRenderer;
    private Vector2[] cardinalDirections;
    private static readonly int Run = Animator.StringToHash("run");
    private bool chasingPlayer;
    private const int damageAmount = 10;
    private Vector2 moveDir;
    private const float delayTimer = 0.3f;
    private bool playerDetected;
    private float moveDistance;
    private bool attackTrigger;
    private float timer;

    private AIDetectPlayer aiScript;

    private void Awake()
    {
        aiScript = GetComponent<AIDetectPlayer>();
        enemyAnim = GetComponent<Animator>();
        enemyRenderer = GetComponent<SpriteRenderer>();
    }

    // Start is called before the first frame update
    void Start()
    {
        cardinalDirections = new Vector2[4];
        timer = 2f;
        CreateCardinalDirections();
        StartCoroutine(routine: PatrolCoroutine());
        if(aiScript != null)
            AIDetectPlayer.detectionEvent += AttackCoroutineEvent;
    }
    
    private void AttackCoroutineEvent()
    {
        if (!attackTrigger)
        {
            Debug.Log("Attack Coroutine Triggered");
            playerDetected = true;
            attackTrigger = true;
            StopAllCoroutines();
            StartCoroutine(AttackCoroutine());
        }
    }
    
    IEnumerator PatrolCoroutine()
    {
        yield return new WaitForSeconds(delayTimer);
        StartCoroutine(routine:Patrol());
    }

    IEnumerator Patrol()
    {
        if(playerDetected) yield break;
        float speed = Random.Range(1f, 3f);
        Vector2 direction = cardinalDirections[Random.Range(0, 4)].normalized;
        moveDir = direction;
        Vector2 targetDir = direction * Random.Range(1, 10);

        while (Vector2.Distance(transform.position, targetDir) > 0.1f)
        {
            transform.position = Vector2.MoveTowards(transform.position, targetDir, speed * Time.deltaTime);
            enemyAnim.SetBool(id:Run, true);
            FlipSprite(direction);
            yield return null;
        }
        enemyAnim.SetBool(id:Run, false);
        yield return new WaitForSeconds(Random.Range(1, 4));
        StartCoroutine(PatrolCoroutine());
    }

    IEnumerator AttackCoroutine()
    {
        Debug.Log("AttackCoroutine Started");
        yield return new WaitForSeconds(delayTimer);
        StartCoroutine(Attack());
    }

    IEnumerator Attack()
    {
        var targetPlayer = aiScript.Target;
        var chaseSpeed = 3f;
        Vector2 direction = (targetPlayer.transform.position - transform.position).normalized;
        moveDir = direction;

        while (Vector2.Distance(transform.position, targetPlayer.transform.position) > 1.5f)
        {
            transform.position = Vector2.MoveTowards(transform.position, targetPlayer.transform.position, chaseSpeed * Time.deltaTime);
            enemyAnim.SetBool(id:Run, true);
            FlipSprite(direction);
            yield return null;
        }

        while (Vector2.Distance(transform.position, targetPlayer.transform.position) <= 2f)
        {
            enemyAnim.SetBool(id:Run, false);
            DealDamage();
            yield return null;
        }
        
        StartCoroutine(AttackCoroutine());
    }

    private void DealDamage()
    {
        if (timer > 0f)
        {
            timer -= Time.deltaTime;
        }
        else
        {
            PlayerController.PlayerHealth -= damageAmount;
            Debug.Log(PlayerController.PlayerHealth);
            timer = 2f;
        }
    }

    private void FlipSprite(Vector2 facingDir)
    {
        enemyRenderer.flipX = moveDir.magnitude * Mathf.Sign(facingDir.x) < 0f;
    }

    private void CreateCardinalDirections()
    {
        for (int i = 0; i < cardinalDirections.Length; i++)
        {
            cardinalDirections[i] = CardinalDirections.direction[i];
        }
    }
}

public struct CardinalDirections
{
    public static readonly Vector2[] direction = { Vector2.up, Vector2.down, Vector2.left, Vector2.right };
}
