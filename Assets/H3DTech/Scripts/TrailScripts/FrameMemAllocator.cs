
using UnityEngine;
using System.Reflection;
using System.Collections.Generic;
public class FrameMemAllocator<T>
{
    public long DataCount
    {
        get
        {
            return m_DataCount;
        }
    }

    public T[] Buffer
    {
        get
        {
            return m_Buffer;
        }
    }

    public virtual void InitAllocator()
    {
        return;
    }

    public void InitBuffer(int index, T initValue)
    {
        if(
            (index < 0)
            || (index > m_Buffer.Length)
            )
        {
            return;
        }
        m_Buffer[index] = initValue;
    }
    public T GetData(long index)
    {
        if(
            (index < 0)
            || (index > m_DataCount)
            )
        {
            return default(T);
        }

        return m_Buffer[index];
    }
    public T[] AllocMem(long memCount)
    {
        if (memCount > m_BufferSize)
        {
            m_Buffer = new T[m_BufferSize * 2];
            m_BufferSize = m_BufferSize * 2;
        }

        Clear();

        return m_Buffer;
    }

    private void CheckAndUpdateBuffer(long dataCount)
    {
        if(dataCount < 0)
        {
            return;
        }

        if (dataCount > m_BufferSize)
        {
            T[] tempBuffer = new T[m_BufferSize * 2];
            for (int index = 0; index < m_Buffer.Length; index++)
            {
                tempBuffer[index] = m_Buffer[index];
            }

            m_Buffer = tempBuffer;
            m_BufferSize *= 2;
        }
    }

    public void Add(T target)
    {
        CheckAndUpdateBuffer(m_DataCount + 1);

        CopyTo(target, ref m_Buffer[m_DataCount]);
        //m_Buffer[m_DataCount] = target;
        m_DataCount++;
    }

    public void Replace(long index, T target)
    {
        if(
            (index < 0)
            || (index > m_DataCount)
            )
        {
            return;
        }

        CopyTo(target, ref m_Buffer[index]);
    }

    public void Insert(long index, T target)
    {
        if(
            (null == target)
            || (index < 0)
            || (index > m_DataCount)
            )
        {
            return;
        }

        CheckAndUpdateBuffer(m_DataCount + 1);

        for (long i = m_DataCount - 1; i >= index; i--)
        {
            CopyTo(m_Buffer[i], ref m_Buffer[i + 1]);
        }

        CopyTo(target, ref m_Buffer[index]);

        m_DataCount++;
    }
    public void RemoveAt(long index)
    {
        if(index >= m_DataCount)
        {
            return;
        }

        for (long i = index; i < m_DataCount; i++)
        {
            CopyTo(m_Buffer[i + 1], ref m_Buffer[i]);
        }

        DefaultData(m_DataCount - 1);

        m_DataCount--;
    }

    protected virtual void CopyTo(T src, ref T des)
    {
        if(
            (null == src)
            || (null == des)
            )
        {
            return;
        }

        //object desObj = des as object;
        //object srcResult, desResult;
        //FieldInfo[] thisFields = typeof(T).GetFields(BindingFlags.Instance | BindingFlags.Public);
        //foreach(var item in thisFields)
        //{
        //    srcResult = item.GetValue(src);
        //    desResult = item.GetValue(des);
        //    item.SetValue(desObj, srcResult);
        //}

        //des = (T)desObj;

        des = src;
    }
    public virtual void Clear()
    {
        //for (int index = 0; index < m_Buffer.Length; index++)
        //{
        //    DefaultData(index);
        //}

        m_DataCount = 0;
    }

    protected virtual void DefaultData(long index)
    {
        if(
            (index < 0)
            || (index > m_Buffer.Length)
            )
        {
            return;
        }
        m_Buffer[index] = default(T);
    }

    protected long m_DataCount = 0;
    protected long m_BufferSize = 6000;
    protected T[] m_Buffer = new T[6000];
}