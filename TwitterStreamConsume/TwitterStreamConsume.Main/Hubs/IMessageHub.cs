using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TwitterStreamConsume.Main.Hubs
{
    public interface IMessageHub
    {
        Task DisplayMessage(string strMessage);
    }
}
