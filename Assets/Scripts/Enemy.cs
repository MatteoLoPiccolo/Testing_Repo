using UnityEngine;

public class Enemy : MonoBehaviour
{
    #region Variables
    [SerializeField]
    private float _speed = 3.5f;
    [SerializeField]
    private int _points = 10;
    [SerializeField]
    private GameObject _laserPrefab;
    [SerializeField]
    private AudioClip _explosionClip;

    private Player _player;
    private Animator _anim;
    private AudioSource _audiosource;
    private Collider2D _collider2D;
    private float _fireRate = 3.0f;
    private float _canFire = -1;
    private bool _enemyFire = true;
    #endregion

    private void Start()
    {
         _player = GameObject.Find("Player").GetComponent<Player>();
        _anim = GetComponent<Animator>();
        _audiosource = GetComponent<AudioSource>();
        _collider2D = GetComponent<Collider2D>();

        if (_player == null)
            Debug.LogError("Player is NULL!");

        if (_anim == null)
            Debug.LogError("Animator is NULL!");

        if (_collider2D == null)
            Debug.LogError("Collider2D is NULL!");

        if (_audiosource == null)
            Debug.LogError("Audiosource is NULL!");
        else
            _audiosource.clip = _explosionClip;
    }

    // Update is called once per frame
    void Update()
    {
        CalculateMovement();

        if (Time.time > _canFire && _enemyFire)
        {
            _fireRate = Random.Range(3.0f, 7.0f);
            _canFire = Time.time + _fireRate;
            var enemyLaser = Instantiate(_laserPrefab, transform.position, Quaternion.identity);
            Laser[] lasers = enemyLaser.GetComponentsInChildren<Laser>();
            foreach (var laser in lasers)
                laser.AssignEnemyLaser();
        }
    }

    private void CalculateMovement()
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

            _enemyFire = false;
            _anim.SetTrigger("OnEnemyDeath");
            _audiosource.Play();
            _speed = 0;
            _collider2D.enabled = false;
            Destroy(gameObject, 2.8f);
        }

        if (other.tag == "Laser")
        {
            Destroy(other.gameObject);

            if (_player != null)
                _player.AddScore(_points);

            _enemyFire = false;
            _anim.SetTrigger("OnEnemyDeath");
            _fireRate = 100;
            _audiosource.Play();
            AudioSource.PlayClipAtPoint(_explosionClip, transform.position + new Vector3(0, 0, -10));
            _speed = 0;
            _collider2D.enabled = false;
            Destroy(gameObject, 2.8f);
        }

        if (other.tag == "Cannon")
        {
            if (_player != null)
                _player.AddScore(_points);

            _enemyFire = false;
            _anim.SetTrigger("OnEnemyDeath");
            _speed = 0;
            _audiosource.Play();
            _collider2D.enabled = false;
            Destroy(gameObject, 2.8f);
        }
    }
}