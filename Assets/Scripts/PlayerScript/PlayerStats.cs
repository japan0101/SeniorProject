using UnityEngine;

[CreateAssetMenu(fileName = "PlayerStats", menuName = "PlayerStats")]
public class PlayerStats : ScriptableObject
{
    //Base stats
    public float maxHP = 250;
    public float currentHP = 250;

    public float maxEnergy = 100;
    public float currentEnergy = 100;
    public float EnergyRegenRate = 1f;

    public float moveSpeed = 7;
    public float sprintSpeed = 10;
    public float sprintCost = 0.5f;

    public float dashForce = 20;
    public float dashDuration = 0.2f;
    public float dashCooldown = 0.8f;
    public float dashCost = 10;

    public float jumpForce = 8;
    public float jumpCooldown = 0.5f;
    public float jumpCost = 15;

    public void ModifyHP(float amount)
    {
        currentHP = Mathf.Clamp(currentHP + amount, 0, maxHP);
    }

    public void ModifyEnergy(float amount)
    {
        currentEnergy = Mathf.Clamp(currentEnergy + amount, 0, maxEnergy);
    }
}
