using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class DestroyAfter : NetworkBehaviour
{

    [SerializeField] private float time = 3f;

    private float timer = 0f;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;

        if (isServer && timer >= time)
        {
            NetworkServer.Destroy(this.gameObject);
        }
    }
}
