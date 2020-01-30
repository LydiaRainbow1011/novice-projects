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

    public Dictionary<List<Vector3>, int> blacklines;
    public Dictionary<List<Vector3>, int> whitelines;

    public Dictionary<Vector3, int> chessAndValue = new Dictionary<Vector3, int>();

    public int blackscore;
    public int whitescore;

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
        allLine();
        blackscore = boardEval(blacklines);
        whitescore = boardEval(whitelines);
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


    //evaluate the whole score of a given chessboard
    public int boardEval (Dictionary<List<Vector3>, int> board)
    {
        List<int> result = new List<int>();
        foreach (KeyValuePair<List<Vector3>, int> item in board)
        {
            List<Vector3> list = item.Key;
            int len = list.Count;
            int live = item.Value;

            int score = 0;

            if (len == 5 || (len == 4 && live == 2))
            {
                score += 99999;
            }
            if (len == 4 && live == 1)
            {
                score += 10000;
            }
            if (len == 3 && live == 2)
            {
                score += 1000;
            }
            if (len ==3 && live == 1)
            {
                score += 500;
            }
            if (len == 2 && live == 2)
            {
                score += 200;
            }
            if (len == 2 && live == 1)
            {
                score += 100;
            }

            result.Add(score);
        }

        int final = result.Sum();
        return final;

    }


    //generate all lines and its lives
    public void allLine()
    {
        blacklines = new Dictionary<List<Vector3>, int>();
        whitelines = new Dictionary<List<Vector3>, int>();


        List<Vector3> group1 = new List<Vector3>();
        group1 = blacks;

        Dictionary<List<Vector3>, int> lr1 = LRline(group1);
        Dictionary<List<Vector3>, int> ud1 = UDline(group1);
        Dictionary<List<Vector3>, int> cu1 = CUline(group1);
        Dictionary<List<Vector3>, int> cd1 = CDline(group1);

        blacklines = combineDic(combineDic(combineDic(lr1, ud1), cu1), cd1);


        List<Vector3> group2 = new List<Vector3>();
        group2 = whites;

        Dictionary<List<Vector3>, int> lr2 = LRline(group2);
        Dictionary<List<Vector3>, int> ud2 = UDline(group2);
        Dictionary<List<Vector3>, int> cu2 = CUline(group2);
        Dictionary<List<Vector3>, int> cd2 = CDline(group2);

        whitelines = combineDic(combineDic(combineDic(lr2, ud2), cu2), cd2);

    }

    //manage chesses in one unique single line
    public Dictionary<List<Vector3>, int> LRline(List<Vector3> list)
    {
        List<List<Vector3>> newlist = new List<List<Vector3>>();
        Dictionary<List<Vector3>, int> lineAValue = new Dictionary<List<Vector3>, int>();


        if (list.Count > 0)
        {
            List<Vector3> aline = new List<Vector3>();
            int live = 0;

            Vector3 one = list[0];
            while (list.Contains(one))
            {
                aline.Add(one);
                one += Vector3.forward;
            }

            Vector3 two = list[0];
            while (list.Contains(two))
            {
                if (!aline.Contains(two))
                {
                    aline.Add(two);
                }
                two += Vector3.back;
            }

            if (empty.Contains(one) && empty.Contains(two))
            {
                live = 2;
            }
            else if (!empty.Contains(one) && !empty.Contains(two))
            {
                live = 0;
            }
            else
            {
                live = 1;
            }


            lineAValue.Add(aline,live);
            list = list.Except(aline).ToList();

            lineAValue = lineAValue.Concat(LRline(list)).ToDictionary(x => x.Key, x => x.Value);
        }

        

        return lineAValue;
    }

    public Dictionary<List<Vector3>, int> UDline(List<Vector3> list)
    {
        List<List<Vector3>> newlist = new List<List<Vector3>>();
        Dictionary<List<Vector3>, int> lineAValue = new Dictionary<List<Vector3>, int>();


        if (list.Count > 0)
        {
            List<Vector3> aline = new List<Vector3>();
            int live = 0;

            Vector3 one = list[0];
            while (list.Contains(one))
            {
                aline.Add(one);
                one += Vector3.up;
            }

            Vector3 two = list[0];
            while (list.Contains(two))
            {
                if (!aline.Contains(two))
                {
                    aline.Add(two);
                }
                two += Vector3.down;
            }

            if (empty.Contains(one) && empty.Contains(two))
            {
                live = 2;
            }
            else if (!empty.Contains(one) && !empty.Contains(two))
            {
                live = 0;
            }
            else
            {
                live = 1;
            }


            lineAValue.Add(aline, live);
            list = list.Except(aline).ToList();

            lineAValue = lineAValue.Concat(UDline(list)).ToDictionary(x => x.Key, x => x.Value);
        }



        return lineAValue;
    }

    public Dictionary<List<Vector3>, int> CUline(List<Vector3> list)
    {
        List<List<Vector3>> newlist = new List<List<Vector3>>();
        Dictionary<List<Vector3>, int> lineAValue = new Dictionary<List<Vector3>, int>();


        if (list.Count > 0)
        {
            List<Vector3> aline = new List<Vector3>();
            int live = 0;

            Vector3 one = list[0];
            while (list.Contains(one))
            {
                aline.Add(one);
                one += Vector3.up + Vector3.forward;
            }

            Vector3 two = list[0];
            while (list.Contains(two))
            {
                if (!aline.Contains(two))
                {
                    aline.Add(two);
                }
                two += Vector3.down + Vector3.back;
            }

            if (empty.Contains(one) && empty.Contains(two))
            {
                live = 2;
            }
            else if (!empty.Contains(one) && !empty.Contains(two))
            {
                live = 0;
            }
            else
            {
                live = 1;
            }


            lineAValue.Add(aline, live);
            list = list.Except(aline).ToList();

            lineAValue = lineAValue.Concat(CUline(list)).ToDictionary(x => x.Key, x => x.Value);
        }



        return lineAValue;
    }

    public Dictionary<List<Vector3>, int> CDline(List<Vector3> list)
    {
        List<List<Vector3>> newlist = new List<List<Vector3>>();
        Dictionary<List<Vector3>, int> lineAValue = new Dictionary<List<Vector3>, int>();


        if (list.Count > 0)
        {
            List<Vector3> aline = new List<Vector3>();
            int live = 0;

            Vector3 one = list[0];
            while (list.Contains(one))
            {
                aline.Add(one);
                one += Vector3.down + Vector3.forward;
            }

            Vector3 two = list[0];
            while (list.Contains(two))
            {
                if (!aline.Contains(two))
                {
                    aline.Add(two);
                }
                two += Vector3.up + Vector3.back;
            }

            if (empty.Contains(one) && empty.Contains(two))
            {
                live = 2;
            }
            else if (!empty.Contains(one) && !empty.Contains(two))
            {
                live = 0;
            }
            else
            {
                live = 1;
            }


            lineAValue.Add(aline, live);
            list = list.Except(aline).ToList();

            lineAValue = lineAValue.Concat(CDline(list)).ToDictionary(x => x.Key, x => x.Value);
        }



        return lineAValue;
    }



    //if there is duplicate item, add its value
    public Dictionary<List<Vector3>, int> combineDic(Dictionary<List<Vector3>, int> dic1, Dictionary<List<Vector3>, int> dic2)
    {
        foreach (KeyValuePair<List<Vector3>, int> item in dic2)
        {
            dic1.Add(item.Key, item.Value);
        }

        return dic1;
    }
 
    //check whether there is cross point between two line/rwo/crossline
    public bool ifcross(List<Vector3> list1, List<Vector3> list2)
    {
        if ((list1.Intersect(list2).ToList()).Count == 0)
            return false;
        else
            return true;
    }

}
