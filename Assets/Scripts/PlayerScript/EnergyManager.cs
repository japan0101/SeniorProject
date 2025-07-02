using UnityEngine;
using UnityEngine.UI;

public class EnergyManager : MonoBehaviour
{
    [Header("Attributes")]
    public float maxEnergy = 100;
    public float regenerationRate = 1f;

    private float energy;
    public Image energyDisplay;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        energy = maxEnergy;
        updateDisplay();
    }
    private void Update()
    {
        regenEnergy();
        updateDisplay();
    }
    private void regenEnergy()
    {
        if (energy < maxEnergy) {
            energy += regenerationRate * Time.deltaTime;
        }
        
    }
    void updateDisplay()
    {
        energyDisplay.fillAmount = energy/maxEnergy;
    }
    public bool consumeEnergy(float unit)
    {
        if (energy - unit < 0) {
            return false;
        }
        else
        {
            energy -= unit;
            updateDisplay();
        }
        return true;
    }
    public void gainEnergy(float unit)
    {
        if (energy + unit > maxEnergy)
        {
            energy = maxEnergy;
        }
        else
        {
            energy += unit;
            updateDisplay();
        }
    }

    public float getEnergy()
    {
        return energy;
    }
}
