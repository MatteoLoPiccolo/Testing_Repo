using UnityEngine;

public class AmmoPackPowerup : Powerup
{
    #region Variables
    private int _addingAmmoLaser;
    #endregion

    protected override void Start()
    {
        base.Start();
        _player = GameObject.Find("Player").GetComponent<Player>();
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
            if (base._player != null)
                base._player.AddAmmo(_addingAmmoLaser, _player.AmmoCount);

            AudioSource.PlayClipAtPoint(_pickUpClip, transform.position + new Vector3(0, 0, -10));
            Destroy(gameObject);
        }
    }
}