
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class NewDrawCallTestRunner : DrawCallTestRunner {
	
	public GameObject add_object;
	public string     add_object_path;
	public Transform  range;
	public int        count  = 15;

	float lenght = 25.0f;

	protected override GameObject biuldObjects()
	{
		var addObject = Instantiate(add_object ) as GameObject;
		addObject.GetComponent<MeshRenderer>().material =new Material( this.test_object_material);
		int id = testGameobjectLst.Count;
		addObject.name = id.ToString();
		float offset = lenght/count;
		int h    = id/(count*count);
		int row  = (id%(count*count))/count;
		int col  = id%count;
		addObject.transform.position = range.transform.position + new Vector3 (-lenght/2.0f,0,-lenght/2.0f)+new Vector3( row*offset,h+1, col* offset) ;
		return addObject;
	}

		
}

