using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class SearchPlayer : MonoBehaviour
{
    private class FoundData
    {
        private GameObject m_obj = null;
        public FoundData(GameObject obj)
        {
            m_obj = obj;
        }
        //今プレイヤーを見つけているか
        private bool m_isCurrentFound = false;
        //前プレイヤーを見つけていたか
        private bool m_isPrevFound = false;
        public GameObject Obj
        {
            get
            {
                return m_obj;
            }
        }
        public Vector3 Position
        {
            get
            {
                if (Obj != null)
                    return Obj.transform.position;
                else
                    return Vector3.zero;
            }
        }
        public void Update(bool isFound)
        {
            m_isPrevFound = m_isCurrentFound;
            m_isCurrentFound = isFound;
        }
        public bool IsFound()
        {
            //前回見つけておらず今回見つけたら「見つけた」という処理
            return m_isCurrentFound && !m_isPrevFound;
        }
        public bool IsLost()
        {
            //前回見つけ今回見つけなかったら「見失った」という処理
            return !m_isCurrentFound && m_isPrevFound;
        }
        public bool IsCurrentFound()
        {
            return m_isCurrentFound;
        }
    }


    //感知する角度
    [SerializeField, Range(0.0f, 360.0f)]
    private float m_searchAngle = 0.0f;
    //感知用コライダー
    private SphereCollider m_sphereCollider = null;
    //対象との内積のcosθ
    private float m_searchCosTheta = 0.0f;

    private List<FoundData> m_foundList = new List<FoundData>();
    //スクリプトがリロードされたとき
    private void OnDisable()
    {
        m_foundList.Clear();
    }

    //角度を返す
    public float SearchAngle
    {
        get
        {
            return m_searchAngle;
        }
    }
    //コライダーの大きさを返す
    public float SearchRadius
    {
        get
        {
            if (m_sphereCollider == null)
            {
                m_sphereCollider = GetComponent<SphereCollider>();
            }
            if (m_sphereCollider != null)
                return m_sphereCollider.radius;
            else
                return 0.0f;
        }
    }

    private void Awake()
    {
        m_sphereCollider = GetComponent<SphereCollider>();
    }
    //[SerializeField]された変数が変更されたときに呼び出される関数
    private void OnValidate()
    {
        ApplySearchAngle();
    }
    private void ApplySearchAngle()
    {
        float searchRad = m_searchAngle * 0.5f * Mathf.Deg2Rad;
        m_searchCosTheta = Mathf.Cos(searchRad);
    }


    public event System.Action<GameObject> onFound = (obj) => { };
    public event System.Action<GameObject> onLost = (obj) => { };

    private void OnTriggerEnter(Collider other)
    {
        GameObject enterObject = other.gameObject;

        if (m_foundList.Find(value => value.Obj == enterObject) == null)
        {
            m_foundList.Add(new FoundData(enterObject));
        }
    }
    private void OnTriggerExit(Collider other)
    {
        GameObject exitObject = other.gameObject;

        var foundData = m_foundList.Find(value => value.Obj == exitObject);

        if (foundData == null)
        {
            return;
        }
        if (foundData.IsCurrentFound())
        {
            onLost(foundData.Obj);
        }
        m_foundList.Remove(foundData);
        
    }
    private void Update()
    {
        UpdateFoundObject();
    }
    private void UpdateFoundObject()
    {
        foreach(var foundData in m_foundList)
        {
            GameObject targetObject = foundData.Obj;
            if (targetObject == null)
            {
                continue;
            }
            bool isFound = CheckFoundObject(targetObject);
            foundData.Update(isFound);
            if (foundData.IsFound())
            {
                onFound(targetObject);
            }
            if (foundData.IsLost())
            {
                onLost(targetObject);
            }
        }
    }
    private bool CheckFoundObject(GameObject target)
    {
        //ターゲット（プレイヤー）の座標
        Vector3 targetPosition = target.transform.position;
        //自分（敵）の座標
        Vector3 myPosition = transform.position;
        Vector3 myPositionXZ = Vector3.Scale(myPosition,new Vector3(1.0f,0.0f,1.0f));
        Vector3 targetPositionXZ = Vector3.Scale(targetPosition,new Vector3(1.0f,0.0f,1.0f));
        //ターゲットへの水平なベクトル
        Vector3 toTargetFlatDir = (targetPositionXZ - myPositionXZ).normalized;
        Vector3 myForward = transform.forward;
        if (!IsWithinRangeAngle(myForward, toTargetFlatDir, m_searchCosTheta))
        {
            return false;
        }
        myPosition += new Vector3(0, 0.5f, 0);
        targetPosition += new Vector3(0, 0.5f, 0);
        Vector3 toTargetDir = (targetPosition - myPosition).normalized;
        if (!IsHitRay(myPosition, toTargetDir, target))
        {
            return false;
        }

        return true;
    }
    //範囲内にターゲットがいるかどうかの判定
    private bool IsWithinRangeAngle(Vector3 forwardDir,Vector3 toTargetDir,float cosTheta)
    {
        //方向ベクトルがないかどうかの判定（なければターゲットとプレイヤーが同一に存在するためtrue）
        if (toTargetDir.sqrMagnitude <= Mathf.Epsilon)
        {
            return true;
        }
        float dot = Vector3.Dot(forwardDir, toTargetDir);
        return dot >= cosTheta;
    }
    //レイがヒットしたかどうか
    private bool IsHitRay(Vector3 fromPosition,Vector3 toTargetDir,GameObject target)
    {
        if (toTargetDir.sqrMagnitude <= Mathf.Epsilon)
        {
            return true;
        }
        RaycastHit onHitRay;
        Debug.DrawRay(fromPosition, toTargetDir*SearchRadius, Color.black);
        if(!Physics.Raycast(fromPosition, toTargetDir, out onHitRay, SearchRadius))
        {
            return false;
        }
        if (onHitRay.transform.gameObject != target)
        {
            return false;
        }
        return true;
    }
}
