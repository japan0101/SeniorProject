using System;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
    [Header("Weapon Config")]
    public float primaryCooldown;
    
    private bool primaryReadyToShoot;

    [Header("Keybinds")]
    public KeyCode Primary = KeyCode.Mouse0;

    [Header("Object Ref")]
    public GameObject bullet;
    public Transform orientation;
    public Transform gunPos;
    private void MyInput()
    {
        if (Input.GetKey(Primary) && primaryReadyToShoot)
        {
            primaryReadyToShoot = false;
            Shoot();
            Invoke(nameof(ResetCooldown), primaryCooldown);
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

    private void Shoot()
    {
        if (Physics.Raycast(gunPos.position, orientation.forward, out RaycastHit hitInfo, 200))
        {
            Instantiate(bullet, hitInfo.point, transform.rotation);
            Debug.Log(hitInfo.point);
            Debug.DrawLine(gunPos.position, hitInfo.point, Color.green, 20f);
        }
    }
    void Update()
    {
        MyInput();
        //Vector3 forward = orientation.TransformDirection(Vector3.forward) * 10;
        //Debug.DrawRay(transform.position, forward, Color.green);
    }
}
