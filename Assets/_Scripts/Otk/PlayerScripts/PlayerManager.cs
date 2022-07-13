using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Photon.Pun;

public class PlayerManager : MonoBehaviour
{
    PhotonView PV;

    void Awake() {
        PV = GetComponent<PhotonView>();
    }

    // Start is called before the first frame update
    void Start() {
        if (PV.IsMine) {
            CreateController();
        }
    }

    // will call this function to initialize charcter controller
    void CreateController() {
        PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerController"), Vector3.zero, Quaternion.identity);
    }
}
