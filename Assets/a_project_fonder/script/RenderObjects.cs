using UnityEngine;
using System.Collections;

public class RenderObjects : MonoBehaviour {
	
	MeshFilter mFilter;

	void Start () {

		mFilter = this.GetComponent<MeshFilter>();
		mFilter.mesh = PlaneBiulder.MaxMesh;
//		Debug.Log("tringgels:"+level*level*2/1000.0+"k");
//		Debug.Log("vertex: " +level*level);
//		Debug.Log(mFilter.mesh.triangles.Length/3);

		for(int i = 0;i<50;i++)
		{
			GameObject g = new GameObject();
			var f = g.AddComponent<MeshFilter>();
			f.mesh = mFilter.mesh;
			var r = g.AddComponent<MeshRenderer>();
			r.material = this.GetComponent<MeshRenderer>().material;

		}
	
	}
	

	void Update () {
	
	}
}
