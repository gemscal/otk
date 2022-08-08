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
    Hashtable playerProperties = new Hashtable();
    
    Player player;

    public void SetUp(Player _player) {
        player = _player;
        playerName.text = player.NickName;

        if (_player == PhotonNetwork.LocalPlayer) {
            ApplyLocalChanges();
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

    public void ApplyLocalChanges() {
        roomPlayerBG.color = roomPlayerColor;
        characterClass.SetActive(true);
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps) {
        if (player == targetPlayer) {
            playerClass.text = (string)player.CustomProperties[PlayerProperties.PlayerClass];
        }
    }

    // player properties
    public void SetCharacterClass(string charClass) {
        playerProperties[PlayerProperties.PlayerClass] = charClass;
        PhotonNetwork.SetPlayerCustomProperties(playerProperties);
    }


    // TODO: Let players know who is the master client via icon

    // public void CheckMasterClient(Player _player) {
    //     if (_player == PhotonNetwork.MasterClient) {
    //         roomMasterIcon.SetActive(true);
    //     }
    // }

    // public override void OnMasterClientSwitched(Player newMasterClient) {
    //     CheckMasterClient(newMasterClient);
    // }
}
