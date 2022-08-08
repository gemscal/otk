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
    [SerializeField] GameObject characterClass;
    Hashtable playerCustomProp = new Hashtable();
    Player player;

    public void SetUp(Player _player) {
        player = _player;
        playerName.text = player.NickName;

        if (_player == PhotonNetwork.LocalPlayer) {
            ApplyLocalChanges();
            PlayerCustomProperties();
        }
    }

    public override void OnPlayerLeftRoom(Player otherPlayer) {
        if (player == otherPlayer) {
            Destroy(gameObject);
        }
    }

    public override void OnLeftRoom() {
        Destroy(gameObject);
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps) {
        if (player == targetPlayer) {
            playerClass.text = (string)player.CustomProperties[PlayerProperties.PlayerClass];
            Debug.Log(player.CustomProperties[PlayerProperties.PlayerReady]);
        }
    }

    /// <summary> Apply local changes </summary>
    public void ApplyLocalChanges() {
        roomPlayerBG.color = roomPlayerColor;
        characterClass.SetActive(true);
    }

    /// <summary> Set player class </summary>
    public void SetPlayerClass(string playerClass) {
        playerCustomProp[PlayerProperties.PlayerClass] = playerClass;
        player.SetCustomProperties(playerCustomProp);
    }

    /// <summary> Set player custom properties </summary>
    public void PlayerCustomProperties() {
        playerCustomProp[PlayerProperties.PlayerClass] = "Warrior";
        playerCustomProp[PlayerProperties.PlayerReady] = "False";
        playerCustomProp[PlayerProperties.PlayerTeam] = "FFA";
        player.SetCustomProperties(playerCustomProp);
    }

    // TODO: Let players know who is the master client via icon
}
