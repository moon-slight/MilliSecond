using UnityEngine;
using System.Collections;

public class RunSoldier : MonoBehaviour {

    public GameObject Bar;              //血條框
    public GameObject HealthBar;        //綠色血條
    public GameObject MainCamera;       //玩家攝影機


    private float max_Health;           //初始目標血條
    private float cur_Health;           //現在目標血條
    private float dyna_Health;          //動態血量
    private float calc_Health;          //計算扣血量
    private Vector3 destination;        //目的地
    private bool death_count;           //數量-1
    private Animator anim;              //動畫
    private NavMeshAgent man;           //導航


    /**** 生產士兵 ****/
    public static void Create(GameObject prefab, Vector3 createposition, Vector3 destination )
    {
        /**** 生成物件 ****/
        GameObject SoldierSprite = (GameObject)Instantiate(prefab, createposition, Quaternion.identity);
        RunSoldier soldier = SoldierSprite.GetComponent<RunSoldier>();
        soldier.destination = destination;
    }

	// Use this for initialization
	void Start () {
        max_Health = 100f;                              //初始目標血條
        cur_Health = 0f;                                //現在目標血條
        cur_Health = max_Health;                        //血量初始化
        dyna_Health = max_Health;                       //血量初始化
        death_count = false;                            //數量-1
        MainCamera = GameObject.Find("Main Camera");    //設定玩家攝影機
        anim = GetComponent<Animator>();                //動畫
        man = GetComponent<NavMeshAgent>();             //取得導航系統

        man.SetDestination(destination);
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
            anim.Play("rocketlauncher_death_B");
            if (!death_count)
            {
                RunSoldierSpawn.RunSoldier_Num -= 1;
                if (GameSystem.isGameing)
                {
                    GameSystem.kill_Num += 1;
                    GameSystem.RunSoldier_count += 1;
                }
                death_count = true;
            }
        }
        else if (transform.position.x == destination.x && transform.position.z == destination.z)
        {
            Destroy(this.gameObject);
            RunSoldierSpawn.RunSoldier_Num -= 1;
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
}
