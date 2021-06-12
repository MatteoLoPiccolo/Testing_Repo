using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField]
    private GameObject _enemyPrefab;
    [SerializeField]
    private GameObject[] _powerupsPrefab;
    [SerializeField]
    private GameObject _enemyContainer;

    private bool _stopSpawning;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(SpawnEnemyRoutine());
        StartCoroutine(SpawnPowerupRoutine());
    }

    IEnumerator SpawnEnemyRoutine()
    {
        while (!_stopSpawning)
        {
            var posToSpawn = new Vector3(Random.Range(8f, -8f), 7, 0);
            var newEnemy = Instantiate(_enemyPrefab, posToSpawn, Quaternion.identity);
            newEnemy.transform.parent = _enemyContainer.transform;
            yield return new WaitForSeconds(5f);
        }
    }

    IEnumerator SpawnPowerupRoutine()
    {
        while (!_stopSpawning)
        {
            var randomDelay = Random.Range(3f, 7f);
            var randomPowerup = Random.Range(0, _powerupsPrefab.Length);
            var posToSpawn = new Vector3(Random.Range(8f, -8f), 7, 0);
            Instantiate(_powerupsPrefab[randomPowerup], posToSpawn, Quaternion.identity);
            yield return new WaitForSeconds(randomDelay);
        }
    }

    public void OnPlayerDeath()
    {
        _stopSpawning = true;
    }
}
