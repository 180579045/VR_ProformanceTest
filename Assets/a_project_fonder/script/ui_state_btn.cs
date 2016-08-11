using UnityEngine;
using System.Collections;



public class ui_state_btn : MonoBehaviour {

	public GameObject _select;
    public GameObject[] _states;
	public delegate void vobject(GameObject g);
	public event vobject stateChange;
	void Start () {

	     foreach(var item in _states)
		{
			UIEventListener.Get(item).onClick += click ;
		}
		_select.SetActive(false);
	}

	void click(GameObject g)
	{
		iTween.MoveTo(_select,g.transform.position,0.6f);
		_select.SetActive(true);
		if (stateChange!=null)
		{
			stateChange(g);
		}

	}


}

