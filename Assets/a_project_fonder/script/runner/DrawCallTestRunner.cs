using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class DrawCallTestRunner :  PerformanceTestRunner {

	public int  preAddObjectCount       = 30;
	public int  curentAddObjectCount    = 30;
	public int  precisionAddObjectCount = 1;

	bool adding = true;

	protected virtual GameObject biuldObjects()
	{
		GameObject g          = new GameObject();
		MeshFilter mFilter    = g.AddComponent<MeshFilter>();
//		PlaneBiulder.pLength  = 10;
		PlaneBiulder.pLength  = 100.0f;
		mFilter.mesh          = PlaneBiulder.MinMesh;
		
		MeshRenderer render   = g.AddComponent<MeshRenderer>();
		Material   nm         = new Material(test_object_material);
		render.material       = nm;
		int id  = testGameobjectLst.Count;
//		g.transform.position = this.transform.position +new Vector3(id%10*10,((id%100)/10)*10,0);
		g.transform.position = g.transform.position -Vector3.forward*id*0.001f;
//		g.transform.parent    = this.transform;
		return g;
	}

	protected override IEnumerator run()
	{
        Debug.Log("DrawCall run!!!!");
		
		while(!testOver)
		{
			if(adding)
			{
                Debug.Log("testOver is" + testOver);
				for(int i = 0;i<curentAddObjectCount;i++)
				{
					testGameobjectLst.Push(biuldObjects());
					yield return 0;
				}
			}
			else
			{
                Debug.Log("testOver2 is" + testOver);
                curentAddObjectCount -= precisionAddObjectCount;
				for(int i = 0;i<precisionAddObjectCount;i++)
				{
					var bigObject =testGameobjectLst.Pop();
					DestroyImmediate(bigObject);
				}
			}

            Debug.Log("testOver3 is" + testOver);
            yield return 0;
			yield return StartCoroutine(accurateTestFps());

            Debug.Log("testOver4 is" + testOver);

            this.biuldReport();
			if(curentAddObjectCount < precisionAddObjectCount && Mathf.Abs( currentFps - fpsTestLimitValue)<=precisionFps)
			{
				Debug.Log("test over");

				this.report_text += "Test Over!!!!";
                this.Rpt_Text.text += "Test Over!!!!";

                testOver = true;
				yield break;
			}
			else if(currentFps < fpsTestLimitValue)
			{
				adding = false;
			}
			else
			{
				adding =true;
			}

		}
	}


}
