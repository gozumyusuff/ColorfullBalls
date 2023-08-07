using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ObtsacleGenerator : MonoBehaviour
{

    public GameObject ObstaclePrefab;
    public Transform[] ObstaclePositionZ;
    public Transform[] ObstaclePositionX;


    float yPosition = 20.3f;


    // Start is called before the first frame update
    void Start()
    {
        GenerateObstacles();
    }

    private void GenerateObstacles()
    {
        for (int i = 0; i < ObstaclePositionZ.Length; i++)
        {
            GameObject instantiatedObstacle = Instantiate(ObstaclePrefab, this.transform);           

            instantiatedObstacle.transform.localPosition = new Vector3(ObstaclePositionX[Random.Range(0, 3)].localPosition.x, yPosition, ObstaclePositionZ[i].localPosition.z);
        }
    }
}
