/*
 * Date:    July 29, 2018
 * Author:  Corinne Mullan
 * 
 * The ProductSupplier.cs file defines the ProductSupplier class.
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TravelExpertsClassLibrary
{
    public class ProductSupplier
    {
        public int ProductSupplierID { get; set; }
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public int SupplierID { get; set; }
        public string SupName { get; set; }

        public ProductSupplier GetCopy()
        {
            ProductSupplier copy = new ProductSupplier();

            copy.ProductSupplierID = this.ProductSupplierID;
            copy.ProductID = this.ProductID;
            copy.ProductName = this.ProductName;
            copy.SupplierID = this.SupplierID;
            copy.SupName = this.SupName;

            return copy;
        }
    }
}
