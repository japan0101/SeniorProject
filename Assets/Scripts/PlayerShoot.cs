using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
    [Header("Weapon Config")]
    public float primaryCooldown;
    private bool primaryReadyToShoot;

    [Header("Keybinds")]
    public KeyCode Primary = KeyCode.Mouse0;

    [Header("Object Ref")]
    public GameObject bullet;
    public Transform orientation;
    public Transform gunPos;
    public Camera playerCam;

    private Coroutine resetPrimaryRoutine;
    public static Action<Vector3, Vector3> shootInput;
    private void MyInput()
    {
        if (Input.GetKey(Primary) && primaryReadyToShoot)
        {
            if (resetPrimaryRoutine != null)
                StopCoroutine(resetPrimaryRoutine);
            primaryReadyToShoot = false;
            shootInput?.Invoke(playerCam.transform.forward, gunPos.position);
            resetPrimaryRoutine = StartCoroutine(PrimaryCooldown(primaryCooldown));
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
    private IEnumerator PrimaryCooldown(float delay)
    {
        yield return new WaitForSeconds(delay);
        primaryReadyToShoot = true;
        resetPrimaryRoutine = null; //clear ref
    }

    void Update()
    {
        MyInput();
        //Vector3 forward = orientation.TransformDirection(Vector3.forward) * 10;
        //Debug.DrawRay(transform.position, forward, Color.green);
    }
}
