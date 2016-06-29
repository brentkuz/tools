using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace CQRS
{
    public interface ICommand
    {
        int Execute(IDbConnection db);
    }
}