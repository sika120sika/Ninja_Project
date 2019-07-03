using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletGenerator : MonoBehaviour
{
    [SerializeField] private GameObject Bullet;
    [SerializeField] private Transform Muzzle;
    [SerializeField] private int Speed;

    // Start is called before the first frame update
    void Start()
    {

    }
    // Update is called once per frame
    void Update()
    {

        if (Input.GetMouseButtonDown(0))
        {
            var Bullets = Instantiate(Bullet);

            //仮打ち出し場所
            Bullets.transform.position = transform.position;

            //打ち出しスピード
            Bullets.GetComponent<Rigidbody>().AddForce(new Vector3(0, 0, Speed));

            // 弾丸の位置を調整
            Bullets.transform.position = Muzzle.position;

            //仮削除
            Destroy(Bullets, 3.0f);
        }
    }

    void OnTriggerEnter(Collider t)
    {
        //敵に当たったら削除
        Destroy(gameObject);
    }
}
