using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface INeedRequirements
{
    [SerializeField]IRequirements[] requirements
    {
        get;
        set;
    }
    public void OnRequirementChange()
    {
        bool requirementMet = true;
        foreach (IRequirements requirement in requirements)
        {
            if (!requirement.isTurnedOn)
            {
                requirementMet = false;
                break;
            }
        }
        if (requirementMet) OnRequirementMet();
    }
    abstract void OnRequirementMet();
}
