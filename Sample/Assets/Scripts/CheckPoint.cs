using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    //自分の次のチェックポイント
    [SerializeField]
    private CheckPoint next=null;
    //自分の前のチェックポイント
    [SerializeField]
    private CheckPoint prev=null;
    public CheckPoint Next
    {
        get { return next; }
    }
    public CheckPoint Prev
    {
        get { return prev; }
    }
    public Vector3 Pos {
        get { return transform.position; }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            Enemy enemy = other.gameObject.GetComponent<Enemy>();
            if (enemy != null)
            {
                //敵のチェックポイントを設定
                SetCheckPoint(other.gameObject.GetComponent<Enemy>());
            }
        }
    }
    //チェックポイント設定
    private void SetCheckPoint(Enemy enemy)
    {
        if (enemy.CheckPoint == null)
        {
            enemy.CheckPoint = this;
        }
        else
        {
            if (next == null) return;
            enemy.CheckPoint = next;
        }
    }
}
