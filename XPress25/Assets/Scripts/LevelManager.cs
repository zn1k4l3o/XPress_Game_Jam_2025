using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    [Header("LevelSetup")]
    public bool isTimed = false;
    public bool isKillEveryone = false;
    public bool isCollectItems = false;
    public bool isTwoPhase = false;

    public GameObject TutorialPanel;
    public GameObject DeathScreen;
    public GameObject WinScreen;
    public GameObject PausePanel;
    private bool isRunning = false;
    private bool isFinished = false;

    public PlayerController playerController;
    public List<EnemyController> enemyControllers;

    public Text timer;
    public float targetTime;
    private float timeAlive;

    public Vector2 topRightSpawnBorder;
    public Vector2 bottomLeftSpawnBorder;

    public int maxNumOfEnemies = 8;
    public float timeToSpawn = 5f;
    public float spawnTimer = 2f;

    public GameObject enemyPrefab;
    public int collectedItems = 0;
    public int totalItems = 15;
    public GameObject boss;
    private bool isSecondaPhase = false;

    void Start()
    {
        if (isCollectItems)
        {
            totalItems = GameObject.FindGameObjectsWithTag("Collectable").Length;
        }
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        foreach(GameObject obj in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            enemyControllers.Add(obj.GetComponent<EnemyController>());
        }
        ChangeGameState(false);
        DeathScreen.SetActive(false);
        WinScreen.SetActive(false);
        if (PausePanel != null)
            PausePanel.SetActive(false);
        UpdateTimer();
    }

    void Update()
    {
        if (isRunning)
        {
            timeAlive += Time.deltaTime;
            spawnTimer -= Time.deltaTime;
        }
        if (isTimed)
        {
            CheckTimer();
            UpdateTimer();
        }
        if (spawnTimer < 0 && isRunning)
        {
            if (GameObject.FindGameObjectsWithTag("Enemy").Length < maxNumOfEnemies)
            {
                spawnTimer = timeToSpawn;
                SpawnEnemy();
            }
        }
        if (isTwoPhase)
        {
            if (timeAlive > targetTime)
            {
                GameObject newBoss = Instantiate(boss, bottomLeftSpawnBorder, Quaternion.identity);
                newBoss.GetComponent<EnemyController>().canPlay = true;
                newBoss.GetComponent<EnemyController>().isFinal = true;
                enemyControllers.Add(newBoss.GetComponent<EnemyController>());
                GameObject.Find("Audio").GetComponent<FinalAudio>().ReplaceMusic();
                isTwoPhase = false;
            }
            UpdateTimer();

        }
    }

    private void CheckTimer()
    {
        if (timeAlive >= targetTime)
        {
            ChangeGameState(false);
            WinScreen.SetActive(true);
        }
    }

    public void SpawnEnemy()
    {
        Vector3 spawnPos = new Vector3(Random.Range(bottomLeftSpawnBorder.x, topRightSpawnBorder.x), Random.Range(bottomLeftSpawnBorder.y, topRightSpawnBorder.y), 0f);
        //while (Vector3.Distance(spawnPos, playerController.transform.position) < 3f)
        //{
        //    spawnPos = new Vector3(Random.Range(bottomLeftSpawnBorder.x, topRightSpawnBorder.x), Random.Range(bottomLeftSpawnBorder.y, topRightSpawnBorder.y), 0f);
        //}
        GameObject newEnemy = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
        newEnemy.GetComponent<EnemyController>().canPlay = true;
        enemyControllers.Add(newEnemy.GetComponent<EnemyController>());
    }

    public void StartLevelQuitTutorial()
    {
        TutorialPanel.SetActive(false);
        isRunning = true;
        ChangeGameState(true);
    }

    public void ChangeGameState(bool canPlay)
    {
        //playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        playerController.canPlay = canPlay;
        foreach (EnemyController controller in enemyControllers)
        {
            controller.canPlay = canPlay;
        }
        isRunning = canPlay;
    }

    public void ResetProgress()
    {
        SceneManager.LoadScene(0);
    }

    public void SwitchScene(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
    }

    public void ShowDeathScreen(int numOfDeaths)
    {
        ChangeGameState(false);
        DeathScreen.SetActive(true);
        DeathScreen.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = numOfDeaths.ToString();
    }

    public void UpdateTimer()
    {
        if (isTimed || isTwoPhase)
        {
            if (timeAlive >= targetTime)
            {
                timer.text = targetTime.ToString("F1") + " / " + targetTime.ToString("F1");
                timer.color = Color.green;
            }
            else
            {
                timer.text = timeAlive.ToString("F1") + " / " + targetTime.ToString("F1");
                timer.color = Color.red;
            }
        }
        else if (isKillEveryone || isCollectItems)
        {
            if (collectedItems >= totalItems)
            {
                timer.text = totalItems + " / " + totalItems;
                timer.color = Color.green;
            }
            else
            {
                timer.text = collectedItems + " / " + totalItems;
                timer.color = Color.red;
            }
        }


    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void SetPause(bool open)
    {
        enemyControllers.Clear();
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            enemyControllers.Add(obj.GetComponent<EnemyController>());
        }
        ChangeGameState(!open);
        PausePanel.SetActive(open);
    }

    public void AddItem()
    {
        collectedItems++;
        if (collectedItems == totalItems)
        {
            ChangeGameState(false);
            WinScreen.SetActive(true);
        }
        UpdateTimer();
    }

    public void FinalWin()
    {
        ChangeGameState(false);
        WinScreen.SetActive(true);
    }

}
