using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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
    public AudioSource footsteps;
    public AudioSource layer1;
    public AudioSource layer2;

    public Canvas gameOverCanvas;
    public Canvas scoreCanvas;
    public Text gameOverText;

    public ShameRays shameinfo;

    public int numberOfMoves = 7;
    public float turnSpeed = 4f;
    public float walkSpeed = 4f;

    public float delayBeforeStart = 2;
    public float waitDelay = 2;
    float waitTimer;

    float layer1volume = 0.0f;
    float layer2volume = 0.0f;
    bool walkedBackToEntrance = false;
    bool footstepplaying = false;

    
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
            fadeInLayer1(layer1);
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
            fadeInLayer2(layer2);
            if (!footstepplaying)
            {
                footsteps.Play();
                footstepplaying = true;
            }
            transform.forward = Vector3.RotateTowards(transform.forward, targetWayPoint.transform.position - transform.position, turnSpeed * Time.deltaTime, 0.0f);
            transform.position = Vector3.MoveTowards(transform.position, targetWayPoint.transform.position, walkSpeed * Time.deltaTime);
        } else
        {
            footsteps.Pause();
            footstepplaying = false;
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

        if (shameinfo.getShameScore() > 3000)
        {
            gameOverText.text = "SHAME ON YOU\r\n You dissapoint your mother.";
        }
         else
        {
            gameOverText.text = "You have escaped your mother's wrath. (She loves you!)";
        }
        gameOverCanvas.gameObject.SetActive(true);
    }

    void fadeInLayer1(AudioSource source)
    {
        if (source.volume < 1)
        {
            layer1volume += 0.1f * Time.deltaTime;
            source.volume = layer1volume;
        }
    }

    void fadeInLayer2(AudioSource source)
    {
        if (source.volume < 1)
        {
            layer2volume += 0.1f * Time.deltaTime;
            source.volume = layer2volume;
        }
    }

}
