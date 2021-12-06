using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Asteroid : MonoBehaviour
{
    [SerializeField]
    private float _speedRotation = 15f;
    [SerializeField]
    private AudioClip _explosionClip;

    private Animator _anim;
    private CircleCollider2D _circleCollider2;
    private AudioSource _audioSource;

    // Start is called before the first frame update
    void Start()
    {
        _circleCollider2 = GetComponent<CircleCollider2D>();
        _anim = GetComponent<Animator>();
        _audioSource = GetComponent<AudioSource>();

        if (_circleCollider2 == null)
            Debug.LogError("CircleCollider2D is NULL!");

        if (_anim == null)
            Debug.LogError("Animator is NULL!");

        if (_audioSource == null)
            Debug.LogError("Audiosource is NULL!");
        else
            _audioSource.clip = _explosionClip;

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
            _audioSource.Play();
            SpawnManager.Instance.StartSpawning();
            Destroy(gameObject, 2.8f);
        }
    }
}
