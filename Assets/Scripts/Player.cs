using UnityEngine;
using Mirror;
using Rewired;
using UnityEngine.SceneManagement;
public class Player : NetworkBehaviour
{

    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float fireRate = 0.6f;
    [SerializeField] private float freezeAfterFire = 0.2f;
    [SerializeField] private float invicilityAfterHit = 1f;
    [SerializeField] private GameObject bullet;
    [SerializeField] private GameObject firePosition;
    [SerializeField] private GameObject muzzlePrefab;

    [SerializeField]
    [SyncVar]
    private float health = 100;
    private Vector3 up;

    // Cached variables
    private Rewired.Player rwPlayer;
    private Animator animator;
    private Blink blink;

    // State
    private Vector3 direction = Vector3.zero;
    private Vector3 fireDirection = Vector3.zero;

    [SyncVar]
    public bool dead = false;

    // Timers
    private float fireRateTimer = 0f;
    private float freezeAfterFireTimer = 0f;
    private float invicibilityTimer = 0f;

    public bool Dead { get => dead; set => dead = value; }

    private void Start()
    {
        Debug.Log("START");
        rwPlayer = ReInput.players.GetPlayer(0);
        animator = GetComponentInChildren<Animator>();
        blink = GetComponent<Blink>();

        var gameManager = FindObjectOfType<GameManager>();
        if (gameManager && isServer)
        {
            gameManager.PlayerJoined(this);
        }
    }

    private void Update()
    {
        if (!isLocalPlayer)
        {
            return;
        }

        if (Dead)
        {
            direction = Vector3.zero;
            animator.SetBool("IsDead", true);
            return;
        }

        RegisterInputs();
        ManageTimers();
        Look();
        Fire();

        animator.SetBool("IsDead", Dead);
        animator.SetBool("IsIdle", direction == Vector3.zero);
        animator.SetBool("IsRunning", direction != Vector3.zero);
        animator.SetBool("IsFiring", freezeAfterFireTimer > 0);
    }

    private void FixedUpdate()
    {
        if (freezeAfterFireTimer <= 0)
        {
            Move();
        }
    }

    private void ManageTimers()
    {
        if (invicibilityTimer > 0)
        {
            invicibilityTimer -= Time.deltaTime;
        }

        if (fireRateTimer > 0)
        {
            fireRateTimer -= Time.deltaTime;
        }

        if (freezeAfterFireTimer > 0)
        {
            freezeAfterFireTimer -= Time.deltaTime;
        }
    }

    private void RegisterInputs()
    {
        var directionX = rwPlayer.GetAxis("Move Horizontal");
        var directionZ = rwPlayer.GetAxis("Move Vertical");
        var direction = new Vector3(directionX, 0, directionZ).normalized;

        var fireDirectionX = rwPlayer.GetAxis("Fire Horizontal");
        var fireDirectionZ = rwPlayer.GetAxis("Fire Vertical");
        var fireDirection = new Vector3(fireDirectionX, 0, fireDirectionZ).normalized;

        this.direction = direction;
        this.fireDirection = fireDirection;
    }

    private void Fire()
    {
        var buttonPressed = rwPlayer.GetButton("Fire") || fireDirection != Vector3.zero;
        if (buttonPressed && fireRateTimer <= 0)
        {
            fireRateTimer = fireRate;
            freezeAfterFireTimer = freezeAfterFire;
            DoFire(firePosition.transform.position, transform.rotation);
        }
    }

    private void Look()
    {
        if (direction == Vector3.zero)
        {
            return;
        }

        // var matrix = Matrix4x4.Rotate(Quaternion.Euler(0, 45, 0));
        // var skewedInput = matrix.MultiplyPoint3x4(direction);

        var rot = Quaternion.LookRotation(direction, Vector3.up);

        transform.rotation = rot;

    }

    private void Move()
    {
        if (direction == Vector3.zero)
        {
            return;
        }

        transform.position += transform.forward * moveSpeed * Time.deltaTime;
    }

    [Command]
    private void DoFire(Vector3 position, Quaternion rotation)
    {
        var fire = Instantiate(bullet, position, rotation);
        NetworkServer.Spawn(fire);

        if (muzzlePrefab != null)
        {
            var muzzle = Instantiate(muzzlePrefab, position, rotation);
            NetworkServer.Spawn(muzzle);
        }

    }


    [ClientRpc]
    public void TakeDamage(float damage)
    {
        if (invicibilityTimer > 0)
        {
            return;
        }

        Debug.Log("TAKE");

        this.health -= damage;
        invicibilityTimer = invicilityAfterHit;
        blink.doBlink(0.3f);
        if (health <= 0)
        {
            CmdDie();
            Debug.Log("DIE");
        }
    }

    [Command]
    private void CmdDie()
    {
        Dead = true;
    }

    [ClientRpc]
    public void InitPosition(Vector3 pos)
    {
        this.transform.position = pos;
    }
}
