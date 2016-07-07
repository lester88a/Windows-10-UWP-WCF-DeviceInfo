using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace AccessSQLService
{
    [ServiceContract]
    public interface IService
    {
        // Query data
        [OperationContract]
        DataSet querySql(out bool queryParam);
    }
}
