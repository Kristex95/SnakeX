using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Snake_Movement_Script : MonoBehaviour
{
    [SerializeField]
    private Sprite bodySprite;

    [SerializeField]
    private Grid grid;

    [SerializeField]
    private Tilemap borderTilemap;

    [SerializeField]
    private Vector3 dir;
    private Vector3 desiredPos = Vector3.zero;

    [SerializeField]
    private Game_Manager_Script game_manager_script;

    [HideInInspector]
    public Vector3Int gridPosition = Vector3Int.zero;

    private float gridMoveTimer;
    private float gridMoveTimerMax;

    [HideInInspector]
    private bool isAlive = true;
    private int snakeBodySize = 3;
    private List<Vector3Int> snakeMovePositionList = new List<Vector3Int>();


    private void Awake()
    {
        gridMoveTimerMax = 0.6f;
        gridMoveTimer = gridMoveTimerMax;
    }

    // Start is called before the first frame update
    void Start()
    {
        dir = new Vector3(1, 0, 0);

        transform.position = Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isAlive) 
            return;

        GetInput();
        MovePosition();
    }

    public void GetInput()
    {
        //up
        if (Input.GetKeyDown(KeyCode.W))
        {
            if (dir == new Vector3(-1, 0, 0)) return;
            dir = new Vector3Int(1, 0, 0);
        }
        //down
        else if (Input.GetKeyDown(KeyCode.S))
        {
            if (dir == new Vector3(1, 0, 0)) return;
            dir = new Vector3Int(-1, 0, 0);
        }
        //upRight
        else if(Input.GetKey(KeyCode.E))
        {
            if (dir == new Vector3(-0.5f, -1, 0)) return;
            dir = new Vector3(0.5f, 1, 0);
        }
        //downRight
        else if (Input.GetKey(KeyCode.D))
        {
            if (dir == new Vector3(0.5f, -1, 0)) return;
            dir = new Vector3(-0.5f, 1, 0);
        }
        //upLeft
        else if (Input.GetKey(KeyCode.Q))
        {
            if (dir == new Vector3(-0.5f, 1, 0)) return;
            dir = new Vector3(0.5f, -1, 0);
        }
        //downLeft
        else if (Input.GetKey(KeyCode.A))
        {
            if (dir == new Vector3(0.5f, 1, 0)) return;
            dir = new Vector3(-0.5f, -1, 0);
        }
    }

    public void MovePosition()
    {
        gridMoveTimer += Time.deltaTime;
        if(gridMoveTimer >= gridMoveTimerMax)
        {
            gridMoveTimer -= gridMoveTimerMax;


            desiredPos += dir;
            gridPosition = new Vector3Int((int)Mathf.Floor(desiredPos.x), (int)desiredPos.y, 0);
            Vector3 snakeWorldPos = grid.CellToWorld(gridPosition);

            if (borderTilemap.GetTile(gridPosition) != null)
            {
                game_manager_script.GameOver();
                return;
            }

            snakeMovePositionList.Insert(0, gridPosition);

            if (snakeMovePositionList.Count >= snakeBodySize + 1)
                snakeMovePositionList.RemoveAt(snakeMovePositionList.Count - 1);

            for (int i = 1; i < snakeMovePositionList.Count; i++)
            {
                GameObject bodyPart = new GameObject();
                bodyPart.AddComponent<SpriteRenderer>().sprite = bodySprite;
                bodyPart.GetComponent<SpriteRenderer>().color = Color.black;

                Vector3 bodyPartWorldPos = grid.CellToWorld(snakeMovePositionList[i]);

                bodyPart.transform.position = new Vector3(bodyPartWorldPos.x, bodyPartWorldPos.y, 0);
                Destroy(bodyPart, gridMoveTimerMax);
            }

            for(int i = 1; i < snakeMovePositionList.Count; i++)
            {
                if(gridPosition == snakeMovePositionList[i])
                {
                    game_manager_script.GameOver();
                }

            }
            
            transform.position = snakeWorldPos;
            transform.eulerAngles = new Vector3(0, 0, GetAngleFromVector(dir) - 90);

            game_manager_script.MoveSnake(gridPosition);
        }

    }

    public void SetIsAlive(bool _bool)
    {
        isAlive = _bool;
    }

    public void IncreaseBodyLength()
    {
        snakeBodySize++;
    }

    private float GetAngleFromVector(Vector3 dir)
    {
        float n = Mathf.Atan2(dir.x, dir.y) * Mathf.Rad2Deg;
        if (n < 0) n += 360;
        return n;
    }

    public List<Vector3Int> GetFullGridPositionList()
    {
        List<Vector3Int> gridPositonList = new List<Vector3Int>() { gridPosition };
        gridPositonList.AddRange(snakeMovePositionList);

        return gridPositonList;
    }
}
