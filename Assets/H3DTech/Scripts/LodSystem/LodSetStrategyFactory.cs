using UnityEngine;
using System.Collections;

public class LodSetStrategyFactory
{
    public virtual LodSetStrategy Create( int type )
    {
        return null;
    }
}
