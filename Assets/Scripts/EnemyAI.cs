using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    [SerializeField] private GameObject player;
    private Transform target;
    [HideInInspector] public NavMeshAgent navMeshAgent;
    public Animator animator;

    [SerializeField] private float viewRadius = 30f;
    [SerializeField] private float viewAngle = 90f;

    [SerializeField] private LayerMask obstacleMask;
    [SerializeField] private LayerMask playersLayer;


    private float pathUpdateTimer;
    private float targetHiddenTimer;

    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        CheckDetection();

        if (target != null)
        {
            bool isCloseToTarget = Vector3.Distance(transform.position, target.position) < navMeshAgent.stoppingDistance;
            if (isCloseToTarget)
            {
                LookAtTarget();
            }
            else
            {
                UpdatePath();
            }
        }
        float clampedSpeed = Mathf.Clamp(navMeshAgent.velocity.magnitude / navMeshAgent.speed, 0, 1);
        float logBase = 20.0f;
        float logSpeed = Mathf.Log(clampedSpeed * (logBase - 1) + 1, logBase);
        animator.SetFloat("speed", logSpeed);
    }

    private void LookAtTarget()
    {
        Vector3 direction = (target.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }

    private void UpdatePath()
    {
        if (pathUpdateTimer < 0.5f)
        {
            pathUpdateTimer += Time.deltaTime;
        }
        else
        {
            navMeshAgent.SetDestination(target.position);
            pathUpdateTimer = 0f;
        }
    }

    private void CheckDetection()
    {
        Collider[] playerInViewRadius = Physics.OverlapSphere(transform.position, viewRadius, playersLayer);

        Transform closestPlayer = null;
        float closestDistance = Mathf.Infinity;

        foreach (Collider playerCollider in playerInViewRadius)
        {
            Vector3 directionToPlayer = (playerCollider.transform.position - transform.position).normalized;
            if (Vector3.Angle(transform.forward, directionToPlayer) < viewAngle / 2)
            {
                float distanceToPlayer = Vector3.Distance(transform.position, playerCollider.transform.position);
                if (!Physics.Raycast(transform.position, directionToPlayer, distanceToPlayer, obstacleMask))
                {
                    if (distanceToPlayer < closestDistance)
                    {
                        closestDistance = distanceToPlayer;
                        closestPlayer = playerCollider.transform;
                    }
                }
            }
        }
        if (closestPlayer != null)
        {
            target = closestPlayer;
            UpdateTarget();
        }
        else target = null;

        if (target != null) HidePlayer();
    }

    private void UpdateTarget()
    {
        if (target != null)
        {
            if (targetHiddenTimer < 0.5f) return;
            targetHiddenTimer = 0f;
            Debug.Log("Player Found");
        }
        else
        {
            target = player.transform;
            Debug.Log("New Player Detected");
        }
    }
    private int lastLoggedSecond = -1;
    private void HidePlayer()
    {
        if (targetHiddenTimer < 5f)
        {
            targetHiddenTimer += Time.deltaTime;
            int currentSecond = Mathf.FloorToInt(targetHiddenTimer); // Convert to whole number

            if (currentSecond != lastLoggedSecond)
            {
                Debug.Log("Seconds left till hidden: " + (5 - currentSecond));
                lastLoggedSecond = currentSecond; // Update the last logged second
            }
        }
        else
        {
            Debug.Log("Player Hidden");
            target = null;
            navMeshAgent.ResetPath();
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, viewRadius);

        Vector3 viewAngleA = DirFromAngle(-viewAngle / 2, false);
        Vector3 viewAngleB = DirFromAngle(viewAngle / 2, false);

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + viewAngleA * viewRadius);
        Gizmos.DrawLine(transform.position, transform.position + viewAngleB * viewRadius);
    }

    private Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
        {
            angleInDegrees += transform.eulerAngles.y;
        }
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }
}
