using System;
using UnityEngine;
using UnityEngine.VFX;

public class ExplodingHit : OnHitBehavior
{
    [Header("BulletAttribute")]
    public float blastRadius;
    [Header("References")]
    [SerializeField] public VisualEffect blastVFX;
    public VFXEventAttribute blastAttribute;
    private void Awake()
    {
        // Auto-get the VFX from a child object
        if (blastVFX == null)
        {
            blastVFX = GetComponentInChildren<VisualEffect>(true); // 'true' includes inactive
        }

        if (blastVFX != null)
        {
            blastAttribute = blastVFX.CreateVFXEventAttribute();
        }
        else
        {
            Debug.LogError("No VisualEffect found in children!", this);
        }
    }

    public override void OnBulletHit(Collision other, Vector3 lastVelocity)
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, blastRadius);
        if (blastVFX != null && blastAttribute != null)
        {
            //blastAttribute.SetFloat("Power", GetComponent<MainBullet>().damage);
            //Debug.Log(blastAttribute.GetFloat("Power"));
            blastVFX.SetFloat("Power", GetComponent<MainBullet>().damage);
            blastVFX.SetFloat("Radius", blastRadius);
            blastVFX.SendEvent("OnExplode", blastAttribute);
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
                hitCollider.GetComponent<BaseEnemy>().TakeDamage(hitCollider.transform.position, GetComponent<MainBullet>().damage);
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
