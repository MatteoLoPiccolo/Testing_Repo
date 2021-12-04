using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    [SerializeField]
    private float _speed = 4.5f;
    [SerializeField]
    private Enemy _enemy;
    [SerializeField]
    private GameObject _laserPrefab;
    [SerializeField]
    private GameObject _tripleShotPrefab;
    [SerializeField]
    private GameObject _cannonPrefab;
    [SerializeField]
    private GameObject _shieldVisualizer;
    [SerializeField]
    private GameObject _rightEngine;
    [SerializeField]
    private GameObject _leftEngine;
    [SerializeField]
    private AudioClip _laserClip;
    [SerializeField]
    private CameraShake _cam;
    [SerializeField]
    private float _fireRate = 0.5f;
    [SerializeField]
    private int _lives = 3;
    [SerializeField]
    private int _score;
    [SerializeField]
    private float _acceleration = 10f;
    [SerializeField]
    private float _maxSpeed = 15f;
    [SerializeField]
    private int _ammoCount = 15;

    private AudioSource _audioSource;
    private UIManager _uIManager;
    private SpawnManager _spawnManager;
    private ThrusterHud _thrusterHud;
    private float _startingSpeed = 4.5f;
    private bool _isTripleShotActive;
    private bool _isShieldActive;
    private bool _isCannonActive;
    private bool _isThrusterActive;
    private float _speedMultiplier = 2;
    private float _canFire = -1;
    private int _shieldLives = 3;
    private int _currentAmmo;


    // Start is called before the first frame update
    void Start()
    {
        _cannonPrefab.SetActive(false);

        //_rightEngine.SetActive(false);
        //_leftEngine.SetActive(false);

        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();
        _thrusterHud = GameObject.Find("Canvas").GetComponentInChildren<ThrusterHud>();
        _uIManager = GameObject.Find("Canvas").GetComponent<UIManager>();
        _cam = GameObject.Find("Main Camera").GetComponent<CameraShake>();
        _audioSource = GetComponent<AudioSource>();

        if (_uIManager == null)
            Debug.LogError("The Spawn Manager is NULL");

        if (_spawnManager == null)
            Debug.LogError("The UI Manager is NULL");

        if (_thrusterHud == null)
            Debug.LogError("The Thruster HUD is NULL");

        if (_audioSource == null)
            Debug.LogError("The Audiosource is NULL");
        else
            _audioSource.clip = _laserClip;

        transform.position = Vector3.zero;
        _speed = _startingSpeed;
        _currentAmmo = _ammoCount;
    }

    // Update is called once per frame
    void Update()
    {
        Movement();

        if (Input.GetKeyDown(KeyCode.Space) && Time.time > _canFire)
        {
            FireLaser();
        }
    }

    public void FireLaser()
    {
        _canFire = Time.time + _fireRate;

        if (_currentAmmo >= 1)
        {
            _currentAmmo--;
            _uIManager.UpdateAmmoCount(_currentAmmo);

            if (_isTripleShotActive)
                Instantiate(_tripleShotPrefab, transform.position, Quaternion.identity);
            if (_isCannonActive)
            {
                _cannonPrefab.SetActive(true);
            }
            else
                Instantiate(_laserPrefab, transform.position + new Vector3(0, 1.05f, 0), Quaternion.identity);

            _audioSource.Play();
        }
        else
            _currentAmmo = 0;
    }

    private void Movement()
    {
        CalculateMovement();
        Thruster();
        Bounderies();
    }

    private void CalculateMovement()
    {
        float horizonatalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 direction = new Vector3(horizonatalInput, verticalInput, 0);
        transform.Translate(direction * _speed * Time.deltaTime);
    }

    private void Bounderies()
    {
        var screenXWrap = 11.5f;
        var lowYPos = -3.8f;

        transform.position = new Vector3(transform.position.x, Mathf.Clamp(transform.position.y, lowYPos, 0), transform.position.z);

        if (transform.position.x >= screenXWrap)
            transform.position = new Vector3(-screenXWrap, transform.position.y, transform.position.z);
        else if (transform.position.x <= -screenXWrap)
            transform.position = new Vector3(screenXWrap, transform.position.y, transform.position.z);
    }

    private void Thruster()
    {
        if (Input.GetKey(KeyCode.LeftShift) && _speed < _maxSpeed)
        {
            _isThrusterActive = true;
            _speed += (_acceleration * Time.deltaTime);
        }
        else if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            _isThrusterActive = false;
        }

        if (_isThrusterActive == false)
        {
            if (_speed <= _startingSpeed)
            {
                _speed = _startingSpeed;
                return;
            }

            _speed -= (_acceleration * Time.deltaTime);
        }

        _thrusterHud.SetThrusterSliderValue(_speed);
    }

    public void Damage()
    {
        if (_isShieldActive)
        {
            _shieldLives--;
            var shieldColor = _shieldVisualizer.GetComponent<SpriteRenderer>();

            switch (_shieldLives)
            {
                case 2:
                    shieldColor.color = Color.green;
                    break;
                case 1:
                    shieldColor.color = Color.red;
                    break;
                case 0:
                    _isShieldActive = false;
                    _shieldVisualizer.SetActive(false);
                    shieldColor.color = new Color(1, 1, 1, 1);
                    break;
                default:
                    break;
            }

            return;
        }

        _lives--;
        _uIManager.UpdateLives(_lives);
        EngineDamageVisualization();
        _cam.CamPlayerDamage();

        if (_lives < 1)
        {
            _spawnManager.OnPlayerDeath();
            Destroy(gameObject);
        }
    }

    private void EngineDamageVisualization()
    {
        switch (_lives)
        {
            case 3:
                _rightEngine.SetActive(false);
                _leftEngine.SetActive(false);
                break;
            case 2:
                _rightEngine.SetActive(true);
                _leftEngine.SetActive(false);
                break;
            case 1:
                _leftEngine.SetActive(true);
                break;
            default:
                break;
        }
    }

    public void TripleShotActive()
    {
        _isTripleShotActive = true;
        StartCoroutine(TripleShotPowerDownRoutine());
    }

    public void CannonActive()
    {
        _isCannonActive = true;
        StartCoroutine(CannonPowerDownRoutine());
    }

    public void SpeedBoostActive()
    {
        _speed *= _speedMultiplier;
        StartCoroutine(SpeedBoostPowerDownRoutine());
    }

    public void ShieldIsActive()
    {
        if (!_isShieldActive)
            _shieldLives = 3;

        _isShieldActive = true;
        _shieldVisualizer.SetActive(true);
    }

    IEnumerator TripleShotPowerDownRoutine()
    {
        yield return new WaitForSeconds(5.0f);
        _isTripleShotActive = false;
    }

    IEnumerator SpeedBoostPowerDownRoutine()
    {
        yield return new WaitForSeconds(5.0f);
        _speed = _startingSpeed;
    }

    IEnumerator CannonPowerDownRoutine()
    {
        yield return new WaitForSeconds(5.0f);
        _cannonPrefab.SetActive(false);
        _isCannonActive = false;
    }

    public void AddAmmo(int ammo)
    {
        _currentAmmo += ammo;

        if (_currentAmmo > _ammoCount)
            _currentAmmo = _ammoCount;
        
        _uIManager.UpdateAmmoCount(_currentAmmo);
    }

    public void AddLives(int live)
    {
        if (_lives == 3)
            return;

        _lives += live;

        EngineDamageVisualization();
        _uIManager.UpdateLives(_lives);
    }

    public void AddScore(int points)
    {
        _score += points;
        _uIManager.UpdateScore(_score);
    }
}
