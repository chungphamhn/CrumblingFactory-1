﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserPointer : MonoBehaviour {

    private SteamVR_TrackedObject trackedObj;

    // material objects
    public Material mat1;
    public Material mat2;
    private Renderer rend;

    // laser objects
    public GameObject laserP;
    private GameObject laser;
    private Transform laserTransform;
    private Vector3 hitPoint;

    // teleport objects
    public Transform cameraRigTransform;
    public GameObject teleportReticlePrefab;
    private GameObject reticle;
    private Transform teleportReticleTransform;
    public Transform headTransform;
    public Vector3 teleportReticleOffset;
    public LayerMask teleportMask; // You have to create a new layer and put it here to be able to teleport on the floor
    private bool shouldTeleport;

    private SteamVR_Controller.Device Controller // Gets the controller object
    {
        get { return SteamVR_Controller.Input((int)trackedObj.index); }
    }

    private void Awake() // Starts tracking on Awake
    {
        trackedObj = GetComponent<SteamVR_TrackedObject>();
    }

    private void ShowLaser(RaycastHit hit) // Enables and builds the laser
    {
        laser.SetActive(true);
        laserTransform.position = Vector3.Lerp(trackedObj.transform.position, hitPoint, 0.5f);
        laserTransform.LookAt(hitPoint);
        laserTransform.localScale = new Vector3(laserTransform.localScale.x, laserTransform.localScale.y, hit.distance);
    }

    private void Teleport() 
    {
        shouldTeleport = false; // sets the bool back to false
        reticle.SetActive(false); // sets reticle to false
        Vector3 difference = cameraRigTransform.position - headTransform.position;
        difference.y = 0; // keeps you in the correct area on y-axis
        cameraRigTransform.position = hitPoint + difference; // teleports Camera Rig to the reticle
    }

    // Use this for initialization
    void Start () {
        Renderer rend = laser.GetComponent<Renderer>();

        laser = Instantiate(laserP); // instantiates our laser prefab
        laserTransform = laser.transform;

        reticle = Instantiate(teleportReticlePrefab); // instantiates our reticle prefab
        teleportReticleTransform = reticle.transform;
    }
	
	// Update is called once per frame
	void Update () {
         
        if (Controller.GetPress(SteamVR_Controller.ButtonMask.Trigger)) // checks for a trigger press
        {
            RaycastHit hit; 
            if (Physics.Raycast(trackedObj.transform.position, transform.forward, out hit, 25)) // Checks what raycast is colliding with and if it has the correct layer active to teleport
            {
                hitPoint = hit.point;
                ShowLaser(hit); 
                reticle.SetActive(true); // sets reticle active
                teleportReticleTransform.position = hitPoint + teleportReticleOffset; // changes reticles position to raycasts hit point
                if (hit.collider.gameObject.layer == 8)
                {
                    rend.material = mat1;
                    shouldTeleport = true; // enables the use of Teleport();
                } else
                {
                    rend.material = mat2;
                }
            }
        }
        else // disables laser and reticle if the trigger isn't pressed
        {
            laser.SetActive(false);
            reticle.SetActive(false);
        }

        if (Controller.GetPressUp(SteamVR_Controller.ButtonMask.Trigger) && shouldTeleport) // Teleports when the trigger is released if shouldTeleport bool is true
        {
            Teleport();
        }
    }
}
