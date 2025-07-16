using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerShoot : MonoBehaviour
{
    [Header("Weapon Config")]
    [SerializeField] private Vector3 rotationOffset;
    private bool equipedReadyToShoot = true;
    public Color equipedColor = new Color(0f, 1f, 0.5f, 0.5f);
    public Color nonEquipedColor = new Color(0f, 0f, 0f, 0.3921f);

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
    private float reloadTimer;
    private bool isReloading;

[SerializeField] public Image PrimaryPanel;
[SerializeField] public Image SecondaryPanel;

    public bool IsReloading() => activeReloadRoutine != null;

    void Start()
    {
        SwitchWeapon(true);
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
        if (Input.GetKey(Primary) && equipedReadyToShoot && equipedWeapon.CanShoot() && activeReloadRoutine == null)
        {
            ShootWeapon();
        }

        // Weapon Switching
        if (Input.GetKeyDown(EquipPrimary)) SwitchWeapon(true);
        if (Input.GetKeyDown(EquipSecondary)) SwitchWeapon(false);

        // Reloading
        if (Input.GetKeyDown(ReloadKey) && equipedWeapon.CanReload() && !isReloading)
        {
            isReloading = true;
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

    public float GetReloadProgress()
    {
        return IsReloading() ? reloadTimer / equipedWeapon.reloadTime : 0;
    }
    private IEnumerator ReloadWeapon()
    {
        reloadTimer = 0;
        while (reloadTimer < equipedWeapon.reloadTime)
        {
            reloadTimer += Time.deltaTime;
            yield return null;
        }
        //Debug.Log("Reloading...");
        equipedWeapon.StartReload(); // Visual/audio feedback
        
        //yield return new WaitForSeconds(equipedWeapon.reloadTime);
        
        equipedWeapon.FinishReload();
        UpdateAmmoUI();
        activeReloadRoutine = null;
        isReloading = false;
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
        PrimaryPanel.color = primary ? equipedColor : nonEquipedColor;// changes ui color to indicate equiped weapon
        SecondaryPanel.color = primary ? nonEquipedColor : equipedColor;// changes ui color to indicate equiped weapon
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