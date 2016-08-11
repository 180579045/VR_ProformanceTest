using UnityEngine;
using System.Collections;

public class MUI : MonoBehaviour {

	//public UILabel FpsLable;
    public UILabel FpsLableVR;
	public UILabel ReportLable;
	static MUI _instance;
	public GameObject runer;
	public GameObject menuBtn;

    public GameObject level;

    string report_text;
	public static MUI Instance{
		get{
			return _instance;
		}
	}
	void Awake()
	{
		DontDestroyOnLoad(this);

		_instance = this;
	}


	void Start () {
        Debug.Log("VRMode:" + Cardboard.SDK.VRModeEnabled);
        if (Cardboard.SDK.VRModeEnabled)
        {
            UIEventListener.Get(runer).onClick += click;
            //menuBtn.GetComponent<ui_state_btn>().stateChange += statachanged;
        }
    }
	void statachanged(GameObject g)
	{
		MUI.Instance.runer.SetActive(false);
		if(g.name=="Triangle")
		{
            Debug.Log("button Triangle!");
            //加载关卡
			Application.LoadLevel(1);
		}
		else if(g.name == "Drawcall")
		{
			
			Application.LoadLevel(2);
		}
		else if(g.name == "AlphaBlend")
		{
			Debug.Log(g.name);
			Application.LoadLevel(3);
			
		}
		else if(g.name == "AlphaTest")
		{
			Application.LoadLevel(4);
			
		}
		else if(g.name == "SkinMesh")
		{
			
			Application.LoadLevel(5);
		}
		else if(g.name == "Rigibody")
		{
			Application.LoadLevel(6);
			
		}
        else if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Home))
        {
            Application.Quit();
        }
        else {
			
			
		}
	}

	int curent_level =1;
	void click(GameObject g)
	{
//		menuBtn.SetActive(false);
//		g.SetActive(false);
//		Application.LoadLevel(curent_level);

	}

    public void TrangleOnclick()
    {
//         level = GameObject.Find("UI Root VR_1");
//         level.SetActive(true);
        Application.LoadLevel(1);
    }

    public void DrawCallOnclick()
    {
//         level = GameObject.Find("UI Root VR_2");
//         level.SetActive(true);
        Application.LoadLevel(2);
    }

    public void AlphaBlendOnclick()
    {
//         level = GameObject.Find("UI Root VR_3");
//         level.SetActive(true);
        Application.LoadLevel(3);
    }

    public void AlphaTestOnclick()
    {
//         level = GameObject.Find("UI Root VR_4");
//         level.SetActive(true);
        Application.LoadLevel(4);
    }

    public void SkinMeshOnclick()
    {
//         level = GameObject.Find("UI Root VR_5");
//         level.SetActive(true);
        Application.LoadLevel(5);
    }


    public void RigibodyOnclick()
    {
//         level = GameObject.Find("UI Root VR_6");
//         level.SetActive(true);
        Application.LoadLevel(6);
    }


    void Update () {
		//FpsLableVR.text = FPSCounter.Instance.currentFpsText;
        //Debug.Log("In MUI FPS Current Text is" + FpsLableVR.text);

        //		GameObject g = GameObject.Find("runner");
        //		if(g==null)
        //			return;
        //		PerformanceTestRunner runner = g.GetComponent<PerformanceTestRunner>();
        //		if(runner == null)
        //			return;
        //		if(runner.testOver && curent_level<6)
        //		{
        //			if(curent_level == 1)
        //			{
        //				report_text += "Triangle Test:\n";
        //			}
        //			else if(curent_level ==2)
        //			{
        //				report_text += "draw call Test:\n";
        //			}
        //			else if(curent_level ==3)
        //			{
        //				report_text +="AlphaBlend Test:\n";
        //			}
        //			else if(curent_level ==4)
        //			{
        //				report_text += "AlphaTest Test:\n";
        //			}
        //			else if(curent_level ==5)
        //			{
        //				report_text += "SkinMesh Test: \n";
        //			}
        //			report_text  += runner.report_text;
        //			curent_level +=1;
        //			Application.LoadLevel(curent_level);
        //		    
        //		}
        //		else if(runner.testOver && curent_level ==6)
        //		{
        //			report_text += "Rigibody Test:\n";
        //			report_text += runner.report_text;
        //			ReportLable.text = report_text;
        //		}

    }
}
