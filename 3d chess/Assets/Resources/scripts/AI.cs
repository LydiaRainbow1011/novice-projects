using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class AI : MonoBehaviour
{
    private GameObject manager;
    private GameManager gamemanager;
    private Play play;

    private Dictionary<ChessType, List<Vector3>> dicOfChesses;
    private List<Vector3> blacks;
    private List<Vector3> whites;
    private List<Vector3> empty;

    public Dictionary<Vector3, int> chessAndValue = new Dictionary<Vector3, int>();

    public Vector3 result;



    // Start is called before the first frame update
    void Start()
    {
        manager = GameObject.Find("Game Manager");
        gamemanager = manager.GetComponent<GameManager>();

    }

    // Update is called once per frame
    void Update()
    {
        boardUpdate();
    }

    public void boardUpdate()
    {
        dicOfChesses = gamemanager.dicOfChesses;
        empty = dicOfChesses[ChessType.Chess];
        blacks = dicOfChesses[ChessType.Black];
        whites = dicOfChesses[ChessType.White];

        play = gamemanager.currentPlay;

        evalDict();
        result = finalEval();

    }

    //find the location with max value
    public Vector3 finalEval()
    {
        List<Vector3> listOfKeys = chessAndValue.Keys.ToList();
        List<int> listOfValues = chessAndValue.Values.ToList();
        int indexOfMax = listOfValues.IndexOf(listOfValues.Max());
        return listOfKeys[indexOfMax];
    }

    //give a value for each location in dictionary
    public void evalDict()
    {
        chessAndValue.Clear();
        List<Vector3> veclist = empty;
        foreach (Vector3 loc in veclist)
        {
            chessAndValue[loc] = evalAStep(loc);
        }
    }

    //evaluate each location
    public int evalAStep(Vector3 vec)
    {
        return 0;
    }

    
}
