using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyState
{
    //徘徊
    Wander,
    //追跡
    Chase,
    //攻撃
    Attack,
    //死亡
    Death
}
public class Enemy: StatefulObjectBase<Enemy,EnemyState>
{

    //アニメーター（ステートクラス内で弄る）
    private Animator m_animator = null;
    
    [SerializeField]
    private float m_speed=0.0f;


    //移動用navmesh
    private NavMeshAgent m_navMeshAgent=null;
    



    //チェックポイント（初期値は任意で決定）
    [SerializeField]
    private CheckPoint m_checkPoint = null;

    public CheckPoint CheckPoint {
        get { return m_checkPoint; }
        set { m_checkPoint = value; }
    }

    

    private CharacterController m_characterController;
    
    

    //捕捉したプレイヤー
    private GameObject m_target = null;
    
    private void Start()
    {
        Init();
    }

    //初期化処理
    public void Init()
    {
        stateList.Add(new StateWander(this));
        stateList.Add(new StateChase(this));
        stateList.Add(new StateAttack(this));
        stateList.Add(new StateDeath(this));
        //ステートマシンを初期化
        stateMachine = new StateMachine<Enemy>();
        ChangeState(EnemyState.Wander);

        //キャラクターコントローラー取得
        m_characterController = GetComponent<CharacterController>();
        //NavMeshAgent取得
        m_navMeshAgent = GetComponent<NavMeshAgent>();
    }


    private void Awake()
    {
        var searching = GetComponentInChildren<SearchPlayer>();

        //アニメーター取得
        m_animator = GetComponentInChildren<Animator>();

        searching.onFound += OnFound;
        searching.onLost += OnLost;
    }
    //見つけた時
    private void OnFound(GameObject foundObject)
    {
        m_target = foundObject;
        ChangeState(EnemyState.Chase);
        
    }
    //見失ったとき
    private void OnLost(GameObject lostObject)
    {
        m_target = null;
        if (m_target == null) { 
        }
        ChangeState(EnemyState.Wander);
    }
    //ステートクラス設定
    #region States
    //ステート：徘徊
    private class StateWander : State<Enemy>
    {
        public StateWander(Enemy owner) : base(owner){ }
        public override void Enter()
        {
            if (owner.m_animator != null)
            {
                owner.m_animator.SetBool("Walk", true);
            }

            owner.m_speed = 1.0f;
        }
        public override void Execute()
        {
            if (owner.m_checkPoint == null) return;

            //ナビメッシュで移動
            if (owner.m_navMeshAgent != null)
            {
                //移動位置を指定
                owner.m_navMeshAgent.destination = owner.m_checkPoint.Pos;
                //移動速度を設定
                owner.m_navMeshAgent.speed = owner.m_speed;
            }
        }
        public override void Exit()
        {
            if (owner.m_animator != null)
            {
                owner.m_animator.SetBool("Walk", false);
            }
        }
    }
    //ステート：追跡
    private class StateChase : State<Enemy>
    {
        public StateChase(Enemy owner) : base(owner) { }

        //平行進行方向ベクトル
        private Vector3 moveDir;

        public override void Enter()
        {
            if (owner.m_animator != null)
            {
                owner.m_animator.SetBool("Run", true);
            }
            owner.m_speed = 1.5f;
        }
        public override void Execute()
        {
            Vector3 myPosXZ = Vector3.Scale(new Vector3(1,0,1),owner.transform.position);
            Vector3 targetPosXZ = Vector3.Scale(new Vector3(1, 0, 1), owner.m_target.transform.position);
            moveDir = targetPosXZ -myPosXZ;
            //距離が近くなれば攻撃
            if (moveDir.sqrMagnitude < 2.0f)
            {
                owner.ChangeState(EnemyState.Attack);
                return;
            }
            //ナビメッシュで移動
            if (owner.m_navMeshAgent != null)
            {
                //移動位置を指定
                owner.m_navMeshAgent.destination = owner.m_target.transform.position;
                //移動速度を設定
                owner.m_navMeshAgent.speed = owner.m_speed;
            }
        }
        public override void Exit()
        {
            if (owner.m_animator != null)
            {
                owner.m_animator.SetBool("Run", false);
            }
        }
    }
    //ステート：攻撃
    private class StateAttack : State<Enemy>
    {
        public StateAttack(Enemy owner) : base(owner) { }

        private Vector3 moveDir;
        //敵の子要素の武器コンポーネント
        private Weapon weapon = null;
        public override void Enter()
        {
            //位置を動かさない
            owner.m_navMeshAgent.destination = owner.transform.position;
            //ワンチャン重い
            weapon = owner.GetComponentInChildren<Weapon>();
            //アニメーション設定
            if (owner.m_animator != null)
            {
                owner.m_animator.SetBool("Attack", true);
            }

            owner.m_speed = 1.0f;
        }
        public override void Execute()
        {

            Vector3 myPosXZ = Vector3.Scale(new Vector3(1, 0, 1), owner.transform.position);
            Vector3 targetPosXZ = Vector3.Scale(new Vector3(1, 0, 1), owner.m_target.transform.position);
            moveDir = targetPosXZ - myPosXZ;
            //離れたら追いかける
            if (moveDir.sqrMagnitude >= 2.0f)
            {
                owner.ChangeState(EnemyState.Chase);
                return;
            }
            //現在のアニメーションが攻撃アニメーションで
            //武器を振り下ろしている状態なら武器を攻撃状態にする
            if (owner.m_animator.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.Slash") && owner.m_animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.7)
            {
                weapon.IsAttacking = true;
            }
            else
            {
                weapon.IsAttacking = false;
            }
        }
        public override void Exit()
        {
            if(weapon!=null) weapon.IsAttacking = false;
            if (owner.m_animator != null)
            {
                owner.m_animator.SetBool("Attack",false);
            }
        }
    } //ステート：死亡
    private class StateDeath : State<Enemy>
    {
        public StateDeath(Enemy owner) : base(owner) { }
        public override void Enter()
        {
        }
        public override void Execute()
        {
        }
        public override void Exit()
        {
        }
    }
    #endregion
}
