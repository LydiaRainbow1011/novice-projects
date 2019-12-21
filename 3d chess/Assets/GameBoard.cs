using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBoard : MonoBehaviour
{

    public GameObject chess;

    void Start()
    {
        for (int y=0; y<8; ++y)
        {
            for (int z=0; z<8; ++z)
            {
                Instantiate(chess);
            }
        }
    }


}
