using System;
using System.Diagnostics.Tracing;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.VFX;

public class Weapon : MonoBehaviour
{
    [Header("Weapon Attribute")]
    public int currentAmmo;
    public int magsSize;
    public float power; //shot's bullet initial velocity
    public float cooldown;

    [Header("Assets References")]
    public GameObject bullet;
    public VisualEffect muzzleFlash;
    public VFXEventAttribute muzzleAttribute;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    public void Equip()
    {
        PlayerShoot.shootInput += Shoot;
        GetComponent<MeshRenderer>().enabled = true;
    }

    public void Unequip()
    {
        PlayerShoot.shootInput = null;
        GetComponent<MeshRenderer>().enabled = false;
    }

    public void Reload()
    {
        currentAmmo = magsSize;
    }

    public bool CanShoot()
    {
        return currentAmmo > 0;
    }
    public void Shoot(Vector3 shootDirection, Vector3 shotPosition)
    {
        //create bullet then add force to it
        //Vector3 shootDirection = playerCam.transform.forward;
        currentAmmo -= 1;
        muzzleFlash.SendEvent("OnPlay");
        var bulletInstance = Instantiate(bullet, shotPosition, Quaternion.LookRotation(shootDirection));
        var bulletRB = bulletInstance.GetComponentInChildren<Rigidbody>();
        //bulletRB.AddForce(orientation.forward * 20 + orientation.up * 10, ForceMode.Impulse);
        bulletRB.AddForce(shootDirection * power, ForceMode.Impulse);

        //casting rays to check hit direction currently as debug
        //Debug.DrawRay(shotPosition, shootDirection * 5, Color.red, 1f);
    }
    void Update()
    {
        
    }
}
