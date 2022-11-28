using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class State
{
    protected readonly PlayerSystem playerSystem;

    protected State(PlayerSystem playerSystem)
    {
        this.playerSystem = playerSystem;
    }

    public virtual IEnumerator Awake()
    {
        yield break;
    }
    public virtual IEnumerator Start()
    {
        yield break;
    }

    public virtual IEnumerator Update()
    {
        yield break;
    }

    public virtual IEnumerator FixedUpdate()
    {
        yield break;
    }

    public virtual IEnumerator UseInput_A()
    {
        yield break;
    }

    public virtual IEnumerator UseInput_B()
    {
        yield break;
    }

    public virtual IEnumerator UseInput_rightArrow()
    {
        yield break;
    }

    public virtual IEnumerator UseInput_upArrow()
    {
        yield break;
    }

    public virtual IEnumerator UseInput_downArrow()
    {
        yield break;
    }

    public virtual IEnumerator HitEffect()
    {
        yield break;
    }

    public virtual IEnumerator AnimationEnded()
    {
        yield break;
    }
}

