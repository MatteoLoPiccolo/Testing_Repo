using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    private static PoolManager _instance;

    public static PoolManager Instance 
    { 
        get 
        {
            if (_instance == null)
                Debug.LogError("Pool Manager is null");

            return _instance;
        }
    }

    [SerializeField]
    private GameObject _container;
    [SerializeField]
    private GameObject _laserPrefab;
    [SerializeField]
    private List<GameObject> _laserPool;

    private void Awake()
    {
        _instance = this;
    }

    private void Start()
    {
        _laserPool = _GenerateLaser(15);
    }


    List<GameObject> _GenerateLaser(int amountOfLaser)
    {
        for (int i = 0; i < amountOfLaser; i++)
        {
            GameObject laser = Instantiate(_laserPrefab);
            laser.transform.parent = _container.transform;
            laser.SetActive(false);
            _laserPool.Add(laser);
        }

        return _laserPool;
    }

    public GameObject RequestLaser()
    {
        foreach (var laser in _laserPool)
        {
            if (laser.activeInHierarchy == false)
            {
                laser.SetActive(true);
                return laser;
            }
        }

        GameObject newLaser = Instantiate(_laserPrefab);
        newLaser.transform.parent = _container.transform;
        newLaser.SetActive(false);
        _laserPool.Add(newLaser);

        return newLaser;
    }
}
