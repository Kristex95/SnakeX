using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Snake_Movement_Script : MonoBehaviour
{
    [SerializeField]
    private Grid grid;

    [SerializeField]
    private Vector3 dir;
    private Vector3 desiredPos = Vector3.zero;

    private float gridMoveTimer;
    private float gridMoveTimerMax;


    private void Awake()
    {
        gridMoveTimerMax = 1f;
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
            transform.position = grid.CellToWorld(new Vector3Int((int)Mathf.Floor(desiredPos.x), (int)desiredPos.y, 0));
        }

    }
}
