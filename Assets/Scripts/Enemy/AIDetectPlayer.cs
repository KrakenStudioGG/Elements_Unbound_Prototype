using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

/* Script Attached to EnemyObject to passively detect Collisions*/
public class AIDetectPlayer : MonoBehaviour
{

    public delegate void PlayerDetectedEvent();
    public static event PlayerDetectedEvent detectionEvent;
    public bool PlayerDetected { get; private set; }
    public Vector2 DirectionToTarget => target.transform.position - detectOrigin.position;

    private Transform detectOrigin;
    private float radiusSize = 5f;
    private Vector2 offset;
    private float detectionDelay = 0.3f;
    public LayerMask detectLayerMask;

    public Color gizmoIdleColor = Color.green;
    public Color gizmoDetectedColor = Color.red;
    public bool showGizmos = true;

    private GameObject target;

    public GameObject Target
    {
        get => target;
        private set
        {
            target = value;
            PlayerDetected = target != null;
        }
    }

    private void Start()
    {
        StartCoroutine(routine: DetectionCoroutine());
    }

    IEnumerator DetectionCoroutine()
    {
        yield return new WaitForSeconds(detectionDelay);
        PerformDetection();
        StartCoroutine(DetectionCoroutine());
    }

    public void PerformDetection()
    {
        Collider2D collider = Physics2D.OverlapCircle(transform.position, radiusSize, detectLayerMask);

        if (collider != null)
        {
            Target = collider.gameObject;
            detectionEvent?.Invoke();
        }
        else
        {
            Target = null;
        }
    }

    private void OnDrawGizmos()
    {
        if (showGizmos && transform.position != null)
        {
            Gizmos.color = gizmoIdleColor;
            if (PlayerDetected)
                Gizmos.color = gizmoDetectedColor;
            Gizmos.DrawWireSphere(transform.position, radiusSize);
        }
    }
}
