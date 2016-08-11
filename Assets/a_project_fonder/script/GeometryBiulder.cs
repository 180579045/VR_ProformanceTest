using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class  PlaneBiulder  {

	public static float pLength = 100.0f;

	public static  Mesh MaxMesh{
		get{
			return biuldMesh(254); // 254*254 = 64516  unity support 65000 vertices
		}
	}

	public static Mesh MinMesh{
		get{
			return biuldMesh(1);
		}

	}

	public static  Mesh getMesh(int level)
	{
		return biuldMesh(level);
	}

	static Mesh biuldMesh(int level)
	{
		Mesh        myMesh = new Mesh();
		List<Vector3> vBuffer = new List<Vector3>();
		List<int>     trigangleBuffer = new List<int>();
		List<Vector2> uvBuffer = new List<Vector2>();
		
		float offset = pLength/(float) level;
		
		// fill vertex uv
		for(int i=0 ; i <level+1 ; i++)
		{
			for(int j= 0; j<level+1 ; j++)
			{
				vBuffer.Add(new Vector3(i*offset,j*offset,0));
				uvBuffer.Add(new Vector2(i/(float)level,j/(float)level));
			}
		}

		// fill trigangles
		for(int i = 0; i<level; i++)
		{
			for(int j = 0; j<level; j++)
			{
				trigangleBuffer.Add(j+i*(level+1));
				trigangleBuffer.Add(j+(i+1)*(level+1));
				trigangleBuffer.Add((j+1)+(i+1)*(level+1));
				
				trigangleBuffer.Add(j+i*(level+1));
				trigangleBuffer.Add(j+1+(i+1)*(level+1));
				trigangleBuffer.Add((j+1)+i*(level+1));
			}
		}

		myMesh.vertices = vBuffer.ToArray();
		myMesh.triangles = trigangleBuffer.ToArray();
		myMesh.uv = uvBuffer.ToArray();
		myMesh.RecalculateNormals();
		System.GC.Collect();
		return myMesh;

	}
	
}
