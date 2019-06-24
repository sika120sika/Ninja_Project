using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    //武器の攻撃力
    [SerializeField]
    private int Power=0;
    private bool onAttack=false;

    //攻撃中かどうか
    public bool OnAttack {
        get { return onAttack; }
        set
        {
            onAttack = value;
        }

    }


    //=============================================================
    //プレイヤークラスを取得してダメージ（Playerがない時は使えない）
    //=============================================================

    private void OnTriggerEnter(Collider other)
    {
        //攻撃中のみ判定
        if (onAttack)
        {
            //    var player = other.GetComponent<Player>();
            //    if (player != null)
            //    {
            //        player.Hp -= Power;
            //    }
        }
    }
}
