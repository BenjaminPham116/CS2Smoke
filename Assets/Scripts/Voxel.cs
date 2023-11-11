using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Voxel : MonoBehaviour
{
    public GameObject voxel;

    public float density;
    public bool expandable;

    // Start is called before the first frame update
    public void expand(Queue<Voxel> fillableVoxels)
    {

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                for (int z = -1; z <= 1; z++)
                {
                    if (x != 0 || y != 0 || z != 0)
                    {
                        fill(new Vector3(x, y, z), fillableVoxels);
                    }
                }
            }
        }
    }
    public bool fill(Vector3 dir, Queue<Voxel> fillableVoxels)
    {
        Ray ray = new Ray(transform.position, dir);
        RaycastHit hit;
        if (!Physics.Raycast(ray, out hit, 2.5f))
        {
            Voxel v = Instantiate(voxel, transform.position + dir, Quaternion.identity, transform.parent).GetComponent<Voxel>();
            fillableVoxels.Enqueue(v);
            v.expandable = true;
            return true;
        }
        else
        {
            if (!hit.transform.CompareTag("Voxel"))
            {
                Voxel v = Instantiate(voxel, transform.position + dir, Quaternion.identity, transform.parent).GetComponent<Voxel>();
                fillableVoxels.Enqueue(v);
                v.expandable = false;
                return true;
            }
        }
        return false;
    }
}
