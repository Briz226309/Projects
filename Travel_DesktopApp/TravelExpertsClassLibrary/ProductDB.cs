using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TravelExpertsClassLibrary
{
    public static class ProductDB
    {
        
        public static List<Product> GetProducts()
        {
            List<Product> products = new List<Product>();
            Product prod;
            SqlConnection con = TravelExpertsDB.GetConnection();
            // select columns ProductsID and ProdName from Products table and order by ProductID; from TravelExperts Database
            string selectStatement = "SELECT ProductID, ProdName " +
                                     "FROM Products ORDER BY ProductID";

            SqlCommand selectCmd = new SqlCommand(selectStatement, con);

            try
            {
                // open connection and read ProductID and ProdName columns from Products table
                con.Open();
                SqlDataReader dr = selectCmd.ExecuteReader();
                while (dr.Read())
                {
                    prod = new Product();
                    prod.ProductID = (int)dr["ProductID"];
                    prod.ProdName = (string)dr["ProdName"];
                    products.Add(prod);
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
            return products;
        }

        public static int AddProducts(Product prod)
        {
            SqlConnection con = TravelExpertsDB.GetConnection();
            string insertStatement = "INSERT INTO Products (ProdName) " +
                "VALUES (@ProdName)";
            SqlCommand cmd = new SqlCommand(insertStatement, con);
            cmd.Parameters.AddWithValue("@ProdName", prod.ProdName);
            try
            {
                con.Open();
                cmd.ExecuteNonQuery(); // run insert command

                // get the generated ID - current identity value for Products table
                string selectQuery = "SELECT IDENT_CURRENT('Products') FROM Products";
                SqlCommand selectCmd = new SqlCommand(selectQuery, con);
                int ProductID = Convert.ToInt32(selectCmd.ExecuteScalar());              
                return ProductID;
            }
            catch(SqlException ex)
            {
                throw ex;
            }
            finally
            {
                con.Close();
            }
        }
        public static bool DeleteProduct(Product prod)
        {
            SqlConnection con = TravelExpertsDB.GetConnection();
            string deleteStatement = "DELETE FROM Products " +
                                     "WHERE ProductID = @ProductID " +
                                     "AND ProdName = @ProdName ";
            SqlCommand cmd = new SqlCommand(deleteStatement, con);
            cmd.Parameters.AddWithValue("@ProductID", prod.ProductID);
            cmd.Parameters.AddWithValue("@ProdName", prod.ProdName);
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
        /// Coded by Jason McIntyre
        /// Student ID: 000788654
        /// SAIT OOSD Workshop Project 4
        /// Updates existing customer record
        /// </summary>
        /// <param name="oldProd">data before update</param>
        /// <param name="newProd">new data for the update</param>
        /// <returns>indicator of success</returns>
        public static bool UpdateProduct(Product oldProd, Product newProd)
        {
            SqlConnection con = TravelExpertsDB.GetConnection();
            string updateStatement = "UPDATE Products " +
                                     "SET ProdName = @NewProdName " +
                                     "WHERE ProductID = @OldProductID " +
                                     "AND ProdName = @OldProdName ";
            SqlCommand cmd = new SqlCommand(updateStatement, con);
           
            cmd.Parameters.AddWithValue("@NewProdName", newProd.ProdName);
            cmd.Parameters.AddWithValue("@OldProductID", oldProd.ProductID);
            cmd.Parameters.AddWithValue("@OldProdName", oldProd.ProdName);

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
