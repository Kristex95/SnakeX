using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Apple_Create_Script : MonoBehaviour
{
    [SerializeField]
    private int gridRadius = 4;

    [SerializeField]
    private GameObject apple;

    [SerializeField]
    private Grid grid;

    private Vector3Int pos;

    // Start is called before the first frame update
    void Start()
    {
        int y = 0;
        int x = 0;
        y = Random.Range(-gridRadius, gridRadius);
        switch (y){
            case 4:
                x = 0;
                break;
            case -4:
                x = 0;
                break;
            case 3:
                x = Random.Range(-2, 2);
                break;
            case -3:
                x = Random.Range(-2, 2);
                break;
            case 2:
                x = Random.Range(-3, 3);
                break;
            case -2:
                x = Random.Range(-3, 3);
                break;
            default:
                x = Random.Range(-4, 4);
                break;
        }
        pos = new Vector3Int(y, x, 0);
        Vector3 worldPos = grid.CellToWorld(pos);
        Instantiate(apple, worldPos, Quaternion.identity);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
