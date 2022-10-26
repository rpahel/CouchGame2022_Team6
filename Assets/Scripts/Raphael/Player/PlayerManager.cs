using Data;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerManager : MonoBehaviour
{
    #region Variables
    //============================
    private PLAYER_STATE playerState;
    public PLAYER_STATE PlayerState { get; set; }

    //============================
    private Vector2 sensDuRegard; // � ne pas confondre avec aimDirection
    public Vector2 SensDuRegard { get; set; }

    //============================
    private Vector2 aimDirection;
    public Vector2 AimDirection { get => aimDirection; set => aimDirection = value; }

    #endregion

    #region Unity_Functions
    private void Update()
    {
        #if UNITY_EDITOR
            Debug.DrawRay(transform.position - Vector3.forward, aimDirection * 5f, Color.cyan, Time.deltaTime);
        #endif
    }
    #endregion

    #region Custom_Functions
    /// <summary>
    /// Essaie de r�cup�rer, dans le Player gameObject, le Component donn�. Arr�te le mode Play et retourne une erreur lorsque le Component n'est pas trouv�.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="component">Le component � r�cup�rer.</param>
    /// <exception cref="Exception">Le component n'a pas �t� trouv�</exception>
    public void TryGetPlayerComponent<T>(out T component)
    {
        //Debug.Log($"Trying to get {typeof(T)} from the player game object...");

        if (!TryGetComponent<T>(out component))
        {
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #endif
            throw new Exception($"No {typeof(T)} component found in Player game object !");
        }
    }
    #endregion
}
