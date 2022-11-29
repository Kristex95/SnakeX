using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Snake_Movement_Script : MonoBehaviour
{

    public enum Direction
    {
        Up,
        UpLeft,
        UpRight,
        Down,
        DownLeft,
        DownRight,
    }

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

    [SerializeField]
    private Sprite bodySprite;

    [SerializeField]
    private Grid grid;

    [SerializeField]
    private Tilemap borderTilemap;

    [SerializeField]
    private Direction dir;
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
    private List<SnakeMovement> snakeMovePositionList = new List<SnakeMovement>();

    private void Awake()
    {
        gridMoveTimerMax = 0.6f;
        gridMoveTimer = gridMoveTimerMax;
    }

    // Start is called before the first frame update
    void Start()
    {
        dir = Direction.Up;
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

    public bool IsOppositeDirection(Direction direction)
    {
        if ((direction == Direction.Up && dir == Direction.Down)
            || (direction == Direction.Down && dir == Direction.Up)
            || (direction == Direction.UpLeft && dir == Direction.DownRight)
            || (direction == Direction.DownRight && dir == Direction.UpLeft)
            || (direction == Direction.DownLeft && dir == Direction.UpRight)
            || (direction == Direction.UpRight && dir == Direction.DownLeft))
        {
            return true;
        }

        return false;
    }

    public void TrySetDirection(Direction direction)
    {
        if (!IsOppositeDirection(direction)) dir = direction;
    }

    public void GetInput()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            TrySetDirection(Direction.Up);
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            TrySetDirection(Direction.Down);
        }
        else if (Input.GetKey(KeyCode.E))
        {
            TrySetDirection(Direction.UpRight);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            TrySetDirection(Direction.DownRight);
        }
        else if (Input.GetKey(KeyCode.Q))
        {
            TrySetDirection(Direction.UpLeft);
        }
        else if (Input.GetKey(KeyCode.A))
        {
            TrySetDirection(Direction.DownLeft);
        }
    }

    public void MovePosition()
    {
        gridMoveTimer += Time.deltaTime;

        if (gridMoveTimer >= gridMoveTimerMax)
        {
            gridMoveTimer -= gridMoveTimerMax;

            desiredPos += Directions[dir];

            gridPosition = new Vector3Int((int)Mathf.Floor(desiredPos.x), (int)desiredPos.y, 0);
            Vector3 snakeWorldPos = grid.CellToWorld(gridPosition);

            // Check border Collision
            if (borderTilemap.GetTile(gridPosition) != null)
            {
                game_manager_script.GameOver();
                return;
            }

            SnakeMovement previousSnakeMovePosition = null;
            if (snakeMovePositionList.Count > 0)
            {
                previousSnakeMovePosition = snakeMovePositionList[0];
            }
            snakeMovePositionList.Insert(0, new SnakeMovement(previousSnakeMovePosition, dir, gridPosition));

            // Remove Extra Piece On Rerender
            if (snakeMovePositionList.Count >= snakeBodySize + 1)
                snakeMovePositionList.RemoveAt(snakeMovePositionList.Count - 1);

            // Create Snake Body Part
            for (int i = 1; i < snakeMovePositionList.Count; i++)
            {
                GameObject bodyPart = new GameObject();
                bodyPart.AddComponent<SpriteRenderer>().sprite = bodySprite;

                Vector3 bodyPartWorldPos = grid.CellToWorld(snakeMovePositionList[i].GetGridPosition());

                bodyPart.transform.position = new Vector3(bodyPartWorldPos.x, bodyPartWorldPos.y, 0);
                bodyPart.transform.eulerAngles = new Vector3(0, 0, GetAngleForBodyPart(snakeMovePositionList[i-1]));
                Destroy(bodyPart, gridMoveTimerMax);
            }

            // Check Snake Body collisions;
            for (int i = 1; i < snakeMovePositionList.Count; i++)
            {
                if (gridPosition == snakeMovePositionList[i].GetGridPosition())
                {
                    game_manager_script.GameOver();
                }

            }

            transform.position = snakeWorldPos;
            transform.eulerAngles = new Vector3(0, 0, GetAngleFromVector(Directions[dir]) - 90);
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

    private float GetAngleForBodyPart(SnakeMovement snakeMovement)
    {
        float angle = 0;
        switch (snakeMovement.GetDirection())
        {
            default:
            case Direction.Up:
                switch (snakeMovement.GetPreviousSnakePositionDirection())
                {
                    default:
                        angle = 0;
                        break;
                    case Direction.UpRight:
                        angle = 30;
                        break;
                    case Direction.DownRight:
                        angle = 60;
                        break;
                    case Direction.DownLeft:
                        angle = 300;
                        break;
                    case Direction.UpLeft:
                        angle = 330;
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
                        angle = 330;
                        break;
                    case Direction.UpRight:
                        angle = 360;
                        break;
                    case Direction.DownLeft:
                        angle = 270;
                        break;
                    case Direction.Down:
                        angle = 240;
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
                        angle = 30;
                        break;
                    case Direction.UpLeft:
                        angle = 0;
                        break;
                    case Direction.DownRight:
                        angle = 90;
                        break;
                    case Direction.Down:
                        angle = 120;
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
                        angle = 150;
                        break;
                    case Direction.DownLeft:
                        angle = 180;
                        break;
                    case Direction.UpRight:
                        angle = 90;
                        break;
                    case Direction.Up:
                        angle = 60;
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
                        angle = 210;
                        break;
                    case Direction.DownRight:
                        angle = 180;
                        break;
                    case Direction.UpLeft:
                        angle = 270;
                        break;
                    case Direction.Up:
                        angle = 300;
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
                        angle = 150;
                        break;
                    case Direction.DownLeft:
                        angle = 210;
                        break;
                    case Direction.UpRight:
                        angle = 120;
                        break;
                    case Direction.UpLeft:
                        angle = 240;
                        break;
                }
                break;
        }
        return -angle;
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
}
