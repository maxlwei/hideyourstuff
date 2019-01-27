using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShameRays : MonoBehaviour
{
    public float fieldOfViewAngle = 80f;           // Number of degrees, centred on forward, for the enemy see.

    private float size = 0.3f;
    private UnityEngine.AI.NavMeshAgent nav;        // Reference to the NavMeshAgent component.
    private SphereCollider col;                     // Reference to the sphere collider trigger component.
    private Animator anim;                          // Reference to the Animator.

    private GameObject[] shames;                       // Reference to the shames.
    public bool objectInSight;                      // Whether or not an object is currently sighted.

    private List<Vector3[]> shameVertices;

    public Text scoreText;
    private int shameScore = 0;

    void Awake()
    {
        // Setting up the references.
        nav = GetComponent<UnityEngine.AI.NavMeshAgent>();
        col = GetComponent<SphereCollider>();
        anim = GetComponent<Animator>();
        shames = GameObject.FindGameObjectsWithTag("Shames");
        shameVertices = new List<Vector3[]>();
        foreach (var shame in shames)
        {
            shameVertices.Add(shame.GetComponent<MeshFilter>().mesh.vertices);
            Debug.Log("shame vertices size: " + shameVertices.Count);
        }
        
    }


    void Update()
    {
        scoreText.text = "JUDGEMENT: " + shameScore.ToString();
    }


    void OnTriggerStay(Collider other)
    {
        // If the player has entered the trigger sphere...
        for (int i = 0; i < shames.Length; i++)
        {
            if (other.gameObject == shames[i])
            {
                //Debug.Log(shames[i].name);
                // By default the player is not in sight.
                objectInSight = false;
                shames[i].GetComponent<Renderer>().material.color = Color.green;

                // Create a vector from the enemy to the player and store the angle between it and forward.
                Vector3 direction = other.transform.position - transform.position;
                float angle = Vector3.Angle(direction, transform.forward);

                //Debug.Log("Triggered but not found");

                // If the angle between forward and where the player is, is less than half the angle of view...
                if (angle < fieldOfViewAngle / 2f)
                {
                    RaycastHit hit;

                    // get random vertice on the mesh
                    Vector3 target = shames[i].transform.TransformPoint(shameVertices[i][Random.Range(0, shameVertices[i].Length - 1)]);

                    // ... and if a raycast towards the player hits something...
                    bool isHit = Physics.Raycast(transform.position, (target - this.transform.position), out hit, 40f);
                    Debug.DrawRay(this.transform.position, (target - this.transform.position));

                    if (isHit)
                    {
                        //Debug.Log("Ray shooty tooty");
                        // ... and if the raycast hits the player...
                        if (hit.collider.gameObject == shames[i])
                        {
                            //Debug.Log("Ray hitting");
                            // ... the player is in sight.
                            objectInSight = true;
                            // TODO: Create detection shader
                            shames[i].GetComponent<Renderer>().material.color = Color.red;
                            shameScore += 1;
                        }
                    }
                }
                else
                {
                    shames[i].GetComponent<Renderer>().material.color = Color.green;
                }
            }
        }
    }


    void OnTriggerExit(Collider other)
    {
        foreach (GameObject shameinst in shames)
        {
            // If the player leaves the trigger zone...
            if (other.gameObject == shameinst)
            {
                // ... the player is not in sight.
                objectInSight = false;
                shameinst.GetComponent<Renderer>().material.color = Color.green;
            }
        }
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