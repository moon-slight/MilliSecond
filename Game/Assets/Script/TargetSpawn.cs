using UnityEngine;
using System.Collections;

public class TargetSpawn : MonoBehaviour
{
    public GameObject TargetObject;             //標靶物件

    private int TotalPositionNum;               //總共位置
    private int TargetNum;                      //標靶數
    private Vector3[] Initial_Position;         //建置位置
    private Vector3 CreatePosition;             //物件生產位置
    private bool[] Stage;                       //階段
    private bool[] Used_Position;               //已使用位置
    private int Stage_Count;                    //第幾階段
    private int Random_position;                //隨機座標位置
    private float Random_Speed;                 //隨機速度
    private float MoveSpeed;                    //移動速度


    // Use this for initialization
    void Start()
    {
        TotalPositionNum = 15;
        TargetNum = 1;
        initialposition();

        Stage = new bool[10];
        for (int i = 0; i < 10; i++)
        {
            Stage[i] = false;
        }
        Stage_Count = 0;

        MoveSpeed = 3f;
    }

    // Update is called once per frame
    void Update()
    {
        if (GameSystem.isGameing && GameSystem.game_Mode == 1)
        {
            if (!Stage[Stage_Count])
            {
                Invoke("Stage_" + Stage_Count.ToString(), 0.0f);
            }
            if (GameSystem.target_Num == 0 && Stage[Stage_Count])
            {

                Stage_Count += 1;
                if (Stage_Count == 4)
                {
                    MoveSpeed += 1f;
                    if (MoveSpeed >= 10f)
                    {
                        MoveSpeed = 10f;
                    }
                    TargetNum += 2;
                    if (TargetNum >= TotalPositionNum)
                    {
                        TargetNum = TotalPositionNum;
                    }

                    Stage_Count = 3;
                    Stage[Stage_Count] = false;
                    for (int i = 0; i < TotalPositionNum; i++)
                    {
                        Used_Position[i] = false;
                    }
                }
            }
        }
    }

    void Stage_0()
    {
        CreatePosition = new Vector3(216.59f, 0.12f, 73f);
        Target.Create(TargetObject, CreatePosition, 0f, false);                  //建立標靶
        GameSystem.target_Num += 1;                                         //標靶數目+1

        CreatePosition = new Vector3(240.2f, 0.12f, 73f);
        Target.Create(TargetObject, CreatePosition, 0f, false);                 //建立標靶
        GameSystem.target_Num += 1;                                         //標靶數目+1


        Stage[0] = true;
    }
    void Stage_1()
    {
        CreatePosition = new Vector3(223.5f, 0.12f, 84.5f);
        Target.Create(TargetObject, CreatePosition, 0f, false);                 //建立標靶
        GameSystem.target_Num += 1;                                         //標靶數目+1

        CreatePosition = new Vector3(229.5f, 0.12f, 84.5f);
        Target.Create(TargetObject, CreatePosition, 0f, false);                 //建立標靶
        GameSystem.target_Num += 1;                                         //標靶數目+1

        CreatePosition = new Vector3(235.89f, 0.12f, 84.5f);
        Target.Create(TargetObject, CreatePosition, 0f, false);                 //建立標靶
        GameSystem.target_Num += 1;                                         //標靶數目+1


        Stage[1] = true;
    }

    void Stage_2()
    {
        CreatePosition = new Vector3(229.5f, 0.12f, 98f);
        Target.Create(TargetObject, CreatePosition, 0f, false);                 //建立標靶
        GameSystem.target_Num += 1;                                         //標靶數目+1

        CreatePosition = new Vector3(249.5f, 0.12f, 98f);
        Target.Create(TargetObject, CreatePosition, 0f, false);                 //建立標靶
        GameSystem.target_Num += 1;                                             //標靶數目+1

        CreatePosition = new Vector3(269.5f, 0.12f, 98f);
        Target.Create(TargetObject, CreatePosition, 0f, false);                 //建立標靶
        GameSystem.target_Num += 1;                                             //標靶數目+1

        CreatePosition = new Vector3(209.5f, 0.12f, 98f);
        Target.Create(TargetObject, CreatePosition, 0f, false);                 //建立標靶
        GameSystem.target_Num += 1;                                             //標靶數目+1

        CreatePosition = new Vector3(189.5f, 0.12f, 98f);
        Target.Create(TargetObject, CreatePosition, 0f, false);                 //建立標靶
        GameSystem.target_Num += 1;                                             //標靶數目+1


        Stage[2] = true;
    }
    void Stage_3()
    {
        for (int i = 0; i < TargetNum; i++)
        {
            while (true)
            {
                Random_position = Random.Range(0, TotalPositionNum);
                if (!Used_Position[Random_position])
                {
                    Used_Position[Random_position] = true;
                    break;
                }
            }
            Random_Speed = Random.Range(1, MoveSpeed + 1);

            CreatePosition = Initial_Position[Random_position];
            Target.Create(TargetObject, CreatePosition, Random_Speed, true);           //建立標靶
            GameSystem.target_Num += 1;                                             //標靶數目+1
        }

        Stage[3] = true;
    }

    void initialposition()
    {
        Initial_Position = new Vector3[TotalPositionNum];
        Used_Position = new bool[TotalPositionNum];

        for (int i = 0; i < TotalPositionNum; i++)
        {
            Used_Position[i] = false;
        }

        Initial_Position[0] = new Vector3(194.08f, 0.12f, 137.83f);
        Initial_Position[1] = new Vector3(134.3f, 0.12f, 132.8f);
        Initial_Position[2] = new Vector3(155.9f, 0.12f, 157.9f);
        Initial_Position[3] = new Vector3(227f, 0.12f, 157.9f);
        Initial_Position[4] = new Vector3(260.4f, 0.12f, 131.33f);
        Initial_Position[5] = new Vector3(290.58f, 0.12f, 131.33f);
        Initial_Position[6] = new Vector3(318.25f, 0.12f, 149f);
        Initial_Position[7] = new Vector3(322.3f, 0.12f, 128.75f);
        Initial_Position[8] = new Vector3(281.17f, 0.12f, 162.41f);
        Initial_Position[9] = new Vector3(222.4f, 0.12f, 134.3f);
        Initial_Position[10] = new Vector3(194.08f, 0.12f, 82.7f);
        Initial_Position[11] = new Vector3(211.81f, 0.12f, 70.33f);
        Initial_Position[12] = new Vector3(227.4f, 0.12f, 96.9f);
        Initial_Position[13] = new Vector3(259.5f, 0.12f, 87.4f);
        Initial_Position[14] = new Vector3(169f, 0.12f, 107.9f);

    }
}
