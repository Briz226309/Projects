using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TravelExpertsClassLibrary
{
    public class Product
    {
        public Product() { }

        public int ProductID { get; set; }

        public string ProdName { get; set; }

        public Product GetCopy()
        {
            Product copy = new Product(); //Make a new product

            //Copy the values of the fields to the new product
            copy.ProductID = this.ProductID;
            copy.ProdName = this.ProdName;

            return copy;

        }
    }
}
