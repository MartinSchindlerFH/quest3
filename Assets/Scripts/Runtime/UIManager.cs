using System;
using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class UIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI coinCounterText;
    private static UIManager instance = null;

    [SerializeField] private Character character;
    [SerializeField] private Image healthBar;
    [SerializeField] private GameObject respawnPoint;

    [SerializeField] private CanvasGroup hudCanvasGroup;
    [SerializeField] private CanvasGroup gameOverCanvaseGroup;
    [SerializeField] private CanvasGroup wonCanvasGroup;
    [SerializeField] private float fadingTime = 2.0f;

    private bool isFadingin = false;



    public static UIManager Instance => instance;
    private class PlayerStatistics
    {
        public int coinCounter = 0;
        //... add more statistics ^^ (e.g. enemies jumped on etc.)
    }

    private PlayerStatistics statistics;
    private void Awake()
    {
        instance = this;
        this.statistics = new PlayerStatistics() { coinCounter = 0 };
    }
    private void Update()
    {
        float percent = this.character.GetHealth();
        this.healthBar.fillAmount = percent;

        if (percent <= 0.0f && !this.isFadingin)
        {
            this.StartCoroutine(this.FadeInCanvas(hudCanvasGroup,gameOverCanvaseGroup));
        }
    }

    public void CollectCoin()
    {
        this.statistics.coinCounter++;
        string coinText = $" {this.statistics.coinCounter} ";
        this.coinCounterText.text = coinText;
    }

    public void RespawnPlayer()
    {
        var controller = character.gameObject.GetComponent<CharacterController>();
        controller.enabled = false;
        controller.transform.position = this.respawnPoint.transform.position;
        controller.enabled = true;

        character.SetHealth(character.GetMaxHealth());

        this.statistics.coinCounter = 0;
        string coinText = $" {this.statistics.coinCounter} ";
        this.coinCounterText.text = coinText;

        this.StartCoroutine(FadeInCanvas(gameOverCanvaseGroup, hudCanvasGroup));
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void RestartGame()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);
    }

    public void GameFinished()
    {
        FadeInCanvas(hudCanvasGroup, wonCanvasGroup);
    }

    private IEnumerator FadeInCanvas(CanvasGroup oldCanvas, CanvasGroup newCanvas)
    {
        if(oldCanvas == hudCanvasGroup) character.GetComponent<CharacterController>().enabled = false;
        if(newCanvas == hudCanvasGroup) character.GetComponent<CharacterController>().enabled = true;

        this.isFadingin = true;
        float timer = 0.0f;
        while (timer < this.fadingTime)
        {
            float percent = timer / this.fadingTime;
            oldCanvas.alpha = 1.0f - percent;
            newCanvas.alpha = percent;
            yield return null;
            timer += Time.deltaTime;
        }
        oldCanvas.alpha = 0.0f;
        newCanvas.alpha = 1.0f;

        this.isFadingin = false;
    }


}