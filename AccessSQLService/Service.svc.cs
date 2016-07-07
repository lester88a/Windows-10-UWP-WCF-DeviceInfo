using System.Data;
using System.Data.SqlClient;

namespace AccessSQLService
{
    public class Service : IService
    {
        /// <summary>
        /// Query data in TestTable
        /// </summary>
        /// <returns></returns>

        //SqlConnection sqlCon = new SqlConnection("Data Source=SAPC031;Initial Catalog=EasyReportDB;Integrated Security =True;");
        SqlConnection sqlCon = new SqlConnection("Data Source=192.168.10.199,1433;Initial Catalog=EasyReportDB;User ID=user;Password=test8960");
        public DataSet querySql(out bool queryParam)
        {
            try
            {
                sqlCon.Open();
                string strSql = "select Manufacturer from tblManufacturer";
                DataSet ds = new DataSet();
                SqlDataAdapter sqlDa = new SqlDataAdapter(strSql, sqlCon);
                sqlDa.Fill(ds);
                queryParam = true;
                return ds;
            }
            catch
            {
                queryParam = false;
                return null;
            }
            finally
            {
                sqlCon.Close();
            }
        }
    }
}
