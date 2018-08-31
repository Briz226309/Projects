/*
 * Date:  July 29, 2018
 * Author:  Corinne Mullan
 * 
 * The PackageDB.cs file contains the PackageDB class, with static methods for retrieving, adding 
 * and editing packages in the Packages table in the TravelExperts SQL Server database.
 * Also included here are methods for retrieving information from and updating the 
 * Packages_Products_Suppliers tables.
 */

using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace TravelExpertsClassLibrary
{
    public static class PackageDB
    {
        /// <summary>
        /// The GetPackage() method returns the information from the Packages table with
        /// the PackageId matching pkgId.
        /// </summary>
        /// <param name="pkgId"></param>
        /// <returns>Package object</returns>
        public static Package GetPackage(int pkgId)
        {

            Package pkg = null;       // Empty Package object for storing the results

            // Open a connection to the database
            SqlConnection con = TravelExpertsDB.GetConnection();

            // Define the SQL statement and execute the command for retrieving the package information.
            // At most a single row will be returned.
            string selectStatement = "SELECT PackageId, PkgName, PkgStartDate, PkgEndDate, " +
                                     "PkgDesc, PkgBasePrice, PkgAgencyCommission " +
                                     "FROM Packages " +
                                     "WHERE PackageId = @PackageID";

            SqlCommand cmd = new SqlCommand(selectStatement, con);
            cmd.Parameters.AddWithValue("@PackageId", pkgId);

            try
            {
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader(System.Data.CommandBehavior.SingleRow);

                // TO DO -- should any of these fields allow for null values?
                // If a package is found, populate the Package object
                if (reader.Read())
                {
                    pkg = new Package();
                    pkg.PackageId = Convert.ToInt32(reader["PackageId"]);
                    pkg.PackageName = reader["PkgName"].ToString();
                    pkg.PkgStartDate = (DateTime)reader["PkgStartDate"];
                    pkg.PkgEndDate = (DateTime)reader["PkgEndDate"];
                    pkg.PkgDesc = reader["PkgDesc"].ToString();
                    pkg.PkgBasePrice = Convert.ToDecimal(reader["PkgBasePrice"]);
                    pkg.PkgAgencyCommission = Convert.ToDecimal(reader["PkgAgencyCommission"]);
                }
            }
            // If unsuccessful, throw the exception (it will be an SqlException object)
            catch (SqlException ex)
            {
                throw ex;
            }
            // Close the database connection
            finally
            {
                con.Close();
            }

            return pkg;
        }

        /// <summary>
        /// The GetAllPackages() method returns a list of all packages from the Packages table.
        /// </summary>
        /// <returns>List of Package objects</returns>
        public static List<Package> GetAllPackages()
        {
            List<Package> pkgList = new List<Package>();    // Empty list for storing the packages
            Package pkg = null;                             // Empty package object for reading

            // Open a connection to the database
            SqlConnection con = TravelExpertsDB.GetConnection();

            // Define the SQL statement and execute the command for retrieving the package information.
            string selectStatement = "SELECT PackageId, PkgName, PkgStartDate, PkgEndDate, " +
                                     "PkgDesc, PkgBasePrice, PkgAgencyCommission " +
                                     "FROM Packages";

            SqlCommand cmd = new SqlCommand(selectStatement, con);

            try
            {
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                // TO DO -- should any of these fields allow for null values?
                // If a package is found, populate the Package object
                while (reader.Read())
                {
                    pkg = new Package();
                    pkg.PackageId = Convert.ToInt32(reader["PackageId"]);
                    pkg.PackageName = reader["PkgName"].ToString();
                    pkg.PkgStartDate = (DateTime)reader["PkgStartDate"];
                    pkg.PkgEndDate = (DateTime)reader["PkgEndDate"];
                    pkg.PkgDesc = reader["PkgDesc"].ToString();
                    pkg.PkgBasePrice = Convert.ToDecimal(reader["PkgBasePrice"]);
                    pkg.PkgAgencyCommission = Convert.ToDecimal(reader["PkgAgencyCommission"]);

                    pkgList.Add(pkg);
                }
            }
            // If unsuccessful, throw the exception (it will be an SqlException object)
            catch (SqlException ex)
            {
                throw ex;
            }
            // Close the database connection
            finally
            {
                con.Close();
            }

            return pkgList;
        }

        /// <summary>
        /// The AddPackage() method adds a new row to the Packages table, using the data in the
        /// package object pkg passed to it.
        /// </summary>
        /// <param name="pkg"></param>
        /// <returns>pkgId of the new record</returns>
        public static int AddPackage(Package pkg)
        {
            // Open a connection to the database
            SqlConnection con = TravelExpertsDB.GetConnection();

            // Define the SQL statement and execute the command for inserting the new package.
            string selectStatement = "INSERT INTO Packages " +
                                     "(PkgName, PkgStartDate, PkgEndDate, " +
                                     "PkgDesc, PkgBasePrice, PkgAgencyCommission) " +
                                     "VALUES (@PkgName, @PkgStartDate, @PkgEndDate," +
                                     "@PkgDesc, @PkgBasePrice, @PkgAgencyCommission)";

            SqlCommand cmd = new SqlCommand(selectStatement, con);
            cmd.Parameters.AddWithValue("@PkgName", pkg.PackageName);
            cmd.Parameters.AddWithValue("@PkgStartDate", pkg.PkgStartDate);
            cmd.Parameters.AddWithValue("@PkgEndDate", pkg.PkgEndDate);
            cmd.Parameters.AddWithValue("@PkgDesc", pkg.PkgDesc);
            cmd.Parameters.AddWithValue("@PkgBasePrice", pkg.PkgBasePrice);
            cmd.Parameters.AddWithValue("@PkgAgencyCommission", pkg.PkgAgencyCommission);

            try
            {
                con.Open();
                cmd.ExecuteNonQuery();

                // Obtain the packageId of the newly created record
                string selectPkgId = "SELECT IDENT_CURRENT('Packages') FROM Packages";
                SqlCommand selectCmd = new SqlCommand(selectPkgId, con);
                int pkgId = Convert.ToInt32(selectCmd.ExecuteScalar());

                return pkgId;
            }
            // If unsuccessful, throw the exception (it will be an SqlException object)
            catch (SqlException ex)
            {
                throw ex;
            }
            // Close the database connection
            finally
            {
                con.Close();
            }
        }

        /// <summary>
        /// The DeletePackage() methods deletes the specified package from the Packages table.
        /// Optimistic concurrency is enforced by ensuring that all of the database fields
        /// match the properties in the pkg object.
        /// </summary>
        /// <param name="pkg"></param>
        /// <returns>true on success, false otherwise</returns>
        public static bool DeletePackage(Package pkg)
        {
            // Open a connection to the database
            SqlConnection con = TravelExpertsDB.GetConnection();

            // Define the SQL statement and execute the command for deleting the package.  
            // Allow for null values where the database allows nulls.
            string deleteStatement = "DELETE FROM Packages " +
                                     "WHERE PackageId = @PackageId " +
                                     "AND PkgName = @PkgName " +
                                     "AND (PkgStartDate = @PkgStartDate OR (PkgStartDate IS NULL AND @PkgStartDate IS NULL)) " +
                                     "AND (PkgEndDate = @PkgEndDate OR (PkgEndDate IS NULL AND @PkgEndDate IS NULL)) " +
                                     "AND (PkgDesc = @PkgDesc OR (PkgDesc IS NULL AND @PkgDesc IS NULL)) " +
                                     "AND PkgBasePrice = @PkgBasePrice " +
                                     "AND (PkgAgencyCommission = @PkgAgencyCommission OR (PkgAgencyCommission IS NULL AND @PkgAgencyCommission IS NULL))";

            SqlCommand cmd = new SqlCommand(deleteStatement, con);
            cmd.Parameters.AddWithValue("@PackageId", pkg.PackageId);
            cmd.Parameters.AddWithValue("@PkgName", pkg.PackageName);
            cmd.Parameters.AddWithValue("@PkgStartDate", pkg.PkgStartDate);
            cmd.Parameters.AddWithValue("@PkgEndDate", pkg.PkgEndDate);
            cmd.Parameters.AddWithValue("@PkgDesc", pkg.PkgDesc);
            cmd.Parameters.AddWithValue("@PkgBasePrice", pkg.PkgBasePrice);
            cmd.Parameters.AddWithValue("@PkgAgencyCommission", pkg.PkgAgencyCommission);

            try
            {
                con.Open();
                int result = cmd.ExecuteNonQuery();

                if (result > 0)
                    return true;
                else
                    return false;

            }
            // If unsuccessful, throw the exception (it will be an SqlException object)
            catch (SqlException ex)
            {
                throw ex;
            }
            // Close the database connection
            finally
            {
                con.Close();
            }
        }

        /// <summary>
        /// The UpdatePackage() method updates the data for the specified package in the 
        /// Packages table.  Optimistic concurrency is enforced by ensuring that all of the 
        /// database fields match the properties in the original pkg object.
        /// </summary>
        /// <param name="pkg"></param>
        /// <returns>true on success, false otherwise</returns>
        public static bool UpdatePackage(Package original_Package, Package package)
        {
            // Open a connection to the database
            SqlConnection con = TravelExpertsDB.GetConnection();

            // Define the SQL statement and execute the command for deleting the package.  
            // Allow for null values where the database allows nulls.
            string updateStatement = "UPDATE Packages " +
                                     "SET PkgName = @PkgName, " +
                                         "PkgStartDate = @PkgStartDate, " +
                                         "PkgEndDate = @PkgEndDate, " +
                                         "PkgDesc = @PkgDesc, " +
                                         "PkgBasePrice = @PkgBasePrice, " +
                                         "PkgAgencyCommission = @PkgAgencyCommission " +
                                     "WHERE PackageId = @OldPackageId " +
                                     "AND PkgName = @OldPkgName " +
                                     "AND (PkgStartDate = @OldPkgStartDate OR (PkgStartDate IS NULL AND @OldPkgStartDate IS NULL))" +
                                     "AND (PkgEndDate = @OldPkgEndDate OR (PkgEndDate IS NULL AND @OldPkgEndDate IS NULL)) " +
                                     "AND (PkgDesc = @OldPkgDesc OR (PkgDesc IS NULL AND @OldPkgDesc IS NULL)) " +
                                     "AND PkgBasePrice = @OldPkgBasePrice " +
                                     "AND (PkgAgencyCommission = @OldPkgAgencyCommission OR (PkgAgencyCommission IS NULL AND @OldPkgAgencyCommission IS NULL))";

            SqlCommand cmd = new SqlCommand(updateStatement, con);
            cmd.Parameters.AddWithValue("@OldPackageId", original_Package.PackageId);
            cmd.Parameters.AddWithValue("@OldPkgName", original_Package.PackageName);
            cmd.Parameters.AddWithValue("@OldPkgStartDate", original_Package.PkgStartDate);
            cmd.Parameters.AddWithValue("@OldPkgEndDate", original_Package.PkgEndDate);
            cmd.Parameters.AddWithValue("@OldPkgDesc", original_Package.PkgDesc);
            cmd.Parameters.AddWithValue("@OldPkgBasePrice", original_Package.PkgBasePrice);
            cmd.Parameters.AddWithValue("@OldPkgAgencyCommission", original_Package.PkgAgencyCommission);
            cmd.Parameters.AddWithValue("@PkgName", package.PackageName);
            cmd.Parameters.AddWithValue("@PkgStartDate", package.PkgStartDate);
            cmd.Parameters.AddWithValue("@PkgEndDate", package.PkgEndDate);
            cmd.Parameters.AddWithValue("@PkgDesc", package.PkgDesc);
            cmd.Parameters.AddWithValue("@PkgBasePrice", package.PkgBasePrice);
            cmd.Parameters.AddWithValue("@PkgAgencyCommission", package.PkgAgencyCommission);

            try
            {
                con.Open();
                int result = cmd.ExecuteNonQuery();

                if (result > 0)
                    return true;
                else
                    return false;

            }
            // If unsuccessful, throw the exception (it will be an SqlException object)
            catch (SqlException ex)
            {
                throw ex;
            }
            // Close the database connection
            finally
            {
                con.Close();
            }
        }

        /// <summary>
        /// The GetPackageProductSuppliers() retrieves all products associated with the current package.
        /// </summary>
        /// <param name="pkgId"></param>
        /// <returns>A list of products</returns>
        public static List<ProductSupplier> GetPackageProductSuppliers(int pkgId)
        {
            List<ProductSupplier> prodSupList = new List<ProductSupplier>();    // Empty list for storing the products/suppliers
            ProductSupplier ps = null;                  // Empty package object for reading

            // Open a connection to the database
            SqlConnection con = TravelExpertsDB.GetConnection();

            // Define the SQL statement and execute the command for retrieving the products/suppliers 
            // associated with a package
            string selectStatement = "SELECT pps.ProductSupplierId as ProductSupplierId, " +
                                     "p.ProductId as ProductId, " +
                                     "ProdName, s.SupplierId as SupplierId, SupName " +
                                     "FROM Packages_Products_Suppliers pps " +
                                     "INNER JOIN Products_Suppliers ps ON pps.ProductSupplierId = ps.ProductSupplierId " +
                                     "INNER JOIN Products p ON ps.ProductId = p.ProductId " +
                                     "INNER JOIN Suppliers s ON ps.SupplierID = s.SupplierID " +
                                     "WHERE PackageId = @PackageId";

            SqlCommand cmd = new SqlCommand(selectStatement, con);
            cmd.Parameters.AddWithValue("@PackageId", pkgId);

            try
            {
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    ps = new ProductSupplier();
                    ps.ProductSupplierID = Convert.ToInt32(reader["ProductSupplierId"]);
                    ps.ProductID = Convert.ToInt32(reader["ProductId"]);
                    ps.ProductName = reader["ProdName"].ToString();
                    ps.SupplierID = Convert.ToInt32(reader["SupplierId"]);
                    ps.SupName = reader["SupName"].ToString();

                    prodSupList.Add(ps);
                }

                return prodSupList;
            }
            // If unsuccessful, throw the exception (it will be an SqlException object)
            catch (SqlException ex)
            {
                throw ex;
            }
            // Close the database connection
            finally
            {
                con.Close();
            }
        }

        /// <summary>
        /// The GetOtherProductSuppliers() retrieves all other products/suppliers that can be added
        /// to the package.
        /// </summary>
        /// <param name="pkgId"></param>
        /// <returns>A list of other available products/suppliers</returns>
        public static List<ProductSupplier> GetOtherProductSuppliers(int pkgId)
        {
            List<ProductSupplier> prodSupList = new List<ProductSupplier>();    // Empty list for storing the products/suppliers
            ProductSupplier ps = null;                  // Empty package object for reading

            // Open a connection to the database
            SqlConnection con = TravelExpertsDB.GetConnection();

            // Define the SQL statement and execute the command for retrieving the products/suppliers 
            // associated with a package
            string selectStatement = "SELECT pps.ProductSupplierId as ProductSupplierId, " +
                                     "p.ProductId as ProductId, ProdName, " +
                                     "s.SupplierId as SupplierId, SupName " +
                                     "FROM Packages_Products_Suppliers pps " +
                                     "INNER JOIN Products_Suppliers ps ON pps.ProductSupplierId = ps.ProductSupplierId " +
                                     "INNER JOIN Products p ON ps.ProductId = p.ProductId " +
                                     "INNER JOIN Suppliers s ON ps.SupplierID = s.SupplierID " +
                                     "WHERE pps.ProductSupplierID NOT IN " +
                                     "(SELECT ProductSupplierID FROM Packages_Products_Suppliers " + 
                                     "WHERE PackageId = @PackageId)";

            SqlCommand cmd = new SqlCommand(selectStatement, con);
            cmd.Parameters.AddWithValue("@PackageId", pkgId);

            try
            {
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    ps = new ProductSupplier();
                    ps.ProductSupplierID = Convert.ToInt32(reader["ProductSupplierId"]);
                    ps.ProductID = Convert.ToInt32(reader["ProductId"]);
                    ps.ProductName = reader["ProdName"].ToString();
                    ps.SupplierID = Convert.ToInt32(reader["SupplierId"]);
                    ps.SupName = reader["SupName"].ToString();

                    prodSupList.Add(ps);
                }

                return prodSupList;
            }
            // If unsuccessful, throw the exception (it will be an SqlException object)
            catch (SqlException ex)
            {
                throw ex;
            }
            // Close the database connection
            finally
            {
                con.Close();
            }
        }

        /// <summary>
        /// The AddProductToPackage() method adds a new record to the Packages_Products_Suppliers
        /// table, in order to add a new product to a package.  
        /// </summary>
        /// <param name="pkgId", "prodSupId"></param>
        /// <returns>true if successful</returns>
        public static bool AddProductToPackage(int pkgId, int prodSupId)
        {
            // Open a connection to the database
            SqlConnection con = TravelExpertsDB.GetConnection();

            // Define the SQL statement and execute the command for retrieving the package information.
            // At most a single row will be returned.
            string selectStatement = "INSERT INTO Packages_Products_Suppliers " +
                                     "(PackageId, ProductSupplierId) " +
                                     "VALUES (@PackageID, @ProductSupplierId)";

            SqlCommand cmd = new SqlCommand(selectStatement, con);
            cmd.Parameters.AddWithValue("@PackageId", pkgId);
            cmd.Parameters.AddWithValue("@ProductSupplierId", prodSupId);

            try
            {
                con.Open();
                cmd.ExecuteNonQuery();
            }
            // If unsuccessful, throw the exception (it will be an SqlException object)
            catch (SqlException ex)
            {
                throw ex;
            }
            // Close the database connection
            finally
            {
                con.Close();
            }

            return true;
        }

    }
}

