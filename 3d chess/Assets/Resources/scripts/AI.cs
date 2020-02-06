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
    private List<Vector3> currents;
    private List<Vector3> opponents;

    public Dictionary<List<Vector3>, int> blacklines;
    public Dictionary<List<Vector3>, int> whitelines;

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
        empty = gamemanager.empty;
        blacks = gamemanager.blacks;
        whites = gamemanager.whites;

        play = gamemanager.currentPlay;

        current();

        blacklines = allLine(blacks,empty);
        whitelines = allLine(whites,empty);

        blackscore = boardEval(blacklines);
        whitescore = boardEval(whitelines);

        result = finalEval();

    }


    public void current ()
    {
        if (play == Play.Black)
        {
            currents = blacks;
            opponents = whites;
        }
        else
        {
            currents = whites;
            opponents = blacks;
        }
    }

    private int currentScore()
    {
        int result = 0;
        if (play == Play.Black)
        {
            result = blackscore;
        }
        else
        {
            result = whitescore;
        }
        return result;
    }
    private int opponentScore()
    {
        int result = 0;
        if (play == Play.Black)
        {
            result = whitescore;
        }
        else
        {
            result = blackscore;
        }
        return result;
    }


    //find the location with max value
    public Vector3 finalEval()
    {
        Dictionary<Vector3, int> pairs = evalDict(empty);

        List<Vector3> listOfKeys = pairs.Keys.ToList();
        List<int> listOfValues = pairs.Values.ToList();

        int indexOfMax = listOfValues.IndexOf(listOfValues.Max());

        return listOfKeys[indexOfMax];
    }

    //give a value for each location in dictionary
    public Dictionary<Vector3, int> evalDict(List<Vector3> group)
    {
        Dictionary<Vector3, int> chessAndValue = new Dictionary<Vector3, int>();

        foreach (Vector3 vec in group)
        {
            chessAndValue.Add(vec, evalAStep(vec));
        }

        return chessAndValue;
        
    }

    //evaluate each location
    public int evalAStep(Vector3 vec)
    {
        int oneResult = 0;

        List<Vector3> mylist = new List<Vector3>();
        List<Vector3> yourlist = new List<Vector3>();
        List<Vector3> newEmpty = new List<Vector3>();

        foreach (Vector3 item in currents)
        {
            mylist.Add(item);
        }
        mylist.Add(vec);

        foreach (Vector3 item in opponents)
        {
            yourlist.Add(item);
        }

        foreach (Vector3 item in empty)
        {
            if (item != vec)
            {
                newEmpty.Add(item);
            }
        }

        int myResult = boardEval(allLine(mylist, newEmpty));
        int yourResult = boardEval(allLine(yourlist, newEmpty));

        int currentscore = currentScore();
        int opponentscore = opponentScore();

        oneResult = (myResult - currentscore) - (yourResult - opponentscore);


        return oneResult;
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
    public Dictionary<List<Vector3>, int> allLine(List<Vector3> group, List<Vector3> aempty)
    {
        Dictionary<List<Vector3>, int> lines = new Dictionary<List<Vector3>, int>();

        Dictionary<List<Vector3>, int> lr = LRline(group,aempty);
        Dictionary<List<Vector3>, int> ud = UDline(group,aempty);
        Dictionary<List<Vector3>, int> cu = CUline(group,aempty);
        Dictionary<List<Vector3>, int> cd = CDline(group,aempty);

        lines = combineDic(combineDic(combineDic(lr, ud), cu), cd);


        return lines;

    }

    //manage chesses in one unique single line
    public Dictionary<List<Vector3>, int> LRline(List<Vector3> list, List<Vector3> aempty)
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

            if (aempty.Contains(one) && aempty.Contains(two))
            {
                live = 2;
            }
            else if (!aempty.Contains(one) && !aempty.Contains(two))
            {
                live = 0;
            }
            else
            {
                live = 1;
            }

            lineAValue.Add(aline,live);
            list = list.Except(aline).ToList();

            lineAValue = lineAValue.Concat(LRline(list,aempty)).ToDictionary(x => x.Key, x => x.Value);
        }

        return lineAValue;
    }

    public Dictionary<List<Vector3>, int> UDline(List<Vector3> list, List<Vector3> aempty)
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

            if (aempty.Contains(one) && aempty.Contains(two))
            {
                live = 2;
            }
            else if (!aempty.Contains(one) && !aempty.Contains(two))
            {
                live = 0;
            }
            else
            {
                live = 1;
            }

            lineAValue.Add(aline, live);
            list = list.Except(aline).ToList();

            lineAValue = lineAValue.Concat(UDline(list,aempty)).ToDictionary(x => x.Key, x => x.Value);
        }

        return lineAValue;
    }

    public Dictionary<List<Vector3>, int> CUline(List<Vector3> list, List<Vector3> aempty)
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

            if (aempty.Contains(one) && aempty.Contains(two))
            {
                live = 2;
            }
            else if (!aempty.Contains(one) && !aempty.Contains(two))
            {
                live = 0;
            }
            else
            {
                live = 1;
            }

            lineAValue.Add(aline, live);
            list = list.Except(aline).ToList();

            lineAValue = lineAValue.Concat(CUline(list,aempty)).ToDictionary(x => x.Key, x => x.Value);
        }

        return lineAValue;
    }

    public Dictionary<List<Vector3>, int> CDline(List<Vector3> list, List<Vector3> aempty)
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

            if (aempty.Contains(one) && aempty.Contains(two))
            {
                live = 2;
            }
            else if (!aempty.Contains(one) && !aempty.Contains(two))
            {
                live = 0;
            }
            else
            {
                live = 1;
            }

            lineAValue.Add(aline, live);
            list = list.Except(aline).ToList();

            lineAValue = lineAValue.Concat(CDline(list,aempty)).ToDictionary(x => x.Key, x => x.Value);
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
