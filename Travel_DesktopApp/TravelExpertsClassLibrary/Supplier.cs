using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TravelExpertsClassLibrary
{
    /**
    * Supplier class that contains the properties and methods for travel experts suppliers
    * Author: Prince Nimoh
    * Context: Threaded Project, Workshop 4 and 5
    * Date: July 8, 2018
    * */
    public class Supplier
    {
        public int SupplierID { get; set; } //property for supplier id

        public string SupplierName { get; set; } //property for supplier name

        //Default constructor for the supplier class
        public Supplier()
        {
        }

        //Parameterized contructor
        //Takes the supplier ID and Supplier name as parameters
        public Supplier(int id, string name)
        {
            SupplierID = id;
            SupplierName = name;
        }

        //To string method for the supplier class
        public override string ToString()
        {
            return SupplierID + " " + SupplierName ;
        }

        //Returns a copy of the supplier
        public Supplier GetCopy()
        {
            Supplier copy = new Supplier();

            copy.SupplierID = this.SupplierID;
            copy.SupplierName = this.SupplierName;

            return copy;
        }
    }
}
