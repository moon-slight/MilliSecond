using UnityEngine;
using System.Collections;

public class AIEnemy : MonoBehaviour
{

    public GameObject player;       //玩家
    public GameObject bulletSpawn;  //子彈生成位置
    public GameObject bulletobject; //子彈

    public float max_Health = 100f; //初始目標血條
    public float cur_Health = 0f;   //現在目標血條
    public GameObject Bar;          //血條框
    public GameObject HealthBar;    //綠色血條
    public GameObject MainCamera;   //玩家攝影機

    private float nextFire = 2.0f;  //間隔開火時間
    private float calc_Health = 0f; //計算扣血量
    private float dyna_Health = 0f; //動態血量

    private NavMeshAgent man;
    public Transform target;

    // Use this for initialization
    void Start()
    {
        man = gameObject.GetComponent<NavMeshAgent>();

        cur_Health = max_Health;                        //血量初始化
        dyna_Health = max_Health;                       //血量初始化
        MainCamera = GameObject.Find("Main Camera");    //設定玩家攝影機
    }

    // Update is called once per frame
    void Update()
    {

        man.SetDestination(target.position);

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
        //Vector3 v=transform.position-MainCamera.transform.position;
        Vector3 v = new Vector3(transform.position.x - MainCamera.transform.position.x, transform.position.y, transform.position.z - MainCamera.transform.position.z);  //紀錄標靶與玩家之間的向量
        Quaternion rotation = Quaternion.LookRotation(v);  //計算面相玩家向量的角度
        HealthBar.transform.rotation = rotation;    //讓血條一直面向攝影機。由於攝影機是以人物為目標，所以v應該為人物的位置到攝影機位置的向量，否則會出現偏差。

        //////////////////////////////////////////////////////////////////////////////////////////////////////////

        /*射擊誤差*/
        Vector3 randomVector3 = new Vector3(Random.Range(-0.5F, 0.5F), Random.Range(-0.5F, 0.5F), 0);

        /*敵人與玩家的距離*/
        Vector3 playerDirect = randomVector3 + player.transform.position - bulletSpawn.transform.position;

        /*敵人瞄準玩家的線*/
        //Debug.DrawRay(bulletSpawn.transform.position, playerDirect , Color.red);
        Debug.DrawRay(bulletSpawn.transform.position, (playerDirect), Color.red);

        /*敵人永遠面向玩家*/
        Vector3 v2 = new Vector3(-(transform.position.x - player.transform.position.x), 0, -(transform.position.z - player.transform.position.z));
        Quaternion rotation2 = Quaternion.LookRotation(v2);
        transform.rotation = rotation2;

        /*槍口YZ方向與玩家YZ方向的向量夾角*/
        float angleXZ = Vector2.Angle(new Vector2(bulletSpawn.transform.forward.x, bulletSpawn.transform.forward.z), new Vector2(playerDirect.x, playerDirect.z));
        /*槍口YZ方向與玩家YZ方向的向量夾角*/
        float angleYZ = Vector2.Angle(new Vector2(bulletSpawn.transform.forward.y, bulletSpawn.transform.forward.z), new Vector2(playerDirect.y, playerDirect.z));
        if (playerDirect.x < 0)
        {
            angleXZ = -angleXZ;
        }
        if (playerDirect.y > 0)
        {
            angleYZ = -angleYZ;
        }

        /*每兩秒自動射擊*/
        if (Time.time > nextFire)
        {
            nextFire += 2.0f;

            /*子彈生成*/
            GameObject bullet = Instantiate(bulletobject, bulletSpawn.transform.position, Quaternion.Euler(bulletSpawn.transform.eulerAngles + new Vector3(90 + angleYZ, -angleXZ, 0))) as GameObject;
            Rigidbody rb = bullet.GetComponent<Rigidbody>();
            rb.velocity = Vector3.Normalize(playerDirect) * 10;


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
    }
}
