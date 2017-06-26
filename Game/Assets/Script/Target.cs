using UnityEngine;
using System.Collections;

public class Target : MonoBehaviour
{
    public GameObject Bar;                  //  血條框
    public GameObject HealthBar;            //  綠色血條
    public GameObject MainCamera;           //  玩家攝影機

    private float max_Health;               //  初始目標血條
    private float cur_Health;               //  現在目標血條
    private float calc_Health;              //  計算扣血量
    private float dyna_Health;              //  動態血量
    private Vector3 origion_rotation;       //  原先角度
    private bool death_count;               //  數量-1

    private bool Auto;                       //  是否自動移動
    private bool Back;                      //  回頭
    private Vector3 Destination;            //  移動目的地
    private float MoveSpeed;                //  移動速度
    



    //**** 生產標靶 ****/
    public static void Create(GameObject prefab, Vector3 createposition, float movespeed, bool auto )
    {
        /**** 生成物件 ****/
        GameObject TargetSprite = (GameObject)Instantiate(prefab, createposition, Quaternion.identity);
        Target target = TargetSprite.GetComponent<Target>();
        target.Auto = auto;
        target.MoveSpeed = movespeed;                                 
    }

    // Use this for initialization
    void Start()
    {
        MainCamera = GameObject.Find("Main Camera");    //設定玩家攝影機

        max_Health = 100f;                              //初始目標血條
        cur_Health = 0f;                                //現在目標血條
        calc_Health = 0f;                               //計算扣血量
        cur_Health = max_Health;                        //血量初始化
        dyna_Health = max_Health;                       //血量初始化
        origion_rotation = this.transform.eulerAngles;  //紀錄原先角度
        death_count = false;                            //數量-1
        Destination = transform.position + new Vector3(30f,0f,0f);      //移動目的地;
        
        Back = false;                                   //回頭
    }

    // Update is called once per frame
    void Update()
    {
        
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

        //HealthBar.transform.position = new Vector3(transform.position.x, transform.position.y + 4.0f, transform.position.z);    //讓血條一直處於的頭頂某處
        Vector3 v = new Vector3(transform.position.x - MainCamera.transform.position.x, transform.position.y, transform.position.z - MainCamera.transform.position.z);  //紀錄標靶與玩家之間的向量
        Quaternion rotation = Quaternion.LookRotation(v);  //計算面相玩家向量的角度
        if (cur_Health != 0)
        {
            HealthBar.transform.rotation = rotation;    //讓血條一直面向攝影機。由於攝影機是以人物為目標，所以v應該為人物的位置到攝影機位置的向量，否則會出現偏差。

            /**** 移動 ****/
            if (Auto)
            {
                transform.position = Vector3.MoveTowards(transform.position, Destination, MoveSpeed * Time.deltaTime);
                if (transform.position == Destination)
                {
                    Back = !Back;
                    if (Back)
                    {
                        Destination = transform.position - new Vector3(30f, 0f, 0f);
                    }
                    else
                    {
                        Destination = transform.position + new Vector3(30f, 0f, 0f);
                    }
                }
            }
        }
        if (cur_Health == 0)
        {
            transform.rotation = Quaternion.Slerp(this.transform.rotation, Quaternion.Euler(origion_rotation + new Vector3(90, 0, 0)), 0.1f);   //90度倒下
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
            HealthBar.transform.rotation = Quaternion.identity;
            cur_Health = 0;
        }

        if (GameSystem.isGameing)
        {
            GameSystem.hit_Num += 1.0f;
        }
    }


}
