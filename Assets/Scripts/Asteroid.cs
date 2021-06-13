using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Asteroid : MonoBehaviour
{
    [SerializeField]
    private float _speedRotation = 15f;

    private Animator _anim;
    private SpawnManager _spawnManager;
    private CircleCollider2D _circleCollider2;

    // Start is called before the first frame update
    void Start()
    {
        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();
        _circleCollider2 = GetComponent<CircleCollider2D>();
        _anim = GetComponent<Animator>();

        if (_spawnManager == null)
            Debug.LogError("Spawn Manager is NULL!");

        if (_circleCollider2 == null)
            Debug.LogError("CircleCollider2D is NULL!");

        if (_anim == null)
            Debug.LogError("Animator is NULL!");

        transform.position = new Vector3(0, 4, 0);
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.forward * _speedRotation * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Laser"))
        {
            Destroy(other.gameObject);
            _circleCollider2.enabled = false;
            _anim.SetTrigger("OnAsteroidDeath");
            _spawnManager.StartSpawning();
            Destroy(gameObject, 2.8f);
        }
    }
}
