using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SkinnedMeshTestRunner  : DrawCallTestRunner {

	public GameObject add_object;
	public string     add_object_path;
	public Transform range;
	public int        count  = 10;
	float length = 20.0f;
	protected override GameObject biuldObjects()
	{

		var addObject = Instantiate(add_object ) as GameObject;
		int id = testGameobjectLst.Count;
		addObject.name = id.ToString();
		float offset = length/count ;
	
		int row  = (id%(count*count))/count;
		int col  = id%count;
		addObject.transform.position = range.transform.position + new Vector3 (-length/2,2,-length/2)+new Vector3( row*offset,0, col* offset) ;
		return addObject;
	}
	protected override  void biuldReport()
	{
		this.report_text = "";
		this.report_text += "Gameobject Count [-]: " + testGameobjectLst.Count.ToString()+ "\n";
		int triangle_count = 0;
		int vertex_count   = 0;
		
		foreach(var item in testGameobjectLst)
		{
			SkinnedMeshRenderer[] skms =  item.GetComponentsInChildren<SkinnedMeshRenderer>();
			foreach(var ite in skms)
			{
				var ms = ite.sharedMesh;
				triangle_count += ms.triangles.Length;
				vertex_count   += ms.vertexCount;
			}

		}
		triangle_count /= 3;
		
		
		this.report_text += "Triangle Count [-]: "  + (triangle_count/1000.0).ToString() + "k \n";
		this.report_text += "Vertices Count [-]: "  + (vertex_count/1000.0).ToString() + "k \n";
		this.report_text += "FPS Limit Count [-]: " + this.fpsTestLimitValue.ToString() +"\n";
        this.report_text += "Current FPS [-]: " + this.currentFps.ToString() + "\n";
        
        //MUI.Instance.ReportLable.text = this.report_text;
        this.Rpt_Text.text = this.report_text;
		
	}

}
