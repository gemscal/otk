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

        // check if the room is private & add or remove private icon
        string checkForPassword = roomInfo.Name.Remove(17);
        bool isPublic = checkForPassword.Contains("NOPASS");
        if (isPublic) {
            privateRoom.SetActive(false);
        }
        
        // TODO: Game mode section

        // TODO: Map section

        // removing the custom room propertiest on the name
        string roomNameDisplay = roomInfo.Name;
        roomNameDisplay = roomNameDisplay.Remove(0, 17);
        roomName.text = roomNameDisplay;
    }

    public void OnCLick() {
        Launcher.Instance.JoinRoom(roomInfo);
    }
}
