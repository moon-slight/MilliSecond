using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using WiimoteApi;


public class HomePageSystem : MonoBehaviour {


    static public Wiimote wiimote;                  // Wiimote手把

    public GameObject Camera;                       // 攝影機
    public GameObject OptionPanel;                  // 設定屏幕
    public GameObject Practice_Button;              // 練習模式按紐
    public GameObject AIEnemy_Button;               // AI敵人模式按紐
    public GameObject StartWiimoteButton;           // 啟動wiimote按鈕
    public GameObject WiimoteText;                  // 顯示Wiimote偵測文字
    public GameObject DetectWiimote_Text;           // 偵測Wiimote

    public Sprite Practice_NoLignt;                 // 練習模式圖片(未選擇)
    public Sprite Practice_Lignt;                   // 練習模式圖片(已選擇)
    public Sprite AIEnemy_NoLignt;                  // AI敵人模式圖片(未選擇)
    public Sprite AIEnemy_Lignt;                    // AI敵人模式圖片(已選擇)
    public Text Practic_HighScore_Text;             // 練習最高分文字
    public Text AIEnemy_HighScore_Text;             // AI最高分文字
    public Text Map_Button_Text;                    // 地圖按鈕
    
    
    private int N;                                  // 選擇模式
    private float Speed ;                           // 旋轉鏡頭速度
    private string[] SceneName ;                    // 場景
    private Vector3[] Pos ;                         // 鏡頭位置
    private bool IsOption;                            // 設定
        
	// Use this for initialization
	void Start () {

        GameSystem.game_Mode = 0;
        Time.timeScale = 1.0f;
        N = 0;
        Speed = 2.0f;
        
        SceneName =  new string[2];
        SceneName[0] = "forest";
        SceneName[1] = "city";

        Pos = new Vector3[2];
        Pos[0] = new Vector3(-1420f, -10.6f, 0f);
        Pos[1] = new Vector3(1420f, 10.6f, 0f);
        Practic_HighScore_Text.text = "HIGHT SCORE : " + PlayerPrefs.GetInt("PracticeHighScore", 0);
        AIEnemy_HighScore_Text.text = "HIGHT SCORE : " + PlayerPrefs.GetInt("AIEnemyHighScore", 0);

        IsOption = false;

        InvokeRepeating("FlashingText", 0, 0.5f);
	}
	
	// Update is called once per frame
	void Update () {
        
        Camera.transform.Rotate(Vector3.up, Speed * Time.deltaTime);    //攝影機旋轉

        /**** 練習模式 ****/
        if (GameSystem.game_Mode == 1)                                  
        {
            Practice_Button.GetComponent<Image>().sprite = Practice_Lignt;
            AIEnemy_Button.GetComponent<Image>().sprite = AIEnemy_NoLignt;
        }

        /**** AI敵人模式 ****/
        if (GameSystem.game_Mode == 2)                                  
        {
            Practice_Button.GetComponent<Image>().sprite = Practice_NoLignt;
            AIEnemy_Button.GetComponent<Image>().sprite = AIEnemy_Lignt;
        }

        /**** 讀取 Wiimote Data 資料 ****/
        if (!WiimoteManager.HasWiimote()) {
            DetectWiimote_Text.SetActive(true);
            return; 
        }
        wiimote = WiimoteManager.Wiimotes[0];
        DetectWiimote_Text.SetActive(false);

	}


    /**** 讀取場景 ****/
    public void LoadScene()
    {
        if (GameSystem.game_Mode != 0 && WiimoteManager.HasWiimote())
        {
            //Application.LoadLevel(SceneName[N]);
            Application.LoadLevel("tutorial");
        }
    }

    /**** 選擇場景 ****/
    public void SelectScence()
    {
        Change_n();
        Camera.transform.position += Pos[N];

    }

    /**** 離開遊戲 ****/
    public void ExitGame()
    {
        Application.Quit();
    }

    /**** 選擇練習模式 ****/
    public void Practice()
    {
        GameSystem.game_Mode = 1;
    }

    /**** 選擇AI敵人模式 ****/
    public void AIEnemy()
    {
        GameSystem.game_Mode = 2;
    }

    /****  設定   ****/
    public void Option()
    {
        IsOption = !IsOption;
        OptionPanel.SetActive(IsOption);
    }

    /****  偵測Wiimote   ****/
    public void FindWiimote()
    {
        WiimoteManager.FindWiimotes();
        DetectWiimote();
    }

    /****  啟動Wiimote   ****/
    public void StartWiimote()
    {
        wiimote.SetupIRCamera(IRDataType.BASIC);
    }

    /****  清除Wiimote   ****/
    public void CleanUpWiimote()
    {
        WiimoteManager.Cleanup(wiimote);
        wiimote = null;
        DetectWiimote();
    }

    /****  偵測是否讀到Wiimote ****/
    private void DetectWiimote()
    {
        WiimoteText.GetComponent<Text>().text = "Wiimote Found : " + WiimoteManager.HasWiimote();

        /*  顯示啟動Wiimote按鈕   */
        if (WiimoteManager.HasWiimote())
        {
            StartWiimoteButton.SetActive(true);
        }
        else
        {
            StartWiimoteButton.SetActive(false);
        }
    }

    /****  地圖切換 ****/
    private void Change_n()
    {
        N = (N + 1) % 2;
        if (N == 0)
        {
            Map_Button_Text.text = "FOREST";
        }
        else
        {
            Map_Button_Text.text = "C I T Y";
        }
    }

    private void FlashingText()
    {
        if (DetectWiimote_Text.GetComponent<Text>().enabled)
            DetectWiimote_Text.GetComponent<Text>().enabled = false;
        else
            DetectWiimote_Text.GetComponent<Text>().enabled = true;
    }

}
