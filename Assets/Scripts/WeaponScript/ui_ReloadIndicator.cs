using UnityEngine;
using UnityEngine.UI;

public class ui_ReloadIndicator : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [Header("References")] public Image reloadingOverlay;

    [SerializeField] private PlayerShoot playerShoot;

    [Header("Settings")]
    //[SerializeField] private Color reloadColor = Color.yellow;
    [SerializeField]
    private Color cooldownColor = Color.white;

    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
        if (playerShoot.IsReloading())
            //reloadingOverlay.color = reloadColor;
            reloadingOverlay.fillAmount = playerShoot.GetReloadProgress();
        else
            reloadingOverlay.fillAmount = 0;
    }
}