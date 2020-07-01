using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelLengthChecker : MonoBehaviour
{
    public string longestLength; // read only
    public string shortestLength; // read only 
    public string allLevelLengthsInOneString4Ya;

    public int[] repeatsPerObjective; // read only

    public string[] allLevelLengths;
    public bool checkAllLevels;

    public enum DataType
    {
        Seconds,
        NodeHits,
        Bars
    }
    public DataType dataTypeToDisplay;
    public int levelToCheck;    

    public void CheckLevelLength()
    {
        GameManager gameManager = FindObjectOfType<GameManager>();
        MusicMeter musicMeter = FindObjectOfType<MusicMeter>();
        float secondsPerBeat = 60f / musicMeter.bpm;

        if (checkAllLevels)
        {
            int stringIndex = 0;
            allLevelLengths = new string[2 * gameManager.levels.Length];
            for (int i = 0; i < allLevelLengths.Length / 2; i++)
            {
                LevelDesigner levelDesigner = gameManager.levels[i].GetComponent<LevelDesigner>();
                LongestLength(levelDesigner, secondsPerBeat);
                ShortestLength(levelDesigner, secondsPerBeat);
                allLevelLengths[stringIndex] = i + "  longest:" + longestLength;
                stringIndex++;
                allLevelLengths[stringIndex] = i + " shortest:" + shortestLength;
                stringIndex++;
            }
        }
        else
        {
            LevelDesigner levelDesigner = gameManager.levels[levelToCheck].GetComponent<LevelDesigner>();
            LongestLength(levelDesigner, secondsPerBeat);
            ShortestLength(levelDesigner, secondsPerBeat);
        }

        for (int i = 0; i < allLevelLengths.Length; i++)
        {
            allLevelLengthsInOneString4Ya = allLevelLengthsInOneString4Ya + " " + allLevelLengths[i];
        }

        shortestLengthHP = " ";
        lvlNum = 0;
        for (int i = 0; i < gameManager.levels.Length; i++)
        {
            lvlNum++;
            LevelDesigner levelDesigner = gameManager.levels[i].GetComponent<LevelDesigner>();
            ShortestLengthHP(levelDesigner, secondsPerBeat);
        }
    }
    public int lvlNum;
    int lvlNumChecker;

    public string shortestLengthHP;
    private void ShortestLengthHP(LevelDesigner levelDesigner, float secondsPerBeat)
    {
        int nodesHitTotal = 0;
        nodesHitTotal += levelDesigner.levelObjectives[0].targetNode;

        GameManager gameManager = FindObjectOfType<GameManager>();
        int energy = gameManager.startEnergy;
        
        for (int i = 0; i < levelDesigner.levelObjectives.Length - 1; i++)
        {
            if (lvlNumChecker != lvlNum)
            {
                nodesHitTotal += levelDesigner.nodeSettings.Length;
                int length = levelDesigner.nodeSettings.Length;
                int thisT = levelDesigner.levelObjectives[i].targetNode;
                int nextT = levelDesigner.levelObjectives[i + 1].targetNode;
                if (thisT < nextT)
                {
                    nodesHitTotal += nextT - thisT;
                }
                else
                {
                    nodesHitTotal += length - thisT;
                    nodesHitTotal += nextT;
                }


                energy += 1 + levelDesigner.levelObjectives[i].spawnTimings.Length;

                if (energy >= gameManager.maxEnergy)
                {
                    lvlNumChecker = lvlNum;
                    float timePerOrbit = levelDesigner.beatsPerBar * secondsPerBeat;
                    float timePerNodeHitAvg = timePerOrbit / levelDesigner.nodeSettings.Length;

                    float barsPerNodeAvg = 1f / levelDesigner.nodeSettings.Length;
                    float barsLevelTotal = barsPerNodeAvg * nodesHitTotal;
                    float timeLevelTotal = timePerNodeHitAvg * nodesHitTotal;

                    shortestLengthHP = shortestLengthHP + " " + lvlNum + ":" + i + ": b:" + barsLevelTotal + " t:" + timeLevelTotal;
                }
            }







        }



        //switch (dataTypeToDisplay)
        //{
        //    case DataType.Seconds:
        //        longestLength = timeLevelTotal.ToString();
        //        break;
        //    case DataType.NodeHits:
        //        longestLength = nodesHitTotal.ToString();
        //        break;
        //    case DataType.Bars:
        //        longestLength = barsLevelTotal.ToString();
        //        break;
        //}
    }

    private void LongestLength(LevelDesigner levelDesigner, float secondsPerBeat)
    {
        int nodesHitTotal = 0;
        nodesHitTotal += levelDesigner.levelObjectives[0].targetNode;

        int energy = FindObjectOfType<GameManager>().startEnergy;

        repeatsPerObjective = new int[levelDesigner.levelObjectives.Length];

        for (int i = 0; i < levelDesigner.levelObjectives.Length - 1; i++)
        {
            repeatsPerObjective[i] = energy - 1;
            for (int e = 0; e < repeatsPerObjective[i]; e++)
            {
                nodesHitTotal += levelDesigner.nodeSettings.Length;
            }
            energy = 1 + 1 + levelDesigner.levelObjectives[i].spawnTimings.Length;
            

            nodesHitTotal += levelDesigner.nodeSettings.Length;
            int length = levelDesigner.nodeSettings.Length;
            int thisT = levelDesigner.levelObjectives[i].targetNode;
            int nextT = levelDesigner.levelObjectives[i + 1].targetNode;
            if (thisT < nextT)
            {
                nodesHitTotal += nextT - thisT;
            }
            else
            {
                nodesHitTotal += length - thisT;
                nodesHitTotal += nextT;
            }
        }

        float timePerOrbit = levelDesigner.beatsPerBar * secondsPerBeat;
        float timePerNodeHitAvg = timePerOrbit / levelDesigner.nodeSettings.Length;

        float barsPerNodeAvg = 1f / levelDesigner.nodeSettings.Length;
        float barsLevelTotal = barsPerNodeAvg * nodesHitTotal;
        float timeLevelTotal = timePerNodeHitAvg * nodesHitTotal;

        switch (dataTypeToDisplay)
        {
            case DataType.Seconds:
                longestLength = timeLevelTotal.ToString();
                break;
            case DataType.NodeHits:
                longestLength = nodesHitTotal.ToString();
                break;
            case DataType.Bars:
                longestLength = barsLevelTotal.ToString();
                break;
        }
    }


    private void ShortestLength(LevelDesigner levelDesigner, float secondsPerBeat)
    {
        int nodesHitTotal = 0;
        nodesHitTotal += levelDesigner.levelObjectives[0].targetNode;

        for (int i = 0; i < levelDesigner.levelObjectives.Length - 1; i++)
        {
            nodesHitTotal += levelDesigner.nodeSettings.Length;

            int length = levelDesigner.nodeSettings.Length;
            int thisT = levelDesigner.levelObjectives[i].targetNode;
            int nextT = levelDesigner.levelObjectives[i + 1].targetNode;
            if (thisT < nextT)
            {
                nodesHitTotal += nextT - thisT;
            }
            else
            {
                nodesHitTotal += length - thisT;
                nodesHitTotal += nextT;
            }
        }
        shortestLength = nodesHitTotal.ToString();


        float timePerOrbit = levelDesigner.beatsPerBar * secondsPerBeat;
        float timePerNodeHitAvg = timePerOrbit / levelDesigner.nodeSettings.Length;

        float timeLevelTotal = timePerNodeHitAvg * nodesHitTotal;

        float barsPerNodeAvg = 1f / levelDesigner.nodeSettings.Length;
        float barsLevelTotal = barsPerNodeAvg * nodesHitTotal;

        switch (dataTypeToDisplay)
        {
            case DataType.Seconds:
                shortestLength = timeLevelTotal.ToString();
                break;
            case DataType.NodeHits:
                shortestLength = nodesHitTotal.ToString();
                break;
            case DataType.Bars:
                shortestLength = barsLevelTotal.ToString();
                break;
        }
    }
}
