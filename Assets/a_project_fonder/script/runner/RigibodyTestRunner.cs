using UnityEngine;
using System.Collections;

public class RigibodyTestRunner :DrawCallTestRunner {


	public GameObject add_object;
	public string     add_object_path;
	public Transform range;
	public int        count  = 15;
	float lenght =  25;
	protected override GameObject biuldObjects()
	{
		var addObject = Instantiate(add_object ) as GameObject;
		int id = testGameobjectLst.Count;
		addObject.name = id.ToString();
		float offset = lenght/count;
		
		
		int h    = id/(count*count);
		int row  = (id%(count*count))/count;
		int col  = id%count;
//		add_object.transform.position = range.transform.position + new Vector3 (-5,2,-5)+new Vector3(id/count *10.0f/count,0,id%count * 10.0f/count) ;
		add_object.transform.position = range.transform.position + new Vector3 (0,2,0) +new Vector3 (-lenght/2.0f,0,-lenght/2.0f)+new Vector3( row*offset,h+1, col* offset);
		return addObject;
	}

}

