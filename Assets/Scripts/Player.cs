using UnityEngine;
using Mirror;
using Rewired;
using UnityEngine.SceneManagement;
public class Player : NetworkBehaviour
{

    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float fireRate = 0.4f;
    [SerializeField] private GameObject bullet;
    [SerializeField] private GameObject firePosition;

    private Vector3 up;

    // Cached variables
    private Rewired.Player rwPlayer;
    private Animator animator;

    // State
    private Vector3 direction = Vector3.zero;

    private void Start()
    {
        rwPlayer = ReInput.players.GetPlayer(0);
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (SceneManager.GetActiveScene().name == "Lobby Scene")
        {
            gameObject.SetActive(false);
            return;
        }

        if (!isLocalPlayer)
        {
            return;
        }

        RegisterInputs();
        Look();
        if (rwPlayer.GetButtonDown("Fire"))
        {
            DoFire(firePosition.transform.position, transform.rotation);
        }

        animator.SetBool("IsIdle", direction == Vector3.zero);
        animator.SetBool("IsRunning", direction != Vector3.zero);
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void RegisterInputs()
    {
        var directionX = rwPlayer.GetAxis("Move Horizontal");
        var directionZ = rwPlayer.GetAxis("Move Vertical");
        var direction = new Vector3(directionX, 0, directionZ).normalized;

        this.direction = direction;
    }

    private void Look()
    {
        if (direction == Vector3.zero)
        {
            return;
        }

        var matrix = Matrix4x4.Rotate(Quaternion.Euler(0, 45, 0));
        var skewedInput = matrix.MultiplyPoint3x4(direction);

        var rot = Quaternion.LookRotation(skewedInput, Vector3.up);

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
    }
}
