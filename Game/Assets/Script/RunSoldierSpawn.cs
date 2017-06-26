using UnityEngine;
using System.Collections;

public class RunSoldierSpawn : MonoBehaviour {

    static public int RunSoldier_Num;

    public GameObject SoldierObject;     //士兵物件
    public GameObject Destination;       //士兵移動目的位置

    private float nextCreate;            //下一次生成時間
    private float createRate;            //生成頻率

	// Use this for initialization
	void Start () {

        RunSoldier_Num = 0;
        createRate = Random.RandomRange(60, 91);
        nextCreate = Time.time + createRate;

	}
	
	// Update is called once per frame
	void Update () {
        if (GameSystem.isGameing && GameSystem.game_Mode == 2 && Time.time > nextCreate)
        {
            RunSoldier.Create(SoldierObject, this.transform.position, Destination.transform.position);     //建立士兵
            RunSoldier_Num += 1;                                                                           //士兵數目+1

            createRate = Random.RandomRange(60, 91);
            nextCreate = Time.time + createRate;
        }
	}

}
