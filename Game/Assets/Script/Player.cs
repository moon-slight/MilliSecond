using UnityEngine;
using System.Collections;

using UnityEngine.UI;
using System.Collections;
using System.Text;
using System;
using WiimoteApi;

public class Player : MonoBehaviour
{
    static public Vector3[] moving_position;        //移動座標點
    static public int Move_Total_Position = 11;     //總共玩家移動位置
    static public int InSlowMotionBullet;           //在慢動作下的子彈
    static public bool OutOfRange;                  //是否超出感測範圍

    public Text P_health;                           //顯示玩家血量
    public Text B_num;                              //顯示玩家子彈數量

    public GameObject _camera;
    public GameObject el_def;                       //人物模組眼睛位置處
    public GameObject ir_dot_canvas;                //瞄準屏幕
    public GameObject gun;                          //槍枝
    public GameObject sniper;                       //狙擊鏡
    public GameObject bulletSpawn;                  //未狙擊鏡子彈生成位置
    public GameObject snipeBulletSpawn;             //狙擊鏡子彈生成位置
    public GameObject bulletobject;                 //子彈物件
    public GameObject dot_5;                        //綠點
    public GameObject panel;                        //屏幕
    public GameObject WiimoteDetect_Text;              //超出感測範圍文字
    public RectTransform[] ir_dots;
    public RectTransform[] ir_bb;
    public RectTransform ir_pointer;
    public RectTransform IR_Dot_Canvas;

    public ParticleEmitter fire;             //開槍時的火焰
    public AudioClip gunshot;               //開槍時的音效
    public AudioClip nobullet;              //沒子彈的音效
    public AudioClip changebullet;          //換子彈的音效
    public AudioClip BoltBullet;            //上膛音效
    public AudioClip HeadShot;              //暴頭音效
    public GameObject shield;
    public GameObject Revise;
    public GameObject HurtPanel;                            //扣寫畫面
    public GameObject BeingDeathPanel;                      //快死亡畫面
    public GameObject SlowMotionPanel;                      //慢動作畫面


    private int playerHealth;                               //玩家血量
    private int cur_Health;                                 //現在玩家血量
    private int bullet_Num;                                 //子彈數量
    private int current_position;                           //玩家目前位置
    private float fireRate;                                 //子彈射速間隔
    private float nextFire;                                 //距離上一秒子彈時間
    private float moveSpeed;                                //移動速度
    private float turnSpeed;                                //旋轉速度
    private Transform m_camTransform;                       //攝影機

    private bool bullet_Changing = false;                   //正在更換子彈 
    private Vector3 target;                                 //移動位置目標
    private Vector3 temp_ir_dot_canvaslocalposition;        //暫存瞄準屏幕位置
    private bool snipering;                                 //開啟狙擊鏡
    private Vector2 pre_ir_pointer_Min;                     // 紀錄上一個Frame的綠點位置
    private Vector2 pre_ir_pointer_Max;                     // 紀錄上一個Frame的綠點位置
    private float vol;                                      // 音效聲音大小
    private AudioSource a_source;                           // 開槍音效
    private bool BeingDeath;                                // 玩家快死亡
    private bool BulletBolting;                             // 子彈正在上膛
    private Vector3 GunDirect ;                             // 相機指向綠點的的方向


    // Use this for initialization
    void Start()
    {
        moving_position = new Vector3[Move_Total_Position];
        PositionCreate();
        InSlowMotionBullet = 0;
        playerHealth = 100;                                             //玩家血量
        cur_Health = playerHealth;                                     //現在玩家血量
        bullet_Num = 10;                                                //子彈數量
        current_position = 5;                                           //玩家目前位置
        fireRate = 2.0F;                                                //子彈射速間隔
        nextFire = 0.0F;                                                //距離上一秒子彈時間
        moveSpeed = 10f;                                                //玩家移動速度
        m_camTransform = Camera.main.transform;                         //獲得攝影機
        target = new Vector3(0, 0, 0);                                  //移動位置目標
        temp_ir_dot_canvaslocalposition = IR_Dot_Canvas.localPosition;  //儲存一開始屏幕位置
        snipering = false;                                              //開啟狙擊鏡
        pre_ir_pointer_Min = new Vector2(0.5f, 0.5f);                   //初值為中間
        pre_ir_pointer_Max = new Vector2(0.5f, 0.5f);                   //初值為中間

        turnSpeed = 50f;                                                //玩家旋轉速度
        //fire = GetComponent<ParticleEmitter> ();
        fire.emit = false;                                              //初值為停止
        a_source = GetComponent<AudioSource>();
        vol = 1f;
        BeingDeath = false;                                             //玩家快死亡

        HurtPanel.GetComponent<Image>().canvasRenderer.SetAlpha(0.0f);  //扣寫透明度條0
        OutOfRange = false;
        BulletBolting = false;

        GunDirect = dot_5.transform.position - m_camTransform.position;  //綠點與視角間的方向

        InvokeRepeating("FlashingText", 0, 0.5f);
    }

    void FixedUpdate()
    {
        ReadWiimoteData();
    }

    // Update is called once per frame
    void Update()
    {
        DynamicBlood();

        DeathDetect();

        P_health.text = "HP : " + playerHealth; 

        SmiteDetect();

        if (cur_Health > 0)
        {
            /*玩家前後左右*/
            if (Input.GetKey("w"))
            {
                transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime * 1 / Time.timeScale);
            }
            if (Input.GetKey("s"))
            {
                transform.Translate(Vector3.back * moveSpeed * Time.deltaTime * 1 / Time.timeScale);
            }
            if (Input.GetKey("a"))
            {
                transform.Translate(Vector3.left * moveSpeed * Time.deltaTime * 1 / Time.timeScale);
            }
            if (Input.GetKey("d"))
            {
                transform.Translate(Vector3.right * moveSpeed * Time.deltaTime * 1 / Time.timeScale);
            }

            /*玩家旋轉視角*/
            if (Input.GetKey("q"))
            {
                transform.Rotate(Vector3.up, -turnSpeed * Time.deltaTime * 1 / Time.timeScale);
            }
            if (Input.GetKey("e"))
            {
                transform.Rotate(Vector3.up, turnSpeed * Time.deltaTime * 1 / Time.timeScale);
            }

            /* 換子彈 */
            if (GameSystem.wiimote.Nunchuck.z && !bullet_Changing && !BulletBolting)
            {
                a_source.PlayOneShot(changebullet, vol);
                bullet_Changing = true;
                Invoke("BulletChange", 2.0f);   //更換子彈2秒
            }
            /*玩家旋轉視角*/
            if (GameSystem.wiimote.Nunchuck.stick[0] < 100)
            {
                transform.Rotate(Vector3.up, -turnSpeed * Time.deltaTime * 1 / Time.timeScale);
                _camera.transform.Rotate(Vector3.up, -turnSpeed * Time.deltaTime * 1 / Time.timeScale);
                shield.transform.Rotate(Vector3.up, -turnSpeed * Time.deltaTime * 1 / Time.timeScale);
            }
            if (GameSystem.wiimote.Nunchuck.stick[0] > 146)
            {
                transform.Rotate(Vector3.up, turnSpeed * Time.deltaTime * 1 / Time.timeScale);
                _camera.transform.Rotate(Vector3.up, turnSpeed * Time.deltaTime * 1 / Time.timeScale);
                shield.transform.Rotate(Vector3.up, turnSpeed * Time.deltaTime * 1 / Time.timeScale);
            }

            GunDirect = dot_5.transform.position - m_camTransform.position;  //綠點與視角間的方向

            /****玩家狙擊鏡縮放****/
            if (GameSystem.wiimote.Nunchuck.c && !bullet_Changing && !BulletBolting && !Shield.onshield)
            {
                if (!snipering)
                {
                    ir_dot_canvas.transform.parent = _camera.transform;
                }

                Camera.main.fieldOfView = 10;

                /****狙擊鏡對準綠點****/
                m_camTransform.rotation = Quaternion.LookRotation(GunDirect);
                sniper.SetActive(true);
                gun.SetActive(false);
                snipering = true;
            }
            else
            {
                Camera.main.fieldOfView = 50;
                sniper.SetActive(false);

                /****狙擊鏡歸位****/
                if (snipering)
                {
                    ir_dot_canvas.transform.parent = m_camTransform;
                    m_camTransform.localRotation = Quaternion.identity;
                    ir_dot_canvas.transform.localRotation = Quaternion.identity;
                    IR_Dot_Canvas.localPosition = temp_ir_dot_canvaslocalposition;
                    snipering = false;
                    gun.SetActive(true);
                }
            }

            if (Time.time > nextFire)
            {
                BulletBolting = false;
            }

            /****射擊****/
            Debug.DrawRay(m_camTransform.position, Vector3.Normalize(GunDirect) * 1000f, Color.blue);

            gun.transform.rotation = Quaternion.LookRotation(GunDirect);    //槍枝跟著相機與綠點夾角旋轉

            if (GameSystem.wiimote.Button.b && !bullet_Changing && !BulletBolting && Shield.onshield == false)        //按下b鍵,射出子彈
            {
                Shoot();
            }
            else
            {
                fire.emit = false;
            }

            B_num.text = (bullet_Num + "/");
        }
        _camera.transform.position = Revise.transform.position;
    }

    

    /**** 扣血 ****/
    public void SetDamage(int damage)
    {
        HurtPanel.GetComponent<Image>().canvasRenderer.SetAlpha(1.0f);
        HurtPanel.GetComponent<Image>().CrossFadeAlpha(0.0f, 1.0f, false);
        cur_Health -= damage;
        if (cur_Health <= 0)
        {
            cur_Health = 0;
            if (GameSystem.isGameing)
            {
                GameSystem.endGame = true;
            }
        }
    }

    /**** 讀取WiimoteData ****/
    void ReadWiimoteData()
    {
        /**** Wiimote Code ****/
        if (!WiimoteManager.HasWiimote())
        {
            WiimoteDetect_Text.SetActive(true);
            WiimoteDetect_Text.GetComponent<Text>().text = "Wiimote Cloud Not Found";
            return;
        }
        else
        {
            WiimoteDetect_Text.SetActive(false);
            WiimoteDetect_Text.GetComponent<Text>().text = "Out Of Sensors Range";
        }

        int ret;
        do
        {
            ret = GameSystem.wiimote.ReadWiimoteData();
        } while (ret > 0);

        if (ir_dots.Length < 4) return;

        float[,] ir = GameSystem.wiimote.Ir.GetProbableSensorBarIR();
        for (int i = 0; i < 2; i++)
        {
            float x = (float)ir[i, 0] / 1023f;
            float y = (float)ir[i, 1] / 767f;
            if (x == -1 || y == -1)
            {
                ir_dots[i].anchorMin = new Vector2(0, 0);
                ir_dots[i].anchorMax = new Vector2(0, 0);
            }

            ir_dots[i].anchorMin = new Vector2(x, y);
            ir_dots[i].anchorMax = new Vector2(x, y);

            if (ir[i, 2] != -1)
            {
                int index = (int)ir[i, 2];
                float xmin = (float)GameSystem.wiimote.Ir.ir[index, 3] / 127f;
                float ymin = (float)GameSystem.wiimote.Ir.ir[index, 4] / 127f;
                float xmax = (float)GameSystem.wiimote.Ir.ir[index, 5] / 127f;
                float ymax = (float)GameSystem.wiimote.Ir.ir[index, 6] / 127f;
                ir_bb[i].anchorMin = new Vector2(xmin, ymin);
                ir_bb[i].anchorMax = new Vector2(xmax, ymax);
            }
        }

        float[] pointer = GameSystem.wiimote.Ir.GetPointingPosition();
        ir_pointer.anchorMin = new Vector2(pointer[0], pointer[1]);
        ir_pointer.anchorMax = new Vector2(pointer[0], pointer[1]);


        /**** 超出Wiimote SensorBar 感應範圍 定格****/
        if (ir_pointer.anchorMax == new Vector2(-1.0f, -1.0f) || ir_pointer.anchorMin == new Vector2(-1.0f, -1.0f))
        {
            WiimoteDetect_Text.SetActive(true);
            ir_pointer.anchorMax = pre_ir_pointer_Max;
            ir_pointer.anchorMin = pre_ir_pointer_Min;
        }
        else
        {
            WiimoteDetect_Text.SetActive(false);
        }

        /****   Smooth 線性插值     ****/
        ir_pointer.anchorMax = Vector2.Lerp(pre_ir_pointer_Max, ir_pointer.anchorMax, 0.2f);
        ir_pointer.anchorMin = Vector2.Lerp(pre_ir_pointer_Min, ir_pointer.anchorMin, 0.2f);

        /****   紀錄目前Frame的綠點值   ****/
        pre_ir_pointer_Max = new Vector2(ir_pointer.anchorMax.x, ir_pointer.anchorMax.y);
        pre_ir_pointer_Min = new Vector2(ir_pointer.anchorMin.x, ir_pointer.anchorMin.y);

        /*********************************************************/
    }

    /**** 偵測Wiimote ***/
    void FlashingText()
    {
        if (WiimoteDetect_Text.GetComponent<Text>().enabled)
            WiimoteDetect_Text.GetComponent<Text>().enabled = false;
        else
            WiimoteDetect_Text.GetComponent<Text>().enabled = true;
    }

    /**** 動態扣血 ****/
    void DynamicBlood()
    {
        /**** 玩家動態扣血 ****/
        if (playerHealth > cur_Health)
        {
            playerHealth--;
        }
    }

    /**** 爆頭偵測 ****/
    void SmiteDetect()
    {
        /**** 子彈爆頭偵測 ****/
        if (InSlowMotionBullet == 0)
        {
            SlowMotionPanel.SetActive(false);
        }
        else
        {
            SlowMotionPanel.SetActive(true);
        }
    }

    /**** 死亡偵測 ****/
    void DeathDetect()
    {
        /**** 血低於20顯示快死畫面 ****/
        if (cur_Health <= 20 && !BeingDeath)
        {
            BeingDeathPanel.SetActive(true);
            BeingDeath = true;
        }
    }

    /**** 子彈上膛 ****/
    void Bolting()
    {
        a_source.PlayOneShot(BoltBullet, vol);
    }

    /**** 更換子彈 ****/
    void BulletChange()
    {
        bullet_Num = 10;
        bullet_Changing = false;
    }

    /**** 射擊 ****/
    void Shoot()
    {
        if (bullet_Num > 0)
        {
            nextFire = Time.time + fireRate;    //設定子彈射出間隔

            fire.emit = true; ;
            a_source.PlayOneShot(gunshot, vol);

            RaycastHit Hit;
            Ray ShootRay = new Ray(m_camTransform.position, Vector3.Normalize(GunDirect));
            if (Physics.Raycast(ShootRay, out Hit, 1000f))
            {
                //Debug.Log(Hit.collider.name);
                Target T = Hit.transform.GetComponent<Target>();
                if (T != null)
                {
                    /**** 命中紅心 ****/
                    if (Hit.collider.name == "Red_point")
                    {
                        a_source.PlayOneShot(HeadShot, vol);
                        T.SetDamage(100);
                        GameSystem.HeadShot_count++;
                    }
                    else if (Hit.collider.name == "Plate")
                    {
                        T.SetDamage(20);
                    }
                    else if (Hit.collider.name == "Root")
                    {
                        T.SetDamage(20);
                    }
                }
                AISoldier S = Hit.transform.GetComponent<AISoldier>();
                if (S != null)
                {
                    /**** 命中紅心 ****/
                    if (Hit.collider.name == "AISoldier_Head")
                    {
                        a_source.PlayOneShot(HeadShot, vol);
                        S.SetDamage(100);
                        GameSystem.HeadShot_count++;
                    }
                    /**** 命中其他部位 ****/
                    if (Hit.collider.name == "AISoldier_Body")
                    {
                        S.SetDamage(50);
                    }
                }

                RunSoldier R = Hit.transform.GetComponent<RunSoldier>();
                if (R != null)
                {
                    /**** 命中紅心 ****/
                    if (Hit.collider.name == "RunSoldier_Head")
                    {
                        a_source.PlayOneShot(HeadShot, vol);
                        R.SetDamage(100);
                        GameSystem.HeadShot_count++;
                    }
                    /**** 命中其他部位 ****/
                    if (Hit.collider.name == "RunSoldier_Body")
                    {
                        R.SetDamage(50);
                    }
                }
            }
            BulletBolting = true;
            Invoke("Bolting", 1.0f);
            bullet_Num -= 1;
            if (GameSystem.isGameing)
            {
                GameSystem.bullet_Num += 1.0f;
            }
        }
        else
        {
            a_source.PlayOneShot(changebullet, vol);
            bullet_Changing = true;
            Invoke("BulletChange", 2.0f);   //更換子彈2秒
        }
    }

    /**** 玩家位置 ****/
    void PositionCreate()
    {
        moving_position[0] = new Vector3(93.14f, 0.03232598f, 87.5f);
        moving_position[1] = new Vector3(124.9f, 0.03232598f, 80.38f);
        moving_position[2] = new Vector3(142.81f, 0.03232598f, 75.77f);
        moving_position[3] = new Vector3(168.53f, 0.03232598f, 88.32f);
        moving_position[4] = new Vector3(190.23f, 0.03232598f, 88.84f);
        moving_position[5] = new Vector3(219.95f, 0.03232598f, 81.68f);
        moving_position[6] = new Vector3(254.85f, 0.03232598f, 65.49f);
        moving_position[7] = new Vector3(294.23f, 0.03232598f, 80.3f);
        moving_position[8] = new Vector3(324.21f, 0.03232598f, 75.28f);
        moving_position[9] = new Vector3(354.29f, 0.03232598f, 92.53f);
        moving_position[10] = new Vector3(382.47f, 0.03232598f, 93.05f);
    }
}
