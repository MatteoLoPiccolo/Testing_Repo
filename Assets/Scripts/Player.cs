using System;
using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    #region Variables
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
    private AudioClip _emptyLaserClip;
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
    private float _speedBoostAcceleration = 6.5f;

    public int AmmoCount { get { return _ammoCount; } }

    private CameraShake _cam;
    private AudioSource _audioSource;
    private ThrusterHud _thrusterHud;
    private float _startingSpeed = 4.5f;
    private bool _isTripleShotActive;
    private bool _isShieldActive;
    private bool _isSpeedBoostActive;
    private bool _isCannonActive;
    private bool _isThrusterActive;
    private float _canFire = -1;
    private int _shieldLives = 3;
    private int _currentAmmo;
    #endregion

    #region Events
    public static event Action<int> OnScoreUpdate;
    public static Action<int, int> OnAmmoUpdate;
    public static Action<int> OnLiveUpdate;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        _cannonPrefab.SetActive(false);

        _rightEngine.SetActive(false);
        _leftEngine.SetActive(false);

        _thrusterHud = GameObject.Find("Canvas").GetComponentInChildren<ThrusterHud>();
        _cam = GameObject.Find("Main Camera").GetComponent<CameraShake>();
        _audioSource = GetComponent<AudioSource>();

        if (_thrusterHud == null)
            Debug.LogError("The Thruster HUD is NULL");

        if (_audioSource == null)
            Debug.LogError("The Audiosource is NULL");

        transform.position = Vector3.zero;
        _speed = _startingSpeed;
        _currentAmmo = _ammoCount;
    }

    // Update is called once per frame
    void Update()
    {
        Movement();

        if (Input.GetKeyDown(KeyCode.Space) && Time.time > _canFire)
            FireLaser();
    }

    public void FireLaser()
    {
        _canFire = Time.time + _fireRate;

        if (_currentAmmo >= 1)
        {
            _currentAmmo--;
            UIManager.Instance.UpdateAmmoCount(_currentAmmo, _ammoCount);

            if (_isTripleShotActive)
                Instantiate(_tripleShotPrefab, transform.position, Quaternion.identity);
            if (_isCannonActive)
                _cannonPrefab.SetActive(true);

            Instantiate(_laserPrefab, transform.position + new Vector3(0, 1.05f, 0), Quaternion.identity);

            _audioSource.clip = _laserClip;
            _audioSource.Play();
        }

        if (_currentAmmo == 0)
        {
            _currentAmmo = 0;
            _audioSource.clip = _emptyLaserClip;
            _audioSource.Play();
        }
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
        if (Input.GetKey(KeyCode.LeftShift) && _speed <= _maxSpeed && !_isSpeedBoostActive)
        {
            _isThrusterActive = true;
            _speed += (_acceleration * Time.deltaTime);

            if (_speed >= _maxSpeed)
                _speed = _maxSpeed;
        }

        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            _isThrusterActive = false;
            _speed -= (_acceleration * Time.deltaTime);
        }

        if (_isThrusterActive == false && !_isSpeedBoostActive)
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
        UIManager.Instance.UpdateLives(_lives);
        EngineDamageVisualization();
        _cam.CamPlayerDamage();

        if (_lives < 1)
        {
            SpawnManager.Instance.OnPlayerDeath();
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
        _isSpeedBoostActive = true;
        _speed += _speedBoostAcceleration;
        _thrusterHud.SetThrusterSliderValue(_speed);
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
        yield return new WaitForSeconds(3.0f);
        _isSpeedBoostActive = false;
        _speed = _startingSpeed;
        _thrusterHud.SetThrusterSliderValue(_speed);
    }

    IEnumerator CannonPowerDownRoutine()
    {
        yield return new WaitForSeconds(3.0f);
        _cannonPrefab.SetActive(false);
        _isCannonActive = false;
    }

    public void AddAmmo(int ammo, int totalAmmo)
    {
        _ammoCount = totalAmmo;
        _currentAmmo += ammo;

        if (_currentAmmo > _ammoCount)
            _currentAmmo = _ammoCount;

        OnAmmoUpdate?.Invoke(_currentAmmo, AmmoCount);
        //UIManager.Instance.UpdateAmmoCount(_currentAmmo, _ammoCount);
    }

    public void AddLives(int live)
    {
        if (_lives == 3)
            return;

        _lives += live;

        EngineDamageVisualization();
        OnLiveUpdate?.Invoke(_lives);
        //UIManager.Instance.UpdateLives(_lives);
    }

    public void AddScore(int points)
    {
        _score += points;
        OnScoreUpdate?.Invoke(_score);
        //UIManager.Instance.UpdateScore(_score);
    }
}