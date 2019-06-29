using UnityEngine;
using System.Collections;
using Gamekit3D;

public class Missile : MonoBehaviour
{

    public Transform target;
    public LayerMask layerMask;

    public float detonationDistance = 5f;

    public float lifeTime = 10f; // Missile life time
    public float despawnDelay = 1f; // Delay despawn in ms
    public float velocity = 30; // Missile velocity
    public float alignSpeed = 4f;
    public float RaycastAdvance = 2f; // Raycast advance multiplier

    public bool DelayDespawn = false; // Missile despawn flag

    public ParticleSystem[] delayedParticles; // Array of delayed particles
    private ParticleSystem[] particles; // Array of Missile particles

    private new Transform transform; // Cached transform

    private bool isHit = false; // Missile hit flag
    private bool isFXSpawned = false; // Hit FX prefab spawned flag

    private float timer = 0f; // Missile timer

    private Vector3 targetLastPos;
    private Vector3 step;

    public GrenadierBehaviour launcher;

    private void Awake()
    {
        // Cache transform and get all particle systems attached
        transform = GetComponent<Transform>();
        particles = GetComponentsInChildren<ParticleSystem>();
    }

    // OnSpawned called by pool manager 
    public void OnSpawned()
    {
        isHit = false;
        isFXSpawned = false;
        timer = 0f;
        targetLastPos = Vector3.zero;
        step = Vector3.zero;
    }


    private void OnHit()
    {
        isHit = true;
    }

    private void Update()
    {
        // If something was hit
        if (isHit)
        {
            // Execute once
            if (!isFXSpawned)
            {
                // spawns explosion on hit
                launcher.SpawnExplosion(transform.position);
                isFXSpawned = true;
                gameObject.SetActive(false);
            }
        }
        // No collision occurred yet
        else
        {
            // Navigate
            if (target != null)
            {
                transform.rotation = Quaternion.Lerp(transform.rotation,
                    Quaternion.LookRotation(target.position - transform.position), Time.deltaTime * alignSpeed);
            }

            // Missile step per frame based on velocity and time
            step = transform.forward * Time.deltaTime * velocity;

            if (target != null && Vector3.SqrMagnitude(transform.position - target.position) <= detonationDistance)
            {
                OnHit();
            }

            // Nothing hit
            else
            {
                // Despawn missile at the end of life cycle
                if (timer >= lifeTime)
                {
                    // Do not detonate
                    isFXSpawned = true;
                    OnHit();
                }
            }

            // Advances missile forward
            transform.position += step;
        }

        // Updates missile timer
        timer += Time.deltaTime;
    }
}