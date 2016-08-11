using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;



public class PerformanceTestRunner : MonoBehaviour {


    public string report_text;

    public Text Rpt_Text;

	public    float     fpsWaitTime  = 3; //add object wait time
	public    int       testCount    = 30;
	public    int       fpsTestLimitValue = 60;
	public    Material  test_object_material;
	protected Stack<GameObject> testGameobjectLst = new Stack<GameObject>();
	[System.NonSerialized]
	protected int       currentFps   = 0;
	public    int       precisionFps = 5;
	[System.NonSerialized]
	public  bool 	    testOver  = false;

	void Awake()
	{
		Screen.sleepTimeout = SleepTimeout.NeverSleep;
        //GetComponent<Cardboard>().OnBackButton += handleOnBackButton;
    }

	void Start()
	{
		execute();
	}

	public void execute()
	{
		StartCoroutine(run());
	}
	
	protected virtual IEnumerator run()
	{
		yield return 1;
	}
	
	protected IEnumerator accurateTestFps()
	{
		yield return new WaitForSeconds(fpsWaitTime);
	
		int   count  = 0;
		int   fpsSum = 0;
		while(count < testCount )
		{
			count++;
			fpsSum += FPSCounter.Instance.currentFps;
            Debug.Log("Current FPS is" + fpsSum);
			yield return 0;
		}
		currentFps = fpsSum/count;
		yield return 1;
	}
	protected virtual void biuldReport()
	{
		this.report_text = "";
		this.report_text += "Gameobject Count [-]: " + testGameobjectLst.Count.ToString()+ "\n";
		int triangle_count = 0;
		int vertex_count   = 0;

        Debug.Log("Gameobject conunt is" + testGameobjectLst.Count.ToString());
		
		foreach(var item in testGameobjectLst)
		{
			var ms = item.GetComponent<MeshFilter>().mesh;
			triangle_count += ms.triangles.Length;
			vertex_count   += ms.vertexCount;
		}
		triangle_count /= 3;
		
		this.report_text += "Triangle Count [-]: "  + (triangle_count/1000.0).ToString() + "k \n";
		this.report_text += "Vertices Count [-]: "  + (vertex_count/1000.0).ToString() + "k \n";
		this.report_text += "FPS Limit Count [-]: " + this.fpsTestLimitValue.ToString() +"\n";
        this.report_text += "Current FPS [-]: " + currentFps.ToString() + "\n";
        //this.report_text += "Test over Flag [-]: " + testOver.ToString() + "\n";
        this.Rpt_Text.text = this.report_text;
        //Debug.Log("111" + this.report_text);
    }
	void Update()
	{
        //MUI.Instance.ReportLable.text = this.report_text;
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Home))
        {
            Application.LoadLevel(0);
        }
    }
}
