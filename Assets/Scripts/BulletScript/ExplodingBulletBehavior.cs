using System;
using UnityEngine;
using UnityEngine.VFX;

public class ExplodingHit : OnHitBehavior
{
    [Header("BulletAttribute")]
    public float blastRadius;
    public float blastDamageMultiplier;
    [Header("References")]
    [SerializeField] public VisualEffect blastVFX;
    private void Awake()
    {
        // Auto-get the VFX from a child object
        if (blastVFX == null)
        {
            blastVFX = GetComponentInChildren<VisualEffect>(true); // 'true' includes inactive
        }
        else
        {
            Debug.LogError("No VisualEffect found in children!", this);
        }
    }

    public override void OnBulletHit(Collision other, Vector3 lastVelocity)
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, blastRadius);
        //play explosion Visual effect on hitting collision
        if (blastVFX != null)
        {
            blastVFX.SetFloat("Power", GetComponent<MainBullet>().damage);
            blastVFX.SetFloat("Radius", blastRadius);
            blastVFX.SendEvent("OnExplode");
        }
        else
        {
            Debug.LogWarning("VFX references not set!");
        }
        foreach (var hitCollider in hitColliders)
        {   
            // Check if the object has a specific component if needed
            if (hitCollider.GetComponent<BaseEnemy>() != null)
            {
                hitCollider.GetComponent<BaseEnemy>().TakeDamage(hitCollider.transform.position, GetComponent<MainBullet>().damage * blastDamageMultiplier);
                Debug.Log("Enemy detected: " + hitCollider.name);
            }
        }
        GetComponentInChildren<MeshRenderer>().enabled = false;
        GetComponentInChildren<Collider>().enabled = false;
        Destroy(gameObject, 0.4f);
    }

    public void Update()
    {

    }
}
