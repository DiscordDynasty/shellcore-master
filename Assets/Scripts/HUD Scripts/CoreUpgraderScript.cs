﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CoreUpgraderScript : GUIWindowScripts
{
    public PlayerCore player;
    public static CoreUpgraderScript instance;
    public static int[] maxAbilityCap = new int[] {15, 8, 15, 15};
    private static int[] minAbilityCap = new int[] {10, 3, 10, 10};
    public GameObject optionPrefab;
    public RectTransform reputationBar;
    public Text repText;
    public Transform optionHolder;

    public void initialize() {
        instance = this;
        player.SetIsInteracting(true);

        Activate();
        gameObject.SetActive(true);
        drawScreen();
        ShardCountScript.StickySlideIn(player.shards);

    }

    public override void CloseUI() {
        ShardCountScript.StickySlideOut();
        player.SetIsInteracting(false);
        gameObject.SetActive(false);
        ResourceManager.PlayClipByID("clip_back");
    }	
    
    public static void DrawScreen() {
        instance.drawScreen();
    }

    private void drawScreen() {
        // reputation display
        var repReq = GetReputationRequirement(player.blueprint.coreShellSpriteID);
        if(repReq > 0) {
            reputationBar.sizeDelta = new Vector2(Mathf.Min(player.reputation * 800 / repReq, 800), 30);
            repText.text = "Reputation: " + player.reputation + "/" + repReq;
        } else {
            reputationBar.sizeDelta = new Vector2(800, 30);
            repText.text = "Core fully upgraded!";
        }

        for(int i = 0; i < optionHolder.childCount; i++) {
            Destroy(optionHolder.GetChild(i).gameObject);
        }
        // create option icons
        var currentID = player.blueprint.coreShellSpriteID;
        var nextIDs = GetNextUpgrades(currentID);
        if(nextIDs != null) {
            var offset = -500 * (nextIDs.Length - 1);
            for(int i = 0; i < nextIDs.Length; i++) {
                var option = Instantiate(optionPrefab, optionHolder, false).GetComponent<RectTransform>();
                option.anchoredPosition = new Vector2((offset - offset * 2 * i) / nextIDs.Length, 20);
                var script = option.GetComponent<CUOptionScript>();
                script.coreID = nextIDs[i];
                script.player = player;
                script.repCost = repReq;
            }
        }

        CUAbilityCapDisplay.Initialize(player.abilityCaps, currentID);
    }

    /// prevent dragging the window if the mouse is on the grid
	public override void OnPointerDown(PointerEventData eventData) {
		base.OnPointerDown(eventData);
	}

    public static void IncrementAbilityCap(int type) {
        instance.incrementAbilityCap(type);
    } 

    private void incrementAbilityCap(int type) {
        if(player.abilityCaps[type] < maxAbilityCap[type]) {
            player.shards -= GetUpgradeCost(type);
            player.abilityCaps[type]++;
            ShardCountScript.UpdateNumber(player.shards);
        }

    }

    public static int GetUpgradeCost(int type) {
        return 5 * Mathf.RoundToInt(Mathf.Pow(2, instance.player.abilityCaps[type] - minAbilityCap[type]));
    }

    public static int GetShards() {
        return instance.player.shards;
    }

    public static string[] GetNextUpgrades(string coreName) {
        switch(coreName) {
            case "core1_shell":
                return new string[] {"core2_shell"};
            case "core2_shell":
                return new string[] {"core3skills_shell", "core3weapons_shell"};
            case "core3skills_shell":
                return new string[] {"core4commando_shell", "core4elite_shell"};
            case "core3weapons_shell":
                return new string[] {"core4captain_shell", "core4admiral_shell"};
            default:
                return null;
        }
    }

    // reputation to upgrade to any core of the next tier, based on the tier of the core passed
    public static int GetReputationRequirement(string coreName) {
        switch(coreName) {
            case "core1_shell":
                return 100;
            case "core2_shell":
                return 500;
            case "core3skills_shell":
            case "core3weapons_shell":
                return 2500;
            default:
                return 0;
        }
    }

    public static int GetPartTierLimit(string coreName) {
        switch(coreName) {
            case "core1_shell":
                return 0;
            case "core2_shell":
                return 1;
            case "core3skills_shell":
            case "core3weapons_shell":
            case "core4commando_shell":
            case "core4elite_shell":
            case "core4captain_shell":
            case "core4admiral_shell":
                return 2;
            default:
                return 0;
        }
    }

    public static string GetDescription(string coreName) {
        switch(coreName) {
            case "core1_shell":
                return "Size-S Core\nThis basic core can handle only small-size parts, but is the building block for all further upgrades.";
            case "core2_shell":
                return "Size-M Core\nThis larger core can handle medium-size parts.";
            case "core3skills_shell":
                return "Size-L Skills Core\nThe sturdy build of this core means\nit can handle large-size parts\nand 3 extra skills as well.";
            case "core3weapons_shell":
                return "Size-L Weapons Core\nHigh aerodynamic capacity allows this core\nto handle large-size parts as well as equip\n3 extra weapons.";
            case "core4commando_shell":
                return "Size-XL Commando Core\nConstructing a spawning hex around your core\nallows for an additional 2 spawns.";
            case "core4elite_shell":
                return "Size-XL Elite Core\nAdding afterburners and structural padding\nto your core allows for an additional 3 passives.";
            case "core4captain_shell":
                return "Size-XL Captain Core\nConstructing a spawning hex around\nyour core allows for an additional 2 spawns.";
            case "core4admiral_shell":
                return "Size-XL Admiral Core\nAdding afterburners and structural padding\nto your core allows for an additional 3 passives.";
            default:
                return "No description.";

        }
    }

    public static int[] GetExtraAbilities(string coreName) {
        switch(coreName) {
            case "core3skills_shell":
                return new int[] {3, 0, 0, 0};
            case "core3weapons_shell":
                return new int[] {0, 0, 3, 0};
            case "core4commando_shell":
                return new int[] {3, 2, 0, 0};
            case "core4elite_shell":
                return new int[] {3, 0, 0, 3};
            case "core4captain_shell":
                return new int[] {0, 2, 3, 0};
            case "core4admiral_shell":
                return new int[] {0, 0, 3, 3};
            default:
                return new int[] {0, 0, 0, 0};
        }
    }
}
