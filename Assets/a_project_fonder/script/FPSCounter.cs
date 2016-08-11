using UnityEngine;
using System.Collections;

public class FPSCounter : MonoBehaviour {

	const   float  fpsMeasurePeriod = 0.5f;
	private int    m_FpsAccumulator = 0;
	private float  m_FpsNextPeriod = 0;
	private int    m_CurrentFps;
	const   string display = "{0} FPS";
	public  string m_Text;

	public  string currentFpsText
	{
		get{
			return m_Text;
		}
	}

	public int currentFps{
		get{
			return m_CurrentFps;
		}
	}
	
	static FPSCounter _instance;
	public static  FPSCounter Instance
	{
		get{
			return _instance;
		}
	}
	void Awake()
	{
		DontDestroyOnLoad(this);
		_instance = this;
	}
	private void Start()
	{
		m_FpsNextPeriod = Time.realtimeSinceStartup + fpsMeasurePeriod;

	}
	
	private void Update()
	{
		m_FpsAccumulator++;
		if (Time.realtimeSinceStartup > m_FpsNextPeriod)
		{
			m_CurrentFps = (int) (m_FpsAccumulator/fpsMeasurePeriod);
			m_FpsAccumulator = 0;
			m_FpsNextPeriod += fpsMeasurePeriod;
			m_Text  = string.Format(display, m_CurrentFps);
		}
	}
}


