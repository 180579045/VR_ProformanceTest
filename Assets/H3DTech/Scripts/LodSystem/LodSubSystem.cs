using UnityEngine;
using System.Collections;

public interface LodSubSystem{
    string Desc{
		get;
		set;
	}
	int CurrLOD
	{
		get;
		set;
	}

	void UpdateAllLODObjects();
}

public class LodSubSystemImpl : LodSubSystem
{
    public string Desc
    {
        get
        {
            return desc;
        }

        set
        {
            desc = value;
        }
    }

    public int CurrLOD
    {
        get
        {
            return currLOD;
        }

        set
        {
            currLOD = value;
            UpdateAllLODObjects();
        }
    }

    public  virtual void UpdateAllLODObjects()
    {

    }
    
    string desc;  
    int currLOD;
}
