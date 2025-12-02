using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    public GameObject painelGameOver;

    public void MostrarGameOver()
    {
        painelGameOver.SetActive(true);
        Time.timeScale = 0f; // pausa o jogo
    }

    public void JogarNovamente()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}