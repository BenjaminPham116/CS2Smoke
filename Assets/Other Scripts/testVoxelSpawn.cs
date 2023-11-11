using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testVoxelSpawn : MonoBehaviour
{
    public GameObject VoxelManager;
    public GameObject Voxel;
    public GameObject Grenade;
    public float maxTimer;
    public float timer;

    public float maxTimer2;
    public float timer2;
    public float throwForce = 5;
    private GameObject thrownGrenade;
    private GameObject thrownGrenade2;
    public Transform[] spawnPoints;
    private GameObject activeManager;

    public float r = 10;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    IEnumerator spawnSmoke(Vector3 position)
    {
        if (activeManager != null)
        {
            activeManager.GetComponent<VoxelManager>().Delete();
            Destroy(activeManager.gameObject);
            activeManager = null;
        }
        yield return null;
        activeManager = Instantiate(VoxelManager, position, Quaternion.identity, null);
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            StopAllCoroutines();
            StartCoroutine(spawnSmoke(spawnPoints[0].position));
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            StopAllCoroutines();
            StartCoroutine(spawnSmoke(spawnPoints[1].position));
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            StopAllCoroutines();
            StartCoroutine(spawnSmoke(spawnPoints[2].position));
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            StopAllCoroutines();
            StartCoroutine(spawnSmoke(spawnPoints[3].position));
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            StopAllCoroutines();
            StartCoroutine(spawnSmoke(spawnPoints[4].position));
        }

        if (Input.GetKeyDown(KeyCode.G))
        {
            if(timer < 0)
            {
                timer = maxTimer;
                thrownGrenade = Instantiate(Grenade, transform.position + transform.forward * 1, Quaternion.identity);
                thrownGrenade.GetComponent<Rigidbody>().AddForce(Camera.main.transform.forward * throwForce);
            }
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (timer2 < 0)
            {
                timer2 = maxTimer2;
                thrownGrenade2 = Instantiate(Grenade, transform.position + transform.forward * 1, Quaternion.identity);
                thrownGrenade2.GetComponent<Rigidbody>().AddForce(Camera.main.transform.forward * throwForce);
            }
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            if (activeManager)
            {
                activeManager.GetComponent<VoxelManager>().Delete();
                Destroy(activeManager.gameObject);
                activeManager = null;
            }
            
            Application.LoadLevel(Application.loadedLevel);
        }
        timer -= Time.deltaTime;
        if(timer < 0 && thrownGrenade)
        {
            StopAllCoroutines();
            StartCoroutine(spawnSmoke(thrownGrenade.transform.position));
            Destroy(thrownGrenade);
            thrownGrenade = null;
        }

        timer2 -= Time.deltaTime;
        if (timer2 < 0 && thrownGrenade2)
        {
            activeManager.GetComponent<VoxelManager>().SphereIntersection(thrownGrenade2.transform.position, r);
            Destroy(thrownGrenade2);
            thrownGrenade2 = null;
        }
    }
}
