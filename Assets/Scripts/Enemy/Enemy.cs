using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;
using Vector2 = UnityEngine.Vector2;

public class Enemy : MonoBehaviour
{
    private Animator enemyAnim;
    private SpriteRenderer enemyRenderer;
    private Rigidbody2D enemyRB;
    private Vector2[] cardinalDirections;
    private static readonly int Run = Animator.StringToHash("run");
    private bool chasingPlayer;
    private int damageAmount = 10;
    private Transform playerPos;
    private Vector2 moveDir;
    private float damageTimer;
    private float enemySpeed = 3f;
    private const float delayTimer = 0.3f;
    private IEnumerator enumerator;
    private bool playerDetected;
    private const float offset = 2f;
    private float moveDistance;
    private bool canAttack;

    private AIDetectPlayer aiScript;

    private void Awake()
    {
        aiScript = GetComponent<AIDetectPlayer>();
        enemyAnim = GetComponent<Animator>();
        enemyRenderer = GetComponent<SpriteRenderer>();
        enemyRB = GetComponent<Rigidbody2D>();
    }

    // Start is called before the first frame update
    void Start()
    {
        cardinalDirections = new Vector2[4];
        damageTimer = 2f;
        canAttack = true;
        CreateCardinalDirections();
        StartCoroutine(routine: PatrolCoroutine());
        if(aiScript != null)
            AIDetectPlayer.detectionEvent += AttackCoroutineEvent;
    }

    private void Update()
    {
        playerDetected = aiScript.PlayerDetected;
    }

    IEnumerator PatrolCoroutine()
    {
        yield return new WaitForSeconds(delayTimer);
        enumerator = Movement();
        StartCoroutine(enumerator);
    }

    IEnumerator Movement()
    {
        if(playerDetected) yield break;
        float speed = Random.Range(1f, 3f);
        Vector2 direction = cardinalDirections[Random.Range(0, 4)];
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

    private void AttackCoroutineEvent()
    {
        StopCoroutine(enumerator);
        enumerator = Attack();
        if (canAttack)
        {
            StartCoroutine(enumerator);
        }
    }

    IEnumerator Attack()
    {
        Debug.Log("Attack Coroutine Called");
        var targetPlayer = aiScript.Target;
        var chaseSpeed = 3f;
        Vector2 direction = (targetPlayer.transform.position - transform.position).normalized;
        moveDir = direction;
        moveDistance = Vector2.Distance(transform.position, targetPlayer.transform.position);
        
        while (moveDistance > enemyRenderer.bounds.extents.x * offset)
        {
            transform.position = Vector2.MoveTowards(transform.position, targetPlayer.transform.position, chaseSpeed * Time.deltaTime);
            enemyAnim.SetBool(id:Run, true);
            FlipSprite(direction);
            yield return null;
        }
        enemyAnim.SetBool(id:Run, false);
        yield return new WaitForSeconds(3f);
        DealDamage();
        canAttack = true;
        StartCoroutine(enumerator);
    }

    private void DealDamage()
    {
        PlayerController.ReduceHealth(damageAmount);
        Debug.Log(PlayerController.PlayerHealth);
    }

    private void FlipSprite(Vector2 facingDir)
    {
        if (moveDir.magnitude * Mathf.Sign(facingDir.x) < 0f)
        {
            enemyRenderer.flipX = true;
        }
        else
        {
            enemyRenderer.flipX = false;
        }
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
