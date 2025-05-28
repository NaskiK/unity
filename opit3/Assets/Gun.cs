using UnityEngine;

public class Gun : MonoBehaviour
{
    public Transform cam; // This can be your "Camera" parent GameObject
    public Transform firePoint;
    public float damage = 20f;
    public float range = 100f;
    public float fireRate = 10f;
    public ParticleSystem muzzleFlash;
    public GameObject hitEffect;

    private float nextTimeToFire = 0f;

    void Update()
    {
        if (Input.GetButton("Fire1") && Time.time >= nextTimeToFire)
        {
            nextTimeToFire = Time.time + 1f / fireRate;
            Shoot();
        }
    }

    void Shoot()
    {
        if (muzzleFlash) muzzleFlash.Play();

        // Use cam's forward direction
        Ray ray = new Ray(cam.position, cam.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, range))
        {
            Debug.Log("Hit: " + hit.collider.name);

            Enemy target = hit.transform.GetComponent<Enemy>();
            if (target != null)
            {
                target.TakeDamage(damage);
            }

            if (hitEffect)
            {
                GameObject impactGO = Instantiate(hitEffect, hit.point, Quaternion.LookRotation(hit.normal));
                Destroy(impactGO, 1f);
            }
        }
    }
}
