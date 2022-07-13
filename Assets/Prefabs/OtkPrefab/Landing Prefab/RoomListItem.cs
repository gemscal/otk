using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Realtime;

public class RoomListItem : MonoBehaviour
{
    [SerializeField] TMP_Text roomName;
    public RoomInfo info;
    
    public void SetUp(RoomInfo _info) {
        info = _info;
        roomName.text = _info.Name;
    }

    public void OnCLick() {
        Launcher.Instance.JoinRoom(info);
    }
}
