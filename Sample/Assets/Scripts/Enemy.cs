using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public Animator Animator
    {
        get { return m_animator; }
    }

    private float m_speed=0.0f;

    public float Speed
    {
        get { return m_speed; }
        set { m_speed = value; }
    }



    //チェックポイント（初期値は任意で決定）
    [SerializeField]
    private CheckPoint m_checkPoint = null;
    
    public CheckPoint CheckPoint
    {
        get
        { return m_checkPoint; }
        set
        { m_checkPoint = value; }
    }

    private CharacterController m_characterController;

    //キャラクターコントローラープロパティ
    public CharacterController CharacterController
    {
        get { return m_characterController; }

    }
    

    //捕捉したプレイヤー
    private GameObject m_target = null;

    public Vector3 targetPos
    {
        get { return m_target.transform.position; }
    }

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

        m_characterController = GetComponent<CharacterController>();
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
        //平行進行方向ベクトル
        private Vector3 flatmoveDir;
        //最終進行方向ベクトル
        private Vector3 moveDir;

        public override void Enter()
        {
            if (owner.Animator != null)
            {
                owner.Animator.SetBool("Walk", true);
            }

            owner.Speed = 1.0f;
        }
        public override void Execute()
        {
            if (owner.CheckPoint == null) return;
            Vector3 targetPositionXZ = Vector3.Scale(owner.CheckPoint.Pos, new Vector3(1, 0, 1));
            Vector3 myPositionXZ = Vector3.Scale(owner.transform.position, new Vector3(1, 0, 1));
            //進行方向取得
            flatmoveDir = targetPositionXZ - myPositionXZ;
            flatmoveDir.Normalize();
            moveDir = flatmoveDir;
            if (!owner.CharacterController.isGrounded)
                moveDir.y = -5f;
            moveDir *= owner.Speed;
            owner.CharacterController.Move(moveDir*Time.deltaTime);
            Rotate();

        }
        public override void Exit()
        {
            if (owner.Animator != null)
            {
                owner.Animator.SetBool("Walk", false);
            }
        }
        //回転処理
        private void Rotate()
        {
            //回転する角度の取得
            float dot = Vector3.Dot(owner.transform.forward, flatmoveDir);
            float rotateAng = Mathf.Acos(dot) * Mathf.Rad2Deg;

            if(rotateAng > 0.1f)
            {
                CheckCrossRotate(rotateAng);
            }
        }
        //外積判定とそれによって回転の向きを変える
        private void CheckCrossRotate(float rotateAng)
        {
            Vector3 crossVec = Vector3.Cross(owner.transform.forward, flatmoveDir);
            crossVec.Normalize();
            if (rotateAng >= 5)
            {
                rotateAng = 5;
            }
            //外積の判定
            if (crossVec.y < -0.9f)
            {
                rotateAng = -rotateAng;
            }
            owner.transform.Rotate(new Vector3(0, rotateAng, 0));
        }
    }
    //ステート：追跡
    private class StateChase : State<Enemy>
    {
        public StateChase(Enemy owner) : base(owner) { }

        //平行進行方向ベクトル
        private Vector3 flatmoveDir;
        //最終進行方向ベクトル
        private Vector3 moveDir;

        public override void Enter()
        {
            if (owner.Animator != null)
            {
                owner.Animator.SetBool("Run", true);
            }
            owner.Speed = 1.5f;
        }
        public override void Execute()
        {
            Vector3 targetPositionXZ = Vector3.Scale(owner.targetPos, new Vector3(1, 0, 1));
            Vector3 myPositionXZ = Vector3.Scale(owner.transform.position, new Vector3(1, 0, 1));
            //進行方向取得
            moveDir =targetPositionXZ - myPositionXZ;

            if (moveDir.sqrMagnitude <= 1.0f)
            {
                owner.ChangeState(EnemyState.Attack);
                return;
            }

            flatmoveDir = targetPositionXZ - myPositionXZ;
            flatmoveDir.Normalize();
            moveDir = flatmoveDir;
            if (!owner.CharacterController.isGrounded)
                moveDir.y = -5f;
            moveDir *= owner.Speed;
            owner.CharacterController.Move(moveDir * Time.deltaTime);
            Rotate();
        }
        public override void Exit()
        {
            if (owner.Animator != null)
            {
                owner.Animator.SetBool("Run", false);
            }
        }
        //回転処理
        private void Rotate()
        {
            //回転角度を取得
            float dot = Vector3.Dot(owner.transform.forward, flatmoveDir);
            float rotateAng = Mathf.Acos(dot) * Mathf.Rad2Deg;

            if (rotateAng > 0.1f)
            {
                CheckCrossRotate(rotateAng);  
            }
        }
        //外積判定とそれによって回転の向きを変える
        private void CheckCrossRotate(float rotateAng)
        {
            Vector3 crossVec = Vector3.Cross(owner.transform.forward, flatmoveDir);
            crossVec.Normalize();
            if (rotateAng >= 5)
            {
                rotateAng = 5;
            }
            //外積の判定
            if (crossVec.y < -0.9f)
            {
                rotateAng = -rotateAng;
            }
            owner.transform.Rotate(new Vector3(0, rotateAng, 0));
        }
    }
    //ステート：攻撃
    private class StateAttack : State<Enemy>
    {
        public StateAttack(Enemy owner) : base(owner) { }

        private Vector3 moveDir;
        private Weapon weapon = null;
        public override void Enter()
        {
            //ワンチャン重い
            weapon = owner.GetComponentInChildren<Weapon>();
            if (weapon != null)
            {
                weapon.OnAttack = true;
            }
            if (owner.Animator != null)
            {
                owner.Animator.SetBool("Attack", true);
            }
            owner.Speed = 1.0f;
        }
        public override void Execute()
        {
            Vector3 targetPositionXZ = Vector3.Scale(owner.targetPos, new Vector3(1, 0, 1));
            Vector3 myPositionXZ = Vector3.Scale(owner.transform.position, new Vector3(1, 0, 1));
            //進行方向取得
            moveDir = targetPositionXZ - myPositionXZ;
            if (moveDir.sqrMagnitude > 1.0f)
            {
                owner.ChangeState(EnemyState.Chase);
                return;
            }
            moveDir.Normalize();
            moveDir *= owner.Speed;
            owner.CharacterController.Move(moveDir * Time.deltaTime);
        }
        public override void Exit()
        {
            if (weapon != null)
            {
                weapon.OnAttack = false;
            }
            if (owner.Animator != null)
            {
                owner.Animator.SetBool("Attack",false);
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
