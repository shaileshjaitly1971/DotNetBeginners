using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using InventoryBeginners.Data;
using InventoryBeginners.Models;
using Microsoft.Exchange.WebServices.Data;
using MyProject.Models;

namespace MyProject.BusinessLogic
{
    public class POTransactionBus
    {
        ISQLRepository sql;
        public POTransactionBus(ISQLRepository repository)
        {
            sql = repository;
        }

        internal async Task<List<PoHeader>> GetPOData(LedgerFilter lf)
        {
            List<PoHeader> lst = new List<PoHeader>();
            SqlParameter[] prms = new SqlParameter[2];

            prms[0] = new SqlParameter("@CommandType", "List");
            prms[1] = new SqlParameter("@Result", 0);
            lst = await sql.GetList<PoHeader>("[dbo].[spListPOApproved]", CommandType.StoredProcedure, prms);
            return lst;
        } // get all records List

        internal async Task<int> Update_POApprove(PoHeader model, string userid)
        {
            int iretVal = 0;

            SqlParameter[] prms = new SqlParameter[4];

            prms[0] = new SqlParameter("@UserId", userid);
            prms[1] = new SqlParameter("@PoNumber", model.PoNumber);
            prms[2] = new SqlParameter("@CommandType", "POApprove"); //@CommandType
            prms[3] = new SqlParameter("@Result", 0);

            prms[3].Direction = ParameterDirection.Output;

            await sql.ExecuteNonQuery("[dbo].[spListPOApproved]", CommandType.StoredProcedure, prms);
            int.TryParse(Convert.ToString(prms[3].Value), out iretVal);

            return iretVal;
        }

        internal async Task<int> Update_VoucherCancel(PoHeader model, string userid)
        {
            int iretVal = 0;

            SqlParameter[] prms = new SqlParameter[4];

            prms[0] = new SqlParameter("@UserId", userid);
            prms[1] = new SqlParameter("@PoNumber", model.PoNumber);
            prms[2] = new SqlParameter("@CommandType", "POCancel"); //@CommandType
            prms[3] = new SqlParameter("@Result", 0);

            prms[3].Direction = ParameterDirection.Output;

            await sql.ExecuteNonQuery("[dbo].[spListPOApproved]", CommandType.StoredProcedure, prms);
            int.TryParse(Convert.ToString(prms[3].Value), out iretVal);

            return iretVal;
        } //Update_VoucherApprove
    }//class
}//business logic
