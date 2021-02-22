using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Xml.Serialization;

public class Game : MonoBehaviour
{

    private static int SCREEN_WIDTH = 64;   //1024px
    private static int SCREEN_HEIGHT = 48;  //768px

    public HUD hud;

    public float speed = 0.1f;
    private float timer = 0;

    public bool simulationEnabled = false;

    Cell[,] grid = new Cell[SCREEN_WIDTH, SCREEN_HEIGHT];

    // Start is called before the first frame update
    void Start()
    {
        PlaceCells(3);
    }

    // Update is called once per frame
    void Update()
    {
        if (simulationEnabled)
        {
            if (timer >= speed)
            {
                timer = 0f;
                CountNeighbors();
                PopulationControl();
            }
            else
            {
                timer += Time.deltaTime;
            }
        }
        UserInput();
    }

    private void LoadPattern()
    {
        string path = "patterns";
        if (!Directory.Exists(path))
        {
            return;
        }

        XmlSerializer serializer = new XmlSerializer(typeof(Pattern));
        path += "/test.xml";

        StreamReader reader = new StreamReader(path);
        Pattern pattern = (Pattern)serializer.Deserialize(reader.BaseStream);
        reader.Close();

        bool isAlive;
        int x = 0, y = 0;

        Debug.Log(pattern.patternString);

        foreach (char c in pattern.patternString)
        {
            if (c.ToString() == "1")
            {
                isAlive = true;

            }
            else
            {
                isAlive = false;
            }

            grid[x, y].SetAlive(isAlive);
            x++;

            if (x == SCREEN_WIDTH)
            {
                x = 0;
                y++;
            }
        }



    }
    private void SavePattern()
    {
        string path = "patterns";

        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);

        }

        Pattern pattern = new Pattern();

        string patternString = null;

        for (int y = 0; y < SCREEN_HEIGHT; y++)
        {
            for (int x = 0; x < SCREEN_WIDTH; x++)
            {
                if (grid[x,y].isAlive == false)
                {
                    patternString += "0";
                }
                else
                {
                    patternString += "1";
                }
            }
        }
        pattern.patternString = patternString;

        XmlSerializer serializer = new XmlSerializer(typeof(Pattern));

        StreamWriter writer = new StreamWriter(path + "/test.xml");
        serializer.Serialize(writer.BaseStream, pattern);
        writer.Close();

        Debug.Log(pattern.patternString);
    }

    void UserInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            int x = Mathf.RoundToInt(mousePoint.x);
            int y = Mathf.RoundToInt(mousePoint.y);

            if ( x >= 0 && y >= 0 && x < SCREEN_WIDTH && y < SCREEN_HEIGHT)
            {
                grid[x, y].SetAlive(!grid[x, y].isAlive);
            }
        }

        if (Input.GetKeyUp(KeyCode.P))//pause
        {
            simulationEnabled = false;
        }

        if (Input.GetKeyUp(KeyCode.B))//build / resume
        {
            simulationEnabled = true;
        }

        if (Input.GetKeyUp(KeyCode.C))
        {
            for (int y = 0; y < SCREEN_HEIGHT; y++)
            {
                for (int x = 0; x < SCREEN_WIDTH; x++)
                {
                    grid[x, y].SetAlive(false);
                }
            }
        }

        if (Input.GetKeyUp(KeyCode.S))
        {
            hud.ShowSaveDialog();
        }

        if (Input.GetKeyUp(KeyCode.L))
        {
            LoadPattern();
        }

    }

    void PlaceCells(int type)
    {
        //central width : 32
        //central height : 24
        if (type == 1)//PREMADE
        {

            for (int y = 0; y < SCREEN_HEIGHT; y++)
            {
                for (int x = 0; x < SCREEN_WIDTH; x++)
                {
                    Cell cell = Instantiate(Resources.Load("Prefabs/Cell", typeof(Cell)), new Vector2(x, y), Quaternion.identity) as Cell;
                    grid[x, y] = cell;
                    grid[x, y].SetAlive(false);
                }
            }
            for (int y = 21; y < 24; y++)
            {
                for (int x = 31; x < 38; x++)
                {
                    //nothing at x = 34
                    // nothing at x = 32 and y = 22
                    // nothing at x = 36 and y = 22
                    if (x != 34)
                    {
                        if (y == 21 || y == 23)
                        {
                            grid[x, y].SetAlive(true);

                        }
                        else if (y == 22 && ((x != 32) && (x != 36)))
                        {
                            grid[x, y].SetAlive(true);
                        }
                    }
                }
            }
        }
        else if (type == 2)//RANDOM
        {
            for (int y = 0; y < SCREEN_HEIGHT; y++)
            {
                for (int x = 0; x < SCREEN_WIDTH; x++)
                {
                    Cell cell = Instantiate(Resources.Load("Prefabs/Cell", typeof(Cell)), new Vector2(x, y), Quaternion.identity) as Cell;
                    grid[x, y] = cell;
                    grid[x, y].SetAlive(RandomAliveCell());
                }
            }
        }
        else if (type == 3)//EMPTY
        {
            for (int y = 0; y < SCREEN_HEIGHT; y++)
            {
                for (int x = 0; x < SCREEN_WIDTH; x++)
                {
                    Cell cell = Instantiate(Resources.Load("Prefabs/Cell", typeof(Cell)), new Vector2(x, y), Quaternion.identity) as Cell;
                    grid[x, y] = cell;
                    grid[x, y].SetAlive(false);
                }
            }
        }
    }
     
    void PopulationControl()
    {
        for (int y = 0; y < SCREEN_HEIGHT; y++)
        {
            for (int x = 0; x < SCREEN_WIDTH; x++)
            {
                //**********************************************************************
                //Rules:
                //1 cell with 2 or 3 live nb survives
                //2 dead cell with 3 live nb becomes live cell
                //3 all other live cells die in next gen while other dead cell stay dead
                //**********************************************************************

                if (grid[x, y].isAlive)
                {
                    if (grid[x, y].numNeighbours !=2 && grid[x, y].numNeighbours != 3)
                    {
                        grid[x, y].SetAlive(false);
                    }
                    //cell-alive
                }
                else
                {
                    if (grid[x, y].numNeighbours == 3)
                    {
                        grid[x, y].SetAlive(true);
                    }
                    //cell-dead
                }
            }
        }
    }

    bool RandomAliveCell()
    {
        int rand = UnityEngine.Random.Range(0, 100);

        if (rand > 75)
        {
            return true;
        }
        return false; 
    }

    void CountNeighbors()
    {
        for (int y = 0; y < SCREEN_HEIGHT; y++)
        {
            for (int x = 0; x < SCREEN_WIDTH; x++)
            {
                int numNeighbors = 0;
                if (y + 1 < SCREEN_HEIGHT)//n
                {
                    if(grid[x, y + 1].isAlive)
                    {
                        numNeighbors++;
                    }
                }
                if(x + 1 < SCREEN_WIDTH)//e
                {
                    if (grid[x + 1, y].isAlive)
                    {
                        numNeighbors++;
                    }
                }
                if (y - 1 >= 0)//s
                {
                    if (grid[x, y - 1].isAlive)
                    {
                        numNeighbors++;
                    }
                }
                if (x - 1 >= 0)//w
                {
                    if (grid[x - 1, y].isAlive)
                    {
                        numNeighbors++;
                    }
                }
                if(x + 1 < SCREEN_WIDTH && y + 1 < SCREEN_HEIGHT)//ne
                {
                    if (grid[x + 1, y + 1].isAlive)
                    {
                        numNeighbors++;
                    }
                }
                if (x - 1 >= 0 && y + 1 < SCREEN_HEIGHT)//nw
                {
                    if (grid[x - 1, y + 1].isAlive)
                    {
                        numNeighbors++;
                    }
                }
                if (x + 1 < SCREEN_WIDTH && y - 1 >= 0)//se
                {
                    if (grid[x + 1, y - 1].isAlive)
                    {
                        numNeighbors++;
                    }
                }
                if (x - 1 >= 0 && y - 1 >= 0)//sw
                {
                    if (grid[x - 1, y - 1].isAlive)
                    {
                        numNeighbors++;
                    }
                }

                grid[x, y].numNeighbours = numNeighbors;//how many neigh everyone has

            }
        }
    }
}
