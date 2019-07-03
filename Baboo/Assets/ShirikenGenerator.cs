using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShirikenGenerator : MonoBehaviour
{

    [SerializeField] private GameObject Shirikenprefab;
    [SerializeField] private int Speed;
    [SerializeField] private int Resistance;
    [SerializeField] private int Rotation;
    [SerializeField] private int Maxcnt;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        RAY();
        if(Input.GetMouseButtonDown(0))
        {
            var Shiriken = Instantiate(Shirikenprefab);

            //仮打ち出し場所
            Shiriken.transform.position = transform.position;

            //アニメーション用
            Shiriken.GetComponent<Rigidbody>().angularVelocity = new Vector3(Rotation, 0, 0);

            //打ち出しスピードと重力に対しての抵抗力
            Shiriken.GetComponent<Rigidbody>().AddForce(new Vector3(0, Resistance, Speed));

            //仮削除
            Destroy(Shiriken, 1.0f);

            //存在している手裏剣の数を調べる
            int Nowcnt = GameObject.FindGameObjectsWithTag("Shiriken").Length;
            //規定値を超えたら消す
            //if (Nowcnt > Maxcnt)
            //{

            //}
        }
    }

    void RAY()
    {
        Ray ray = new Ray(transform.position, new Vector3(0, 0, 1));

        RaycastHit hit;

        float distance = 100;

        if (Physics.Raycast(ray, out hit, distance))
        {
            Debug.Log("aaa");
        }
    }
}
