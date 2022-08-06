using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;
using UnityEngine;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class Launcher : MonoBehaviourPunCallbacks
{
    public static Launcher Instance;

    [SerializeField] TMP_InputField playerNameInputField;
    [SerializeField] TMP_Text errorMessage;
    [SerializeField] Transform roomListContent;
    [SerializeField] Transform playerListContent;
    [SerializeField] GameObject roomListItemPrefab;

    [Header("Create Room Information")]
    [SerializeField] TMP_InputField roomNameInputField;
    [SerializeField] TMP_InputField roomPassInputField;
    [SerializeField] TMP_Text numberOfPlayers;
    [SerializeField] TMP_Text numberOfKills;

    [Header("Inside Room Information")]
    [SerializeField] TMP_Text roomName;
    [SerializeField] GameObject playerListItemPrefab;
    [SerializeField] TMP_Text roomMap;
    [SerializeField] TMP_Text roomGameMode;
    [SerializeField] TMP_Text roomKillGoal;
    [SerializeField] TMP_Text roomNumberOfPlayer;
    [SerializeField] TMP_Text roomPassword;
    [SerializeField] GameObject roomReadyButton;
    [SerializeField] GameObject startGameButton;

    // public List<string> deathMatchRoom = new List<string>();
    private static Dictionary<string, RoomInfo> cachedRoomList = new Dictionary<string, RoomInfo>();

    void Awake() {
        Instance = this;
    }

    void Start() {
        StartCoroutine(loadingEnd());
        Debug.Log("Connecting to Master");
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
    }

    // loading screen
    IEnumerator loadingEnd() {
        yield return new WaitForSeconds(1);
        PhotonNetwork.ConnectUsingSettings();
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

    public void RoomPasswordOn() {
        if (string.IsNullOrEmpty(roomPassInputField.text)) {
            roomPassInputField.text = "R" + Random.Range(10000, 99999).ToString("00000");
        }
    }

    public void RoomPasswordOff() {
        roomPassInputField.text = null;
    }

    public void NumberOfPlayerDec() {
        int numOfPlayers = Int32.Parse(numberOfPlayers.text);
        if (numOfPlayers == 2) {
            return;
        } else {
            numOfPlayers -= 1;
            numberOfPlayers.text = numOfPlayers.ToString();
        }
    }

    public void NumberOfPlayerInc() {
        int numOfPlayers = Int32.Parse(numberOfPlayers.text);
        if (numOfPlayers == 8) {
            return;
        } else {
            numOfPlayers += 1;
            numberOfPlayers.text = numOfPlayers.ToString();
        }
    }

    public void NumberOfKillsDec() {
        int numOfkills = Int32.Parse(numberOfKills.text);
        if (numOfkills == 5) {
            return;
        } else {
            numOfkills -= 5;
            numberOfKills.text = numOfkills.ToString();
        }
    }

    public void NumberOfKillsInc() {
        int numOfkills = Int32.Parse(numberOfKills.text);
        if (numOfkills == 20) {
            return;
        } else {
            numOfkills += 5;
            numberOfKills.text = numOfkills.ToString();
        }
    }

    // prep function for creating room
    public void PrepCreateRoom() {
        if (string.IsNullOrEmpty(roomNameInputField.text)) {
            return;
        }
        
        byte maxPlayer = Convert.ToByte(numberOfPlayers.text);
        int killGoal = Int32.Parse(numberOfKills.text);
        string roomName = roomNameInputField.text;
        string gameMode = "DM";
        string map = "map1";
        string roomPass;

        if (string.IsNullOrEmpty(roomPassInputField.text)) {
            roomPass = "NOPASS";
        } else {
            roomPass = roomPassInputField.text;
        }

        roomName = $"{gameMode} {map} {roomPass} {maxPlayer} {roomName}";
        CreateRoom(maxPlayer, killGoal, roomName, gameMode, map, roomPass);
    }
    
    /// <summary>
    /// Room creation function
    /// </summary>
    public void CreateRoom (byte _maxPlayer, int _killGoal, string _roomName, string _gameMode, string _map, string _roomPass) {
        
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = _maxPlayer;
        
        roomOptions.CustomRoomProperties = new Hashtable();
        roomOptions.BroadcastPropsChangeToAll = true;
        roomOptions.CustomRoomProperties.Add(RoomProperties.Map, _map);
        roomOptions.CustomRoomProperties.Add(RoomProperties.GameMode, _gameMode);
        roomOptions.CustomRoomProperties.Add(RoomProperties.KillGoal, _killGoal);
        roomOptions.CustomRoomProperties.Add(RoomProperties.RoomPassword, _roomPass);

        PhotonNetwork.CreateRoom(_roomName, roomOptions);
        MenuManager.Instance.OpenMenu("loading");
    }

    // get room info from photon server and assign to client
    public void SetRoomInfo() {
        string _roomName = PhotonNetwork.CurrentRoom.Name;
        roomName.text = _roomName.Remove(0, 17);

        int _maxPlayer = (int)PhotonNetwork.CurrentRoom.MaxPlayers;
        roomNumberOfPlayer.text = _maxPlayer.ToString() + " Players";

        int _killGoal = (int)PhotonNetwork.CurrentRoom.CustomProperties[RoomProperties.KillGoal];
        roomKillGoal.text = _killGoal.ToString() + " Kills";

        roomMap.text = (string)PhotonNetwork.CurrentRoom.CustomProperties[RoomProperties.Map];
        roomGameMode.text = (string)PhotonNetwork.CurrentRoom.CustomProperties[RoomProperties.GameMode];
        roomPassword.text = (string)PhotonNetwork.CurrentRoom.CustomProperties[RoomProperties.RoomPassword];
    }

    // will call this function if the room is successfully created
    public override void OnJoinedRoom() {
        Debug.Log("Connected to room");

        // room details setup
        SetRoomInfo();
        MenuManager.Instance.OpenMenu("room");

        Player[] players = PhotonNetwork.PlayerList;

        foreach (Transform child in playerListContent) {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < players.Count(); i++) {
            Instantiate(playerListItemPrefab, playerListContent).GetComponent<PlayerListItem>().SetUp(players[i]);
        }

        startGameButton.SetActive(PhotonNetwork.IsMasterClient);
        // roomMasterIcon.SetActive(PhotonNetwork.IsMasterClient); // need fix
    }

    public override void OnJoinRoomFailed(short returnCode, string message) {
        errorMessage.text = "Joining Room Failed: " + message;
        MenuManager.Instance.OpenMenu("failed");
    }
}
