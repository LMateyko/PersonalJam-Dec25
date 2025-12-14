using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuUI : MonoBehaviour
{
    public void OnStartGame(string levelName)
    {
        // TODO: Add a fade to the gui elements to have a transition directly into the level
        SceneManager.LoadScene(levelName, LoadSceneMode.Single);
    }

    public void OnExitGame()
    {
        Application.Quit();
    }
}
