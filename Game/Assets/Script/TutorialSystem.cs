using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TutorialSystem : MonoBehaviour {

    public GameObject PlayGame_Text;           
    public GameObject Toutorial_Panel;

    private bool CheckKey;
	// Use this for initialization
	void Start () {
        Toutorial_Panel.GetComponent<Image>().canvasRenderer.SetAlpha(0.0f);  //扣寫透明度條0
        Display();

        CheckKey = false;

        InvokeRepeating("FlashingText", 3, 0.5f);
	}
	
	// Update is called once per frame
	void Update () {
        int ret;
        do
        {
            ret = HomePageSystem.wiimote.ReadWiimoteData();
        } while (ret > 0);

        if (HomePageSystem.wiimote.Button.a && !CheckKey)
        {
            CheckKey = true;
            PlayGame_Text.SetActive(false);
            UnDisplay();
            Invoke("PlayGame", 1.0f);

        }
	}

    void Display()
    {
        Toutorial_Panel.GetComponent<Image>().CrossFadeAlpha(1.0f, 3f, false);
    }
    void UnDisplay()
    {
        Toutorial_Panel.GetComponent<Image>().CrossFadeAlpha(0.0f, 1f, false);
    }
    void PlayGame()
    {
        Application.LoadLevel("forest");
    }
    private void FlashingText()
    {
        if (PlayGame_Text.GetComponent<Text>().enabled)
            PlayGame_Text.GetComponent<Text>().enabled = false;
        else
            PlayGame_Text.GetComponent<Text>().enabled = true;
    }
}
