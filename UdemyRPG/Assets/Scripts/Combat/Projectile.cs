using RPG.Resources;
using UnityEngine;
using UnityEngine.Events;

namespace RPG.Combat
{
    public class Projectile : MonoBehaviour
    {
        [SerializeField] float speed = 2f;
        [SerializeField] bool chaseProjectile = false;
        [SerializeField] GameObject hitEffect = null;
        [SerializeField] GameObject[] destroyOnHit = null;
        [SerializeField] float lifeAfterImpact = 0f;
        [SerializeField] UnityEvent lauchEvent = null;
        [SerializeField] UnityEvent impactEvent = null;
        Health target = null;
        Health casterHealth = null;
        Vector3 location;
        float damage = 0f;
        bool missed;
        float timeToRemove = 0;

        private void Awake() {
            if(lauchEvent != null){
                lauchEvent.Invoke();
            } 
        }

        private void Start()
        {
            transform.LookAt(location);
        }

        private void Update()
        {
            if (target != null)
            {
                transform.Translate(Vector3.forward * speed * Time.deltaTime);
                if (!missed)
                {
                    if (chaseProjectile && !target.IsDead())
                    {
                        transform.LookAt(GetAimLocation());
                    }
                    else
                    {
                        float absoluteDistante = Vector3.Distance(location, transform.position);
                        if (absoluteDistante <= 0.2)
                        {
                            missed = true;
                        }
                    }
                }
            }
            timeToRemove += Time.deltaTime;
            if (timeToRemove > 4f)
            {
                DestroyProjectileImmediately();
            }
        }

        private void DestroyProjectileImmediately()
        {
            DestroyProjectile(0f);
        }

        private void DestroyProjectile(float time)
        {
            Destroy(gameObject, time);
            target = null;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (target != null && !target.IsDead())
            {
                Health colliderHealth = other.GetComponent<Health>();
                if (colliderHealth != null && colliderHealth != casterHealth)
                {
                    if(impactEvent != null){
                        impactEvent.Invoke();
                    }
                    colliderHealth.TakeDamage(damage, casterHealth.gameObject);
                    if (hitEffect != null)
                    {
                        Instantiate(hitEffect, GetTargetAimPosition(colliderHealth), transform.rotation);
                    }
                    foreach (GameObject objectToDestroy in destroyOnHit)
                    {
                        Destroy(objectToDestroy);
                    }
                    DestroyProjectile(lifeAfterImpact);
                }
            }
        }

        public void SetTarget(Health target, Health casterHealth, float damage)
        {
            this.target = target;
            this.damage = damage;
            this.casterHealth = casterHealth;
            location = GetAimLocation();
        }

        private Vector3 GetAimLocation()
        {
            return GetTargetAimPosition(target);
        }

        private Vector3 GetTargetAimPosition(Health givenTarget)
        {
            Vector3 rootPosition = givenTarget.transform.position;
            CapsuleCollider collider = givenTarget.GetComponent<CapsuleCollider>();
            if (collider == null)
            {
                return rootPosition;
            }
            return rootPosition + Vector3.up * collider.height / 2;
        }
    }
}