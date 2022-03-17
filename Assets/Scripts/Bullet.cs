using UnityEngine;
using Mirror;
public class Bullet : NetworkBehaviour
{

    [SerializeField] private float speed = 10f;

    [SerializeField] private float damage = 40f;
    [SerializeField] private float knockBack = 0.2f;
    [SerializeField] private GameObject hitPrefab;

    private Vector3 up;
    private new Renderer renderer;

    public float Damage { get => damage; set => damage = value; }
    public float KnockBack { get => knockBack; set => knockBack = value; }

    private void OnCollisionEnter(Collision other)
    {
        if (!isServer)
        {
            return;
        }

        var ennemy = other.gameObject.GetComponent<EnemyController>();

        if (ennemy != null)
        {
            var go = Instantiate(hitPrefab, transform.position, Quaternion.identity);
            NetworkServer.Spawn(go);
            ennemy.TakeDamage(this.Damage, this.KnockBack);
            NetworkServer.Destroy(this.gameObject);
        }
    }

    private void Start()
    {
        renderer = GetComponent<Renderer>();
    }

    private void Update()
    {
        transform.position += transform.forward * speed * Time.deltaTime;

        // if (!renderer.isVisible)
        // {
        //NetworkServer.Destroy(this.gameObject);
        // }

    }

}
