using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedPowerup : Powerup
{
    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (_player != null)
                _player.SpeedBoostActive();

            AudioSource.PlayClipAtPoint(_pickUpClip, transform.position + new Vector3(0, 0, -10));
            Destroy(gameObject);
        }
    }
}