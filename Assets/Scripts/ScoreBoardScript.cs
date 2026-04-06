using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class ScoreBoardScript : MonoBehaviour
{

    public static ScoreBoardScript instance; //marks singleton. I really shound have done this for PlayScript too 

    public int getScore()
    {
        return score;
    }

   
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    private int score;
    private int multiplier;
    private int comboTracker;

    public TMP_Text scoreText;
    public TMP_Text multiplierText;
    public TMP_Text comboText;
    void Start()
    {
        setReset();
    }

    // Update diplay called whenever thing are updated/changed
    void updateDisplay()
    {
        scoreText.text = $"Score: {score}";
        multiplierText.text = $"multiplier: {multiplier}";
        comboText.text = $"combo: {comboTracker * 10}%";
    }

    public void scoreCounter()
    {
        score += 10 * multiplier;
        updateDisplay();
    }

    public void combo(int comboCounter)
    {
        comboTracker += comboCounter;

        if (comboTracker >= 10)
        {
            multiplier += comboTracker / 10;
            comboTracker = comboTracker / 10;
        }
        updateDisplay();
    }

    public void setReset()
    {
        score = 0;
        multiplier = 1;
        comboTracker = 0;
        updateDisplay();
    }
}
