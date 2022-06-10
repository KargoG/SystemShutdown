using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// This component is a general class that handles the corruption of a file.
/// Specific file types like folders or different documents have to inherit
/// from this component to define their specific behaviour.
/// Every gameobject that has the file component needs this component (Files, NOT OpenedFiles)
/// </summary>
public abstract class Corruption : MonoBehaviour, Saveable
{
    // The name this corruption progress is saved under
    // HAS TO BE UNIQUE FOR EVERY TIME THIS COMPONENT IS USED
    [SerializeField] protected string _saveName = "";

    public Slider CorruptionBarVisuals { get; set; } = null;

    // The file this corruption is assigned to
    protected File _file = null;

    // The progress of this files corruption
    private float _corruptionLevel = 0;
    public float CorruptionLevel
    {
        get
        {
            return _corruptionLevel;
        }
        protected set { _corruptionLevel = value; }
    }

    protected virtual void Awake()
    {
        _file = GetComponent<File>();
        if (!_file)
            throw new UnityException("This game object " + gameObject + " doesn't have a file script!");
    }

    /// <summary>
    /// Override this method if you need to prepare before tracking the corruption in a file.
    /// </summary>
    public virtual void PrepareTracking() { }

    /// <summary>
    /// This method updates the current corruption level
    /// and afterwards updated the visuals representing
    /// the corruption.
    /// Override this method to define your custom corruption
    /// tracking.
    /// </summary>
    public virtual void UpdateCorruptionLevel()
    {
        CorruptionBarVisuals.value = _corruptionLevel;
    }

    /// <summary>
    /// Override this method to define how to save the corruption of this file.
    /// </summary>
    public abstract void Save();

    /// <summary>
    /// Override this method to define how to load the corruption of this file.
    /// </summary>
    public abstract void Load();

}
