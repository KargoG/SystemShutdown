using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

/// <summary>
/// This component should be given to a prefab representing an opened file.
/// That prefab will then be instatiated by a File when opened.
/// An opened file only becomes relevant when assigned to a File <see cref="File"/> that opens it.
/// </summary>
public class OpenedFile : MonoBehaviour
{
    // The position to move the player to when this file is opened
    [SerializeField] private Transform _playerPositionOnOpen = null;
    public Vector3 PlayerPositionOnOpen { get { return _playerPositionOnOpen.position; } }

    // A visual representation of the progress of this files corruption
    [SerializeField] private Slider _corruptionBar = null;
    public Slider CorruptionBar { get { return _corruptionBar; } }

    // The File this opened file is assigned to
    public File ClosedFile { get; set; }

    private SortingGroup _layer = null;

    // All files contained in this opened file (only relevant when file is a folder)
    private List<File> _filesInside = null;
    private List<Interactable> _interactablesInside = null;

    // The spawn system spawning enemies
    private SpawnSystem spawnSystem = null;

    private void Awake()
    {
        _layer = GetComponent<SortingGroup>();
        spawnSystem = GetComponent<SpawnSystem>();
        _filesInside = new List<File>();
        GetComponentsInChildren(_filesInside);
        _interactablesInside = new List<Interactable>();
        GetComponentsInChildren(_interactablesInside);
    }

    private void Start()
    {
        if(ClosedFile == null) // if this openedFile has no Closed file assigned to it, we are on the desktop
        {
            Load();
        }
    }

    /// <summary>
    /// This method loads the data for all Saveables contained in this folder
    /// </summary>
    private void Load()
    {
        Saveable[] objectsToLoad = GetComponentsInChildren<Saveable>();

        foreach (Saveable toLoad in objectsToLoad)
        {
            toLoad.Load();
        }
    }

    /// <summary>
    /// This method closes this openedFile, switching to the one below it in the folder hierarchy
    /// </summary>
    public void Close()
    {
        // we remove all enemies and hazards in this opened file
        spawnSystem.RemoveEnemies();
        RemoveBullets();

        // We close the file
        ClosedFile.CloseFile();
    }

    private void RemoveBullets()
    {
        Bullet[] bullets = FindObjectsOfType<Bullet>();
        foreach (Bullet bullet in bullets)
        {
            Destroy(bullet.gameObject);
        }
    }

    /// <summary>
    /// This method sets the sortingOrder of this openedFile
    /// as well as those of all files contained in this openedFile
    /// </summary>
    /// <param name="lastLayer">The layer of this openedFiles file</param>
    public void SetLayer(int lastLayer)
    {
        _layer.sortingOrder = lastLayer + 10;

        File[] files = GetComponentsInChildren<File>();
        foreach (File file in files)
        {
            file.SetLayer(_layer.sortingOrder);
        }
    }

    /// <summary>
    /// This method sets the activity state of all files this
    /// openedFile contains, as well as this openedFile itself.
    /// </summary>
    /// <param name="active">The activity state to set.</param>
    public void SetFileActive(bool active)
    {
        foreach (File file in _filesInside)
        {
            file.gameObject.SetActive(active);
        }
        foreach (Interactable interactable in _interactablesInside)
        {
            (interactable as MonoBehaviour).gameObject.SetActive(active);
        }
        gameObject.SetActive(active);
    }
}
