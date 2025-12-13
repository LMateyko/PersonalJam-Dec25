using UnityEngine;
using UnityEngine.SceneManagement;

public class Collectable_StageClear : MonoBehaviour
{
    [SerializeField] private string NextStage = "Menu";

    private void OnTriggerEnter2D(Collider2D collision)
    {
        ClearStage();
    }

    private void ClearStage()
    {
        SceneManager.LoadScene(NextStage, LoadSceneMode.Single);
    }
}
