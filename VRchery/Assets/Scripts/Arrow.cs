﻿using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    // ============================================================================ Variables
    // Public variables
    [Header("Arrow Physics")]
    public Rigidbody rigidBody;

    [Header("UI")]
    public TMP_Text text;

    [Header("Sound Effects")]
    public AudioSource arrowHit1;
    public AudioSource arrowHit2;
    public AudioSource arrowHit3;
    public AudioSource whoosh;

    // Private variables
    private bool isStopped = true;
    private float speed = 5.0f;
    private TMP_Text textObject = null;
    private bool hasCollided = false;

    // ============================================================================ Public methods
    // Method for firing an arrow
    public void FireArrow(float distance)
    {
        // Update movement variables
        isStopped = false;
        transform.parent = null;
        rigidBody.isKinematic = false;
        rigidBody.useGravity = true;

        // Apply a force
        rigidBody.AddForce(transform.forward * (speed * (distance / 5.0f)));

        // Play sound
        whoosh.volume = distance * 0.01f;
        whoosh.Play();
    }

    // ============================================================================ Private methods
    // Start is called before the first frame update
    void Start() { }

    // Update is called once per frame
    void Update() {
        // Alter text, if created
        if (textObject != null)
        {
            // Move text up
            textObject.transform.position += new Vector3(0.0f, 0.01f, 0.0f);

            // Change text opacity
            Color currentColor = textObject.color;
            textObject.color = new Color(currentColor.r, currentColor.g, currentColor.b, currentColor.a -= 0.005f);

            if (textObject.color.a <= 0.005f) Destroy(textObject);
        }
    }

    // Physics update
    void FixedUpdate()
    {
        // Check if stopped
        if (isStopped) return;

        // Apply rotation
        rigidBody.MoveRotation(Quaternion.LookRotation(rigidBody.velocity, transform.up));
    }

    //private void OnTriggerEnter(Collider other)
    //{
    //    // Stop movement
    //    Stop();

    //    // Check only first collision
    //    if (!hasCollided)
    //    {
    //        // Change boolean
    //        hasCollided = true;

    //        // Check points, but only if target hit
    //        int points = 0;
    //        if (other.CompareTag("Target"))
    //        {
    //            points = other.GetComponent<Target>().CheckPoints(gameObject.transform.position);
    //        }

    //        // Create text if not created
    //        textObject = Instantiate(text, gameObject.transform);
    //        textObject.SetText(points.ToString());
    //        textObject.transform.rotation = Quaternion.Euler(new Vector3(0.0f, -90.0f, 0.0f));

    //        Debug.Log(points);
    //    }
    //}

    private void OnCollisionEnter(Collision collision)
    {
        // Ignore arrows
        if (collision.collider.tag == "Arrow")
        {
            Physics.IgnoreCollision(collision.collider, GetComponentInChildren<Collider>());
            return;
        }

        // Stop movement
        Stop();


        // Play sound effect based on target
        if (collision.collider.tag == "Target") arrowHit1.Play();
        else if (collision.collider.tag == "Ground") arrowHit3.Play();
        else arrowHit2.Play();

        // Check only first collision
        if (!hasCollided)
        {
            // Change boolean
            hasCollided = true;

            // Check points, but only if target hit
            int points = 0;
            if (collision.collider.tag == "Target")
            {
                points = 1 + collision.collider.GetComponent<Target>().CheckPoints(collision);
            }

            // Create text if not created and rotate to look at the camera
            textObject = Instantiate(text, gameObject.transform);
            textObject.SetText(points.ToString());
            textObject.transform.LookAt(FindObjectOfType<OVRPlayerController>().transform.position);
            textObject.transform.rotation *= Quaternion.Euler(new Vector3(0.0f, -180.0f, 0.0f));

            Debug.Log(points);

            // Move forward a little for better embedding
            gameObject.transform.position += gameObject.transform.forward * 0.25f;
        }
    }

    // Stop movement on contact
    private void Stop()
    {
        // Update movement variables
        isStopped = true;
        rigidBody.isKinematic = true;
        rigidBody.useGravity = false;

        // Stop whoosh sound
        whoosh.Stop();
    }
}
