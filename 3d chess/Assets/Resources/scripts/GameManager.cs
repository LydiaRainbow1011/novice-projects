using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;




public enum ChessType
{
    Black, White, Chess
}

public enum Play
{
    Black, White
}

public enum GameState
{
    Run, End
}

public enum Mode
{
    pvp, pve
}

public class GameManager : MonoBehaviour
{
    //gameboard assets and material/object variables

    public GameState gameState;
    public Play currentPlay;
    private Play initialPlay;
    public static Mode mode;

    public static GameObject chesspoints;
    public static GameObject playedchess;
    public static GameObject choice;
    public static Material cover;

    private GameObject originchess;
    private GameObject blackchess;
    private GameObject whitechess;
    private Material blackcover;
    private Material whitecover;

    private GameObject current;
    private GameObject playchess;

    public Dictionary<ChessType, List<Vector3>> dicOfChesses = new Dictionary<ChessType, List<Vector3>>();
    private List<Vector3> blacks = new List<Vector3>();
    private List<Vector3> whites = new List<Vector3>();
    private List<Vector3> empty = new List<Vector3>();
    private List<GameObject> emptychess;

    

    public GameObject gameOverCanvas;

    public int allChessCount;

    private AI ai;





    /// ————————————————— functions —————————————————————————

    void Start()
    {
        initialize();
    }

    void Update()
    {
        //test log
        Test();

        manageTurn();
  
    }


    //generate a list of all chesses locations and print a log
    //for test
    public void Test()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            //print("Black" + "\n" + string.Join("\n", blacks));
            //print("White" + "\n" + string.Join("\n", whites));
            //print("Empty" + "\n" + string.Join("\n", empty));
            //print("dic" + "\n" + string.Join("\n", ai.chessAndValue));

            foreach (var elem in allLine(currentPlay))
            {
                print("line" + "\n" + string.Join("\n", elem));
            }

            
        }
        if (Input.GetKeyDown(KeyCode.Q))
            GameOver();
        if (Input.GetKeyDown(KeyCode.I))
            clearboard();

    }


    //change game state
    public void GameStart()
    {
        gameState = GameState.Run;
    }
    public void GameOver()
    {
        gameState = GameState.End;
        gameOverCanvas.SetActive(true);

    }

    //check if game is running
    public bool gameRunning()
    {
        if (gameState == GameState.Run)
            return true;
        else
            return false;
    }

    //change player and status for next turn
    public void manageTurn()
    {
        if (judge() != ChessType.Chess || empty == null)
            {
                GameOver();
            }
    }

    //generate chess board
    //only use once
    public void generateBoard()
    {
        //introduce all variables
        originchess = Resources.Load("prefabs/ChessPoint") as GameObject;
        blackchess = Resources.Load("prefabs/black") as GameObject;
        whitechess = Resources.Load("prefabs/white") as GameObject;
        blackcover = Resources.Load("materials/black") as Material;
        whitecover = Resources.Load("materials/white") as Material;

        //create and manage gameboard
        chesspoints = new GameObject("positions");
        playedchess = new GameObject("chesses");

        //build empty dictionary to store chesses
        dicOfChesses.Add(ChessType.Black, blacks);
        dicOfChesses.Add(ChessType.White, whites);

        //generate empty chesses
        for (int y = -7; y < 8; ++y)
        {
            for (int z = -7; z < 8; ++z)
            {
                current = Instantiate(originchess, new Vector3(0, y, z), Quaternion.identity);
                current.name = "chess";
                current.transform.SetParent(chesspoints.transform);
            }
        }
    }

    //clear/initialize gameboard
    public void clearboard()
    {
        SceneManager.LoadScene("3d fivechess");
    }

    //set the chess board into initial state
    public void initialize()
    {
        generateBoard();

        GameObject[] empties = GameObject.FindGameObjectsWithTag("chess");
        foreach (GameObject empty in empties)
        {
            empty.GetComponent<ChessUpdate>().enabled = true;
        }

        GameStart();
        manageChess();
        initialPlay = Play.Black;
        currentPlay = initialPlay;
        mode = Mode.pvp;
        manageMatAndchess();

        ai = GameObject.Find("AI").GetComponent<AI>();
    }

    //update empty chesses
    public void manageChess()
    {
        //store empty chesses
        Vector3 pose;
        List<Vector3> newEmpty = new List<Vector3>();

        emptychess = new List<GameObject>(GameObject.FindGameObjectsWithTag("chess"));
        if (emptychess != null)
            newEmpty.Clear();
        foreach (GameObject ches in emptychess)
        {
            pose = ches.transform.position;
            if (!newEmpty.Contains(pose) && !blacks.Contains(pose) && !whites.Contains(pose))
                newEmpty.Add(pose);
        }
        empty = newEmpty;

        dicOfChesses[ChessType.Chess] = empty;
    }

    //change player
    public void managePlayer()
    {
        if (gameRunning())
        {
            allChessCount = dicOfChesses[ChessType.Black].Count + dicOfChesses[ChessType.White].Count;
            if (allChessCount % 2 == 1)
            {
                currentPlay = opponent(initialPlay);
                manageMatAndchess();
            }
            else
            {
                currentPlay = initialPlay;
                manageMatAndchess();
            }
        }

    }

    public Play opponent(Play play)
    {
        if (play == Play.Black)
            return Play.White;
        else
            return Play.Black;
    }

    //change chess as player change
    public void manageMatAndchess()
    {
        if (currentPlay == Play.Black)
        {
            cover = blackcover;
            choice = blackchess;
        }
        else
        {
            cover = whitecover;
            choice = whitechess;
        }
    }


    //place a specific chess at a given location
    public void placeChess(Vector3 loc)
    {
        if (gameRunning())
        {
            manageChess();
            if (empty.Contains(loc))
            {
                GameObject chosen = emptychess.Find(s => s.transform.position == loc);
                playchess = Instantiate(choice);
                if (currentPlay == Play.Black)
                {
                    playchess.name = "b";
                    playchess.transform.position = loc;
                    playchess.transform.SetParent(playedchess.transform);
                    blacks.Add(loc);
                    dicOfChesses[ChessType.Black] = blacks;
                }
                else
                {
                    playchess.name = "w";
                    playchess.transform.position = loc;
                    playchess.transform.SetParent(playedchess.transform);
                    whites.Add(loc);
                    dicOfChesses[ChessType.White] = whites;
                }
                Destroy(chosen);
                manageChess();
                managePlayer();
            }
        }
    }

    public void autoplaceNext()
    {
        if (gameRunning())
        {
            wait();
            Vector3 vec = ai.result;
            //empty[Random.Range(0, empty.Count)];
            placeChess(vec);
        }

    }

    //place a chess at given location
    //then place another for next player (AI)
    public void oneStepAfter(Vector3 loc)
    {
        if (gameRunning())
        {
            placeChess(loc);
            autoplaceNext();
        }
    }

    public void wait()
    {
        System.Threading.Thread.Sleep(500);
    }



    //check which chess wins with 5 chesses in a line
    public ChessType judge()
    {
        if (gameRunning())
        {
            if (judgeIfGet5chess(ChessType.Black))
                return ChessType.Black;
            if (judgeIfGet5chess(ChessType.White))
                return ChessType.White;
            else
                return ChessType.Chess;
        }
        else
            return ChessType.Chess;

    }

    //integrate four situations below
    public bool judgeIfGet5chess(ChessType play)
    {
        List<Vector3> dic = dicOfChesses[play];
        List<bool> checks = new List<bool>();
        foreach (Vector3 vector in dic)
        {
            bool result = judgeLeftRight(dic, vector) || judgeUpDown(dic, vector) || judgeUpCross(dic, vector) || judgeDownCross(dic, vector);
            checks.Add(result);
        }
        if (checks.Contains(true))
            return true;
        else
            return false;
    }

    //check if there is five chesses in a line of vertical/horizontal/oblique
    public bool judgeLeftRight(List<Vector3> list, Vector3 vec)
    {
        Vector3 vec1 = vec + Vector3.forward;
        Vector3 vec2 = vec1 + Vector3.forward;
        Vector3 vec3 = vec2 + Vector3.forward;
        Vector3 vec4 = vec3 + Vector3.forward;
        if (list.Contains(vec1) && list.Contains(vec2) && list.Contains(vec3) && list.Contains(vec4))
            return true;
        else
            return false;
    }

    public bool judgeUpDown(List<Vector3> list, Vector3 vec)
    {
        Vector3 vec1 = vec + Vector3.down;
        Vector3 vec2 = vec1 + Vector3.down;
        Vector3 vec3 = vec2 + Vector3.down;
        Vector3 vec4 = vec3 + Vector3.down;
        if (list.Contains(vec1) && list.Contains(vec2) && list.Contains(vec3) && list.Contains(vec4))
            return true;
        else
            return false;
    }

    public bool judgeUpCross(List<Vector3> list, Vector3 vec)
    {

        Vector3 vec1 = vec + Vector3.forward + Vector3.down;
        Vector3 vec2 = vec1 + Vector3.forward + Vector3.down;
        Vector3 vec3 = vec2 + Vector3.forward + Vector3.down;
        Vector3 vec4 = vec3 + Vector3.forward + Vector3.down;
        if (list.Contains(vec1) && list.Contains(vec2) && list.Contains(vec3) && list.Contains(vec4))
            return true;
        else
            return false;
    }

    public bool judgeDownCross(List<Vector3> list, Vector3 vec)
    {
        Vector3 vec1 = vec + Vector3.forward + Vector3.up;
        Vector3 vec2 = vec1 + Vector3.forward + Vector3.up;
        Vector3 vec3 = vec2 + Vector3.forward + Vector3.up;
        Vector3 vec4 = vec3 + Vector3.forward + Vector3.up;
        if (list.Contains(vec1) && list.Contains(vec2) && list.Contains(vec3) && list.Contains(vec4))
            return true;
        else
            return false;

    }



    public List<List<Vector3>> allLine(Play play)
    {
        List<List<Vector3>> listOfList = new List<List<Vector3>>();

        List<Vector3> group = new List<Vector3>();


        if (play == Play.Black)
        {
            group = blacks;
            listOfList = CDline(group); 

            return listOfList;

        }
        else
        {
            group = whites;
            //listOfList = LRline(group);
            return listOfList;
        }
    }

    //manage chesses in one unique single line
    public List<List<Vector3>> LRline(List<Vector3> list)
    {
        List<List<Vector3>> newlist = new List<List<Vector3>>();

        if (list.Count > 0)
        {
            List<Vector3> aline = new List<Vector3>();
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

            newlist.Add(aline);
            list = list.Except(aline).ToList();
            newlist = newlist.Union(LRline(list)).ToList();
        }
        return newlist;
    }

    public List<List<Vector3>> UDline(List<Vector3> list)
    {
        List<List<Vector3>> newlist = new List<List<Vector3>>();

        if (list.Count > 0)
        {
            List<Vector3> aline = new List<Vector3>();
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

            newlist.Add(aline);
            list = list.Except(aline).ToList();
            newlist = newlist.Union(UDline(list)).ToList();
        }
        return newlist;
    }

    public List<List<Vector3>> CUline(List<Vector3> list)
    {
        List<List<Vector3>> newlist = new List<List<Vector3>>();

        if (list.Count > 0)
        {
            List<Vector3> aline = new List<Vector3>();
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

            newlist.Add(aline);
            list = list.Except(aline).ToList();
            newlist = newlist.Union(CUline(list)).ToList();
        }
        return newlist;
    }

    public List<List<Vector3>> CDline(List<Vector3> list)
    {
        List<List<Vector3>> newlist = new List<List<Vector3>>();

        if (list.Count > 0)
        {
            List<Vector3> aline = new List<Vector3>();
            Vector3 one = list[0];
            while (list.Contains(one))
            {
                aline.Add(one);
                one += Vector3.up + Vector3.back;
            }

            Vector3 two = list[0];
            while (list.Contains(two))
            {
                if (!aline.Contains(two))
                {
                    aline.Add(two);
                }
                two += Vector3.down + Vector3.forward;
            }

            newlist.Add(aline);
            list = list.Except(aline).ToList();
            newlist = newlist.Union(CDline(list)).ToList();
        }
        return newlist;
    }
}
