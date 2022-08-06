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
    [SerializeField] TMP_Text playerWeapon;
    [SerializeField] Color roomPlayerColor;
    [SerializeField] Image roomPlayerBG;
    [SerializeField] GameObject roomMasterIcon;
    [SerializeField] GameObject weaponType;
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
        weaponType.SetActive(true);
    }
    
    // player properties
    public void SetCharacterClass(string weaponType) {
        playerProperties["playerWeapon"] = weaponType;
        PhotonNetwork.SetPlayerCustomProperties(playerProperties);
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps) {
        if (player == targetPlayer) {
            playerWeapon.text = (string)player.CustomProperties["playerWeapon"];
        }
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
