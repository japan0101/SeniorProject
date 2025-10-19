using UnityEngine;
using UnityEngine.UI;

public class EnergyManager : MonoBehaviour
{
    [SerializeField] private PlayerStats _stats;
    public Image energyDisplay;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _stats.ModifyEnergy(_stats.maxEnergy);
        updateDisplay();
    }
    private void Update()
    {
        regenEnergy();
        updateDisplay();
    }
    private void regenEnergy()
    {
        _stats.ModifyEnergy(_stats.EnergyRegenRate * Time.deltaTime);
        
    }
    void updateDisplay()
    {
        energyDisplay.fillAmount = _stats.currentEnergy / _stats.maxEnergy;
    }
    public bool consumeEnergy(float unit)
    {
        if (_stats.currentEnergy - unit < 0) {
            return false;
        }
        else
        {
            _stats.ModifyEnergy(-unit);
            updateDisplay();
        }
        return true;
    }
    public void gainEnergy(float unit)
    {
        _stats.ModifyEnergy(unit);
        updateDisplay();
    }

    public float getEnergy()
    {
        return _stats.currentEnergy;
    }
}
