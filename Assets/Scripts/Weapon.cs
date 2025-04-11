using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class Weapon : MonoBehaviour
{
    public float primaryPower;
    public GameObject bullet;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Equip();
    }

    public void Equip()
    {
        PlayerShoot.shootInput += Shoot;
    }

    private void Shoot(Vector3 shootDirection, Vector3 shotPosition)
    {
        //create bullet then add force to it
        //Vector3 shootDirection = playerCam.transform.forward;
        var bulletInstance = Instantiate(bullet, shotPosition, Quaternion.LookRotation(shootDirection));
        var bulletRB = bulletInstance.GetComponentInChildren<Rigidbody>();
        //bulletRB.AddForce(orientation.forward * 20 + orientation.up * 10, ForceMode.Impulse);
        bulletRB.AddForce(shootDirection * primaryPower, ForceMode.Impulse);

        //casting rays to check hit direction currently as debug
        //Debug.DrawRay(shotPosition, shootDirection * 5, Color.red, 1f);
    }
    void Update()
    {
        
    }
}
