using System.Collections.Generic;

public interface IUIAtlasProgress
{
    int TotalPiece
    {
        get;
    }

    void UpdateProgress(int current);

    void InitProgresser(int total, string dispStr);
}

public class AnalyseConsistencyProgresser : IUIAtlasProgress
{
    private int m_totalPiece = 0;
    private string m_dispStr = string.Empty;

    public int TotalPiece
    {
        get
        {
            return m_totalPiece;
        }
    }

    public void UpdateProgress(int current)
    {
        float currentProgresss = 0f;

        if (m_totalPiece != 0)
        {
            currentProgresss = (float)current / (float)m_totalPiece;
        }

        if (onUpdateProgress != null)
        {
            onUpdateProgress(currentProgresss, m_dispStr);
        }
    }

    public void InitProgresser(int total, string dispStr)
    {
        m_totalPiece = total;
        m_dispStr = dispStr;

        if (onInitProgress != null)
        {
            onInitProgress();
        }
    }

    public delegate void UpdateProgressCommand(float currentProgress, string dispStr);
    public delegate void InitProgressCommand();

    public UpdateProgressCommand onUpdateProgress;
    public InitProgressCommand onInitProgress;

    static private AnalyseConsistencyProgresser m_Instance = null;

    public static AnalyseConsistencyProgresser GetInstance()
    {
        if (m_Instance == null)
        {
            m_Instance = new AnalyseConsistencyProgresser();
        }
        return m_Instance;
    }
    public static void DestoryInstance()
    {
        if (m_Instance != null)
        {
            m_Instance = null;
            AnalyseConsistencyProgresser.DestoryInstance();
        }
    }
}

public class AnnalyseReferenceProgresser : IUIAtlasProgress
{
    private int m_totalPiece = 0;
    private string m_dispStr = "引用导出中";

    public int TotalPiece
    {
        get
        {
            return m_totalPiece;
        }
    }

    public void UpdateProgress(int current)
    {
        float currentProgresss = 0f;

        if (m_totalPiece != 0)
        {
            currentProgresss = (float)current / (float)m_totalPiece;
        }

        if (onUpdateProgress != null)
        {
            onUpdateProgress(currentProgresss, m_dispStr);
        }
    }

    public void InitProgresser(int total, string dispStr)
    {
        m_totalPiece = total;
        m_dispStr = dispStr;

        if (onInitProgress != null)
        {
            onInitProgress();
        }
    }

    public delegate void UpdateProgressCommand(float currentProgress, string dispStr);
    public delegate void InitProgressCommand();

    public UpdateProgressCommand onUpdateProgress;
    public InitProgressCommand onInitProgress;

    static private AnnalyseReferenceProgresser m_Instance = null;

    public static AnnalyseReferenceProgresser GetInstance()
    {
        if (m_Instance == null)
        {
            m_Instance = new AnnalyseReferenceProgresser();
        }
        return m_Instance;
    }
    public static void DestoryInstance()
    {
        if (m_Instance != null)
        {
            m_Instance = null;
            AnnalyseReferenceProgresser.DestoryInstance();
        }
    }
}

public class WriteFileProgresser : IUIAtlasProgress
{
    private int m_totalPiece = 0;
    private string m_dispStr = "CSV文件写入中";

    public int TotalPiece
    {
        get
        {
            return m_totalPiece;
        }
    }

    public void UpdateProgress(int current)
    {
        float currentProgresss = 0f;

        if (m_totalPiece != 0)
        {
            currentProgresss = (float)current / (float)m_totalPiece;
        }

        if (onUpdateProgress != null)
        {
            onUpdateProgress(currentProgresss, m_dispStr);
        }
    }

    public void InitProgresser(int total, string dispStr)
    {
        m_totalPiece = total;
        m_dispStr = dispStr;

        if (onInitProgress != null)
        {
            onInitProgress();
        }
    }

    public delegate void UpdateProgressCommand(float currentProgress, string dispStr);
    public delegate void InitProgressCommand();

    public UpdateProgressCommand onUpdateProgress;
    public InitProgressCommand onInitProgress;

    static private WriteFileProgresser m_Instance = null;

    public static WriteFileProgresser GetInstance()
    {
        if (m_Instance == null)
        {
            m_Instance = new WriteFileProgresser();
        }
        return m_Instance;
    }

    public static void DestoryInstance()
    {
        if (m_Instance != null)
        {
            m_Instance = null;
            WriteFileProgresser.DestoryInstance();
        }
    }

}

public class UpdateReferenceProgresser : IUIAtlasProgress
{
    private int m_totalPiece = 0;
    private string m_dispStr = "引用关系分析中";

    public int TotalPiece
    {
        get
        {
            return m_totalPiece;
        }
    }

    public void UpdateProgress(int current)
    {
        float currentProgresss = 0f;

        if (m_totalPiece != 0)
        {
            currentProgresss = (float)current / (float)m_totalPiece;
        }

        if (onUpdateProgress != null)
        {
            onUpdateProgress(currentProgresss, m_dispStr);
        }
    }

    public void InitProgresser(int total, string dispStr)
    {
        m_totalPiece = total;
        m_dispStr = dispStr;

        if (onInitProgress != null)
        {
            onInitProgress();
        }
    }

    public delegate void UpdateProgressCommand(float currentProgress, string dispStr);
    public delegate void InitProgressCommand();

    public UpdateProgressCommand onUpdateProgress;
    public InitProgressCommand onInitProgress;

    static private UpdateReferenceProgresser m_Instance = null;

    public static UpdateReferenceProgresser GetInstance()
    {
        if (m_Instance == null)
        {
            m_Instance = new UpdateReferenceProgresser();
        }
        return m_Instance;
    }

    public static void DestoryInstance()
    {
        if (m_Instance != null)
        {
            m_Instance = null;
            UpdateReferenceProgresser.DestoryInstance();
        }
    }
}