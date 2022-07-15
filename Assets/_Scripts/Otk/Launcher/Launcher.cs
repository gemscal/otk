using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using Photon.Realtime;
using System.Linq;

public class Launcher : MonoBehaviourPunCallbacks
{
    public static Launcher Instance;

    [SerializeField] TMP_InputField playerNameInputField;
    [SerializeField] TMP_InputField roomNameInputField;
    [SerializeField] TMP_Text roomName;
    [SerializeField] TMP_Text errorMessage;
    [SerializeField] Transform roomListContent;
    [SerializeField] Transform playerListContent;
    [SerializeField] GameObject roomListItemPrefab;
    [SerializeField] GameObject playerListItemPrefab;
    [SerializeField] GameObject startGameButton;
    [SerializeField] TMP_Dropdown gameModeDropdown;
    //private string _gameMode;

    private static Dictionary<string, RoomInfo> cachedRoomList = new Dictionary<string, RoomInfo>();

    void Awake() {
        Instance = this;
    }

    void Start() {
        StartCoroutine(loadingEnd());
        Debug.Log("Connecting to Master");
        gameModeDropdown.value = 0;
    }

    // connect to lobby once the user successfully connected to master server
    public override void OnConnectedToMaster() {
        Debug.Log("Connected to Master");
        PhotonNetwork.JoinLobby();
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    // will call this function once the user successfully connected to lobby
    public override void OnJoinedLobby() {
        Debug.Log("Joined Lobby");
        MenuManager.Instance.OpenMenu("title");
        //PhotonNetwork.NickName = "Player " + Random.Range(0, 1000).ToString("0000");
    }

    // loading screen
    IEnumerator loadingEnd() {
        yield return new WaitForSeconds(5);
        PhotonNetwork.ConnectUsingSettings();
    }

    // creating a room
    public void CreateRoom() {
        if (string.IsNullOrEmpty(roomNameInputField.text)) {
            return;
        }
        PhotonNetwork.CreateRoom(roomNameInputField.text);
        MenuManager.Instance.OpenMenu("loading");
    }

    // will call this function if the room is successfully created
    public override void OnJoinedRoom() {
        Debug.Log("Connected to room");
        roomName.text = PhotonNetwork.CurrentRoom.Name;
        MenuManager.Instance.OpenMenu("room");

        Player[] players = PhotonNetwork.PlayerList;

        foreach (Transform child in playerListContent) {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < players.Count(); i++) {
            Instantiate(playerListItemPrefab, playerListContent).GetComponent<PlayerListItem>().SetUp(players[i]);
        }

        startGameButton.SetActive(PhotonNetwork.IsMasterClient);
    }

    // will call this function if a room is not successfully created
    public override void OnCreateRoomFailed(short returnCode, string message) {
        errorMessage.text = "Room Creation Failed: " + message;
        MenuManager.Instance.OpenMenu("failed");
    }

    // join room function
    public void JoinRoom (RoomInfo info) {
        PhotonNetwork.JoinRoom(info.Name);
        MenuManager.Instance.OpenMenu("loading");
    }

    // leaving room function
    public void LeaveRoom() {
        Debug.Log("Leaving Room");
        PhotonNetwork.LeaveRoom();
        MenuManager.Instance.OpenMenu("loading");
    }

    // will call this function if user successfully leave the room
    public override void OnLeftRoom() {
        Debug.Log("Player left the room");
        MenuManager.Instance.OpenMenu("title");
    }

    // will call this function everytime theres a update in the room listing
    public override void OnRoomListUpdate(List<RoomInfo> roomList) {

        // delete all the rooms everytime we get an update
        foreach (Transform trans in roomListContent) {
            Destroy(trans.gameObject);
        }

        // update list of rooms in the lobby
        // for (int i = 0; i < roomList.Count; i++) {
        //     if (roomList[i].RemovedFromList) 
        //         continue;
        //     Instantiate(roomListItemPrefab, roomListContent).GetComponent<RoomListItem>().SetUp(roomList[i]);
        // }
        for (int i = 0; i < roomList.Count; i++) {
            RoomInfo info = roomList[i];
            if (info.RemovedFromList) {
                cachedRoomList.Remove(info.Name);
            } else {
                cachedRoomList[info.Name] = info;
            }
        }

        foreach (KeyValuePair<string, RoomInfo> entry in cachedRoomList) {
            Instantiate(roomListItemPrefab, roomListContent).GetComponent<RoomListItem>().SetUp(cachedRoomList[entry.Key]);
        }
        Debug.Log("Update Room list");
    }

    // will call this function if the remote player joined the room
    public override void OnPlayerEnteredRoom(Player newPlayer) {
        Instantiate(playerListItemPrefab, playerListContent).GetComponent<PlayerListItem>().SetUp(newPlayer);
    }

    // start game function
    public void StartGame() {
        PhotonNetwork.LoadLevel(1);
    }

    // will call this function if the master client has changed
    public override void OnMasterClientSwitched(Player newMasterClient) {
        startGameButton.SetActive(PhotonNetwork.IsMasterClient);
    }

    // public void GameMode(int gameMode) {
    //     if(gameMode == 0) {
    //         _gameMode = "last man standing";
    //     }

    //     if(gameMode == 1) {
    //         _gameMode = "team battle";
    //     }

    //     if(gameMode == 2) {
    //        _gameMode = "1v1";
    //     }
    // }


    ////////////////////////
    // Tempory Functions  //
    ////////////////////////


    // quit game
    public void QuitGame() {
        Application.Quit();
    }

    public void AssignName() {
        if (string.IsNullOrEmpty(playerNameInputField.text)) {
            playerNameInputField.text = "Player " + Random.Range(0, 1000).ToString("0000");
        }
        PhotonNetwork.NickName = playerNameInputField.text;
    }
}
