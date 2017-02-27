using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Invoice_Generate_System.DbGateway
{
    class ConnectionString
    {
        //public string DBConn = @"Data Source=KYOTO-PC13;Initial Catalog=NewProductList1;User ID=sa;Password=SystemAdministrator;Persist Security Info=True";
        public string DBConn = @"Data Source=tcp:kyotoServer,49172;Initial Catalog=NewProductList1;User ID=sa;Password=SystemAdministrator;Persist Security Info=True";
    }
}
