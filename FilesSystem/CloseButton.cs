using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This script should be given to a files or folders close button.
/// </summary>
public class CloseButton : MonoBehaviour, Interactable
{
    [SerializeField] private OpenedFile _toClose = null;

    public void Interact()
    {
        _toClose.Close();
    }
}
