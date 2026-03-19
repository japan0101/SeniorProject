using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class PlayerStatsUI : MonoBehaviour
{
    public enum TextMeshUI
    {
        MaxHealthUI,
        MaxEnergyUI,
        MoveSpeedUI,
        EnergyRegenUI,
        SprintSpeedUI,
        SprintCostUI,
        DashSpeedUI,
        DashCostUI,
        DashDurationUI,
        DashCooldownUI,
        JumpForceUI,
        JumpCostUI,
        JumpCDUI
    }

    public enum WeaponStatList
    {
        DisplayName,
        CurrentAmmo,
        MaxAmmo,
        Power,
        ShotCooldown,
        ReloadTime
    }

    [Header("Refferences")] public PlayerStats playerData;

    public GameObject statsPanel;
    [SerializeField] private TextMeshProUGUI[] statsElements; // Array mapped to TextMeshUI enum
    [SerializeField] private TextMeshProUGUI[] PrimaryWeaponStatsTextUI;
    [SerializeField] private TextMeshProUGUI[] SecondaryWeaponStatsTextUI;
    [SerializeField] public Weapon PrimaryWeapon;
    [SerializeField] public Weapon SecondaryWeapon;

    [Header("Keybind")] public KeyCode OpenStatsMenu = KeyCode.Tab;

    private Coroutine inputDelayRoutine;
    private bool inputEnabled = true;

    private bool isMenuOpen;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        if (statsElements.Length != Enum.GetNames(typeof(TextMeshUI)).Length)
            Debug.LogError("statsElements array size doesn't match TextMeshUI enum count!");
        if (PrimaryWeaponStatsTextUI.Length != Enum.GetNames(typeof(WeaponStatList)).Length)
            Debug.LogError("PrimaryWeaponStatsTextUI array size doesn't match WeaponStatList enum count!");
        if (SecondaryWeaponStatsTextUI.Length != Enum.GetNames(typeof(WeaponStatList)).Length)
            Debug.LogError("SecondaryWeaponStatsTextUI array size doesn't match WeaponStatList enum count!");
        ExitStats();
        OnEnable();
    }

    // Update is called once per frame
    private void Update()
    {
        HandleInput();
    }

    private void OnEnable()
    {
        playerData.onStatsChanged += UpdateUI;
    }

    private void OnDisable()
    {
        playerData.onStatsChanged -= UpdateUI;
    }

    private void HandleInput()
    {
        if (Input.GetKey(OpenStatsMenu) && !isMenuOpen && inputEnabled)
        {
            UpdateUI();
            DisplayStats();
        }

        if (Input.GetKey(OpenStatsMenu) && isMenuOpen && inputEnabled) ExitStats();
    }

    private IEnumerator MenuDelay()
    {
        yield return new WaitForSeconds(1);
        inputEnabled = !inputEnabled;
    }

    private void UpdateUI()
    {
        //set player stats
        SetText(TextMeshUI.MaxHealthUI, $"Max Health: {playerData.maxHP}");
        SetText(TextMeshUI.MaxEnergyUI, $"Max Energy: {playerData.maxEnergy}");
        SetText(TextMeshUI.MoveSpeedUI, $"Move Speed: {playerData.moveSpeed}");
        SetText(TextMeshUI.EnergyRegenUI, $"Energy Regen: {playerData.EnergyRegenRate}");
        SetText(TextMeshUI.SprintSpeedUI, $"Sprint Speed: {playerData.sprintSpeed}");
        SetText(TextMeshUI.SprintCostUI, $"Sprint Cost: {playerData.sprintCost}");
        SetText(TextMeshUI.DashSpeedUI, $"Dash speed: {playerData.dashForce}");
        SetText(TextMeshUI.DashCostUI, $"Dash cost: {playerData.dashCost}");
        SetText(TextMeshUI.DashDurationUI, $"Dash duration: {playerData.dashDuration}");
        SetText(TextMeshUI.DashCooldownUI, $"Dash cooldown: {playerData.dashCooldown}");
        SetText(TextMeshUI.JumpForceUI, $"Jump Force: {playerData.jumpForce}");
        SetText(TextMeshUI.JumpCostUI, $"Jump Cost: {playerData.jumpCost}");
        SetText(TextMeshUI.JumpCDUI, $"Jump Cooldown: {playerData.jumpCooldown}");

        //set primary stats
        SetWeaponText(WeaponStatList.DisplayName, $"{PrimaryWeapon.DisplayName}", PrimaryWeaponStatsTextUI);
        SetWeaponText(WeaponStatList.CurrentAmmo, $"Ammo: {PrimaryWeapon.currentAmmo}", PrimaryWeaponStatsTextUI);
        SetWeaponText(WeaponStatList.MaxAmmo, $"Mags Size: {PrimaryWeapon.magsSize}", PrimaryWeaponStatsTextUI);
        SetWeaponText(WeaponStatList.Power, $"Power: {PrimaryWeapon.power}", PrimaryWeaponStatsTextUI);
        SetWeaponText(WeaponStatList.ShotCooldown, $"Shot Intervals: {PrimaryWeapon.cooldown}",
            PrimaryWeaponStatsTextUI);
        SetWeaponText(WeaponStatList.ReloadTime, $"Reload Time: {PrimaryWeapon.reloadTime}", PrimaryWeaponStatsTextUI);

        //set secondary stats
        SetWeaponText(WeaponStatList.DisplayName, $"{SecondaryWeapon.DisplayName}", SecondaryWeaponStatsTextUI);
        SetWeaponText(WeaponStatList.CurrentAmmo, $"Ammo: {SecondaryWeapon.currentAmmo}", SecondaryWeaponStatsTextUI);
        SetWeaponText(WeaponStatList.MaxAmmo, $"Mags Size: {SecondaryWeapon.magsSize}", SecondaryWeaponStatsTextUI);
        SetWeaponText(WeaponStatList.Power, $"Power: {SecondaryWeapon.power}", SecondaryWeaponStatsTextUI);
        SetWeaponText(WeaponStatList.ShotCooldown, $"Shot Intervals: {SecondaryWeapon.cooldown}",
            SecondaryWeaponStatsTextUI);
        SetWeaponText(WeaponStatList.ReloadTime, $"Reload Time: {SecondaryWeapon.reloadTime}",
            SecondaryWeaponStatsTextUI);
    }

    private void DisplayStats()
    {
        statsPanel.SetActive(true);
        isMenuOpen = true;

        inputEnabled = !inputEnabled;
        if (inputDelayRoutine != null)
            StopCoroutine(inputDelayRoutine);

        inputDelayRoutine = StartCoroutine(MenuDelay());
    }

    private void ExitStats()
    {
        statsPanel.SetActive(false);
        isMenuOpen = false;
        inputEnabled = !inputEnabled;
        if (inputDelayRoutine != null)
            StopCoroutine(inputDelayRoutine);

        inputDelayRoutine = StartCoroutine(MenuDelay());
    }

    private void SetText(TextMeshUI element, string text)
    {
        statsElements[(int)element].text = text; // set the ui text depending on the enum element and a given string
    }

    private void SetWeaponText(WeaponStatList element, string text, TextMeshProUGUI[] UIArray)
    {
        UIArray[(int)element].text =
            text; // set the ui text depending on the enum element and a given string into an array of weapon ui element
    }
}