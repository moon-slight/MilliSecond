using UnityEngine;
using System.Collections;

public class SoldierSpawn : MonoBehaviour
{
    static public int TotalPositionNum;               //總共位置
    static public Vector3[] Initial_Position;         //建置位置
    static public bool[] Used_Position;               //已使用位置

    public GameObject SoldierObject;     //士兵物件

    
    private Vector3 CreatePosition;             //物件生產位置

    private float nextCreate;            //下一次生成時間
    private float createRate;            //生成頻率
    private float ShootRate;
    private int limit_Num;               //限制士兵數量
    private int Random_position;         //隨機座標位置
    private int CreateNum;               //總共建置數

    // Use this for initialization
    void Start()
    {
        TotalPositionNum = 18;
        initialposition();
        ShootRate = 6f;
        nextCreate = 0f;
        createRate = 0f;
        limit_Num = 5;
        Random_position = 0;                                // 隨機給一個生成座標點位置
        CreateNum = 0;
    }

    // Update is called once per frame
    void Update()
    {


        if (GameSystem.isGameing && GameSystem.game_Mode == 2 && GameSystem.target_Num < limit_Num && Time.time > nextCreate)
        {
            if (CreateNum % 5 == 0 && ShootRate >2f)
            {
                ShootRate -= 1f;
            }

            while (true)
            {
                Random_position = Random.Range(0, TotalPositionNum);
                if (!Used_Position[Random_position])
                {
                    Used_Position[Random_position] = true;
                    break;
                }
            }

            AISoldier.Create(SoldierObject, this.transform.position, Random_position, ShootRate);     //建立士兵
            GameSystem.target_Num += 1;                                                    //士兵數目+1
            CreateNum += 1;

            createRate = Random.RandomRange(1, 11);
            nextCreate = Time.time + createRate;
        }
    }

    void initialposition()
    {
        Initial_Position = new Vector3[TotalPositionNum];
        Used_Position = new bool[TotalPositionNum];

        for (int i = 0; i < TotalPositionNum; i++)
        {
            Used_Position[i] = false;
        }

        Initial_Position[0] = new Vector3(113.7f, 0f, 139.45f);
        Initial_Position[1] = new Vector3(134.07f, 0f, 139.45f);
        Initial_Position[2] = new Vector3(142.04f, 0f, 155.82f);
        Initial_Position[3] = new Vector3(120.41f, 0f, 155.82f);
        Initial_Position[4] = new Vector3(163.9f, 0f, 155.82f);
        Initial_Position[5] = new Vector3(149.41f, 0f, 126.81f);
        Initial_Position[6] = new Vector3(165.66f, 0f, 126.81f);
        Initial_Position[7] = new Vector3(174.66f, 0f, 144.81f);
        Initial_Position[8] = new Vector3(196.64f, 0f, 153.83f);
        Initial_Position[9] = new Vector3(185.61f, 0f, 131.65f);
        Initial_Position[10] = new Vector3(211.58f, 0f, 131.65f);
        Initial_Position[11] = new Vector3(219.44f, 0f, 148.39f);
        Initial_Position[12] = new Vector3(230.69f, 0f, 127.87f);
        Initial_Position[13] = new Vector3(245.47f, 0f, 127.87f);
        Initial_Position[14] = new Vector3(245.47f, 0f, 148.02f);
        Initial_Position[15] = new Vector3(267.71f, 0f, 148.02f);
        Initial_Position[16] = new Vector3(261.9f, 0f, 127.53f);
        Initial_Position[17] = new Vector3(282.87f, 0f, 127.53f);


    }
}
