using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{

    private static int SCREEN_WIDTH = 64;   //1024px
    private static int SCREEN_HEIGHT = 48;  //768px

    public float speed = 0.1f;
    private float timer = 0;

    Cell[,] grid = new Cell[SCREEN_WIDTH, SCREEN_HEIGHT];

    // Start is called before the first frame update
    void Start()
    {
        PlaceCells();

    }

    // Update is called once per frame
    void Update()
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

    void PlaceCells()
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
