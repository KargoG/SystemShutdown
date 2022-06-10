using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This component handles the corruption of word file
/// containing words that can be corrupted.
/// To find out more about the corruption component in
/// general see <see cref="Corruption"/>
/// </summary>
public class WordCorruption : Corruption
{
    // All words that can be corrupted in the document
    private List<CorruptableHealth> _words = null;
    private Material _material = null;

    protected override void Awake()
    {
        base.Awake();
        _material = GetComponentInChildren<Renderer>().material;
    }

    /// <summary>
    /// This method prepares tracking the corruption of the file.
    /// This method gets called when the file this corruption is
    /// assigned to is opened.
    /// </summary>
    public override void PrepareTracking()
    {
        _words = new List<CorruptableHealth>();

        _file.OpenedFile.GetComponentsInChildren<CorruptableHealth>(_words);

        foreach (CorruptableHealth word in _words)
        {
            word.AddOnDeathListener(UpdateCorruptionLevel);
        }
    }

    /// <summary>
    /// This method Updates this word files corruption level by
    /// checking the current Corruption progress of all words this
    /// file contains.
    /// The average corruption of all contained words is this
    /// word files corruption.
    /// </summary>
    public override void UpdateCorruptionLevel()
    {
        // we get all words in the document
        _words = new List<CorruptableHealth>();

        _file.OpenedFile.GetComponentsInChildren<CorruptableHealth>(_words);

        // We calculate and set the average corruption based on the words health
        float accumulatedCorruption = 0;
        foreach (CorruptableHealth word in _words)
        {
            accumulatedCorruption += 1 - (float)word.GetHealth() / word.GetMaxHealth();
        }

        CorruptionLevel = (float)accumulatedCorruption / _words.Count;

        // If the file is corrupted enough its icon starts glitching
        if (CorruptionLevel > 0.8f)
            _material.SetFloat("_ShowGlitching", 1);

        base.UpdateCorruptionLevel();
    }

    /// <summary>
    /// This method saves the corruption level of this file.
    /// First it saves its own corruption value.
    /// Then it goes through all words in the document and saves
    /// how much damage it took.
    /// </summary>
    public override void Save()
    {
        PlayerPrefs.SetFloat(_saveName, CorruptionLevel);

        _words = new List<CorruptableHealth>();

        _file.OpenedFile.GetComponentsInChildren<CorruptableHealth>(_words);

        for (int i = 0; i < _words.Count; i++)
        {
            PlayerPrefs.SetInt(_saveName + i, _words[i].GetMaxHealth() - _words[i].GetHealth());
        }
    }


    /// <summary>
    /// This method loads the corruption level of this file.
    /// First it loads the saved value and uses this for displaying
    /// its corruption.
    /// Then it loads the damage every single word took and reapplies it.
    /// </summary>
    public override void Load()
    {
        CorruptionLevel = PlayerPrefs.GetFloat(_saveName, 0);

        if (CorruptionLevel > 0.8f)
            _material.SetFloat("_ShowGlitching", 1);

        if (!_file.OpenedFile)
            return;

        _words = new List<CorruptableHealth>();

        _file.OpenedFile.GetComponentsInChildren<CorruptableHealth>(_words);

        for (int i = 0; i < _words.Count; i++)
        {
            _words[i].Damage(PlayerPrefs.GetInt(_saveName + i, 0));
        }
    }
}
