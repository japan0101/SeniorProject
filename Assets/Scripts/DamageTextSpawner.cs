using UnityEngine;
using TMPro;

public class DamageTextSpawner : MonoBehaviour
{
    [Header("Settings")]
    public float textDisplayTime = 1.5f;
    public Vector3 textOffset = new Vector3(0, 2f, 0);
    public Color textColor = Color.red;
    public int fontSize = 36;

    [Header("References")]
    public GameObject textPrefab; // Prefab with TextMeshPro component

    private void OnCollisionEnter(Collision collision)
    {
        // Check if collision is with PlayerAttack layer
        if (collision.gameObject.layer == 9)
        {
            MainBullet bullet = collision.gameObject.GetComponent<MainBullet>();
            if (bullet != null)
            {
                SpawnDamageText(collision.contacts[0].point, bullet.damage);
            }
        }
    }

    private void SpawnDamageText(Vector3 position, float damage)
    {
        // Create text object
        GameObject textObj = Instantiate(textPrefab, position + textOffset, Quaternion.identity);
        TextMeshPro tmp = textObj.GetComponent<TextMeshPro>();

        // Configure text
        tmp.text = damage.ToString();
        tmp.fontSize = fontSize;
        tmp.color = textColor;
        tmp.alignment = TextAlignmentOptions.Center;

        // Make text face camera
        textObj.transform.LookAt(Camera.main.transform);
        textObj.transform.Rotate(0, 180f, 0); // Flip to face correctly

        // Destroy after delay
        Destroy(textObj, textDisplayTime);

        // Optional: Add floating animation
        //LeanTween.moveY(textObj, textObj.transform.position.y + 1f, textDisplayTime)
        //    .setEase(LeanTweenType.easeOutQuad);
    }
}