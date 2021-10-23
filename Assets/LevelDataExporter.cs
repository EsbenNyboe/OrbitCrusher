using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelDataExporter : MonoBehaviour
{
    public LevelMusic[] levelMusic;
    public LevelDesigner tutorialMusic;

    public void ExportAllLevelData()
    {
        string allLevelMusicData = "";

        // tutorialMusic tbc..........

        for (int levelIndex = 0; levelIndex < levelMusic.Length; levelIndex++)
        {
            for (int partIndex = 0; partIndex < levelMusic[levelIndex].part.Length; partIndex++)
            {
                for (int soundIndex = 0; soundIndex < levelMusic[levelIndex].part[partIndex].sound.Length; soundIndex++)
                {
                    string soundName = levelMusic[levelIndex].part[partIndex].sound[soundIndex].name;
                    int objective = levelMusic[levelIndex].part[partIndex].objective;
                    allLevelMusicData += "\n" + "l:" + levelIndex + ".p:" + partIndex + ".o:" + objective + ".s:" + soundIndex + ":" + soundName;
                }
            }
        }
        print(allLevelMusicData);
        CheckLevelLength();
    }


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

        allLevelLengths = new string[2 * gameManager.levels.Length];
        string longestLengths = "";
        string shortestLengths = "";
        for (int i = 0; i < allLevelLengths.Length / 2; i++)
        {
            LevelDesigner levelDesigner = gameManager.levels[i].GetComponent<LevelDesigner>();
            longestLengths += LongestLength(levelDesigner, secondsPerBeat, i);
            shortestLengths += ShortestLength(levelDesigner, secondsPerBeat, i);
        }
        print(longestLengths);
        print(shortestLengths);
    }

    private string LongestLength(LevelDesigner levelDesigner, float secondsPerBeat, int lvl)
    {
        int nodesHitTotal = 0;
        nodesHitTotal += levelDesigner.levelObjectives[0].targetNode;

        int energy = FindObjectOfType<GameManager>().startEnergy;

        repeatsPerObjective = new int[levelDesigner.levelObjectives.Length];

        int stageCompleted = 0;
        string stageCompletedTime = "";
        for (int i = 0; i < levelDesigner.levelObjectives.Length - 1; i++)
        {
            stageCompleted++;
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
            float timePerOrbit = levelDesigner.beatsPerBar * secondsPerBeat;
            float timePerNodeHitAvg = timePerOrbit / levelDesigner.nodeSettings.Length;
            float timeLevelTotal = timePerNodeHitAvg * nodesHitTotal;
            longestLength = timeLevelTotal.ToString();

            stageCompletedTime += lvl + ".index:" + stageCompleted + ".time:" + longestLength + "\n";
        }
        return stageCompletedTime;
    }


    private string ShortestLength(LevelDesigner levelDesigner, float secondsPerBeat, int lvl)
    {

        int nodesHitTotal = 0;
        nodesHitTotal += levelDesigner.levelObjectives[0].targetNode;

        int stageCompleted = 0;
        string stageCompletedTime = "";
        for (int i = 0; i < levelDesigner.levelObjectives.Length - 1; i++)
        {
            stageCompleted++;

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

            float timePerOrbit = levelDesigner.beatsPerBar * secondsPerBeat;
            float timePerNodeHitAvg = timePerOrbit / levelDesigner.nodeSettings.Length;
            float timeLevelTotal = timePerNodeHitAvg * nodesHitTotal;
            shortestLength = timeLevelTotal.ToString();

            stageCompletedTime += lvl + ".index:" + stageCompleted + ".time:" + shortestLength + "\n";
        }
        return stageCompletedTime;
    }
}
