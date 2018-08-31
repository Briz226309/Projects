/*
 * Date:  July 29, 2018
 * Author:  Corinne Mullan
 * 
 * The Package.cs file contains the Package class, including properties, constructor, and methods.
 * A Package object models a travel package and its properties match the fields in the "packages"
 * table in the database.
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TravelExpertsClassLibrary
{
    public class Package
    {
        // Private data and public accessor properties
        // Accessor names match the field names in the "packages" table in the database
        public int PackageId { get; set; }          // The PackageId (auto-incremented in the database table)

        public string PackageName { get; set; }     // The package name

        public DateTime PkgStartDate { get; set; }  // The package start date

        private DateTime pkgEndDate;                // The package end date.  Must be >= the start date.
        public DateTime PkgEndDate
        {
            get { return pkgEndDate; }
            set { pkgEndDate = (value < PkgStartDate) ? PkgStartDate : value; }
        }

        public string PkgDesc { get; set; }     // The package description

        private decimal pkgBasePrice;               // The base price (must be > 0)
        public decimal PkgBasePrice
        {
            get { return pkgBasePrice; }
            set { pkgBasePrice = (value < 0) ? 0 : value; }
        }

        private decimal pkgAgencyCommission;        // The agency commission (must be > 0)
        public decimal PkgAgencyCommission
        {
            get { return pkgAgencyCommission; }
            set { pkgAgencyCommission = (value < 0) ? 0 : value; }
        }

        // Public constructor
        // The constructor uses default values if no parameters are supplied.
        public Package(int id = 0, string name = "", DateTime start = default(DateTime), DateTime end = default(DateTime), string desc = "",
                       decimal price = 0, decimal comm = 0)
        {
            PackageId = id;
            PackageName = name;
            PkgStartDate = start;
            PkgEndDate = end;
            PkgDesc = desc;
            PkgBasePrice = price;
            PkgAgencyCommission = comm;
        }
    }
}
