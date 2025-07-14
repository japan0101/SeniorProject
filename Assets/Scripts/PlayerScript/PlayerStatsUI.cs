using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class PlayerStatsUI : MonoBehaviour
{
    [Header("Refferences")]
    public PlayerStats playerData;
    public GameObject statsPanel;
    public TextMeshProUGUI MaxHealthUI;
    public TextMeshProUGUI MaxEnergyUI;
    public TextMeshProUGUI MoveSpeedUI;
    public TextMeshProUGUI EnergyRegenUI;
    public TextMeshProUGUI SprintSpeedUI;
    public TextMeshProUGUI SprintCostUI;
    public TextMeshProUGUI DashSpeedUI;
    public TextMeshProUGUI DashCostUI;
    public TextMeshProUGUI DashDurationUI;
    public TextMeshProUGUI DashCooldownUI;
    public TextMeshProUGUI JumpForceUI;
    public TextMeshProUGUI JumpCostUI;
    public TextMeshProUGUI JumpCDUI;
    [Header("Keybind")]
    public KeyCode OpenStatsMenu = KeyCode.Tab;

    private Coroutine inputDelayRoutine;
    private bool inputEnabled = true;

    private bool isMenuOpen;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ExitStats();
        OnEnable();
    }

    // Update is called once per frame
    void Update()
    {

        HandleInput();
    }
    private void HandleInput()
    {
        if (Input.GetKey(OpenStatsMenu) && !isMenuOpen && inputEnabled)
        {
            UpdateUI();
            DisplayStats();
        }
        if(Input.GetKey(OpenStatsMenu) && isMenuOpen && inputEnabled)
        {
            ExitStats();
        }

    }
    private IEnumerator MenuDelay()
    {
        yield return new WaitForSeconds(1);
        inputEnabled = !inputEnabled;
    }
    void UpdateUI()
    {
        MaxHealthUI.text = "Max Health: " + playerData.maxHP;
        MaxEnergyUI.text = "Max Energy: " + playerData.maxEnergy;
        MoveSpeedUI.text = "Move Speed: " + playerData.moveSpeed;
        EnergyRegenUI.text = "Energy Regen: " + playerData.EnergyRegenRate;
        SprintSpeedUI.text = "Sprint Speed: " + playerData.sprintSpeed;
        SprintCostUI.text = "Sprint Cost: " + playerData.sprintCost;
        DashSpeedUI.text = "Dash speed: " + playerData.dashForce;
        DashCostUI.text = "Dash cost: " + playerData.dashCost;
        DashDurationUI.text = "Dash duration: " + playerData.dashDuration;
        DashCooldownUI.text = "Dash cooldown: " + playerData.dashCooldown;
        JumpForceUI.text = "Jump Froce: " + playerData.jumpForce;
        JumpCostUI.text = "Jump Cost: " + playerData.jumpCost;
        JumpCDUI.text = "Jump Cooldown: " + playerData.jumpCooldown;
    }
    void DisplayStats()
    {
        statsPanel.SetActive(true);
        isMenuOpen = true;

        inputEnabled = !inputEnabled;
        if (inputDelayRoutine != null)
            StopCoroutine(inputDelayRoutine);

        inputDelayRoutine = StartCoroutine(MenuDelay());
    }
    void ExitStats()
    {
        statsPanel.SetActive(false);
        isMenuOpen = false;
        inputEnabled = !inputEnabled;
        if (inputDelayRoutine != null)
            StopCoroutine(inputDelayRoutine);

        inputDelayRoutine = StartCoroutine(MenuDelay());
    }
    void OnEnable()
    {
        playerData.onStatsChanged += UpdateUI;
    }

    void OnDisable()
    {
        playerData.onStatsChanged -= UpdateUI;
    }
}
