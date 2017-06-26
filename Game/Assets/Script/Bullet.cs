using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour
{

    public int ID;              // 1代表玩家的子彈 2代表敵人子彈

    private bool Detect;
    private Rigidbody rb;      // 子彈鋼體

    public static Bullet Create(GameObject prefab, Vector3 createposition, Quaternion euler, int ID)
    {
        GameObject BulletSprite = (GameObject)Instantiate(prefab, createposition, euler);
        Bullet bullet = BulletSprite.GetComponent<Bullet>();
        bullet.ID = ID;
        bullet.Detect = false;
        return bullet;
    }

    // Use this for initialization
    void Start()
    {
        rb = GetComponent<Rigidbody>();  //取得子彈的鋼體
    }

    // Update is called once per frame
    void Update()
    {

    }
    void OnCollisionEnter(Collision collision)
    {

        /**** 讀取玩家物件 ****/
        Player P = collision.transform.GetComponent<Player>();
        if (P != null)
        {
            /**** 命中紅心 ****/
            if (collision.collider.name == "Player_Head" && ID == 2)
            {
                Debug.Log("Hit Player_Head");
                P.SetDamage(100);

            }
            /**** 命中其他部位 ****/
            if (collision.collider.name == "Player_Body" && ID == 2)
            {
                Debug.Log("Hit Player_Body");
                P.SetDamage(10);
            }
        }

        if (Detect)
        {
            Player.InSlowMotionBullet -= 1;
        }

        Destroy(this.gameObject);   //子彈撞到任意東西立刻消除


    }
    void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.name == "DetectArea" && ID != 1)
        {
            Detect = true;
            Player.InSlowMotionBullet += 1;
            rb.velocity = rb.velocity / 8;    //設定子彈速度
        }

    }
    void OnTriggerExit(Collider collision)
    {
        if (collision.gameObject.name == "DetectArea" && ID != 1)
        {
            Detect = false;
            Player.InSlowMotionBullet -= 1;
            rb.velocity = rb.velocity * 8;
        }
    }
}
