using System;
using System.Collections;
using System.Collections.Generic;
using Controller;
using Model.Factions;
using Model.Universe;
using UnityEngine;


[System.Serializable]
public class GameData
{
    public GameManager.GameState gameState;
    public string systemInFocusId;
    public Vector3 playerGalaxyPos;
    public Vector3 playerSystemPos;

    public Character playerCharacter;
    public Party playerParty;
    public GalaxySize galaxySize;
    public Galaxy galaxy;
    
    public struct RotationData
    {
        public int rotationsPassed;
        public float timeSinceLastRotation;
    }

    public RotationData? rotationData;
    
    public GameData()
    {
        gameState = GameManager.GameState.Galaxy;
        playerGalaxyPos = Vector3.zero;
        playerSystemPos = new Vector3(0,0,-25);
        playerCharacter = null;
        systemInFocusId = null;
        playerParty = null;
        galaxySize = GalaxySize.Small;
        galaxy = null;
        rotationData = null;
    }
}
