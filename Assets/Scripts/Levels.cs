using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelStowage", menuName = "Scriptable Objects/LevelStowage")]
public class Levels : ScriptableObject
{
    
    public Dictionary<int, int> getLevel(int levelNum)
    {
        switch (levelNum)
        {
            case 1: return level_1;
            default: 
                Debug.LogError($"Level {levelNum} not found.");
                return null;
        }
    }

    // key value correlates to position. Rows alternate between 12 and 11 balls wide. First diggit is row, third is position in row
    // from left to right. Three digits to prevent collission for key value "10"

    Dictionary<int, int> level_1 = new Dictionary<int, int>
    {
        //source
        {-1,0 }, 
        // Row 0 (12 wide): orange (color 3)
        {000, 3}, {001, 3}, {002, 3}, {003, 3}, {004, 3}, {005, 3}, {006, 3}, {007, 3}, {008, 3}, {009, 3}, {010, 3}, {011, 3},
        // Row 1 (11 wide): green (color 2) | purple (color 4) stripes
        {100, 2}, {101, 2}, {102, 2}, {103, 2}, {104, 2}, {105, 4}, {106, 4}, {107, 4}, {108, 4}, {109, 4}, {110, 4},
        // Row 2 (12 wide): green | purple stripes
        {200, 2}, {201, 2}, {202, 2}, {203, 2}, {204, 2}, {205, 2}, {206, 4}, {207, 4}, {208, 4}, {209, 4}, {210, 4}, {211, 4},
        // Row 3 (11 wide): green | purple stripes
        {300, 2}, {301, 2}, {302, 2}, {303, 2}, {304, 2}, {305, 4}, {306, 4}, {307, 4}, {308, 4}, {309, 4}, {310, 4},
        // Row 4 (12 wide): blue (color 1)
        {400, 1}, {401, 1}, {402, 1}, {403, 1}, {404, 1}, {405, 1}, {406, 1}, {407, 1}, {408, 1}, {409, 1}, {410, 1}, {411, 1},
        // Row 5 (11 wide): purple (color 4) | purple (color 4) stripes
        {500, 4}, {501, 4}, {502, 4}, {503, 4}, {504, 4}, {505, 2}, {506, 2}, {507, 2}, {508, 2}, {509, 2}, {510, 2},
        // Row 6 (12 wide): purple | green stripes
        {600, 4}, {601, 4}, {602, 4}, {603, 4}, {604, 4}, {605, 4}, {606, 2}, {607, 2}, {608, 2}, {609, 2}, {610, 2}, {611, 2},
        // Row 7 (11 wide): purple | green stripes
        {700, 4}, {701, 4}, {702, 4}, {703, 4}, {704, 4}, {705, 2}, {706, 2}, {707, 2}, {708, 2}, {709, 2}, {710, 2},
    };
}
