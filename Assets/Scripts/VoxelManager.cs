using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoxelManager : MonoBehaviour
{
    public GameObject voxel;
    public List<Voxel> voxels;
    public Queue<Voxel> fillableVoxels;
    public float density;
    public bool finished = false;

    public float regrowRate = .8f;
    Coroutine expand;
    public Material mat;

    //Light Data
    private Vector3 sunDir;
    public Light sun;

    //Sphere Data
    private Vector3 sp = Vector3.zero;
    private float r = 5;

    //Grid Data
    public bool[,,] grid;
    public Color[,,] densities;
    public Color[,,] sunlight;
    public Texture3D densityBuffer;
    public Texture3D sunBuffer;
    public Vector3 lowBound;
    public Vector3 highBound;

    //constants
    Color on = new Color(1, 1, 1, 1);
    Color off = new Color(0, 0, 1, 0);
    Color explode = new Color(0, 0, 0, -.5f);

    public float fireRate = .05f;
    private float fireBuffer;
    private Ray testRay;

    void Awake()
    {
        sun = GameObject.FindGameObjectWithTag("Sun").GetComponent<Light>();
        sunDir = -(sun.transform.forward);
        transform.position = new Vector3(Mathf.Floor(transform.position.x), 
                                         Mathf.Floor(transform.position.y + 1),
                                         Mathf.Floor(transform.position.z));
        lowBound = transform.position;
        highBound = transform.position;
        voxels = new List<Voxel>();
        fillableVoxels = new Queue<Voxel>();

        Voxel start = Instantiate(voxel, transform.position, Quaternion.identity, transform.parent).GetComponent<Voxel>();
        start.expandable = true;
        fillableVoxels.Enqueue(start);

        expand = StartCoroutine(Disperse());
        fireBuffer = 0;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(sp, r);
        Gizmos.DrawRay(testRay);
    }
    private void Update()
    {
        if (finished)
        {
            CalcSun();
            IncreaseDensity();
            fireBuffer -= Time.deltaTime;
            if (Input.GetKeyDown(KeyCode.R))
            {
                sp = Camera.main.transform.position;
                SphereIntersection(sp, r);
            }
            if (Input.GetKey(KeyCode.Mouse0) && fireBuffer < 0)
            {
                fireBuffer = fireRate;
                testRay = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
                RayIntersection(testRay);
            }
        }
    }

    //Iterate over each cell in the bounding box and check if it has an
    //uninterrupted path to the sun
    private void CalcSun()
    {

        sunDir = -(sun.transform.forward);
        Vector3 center = Vector3.one * .5f;
        for (int x = 0; x < grid.GetLength(0); x++)
        {
            for (int y = 0; y < grid.GetLength(1); y++)
            {
                for (int z = 0; z < grid.GetLength(2); z++)
                {
                    if (grid[x, y, z])
                    {
                        RaycastHit hit;
                        Ray ray = new Ray(lowBound + new Vector3(x,y,z) + center, sunDir);
                        testRay = ray;
                        if (Physics.Raycast(ray, out hit, 1000, 1, QueryTriggerInteraction.Collide))
                        {
                            sunlight[x, y, z] = off;
                        }
                        else
                        {
                            sunlight[x, y, z] = on;
                        }
                    }
                }
            }
        }
    }

    //Regenerates voxels when they are faded by bullets or grenades
    private void IncreaseDensity()
    {
        for (int x = 0; x < grid.GetLength(0); x++)
        {
            for (int y = 0; y < grid.GetLength(1); y++)
            {
                for (int z = 0; z < grid.GetLength(2); z++)
                {
                    if (grid[x, y, z])
                    {
                        densities[x, y, z].a = Mathf.Min(densities[x, y, z].a + Time.deltaTime * regrowRate, 1);
                    }
                }
            }
        }
        UpdateGrid();
    }

    //Checks if pos in the bounding box
    private bool CheckInBounds(Vector3 pos)
    {
        if (pos.x < lowBound.x || pos.y < lowBound.y || pos.z < lowBound.z)
        {
            return false;
        }
        if (pos.x > highBound.x || pos.y > highBound.y || pos.z > highBound.z)
        {
            return false;
        }

        return true;
    }

    //Fade all voxels intersected by the given ray
    private void RayIntersection(Ray ray)
    {
        float dist;
        for (int x = 0; x < grid.GetLength(0); x++)
        {
            Plane plane = new Plane(Vector3.right, lowBound + Vector3.right * x);
            if(plane.Raycast(ray, out dist))
            {
                Vector3 hit = ray.GetPoint(dist);
                if (CheckInBounds(hit))
                {
                    hit -= lowBound;
                    //hit neighbors
                    for(int i = 0; i <= 1; i++)
                    {
                        for (int j = 0; j <= 1; j++)
                        {
                            if (hit.y + i >= 0 && hit.y + i < grid.GetLength(1) &&
                                hit.z + j >= 0 && hit.z + j < grid.GetLength(2))
                            {
                                densities[(int)hit.x, (int)hit.y + i, (int)hit.z + j].a = Mathf.Min(densities[(int)hit.x, (int)hit.y + i, (int)hit.z + j].a, 0);
                            }
                        }
                    }
                }
            }
        }
        for (int y = 0; y < grid.GetLength(1); y++)
        {
            Plane plane = new Plane(Vector3.up, lowBound + Vector3.up * y);
            if (plane.Raycast(ray, out dist))
            {
                Vector3 hit = ray.GetPoint(dist);
                if (CheckInBounds(hit))
                {
                    hit -= lowBound;
                    //hit neighbors
                    for (int i = 0; i <= 1; i++)
                    {
                        for (int j = 0; j <= 1; j++)
                        {
                            if (hit.x + i >= 0 && hit.x + i < grid.GetLength(0) &&
                                hit.z + j >= 0 && hit.z + j < grid.GetLength(2))
                            {
                                densities[(int)hit.x + i, (int)hit.y, (int)hit.z + j].a = Mathf.Min(densities[(int)hit.x + i, (int)hit.y, (int)hit.z + j].a, 0);
                            }
                        }
                    }
                }
            }
        }
        for (int z = 0; z < grid.GetLength(2); z++)
        {
            Plane plane = new Plane(Vector3.forward, lowBound + Vector3.forward * z);
            if (plane.Raycast(ray, out dist))
            {
                Vector3 hit = ray.GetPoint(dist);
                if (CheckInBounds(hit))
                {
                    hit -= lowBound;
                    //hit neighbors
                    for (int i = 0; i <= 1; i++)
                    {
                        for (int j = 0; j <= 1; j++)
                        {
                            if (hit.x + i >= 0 && hit.x + i < grid.GetLength(0) &&
                                hit.y + j >= 0 && hit.y + j < grid.GetLength(1))
                            {
                                densities[(int)hit.x + i, (int)hit.y + j, (int)hit.z].a = Mathf.Min(densities[(int)hit.x + i, (int)hit.y + j, (int)hit.z].a, 0);
                            }
                        }
                    }
                }
            }
        }

        UpdateGrid();
    }

    //Fade all voxels in radius around sPos
    public void SphereIntersection(Vector3 sPos, float radius)
    {
        for(int x = 0; x < grid.GetLength(0); x++)
        {
            for (int y = 0; y < grid.GetLength(1); y++)
            {
                for (int z = 0; z < grid.GetLength(2); z++)
                {
                    Vector3 pos = new Vector3(x, y, z);
                    pos += Vector3.one * .5f;
                    pos += lowBound;
                    if (Vector3.Distance(sPos, pos) < radius) densities[x, y, z] = explode;
                }
            }
        }

        UpdateGrid();
    }

    //Update bounding box to include pos
    private void UpdateBound(Vector3 pos)
    {
        lowBound.x = Mathf.Min(lowBound.x, pos.x - 1);
        lowBound.y = Mathf.Min(lowBound.y, pos.y - 1);
        lowBound.z = Mathf.Min(lowBound.z, pos.z - 1);

        highBound.x = Mathf.Max(highBound.x, pos.x + 2);
        highBound.y = Mathf.Max(highBound.y, pos.y + 2);
        highBound.z = Mathf.Max(highBound.z, pos.z + 2);

    }

    

    // Update is called once per frame
    private void UpdateGrid()
    {
        for (int x = 0; x < grid.GetLength(0); x++)
        {
            for (int y = 0; y < grid.GetLength(1); y++)
            {
                for (int z = 0; z < grid.GetLength(2); z++)
                {
                    sunBuffer.SetPixel(x, y, z, sunlight[x, y, z]);
                    densityBuffer.SetPixel(x, y, z, densities[x,y,z]);
                }
            }
        }
        sunBuffer.Apply();
        densityBuffer.Apply();
        mat.SetVector("_SunDir", sunDir);
        mat.SetTexture("_SunBuffer", sunBuffer);
        mat.SetTexture("_Grid", densityBuffer);
        mat.SetVector("_Origin", lowBound);
    }

    //Initializes the grid and all its buffers
    private void BuildGrid()
    {
        Vector3 dim = highBound - lowBound;
        
        grid = new bool[(int)dim.x, (int)dim.y, (int)dim.z];
        densities = new Color[(int)dim.x, (int)dim.y, (int)dim.z];
        sunlight = new Color[(int)dim.x, (int)dim.y, (int)dim.z];

        densityBuffer = new Texture3D(grid.GetLength(0), grid.GetLength(1), grid.GetLength(2), TextureFormat.RGBA32, false);
        sunBuffer = new Texture3D(grid.GetLength(0), grid.GetLength(1), grid.GetLength(2), TextureFormat.RGBA32, false);

        foreach (Voxel v in voxels)
        {
            grid[(int)(v.transform.position.x - lowBound.x), (int)(v.transform.position.y - lowBound.y), (int)(v.transform.position.z - lowBound.z)] = true;
        }

        for (int x = 0; x < grid.GetLength(0); x++)
        {
            for (int y = 0; y < grid.GetLength(1); y++)
            {
                for (int z = 0; z < grid.GetLength(2); z++)
                {
                    densities[x, y, z] = grid[x, y, z] ? on : off;
                }
            }
        }

        mat.SetVector("_Dim", dim);
            CalcSun();
        UpdateGrid();
    }

    //Place voxels and expand bounding box
    IEnumerator Disperse()
    {
        float radius = 1;
        while (density > 0 && fillableVoxels.Count > 0)
        {
            float count = fillableVoxels.Count;
            for (int i = 0; i < count; i++)
            {
                Voxel v = fillableVoxels.Dequeue();
                if (Vector3.Dot(v.transform.position - transform.position, v.transform.position - transform.position) <= radius * radius)
                {
                    voxels.Add(v);
                    UpdateBound(v.transform.position);
                    density--;


                    if (density <= 0)  break; 
                    if (v.expandable) v.expand(fillableVoxels);
                }
                else
                {
                    fillableVoxels.Enqueue(v);
                }
            }

            BuildGrid();
            radius++;
            yield return new WaitForSeconds(.05f);
        }
        while (fillableVoxels.Count > 0)
        {
            Voxel v = fillableVoxels.Dequeue();
            Destroy(v.gameObject);
        }
        BuildGrid();
        finished = true;
    }

    //Destroy the smoke
    public void Delete()
    {
        StopAllCoroutines();
        for (int x = 0; x < grid.GetLength(0); x++)
        {
            for (int y = 0; y < grid.GetLength(1); y++)
            {
                for (int z = 0; z < grid.GetLength(2); z++)
                {
                     densities[x, y, z] = off;
                }
            }
        }
        UpdateGrid();
        while (fillableVoxels.Count > 0)
        {
            Voxel v = fillableVoxels.Dequeue();
            Destroy(v.gameObject);
        }
        foreach (Voxel v in voxels)
        {
            Destroy(v.gameObject);
        }
    }
}
 