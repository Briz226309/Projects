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
 * Date: July 22, 2018
 * Travel Experts.
 * DB class for the suppliers entity class
 * */
    public class SupplierDB
    {
        /// <summary>
        /// Returns all the suppliers from the Travel experts DB
        /// </summary>
        /// <returns>a list of suppliers</returns>
        public static List<Supplier> GetAllSuppliers()
        {
            List<Supplier> suppliers = new List<Supplier>();
            Supplier currentSupplier = null;
            SqlConnection con = TravelExpertsDB.GetConnection();


            string selectStatement = "SELECT * "
                                      + "From Suppliers " +
                                      "ORDER BY SupName";

            SqlCommand cmd = new SqlCommand(selectStatement, con);

            try
            {
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read()) //While there are orders
                {
                    currentSupplier = new Supplier();

                    currentSupplier.SupplierID = Convert.ToInt32(reader["SupplierId"]);
                    currentSupplier.SupplierName = Convert.ToString(reader["SupName"]);

                    suppliers.Add(currentSupplier);
                }
            }
            catch (SqlException ex)
            {
                throw ex;
            }
            finally
            {
                con.Close();

            }
            return suppliers;
        }


        /// <summary>
        /// Adds a new supplier to Travel Expers DB
        /// </summary>
        /// <param name="supplier"> Supplier object that contains data for the new record</param>
        /// <returns>new supplier id</returns>
        public static int AddSupplier(Supplier supplier)
        {
            //Get the connection
            SqlConnection con = TravelExpertsDB.GetConnection();

            //Prepare the insert statement
            string insertStatement = "INSERT INTO Suppliers (SupplierId, SupName) " +
                                     "VALUES(@SupplierId, @Name)";
            

            //Prepare the statement to get the highest SupplierID in the DB
            string highestIDStatement = "SELECT MAX(SupplierId) from Suppliers";
            SqlCommand readCmd = new SqlCommand(highestIDStatement, con);

            try
            {
                con.Open();

                int currentHighestSupplierID = Convert.ToInt32(readCmd.ExecuteScalar()); // single value
                supplier.SupplierID = ++currentHighestSupplierID; //Increament the supplier ID and assign to new supplier

                //Prepare to insert the new supplier
                SqlCommand cmd = new SqlCommand(insertStatement, con);
                cmd.Parameters.AddWithValue("@SupplierId", supplier.SupplierID);
                cmd.Parameters.AddWithValue("@Name", supplier.SupplierName);

                cmd.ExecuteNonQuery(); // run the insert command
                
                return currentHighestSupplierID;
            }
            catch (SqlException ex)
            {
                throw ex;
            }
            finally
            {
                con.Close();
            }
        }

        /// <summary>
        /// Deletes a supplier from the travel experts database
        /// Uses optimistic concurrency to ensure that the data hasn't changed since we
        /// last read from the database.
        /// </summary>
        /// <param name="sup">Supplier to delete</param>
        /// <returns>Bool value indicating success</returns>
        public static bool DeleteSupplier(Supplier sup)
        {
            SqlConnection con = TravelExpertsDB.GetConnection();
            string deleteStatement = "DELETE FROM Suppliers " +
                                     "WHERE SupplierId = @SupplierId " + // to identify the customer to be  deleted
                                     " AND SupName = @SupName"; // remaining conditions - to ensure optimistic concurrency

            SqlCommand cmd = new SqlCommand(deleteStatement, con);
            cmd.Parameters.AddWithValue("@SupplierId", sup.SupplierID);
            cmd.Parameters.AddWithValue("@SupName", sup.SupplierName);

            try
            {
                con.Open();
                int count = cmd.ExecuteNonQuery();
                if (count > 0) return true;
                else return false;
            }
            catch (SqlException ex)
            {
                throw ex;
            }
            finally
            {
                con.Close();
            }
        }

        /// <summary>
        /// Updates existing suppliers record.
        /// Uses optimistic concurrency to ensure that the data hasn't changed
        /// since we last read from the database.
        /// </summary>
        /// <param name="oldSup">data before update</param>
        /// <param name="newSup">new data for the update</param>
        /// <returns>indicator of success</returns>
        public static bool UpdateSupplier(Supplier oldSup, Supplier newSup)
        {
            SqlConnection con = TravelExpertsDB.GetConnection();
            string updateStatement = "UPDATE Suppliers " +
                                     "SET SupName = @NewSupName " +
                                     "WHERE SupplierId = @OldSupplierId " +
                                     "AND SupName = @OldSupName "; //This is to check that the supplier name hasn't changed
            SqlCommand cmd = new SqlCommand(updateStatement, con);
            cmd.Parameters.AddWithValue("@NewSupName", newSup.SupplierName);
            cmd.Parameters.AddWithValue("@OldSupplierId", oldSup.SupplierID);
            cmd.Parameters.AddWithValue("@OldSupName", oldSup.SupplierName);
            
            try
            {
                con.Open();
                int count = cmd.ExecuteNonQuery();
                if (count > 0) return true;
                else return false;
            }
            catch (SqlException ex)
            {
                throw ex;
            }
            finally
            {
                con.Close();
            }
        }

    }
}
