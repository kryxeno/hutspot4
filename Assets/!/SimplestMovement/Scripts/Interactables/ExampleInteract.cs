using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExampleInteract : MonoBehaviour
{
    [SerializeField] Material switchMat;
    [SerializeField] Color color1;
    [SerializeField] Color color2;

    private int i;

    private void Awake()
    {
        i = 0;
    }

    //This script changes the objects material color between two selected ones
    public void SetNextColor()
    {
        if (i == 0)
        {
            i = 1;
            switchMat.color = color1;
        } 
        else
        {
            i = 0;
            switchMat.color = color2;
        }
    }
}
