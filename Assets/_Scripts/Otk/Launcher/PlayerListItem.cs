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
            if ((string)rp[i].CustomProperties[PlayerProperties.PlayerReady] == "True") {
                rc = rc + 1;
            }
        }
        Launcher.Instance.RecalculateReady(rc);
    }

    // called everytime players custom properties changed
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps) {
        if (player == targetPlayer) {
            // default properties
            if (changedProps.Count == 3) {
                playerClass.text = (string)player.CustomProperties[PlayerProperties.PlayerClass];
                if ((string)player.CustomProperties[PlayerProperties.PlayerReady] == "True") {
                    roomReadyIcon.SetActive(true);
                }
            }
            // specific change of properties
            if (changedProps.Count == 1) {
                // updating player character class property
                if (changedProps.ContainsKey(PlayerProperties.PlayerClass)) {
                    playerClass.text = (string)player.CustomProperties[PlayerProperties.PlayerClass];
                }
                // updating player ready property
                if (changedProps.ContainsKey(PlayerProperties.PlayerReady)) {
                    if ((string)player.CustomProperties[PlayerProperties.PlayerReady] == "True") {
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
        Player[] firstPlayers = PhotonNetwork.PlayerList;
        for (int i = 0; i < firstPlayers.Length; i++) {
            if (firstPlayers[i] != PhotonNetwork.LocalPlayer) {
                Hashtable updatePlayersProp = new Hashtable();
                string isReady = (string)firstPlayers[i].CustomProperties[PlayerProperties.PlayerReady];
                string charClass = (string)firstPlayers[i].CustomProperties[PlayerProperties.PlayerClass];
                string charTeam = (string)firstPlayers[i].CustomProperties[PlayerProperties.PlayerTeam];
                
                if (isReady == "True") {
                    Launcher.Instance.RemotePlayerReadyCount(true);
                }

                updatePlayersProp[PlayerProperties.PlayerReady] = isReady;
                updatePlayersProp[PlayerProperties.PlayerClass] = charClass;
                updatePlayersProp[PlayerProperties.PlayerTeam] = charTeam;
                firstPlayers[i].SetCustomProperties(updatePlayersProp);
            } 
        }
    }

    /// <summary> Set player class </summary>
    public void SetPlayerClass(string playerClass) {
        Hashtable playerClassProp = new Hashtable();
        playerClassProp[PlayerProperties.PlayerClass] = playerClass;
        player.SetCustomProperties(playerClassProp);
    }

    /// <summary> Set player default custom properties </summary>
    public void PlayerCustomProperties() {
        Hashtable defaultCustomProp = new Hashtable();
        defaultCustomProp[PlayerProperties.PlayerClass] = "Warrior";
        defaultCustomProp[PlayerProperties.PlayerReady] = "False";
        defaultCustomProp[PlayerProperties.PlayerTeam] = "FFA";
        player.SetCustomProperties(defaultCustomProp);
    }

    // TODO: Let players know who is the master client via icon
}
