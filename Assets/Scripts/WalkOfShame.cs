using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class WalkOfShame : MonoBehaviour
{
    //public GameObject parentWayPoints;
    //private List<GameObject> wayPointList;
    public GameObject[] wayPointList;
    int currentWayPoint = 0;
    public GameObject entranceWayPoint;
    public GameObject exitWayPoint;
    GameObject targetWayPoint;

    public Canvas gameOverCanvas;
    public Canvas scoreCanvas;

    public int numberOfMoves = 7;
    public float turnSpeed = 4f;
    public float walkSpeed = 4f;

    public float delayBeforeStart = 2;
    public float waitDelay = 2;
    float waitTimer;

    bool walkedBackToEntrance = false;
    
    private void Start()
    {
        /*
        foreach (Transform child in parentWayPoints.transform)
        {
            wayPointList.Add(child.gameObject);
        }
        */
        targetWayPoint = entranceWayPoint;
    }

    void Update()
    {
        waitTimer -= Time.deltaTime;
        if (delayBeforeStart > 0)
        {
            delayBeforeStart -= Time.deltaTime;
        } else
        {
            scoreCanvas.gameObject.SetActive(true);
            if (numberOfMoves > 0)
            {
                Walk();
            } else
            {
                FinalWalk();
            }
        }
    }

    void Walk()
    {
        if (waitTimer < 0)
        {
            transform.forward = Vector3.RotateTowards(transform.forward, targetWayPoint.transform.position - transform.position, turnSpeed * Time.deltaTime, 0.0f);
            transform.position = Vector3.MoveTowards(transform.position, targetWayPoint.transform.position, walkSpeed * Time.deltaTime);
        }
    }

    // Exit and entrance
    void FinalWalk()
    {
        if (walkedBackToEntrance == false)
        {
            targetWayPoint = entranceWayPoint;
            Walk();
        } else
        {
            targetWayPoint = exitWayPoint;
            Walk();
        }

    }

    private void OnTriggerStay(Collider collider)
    {
        if (targetWayPoint)
        {
            Collider target = targetWayPoint.GetComponent<Collider>();
            if (collider == target)
            {
                if (numberOfMoves < 1)
                {
                    if (target.gameObject.name == "Exit Waypoint")
                    {
                        TriggerGameOver();
                        return;
                    }
                    walkedBackToEntrance = true;
                } else
                {
                    SetNewWaypoint();
                }
                ResetTimer();
                numberOfMoves--;
            }
        }
    }

    void ResetTimer()
    {
        waitTimer = waitDelay;
    }
    
    void SetNewWaypoint()
    {
        int tmpWayPoint = currentWayPoint;
        while (tmpWayPoint == currentWayPoint)
        {
            tmpWayPoint = Random.Range(0, wayPointList.Length);
        }
        currentWayPoint = tmpWayPoint;
        targetWayPoint = wayPointList[currentWayPoint];
    }

    void TriggerGameOver()
    {
        gameOverCanvas.gameObject.SetActive(true);
    }
}
