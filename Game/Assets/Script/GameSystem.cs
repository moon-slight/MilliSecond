using UnityEngine;
using UnityEngine.UI;
using System.Collections;

using System.Text;
using System;
using WiimoteApi;

public class GameSystem : MonoBehaviour
{
    static public Wiimote wiimote;                  // Wiimote手把
    static public int target_Num;                   // 標靶數量
    static public int kill_Num;                     // 擊殺數量
    static public int RunSoldier_count;             // 跑步士兵數量
    static public int HeadShot_count;               // 暴頭數   
    static public float hit_Num;                    // 命中數量
    static public float bullet_Num;                 // 發射數量
    static public int game_Mode = 1;                  // 遊戲模式
    static public bool isGameing;                   // 遊戲開始
    static public int Minute;                       // 遊戲分鐘
    static public float Second;                     // 遊戲秒數
    static public bool slowMotion;                  // 慢動作偵測
    static public int bulletInDetectAreaNum;        // 偵測子彈數
    static public float slowSpeed;                  // 慢動作速度
    static public bool endGame;                     // 遊戲結束
    static public int HighScore = 0;                // 記錄遊戲最高分數

    public Text HighScore_Text;             // 最高分文字
    public Text Score_Text;                 // 分數文字
    public Text Ready_Text;                 // 準備文字
    public Text StartCount_Text;            // 遊戲開始倒數文字
    public Text GameOver_Text;              // 遊戲結束文字
    public Text Counter;                    // 計時器文字
    public Text Minute_Text;                // 分鐘文字
    public Text Second_Text;                // 秒文字
    public Text MilliSecond_Text;           // 毫秒文字;
    public Text E_count;                    // 顯示敵人數量
    public Text KillNum_Text;               // 顯示擊殺數量
    public Text RunSoldier_Text;            // 顯示跑步士兵數量
    public Text HeadShot_Text;              // 顯示暴頭數量
    public Text HitRate_Text;               // 顯示命中率
    public Text Total_Text;                 // 顯示結算成績
    public GameObject player;               // 玩家
    public GameObject wiimoteText;          // 文字
    public GameObject startWiimoteButton;   // 按鈕
    public GameObject pausePanel;           // 暫停屏幕
    public GameObject SettlementPanel;      // 結算畫面

    private bool isPaused;                  // 遊戲暫停
    private int GameStartCount;             // 倒數秒數
    private int TotalScore;                 // 總分
    private AudioSource Music;              // 背景音樂
    private AudioClip Clip1;                // 背景音樂1
    private AudioClip Clip2;                // 背景音樂2
    private bool MusicChange;               // 切換背景音樂


    // Use this for initialization
    void Start()
    {

        target_Num = 0;                                                                 // 標靶數量
        kill_Num = 0;                                                                   // 擊殺數量
        RunSoldier_count = 0;                                                           // 跑步士兵數量
        HeadShot_count = 0;                                                             // 爆頭數
        hit_Num = 0.0f;                                                                 // 命中數量
        bullet_Num = 0.0f;                                                              // 發射數量
        slowMotion = false;                                                             // 偵測慢動作
        bulletInDetectAreaNum = 0;                                                      // 偵測子彈數
        slowSpeed = 0.01f;                                                              // 慢動作速度

        if (game_Mode == 1)
        {
            HighScore = PlayerPrefs.GetInt("PracticeHighScore", 0);
        }
        if (game_Mode == 2)
        {
            HighScore = PlayerPrefs.GetInt("AIEnemyHighScore", 0);
        }
        isGameing = false;                                                              // 遊戲開始
        Minute = 3;                                                                     // 遊戲分鐘
        Second = 0.0f;                                                                  // 遊戲秒數
        GameStartCount = 3;                                                             // 倒數秒數
        TotalScore = 0;                                                                 // 總分;
        isPaused = false;                                                               // 遊戲暫停
        endGame = false;                                                                // 遊戲結束
        startWiimoteButton.SetActive(false);
        Music = GetComponent<AudioSource>();
        Clip1 = Resources.Load<AudioClip>("Sounds/SRW Impact - Shine in the Storm");    // 背景音樂1
        Clip2 = Resources.Load<AudioClip>("Sounds/GundamOO_awake");                     // 背景音樂2
        Music.clip = Clip1;
        MusicChange = false;                                                            // 切換背景音樂
        Invoke("GameReady", 3.0f);                                                      // 三秒後顯示文字

    }

    // Update is called once per frame
    void Update()
    {


        //Debug.Log("Kill : " + kill_Num);
        //Debug.Log("Special: " + RunSoldier_count);
        //Debug.Log("HeadShot: " + HeadShot_count);
        //Debug.Log("Hit : " + hit_Num);
        //Debug.Log("Bullet: " + bullet_Num);

        TotalScore = kill_Num * 10 + RunSoldier_count * 50 + HeadShot_count * 30;
        if (TotalScore > HighScore)
        {
            HighScore = TotalScore;
        }
        HighScore_Text.text = "HighScore : " + HighScore;
        Score_Text.text = "Score : " + TotalScore;

        if (game_Mode == 1 || game_Mode == 2)
        {
            /*  顯示螢幕文字  */
            E_count.text = "Enemy : " + (GameSystem.target_Num + RunSoldierSpawn.RunSoldier_Num);
            if (isGameing)
            {
                if (Minute == 0)
                {
                    Counter.color = new Color(255, 0, 0, 255);
                    Minute_Text.color = new Color(255, 0, 0, 255);
                    Second_Text.color = new Color(255, 0, 0, 255);
                    MilliSecond_Text.color = new Color(255, 0, 0, 255);
                    if (!MusicChange)
                    {
                        Music.clip = Clip2;
                        Music.time = 60;
                        Music.Play();
                        MusicChange = true;
                    }
                }
                Counter.enabled = true;
                Minute_Text.enabled = true;
                Second_Text.enabled = true;
                MilliSecond_Text.enabled = true;
                if (Minute == 0 && Second == 0)
                {
                    Minute_Text.text = "0";
                    Second_Text.text = "00";
                    MilliSecond_Text.text = "00";
                }
                else if (Second < 10)
                {
                    Minute_Text.text = "" + Minute;
                    Second_Text.text = "0" + Second.ToString("0.00").Substring(0, 1);
                    MilliSecond_Text.text = "" + Second.ToString("0.00").Substring(2, 2);
                }
                else
                {
                    Minute_Text.text = "" + Minute;
                    Second_Text.text = "" + Second.ToString("0.00").Substring(0, 2);
                    MilliSecond_Text.text = "" + Second.ToString("0.00").Substring(3, 2);
                }
            }
        }


        /*  按P暫停遊戲  */
        if (Input.GetKeyDown("p"))
        {
            SwitchPause();
        }


        if (isPaused)
        {
            PauseGame(true);
            wiimoteText.GetComponent<Text>().text = "Wiimote Found : " + WiimoteManager.HasWiimote();

            /*  顯示啟動Wiimote按鈕   */
            if (WiimoteManager.HasWiimote())
            {
                startWiimoteButton.SetActive(true);
            }
            else
            {
                startWiimoteButton.SetActive(false);
            }
        }
        else
        {
            PauseGame(false);
        }

        if (endGame)
        {
            isGameing = false;
            GameOver_Text.enabled = true;
            GameOver_Text.text = "Game Over";
            Invoke("GameEnd", 3.0f);          // 三秒後顯示結算畫面
            endGame = false;
        }
        /*讀取 Wiimote Data 資料*/
        if (!WiimoteManager.HasWiimote()) { return; }
        wiimote = WiimoteManager.Wiimotes[0];

    }

    /*  遊戲暫停/開始   */
    void PauseGame(bool state)
    {
        if (state)
        {
            player.GetComponent<Player>().enabled = false;
            Time.timeScale = 0.0f;
        }
        else
        {
            player.GetComponent<Player>().enabled = true;
            Time.timeScale = 1.0f;
        }

        pausePanel.SetActive(state);
    }

    /*  判斷Pause */
    public void SwitchPause()
    {
        if (isPaused)
        {
            Music.Play();
            isPaused = false;
        }
        else
        {
            Music.Pause();
            isPaused = true;
        }
    }

    /*  偵測Wiimote   */
    public void FindWiimote()
    {
        WiimoteManager.FindWiimotes();

    }

    /*  啟動Wiimote   */
    public void StartWiimote()
    {
        wiimote.SetupIRCamera(IRDataType.BASIC);

    }

    /*  清除Wiimote   */
    public void CleanUpWiimote()
    {
        WiimoteManager.Cleanup(wiimote);
        wiimote = null;
    }

    /*  回主畫面    */
    public void GoBackToHomePage()
    {
        Application.LoadLevel("home page");
    }

    /*  遊戲準備  */
    void GameReady()
    {

        Ready_Text.enabled = true;              //顯示文字
        Ready_Text.text = "Are you ready ?";
        Invoke("PrintCounter", 2f);             //兩秒後顯示倒數秒數

        InvokeRepeating("GameStart", 3f, 1f);   //三秒後倒數

    }

    /*  遊戲開始  */
    void GameStart()
    {
        if (GameStartCount == 0 /*&& game_Mode == 1*/)
        {
            CancelInvoke("GameStart");
            InvokeRepeating("TimerCounting", 0f, 0.01f);
            PrintCounter();
            Ready_Text.enabled = false;
            isGameing = true;
            Music.enabled = true;
        }
        else
        {
            GameStartCount -= 1;
            StartCount_Text.fontSize = 90;
            InvokeRepeating("PrintText", 0f, 0.1f);
        }
    }
    void GameEnd()
    {
        CancelInvoke("TimerCounting");
        GameOver_Text.enabled = false;
        SettlementPanel.SetActive(true);
        float hit_Rate = 0.0f;
        if (bullet_Num != 0)
        {
            hit_Rate = hit_Num / bullet_Num * 100;
        }
        KillNum_Text.text = "" + kill_Num;
        RunSoldier_Text.text = "" + RunSoldier_count;
        HeadShot_Text.text = "" + HeadShot_count;
        HitRate_Text.text = hit_Rate.ToString("0.00") + "%";


        Total_Text.text = "TOTAL      " + TotalScore;
        if (game_Mode == 1)
        {
            PlayerPrefs.SetInt("PracticeHighScore", HighScore);
        }
        if (game_Mode == 2)
        {
            PlayerPrefs.SetInt("AIEnemyHighScore", HighScore);
        }
    }

    /*  時間計時  */
    void TimerCounting()
    {
        if (Minute <= 0 && Second <= 0.0f)
        {

            CancelInvoke("TimerCounting");
            Minute = 0;
            Second = 0.00f;

            Minute_Text.text = "0";
            Second_Text.text = "00";
            MilliSecond_Text.text = "00";
            endGame = true;
        }
        else
        {
            if (Second <= 0.00f)
            {
                Minute -= 1;
                Second = 60.0f;
            }
            Second -= 0.01f;
        }



    }

    /*  顯示開始倒數  */
    void PrintCounter()
    {
        if (!StartCount_Text.enabled)
        {
            StartCount_Text.enabled = true;      //顯示開始倒數
            InvokeRepeating("PrintText", 0f, 0.1f);
        }
        else
        {
            StartCount_Text.enabled = false;     //消失開始倒數
        }
    }

    /*  秒數放大  */
    void PrintText()
    {
        StartCount_Text.fontSize += 2;
        if (GameStartCount == 0)
        {
            StartCount_Text.text = "START";
        }
        else
        {
            StartCount_Text.text = GameStartCount.ToString();
        }

        if (StartCount_Text.fontSize == 120)
        {
            CancelInvoke("PrintText");
        }
    }

}

