using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    [SerializeField]
    private float shakeAmount = 0.3f;
    [SerializeField]
    private float _camShakeDuration = 0.3f;

    private Transform _camTransform;
    private Vector3 _originPos;
    private bool _isPlayerDamage;

    // Start is called before the first frame update
    void Start()
    {
        _camTransform = GetComponent<Transform>();

        if (_camTransform == null)
            Debug.LogError("TRansform is NULL!");

        _originPos = _camTransform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (_isPlayerDamage)
            StartCoroutine(Shake());
    }

    IEnumerator Shake()
    {
        _camTransform.position = _originPos + Random.insideUnitSphere * shakeAmount;
        yield return new WaitForSeconds(_camShakeDuration);
        _isPlayerDamage = false;
    }

    public void CamPlayerDamage()
    {
        _isPlayerDamage = true;
        StartCoroutine(Shake());
    }
}
