using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    //武器の攻撃力
    [SerializeField]
    private int Power=0;
    private bool isAttacking=false;

    //攻撃中かどうか
    public bool IsAttacking {
        get { return isAttacking; }
        set
        {
            isAttacking = value;
        }

    }


    //=============================================================
    //プレイヤークラスを取得してダメージ（Playerがない時は使えない）
    //=============================================================

    private void OnTriggerEnter(Collider other)
    {
        //攻撃中のみ判定
        if (IsAttacking)
        {
            //当たり判定のデバッグ用
            Debug.Log("collision");
            //    var player = other.GetComponent<Player>();
            //    if (player != null)
            //    {
            //        player.Hp -= Power;
            //    }
        }
    }
}
