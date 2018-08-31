using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TravelExpertsClassLibrary
{
 /**
 * Author: Prince Nimoh
 * Student #: 000792122
 * Date: July 18, 2018
 * Travel Experts.
 * DB class for connecting to the Travel Experts database
 * */
    public class TravelExpertsDB
    {
        /// <summary>
        /// Returns an SQL database connection for the Travel Experts DB
        /// </summary>
        /// <returns>SQL database connection</returns>
        public static SqlConnection GetConnection()
        {
            string connectionstring = @"Data Source=localhost\sqlexpress;Initial Catalog=TravelExperts;Integrated Security=True";
            SqlConnection con = new SqlConnection(connectionstring);

            return con;
        }
    }
}
