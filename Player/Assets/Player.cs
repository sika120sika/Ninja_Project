using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum PlayerState
{
    //立つ
    Stand,

    //歩く
    Walk,

    //走る
    Run,

    //しゃがむ
    Squat,

    //しゃがんで歩く
    Squat_Run,

    //攻撃
    Attack
}

public class Player : MonoBehaviour
{
    private CharacterController controller = null;
    private Animator anim = null;
    private GameObject camera = null;

    bool Leyflg = false;

    //ヒットポイント
    [SerializeField] private int HP = 100;

    //歩くスピード
    [SerializeField] private float WalkSpeed = 0.5f;

    //振り向く速度
    [SerializeField] private float applySpeed = 0.2f;

    //重力
    [SerializeField] private float Gravity = 0.5f;   

    CharacterController characterController;

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();
        camera = Camera.main.gameObject;
        characterController = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        var vMove = new Vector3(0, 0, 0);

        //移動ベクトルを設定
        var vAddF = camera.transform.forward;
        var vAddS = camera.transform.right;

        //Y軸を０にして、正規化する
        vAddF.y = 0;
        vAddF.Normalize();
        vAddS.y = 0;
        vAddS.Normalize();

        //移動ベクトル取得
        if (Input.GetKey(KeyCode.W)) { vMove += vAddF; }
        if (Input.GetKey(KeyCode.S)) { vMove += -vAddF; }
        if (Input.GetKey(KeyCode.A)) { vMove += -vAddS; }
        if (Input.GetKey(KeyCode.D)) { vMove += vAddS; }

        //右クリック
        if (Input.GetMouseButtonDown(0))
        {

        }        

        // 速度調整
        vMove.Normalize();
        vMove *= WalkSpeed;


        //キャラの振り向き処理
        if (vMove.magnitude > 0)
        {
            //プレイヤーが進行方向に向く
            transform.rotation = Quaternion.Slerp(transform.rotation,
                Quaternion.LookRotation(vMove),
            applySpeed
            );

            var tmpvec = new Vector3(vMove.x, 0, vMove.z);

        }

        //地面についていなっかたら落下
        if (!characterController.isGrounded)
        {
            vMove.y -= Gravity;          
        }        

        

        //移動を実行
        controller.Move(vMove);
    }
}
