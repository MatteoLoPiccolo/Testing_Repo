using UnityEngine;

public class Powerup : MonoBehaviour
{
    #region Variables
    [SerializeField]
    protected float _speed = 3.0f;
    [SerializeField]
    protected int _powerupId;
    [SerializeField]
    protected AudioClip _pickUpClip;

    protected Player _player;
    #endregion

    protected virtual void Start()
    {
        _player = GameObject.Find("Player").GetComponent<Player>();

        if (_player == null)
            Debug.LogError("Player is NULL!");
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        transform.Translate(Vector3.down * _speed * Time.deltaTime);

        if (transform.position.y < -4.5f)
            Destroy(gameObject);
    }
}