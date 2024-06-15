using UnityEngine;
using UnityEngine.SceneManagement; // Необходимо для управления сценами

public class SceneSwitcher : MonoBehaviour
{
    // Метод для загрузки сцены
    public void SwitchScene(int sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
