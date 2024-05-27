using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using UnityEngine;

public class SoundGeneralControl : MonoBehaviour
{
    public static SoundGeneralControl Instance;

    private void Awake()
    {
        Instance = this;
       
    }
    public LayerMask soundReceiversLayerMask;

        
}
