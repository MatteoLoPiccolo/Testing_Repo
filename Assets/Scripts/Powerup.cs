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

    private void Start()
    {
        _cam = GameObject.Find("Main Camera").GetComponent<Camera>();

        if (_cam == null)
            Debug.LogError("The Main Camera is NULL!");
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
            AudioSource.PlayClipAtPoint(_pickUpClip, _cam.transform.position, 0.5f);
            var player = other.transform.GetComponent<Player>();

            if (player != null)
            {
                switch (_powerupId)
                {
                    case 0:
                        player.TripleShotActive();
                        break;
                    case 1:
                        player.SpeedBoostActive();
                        break;
                    case 2:
                        player.ShieldIsActive();
                        break;
                    default:
                        break;
                }
            }

            Destroy(gameObject);
        }
    }
}
