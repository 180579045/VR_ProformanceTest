
using UnityEngine;

public class TrailSectionAllocator : FrameMemAllocator<TrailSection>
{
    public TrailSectionAllocator()
    {
        for(int index = 0; index < m_Buffer.Length; index++)
        {
            m_Buffer[index] = new TrailSection();
        }
    }

    public override void InitAllocator()
    {
        base.InitAllocator();
    }

    protected override void DefaultData(long index)
    {
        m_Buffer[index].Pos = Vector3.zero;
        m_Buffer[index].UpdateTime = float.PositiveInfinity;
    }

    protected override void CopyTo(TrailSection src, ref TrailSection des)
    {
        if (
            (null == src)
            || (null == des)
            )
        {
            return;
        }

        des.Pos = src.Pos;
        des.UpdateTime = src.UpdateTime;
    }
   
}