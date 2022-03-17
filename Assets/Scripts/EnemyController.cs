using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System.Linq;

public class EnemyController : NetworkBehaviour
{
    // Config parameters
    [SerializeField] private float moveSpeed = 10;
    [SerializeField] private float turnSpeed = 10;
    [SerializeField] private GameObject deathParticleSystem;
    [SerializeField] private float damage;
    [SerializeField] private GameObject healthBar;

    [SyncVar]
    private float health;

    [SerializeField]
    private float maxHealth = 100f;

    [SerializeField] private float blinkDuration = 0.2f;
    [SerializeField] private GameObject damageDisplayer;
    private Player[] players;


    // Cached variables
    private Rigidbody body;
    private Blink blink;

    void Start()
    {
        body = GetComponent<Rigidbody>();
        blink = GetComponent<Blink>();
        health = maxHealth;
        players = FindObjectsOfType<Player>();
    }

    void OnCollisionStay(Collision other)
    {
        if (!isServer)
        {
            return;
        }

        if (other.gameObject.tag == Tags.Player)
        {
            var player = other.gameObject.GetComponent<Player>();

            if (player != null)
            {
                player.TakeDamage(this.damage);
            }
        }
    }

    private void KnockBack(float value)
    {
        transform.position += -transform.forward * value;
    }

    public void TakeDamage(float damage, float knockBack)
    {
        health = Mathf.Max(0, health - damage);
        healthBar.transform.localScale = new Vector3(health / maxHealth, healthBar.transform.localScale.y, healthBar.transform.localScale.z);

        if (health <= 0)
        {
            NetworkServer.Destroy(this.gameObject);
            var seed = Random.Range(1, 20000);
            EmitDeathParticles((uint)seed);
        }
        else
        {
            KnockBack(knockBack);
            DoBlink();
        }
    }

    [ClientRpc]
    private void DoBlink()
    {
        blink.doBlink(blinkDuration);
    }

    // Update is called once per frame
    void Update()
    {
        if (!isServer)
        {
            return;
        }

        body.velocity = Vector3.zero;

        var closest = FindClosestPlayer();
        if (closest != null)
        {
            var targetPos = new Vector3(closest.transform.position.x, this.transform.position.y, closest.transform.position.z);
            transform.position = Vector3.MoveTowards(transform.position, targetPos, Time.deltaTime * moveSpeed);
            transform.LookAt(targetPos);
        }
    }

    public void EmitDeathParticles(uint seed)
    {
        var particles = Instantiate(deathParticleSystem, transform.position, transform.rotation);
        NetworkServer.Spawn(particles);
    }

    public void SetPlayers(Player[] players)
    {
        this.players = players;
    }

    private Player FindClosestPlayer()
    {
        float minDistance = 100000000;
        Player minPlayer = null;
        var filtered = players.Where(pl => !pl.Dead);
        foreach (var player in filtered)
        {
            var distance = Vector3.Distance(transform.position, player.transform.position);
            if (!player.Dead && distance < minDistance)
            {
                minPlayer = player;
                minDistance = distance;
            }
        }

        return minPlayer;
    }

}
