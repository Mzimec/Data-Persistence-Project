using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public class MainManager : MonoBehaviour
{


    public Brick BrickPrefab;
    public int LineCount = 6;
    public Rigidbody Ball;

    public Text curentName;
    public Text topName;

    private static int topScore;
    private static string topPlayer;

    public Text ScoreText;
    public GameObject GameOverText;

    private bool m_Started = false;
    private int m_Points;

    private bool m_GameOver = false;

    private void Awake()
    {
        LoadScore();
    }
    // Start is called before the first frame update
    void Start()
    {
        const float step = 0.6f;
        int perLine = Mathf.FloorToInt(4.0f / step);

        int[] pointCountArray = new[] { 1, 1, 2, 2, 5, 5 };
        for (int i = 0; i < LineCount; ++i)
        {
            for (int x = 0; x < perLine; ++x)
            {
                Vector3 position = new Vector3(-1.5f + step * x, 2.5f + i * 0.3f, 0);
                var brick = Instantiate(BrickPrefab, position, Quaternion.identity);
                brick.PointValue = pointCountArray[i];
                brick.onDestroyed.AddListener(AddPoint);
            }
        }
        curentName.text = DataManager.instance.playerName;
        SetBest();
    }

    private void Update()
    {
        if (!m_Started)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                m_Started = true;
                float randomDirection = Random.Range(-1.0f, 1.0f);
                Vector3 forceDir = new Vector3(randomDirection, 1, 0);
                forceDir.Normalize();

                Ball.transform.SetParent(null);
                Ball.AddForce(forceDir * 2.0f, ForceMode.VelocityChange);
            }
        }
        else if (m_GameOver)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }
    }

    void AddPoint(int point)
    {
        m_Points += point;
        DataManager.instance.score = m_Points;
        ScoreText.text = $"Score : {m_Points}";
    }

    public void GameOver()
    {
        m_GameOver = true;
        WhoIsBest();
        GameOverText.SetActive(true);
    }

    void WhoIsBest()
    {
        int currentScore = DataManager.instance.score;

        if (currentScore > topScore)
        {
            topPlayer = DataManager.instance.playerName;
            topScore = currentScore;
            topName.text = $"Best Score: {topPlayer}: {topScore}";
            SaveScore(topPlayer, topScore);
        }
    }

    private void SetBest()
    {
        if (topPlayer == null && topScore == 0)
        {
            topName.text = "";
        }
        else
        {
            topName.text = $"Best Score: {topPlayer} : {topScore}";
        }
    }

    public void SaveScore(string bestName, int bestScore)
    {
        SaveData data = new SaveData();
        data.theTopPlayer = bestName;
        data.theTopScore = bestScore;
        string json = JsonUtility.ToJson(data);
        File.WriteAllText(Application.persistentDataPath + "/savefile.json", json);
    }

    public void LoadScore()
    {
        string path = Application.persistentDataPath + "/savefile.json";
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            SaveData data = JsonUtility.FromJson<SaveData>(json);
            topPlayer = data.theTopPlayer;
            topScore = data.theTopScore;
        }
    }

    [System.Serializable]
    class SaveData
    {
        public string theTopPlayer;
        public int theTopScore;
    }
} 
