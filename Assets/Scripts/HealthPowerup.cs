using UnityEngine;

public class HealthPowerup : Powerup
{
    #region Variables
    private int _live = 1;
    #endregion

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
                _player.AddLives(_live);

            AudioSource.PlayClipAtPoint(_pickUpClip, transform.position + new Vector3(0, 0, -10));
            Destroy(gameObject);
        }
    }
}