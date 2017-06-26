using UnityEngine;
using System.Collections;

public class test : MonoBehaviour {

    public GameObject player_body;      //玩家身體
    public GameObject player_head;      //玩家頭部
    public GameObject bulletSpawn;      //子彈生成位置
    public GameObject bulletobject;     //子彈
    public GameObject Bar;              //血條框
    public GameObject HealthBar;        //綠色血條
    public GameObject MainCamera;       //玩家攝影機

    public int initialposition;
    private Vector3 player_target;      //玩家部位
    private Vector3 destination;        //目的地
    private float max_Health;           //初始目標血條
    private float cur_Health;           //現在目標血條
    private float nextFire;             //下一秒開火時間
    private float fireRate;             //間隔開火時間
    private float calc_Health;          //計算扣血量
    private float dyna_Health;          //動態血量
    private bool shooting;              //正在射擊
    private bool moving;               //正在移動
    private bool death_count;           //數量-1
    private Animator anim;              //動畫
    private NavMeshAgent man;           //導航


	// Use this for initialization
	void Start () {
        max_Health = 100f;                              //初始目標血條
        cur_Health = 0f;                                //現在目標血條
        cur_Health = max_Health;                        //血量初始化
        dyna_Health = max_Health;                       //血量初始化
        nextFire = 0f;                                  //下一秒開火時間
        fireRate = 2.0f;                                //間隔開火時間
        calc_Health = 0f;                               //計算扣血量
        shooting = true;                               //正在射擊
        moving = false;                                 //正在移動
        death_count = false;                            //數量-1

        MainCamera = GameObject.Find("Main Camera");    //設定玩家攝影機
        player_body = GameObject.Find("Player_Body");   //設定玩家身體
        player_head = GameObject.Find("Player_HeadCenter");   //設定玩家頭部
        player_target = new Vector3(0, 0, 0);            //設定玩家部位
        anim = GetComponent<Animator>();                //動畫
        man = GetComponent<NavMeshAgent>();             //取得導航系統
	}
	
	// Update is called once per frame
	void Update () {
        /**** 當動態血量大於現在血量將做扣血的動畫 ****/
        if (dyna_Health > cur_Health)
        {
            dyna_Health--;
            calc_Health = dyna_Health / max_Health; // 比例 0 - 1 之間
            setHealthBar(calc_Health);

            /**** 血歸零刪除 ****/
            if (dyna_Health == 0)
            {
                Destroy(this.gameObject, 1.0f);
            }
        }

        HealthBar.transform.position = new Vector3(transform.position.x, transform.position.y + 4.0f, transform.position.z);    //讓血條一直處於的頭頂某處
        Vector3 HealthBarV = new Vector3(transform.position.x - MainCamera.transform.position.x, transform.position.y, transform.position.z - MainCamera.transform.position.z);  //紀錄標靶與玩家之間的向量
        Quaternion rotation = Quaternion.LookRotation(HealthBarV);  //計算面相玩家向量的角度
        HealthBar.transform.rotation = rotation;    //讓血條一直面向攝影機。由於攝影機是以人物為目標，所以v應該為人物的位置到攝影機位置的向量，否則會出現偏差。

        //////////////////////////////////////////////////////////////////////////////////////////////////////////


        if (cur_Health == 0) //死亡時做死亡動作
        {
            man.Stop();
            anim.Play("assault_death_A");
            if (!death_count)
            {
                GameSystem.target_Num -= 1;
                if (GameSystem.isGameing)
                {
                    GameSystem.kill_Num += 1;
                }
                death_count = true;
            }
        }
        else
        {
           
            {
                if (GameSystem.isGameing)
                {
                    /*  0.1機率暴頭  */
                    int Randomnum = Random.Range(0, 11);
                    Randomnum = 0;
                    if (Randomnum == 0)
                    {
                        player_target = player_head.transform.position - bulletSpawn.transform.position;
                    }
                    else
                    {
                        /*射擊誤差*/
                        Vector3 randomVector3 = new Vector3(Random.Range(-1.5F, 1.5F), 0, Random.Range(-1.5F, 1.5F));
                        /*敵人與玩家的距離*/
                        player_target = randomVector3 + player_body.transform.position - bulletSpawn.transform.position;
                    }



                    /*敵人瞄準玩家的線*/
                    //Debug.DrawRay(bulletSpawn.transform.position, playerDirect , Color.red);
                    Debug.DrawRay(bulletSpawn.transform.position, (player_target), Color.red);

                    /*敵人永遠面向玩家*/
                    Vector3 EnemyV = new Vector3(player_body.transform.position.x - transform.position.x, 0, player_body.transform.position.z - transform.position.z);
                    Quaternion EnemyR = Quaternion.LookRotation(EnemyV);
                    transform.rotation = EnemyR;


                    /*每兩秒自動射擊*/
                    if (Time.time > nextFire && shooting)
                    {
                        nextFire = Time.time + fireRate;

                        /*子彈生成*/
                        Bullet bullet = Bullet.Create(bulletobject, bulletSpawn.transform.position, Quaternion.LookRotation(player_target), 2);
                        bullet.transform.rotation = Quaternion.Euler(bullet.transform.eulerAngles + new Vector3(90, 0, 0));
                        //GameObject bullet = Instantiate(bulletobject, bulletSpawn.transform.position, Quaternion.LookRotation(player_target)) as GameObject;
                        //bullet.transform.rotation = Quaternion.Euler(bullet.transform.eulerAngles + new Vector3(90, 0, 0));

                        Rigidbody rb = bullet.GetComponent<Rigidbody>();
                        rb.velocity = Vector3.Normalize(player_target) * 10;

                        anim.Play("assault_combat_shoot_shotgun");
                    }
                }
            }


        }

	}
    /****計算血條****/
    public void setHealthBar(float myHealth)
    {
        //myHealth = 0 - 1 
        Bar.transform.localScale = new Vector3(myHealth, Bar.transform.localScale.y, Bar.transform.localScale.z);   //拉動綠色血條
    }

    /****扣血****/
    public void SetDamage(int damage)
    {
        cur_Health -= damage;

        //當血小於0時,不再做扣血的動作
        if (cur_Health <= 0)
        {

            cur_Health = 0;
        }
        if (GameSystem.isGameing)
        {
            GameSystem.hit_Num += 1.0f;
        }
    }
    void Shoot()
    {
        shooting = true;
    }
}
