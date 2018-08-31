/*
 * Date:  July 20, 2018
 * Author:  Corinne Mullan
 * 
 * The PackageValidation.cs file contains validation methods for the package data.
 * Exceptions are thrown if any of the data fails the validation rules.
 * 
 */
 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace TravelExpertsClassLibrary
{
    public static class PackageValidation
    {
        public static bool ValidatePackageData(Package pkg)
        {
            // Check that the agency commission not greater than the base price
            if (pkg.PkgAgencyCommission > pkg.PkgBasePrice)
            {
                throw new Exception("Error:  the agency commission cannot be greater than the base price");
            }

            // Check that the package end date is later than the package start date
            if (pkg.PkgEndDate <= pkg.PkgStartDate)
            {
                throw new Exception("Error:  the package end date must be later than the package start date");
            }

            // Check that the package name is not null
            if (pkg.PackageName == "")
            {
                throw new Exception("Error:  the package name cannot be null");
            }

            // Check that the package description is not null
            if (pkg.PkgDesc == "")
            {
                throw new Exception("Error:  the package description cannot be null");
            }

            return true;
        }
    }
}
