using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    //武器の攻撃力
    [SerializeField]
    private int Power=0;


    //=============================================================
    //プレイヤークラスを取得してダメージ（Playerがない時は使えない）
    //=============================================================

    //private void OnTriggerEnter(Collider other)
    //{
    //    var player = other.GetComponent<Player>();
    //    if (player != null)
    //    {
    //        player.Hp -= Power;
    //    }
    //}
}
