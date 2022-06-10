using Pathfinding;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

/// <summary>
/// This component should be given to any file in the game.
/// A file in this scenario is the Icon of a file like you
/// have on your own computer. This is the NOT yet OPENED file.
/// Every gameobject using this behaviour also needs a Corruption Component
/// </summary>
public class File : MonoBehaviour, Interactable
{
    // The file/level that is opened when the player interacts with this
    [SerializeField] private OpenedFile _levelToOpen = null;
    [SerializeField] private Vector3 _openedFilePosition = Vector3.zero;

    // Should the player moved to a defined position when this file is opened/ closed
    [SerializeField] private bool _movePlayerOnOpen = false;
    [SerializeField] private bool _movePlayerOnClose = false;
    [SerializeField] private Transform _positionToMovePlayerToOnClose = null;

    // The story to play once this file is fully corrupted
    [SerializeField] private int _onCorruptedNextStory = -1; 

    public Corruption Corruption { get; private set; } = null;
    public OpenedFile OpenedFile { get; private set; } = null;

    // The opened file this still closed file is stored in (think of folder structures) (The desktop also counts)
    private OpenedFile _fileThisIsIn = null;

    private Vector3 _playerPositionBefore = Vector3.zero;

    private GameObject _player = null;

    private SortingGroup _layer = null;

    private void Awake()
    {
        _layer = GetComponent<SortingGroup>();
        Corruption = GetComponent<Corruption>();
    }

    private void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
        _fileThisIsIn = transform.parent.GetComponent<OpenedFile>();
        if (!_fileThisIsIn)
            throw new UnityException("Every file needs to be in an opened file!");
    }

    /// <summary>
    /// This method opens this file and shows its contents.
    /// It also deactivates possible interactions in the currently open file.
    /// </summary>
    private void OpenFile()
    {
        // We instantiate the openedFile this File is assigned to
        OpenedFile = Instantiate(_levelToOpen, _openedFilePosition, Quaternion.identity);
        OpenedFile.ClosedFile = this;
        // We need to set the sorting layer to be able to open files over other files lower in the hierarchy
        OpenedFile.SetLayer(_layer.sortingOrder);

        _playerPositionBefore = _player.transform.position;

        if(_movePlayerOnOpen)
            _player.transform.position = OpenedFile.PlayerPositionOnOpen;

        // We deactivate behaviour on the openedFile this file is in to prevent opening folders in it or something similar
        _fileThisIsIn.SetFileActive(false);

        // We load the corruption of the file we are opening and are preparing the visualization
        Corruption.Load();
        Corruption.PrepareTracking();
        Corruption.CorruptionBarVisuals = OpenedFile.CorruptionBar;

        // We update the pathfinding to accomodate for the level changes
        AstarPath.active.Scan(AstarPath.active.data.gridGraph);
    }

    /// <summary>
    /// This method is called when the openedFile assigned to this file is closed.
    /// It takes care of saving the corruption and progressing the story.
    /// It also re-enables the openedFile this file is in, making it interactable again.
    /// </summary>
    public void CloseFile()
    {
        // We save the new corruption level
        Corruption.UpdateCorruptionLevel();
        Corruption.Save();

        if (_movePlayerOnClose)
            _player.transform.position = _positionToMovePlayerToOnClose.position;

        // We re-enable the openedFile this file is stored in
        _fileThisIsIn.SetFileActive(true);

        // The openedFile assigned to this File is getting destroyed
        Destroy(OpenedFile.gameObject);
        // We update the pathfinding to accomodate for the level changes
        AstarPath.active.Scan(AstarPath.active.data.gridGraph);

        // if there is story progression related to this file and it was corrupted enough before closing, we progress the story
        if (_onCorruptedNextStory != -1 && Corruption.CorruptionLevel >= 0.95f)
        {
            StoryManager._instance.SetStoryIndex(_onCorruptedNextStory);
        }

    }

    /// <summary>
    /// This method sets the sorting layer of this file
    /// </summary>
    /// <param name="folderLayer">The layer of the openedFile this file is in</param>
    public void SetLayer(int folderLayer)
    {
        _layer.sortingOrder = folderLayer + 10;
    }

    public void Interact()
    {
        OpenFile();
    }
}
