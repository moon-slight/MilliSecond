using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class StartGame : MonoBehaviour {

    public GameObject LOGO_Panel;

	// Use this for initialization
	void Start () {
        LOGO_Panel.GetComponent<Image>().canvasRenderer.SetAlpha(0.0f);  //扣寫透明度條0
        Display();
	}
	
	// Update is called once per frame
	void Update () {
        Invoke("UnDisplay", 3.0f);
        Invoke("GoToHomePage", 5.0f);
	}
    void Display()
    {
        LOGO_Panel.GetComponent<Image>().CrossFadeAlpha(1.0f, 1f, false);
    }
    void UnDisplay()
    {
        LOGO_Panel.GetComponent<Image>().CrossFadeAlpha(0.0f, 1f, false);
    }
    void GoToHomePage()
    {
        Application.LoadLevel("home page");
    }
}
