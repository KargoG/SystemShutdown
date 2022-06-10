using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This component handles the corruption of folders
/// containing other files. To find out more about the corruption
/// component in general see <see cref="Corruption"/>
/// </summary>
public class FolderCorruption : Corruption
{
    // A list of all Corruption components of files in the folder this Corruption is assigned to
    private List<Corruption> _corruptionInFiles = null;

    private Material _material = null;

    protected override void Awake()
    {
        base.Awake();
        _material = GetComponentInChildren<Renderer>().material;
    }

    /// <summary>
    /// This method Updates this folders corruption level by
    /// checking the current Corruption progress in all files
    /// it contains.
    /// The average corruption of all contained files is this
    /// folders corruption.
    /// </summary>
    public override void UpdateCorruptionLevel()
    {
        // If this file is not open there is not point in updating its corruption
        if (!_file.OpenedFile)
            return;

        // we get all Corruption components of files in the current folder
        _corruptionInFiles = new List<Corruption>();
        _file.OpenedFile.GetComponentsInChildren<Corruption>(_corruptionInFiles);

        if (_corruptionInFiles.Count <= 0)
            return;

        float totalCorruption = 0;

        // We calculate and set the average corruption
        foreach (Corruption fileCorruption in _corruptionInFiles)
        {
            totalCorruption += fileCorruption.CorruptionLevel;
        }
        CorruptionLevel = totalCorruption / _corruptionInFiles.Count;

        // We display the corruption by calling the base function
        base.UpdateCorruptionLevel();
    }

    /// <summary>
    /// This method saves the current corruption level as a plain number
    /// </summary>
    public override void Save()
    {
        PlayerPrefs.SetFloat(_saveName, CorruptionLevel);
    }


    /// <summary>
    /// This method loads the corruption level of this folder.
    /// First it loads the saved value and uses this for displaying
    /// its corruption.
    /// If the file is opened the method also tells all files in
    /// this folder to load their corruption
    /// </summary>
    public override void Load()
    {
        // We load the saved corruptiom
        CorruptionLevel = PlayerPrefs.GetFloat(_saveName, 0);

        // If the folder is corrupted enough its icon starts glitching
        if (CorruptionLevel > 0.8f)
            _material.SetFloat("_ShowGlitching", 1);

        if (!_file.OpenedFile)
            return;

        // If the folder is opened we get all Corruption components of the contained files
        _corruptionInFiles = new List<Corruption>();
        _file.OpenedFile.GetComponentsInChildren<Corruption>(_corruptionInFiles);

        // We load the corruption of all contained files
        foreach (Corruption corruption in _corruptionInFiles)
        {
            corruption.Load();
        }
    }
}
