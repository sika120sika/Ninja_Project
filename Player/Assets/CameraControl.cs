using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    [SerializeField] Transform Player;
    [SerializeField] float RotateSpeed;
    [SerializeField] private float posY = 7.0f;
    [SerializeField] private float posZ = 12.0f;

    [SerializeField] private float LookY = 5.0f;

    public float yaw, pitch;

    // Start is called before the first frame update
    void Start()
    {
        RotateSpeed = 1;

        //カーソルを画面中央に固定
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {                      
        //回転処理
        yaw += Input.GetAxis("Mouse X") * RotateSpeed;               //横回転入力
        pitch -= Input.GetAxis("Mouse Y") * RotateSpeed;             //縦回転入力

        pitch = Mathf.Clamp(pitch, -80, 60);                        //縦回転角度制限する

        transform.eulerAngles = new Vector3(pitch, yaw, 0.0f);      //回転の実行

        //移動処理
        Vector3 vec = -transform.forward;

        transform.position = vec * posZ;

        //プレイヤー位置を追従する
        transform.position = new Vector3(
            transform.position.x + Player.position.x, 
            transform.position.y + Player.position.y + posY,
            transform.position.z + Player.position.z        
        );

        Vector3 cleatePos = new Vector3(
            Player.position.x,
            Player.position.y + LookY,
            Player.position.z
            );

        //プレイヤーを見る
        transform.LookAt(cleatePos, Vector3.up);
    }
}
