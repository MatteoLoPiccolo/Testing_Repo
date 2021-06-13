using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    private float _speed = 3.5f;
    [SerializeField]
    private int _points = 10;

    private Player _player;
    private Animator _anim;
    private Collider2D _collider2D;

    private void Start()
    {
         _player = GameObject.Find("Player").GetComponent<Player>();
        _anim = GetComponent<Animator>();
        _collider2D = GetComponent<Collider2D>();

        if (_player == null)
            Debug.LogError("Player is NULL!");

        if (_anim == null)
            Debug.LogError("Animator is NULL!");

        if (_collider2D == null)
            Debug.LogError("Collider2D is NULL!");
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.down * _speed * Time.deltaTime);

        if (transform.position.y < -5f)
        {
            float randomX = Random.Range(8f, -8f);
            transform.position = new Vector3(randomX, 7);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            if (_player != null)
                _player.Damage();

            _anim.SetTrigger("OnEnemyDeath");
            _speed = 0;
            _collider2D.enabled = false;
            Destroy(gameObject, 2.8f);
        }

        if (other.tag == "Laser")
        {
            Destroy(other.gameObject);

            if (_player != null)
                _player.AddScore(_points);

            _anim.SetTrigger("OnEnemyDeath");
            _speed = 0;
            _collider2D.enabled = false;
            Destroy(gameObject, 2.8f);
        }
    }
}
