using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoPackPowerup : Powerup
{
    private int _addingAmmoLaser;

    protected override void Start()
    {
        base.Start();
        _addingAmmoLaser = Random.Range(5, 10);
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
                _player.AddAmmo(_addingAmmoLaser);

            AudioSource.PlayClipAtPoint(_pickUpClip, transform.position + new Vector3(0, 0, -10));
            Destroy(gameObject);
        }
    }
}