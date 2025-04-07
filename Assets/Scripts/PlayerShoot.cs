using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
    [Header("Weapon Config")]
    public float primaryCooldown;
    public float primaryPower;
    
    private bool primaryReadyToShoot;

    [Header("Keybinds")]
    public KeyCode Primary = KeyCode.Mouse0;

    [Header("Object Ref")]
    public GameObject bullet;
    public Transform orientation;
    public Transform gunPos;
    public Camera playerCam;

    private Coroutine resetPrimaryRoutine;
    private void MyInput()
    {
        if (Input.GetKey(Primary) && primaryReadyToShoot)
        {
            if (resetPrimaryRoutine != null)
                StopCoroutine(resetPrimaryRoutine);
            primaryReadyToShoot = false;
            Shoot();
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

    private void Shoot()
    {
        //create bullet then add force to it
        Vector3 shootDirection = playerCam.transform.forward;
        var bulletInstance = Instantiate(bullet, gunPos.position, Quaternion.LookRotation(shootDirection));
        var bulletRB = bulletInstance.GetComponentInChildren<Rigidbody>();
        //bulletRB.AddForce(orientation.forward * 20 + orientation.up * 10, ForceMode.Impulse);
        bulletRB.AddForce(shootDirection * primaryPower, ForceMode.Impulse);

        //casting rays to check hit direction currently as debug
        Debug.DrawRay(gunPos.position, shootDirection * 5, Color.red, 1f);
    }
    void Update()
    {
        MyInput();
        //Vector3 forward = orientation.TransformDirection(Vector3.forward) * 10;
        //Debug.DrawRay(transform.position, forward, Color.green);
    }
}
