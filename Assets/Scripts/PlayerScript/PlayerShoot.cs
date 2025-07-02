using UnityEngine;
using System.Collections;

public class PlayerShoot : MonoBehaviour
{
    [Header("Weapon Config")]
    [SerializeField] private Vector3 rotationOffset;
    private bool equipedReadyToShoot = true;

    [Header("Keybinds")]
    public KeyCode Primary = KeyCode.Mouse0;
    public KeyCode ReloadKey = KeyCode.R;
    public KeyCode EquipPrimary = KeyCode.Alpha1;
    public KeyCode EquipSecondary = KeyCode.Alpha2;

    [Header("References")]
    public Transform gunHolder;
    public Transform gunPos;
    public Camera playerCam;
    public Transform visualizer;
    public Weapon primaryWeapon;
    public Weapon secondaryWeapon;

    [Header("Ray Settings")]
    public float rayLength = 10f;
    public LayerMask detectionLayer;
    public Color rayColor = Color.red;

    private bool isPrimary;
    private Weapon equipedWeapon;
    private Coroutine activeReloadRoutine;
    private Coroutine shootCooldownRoutine;
    private Vector3 shootDirection;

    void Start()
    {
        SwitchWeapon(true);
        Debug.Log($"Primary Weapon Assigned: {primaryWeapon != null}");
        Debug.Log($"Secondary Weapon Assigned: {secondaryWeapon != null}");
        Debug.Log($"Primary Active: {primaryWeapon.gameObject.activeInHierarchy}");
        Debug.Log($"Secondary Active: {secondaryWeapon.gameObject.activeInHierarchy}");
        //UpdateAmmoUI();
    }

    void Update()
    {
        HandleInput();
        UpdateAiming();
    }

    private void HandleInput()
    {
        // Shooting
        if (Input.GetKey(Primary) && equipedReadyToShoot && equipedWeapon.CanShoot())
        {
            ShootWeapon();
        }

        // Weapon Switching
        if (Input.GetKeyDown(EquipPrimary)) SwitchWeapon(true);
        if (Input.GetKeyDown(EquipSecondary)) SwitchWeapon(false);

        // Reloading
        if (Input.GetKeyDown(ReloadKey) && equipedWeapon.CanReload())
        {
            StartReload();
        }
    }

    private void ShootWeapon()
    {
        if (shootCooldownRoutine != null)
            StopCoroutine(shootCooldownRoutine);

        equipedReadyToShoot = false;
        equipedWeapon.Shoot(shootDirection, gunPos.position);
        UpdateAmmoUI();
        
        shootCooldownRoutine = StartCoroutine(ShootCooldown(equipedWeapon.cooldown));
    }

    private void StartReload()
    {
        // Cancel existing reload if in progress
        if (activeReloadRoutine != null)
        {
            StopCoroutine(activeReloadRoutine);
            Debug.Log("Reload interrupted");
        }

        // Start new reload
        activeReloadRoutine = StartCoroutine(ReloadWeapon());
    }

    private IEnumerator ReloadWeapon()
    {
        Debug.Log("Reloading...");
        equipedWeapon.StartReload(); // Visual/audio feedback
        
        yield return new WaitForSeconds(equipedWeapon.reloadTime);
        
        equipedWeapon.FinishReload();
        UpdateAmmoUI();
        activeReloadRoutine = null;
        Debug.Log("Reload complete");
    }

    private IEnumerator ShootCooldown(float delay)
    {
        yield return new WaitForSeconds(delay);
        equipedReadyToShoot = true;
        shootCooldownRoutine = null;
    }

    private void SwitchWeapon(bool primary)
    {
        if (isPrimary == primary) return;

        // Cancel any ongoing reload when switching weapons
        if (activeReloadRoutine != null)
        {
            StopCoroutine(activeReloadRoutine);
            activeReloadRoutine = null;
        }

        isPrimary = primary;
        equipedWeapon = primary ? primaryWeapon : secondaryWeapon;
        
        primaryWeapon.SetEquipped(primary);
        secondaryWeapon.SetEquipped(!primary);
        
        UpdateAmmoUI();
    }

    private void UpdateAiming()
    {
        Ray ray = new Ray(playerCam.transform.position, playerCam.transform.forward);
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

    private void UpdateAmmoUI()
    {
        GetComponent<ui_DisplayAmmo>().updateAmmo(equipedWeapon.currentAmmo, equipedWeapon.magsSize);
    }
}