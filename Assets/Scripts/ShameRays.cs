using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShameRays : MonoBehaviour
{
    public float fieldOfViewAngle = 80f;           // Number of degrees, centred on forward, for the enemy see.
    public bool objectInSight;                      // Whether or not an object is currently sighted.


    private UnityEngine.AI.NavMeshAgent nav;        // Reference to the NavMeshAgent component.
    private SphereCollider col;                     // Reference to the sphere collider trigger component.
    private Animator anim;                          // Reference to the Animator.
    private GameObject shame;                       // Reference to the player.


    void Awake()
    {
        // Setting up the references.
        nav = GetComponent<UnityEngine.AI.NavMeshAgent>();
        col = GetComponent<SphereCollider>();
        anim = GetComponent<Animator>();
        shame = GameObject.FindGameObjectWithTag("Shames");
    }


    void Update()
    {
        if (objectInSight == false)
        {
            shame.GetComponent<Renderer>().material.color = Color.green;
        }
    }


    void OnTriggerStay(Collider other)
    {

        Debug.Log("ontrigger");
        // If the player has entered the trigger sphere...
        if (other.gameObject == shame)
        {
            // By default the player is not in sight.
            objectInSight = false;

            // Create a vector from the enemy to the player and store the angle between it and forward.
            Vector3 direction = other.transform.position - transform.position;
            float angle = Vector3.Angle(direction, transform.forward);

            //Debug.Log("Triggered but not found");

            // If the angle between forward and where the player is, is less than half the angle of view...
            if (angle < fieldOfViewAngle)
            {
                RaycastHit hit;

                Debug.Log("Within angle");

                // ... and if a raycast towards the player hits something...
                bool isHit = Physics.Raycast(transform.position, (shame.transform.position - this.transform.position), out hit);
                Debug.DrawRay(this.transform.position, (shame.transform.position - this.transform.position));

                if (Physics.Raycast(transform.position, (shame.transform.position - this.transform.position), out hit))
                {
                    Debug.Log("Ray shooty tooty");
                    // ... and if the raycast hits the player...
                    if (hit.collider.gameObject == shame)
                    {
                        Debug.Log("Ray hitting");
                        // ... the player is in sight.
                        objectInSight = true;
                        // TODO: Create detection shader
                        shame.GetComponent<Renderer>().material.color = Color.red;
                    }
                }
            } else
            {
                //Debug.Log("Not Within angle");
            }
        }
    }


    void OnTriggerExit(Collider other)
    {
        // If the player leaves the trigger zone...
        if (other.gameObject == shame)
            // ... the player is not in sight.
            objectInSight = false;
    }


    float CalculatePathLength(Vector3 targetPosition)
    {
        // Create a path and set it based on a target position.
        UnityEngine.AI.NavMeshPath path = new UnityEngine.AI.NavMeshPath();
        if (nav.enabled)
            nav.CalculatePath(targetPosition, path);

        // Create an array of points which is the length of the number of corners in the path + 2.
        Vector3[] allWayPoints = new Vector3[path.corners.Length + 2];

        // The first point is the enemy's position.
        allWayPoints[0] = transform.position;

        // The last point is the target position.
        allWayPoints[allWayPoints.Length - 1] = targetPosition;

        // The points inbetween are the corners of the path.
        for (int i = 0; i < path.corners.Length; i++)
        {
            allWayPoints[i + 1] = path.corners[i];
        }

        // Create a float to store the path length that is by default 0.
        float pathLength = 0;

        // Increment the path length by an amount equal to the distance between each waypoint and the next.
        for (int i = 0; i < allWayPoints.Length - 1; i++)
        {
            pathLength += Vector3.Distance(allWayPoints[i], allWayPoints[i + 1]);
        }

        return pathLength;
    }
}