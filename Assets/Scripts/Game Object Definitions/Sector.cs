﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct IntRect
{
    public int x, y, w, h;
    public IntRect(int X, int Y, int W, int H)
    {
        x = X;
        y = Y;
        w = W;
        h = H;
    }

    public bool contains(Vector2 position)
    {
        return position.x > x && position.x < x + w && position.y > y && position.y < y + h;
    }
}

[CreateAssetMenu(fileName = "Sector", menuName = "ShellCore/Sector", order = 7)]
public class Sector : ScriptableObject
{
    [System.Serializable]
    public struct LevelEntity
    {
        public string name;
        public string ID;
        public int faction;
        public string assetID;
        public string dialogueID;
        public string vendingID;
        public string pathID;
        public Vector2 position;
    }

    public enum SectorType
    {
        Neutral,
        Haven,
        BattleZone,
        DangerZone
    }

    public string sectorName;
    public IntRect bounds;
    public SectorType type;
    public Color backgroundColor;
    public LevelEntity[] entities;
    public LandPlatform platform;
    public string[] targets;
}
