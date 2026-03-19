using AutoPlayerScript;
using UnityEngine;
using UnityEngine.VFX;

public class Weapon : MonoBehaviour
{
    [Header("Weapon Attribute")] public string DisplayName;

    public int currentAmmo;
    public int magsSize;
    public float power; //shot's bullet initial velocity
    public float cooldown;
    public float reloadTime;

    [Header("Assets References")] public GameObject bullet;

    public VisualEffect muzzleFlash;
    public AudioClip shotEffect;
    public AudioSource audioSource;
    public VFXEventAttribute muzzleAttribute;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        //SetEquipped(false);
    }

    private void Update()
    {
    }

    public void SetEquipped(bool value)
    {
        //PlayerShoot.shootInput += Shoot;
        GetComponent<MeshRenderer>().enabled = value;
    }

    public void Unequip()
    {
        //PlayerShoot.shootInput = null;
        GetComponent<MeshRenderer>().enabled = false;
    }

    public bool CanShoot()
    {
        return currentAmmo > 0;
    }

    public void StartReload()
    {
    }

    public void FinishReload()
    {
        currentAmmo = magsSize;
    }

    public bool CanReload()
    {
        return true;
    }

    public void Shoot(Vector3 shootDirection, Vector3 shotPosition)
    {
        //create bullet then add force to it
        playShootEffect();
        currentAmmo -= 1;
        muzzleFlash.SendEvent("OnPlay");
        var bulletInstance = Instantiate(bullet, shotPosition, Quaternion.LookRotation(shootDirection));
        var bulletRB = bulletInstance.GetComponentInChildren<Rigidbody>();
        //bulletRB.AddForce(orientation.forward * 20 + orientation.up * 10, ForceMode.Impulse);
        bulletRB.AddForce(shootDirection * power, ForceMode.Impulse);
    }

    public void Shoot(Vector3 shootDirection, Vector3 shotPosition, AutoPlayerAgent shooter)
    {
        //create bullet then add force to it
        playShootEffect();
        currentAmmo -= 1;
        muzzleFlash.SendEvent("OnPlay");
        var bulletInstance = Instantiate(bullet, shotPosition, Quaternion.LookRotation(shootDirection));
        var bulletRB = bulletInstance.GetComponentInChildren<Rigidbody>();
        bulletInstance.GetComponent<MainBullet>().agent = shooter;
        //bulletRB.AddForce(orientation.forward * 20 + orientation.up * 10, ForceMode.Impulse);
        bulletRB.AddForce(shootDirection * power, ForceMode.Impulse);
    }

    public void playShootEffect()
    {
        audioSource.PlayOneShot(shotEffect);
    }
}