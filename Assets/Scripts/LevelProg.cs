using UnityEngine;
using System.Collections;
using System.Collections.Generic;


[CreateAssetMenu(fileName = "LevelProg", menuName = "Scriptable Objects/LevelProg")]
public class LevelProg : ScriptableObject
{
    public Queue<(int ballId, int colorID)> getLevelBag(int levelNum)
    {
        switch (levelNum)
        {
            case 1: return ballBag_1;
            default:
                Debug.LogError($"Level {levelNum} not found.");
                return null;
        }
    }

    // bags are simpler and have a simple KeyValue and corresponding ColorID
    // key correlates to order given to player, starting at 1000 (to prevent collissions when linked to graph)
    // Bag number corresponds directly to level number

    Queue<(int ballId, int colorID)> ballBag_1 = new Queue<(int ballId, int colorId)>(new[]
    {
        (1001, 2),
        (1002, 2),
        (1003, 3),
        (1004, 4),
        (1005, 3),
        (1006, 1),
        (1007, 2),
        (1008, 2),
        (1009, 3),
        (1010, 4),
        (1011, 3)
    });
        
    
}
