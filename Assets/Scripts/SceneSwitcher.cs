using UnityEngine;
using UnityEngine.SceneManagement; // ���������� ��� ���������� �������

public class SceneSwitcher : MonoBehaviour
{
    // ����� ��� �������� �����
    public void SwitchScene(int sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
