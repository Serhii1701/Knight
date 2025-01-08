using System.Collections;
using System.Collections.Generic;
//using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public bool isGameActive;

    Health playerHealth;

    [SerializeField] GameObject gameMenu;
    [SerializeField] GameObject resumeGameButton;

    private void Awake()
    {
        playerHealth = GameObject.Find("Player").GetComponent<Health>();
        Resume();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (!isGameActive)
            {
                Resume();
            }
            else
            {
                PauseGame();
            }
        }

        if (playerHealth.isDie)
        {
            GameOver();
        }
    }

    void PauseGame()
    {
        Time.timeScale = 0;
        isGameActive = false;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        SetActiveGameMenu();
    }

    public void Resume()
    {
        Time.timeScale = 1;
        isGameActive = true;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        SetInactiveGameMenu();
    }

    void GameOver()
    {
        PauseGame();
        SetActiveGameMenu();
    }

    void SetActiveGameMenu()
    {
        gameMenu.SetActive(true);
        if (playerHealth.isDie)
        {
            resumeGameButton.SetActive(false);
        }
    }

    void SetInactiveGameMenu()
    {
        gameMenu.SetActive(false);
    }

    public void RestartGame()
    {
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().name);
        Resume();
    }

    public void SaveScene()
    {
        DataPersistenceManager.Instance.SaveGame();
    }

    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene(0);
    }
}
