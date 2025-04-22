using System;
using UnityEngine;
using UnityEngine.VFX;

public class ExplodingHit : OnHitBehavior
{
    [Header("BulletAttribute")]
    public float blastRadius;
    public float blastDamageMultiplier;
    public float blastKnockback;
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
                hitCollider.GetComponent<BaseEnemy>().TakeDamage(hitCollider.ClosestPoint(transform.position), GetComponent<MainBullet>().damage * blastDamageMultiplier);
                //Debug.Log("Enemy detected: " + hitCollider.name);
            }
            if (hitCollider.GetComponent<PlayerHealth>() != null)
            {
                hitCollider.GetComponent<PlayerHealth>().TakeDamage(hitCollider.ClosestPoint(transform.position), (GetComponent<MainBullet>().damage * blastDamageMultiplier) / 2);
                //Debug.Log("Enemy detected: " + hitCollider.name);
            }
            if (hitCollider.GetComponent<Rigidbody>() != null) {
                //Debug.Log("RigidBody: " + hitCollider.name);
                Ray ray = new Ray(transform.position, hitCollider.ClosestPoint(transform.position) - transform.position);
                RaycastHit hit;
                Vector3 targetPoint;

                // Check if ray hits something
                if (Physics.Raycast(ray, out hit, blastRadius))
                {
                    // Use the hit point if we hit something
                    targetPoint = hit.point;
                    //Debug.Log("Hit: " + hit.collider.name);
                }
                else
                {
                    // Use the ray's end point if nothing was hit
                    targetPoint = ray.origin + ray.direction * blastRadius;
                }

                //get traget direction
                Vector3 direction = targetPoint - transform.position;
                hitCollider.GetComponent<Rigidbody>().AddForce(direction * blastKnockback, ForceMode.Impulse);


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
