using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    private float _speed = 3.5f;
    [SerializeField]
    private GameObject _laserPrefab;
    [SerializeField]
    private GameObject _tripleShotPrefab;
    [SerializeField]
    private GameObject _shieldVisualizer;
    [SerializeField]
    private GameObject _rightEngine;
    [SerializeField]
    private GameObject _leftEngine;
    [SerializeField]
    private AudioClip _laserClip;
    [SerializeField]
    private float _fireRate = 0.5f;
    [SerializeField]
    private int _lives = 3;
    [SerializeField]
    private int _score;

    private AudioSource _audioSource;
    private UIManager _uIManager;
    private SpawnManager _spawnManager;
    private bool _isTripleShotIsActive;
    private bool _isPowerBoostIsActive;
    private bool _isShieldIsActive;
    private float _speedMultiplier = 2;
    private float _canFire = -1;


    // Start is called before the first frame update
    void Start()
    {
        _rightEngine.SetActive(false);
        _leftEngine.SetActive(false);

        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();
        _uIManager = GameObject.Find("Canvas").GetComponent<UIManager>();
        _audioSource = GetComponent<AudioSource>();

        if (_uIManager == null)
            Debug.LogError("The Spawn Manager is NULL");

        if (_spawnManager == null)
            Debug.LogError("The UI Manager is NULL");

        if (_audioSource == null)
            Debug.LogError("The Audiosource is NULL");
        else
            _audioSource.clip = _laserClip;

        transform.position = Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {
        CalculateMovement();

        if (Input.GetKeyDown(KeyCode.Space) && Time.time > _canFire)
        {
            FireLaser();
        }
    }

    public void FireLaser()
    {
        _canFire = Time.time + _fireRate;

        if (_isTripleShotIsActive == true)
            Instantiate(_tripleShotPrefab, transform.position, Quaternion.identity);
        else
            Instantiate(_laserPrefab, transform.position + new Vector3(0, 1.05f, 0), Quaternion.identity);

        _audioSource.Play();
    }

    private void CalculateMovement()
    {
        float horizonatalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 direction = new Vector3(horizonatalInput, verticalInput, 0);

        transform.Translate(direction * _speed * Time.deltaTime);

        transform.position = new Vector3(transform.position.x, Mathf.Clamp(transform.position.y, -3.8f, 0), transform.position.z);

        if (transform.position.x >= 11.5f)
            transform.position = new Vector3(-11.5f, transform.position.y, transform.position.z);
        else if (transform.position.x <= -11.5f)
            transform.position = new Vector3(11.5f, transform.position.y, transform.position.z);
    }

    public void Damage()
    {
        if (_isShieldIsActive == true)
        {
            _isShieldIsActive = false;
            _shieldVisualizer.SetActive(false);
            return;
        }

        _lives--;

        switch (_lives)
        {
            case 2:
                _rightEngine.SetActive(true);
                break;
            case 1:
                _leftEngine.SetActive(true);
                break;
            default:
                break;
        }

        _uIManager.UpdateLives(_lives);

        if (_lives < 1)
        {
            _spawnManager.OnPlayerDeath();
            Destroy(gameObject);
        }
    }

    public void TripleShotActive()
    {
        _isTripleShotIsActive = true;
        StartCoroutine(TripleShotPowerDownRoutine());
    }

    public void SpeedBoostActive()
    {
        _isPowerBoostIsActive = true;
        _speed *= _speedMultiplier;
        StartCoroutine(SpeedBoostPowerDownRoutine());
    }

    public void ShieldIsActive()
    {
        _isShieldIsActive = true;
        _shieldVisualizer.SetActive(true);
    }

    IEnumerator TripleShotPowerDownRoutine()
    {
        yield return new WaitForSeconds(5.0f);
        _isTripleShotIsActive = false;
    }

    IEnumerator SpeedBoostPowerDownRoutine()
    {
        yield return new WaitForSeconds(5.0f);
        _isPowerBoostIsActive = false;
        _speed /= _speedMultiplier;
    }

    public void AddScore(int points)
    {
        _score += points;
        _uIManager.UpdateScore(_score);
    }
}
