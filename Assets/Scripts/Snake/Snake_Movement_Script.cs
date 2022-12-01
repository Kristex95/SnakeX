using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Snake_Movement_Script : MonoBehaviour
{
    [SerializeField]
    private Game_Manager_Script game_manager_script;

    // Sprites
    [SerializeField]
    private Sprite bodyStaightSprite;
    [SerializeField]
    private Sprite body60Sprite;
    [SerializeField]
    private Sprite body120Sprite;
    [SerializeField]
    private GameObject bodyPartPrefab;

    // Grid
    [SerializeField]
    private Grid grid;
    [SerializeField]
    private Tilemap borderTilemap;

    // Game Timers
    private float gridMoveTimer;
    private float gridMoveTimerMax;

    // Head Directions: dir = current;
    [SerializeField]
    private Direction dir;
    private Direction previousDirection;

    private Vector3 desiredPos = Vector3.zero;
    [HideInInspector]
    public Vector3Int gridPosition = Vector3Int.zero;
    public Vector3Int previousGridPosition;

    // Snake Params
    // bodyPart = views | snakeMovePosition = coordinates => BodyParts Logic
    [HideInInspector]
    private bool isAlive = true;
    private int snakeBodySize = 2;
    private List<SnakeMovement> snakeMovePositionList;
    private List<GameObject> bodyParts;

    // Setters and Getters
    public void SetIsAlive(bool _bool)
    {
        isAlive = _bool;
    }
    public Vector3Int SnakeMovementVector(SnakeMovement snakeMovement)
    {
        return snakeMovement.GetGridPosition();
    }
    public List<Vector3Int> GetFullGridPositionList()
    {
        List<Vector3Int> gridPositonList = new List<Vector3Int>() { gridPosition };
        gridPositonList.AddRange(snakeMovePositionList.ConvertAll(new Converter<SnakeMovement, Vector3Int>(SnakeMovementVector)));
        return gridPositonList;
    }

    // Game LifeCycle 
    private void InitSnakeBody()
    {
        snakeMovePositionList = new List<SnakeMovement>();


        bodyParts = new List<GameObject>();
        snakeBodySize = 2;
        dir = Direction.Up;
        previousDirection = dir;
        desiredPos = Vector3.zero;
        gridPosition = Vector3Int.zero;
        transform.position = grid.CellToWorld(gridPosition);
        transform.eulerAngles = Vector3.zero;
        gridMoveTimerMax = 0.5f;
        gridMoveTimer = gridMoveTimerMax; 
        
        for (int i = 0; i < snakeBodySize; i++)
        {
            snakeMovePositionList.Add(new SnakeMovement(null, Direction.Up, new Vector3Int(-1 - i, 0, 0)));
        }

        for (int i = 0; i < snakeBodySize; i++)
        {
            bodyParts.Add(Instantiate(bodyPartPrefab, grid.CellToWorld(snakeMovePositionList[i].GetGridPosition()), Quaternion.identity));
        }

        SetIsAlive(true);
    }
    private void Awake()
    {
        gridMoveTimerMax = 0.5f;
        gridMoveTimer = gridMoveTimerMax;
    }
    void Start()
    {
        InitSnakeBody();
    }
    void Update()
    {
        if (!isAlive)
            return;
        MoveSnake();
    }
    public void ResetSnake()
    {
        foreach (GameObject gm in bodyParts)
        {
            Destroy(gm);
        }
        InitSnakeBody();  
    }

    // LOGIC: If the snake hasn't eaten an apple than the last part of the body will move to the previous snake head posiion, else it will stay and new body part will be added;
    public void UpdateSnakeBody()
    {
        // Remove Last Piecs If Apple Not Eaten
        if (!game_manager_script.TryEatApple(gridPosition))
        {
            snakeMovePositionList.RemoveAt(snakeMovePositionList.Count - 1);
            Destroy(bodyParts[bodyParts.Count - 1]);
            bodyParts.RemoveAt(bodyParts.Count - 1);
        }
        else
        {
            snakeBodySize++;
        }

        // Insert BodyPart In Previous Head Position
        SnakeMovement previousSnakeMovePosition = null;
        if (snakeMovePositionList.Count > 0)
        {
            previousSnakeMovePosition = snakeMovePositionList[0];
        }
        snakeMovePositionList.Insert(0, new SnakeMovement(previousSnakeMovePosition, dir, previousGridPosition));
        bodyParts.Insert(0, Instantiate(bodyPartPrefab, grid.CellToWorld(previousGridPosition), Quaternion.identity));
        SpriteRenderer sp = bodyParts[0].GetComponent<SpriteRenderer>();
        (float angle, Sprite sprite, bool flip) tuple = GetBodyPartView(new SnakeMovement(new SnakeMovement(null, previousDirection, previousGridPosition), dir, previousGridPosition));
        sp.sprite = tuple.sprite;
        sp.flipX = tuple.flip;
        bodyParts[0].transform.eulerAngles = new Vector3(0, 0, tuple.angle);
    }
    public void UpdateSnakeHead()
    {
        desiredPos += Directions[dir];
        gridPosition = new Vector3Int((int)Mathf.Floor(desiredPos.x), (int)desiredPos.y, 0);
        Vector3 snakeWorldPos = grid.CellToWorld(gridPosition);
        transform.position = snakeWorldPos;
        transform.eulerAngles = new Vector3(0, 0, GetAngleFromVector(Directions[dir]) - 90);
    }
    public void CheckCollisions()
    {
        // With Borders
        if (borderTilemap.GetTile(gridPosition) != null)
        {
            game_manager_script.GameOver();
            return;
            
        }

        // With Body
        for (int i = 1; i < snakeMovePositionList.Count; i++)
        {
            if (gridPosition == snakeMovePositionList[i].GetGridPosition())
            {
                game_manager_script.GameOver();
                return;
            }
        }
    }
    public void MoveSnake()
    {
        gridMoveTimer += Time.deltaTime;
        if (gridMoveTimer >= gridMoveTimerMax)
        {
            previousGridPosition = gridPosition;
            UpdateSnakeHead();
            CheckCollisions();
            UpdateSnakeBody();
            previousDirection = dir;
            gridMoveTimer -= gridMoveTimerMax;
        }
    }

    // View Helpers
    private float GetAngleFromVector(Vector3 dir)
    {
        float n = Mathf.Atan2(dir.x, dir.y) * Mathf.Rad2Deg;
        if (n < 0) n += 360;
        return n;
    }
    private (float angle, Sprite sprite, bool flip) GetBodyPartView(SnakeMovement snakeMovement)
    {
        float angle = 0;
        Sprite sprite= bodyStaightSprite;
        bool flip = false;
        switch (snakeMovement.GetDirection())
        {
            default:
            case Direction.Up:
                switch (snakeMovement.GetPreviousSnakePositionDirection())
                {
                    default:
                        angle = 0;
                        sprite = bodyStaightSprite;
                        break;
                    case Direction.UpRight:
                        angle = 180;
                        sprite = body120Sprite;
                        break;
                    case Direction.DownRight:
                        angle = 180;
                        sprite = body60Sprite;
                        break;
                    case Direction.DownLeft:
                        angle = 180;
                        flip = true;
                        sprite = body60Sprite;
                        break;
                    case Direction.UpLeft:
                        angle = 180;
                        flip = true;
                        sprite = body120Sprite;                        
                        break;
                }
                break;
            case Direction.UpLeft:
                switch (snakeMovement.GetPreviousSnakePositionDirection())
                {
                    default:
                        angle = 300;
                        break;
                    case Direction.Up:
                        angle = 0;
                        sprite = body120Sprite;
                        flip = true;
                        break;
                    case Direction.UpRight:
                        angle = 60;
                        sprite = body60Sprite;
                        flip = true;
                        break;
                    case Direction.DownLeft:
                        angle = -120;
                        sprite = body120Sprite;
                        break;
                    case Direction.Down:
                        angle = 180;
                        sprite = body60Sprite;
                        break;
                }
                break;
            case Direction.UpRight:
                switch (snakeMovement.GetPreviousSnakePositionDirection())
                {
                    default:
                        angle = 60;
                        break;
                    case Direction.Up:
                        angle = 0;
                        sprite = body120Sprite;
                        break;
                    case Direction.UpLeft:
                        angle = -60;
                        sprite = body60Sprite;
                        break;
                    case Direction.DownRight:
                        angle = 120;
                        sprite = body120Sprite;
                        flip = true;
                        break;
                    case Direction.Down:
                        angle = 180;
                        sprite = body60Sprite;
                        flip = true;
                        break;
                }
                break;
            case Direction.DownRight:
                switch (snakeMovement.GetPreviousSnakePositionDirection())
                {
                    default:
                        angle = 120;
                        break;
                    case Direction.Down:
                        angle = 180;
                        sprite = body120Sprite;
                        flip = true;
                        break;
                    case Direction.DownLeft:
                        angle = -120;
                        sprite = body60Sprite;
                        flip = true;
                        break;
                    case Direction.UpRight:
                        angle = 60;
                        sprite = body120Sprite;
                        break;
                    case Direction.Up:
                        angle = 0;
                        sprite = body60Sprite;
                        break;
                }
                break;
            case Direction.DownLeft:
                switch (snakeMovement.GetPreviousSnakePositionDirection())
                {
                    default:
                        angle = 240;
                        break;
                    case Direction.Down:
                        angle = 180;
                        sprite = body120Sprite;
                        break;
                    case Direction.DownRight:
                        angle = 120;
                        sprite = body60Sprite;
                        break;
                    case Direction.UpLeft:
                        angle = -60;
                        sprite = body120Sprite;
                        flip = true;
                        break;
                    case Direction.Up:
                        angle = 0;
                        sprite = body60Sprite;
                        flip = true;
                        break;
                }
                break;
            case Direction.Down:
                switch (snakeMovement.GetPreviousSnakePositionDirection())
                {
                    default:
                        angle = 180;
                        break;
                    case Direction.DownRight:
                        angle = 120;
                        sprite = body120Sprite;
                        break;
                    case Direction.DownLeft:
                        angle = -120;
                        sprite = body120Sprite;
                        flip = true;
                        break;
                    case Direction.UpRight:
                        angle = 60;
                        sprite = body60Sprite;
                        break;
                    case Direction.UpLeft:
                        angle = -60;
                        sprite = body60Sprite;
                        flip = true;
                        break;
                }
                break;
        }
        (float, Sprite, bool) tuple = (-angle, sprite, flip);
        return tuple;
    }

    // Direction Buttons Handlers
    private void HandleButtonPress(Direction btnDirection)
    {
        if (!isAlive) return;
        TrySetDirection(btnDirection);
    }
    public void PressButtonUp()
    {
        HandleButtonPress(Direction.Up);
    }
    public void PressButtonDown()
    {
        HandleButtonPress(Direction.Down);
    }
    public void PressButtonUpRight()
    {
        HandleButtonPress(Direction.UpRight);
    }
    public void PressButtonUpLeft()
    {
        HandleButtonPress(Direction.UpLeft);
    }
    public void PressButtonDownRight()
    {
        HandleButtonPress(Direction.DownRight);
    }
    public void PressButtonDownLeft()
    {
        HandleButtonPress(Direction.DownLeft);
    }

    // Change Direction Helpers
    public bool IsOppositeDirection(Direction direction)
    {
        if ((direction == Direction.Up && previousDirection == Direction.Down)
            || (direction == Direction.Down && previousDirection == Direction.Up)
            || (direction == Direction.UpLeft && previousDirection == Direction.DownRight)
            || (direction == Direction.DownRight && previousDirection == Direction.UpLeft)
            || (direction == Direction.DownLeft && previousDirection == Direction.UpRight)
            || (direction == Direction.UpRight && previousDirection == Direction.DownLeft))
        {
            return true;
        }

        return false;
    }
    public void TrySetDirection(Direction direction)
    {
        if (!IsOppositeDirection(direction)) dir = direction;
    }

    // Additional Classes and other for Movement Logic
    public enum Direction
    {
        Up,
        UpLeft,
        UpRight,
        Down,
        DownLeft,
        DownRight,
    }
    public Dictionary<Direction, Vector3> Directions = new Dictionary<Direction, Vector3>()
    {
        { Direction.Up, new Vector3Int(1, 0, 0) },
        {Direction.UpLeft, new Vector3(0.5f, -1, 0) },
        {Direction.UpRight, new Vector3(0.5f, 1, 0) },
        {Direction.Down, new Vector3Int(-1, 0, 0) },
        {Direction.DownLeft, new Vector3(-0.5f, -1, 0) },
        {Direction.DownRight, new Vector3(-0.5f, 1, 0) }
    };
    public class SnakeMovement
    {
        public Direction direction;
        public Vector3Int gridPosition;
        public SnakeMovement previousSnakePosition;
        public SnakeMovement(SnakeMovement previousSnakePosition, Direction direction, Vector3Int gridPosition)
        {
            this.previousSnakePosition = previousSnakePosition;
            this.gridPosition = gridPosition;
            this.direction = direction;
        }

        public Direction GetDirection()
        {
            return this.direction;
        }

        public void SetDirection(Direction direction)
        {
            this.direction = direction;
        }

        public Vector3Int GetGridPosition()
        {
            return this.gridPosition;
        }

        public void SetGridPosition(Vector3Int gridPosition)
        {
            this.gridPosition = gridPosition;
        }

        public Direction GetPreviousSnakePositionDirection()
        {
            if (this.previousSnakePosition == null)
            {
                return Direction.Up;
            }
            return this.previousSnakePosition.GetDirection();
        }

        public SnakeMovement GetPreviousSnakePosition()
        {
            return this.previousSnakePosition;
        }

    }
}
