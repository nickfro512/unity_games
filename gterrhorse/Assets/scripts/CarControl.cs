﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AxleInfo
{
    public WheelCollider leftWheel;
    public WheelCollider rightWheel;
    public bool motor;
    public bool steering;
}


public class CarControl : MonoBehaviour {

    private Rigidbody rb;

    public ParticleSystem The_parts;
    public float speed;
    public int stop_limit;

    private AudioSource source;
    private AudioClip the_clip;

    private float destroy_time;
    private int stopped_count;
    private bool correcting;
    private float correcting_time;
    private float flip_correction = 0f;


    public List<AxleInfo> axleInfos;
    public float maxMotorTorque;
    public float maxSteeringAngle;

    // Use this for initialization
    void Start()
    {
        source = GetComponent<AudioSource>();
        the_clip = GetComponent<AudioClip>();

        rb = GetComponent<Rigidbody>();

        destroy_time = 0;
        stopped_count = 0;

        correcting = false;
        correcting_time = 0;
    }

    // finds the corresponding visual wheel
    // correctly applies the transform
    public void ApplyLocalPositionToVisuals(WheelCollider collider)
    {
        if (collider.transform.childCount == 0)
        {
            return;
        }

        Transform visualWheel = collider.transform.GetChild(0);

        Vector3 position;
        Quaternion rotation;
        collider.GetWorldPose(out position, out rotation);

        visualWheel.transform.position = position;
        visualWheel.transform.rotation = rotation;
        //visualWheel.transform.rotation = new Quaternion(rotation.x, rotation.y, rotation.z, 0);
    }

    public void FixedUpdate()
    {
        float motor = maxMotorTorque;
        float steering = maxSteeringAngle * Input.GetAxis("Horizontal");

        if (!correcting)
        {
            foreach (AxleInfo axleInfo in axleInfos)
            {
                if (axleInfo.steering)
                {
                    axleInfo.leftWheel.steerAngle = steering;
                    axleInfo.rightWheel.steerAngle = steering;
                }
                if (axleInfo.motor)
                {
                    axleInfo.leftWheel.motorTorque = motor;
                    axleInfo.rightWheel.motorTorque = motor;
                }
                ApplyLocalPositionToVisuals(axleInfo.leftWheel);
                ApplyLocalPositionToVisuals(axleInfo.rightWheel);
            }

            if (rb.velocity.x < .1f && rb.velocity.z < .1f)
            {
                stopped_count++;
                if (stopped_count > stop_limit)
                {
                    stopped_count = 0;

                    /*
                    if (Mathf.Abs(transform.eulerAngles.z) > 160f)
                    {
                        flip_correction = -(transform.eulerAngles.z);
                    }
                    */
                    
                    //transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y + 180.0f, transform.eulerAngles.z + flip_correction); stopped_count = 0;

                    //rb.AddForce(transform.forward * -500 * Time.deltaTime);
                    //rb.AddRelativeForce(transform.up * 50000);
                    Debug.Log("correcting");

                    correcting_time = Time.time + 5f;
                    correcting = true;
                }
            }
            else
            {
                stopped_count = 0;
                rb.AddForce(transform.forward * speed * Time.deltaTime);
            }
            // BAD HACK
            if (transform.position.y < -15)
            {
                //transform.position = new Vector3(transform.position.x, 10, transform.position.z);
                //Destroy(gameObject);
            }
        }

        else    // correcting
        {
            Debug.Log("in the correction");
            rb.AddForce(transform.forward * -1);
            //rb.AddTorque(new Vector3(100, 0, 0));
        }
    }

    private void Update()
    {   
        



        /*if (destroy_time != 0 && Time.time > destroy_time)
        {
            Destroy(gameObject);
        }*/
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == "player_weapon")
        //if (collision.collider.tag == "player_weapon" || transform.position.y < -3000) ;
        {
            Instantiate(The_parts, transform.position, transform.rotation);
            source.PlayOneShot(the_clip);
            Destroy(gameObject);
        }
        //destroy_time = Time.time + .1f;
       


    }
}