using UnityEngine;
using TMPro;

public class EndGameScript : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI endGameText;
    [SerializeField] private ScoreBoardScript scoreBoard;

    public static EndGameScript instance; //marks singleton. I really shound have done this for PlayScript too 

    private void Awake()
    {
        gameObject.SetActive(false);

        if (instance == null)
        {
            instance = this;
        }
    }

    public void winGame()
    {
        gameObject.SetActive(true);
        endGameText.text = $"You Win!\n{scoreBoard.getScore()}";
    }

    public void loseGame()
    {
        gameObject.SetActive(true);
        endGameText.text = "Try Again Later";
    }
}
