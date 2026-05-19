using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonManager : MonoBehaviour
{
    public void RespawnPlayer()
    {
        GameObject obj = GameObject.Find("UIManager");
        var script = obj.GetComponent<UIManager>();
        script.RespawnPlayer();
    }
    public void QuitGame()
    {
        GameObject obj = GameObject.Find("UIManager");
        var script = obj.GetComponent<UIManager>();
        script.QuitGame();
    }

    public void RestartGame()
    {
        GameObject obj = GameObject.Find("UIManager");
        var script = obj.GetComponent<UIManager>();
        script.RestartGame();
    }
}
