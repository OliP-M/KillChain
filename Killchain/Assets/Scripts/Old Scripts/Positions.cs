using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Positions : MonoBehaviour
{
    private Vector3[] posArray;
    private List<int> takenPositions = new List<int>();
    private List<Vector3> posList = new List<Vector3>();
    private int[] cover;

    // Start is called before the first frame update
    void Start()
    {
        // Creates a copy of the empty gameobject at every available position
        for (int x = -40; x < 0; x++)
        {
            for (int z = -40; z < 0; z++)
            {
                if (NavMesh.SamplePosition(new Vector3(x, 0, z), out NavMeshHit hit, 0.1f, NavMesh.AllAreas))
                {
                    posList.Add(new Vector3(x, 0, z));
                }
            }
        }

        posArray = posList.ToArray();
        //posArray = GetComponentsInChildren<Transform>();
        //GameObject.Find("LevelController").GetComponent<LevelController>().SetPositions(posArray);

        int count = 0;
        cover = new int[posArray.Length];

        //COVER CALCULATION
        foreach (Vector3 pos in posArray)
        {
            int coverCounter = 0;
            Vector3 coverPos = pos + Vector3.up * 0.5f;
            if (Physics.Raycast(coverPos, Vector3.right, 1) && !Physics.Raycast(coverPos + Vector3.up, Vector3.right, 1))
            {
                coverCounter += 1;
            }
            if (Physics.Raycast(coverPos, -Vector3.right, 1) && !Physics.Raycast(coverPos + Vector3.up, -Vector3.right, 1))
            {
                coverCounter += 1;
            }
            if (Physics.Raycast(coverPos, Vector3.forward, 1) && !Physics.Raycast(coverPos + Vector3.up, Vector3.forward, 1))
            {
                coverCounter += 1;
            }
            if (Physics.Raycast(coverPos, -Vector3.forward, 1) && !Physics.Raycast(coverPos + Vector3.up, -Vector3.right, 1))
            {
                coverCounter += 1;
            }
            if (coverCounter > 3)
            {
                Debug.Log("Error with cover counting at pos: " + pos);
            }
            cover[count] = coverCounter * 10;
            count += 1;
        }
    }

    public Vector3[] GetPositions()
    {
        return posArray;
    }

    public int[] GetCover()
    {
        return cover;
    }

    public List<int> GetTaken()
    {
        return takenPositions;
    }

    public bool ClaimPosition(int pos)
    {
        if (!takenPositions.Contains(pos))
        {
            takenPositions.Add(pos);
            return true;
        }
        else
        {
            return false;
        }
    }

    public void ReleasePosition(int pos)
    {
        takenPositions.Remove(pos);
    }
}