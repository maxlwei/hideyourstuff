using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadBob : MonoBehaviour
{
    float spinTimer = 0.5f;
    float headSpinUpDownSpeed = 55f;
    [SerializeField]
    bool headSpinUp = true;
    bool headSpinDown = false;
    bool headPause = false;
    public float spinSpeed;

    bool headBobUp = true;

    float changeDirectionTimer = 2.5f;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        spinTimer -= Time.deltaTime ;
        changeDirectionTimer -= Time.deltaTime;

        HeadBobbing();
        HeadSpinning();
        //changeHeadDirection();
    }

    void HeadBobbing()
    {
        if (spinTimer < 0)
        {
            headBobUp = !headBobUp;
        }
        if (headBobUp)
        {
            this.transform.Translate(0, 0.0025f, 0);
        } else
        {
            this.transform.Translate(0, -0.0025f, 0);
        }
    }
    
    void HeadSpinning()
    {
        if (headPause)
        {
            if (spinTimer < 0)
            {
                headPause = false;
                spinTimer = 1f;
                if (headSpinUp)
                {
                    headSpinUp = false;
                    headSpinDown = true;
                } else
                {
                    headSpinUp = true;
                    headSpinDown = false;
                }
            }
        } else
        {
            if (spinTimer < 0)
            {
                headPause = true;
                spinTimer = 1f;
            } else
            {
                if (headSpinUp)
                {
                    this.transform.Rotate(Vector3.up * Time.deltaTime * -headSpinUpDownSpeed);
                }

                if (!headSpinUp)
                {
                    this.transform.Rotate(Vector3.up * Time.deltaTime * headSpinUpDownSpeed);
                }
            }
        }
    }

    void changeHeadDirection()
    {
        if (changeDirectionTimer < 0)
        {
            changeDirectionTimer = 2.5f;

            float x = Random.Range(-20f, 20f);
            float y = Random.Range(-20f, 20f);
            Vector3 targetDir = new Vector3(x, y, 0);
            Vector3 newDir = Vector3.RotateTowards(transform.forward, targetDir, 1f, 1f);

            this.transform.Rotate(newDir);
        }
    }
}