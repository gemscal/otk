using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class PlayerListItem : MonoBehaviourPunCallbacks
{
    [SerializeField] TMP_Text playerName;
    [SerializeField] TMP_Text playerClass;
    [SerializeField] Color roomPlayerColor;
    [SerializeField] Image roomPlayerBG;
    [SerializeField] GameObject roomMasterIcon;
    [SerializeField] GameObject roomReadyIcon;
    [SerializeField] GameObject characterClass;
    Player player;

    /// <summary> Setup player prefab and other player settings </summary>
    public void SetUp(Player _player) {
        player = _player;
        playerName.text = player.NickName;

        if (_player == PhotonNetwork.LocalPlayer) {
            ApplyLocalChanges();
            PlayerCustomProperties();
            UpdateOtherPlayerProperties();
        }
    }

    // called when local/client player left the room
    public override void OnLeftRoom() {
        Destroy(gameObject);
    }

    // called when remote player left the room
    public override void OnPlayerLeftRoom(Player otherPlayer) {
        if (player == otherPlayer) {
            Destroy(gameObject);
        }

        // recalculate the players on ready mode
        Player[] rp = PhotonNetwork.PlayerList;
        int rc = 1; // ready count
        for (int i = 0; i < rp.Length; i++) {
            if ((string)rp[i].CustomProperties[PlayerProperties.PR] == "True" && rp[i] != PhotonNetwork.MasterClient) {
                rc = rc + 1;
            }
        }

        Launcher.Instance.DisplayReadyCount(rc);
    }

    // called everytime players custom properties changed
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps) {
        if (player == targetPlayer) {
            // default & get first players properties
            if (changedProps.Count == 4) {
                playerClass.text = (string)player.CustomProperties[PlayerProperties.PC];
                // check if the first players is on ready mode
                if ((string)player.CustomProperties[PlayerProperties.PR] == "True") {
                    roomReadyIcon.SetActive(true);
                }
                // check if the player is the master client
                if ((string)player.CustomProperties[PlayerProperties.PM] == "Yes") {
                    roomMasterIcon.SetActive(true);
                }
            }

            // specific change when master client change
            if (changedProps.Count == 2) {
                // check if the first players is on ready mode
                if ((string)player.CustomProperties[PlayerProperties.PR] == "False") {
                    roomReadyIcon.SetActive(false);
                }
                // check if the player is the master client
                if ((string)player.CustomProperties[PlayerProperties.PM] == "Yes") {
                    roomMasterIcon.SetActive(true);
                }
            }

            // specific change of properties when player activate/deactivate ready button and char class
            if (changedProps.Count == 1) {
                // updating player character class property
                if (changedProps.ContainsKey(PlayerProperties.PC)) {
                    playerClass.text = (string)player.CustomProperties[PlayerProperties.PC];
                }
                // updating player ready property
                if (changedProps.ContainsKey(PlayerProperties.PR)) {
                    if ((string)player.CustomProperties[PlayerProperties.PR] == "True") {
                        roomReadyIcon.SetActive(true);
                        Launcher.Instance.RemotePlayerReadyCount(true);
                    } else {
                        roomReadyIcon.SetActive(false);
                        Launcher.Instance.RemotePlayerReadyCount(false);
                    }
                }
            }
        }
    }

    /// <summary> Apply local changes </summary>
    public void ApplyLocalChanges() {
        roomPlayerBG.color = roomPlayerColor;
        characterClass.SetActive(true);
    }

    /// <summary> Update newly joined local player about the other 
    /// players custom properties in the room </summary>
    public void UpdateOtherPlayerProperties() {
        Player[] existingP = PhotonNetwork.PlayerList;
        for (int i = 0; i < existingP.Length; i++) {
            if (existingP[i] != PhotonNetwork.LocalPlayer) {
                Hashtable updatePlayersProp = new Hashtable();
                string isReady = (string)existingP[i].CustomProperties[PlayerProperties.PR];
                string isMaster = (string)existingP[i].CustomProperties[PlayerProperties.PM];
                string charClass = (string)existingP[i].CustomProperties[PlayerProperties.PC];
                string charTeam = (string)existingP[i].CustomProperties[PlayerProperties.PT];
                
                if (isReady == "True") {
                    Launcher.Instance.RemotePlayerReadyCount(true);
                }

                updatePlayersProp[PlayerProperties.PR] = isReady;
                updatePlayersProp[PlayerProperties.PC] = charClass;
                updatePlayersProp[PlayerProperties.PT] = charTeam;
                updatePlayersProp[PlayerProperties.PM] = isMaster;
                existingP[i].SetCustomProperties(updatePlayersProp);
            } 
        }
    }

    /// <summary> Set player class </summary>
    public void SetPlayerClass(string playerClass) {
        Hashtable playerClassProp = new Hashtable();
        playerClassProp[PlayerProperties.PC] = playerClass;
        player.SetCustomProperties(playerClassProp);
    }

    /// <summary> Set player default custom properties </summary>
    public void PlayerCustomProperties() {
        Hashtable defaultProp = new Hashtable();
        defaultProp[PlayerProperties.PC] = "Warrior";
        defaultProp[PlayerProperties.PR] = "False";
        defaultProp[PlayerProperties.PT] = "FFA";
        if (player == PhotonNetwork.MasterClient) {
            defaultProp[PlayerProperties.PM] = "Yes";
        } else {
            defaultProp[PlayerProperties.PM] = "No";
        }
        player.SetCustomProperties(defaultProp);
    }

    /// <summary> Get custom player properties </summary>
    public void GetPlayerCustomProp() {

    }
}
