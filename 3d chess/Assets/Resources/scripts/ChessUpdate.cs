using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChessUpdate : MonoBehaviour
{
    private GameObject current;
    private bool played;
    private Material origin;

    private static GameObject playedchess;
    private GameObject manager;
    private GameManager gamemanager;
    public Material cover;

    private GameState state;
    private Mode mode;
    private List<Vector3> empty;




    void Start()
    {
        played = false;
        origin = Resources.Load("materials/origin") as Material;
        playedchess = GameManager.playedchess;
        manager = GameObject.Find("Game Manager");
        gamemanager = manager.GetComponent<GameManager>();
    }

    void Update()
    {
        updateEverything();
    }

    public void updateEverything()
    {
        cover = GameManager.cover;
        mode = gamemanager.mode;
        state = gamemanager.gameState;
    }


    //mouse click and chess placed
    public void OnMouseDown()
    {
        if (gamemanager.gameRunning())
        {
            if (played == false)
            {
                Vector3 vec = this.transform.position;
                if (mode == Mode.pvp)
                {
                    gamemanager.placeChess(vec);
                }
                else
                {
                    gamemanager.placeChess(vec);
                    gamemanager.autoplaceNext();
                }
            }
        }
    }


    //mouse sweep
    public void OnMouseEnter()
    {
        if (state == GameState.Run)
            if (played == false)
                changeCover(cover);
    }

    //chess not placed, mouse leave
    public void OnMouseExit()
    {
        if (state == GameState.Run)
            if (played == false)
                GetComponent<MeshRenderer>().material = origin;
    }

    public void changeCover(Material cover)
    {
        GetComponent<MeshRenderer>().material = cover;
    }

}
