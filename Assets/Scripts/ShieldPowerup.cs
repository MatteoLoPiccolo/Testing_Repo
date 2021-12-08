using UnityEngine;

public class ShieldPowerup : Powerup
{
    #region Variables
    private Camera _cam;
    #endregion

    protected override void Start()
    {
        base.Start();
        _cam = GameObject.Find("Main Camera").GetComponent<Camera>();

        if (_cam == null)
            Debug.LogError("The Main Camera is NULL!");
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
                _player.ShieldIsActive();

            AudioSource.PlayClipAtPoint(_pickUpClip, _cam.transform.position);
            Destroy(gameObject);
        }
    }
}