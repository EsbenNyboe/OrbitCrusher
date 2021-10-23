using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelMusicChecker : MonoBehaviour
{
    private void CheckLevelMusicOneLevel(LevelDesigner levelDesigner, LevelMusic levelMusic)
    {
        if (levelDesigner.useLevelMusicSystem)
        {
            for (int i = 0; i < levelMusic.part.Length; i++)
            {
                LevelMusic.Part part = levelMusic.part[i];
                for (int e = 0; e < part.sound.Length; e++)
                {
                    LevelMusic.Sound sound = part.sound[e];

                    bool duplicate = false;
                    for (int a = 0; a < soundsIndex; a++)
                    {
                        if (sound.audioObject == audioObjects[a])
                        {
                            duplicate = true;
                        }
                    }
                    if (!duplicate)
                    {
                        audioObjects[soundsIndex] = sound.audioObject;
                        sounds[soundsIndex] = sound.audioObject.soundMultiples[0].soundFile.name;
                        soundsIndex++;
                    }
                }
            }
        }
    }
    AudioObject[] audioObjects;
    public string[] sounds;
    int soundsIndex;
    public void CheckLevelMusic()
    {
        GameManager gameManager = FindObjectOfType<GameManager>();
        sounds = new string[1000];
        audioObjects = new AudioObject[1000];
        soundsIndex = 0;
        for (int i = 0; i < gameManager.levels.Length; i++)
        {
            CheckLevelMusicOneLevel(gameManager.levels[i].GetComponent<LevelDesigner>(), gameManager.levels[i].GetComponent<LevelMusic>());
        }
        string[] temp = new string[soundsIndex + 1];
        for (int i = 0; i < temp.Length; i++)
        {
            temp[i] = sounds[i];
        }
        sounds = temp;

        string printData = "";
        for (int i = 0; i < sounds.Length; i++)
        {
            printData += "\n" + sounds[i];
        }
        print(printData);


        //if (checkAllLevels)
        //{
        //    int stringIndex = 0;
        //    allLevelLengths = new string[2 * gameManager.levels.Length];
        //    for (int i = 0; i < allLevelLengths.Length / 2; i++)
        //    {
        //        LevelDesigner levelDesigner = gameManager.levels[i].GetComponent<LevelDesigner>();
        //        LongestLength(levelDesigner, secondsPerBeat);
        //        ShortestLength(levelDesigner, secondsPerBeat);
        //        allLevelLengths[stringIndex] = i + "  longest:" + longestLength;
        //        stringIndex++;
        //        allLevelLengths[stringIndex] = i + " shortest:" + shortestLength;
        //        stringIndex++;
        //    }
        //}
        //else
        //{
        //    LevelDesigner levelDesigner = gameManager.levels[levelToCheck].GetComponent<LevelDesigner>();
        //    LongestLength(levelDesigner, secondsPerBeat);
        //    ShortestLength(levelDesigner, secondsPerBeat);
        //}

        //for (int i = 0; i < allLevelLengths.Length; i++)
        //{
        //    allLevelLengthsInOneString4Ya = allLevelLengthsInOneString4Ya + " " + allLevelLengths[i];
        //}

        //shortestLengthHP = " ";
        //lvlNum = 0;
        //for (int i = 0; i < gameManager.levels.Length; i++)
        //{
        //    lvlNum++;
        //    LevelDesigner levelDesigner = gameManager.levels[i].GetComponent<LevelDesigner>();
        //    ShortestLengthHP(levelDesigner, secondsPerBeat);
        //}
    }

    public LevelMusic[] levelMusic;
    public void PrintAllLevelMusicData()
    {
        string allLevelMusicData = "";

        for (int levelIndex= 0; levelIndex < levelMusic.Length; levelIndex++)
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
    }
}
