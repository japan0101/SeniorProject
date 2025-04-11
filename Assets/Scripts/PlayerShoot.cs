using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
    [Header("Weapon Config")]
    public float primaryCooldown;
    private bool primaryReadyToShoot;
    public Vector3 rotationOffset; // Optional rotation adjustment

    [Header("Keybinds")]
    public KeyCode Primary = KeyCode.Mouse0;

    [Header("Object Ref")]
    public GameObject bullet;
    public Transform gunHolder;
    public Transform gunPos;
    public Camera playerCam;
    public Transform visualizer; //to display hit position

    [Header("Ray Settings")]
    //public Transform playerCam;
    public float rayLength = 10f;
    public LayerMask detectionLayer;
    public Color rayColor = Color.red;
    

    private Coroutine resetPrimaryRoutine;
    public static Action<Vector3, Vector3> shootInput;
    private Vector3 shootDirection;
    private void MyInput()
    {
        if (Input.GetKey(Primary) && primaryReadyToShoot)
        {
            if (resetPrimaryRoutine != null)
                StopCoroutine(resetPrimaryRoutine);
            primaryReadyToShoot = false;
            shootInput?.Invoke(shootDirection, gunPos.position);
            resetPrimaryRoutine = StartCoroutine(PrimaryCooldown(primaryCooldown));
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ResetCooldown();
    }
    private void ResetCooldown()
    {
        primaryReadyToShoot = true;
    }
    private IEnumerator PrimaryCooldown(float delay)
    {
        yield return new WaitForSeconds(delay);
        primaryReadyToShoot = true;
        resetPrimaryRoutine = null; //clear ref
    }

    void Update()
    {
        MyInput();
        shootDirection = getTarget();
    }

    private Vector3 getTarget()
    {
        Ray ray = new Ray(playerCam.transform.position, playerCam.transform.forward);
        RaycastHit hit;
        Vector3 targetPoint;

        // Check if ray hits something
        if (Physics.Raycast(ray, out hit, rayLength, detectionLayer))
        {
            // Use the hit point if we hit something
            targetPoint = hit.point;
            Debug.Log("Hit: " + hit.collider.name);
        }
        else
        {
            // Use the ray's end point if nothing was hit
            targetPoint = ray.origin + ray.direction * rayLength;
        }
        if (visualizer != null)
        {
            visualizer.position = targetPoint;
        }

        //get traget direction
        Vector3 direction = targetPoint - gunHolder.position;

        // Create the rotation to look at the point
        Quaternion targetRotation = Quaternion.LookRotation(direction);

        //// Apply optional offset
        targetRotation *= Quaternion.Euler(rotationOffset);

        gunHolder.rotation = targetRotation;

        // Debug visualization
        Debug.DrawRay(gunHolder.position, ray.direction * rayLength, rayColor);
        return direction;
    }
}
