using System;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [Header("Weapon Config")]
    public float primaryCooldown;
    
    private bool primaryReadyToShoot;

    [Header("Keybinds")]
    public KeyCode Primary = KeyCode.Mouse0;
    
    private void MyInput()
    {
        if (Input.GetKey(Primary) && primaryReadyToShoot)
        {
            Debug.Log("Pew");
            Invoke(nameof(ResetCooldown), primaryCooldown);
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ResetCooldown();
    }
    private void ResetCooldown()
    {
        primaryReadyToShoot = true;
    }
    // Update is called once per frame
    void Update()
    {
        MyInput();
    }
}
