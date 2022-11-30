using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Game_Manager_Script : MonoBehaviour
{

    [SerializeField]
    private int gridRadius = 4;

    [SerializeField]
    private GameObject applePrefab;


    [SerializeField]
    private Grid grid;

    [SerializeField]
    private Snake_Movement_Script snake_script;
    private Vector3Int appleGridPos;
    private GameObject apple;

    private int score = 0;

    [Header("UI")]

    [SerializeField]
    private TextMeshProUGUI scoreText;
    [SerializeField]
    private GameObject restartPanel;


    void Start()
    {
        SpawnApple();
        score = 0;
        scoreText.text = "Score: " + score;
    }

    void Update()
    {
        
    }

    public void SpawnApple()
    {
        Vector3 worldPos;
        do {
            int y = 0;
            int x = 0;
            y = Random.Range(-gridRadius, gridRadius);
            switch (y)
            {
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
            appleGridPos = new Vector3Int(y, x, 0);
            worldPos = grid.CellToWorld(appleGridPos); }
        while (snake_script.GetFullGridPositionList().IndexOf(appleGridPos) != -1);

        apple = Instantiate(applePrefab, worldPos, Quaternion.identity);
    }

    public void MoveSnake(Vector3Int snakePos)
    {
        if(snakePos == appleGridPos)
        {
            Destroy(apple);
            snake_script.IncreaseBodyLength();
            scoreText.text = "Score: " + ++score;
            SpawnApple();
        }
    }

    public void GameOver()
    {
        snake_script.SetIsAlive(false);
        restartPanel.SetActive(true);
    }

    public void RestartGame()
    {
        snake_script.ResetSnake();
        score = 0;
        scoreText.text = "Score: " + score;
        Destroy(apple);
        restartPanel.SetActive(false);
        SpawnApple();
    }
}
