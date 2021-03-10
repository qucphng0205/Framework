using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogStack
{
    Stack<BaseDialog> stack = new Stack<BaseDialog>();

    public bool Push(BaseDialog dialog)
    {
        if (dialog.Type == DialogType.Popup)
        {
            Debug.LogWarning("Cannot push popup into stack");
            return false;
        }
        
        //get prelast
        BaseDialog preLast = null;
        if (stack.Count > 0)
            preLast = stack.Peek();

        //push
        dialog.Show();
        stack.Push(dialog);

        //check modal
        if (dialog.Type != DialogType.Modal && preLast != null)
        {
            preLast.Hide();
        }

        return true;
    }

    public void Pop()
    {
        BaseDialog dialog = stack.Pop();
        dialog.Hide();

        stack.Peek().Show();
    }
}
