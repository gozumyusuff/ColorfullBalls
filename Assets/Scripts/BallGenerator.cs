using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallGenerator : MonoBehaviour
{
    public GameObject[] BallPrefab;
    public Transform[] BallPositionZ;
    public Transform[] BallPositionX;

    float yPosition = 20.5f;


    void Start() 
    {
        GenerateBalls();
    }

    private void GenerateBalls()
    {
        for (int i = 0; i < BallPositionZ.Length; i++)
        {
            GameObject instantiatedBall = Instantiate(BallPrefab[Random.Range(0, BallPrefab.Length)], this.transform);

            instantiatedBall.transform.localPosition = new Vector3(BallPositionX[Random.Range(0, 3)].localPosition.x, yPosition, BallPositionZ[i].localPosition.z);
        }
    }

}
