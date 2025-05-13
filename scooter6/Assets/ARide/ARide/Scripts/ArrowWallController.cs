using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowWallController : MonoBehaviour
{
    public GameObject arrowPrefab; // Prefab for the arrow
    public int arrowCount = 20;    // Number of arrows in one row
    public int rowCount = 1;    // Number of arrow-rows in one wall
    public float horizontalSpacing = 0.25f; // horizontal spacing between arrows
    public float verticalSpacing = 0.25f; // vertical spacing between arrows
    public float rotationAngle = 0f; // Rotation of Arrows in Wall

    void Start()
    {
        CreateArrowWall();
    }

    //spawn desired amount of arrows
    void CreateArrowWall()
    {
        for (int r = 0; r < rowCount; r++)
        {
            for (int i = 0; i < arrowCount; i++)
            {
                Vector3 position = new Vector3(-0.5f * arrowCount * horizontalSpacing + i * horizontalSpacing, -0.5f * (rowCount - 1) * verticalSpacing + r * verticalSpacing, 0);
                GameObject arrow = Instantiate(arrowPrefab, transform.position + position, Quaternion.identity, transform);
                // Rotate arrow to point in the desired direction
                arrow.transform.rotation = Quaternion.Euler(0, rotationAngle, 0);
            }
        }

    }
}

