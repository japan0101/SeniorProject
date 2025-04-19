using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
    [Header("Weapon Config")]
    private float equipedCooldown;
    private bool primaryReadyToShoot;
    public Vector3 rotationOffset; // Optional rotation adjustment

    [Header("Keybinds")]
    public KeyCode Primary = KeyCode.Mouse0;
    public KeyCode EquipPrimary = KeyCode.Alpha1;
    public KeyCode EquipSecondary = KeyCode.Alpha2;

    [Header("Object Ref")]
    //public GameObject bullet;
    public Transform gunHolder;
    public Transform gunPos;
    public Camera playerCam;
    public Transform visualizer; //to display hit position
    public GameObject primaryWeapon;
    public GameObject secondaryWeapon;

    [Header("Ray Settings")]
    //public Transform playerCam;
    public float rayLength = 10f;
    public LayerMask detectionLayer;
    public Color rayColor = Color.red;
    
    public bool isPrimary = true;
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
            //shootInput?.Invoke(shootDirection, gunPos.position);
            if (isPrimary)
            {
                primaryWeapon.GetComponent<Weapon>().Shoot(shootDirection, gunPos.position);
            }
            else
            {
                secondaryWeapon.GetComponent<Weapon>().Shoot(shootDirection, gunPos.position);
            }
                resetPrimaryRoutine = StartCoroutine(PrimaryCooldown(equipedCooldown));
        }
        //change weapon state and any related variable to the equiped weapon
        if (Input.GetKey(EquipPrimary) && !isPrimary) {
            primaryWeapon.GetComponent<Weapon>().Equip();
            secondaryWeapon.GetComponent<Weapon>().Unequip();
            equipedCooldown = primaryWeapon.GetComponent<Weapon>().cooldown;
            isPrimary = true;
        }
        if (Input.GetKey(EquipSecondary) && isPrimary)
        {
            secondaryWeapon.GetComponent<Weapon>().Equip();
            primaryWeapon.GetComponent<Weapon>().Unequip();
            equipedCooldown = secondaryWeapon.GetComponent<Weapon>().cooldown;
            isPrimary = false;
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ResetCooldown();
        primaryWeapon.GetComponent<Weapon>().Equip();
        secondaryWeapon.GetComponent<Weapon>().Unequip();
        equipedCooldown = primaryWeapon.GetComponent<Weapon>().cooldown;
    }
    private void ResetCooldown()
    {
        primaryReadyToShoot = true;
    }
    private IEnumerator PrimaryCooldown(float delay)
    {
        yield return new WaitForSeconds(delay);
        primaryReadyToShoot = true;
        resetPrimaryRoutine = null; //clear routine ref
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
            //Debug.Log("Hit: " + hit.collider.name);
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
