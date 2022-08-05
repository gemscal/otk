using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class PlayerListItem : MonoBehaviourPunCallbacks
{
    [SerializeField] TMP_Text playerName;
    [SerializeField] GameObject roomMasterIcon;
    [SerializeField] Color roomPlayerColor;
    [SerializeField] Image roomPlayerBG;
    Player player;

    public void SetUp(Player _player) {
        player = _player;
        playerName.text = player.NickName;
        // Debug.Log(PhotonNetwork.IsMasterClient);
        if (_player == PhotonNetwork.LocalPlayer) {
            Debug.Log("true");
            ApplyLocalChanges();
        }

        if (_player == PhotonNetwork.MasterClient) {
            roomMasterIcon.SetActive(PhotonNetwork.IsMasterClient);
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
    }
}
