using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManagerApi.Models.Abstractions;

public abstract class AbstractionService
{
    public bool DoAction(Action action)
    {
        try
        {
            action.Invoke();
            return true;
        }
        catch (Exception e)
        {
            return false;
        }
    }
}