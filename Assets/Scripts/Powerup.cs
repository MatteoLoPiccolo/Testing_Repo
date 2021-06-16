using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powerup : MonoBehaviour
{
    [SerializeField]
    private float _speed = 3.0f;
    [SerializeField]
    private int _powerupId;
    [SerializeField]
    private AudioClip _pickUpClip;

    private Camera _cam;
    private Player _player;
    private int _addingAmmoLaser;
    private int _addLivePowerup = 1;

    private void Start()
    {
        _cam = GameObject.Find("Main Camera").GetComponent<Camera>();
        _player = GameObject.Find("Player").GetComponent<Player>();

        if (_cam == null)
            Debug.LogError("The Main Camera is NULL!");

        if (_player == null)
            Debug.LogError("Player is NULL!");

        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.down * _speed * Time.deltaTime);

        if (transform.position.y < -4.5f)
            Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (_player != null)
            {
                switch (_powerupId)
                {
                    case 0:
                        _player.TripleShotActive();
                        break;
                    case 1:
                        _player.SpeedBoostActive();
                        break;
                    case 2:
                        _player.ShieldIsActive();
                        break;
                    case 3:
                        _addingAmmoLaser = Random.Range(6, 10);
                        _player.AddAmmo(_addingAmmoLaser);
                        break;
                    case 4:
                        _player.AddLives(_addLivePowerup);
                        break;
                    case 5:
                        _player.CannonActive();
                        break;
                    default:
                        break;
                }
            }

            AudioSource.PlayClipAtPoint(_pickUpClip, _cam.transform.position);
            Destroy(gameObject);
        }
    }
}
