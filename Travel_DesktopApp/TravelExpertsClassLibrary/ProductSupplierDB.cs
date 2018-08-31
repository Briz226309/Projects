using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TravelExpertsClassLibrary
{
    /**
 * Author: Prince Nimoh /Jason M.
 * Date: July, 2018
 * Travel Experts.
 * DB class for the Products_Supplers Table
 * */
    public class ProductsSuppliersDB
    {
        /// <summary>
        /// Addes a new product supplier relationship to the the product 
        /// supplier table. The product and supplier IDs provided must already belong to
        /// a product and a supplier in their respective tables.
        /// </summary>
        /// <param name="productID">Product ID to add</param>
        /// <param name="supplierID">Supplier ID to add</param>
        /// <returns></returns>
        public static int AddProductsSuppliers(int productID, int supplierID)
        {
            SqlConnection con = TravelExpertsDB.GetConnection();
            string insertStatement = "INSERT INTO Products_Suppliers (ProductId, SupplierId) " +
                "VALUES (@ProductId, @SupplierId)";
            SqlCommand cmd = new SqlCommand(insertStatement, con);
            cmd.Parameters.AddWithValue("@ProductId", productID);
            cmd.Parameters.AddWithValue("@SupplierId", supplierID);
            try
            {
                con.Open();
                cmd.ExecuteNonQuery(); // run insert command

                // get the generated ID - current identity value from the Products_Suppliers table
                string selectQuery = "SELECT IDENT_CURRENT('Products_Suppliers') FROM Products_Suppliers";
                SqlCommand selectCmd = new SqlCommand(selectQuery, con);
                int productSupplierId = Convert.ToInt32(selectCmd.ExecuteScalar());
                return productSupplierId;
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
        /// Returns all the products suppliers from the Travel experts DB
        /// </summary>
        /// <returns>a list of products suppliers</returns>
        public static List<ProductSupplier> GetAllProductSuppliers()
        {
            List<ProductSupplier> productSuppliers = new List<ProductSupplier>();
            ProductSupplier currentProductSupplier = null;
            SqlConnection con = TravelExpertsDB.GetConnection();


            /*
             select ProductSupplierId, p.ProductId, ProdName, s.SupplierId, SupName
             from [dbo].[Products_Suppliers] ps
             inner join products p on ps.ProductId = p.ProductId
             inner join Suppliers s on ps.SupplierId = s.SupplierId
             */

            string selectStatement = "SELECT ProductSupplierId, p.ProductId as ProductId, ProdName, s.SupplierId as SupplierId, SupName "
                                      + " From Products_Suppliers ps"
                                      + " inner join products p on ps.ProductId = p.ProductId "
                                      + " inner join Suppliers s on ps.SupplierID = s.SupplierId "
                                      + " ORDER BY ProductSupplierId";

            SqlCommand cmd = new SqlCommand(selectStatement, con);

            try
            {
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read()) //While there are orders
                {
                    currentProductSupplier = new ProductSupplier();

                    currentProductSupplier.SupplierID = Convert.ToInt32(reader["SupplierId"]);
                    currentProductSupplier.SupName = Convert.ToString(reader["SupName"]);
                    currentProductSupplier.ProductSupplierID = Convert.ToInt32(reader["ProductSupplierId"]);
                    currentProductSupplier.ProductID = Convert.ToInt32(reader["ProductId"]);
                    currentProductSupplier.ProductName = Convert.ToString(reader["ProdName"]);

                    productSuppliers.Add(currentProductSupplier);
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
            return productSuppliers;
        }

        /// <summary>
        /// Adds a new product supplier to Travel Experss DB
        /// </summary>
        /// <param name="productSupplier"> ProductSupplier object that contains data for the new record</param>
        /// <returns>new productSupplier id</returns>
        public static int AddProductSupplier(ProductSupplier productSupplier)
        {

            SqlConnection con = TravelExpertsDB.GetConnection();
            string insertStatement = "INSERT INTO Products_Suppliers (ProductId, SupplierId) " +
                "VALUES (@ProductId, @SupplierId)";
            SqlCommand cmd = new SqlCommand(insertStatement, con);
            cmd.Parameters.AddWithValue("@ProductId", productSupplier.ProductID);
            cmd.Parameters.AddWithValue("@SupplierId", productSupplier.SupplierID);
            try
            {
                con.Open();
                cmd.ExecuteNonQuery(); // run insert command

                // get the generated ID - current identity value from the Products_Suppliers table
                string selectQuery = "SELECT IDENT_CURRENT('Products_Suppliers') FROM Products_Suppliers";
                SqlCommand selectCmd = new SqlCommand(selectQuery, con);
                int productSupplierId = Convert.ToInt32(selectCmd.ExecuteScalar());
                return productSupplierId;
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
        /// Deletes a productSupplier from the travel experts database
        /// Uses optimistic concurrency to ensure that the data hasn't changed since we
        /// last read from the database.
        /// </summary>
        /// <param name="prodSup">ProductSupplier to delete</param>
        /// <returns>Bool value indicating success</returns>
        public static bool DeleteProductSupplier(ProductSupplier prodSup)
        {
            SqlConnection con = TravelExpertsDB.GetConnection();
            string deleteStatement = "DELETE FROM Products_Suppliers " +
                                     "WHERE ProductSupplierId = @ProductSupplierId " + // to identify the productSupplier to be  deleted
                                     " AND ProductId = @ProductId " + // remaining conditions - to ensure optimistic concurrency
                                     " AND SupplierId = @SupplierId";


            SqlCommand cmd = new SqlCommand(deleteStatement, con);
            cmd.Parameters.AddWithValue("@ProductSupplierId", prodSup.ProductSupplierID);
            cmd.Parameters.AddWithValue("@SupplierId", prodSup.SupplierID);
            cmd.Parameters.AddWithValue("@ProductId", prodSup.ProductID);

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
        /// Updates existing productsupplier record.
        /// Uses optimistic concurrency to ensure that the data hasn't changed
        /// since we last read from the database.
        /// </summary>
        /// <param name="oldProdSup">data before update</param>
        /// <param name="newProdSup">new data for the update</param>
        /// <returns>indicator of success</returns>
        public static bool UpdateProductSupplier(ProductSupplier oldProdSup, ProductSupplier newProdSup)
        {
            SqlConnection con = TravelExpertsDB.GetConnection();
            string updateStatement = "UPDATE Products_Suppliers " +
                                     "SET ProductId = @NewProductId, SupplierId = @NewSupplpierID " +
                                     "WHERE ProductSupplierId = @ProductSupplierId " +
                                     "AND ProductId = @OldProductId " +//This is to check that the supplier name hasn't changed
                                     "AND SupplierId = @OldSupplierId"; 

            SqlCommand cmd = new SqlCommand(updateStatement, con);
            cmd.Parameters.AddWithValue("@NewProductId", newProdSup.ProductID);
            cmd.Parameters.AddWithValue("@NewSupplpierID", newProdSup.SupplierID);
            cmd.Parameters.AddWithValue("@ProductSupplierId", oldProdSup.ProductSupplierID);
            cmd.Parameters.AddWithValue("@OldProductId", oldProdSup.ProductID);
            cmd.Parameters.AddWithValue("@OldSupplierId", oldProdSup.SupplierID);

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
