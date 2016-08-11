using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class TriangleTestRunner : PerformanceTestRunner {

	public int precisionMeshLevel = 20;
	public int dampMeshLeveOffset = 10;

	public Transform range;
	int _maxMeshLevel = 254;
	public int maxMeshLevel{
		get{

			return _maxMeshLevel;
		}
		set{
			_maxMeshLevel = value;
			currentMeshLevel = _maxMeshLevel;
		}
	}
	
	int currentMeshLevel = 254;


	
	void Start()
	{
		execute();
	}

	protected override IEnumerator run()
	{

		while(!testOver)
		{
		
	
			GameObject g          = new GameObject();
			MeshFilter mFilter    = g.AddComponent<MeshFilter>();
		
			PlaneBiulder.pLength = 20;
			mFilter.mesh          = PlaneBiulder.getMesh(currentMeshLevel);
		
			MeshRenderer render   = g.AddComponent<MeshRenderer>();
			render.material       = test_object_material;
			g.transform.parent    = this.transform;
			int c  = testGameobjectLst.Count;
			g.transform.position  = range.position + new Vector3(0,(c+1)*0.1f,0);
			g.transform.localRotation = Quaternion.Euler(-90,0,0);
			testGameobjectLst.Push(g);
		
			yield return 0;
			yield return StartCoroutine(accurateTestFps());
			this.biuldReport();
			if(currentFps<fpsTestLimitValue && currentMeshLevel<precisionMeshLevel ||Mathf.Abs( currentFps - fpsTestLimitValue)<=precisionFps)
			{
		
				Debug.Log("test over");
				this.report_text += "[ff0000]Test Over!!!![-]";
				testOver = true;
				yield break;
			}
			else if(currentFps<fpsTestLimitValue){

				currentMeshLevel -=  dampMeshLeveOffset;
				var bigObject =testGameobjectLst.Pop();
				DestroyImmediate(bigObject);
				yield return StartCoroutine(run());
			}
			else
			{
			    yield return 0;
			}
		}
	}






}
