using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : Menu
{
    [SerializeField] GameObject mainMenu;

    [Header("Menu Navigation")]
    [SerializeField] SaveSlotsMenu saveSlotsMenu;

    [Header("Menu Buttons")]
    [SerializeField] Button newGameButton;
    [SerializeField] Button loadButton;
    
    private void Start()
    {
        DisableButtonsDependingOnData();
    }

    private void DisableButtonsDependingOnData()
    {
        if (!DataPersistenceManager.Instance.HasGameData())
        {
            loadButton.interactable = false;
            newGameButton.interactable = false;
        }
    }

    public void LoadNewGame()
    {
        //create a new game which will initialize our game data
        //DataPersistenceManager.Instance.NewGame();
        //Load the gameplay scene - which will in turn save the game because of OnSceneUnLoaded() in the DataPersistenceManager
        //SceneManager.LoadSceneAsync("Main");
        saveSlotsMenu.ActivateMenu(false);
        this.DeactivateMenu();
    }

    public void LoadSavedGame()
    {
        //Load the next scene - which will in turn load the game because of OnSceneLoaded() in the DataPersistenceManager
        //DataPersistenceManager.Instance.LoadGame();
        //SceneManager.LoadSceneAsync("Main");
        saveSlotsMenu.ActivateMenu(true);
        this.DeactivateMenu();
        
    }

    public void ContinueGame()
    {
        // save the game anytime before loading a new scene
        DataPersistenceManager.Instance.SaveGame();
        //load the next scene - which will in turn load the game because of 
        // OnSceneLoaded() in the DataPersistenceManager
        SceneManager.LoadSceneAsync("Main");
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void ActivateMenu()
    {
        mainMenu.SetActive(true);
        DisableButtonsDependingOnData();
    }

    public void DeactivateMenu()
    {
        mainMenu.SetActive(false);
    }
}
