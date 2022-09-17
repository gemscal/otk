using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerController : MonoBehaviour
{

    public Rigidbody2D rb;
    public float moveSpeed;
    Vector2 movement;
    PhotonView PV;

    void Awake() {
        PV = GetComponent<PhotonView>();
    }

    // Start is called before the first frame update
    void Start() {
        if (!PV.IsMine) {
            Destroy(GetComponentInChildren<Camera>().gameObject);
            Destroy(GetComponentInChildren<Cinemachine.CinemachineVirtualCamera>().gameObject);
        }
    }

    // Update is called once per frame
    void Update() {
        if (!PV.IsMine) return;

        movement.x = Input.GetAxis("Horizontal");
        movement.y = Input.GetAxis("Vertical");
    }

    void FixedUpdate() {
        if (!PV.IsMine) return;
        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
    }
}
