using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Realtime;

public class RoomListItem : MonoBehaviour
{
    [SerializeField] TMP_Text roomName;
    [SerializeField] GameObject map;
    [SerializeField] GameObject gameMode;
    [SerializeField] GameObject privateRoom;

    public RoomInfo roomInfo;
    
    public void SetUp(RoomInfo _info) {
        roomInfo = _info;
        roomName.text = _info.Name;

        Debug.Log(Launcher.Instance.deathMatchRoom.Count);
    }

    public void OnCLick() {
        Launcher.Instance.JoinRoom(roomInfo);
    }
}
