using TMPro;
using UnityEngine;

public class ui_DisplayAmmo : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public TextMeshProUGUI displayText;
    public void updateAmmo(int current, int max)
    {
        displayText.text = "Ammo: " + current + " / " + max;
    }
}
