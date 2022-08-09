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
        }
    }

    // called when remote player left the room
    public override void OnPlayerLeftRoom(Player otherPlayer) {
        if (player == otherPlayer) {
            Destroy(gameObject);
        }
    }

    // called when local/client player left the room
    public override void OnLeftRoom() {
        Destroy(gameObject);
    }

    // called everytime players custom properties changed
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps) {
        Debug.Log(changedProps.Count);
        if (player == targetPlayer) {

            // default properties
            if (changedProps.Count == 3) {
                playerClass.text = (string)player.CustomProperties[PlayerProperties.PlayerClass];
            }

            // specific change of properties
            if (changedProps.Count == 1) {
                if (changedProps.ContainsKey(PlayerProperties.PlayerClass)) {
                    playerClass.text = (string)player.CustomProperties[PlayerProperties.PlayerClass];
                }

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

            Debug.Log(PhotonNetwork.LocalPlayer.CustomProperties);
        }
    }

    /// <summary> Apply local changes </summary>
    public void ApplyLocalChanges() {
        roomPlayerBG.color = roomPlayerColor;
        characterClass.SetActive(true);
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
