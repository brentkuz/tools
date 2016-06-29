using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CQRS
{
    public interface IDatabase : IDisposable
    {
        T Query<T>(IQuery<T> query);
        int Command(ICommand command);
        void ChangeConnectionState(ConnectionAction state);
    }

    public enum ConnectionAction
    {
        Open, Close
    }
}