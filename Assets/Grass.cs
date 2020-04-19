using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grass : MonoBehaviour
{
    [SerializeField]
    private Transform[] grassPoint;
    [SerializeField]
    private GameObject[] grasses;

    public int emptyChance;

    void Start()
    {
        foreach (Transform grass in grassPoint)
        {
            int grassIndex = Random.Range(0, grasses.Length + emptyChance);

            if (grassIndex < grasses.Length)
            {
                Instantiate(grasses[grassIndex], grass);
            }

        }
    }
}
