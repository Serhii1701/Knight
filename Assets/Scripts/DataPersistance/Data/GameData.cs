using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    //public Vector3 playerPosition;
    //public Quaternion playerRotation;
    public long lastUpdated;

    public int enemiesKilled = 0;
    public int qualityLevel = 0;
    public int resolutionIndex = 0;
    public float brightness = 0.5f;
    public float audioVolumeValue = 0;
    public float audioVolumeEffectsValue = 0;

    public bool fullScreen = true;
    public bool isDefaultToggle = true;

    public SerializableDictionary<string, Vector3> playersPosition;
    public SerializableDictionary<string, Quaternion> playersRotation;
    public SerializableDictionary<string, int> playersHealth;
    public SerializableDictionary<string, bool> enemiesKilledDictionary;
    public AttributesData playerAttributesData;

    //the values defined in this constructor will be the default values
    // the game starts with when there's no data to load

    public GameData()
    {
        //this.playerPosition = Vector3.zero;
        //this.playerRotation = Quaternion.Euler(0, 0, 0);
        playersPosition = new SerializableDictionary<string, Vector3>();
        playersRotation = new SerializableDictionary<string, Quaternion>();
        playersHealth = new SerializableDictionary<string, int>();
        enemiesKilledDictionary = new SerializableDictionary<string, bool>();
        playerAttributesData = new AttributesData();
        qualityLevel = 0;
        resolutionIndex = 0;
        brightness = 0.5f;
        isDefaultToggle = true;
        fullScreen = false;
        audioVolumeValue = 0.5f;
        audioVolumeEffectsValue = 0.5f;
    }

    public int GetPercentageComplete()
    {
        // figure out how many enemies we've killed
        int totalKilled = 0;
        foreach(bool isKilled in enemiesKilledDictionary.Values)
        {
            if (isKilled)
            {
                totalKilled++;
            }
        }

        // ensure we don't divide by 0 when calculating the percentage
        int percentageCompleted = -1;
        if (enemiesKilledDictionary.Count != 0)
        {
            percentageCompleted = (totalKilled * 100 / enemiesKilledDictionary.Count);
        }

        CountEnemiesKilledNumber(totalKilled);
        return percentageCompleted;
    }

    private void CountEnemiesKilledNumber(int totalKilled)
    {
        enemiesKilled = totalKilled;
    }
}
