using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class UIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI coinCounterText;
    private static UIManager instance = null;

    [SerializeField] private Character character;
    [SerializeField] private Image healthBar;
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
    }

    public void CollectCoin()
    {
        this.statistics.coinCounter++;
        string coinText = $" {this.statistics.coinCounter} ";
        this.coinCounterText.text = coinText;
    }
}