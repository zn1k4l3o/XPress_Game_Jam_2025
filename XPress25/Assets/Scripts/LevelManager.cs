using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    [Header("LevelSetup")]
    public bool isTimed = false;
    public bool isKillEveryone = false;
    public bool isCollectItems = false;

    public GameObject TutorialPanel;
    public GameObject DeathScreen;
    public GameObject WinScreen;
    private bool isRunning = false;
    private bool isFinished = false;

    public PlayerController playerController;
    public List<EnemyController> enemyControllers;

    public Text timer;
    public float targetTime;
    private float timeAlive;

    void Start()
    {
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        foreach(GameObject obj in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            enemyControllers.Add(obj.GetComponent<EnemyController>());
        }
        ChangeGameState(false);
        DeathScreen.SetActive(false);
        WinScreen.SetActive(false);
    }

    void Update()
    {
        if (isRunning)
        {
            timeAlive += Time.deltaTime;
        }
        if (isTimed)
        {
            CheckTimer();
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

    public void StartLevelQuitTutorial()
    {
        TutorialPanel.SetActive(false);
        isRunning = true;
        ChangeGameState(true);
    }

    public void ChangeGameState(bool canPlay)
    {
        playerController.canPlay = canPlay;
        foreach (EnemyController controller in enemyControllers)
        {
            controller.canPlay = canPlay;
        }
    }

    public void ResetProgress()
    {
        SceneManager.LoadScene(0);
    }

    public void SwitchScene(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
    }

    public void ShowDeathScreen()
    {
        ChangeGameState(false);
        DeathScreen.SetActive(true);
    }

    public void UpdateTimer()
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


}
