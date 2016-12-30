using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ZombieSpawner : MonoBehaviour
{
    public GameObject[] zombies;
    public float waveDuration = 60f * 2;
    public int numZombiesIncrement = 10;
    public int numZombies = 20;
    public int numZones = 2;
    public Transform player;
    public float radius = 100;

    void Start()
    {
        StartCoroutine(WaveCoroutime());
    }

    public IEnumerator WaveCoroutime()
    {
        SpawnWave();

        while (true)
        {
            numZombies += numZombiesIncrement;
            waveDuration += 20f;

            for (int i = 0; i < numZombies; i++)
            {
                yield return new WaitForSeconds(waveDuration / numZombies);

                var closestSpawnPoints = new List<vp_SpawnPoint>();

                closestSpawnPoints = vp_SpawnPoint.SpawnPoints;
                closestSpawnPoints.Sort((x, y) =>
                    {
                        float xd = Vector3.Distance(x.transform.position, player.transform.position);
                        float yd = Vector3.Distance(y.transform.position, player.transform.position);

                        if (xd > yd)
                            return 1;
                        if (xd < yd)
                            return -1;
                        return 0;
                    });

                while (true)
                {
                    int nz = numZones;
                    vp_SpawnPoint spawnPoint = closestSpawnPoints[Random.Range(0, nz)];
                    vp_Placement p = spawnPoint.GetPlacement(2f);
                    Vector3 screenPoint = Camera.main.WorldToViewportPoint(p.Position);
                    bool onScreen = screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1;                    
                    if (onScreen)
                    {
                        continue;
                    }
                    GameObject prefab = zombies[Random.Range(0, zombies.Length)];
                    Vector3 rot = p.Rotation.eulerAngles;
                    rot.x = 0;
                    rot.z = 0;
                    Vector3 position = p.Position;
                    position.y = 4f;
                    GameObject.Instantiate(prefab, position, Quaternion.Euler(rot));
                    break;
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
	
    }

    public void SpawnWave()
    {
        Debug.Log("New Wave");

        var closestSpawnPoints = new List<vp_SpawnPoint>();

        closestSpawnPoints = vp_SpawnPoint.SpawnPoints;
        closestSpawnPoints.Sort((x, y) =>
            {
                float xd = Vector3.Distance(x.transform.position, player.transform.position);
                float yd = Vector3.Distance(y.transform.position, player.transform.position);

                if (xd > yd)
                    return 1;
                if (xd < yd)
                    return -1;
                return 0;
            });
                
        int numZombiesPerSpawnPoint = Mathf.Max(1, numZombies / numZones);

        for (int i = 0; i < numZones; i++)
        {
            vp_SpawnPoint spawnPoint = closestSpawnPoints[i];
            for (int j = 0; j < numZombiesPerSpawnPoint; j++)
            {
                vp_Placement p = spawnPoint.GetPlacement(2f);
                GameObject prefab = zombies[Random.Range(0, zombies.Length)];
                Vector3 rot = p.Rotation.eulerAngles;
                rot.x = 0;
                rot.z = 0;
                GameObject.Instantiate(prefab, p.Position, Quaternion.Euler(rot));
            }
        }
    }
}
