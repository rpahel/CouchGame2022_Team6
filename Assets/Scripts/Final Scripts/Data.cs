using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Data
{
    public enum CUBETYPE
    {
        NONE,
        EDIBLE,
        TRAP,
        BEDROCK
    }
    
    public enum PlayerState
    {
        Aiming,
        Shooting,
        Dashing,
        WallJumping,
        Moving,
        Falling,
    }
        
    public enum SwitchSizeSkin
    {
        Little,
        Medium,
        Big
    }  
}
