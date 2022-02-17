using UnityEngine;
using Mirror;
using Rewired;
using UnityEngine.SceneManagement;
public class Bullet : NetworkBehaviour
{

    [SerializeField] private float speed = 10f;

    private Vector3 up;

    private void Start()
    {

    }

    private void Update()
    {
        transform.position += transform.forward * speed * Time.deltaTime;
    }

}
