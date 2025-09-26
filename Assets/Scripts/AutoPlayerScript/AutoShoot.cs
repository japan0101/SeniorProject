using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using AutoPlayerScript;
using Unity.VisualScripting;

public class AutoShoot : MonoBehaviour
{
    [Header("Weapon Config")]
    [SerializeField] private Vector3 rotationOffset;
    private bool equipedReadyToShoot = true;
    public Color equipedColor = new Color(0f, 1f, 0.5f, 0.5f);
    public Color nonEquipedColor = new Color(0f, 0f, 0f, 0.3921f);
    [Range(0f, 1f)]
    public float chanceToShoot = 0.8f;

    [Header("References")]
    [SerializeField] public Transform gunHolder;
    [SerializeField] public Transform gunPos;
    [SerializeField] public Transform aimLocation;
    [SerializeField] public Transform visualizer;
    [SerializeField] public Weapon primaryWeapon;
    [SerializeField] public Weapon secondaryWeapon;
    [SerializeField] public AutoPlayerAgent agent;

    [Header("Ray Settings")]
    public float rayLength = 10f;
    public LayerMask detectionLayer;
    public Color rayColor = Color.red;

    private bool isPrimary;
    private Weapon equipedWeapon;
    private Coroutine activeReloadRoutine;
    private Coroutine shootCooldownRoutine;
    private Vector3 shootDirection;
    private float reloadTimer;
    private float cooldownTimer;
    private bool isReloading = false;


    public bool IsReloading() => activeReloadRoutine != null;

    private void Awake()
    {
        GetComponent<Rigidbody>().freezeRotation=true;
        SwitchWeapon(true);
    }

    void Start()
    {
        
        //UpdateAmmoUI();
    }

    void Update()
    {
        UpdateAiming();
        //transform.Rotate(0, 20 * Time.deltaTime, 0);
    }

    public int GetCurrentAmmo()
    {
        if (equipedWeapon == null)
        {
            Debug.Log("Not equiped");
            return 0;
        }
        return equipedWeapon.currentAmmo;
    }
    public void ShootWeapon()
    {
        if (shootCooldownRoutine != null)
            StopCoroutine(shootCooldownRoutine);

        equipedReadyToShoot = false;
        equipedWeapon.Shoot(shootDirection, gunPos.position, agent);
        
        shootCooldownRoutine = StartCoroutine(ShootCooldown(equipedWeapon.cooldown));
    }
    public bool canReload()
    {
        return equipedWeapon.CanReload() && !isReloading;
    }
    public void StartReload()
    {
        isReloading = true;
        // Cancel existing reload if in progress
        if (activeReloadRoutine != null)
        {
            StopCoroutine(activeReloadRoutine);
            // Debug.Log("Reload interrupted");
        }

        // Start new reload
        activeReloadRoutine = StartCoroutine(ReloadWeapon());
    }

    public float GetReloadProgress()
    {
        return IsReloading() ? reloadTimer / equipedWeapon.reloadTime : 0;
    }
    public IEnumerator ReloadWeapon()
    {
        //Debug.Log("Reloading...");
        equipedWeapon.StartReload(); // Visual/audio feedback

        reloadTimer = 0;
        while (reloadTimer < equipedWeapon.reloadTime)
        {
            reloadTimer += Time.deltaTime;
            yield return null;
        }
        
        //yield return new WaitForSeconds(equipedWeapon.reloadTime);
        
        equipedWeapon.FinishReload();
        activeReloadRoutine = null;
        isReloading = false;
        // Debug.Log("Reload complete");
    }

    private IEnumerator ShootCooldown(float delay)
    {
        reloadTimer = 0;
        while (reloadTimer < equipedWeapon.cooldown)
        {
            reloadTimer += Time.deltaTime;
            //Debug.Log($"Timer: {reloadTimer}");
            yield return null;
        }
        equipedReadyToShoot = true;
        shootCooldownRoutine = null;
    }

    public void SwitchWeapon(bool primary)
    {
        if (isPrimary == primary) return;

        // Cancel any ongoing reload when switching weapons
        if (activeReloadRoutine != null)
        {
            StopCoroutine(activeReloadRoutine);
            activeReloadRoutine = null;
            isReloading = false ;
        }

        isPrimary = primary;
        equipedWeapon = primary ? primaryWeapon : secondaryWeapon;
        primaryWeapon.SetEquipped(primary);
        secondaryWeapon.SetEquipped(!primary);
        
    }

    public bool CanShoot()
    {
        return equipedWeapon.CanShoot() && equipedReadyToShoot && activeReloadRoutine == null;
    }
    private void UpdateAiming()
    {
        Ray ray = new Ray(aimLocation.position, aimLocation.transform.forward);
        RaycastHit hit;
        Vector3 targetPoint = Physics.Raycast(ray, out hit, rayLength, detectionLayer) 
            ? hit.point 
            : ray.origin + ray.direction * rayLength;
        if (visualizer != null)
            visualizer.position = targetPoint;

        shootDirection = targetPoint - gunHolder.position;
        gunHolder.rotation = Quaternion.LookRotation(shootDirection) * Quaternion.Euler(rotationOffset);
        
        Debug.DrawRay(gunHolder.position, shootDirection, rayColor);
    }

}