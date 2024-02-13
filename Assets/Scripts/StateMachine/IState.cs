using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public interface IState<T> where T : Enum {

    public void EnterState();
    public T UpdateState();
    public void ExitState();

}

