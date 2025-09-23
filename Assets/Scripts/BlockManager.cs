using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Rendering;

public class BlockManager : MonoBehaviour
{
    public List<Vector3> blockPositions = new List<Vector3>();

    void Start()
    {
        FindAllBlocks();
    }

    void FindAllBlocks()
    {
        GameObject[] blocks = GameObject.FindGameObjectsWithTag("Block");

        foreach (GameObject block in blocks)
        {
            Vector3 localPosition = block.transform.localPosition;
            blockPositions.Add(localPosition);

            Debug.Log($"Block: {block.name}, Local position: {localPosition}");
        }

        if (blocks.Length == 0)
        {
            Debug.LogWarning("Нет объектов");
        }
    }
}
