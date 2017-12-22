using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardMonitorScript : MonoBehaviour
{

    Rigidbody rb;

    public bool debug = false;

    // Use this for initialization
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (debug)
        {
            Debug.Log("position: " + transform.position.ToString() + " true? " + (transform.position.y <= -1f));
        }
        if (transform.position.y <= -1f)
        {
            transform.position.Set(0f, 1f, 0f);
            transform.rotation.Set(0f, 0f, 0f, 0f);
        }
    }

    void FixedUpdate()
    {
        if (rb != null)
        {
            if (rb.velocity.magnitude < .001f)
            {
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
        }
    }
}
