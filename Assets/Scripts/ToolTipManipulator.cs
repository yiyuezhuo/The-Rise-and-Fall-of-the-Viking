using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class TooltipManipulator : Manipulator
{

    public TooltipManipulator()
    {
    }

    protected override void RegisterCallbacksOnTarget()
    {
        target.RegisterCallback<MouseEnterEvent>(MouseIn);
        target.RegisterCallback<MouseOutEvent>(MouseOut);
    }

    protected override void UnregisterCallbacksFromTarget()
    {
        target.UnregisterCallback<MouseEnterEvent>(MouseIn);
        target.UnregisterCallback<MouseOutEvent>(MouseOut);
    }

    private void MouseIn(MouseEnterEvent e)
    {
        TooltipRoot.Instance.ShowTooltip(target);
    }

    private void MouseOut(MouseOutEvent e)
    {
        TooltipRoot.Instance.HideTooltip(target);
    }
}