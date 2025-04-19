using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
    [Header("Weapon Config")]
    private bool equipedReadyToShoot;
    public Vector3 rotationOffset; // Optional rotation adjustment

    [Header("Keybinds")]
    public KeyCode Primary = KeyCode.Mouse0;
    public KeyCode ReloadKey = KeyCode.R;
    public KeyCode EquipPrimary = KeyCode.Alpha1;
    public KeyCode EquipSecondary = KeyCode.Alpha2;

    [Header("Object Ref")]
    //public GameObject bullet;
    public Transform gunHolder;
    public Transform gunPos;
    public Camera playerCam;
    public Transform visualizer; //to display hit position
    public Weapon primaryWeapon;
    public Weapon secondaryWeapon;

    [Header("Ray Settings")]
    //public Transform playerCam;
    public float rayLength = 10f;
    public LayerMask detectionLayer;
    public Color rayColor = Color.red;
    
    public bool isPrimary = true;
    private Weapon equipedWeapon;

    private Coroutine resetPrimaryRoutine;
    public static Action<Vector3, Vector3> shootInput;
    private Vector3 shootDirection;
    private void MyInput()
    {
        if (Input.GetKey(Primary) && equipedReadyToShoot && equipedWeapon.CanShoot())
        {
            if (resetPrimaryRoutine != null)
                StopCoroutine(resetPrimaryRoutine);
            equipedReadyToShoot = false;
            //shootInput?.Invoke(shootDirection, gunPos.position);
            if (isPrimary)
            {
                primaryWeapon.Shoot(shootDirection, gunPos.position);
            }
            else
            {
                secondaryWeapon.Shoot(shootDirection, gunPos.position);
            }
                resetPrimaryRoutine = StartCoroutine(PrimaryCooldown(equipedWeapon.cooldown));
        }
        //change weapon state and any related variable to the equiped weapon
        if (Input.GetKey(EquipPrimary) && !isPrimary) {
            primaryWeapon.Equip();
            secondaryWeapon.Unequip();
            equipedWeapon = primaryWeapon;
            isPrimary = true;
        }
        if (Input.GetKey(EquipSecondary) && isPrimary)
        {
            secondaryWeapon.Equip();
            primaryWeapon.Unequip();
            equipedWeapon = secondaryWeapon;
            isPrimary = false;
        }
        if (Input.GetKey(ReloadKey))
        {
            equipedWeapon.Reload();
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ResetCooldown();
        primaryWeapon.Equip();
        secondaryWeapon.Unequip();
        equipedWeapon = primaryWeapon;
        equipedWeapon.cooldown = primaryWeapon.cooldown;
    }
    private void ResetCooldown()
    {
        equipedReadyToShoot = true;
    }
    private IEnumerator PrimaryCooldown(float delay)
    {
        yield return new WaitForSeconds(delay);
        equipedReadyToShoot = true;
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
