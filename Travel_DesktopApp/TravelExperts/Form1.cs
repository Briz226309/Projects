/*
 *  Author :  Brijesh Golakiya
 *  Date   : 25/07/2018
 * 
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TravelExpertsClassLibrary;

namespace TravelExperts
{
    public partial class Form1 : Form
    {
        List<Panel> listPanelProduct = new List<Panel>();
        List<Panel> listPanelPackage = new List<Panel>();
        List<Panel> listPanelSupplier= new List<Panel>();
        List<Product> products;
        List<Supplier> suppliers;
        List<Package> packages;
        Product delProduct;
        Supplier supplier;
        List<ProductSupplier> productSuppliers; //This is only instantiated the first time the product supplier tab is activated

       

        public Form1()
        {
            InitializeComponent();
            tabControlTravel.SelectedIndex = 0;   // by default tab is first tab
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            

            //add panels in product tab
            listPanelProduct.Add(panelProductAdd);
            listPanelProduct.Add(panelProductEdit);

            //add panels of package tab
            listPanelPackage.Add(panelAddPkg);
            listPanelPackage.Add(panelEditPkg);

            //add panels of supplier tab
            listPanelSupplier.Add(panelAddSupplier);
            listPanelSupplier.Add(panelEditSupplier);

            //hide all panels when form loads
            panelProductAdd.Visible = false;
            panelProductEdit.Visible = false;
            panelAddPkg.Visible = false;
            panelEditPkg.Visible = false;
            panelAddSupplier.Visible = false;
            panelEditSupplier.Visible = false;
            panelDetailPkg.Visible = false;

            //Load products data in gridview from database
            try
            {
                products = ProductDB.GetProducts();

                productBindingSource.DataSource = products;

                //load supplier data
                suppliers = SupplierDB.GetAllSuppliers();
                //supplierNameComboBox.DataSource = suppliers;
                //comboBoxPEditSupID.DataSource = suppliers;
               

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.GetType().ToString());
            }

            //Load supplier data in gridview from database

            try
            {
                suppliers = SupplierDB.GetAllSuppliers();

                supplierDataGridView.DataSource = suppliers;
                

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.GetType().ToString());
            }
        }

        //user clicks on add product button
        private void btnAddProductClick_Click(object sender, EventArgs e)
        {
            btnEditProductClick.Enabled = false;
            btnDeleteProductClick.Enabled = false;
            
            //allow user to add product
            
            panelProductAdd.Visible = true;
            listPanelProduct[0].BringToFront();

            //add data to the products table
            //Load data from DB
           
        }

        private void btnEditProductClick_Click(object sender, EventArgs e)
        {
           
            btnAddProductClick.Enabled = false;
            btnDeleteProductClick.Enabled = false;

            //allow user to edit proudct
            panelProductEdit.Visible = true;
            listPanelProduct[1].BringToFront();

            //Get the index of the product selected in the gridview
            int selectedIndex = productDataGridView.CurrentCell.RowIndex;

            //Get the product to be edited
            Product prodToEdit = products[selectedIndex];

            //Display the product to be edited
            textBoxEditProID.Text = prodToEdit.ProductID.ToString();
            textBoxEditProName.Text = prodToEdit.ProdName;

        }

        private void btnCancelAddProduct_Click(object sender, EventArgs e)
        {
            panelProductAdd.Visible = false;
            btnEditProductClick.Enabled = true;
            btnDeleteProductClick.Enabled = true;
        }

        private void btnEditProduct_Click(object sender, EventArgs e)
        {
            
            string pronameText = textBoxEditProName.Text;
            pronameText = pronameText.Trim();
            if (pronameText != "")
            {
                //Get the product to edit
                int selectedIndex = productDataGridView.CurrentCell.RowIndex;
                Product oldProd = products[selectedIndex];
                //Make a copy
                Product newProd = oldProd.GetCopy();
                //Update the copy with new data
                newProd.ProdName = textBoxEditProName.Text;

                if (newProd.ProdName != oldProd.ProdName) //Check if a change was made
                {
                    //Write to DB
                    try
                    {
                       bool result =  ProductDB.UpdateProduct(oldProd, newProd);

                        if (result)
                        {
                            //Update the UI
                            products = ProductDB.GetProducts();
                            productDataGridView.DataSource = products;

                            MessageBox.Show("The new product name has been saved", "Product Updated",
                                             MessageBoxButtons.OK, MessageBoxIcon.Information);

                        }
                        else
                        {
                            MessageBox.Show("The changes were not saved", "Updated Failed",
                                             MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, ex.GetType().ToString(), 
                                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }

                //Hide the panel if updates were done
                panelProductEdit.Visible = false;
                btnAddProductClick.Enabled = true;
                btnDeleteProductClick.Enabled = true;
            }
            else //Display error message if Product name textbox was blank.
            {
                MessageBox.Show("Please provide product name.", "Missing Data", 
                                 MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnCancelEditProduct_Click(object sender, EventArgs e)
        {
            panelProductEdit.Visible = false;
            btnAddProductClick.Enabled = true;
            btnDeleteProductClick.Enabled = true;
        }

        //get data from selected row of datagridview 
        private void productDataGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {

            int index = e.RowIndex;
            if(index >= 0)
            {
                DataGridViewRow selectedRow = productDataGridView.Rows[index];
                int selectedprodID = (int)selectedRow.Cells[0].Value;
                string selectedprodName = selectedRow.Cells[1].Value.ToString();
                delProduct = new Product();
                delProduct.ProductID = selectedprodID;
                delProduct.ProdName = selectedprodName;
            }
            else
            {
                delProduct = null;
            }    
               
        }

        private void btnDeleteProductClick_Click(object sender, EventArgs e)
        {
            if (delProduct != null)
            {
                
                DialogResult result = MessageBox.Show("Are you sure you want to delete " + delProduct.ProdName
                                 + " product ID " + delProduct.ProductID + "? ", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question,
                                 MessageBoxDefaultButton.Button2);
                if (result == DialogResult.Yes)
                {
                    try
                    {
                        ProductDB.DeleteProduct(delProduct);
                        products = ProductDB.GetProducts();
                        productDataGridView.DataSource = products;
                        delProduct = null;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, ex.GetType().ToString());
                    }
                }
            }
            else 
            {
                MessageBox.Show("Please select a product to delete", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //to add supplier
        private void btnAddSupClick_Click(object sender, EventArgs e)
        {
            btnEditSupClick.Enabled = false;
            btnDelelteSupClick.Enabled = false;

            //allow user to edit proudct
            panelAddSupplier.Visible = true;
            listPanelSupplier[1].BringToFront();
        }

        /// <summary>
        /// Event is fired when the add button on the supplier tab is clicked.
        /// 
        /// </summary>
        /// <param name="sender">The button that was clicked</param>
        /// <param name="e">The arguements of the event</param>
        private void btnAddSupplier_Click(object sender, EventArgs e)
        {
            //Get the information from the text box
            string newSupName = txtboxAddSupName.Text;
            newSupName = newSupName.Trim();

            if (newSupName != "") //A value was provided
            {
                //Create a new supplier and assign the supplier name provided to the 
                //SupplierName property
                Supplier newSupplier = new Supplier();
                newSupplier.SupplierName = newSupName;

                //Write the new supplier to the DB
                try
                {
                    int newSupID = SupplierDB.AddSupplier(newSupplier);
                    
                    //Read supplier data again
                    suppliers = SupplierDB.GetAllSuppliers();
                    supplierDataGridView.DataSource = suppliers;

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error Message");
                }

                //setup the UI after adding a new supplier
                panelAddSupplier.Visible = false;
                btnDelelteSupClick.Enabled = true;
                btnEditSupClick.Enabled = true;
            }
            else
            {
                MessageBox.Show("Please provide the suppliers name", "Required information");
            }
            txtboxAddSupName.Text = "";
        }

        private void btnCancelAddSupplier_Click(object sender, EventArgs e)
        {
            panelAddSupplier.Visible = false;
            btnDelelteSupClick.Enabled = true;
            btnEditSupClick.Enabled = true;
        }

        /// <summary>
        /// Edit supplier button allows the user to edit the currently selected supplier
        /// For suppliers on the supplier name is an editable field
        /// </summary>
        /// <param name="sender"> The button that sends the event</param>
        /// <param name="e">Parameters on the event</param>
        private void btnEditSupClick_Click(object sender, EventArgs e)
        {
            btnAddSupClick.Enabled = false;
            btnDelelteSupClick.Enabled = false;

            //allow user to edit proudct
            panelEditSupplier.Visible = true;

            //Get the selected supplier 
            Supplier supplierToEdit = suppliers[supplierDataGridView.CurrentCell.RowIndex];

            //Display the current supplier information so the user can update
            txtboxEditSupID.Text = supplierToEdit.SupplierID.ToString();
            txtboxEditSupName.Text = supplierToEdit.SupplierName;
        }

        private void btnEditSupplier_Click(object sender, EventArgs e)
        { 
            //Get the supplier name value from the textbox.
            string updatedSupName = txtboxEditSupName.Text;
            updatedSupName =  updatedSupName.Trim();

            if (updatedSupName != "")//check if a value was provided
            {
                Supplier oldSupplier = suppliers[supplierDataGridView.CurrentCell.RowIndex];
                if (updatedSupName != oldSupplier.SupplierName)
                {
                    //Make a copy of the supplier we are updating
                    Supplier newSupplier = oldSupplier.GetCopy();

                    //Update the information in the new supplier;
                    newSupplier.SupplierName = updatedSupName;

                    try
                    {
                        bool result = SupplierDB.UpdateSupplier(oldSupplier, newSupplier);
                        if (result)
                        {

                            //Update the UI
                            suppliers = SupplierDB.GetAllSuppliers();
                            supplierDataGridView.DataSource = suppliers;
                            

                            MessageBox.Show("The new supplier name has been saved", "Supplier Updated",
                                             MessageBoxButtons.OK, MessageBoxIcon.Information);

                        }
                        else
                        {
                            MessageBox.Show("The changes were not saved", "Updated Failed",
                                             MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }

                        panelEditSupplier.Visible = false;
                        btnDelelteSupClick.Enabled = true;
                        btnAddSupClick.Enabled = true;

                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, ex.GetType().ToString(),
                                             MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    
                }
            }
        }

        private void btnCancelEditSupplier_Click(object sender, EventArgs e)
        {
            panelEditSupplier.Visible = false;
            btnDelelteSupClick.Enabled = true;
            btnAddSupClick.Enabled = true;
        }

        private void btnAddPkgClick_Click(object sender, EventArgs e)
        {
            panelDetailPkg.Visible = false;
            panelAddPkg.Visible = true;
            btnEditPkgClick.Enabled = false;
            btnDeletePkgClick.Enabled = false;
        }


        //add data to the database
        private void btnAddPkg_Click(object sender, EventArgs e)
        {
           
            DateTime newStartDate = new DateTime();
            DateTime newEndDate = new DateTime();
            decimal newBasePrice;
            decimal newCommission;

            // Check that the input data can be parsed
            if (!DateTime.TryParse(textBoxAddStartDate.Text, out newStartDate))
                MessageBox.Show("Please enter a date for the start date", "Error");
            if (!DateTime.TryParse(textBoxAddEndDate.Text, out newEndDate))
                MessageBox.Show("Please enter a date for the end date", "Error");

            if (!Decimal.TryParse(textBoxAddBasePrice.Text, out newBasePrice))
                MessageBox.Show("Please enter a decimal value for the base price", "Error");
            if (!Decimal.TryParse(textBoxAddComission.Text, out newCommission))
                MessageBox.Show("Please enter a decimal value for the agency commission", "Error");

            // Create the package object to be added
            Package newPkg = new Package(0, textBoxAddPkgName.Text, newStartDate, newEndDate, textBoxAddDesc.Text, newBasePrice, newCommission);

            // Validate the values of the input data.  If the input is valid, add the new package
            // to the Packages table in the database.
            try
            {
                if (PackageValidation.ValidatePackageData(newPkg))
                    PackageDB.AddPackage(newPkg);
                LoadUIforPackages();
                comboBoxPkgName.SelectedIndex = (packages.Count() - 1);

                panelDetailPkg.Visible = true;
                panelAddPkg.Visible = false;
                btnEditPkgClick.Enabled = true;
                btnDeletePkgClick.Enabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.GetType().ToString());
            }
            
        }

        //cancel adding data
        private void btnAddCancelPkg_Click(object sender, EventArgs e)
        {
            panelDetailPkg.Visible = true;
            panelAddPkg.Visible = false;
            btnEditPkgClick.Enabled = true;
            btnDeletePkgClick.Enabled = true;
        }

        //show form to edit the database

        private void btnEditPkgClick_Click(object sender, EventArgs e)
        {
            // Return if no pacakge to edit
            int curCboIndex = comboBoxPkgName.SelectedIndex;
            if (curCboIndex == -1)
                return;

            panelDetailPkg.Visible = false;
            panelAddPkg.Visible = false;
            panelEditPkg.Visible = true;
            btnAddPkgClick.Enabled = false;
            btnDeletePkgClick.Enabled = false;

            // Obtain the package data for the package selected in the combo box.
            // If the nth index is selected in the combo box, the current package
            // is the nth entry in the "packages" list
            Package currPkg = packages[curCboIndex];

            // Display the packge details to be edited
            textBoxEditPkgID.Text = currPkg.PackageId.ToString();
            textBoxEditPkgName.Text = currPkg.PackageName;
            textBoxEditStartDate.Text = currPkg.PkgStartDate.ToString("yyyy-MM-dd");
            textBoxEditEndDate.Text = currPkg.PkgEndDate.ToString("yyyy-MM-dd");
            textBoxEditDesc.Text = currPkg.PkgDesc;
            textBoxEditBasePrice.Text = currPkg.PkgBasePrice.ToString("0.00");
            textBoxEditComission.Text = currPkg.PkgAgencyCommission.ToString("0.00");
            // ---------------------------
        }

        //edit the selected package
        private void btnEditPkg_Click(object sender, EventArgs e)
        {
            // ------ Corinne Mullan ------
            // Obtain the original package data for the package currently selected in 
            // the combo box.
            // If the nth index is selected in the combo box, the current package
            // is the nth entry in the "packages" list
            int curCboIndex = comboBoxPkgName.SelectedIndex;
            Package currPkg = packages[curCboIndex];

            // Next, obtain and verify the edited data entered by the user
            DateTime edStartDate = new DateTime();
            DateTime edEndDate = new DateTime();
            decimal edBasePrice;
            decimal edCommission;

            // Check that the input data can be parsed
            if (!DateTime.TryParse(textBoxEditStartDate.Text, out edStartDate))
                MessageBox.Show("Please enter a date for the start date", "Error");
            if (!DateTime.TryParse(textBoxEditEndDate.Text, out edEndDate))
                MessageBox.Show("Please enter a date for the end date", "Error");

            if (!Decimal.TryParse(textBoxEditBasePrice.Text, out edBasePrice))
                MessageBox.Show("Please enter a decimal value for the base price", "Error");
            if (!Decimal.TryParse(textBoxEditComission.Text, out edCommission))
                MessageBox.Show("Please enter a decimal value for the agency commission", "Error");

            // Create the edited package object.  The package ID will be the same as the original
            // package.
            Package edPkg = new Package(currPkg.PackageId, textBoxEditPkgName.Text, edStartDate, edEndDate, textBoxEditDesc.Text, edBasePrice, edCommission);

            // Validate the values of the input data.  If the input is valid, add the new package
            // to the Packages table in the database.
            try
            {
                if (PackageValidation.ValidatePackageData(edPkg))
                    PackageDB.UpdatePackage(currPkg, edPkg);

                LoadUIforPackages();
                comboBoxPkgName.SelectedIndex = curCboIndex;

                panelDetailPkg.Visible = true;
                panelEditPkg.Visible = false;
                btnAddPkgClick.Enabled = true;
                btnDeletePkgClick.Enabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.GetType().ToString());
            }
            // ----------------------------         
        }

        //cancel editing package
        private void btnEditCanclePkg_Click(object sender, EventArgs e)
        {
            panelDetailPkg.Visible = true;
            panelEditPkg.Visible = false;
            btnAddPkgClick.Enabled = true;
            btnDeletePkgClick.Enabled = true;
        }
        private void btnPkgDetailCancel_Click(object sender, EventArgs e)
        {
            panelDetailPkg.Visible = false;
        }

        private void comboBoxPkgName_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Determine the package id from the selection in the combo box (i.e., if the nth 
            // index is selected, the package Id can be obtained from the nth package in the
            // "packages" list).
            int pkgId = packages[comboBoxPkgName.SelectedIndex].PackageId;

            // Display the package details in the Detail Package panel
            panelDetailPkg.Visible = true;

            textBoxDetailPkgID.Text = pkgId.ToString();
            textBoxDetailPkgName.Text = packages[comboBoxPkgName.SelectedIndex].PackageName;
            textBoxDetailStartDate.Text = packages[comboBoxPkgName.SelectedIndex].PkgStartDate.ToString("yyyy-MM-dd");
            textBoxDetailEndDate.Text = packages[comboBoxPkgName.SelectedIndex].PkgEndDate.ToString("yyyy-MM-dd");
            textBoxDetailDesc.Text = packages[comboBoxPkgName.SelectedIndex].PkgDesc;
            textBoxDetailBasePrice.Text = packages[comboBoxPkgName.SelectedIndex].PkgBasePrice.ToString("0.00");
            textBoxDetailComission.Text = packages[comboBoxPkgName.SelectedIndex].PkgAgencyCommission.ToString("0.00");

            // Obtain all of the product/supplier combinations for this package
            List <ProductSupplier> ppsList = PackageDB.GetPackageProductSuppliers(pkgId);

            // Display the results in the first grid view
            gridViewPkgProduct.DataSource = ppsList;

            // Obtain all of the other availabe product/supplier combinations that are not yet
            // used by this package
            List<ProductSupplier> ppsOtherList = PackageDB.GetOtherProductSuppliers(pkgId);

            // Display the results in the first grid view
            gridViewOtherProduct.DataSource = ppsOtherList;

            // Ensure only the details panel is visible
            panelDetailPkg.Visible = true;
            panelEditPkg.Visible = false;
            panelAddPkg.Visible = false;

            // The ADD, EDIT and DELETE buttons should all be enabled
            btnAddPkgClick.Enabled = true;
            btnEditPkgClick.Enabled = true;
            btnDeletePkgClick.Enabled = true;
        }
        // -------------------------

        private void supplierDataGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            int index = e.RowIndex;
            if (index >= 0)
            {
                DataGridViewRow selectedRow = supplierDataGridView.Rows[index];
                int supID = (int)selectedRow.Cells[0].Value;
                string supName = selectedRow.Cells[1].Value.ToString();
                supplier = new Supplier();
                supplier.SupplierID = supID;
                supplier.SupplierName = supName;
            }
            else
            {
                supplier = null;
            }
        }

        private void btnDeletePkgClick_Click(object sender, EventArgs e)
        {

            // Return if no pacakge to delete
            if (comboBoxPkgName.SelectedIndex == -1)
                return;

            // Get the current package to delete.
            // If the nth index is selected in the combo box, the current package
            // is the nth entry in the "packages" list
            Package delPkg = packages[comboBoxPkgName.SelectedIndex];

            // Confirm with the user before proceeding
            DialogResult dr = MessageBox.Show("Delete the selected package from the database?", 
                              "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Information);

            if (dr == DialogResult.Yes)
            {
                // Delete the package from the database (note: cascading deletes have been 
                // implemented in the Travel Experts database)
                try
                {
                    bool result = PackageDB.DeletePackage(delPkg);
                    if (result)
                    {
                        MessageBox.Show("The package was successfully deleted", "Information");
                        LoadUIforPackages();
                        comboBoxPkgName.SelectedIndex = 0;
                    }
                    else
                    {
                        MessageBox.Show("Error deleting package from database", "Error");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, ex.GetType().ToString());
                }

            }

        }

        private void btnAddProductSupplier_Click(object sender, EventArgs e)
        {
            //To do
            panelAddProdSupplier.Visible = true;
            btnEditProductSupplier.Enabled = false;
            btnDeleteProductSupplier.Enabled = false;

            //Set up product combo box
            cboProdNameOnAddProductSupplierPanel.DataSource = products;
            cboProdNameOnAddProductSupplierPanel.DisplayMember = "ProdName";
            cboProdNameOnAddProductSupplierPanel.ValueMember = "ProductID";
            cboProdNameOnAddProductSupplierPanel.SelectedIndex = -1;

            //Set up supplier combo box
            cboProdSupplierIDOnAddProductSupplierPanel.DataSource = suppliers;
            cboProdSupplierIDOnAddProductSupplierPanel.DisplayMember = "SupplierName";
            cboProdSupplierIDOnAddProductSupplierPanel.ValueMember = "SupplierID";
            cboProdSupplierIDOnAddProductSupplierPanel.SelectedIndex = -1;
        }

        private void btnCancelOnAddProdSupplierPanel_Click(object sender, EventArgs e)
        {
            panelAddProdSupplier.Visible = false;
            btnEditProductSupplier.Enabled = true;
            btnDeleteProductSupplier.Enabled = true;
        }

        private void btnEditProductSupplier_Click(object sender, EventArgs e)
        {
            panelEditProductSupplier.Visible = true;
            btnAddProductSupplier.Enabled = false;
            btnDeleteProductSupplier.Enabled = false;



            ProductSupplier productSupplierToEdit = productSuppliers[productSupplierDataGridView.CurrentCell.RowIndex];
            txtProductIDOnEditProductSuppliersPanel.Text = productSupplierToEdit.ProductID.ToString();
            txtProdNameOnEditProductSupplierPanel.Text = productSupplierToEdit.ProductName;

            cboSupplierNameOnEditProdSupplierPanel.DataSource = suppliers;
            cboSupplierNameOnEditProdSupplierPanel.DisplayMember = "SupplierName";
            cboSupplierNameOnEditProdSupplierPanel.ValueMember = "SupplierID";
            cboSupplierNameOnEditProdSupplierPanel.SelectedValue = productSupplierToEdit.SupplierID;
        }

        private void btnCancelOnEditProdSupplierPanel_Click(object sender, EventArgs e)
        {
            panelEditProductSupplier.Visible = false;
            btnAddProductSupplier.Enabled = true;
            btnDeleteProductSupplier.Enabled = true;
        }

        private void tabControlTravel_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControlTravel.SelectedTab == tabControlTravel.TabPages["tabProductSuppliers"])
            {
                loadUIforProductSuppliers();
            }
            else if (tabControlTravel.SelectedTab == tabControlTravel.TabPages["tabPackages"])
            {
                LoadUIforPackages();
                comboBoxPkgName.SelectedIndex = 0;
            }
        }

        // Method called to load (or reload) the list of packages to the combo box in the 
        // packages tab.  Also clears the gridviews and Detail Package panel.
        private void LoadUIforPackages()
        {
            packages = PackageDB.GetAllPackages();

            // First clear the combo box
            comboBoxPkgName.Items.Clear();

            // Next, loop through the list of packages and populate the combo box
            foreach (Package p in packages)
                comboBoxPkgName.Items.Add(p.PackageName);

            // Hide the panels
            panelDetailPkg.Visible = false;
            panelEditPkg.Visible = false;
            panelAddPkg.Visible = false;
        }

        //Method called to get productsupplier data and load to UI 
        //when the product supplier tab is gets focus
        private void loadUIforProductSuppliers()
        {
            productSuppliers =   ProductsSuppliersDB.GetAllProductSuppliers();
            productSupplierDataGridView.DataSource = productSuppliers;
        }

        private void btnAddProduct_Click(object sender, EventArgs e)
        {
            string pronameText = textBoxAddProName.Text;
            pronameText = pronameText.Trim();
            if (pronameText != "")
            {
                panelProductAdd.Visible = false;
                btnEditProductClick.Enabled = true;
                btnDeleteProductClick.Enabled = true;
                //add product to the database

                //Get the data from the panel
                Product newProd = new Product();
                newProd.ProdName = textBoxAddProName.Text;
                // writing new Product and ProductSupplier tables 
                try
                {
                    //Writing to Products Table
                    int newProdID = ProductDB.AddProducts(newProd);
                    products = ProductDB.GetProducts();
                    productDataGridView.DataSource = products;

                    //Writing to ProductSupplier table
                    //Supplier sup = suppliers[supplierNameComboBox.SelectedIndex];
                    //ProductsSuppliersDB.AddProductsSuppliers(newProdID, sup.SupplierID);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, ex.GetType().ToString());
                }


            }
            else
            {
                MessageBox.Show("Fill up all the required data");
            }
            textBoxAddProName.Text = "";
        }

        private void btnDelelteSupClick_Click(object sender, EventArgs e)
        {
            Supplier delSupplier = suppliers[supplierDataGridView.CurrentCell.RowIndex];

            if (delSupplier != null)
            {

                DialogResult result = MessageBox.Show("Are you sure you want to delete " + delSupplier.SupplierName
                                 + " supplier ID " + delSupplier.SupplierID + "? ", "Confirm Delete", 
                                 MessageBoxButtons.YesNo, MessageBoxIcon.Question,
                                 MessageBoxDefaultButton.Button2);
                if (result == DialogResult.Yes)
                {
                    try
                    {
                        SupplierDB.DeleteSupplier(delSupplier);
                        suppliers = SupplierDB.GetAllSuppliers();
                        supplierDataGridView.DataSource = suppliers;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, ex.GetType().ToString());
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select a supplier to delete", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

   
        private void btnAddProdToPkg_Click(object sender, EventArgs e)
        {
            // Add the selected product/supplier to the package.  Multiselect has been set to
            // false for the data grid view so only one row can be selected at a time.

            // Return if no row selected
            if (gridViewOtherProduct.SelectedRows[0] == null)
                return;

            // Get the package ID of the current package
            int curCboSelIndex = comboBoxPkgName.SelectedIndex;
            int pkgId = packages[curCboSelIndex].PackageId;

            // Obtain the ProductSupplierId from the datagridview
            int prodSupId = Convert.ToInt32(gridViewOtherProduct.SelectedRows[0].Cells[0].Value);

            // Then add a new record to the Packages_Products_Suppliers table
            try
            {
                if (PackageDB.AddProductToPackage(pkgId, prodSupId))
                {
                    MessageBox.Show("Product/Supplier successfully added to pacakge", "Information");
                    LoadUIforPackages();
                    comboBoxPkgName.SelectedIndex = curCboSelIndex;
                }
                else
                    MessageBox.Show("Error adding Product/Supplier to pacakge", "Error");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.GetType().ToString());
            }

        }

        //Event for adding Product Suppliers to the database
        private void btnAddOnProdSupplierPanel_Click(object sender, EventArgs e)
        {
            if (cboProdNameOnAddProductSupplierPanel.SelectedIndex != -1 &&
                cboProdSupplierIDOnAddProductSupplierPanel.SelectedIndex != -1) //Check that something is selected in both combo boxes
            {
                //Create a new product supplier
                ProductSupplier newProdSup = new ProductSupplier();

                //Set the properties of the new product supplier
                newProdSup.ProductID = products[cboProdNameOnAddProductSupplierPanel.SelectedIndex].ProductID;
                newProdSup.ProductName = products[cboProdNameOnAddProductSupplierPanel.SelectedIndex].ProdName;
                newProdSup.SupplierID = suppliers[cboProdSupplierIDOnAddProductSupplierPanel.SelectedIndex].SupplierID;
                newProdSup.SupName = suppliers[cboProdSupplierIDOnAddProductSupplierPanel.SelectedIndex].SupplierName;

                //Write new product supplier to database
                try
                {
                    ProductsSuppliersDB.AddProductSupplier(newProdSup);
                    productSupplierDataGridView.DataSource = ProductsSuppliersDB.GetAllProductSuppliers();
                    btnCancelOnAddProdSupplierPanel_Click(null, EventArgs.Empty);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, ex.GetType().ToString(), 
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                
            }
            else
            {
                MessageBox.Show("Please select a product and a supplier", "Select a value",
                                 MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnDeleteProductSupplier_Click(object sender, EventArgs e)
        {
            //suppliers[supplierDataGridView.CurrentCell.RowIndex]
            ProductSupplier delProductSupplier = productSuppliers[productSupplierDataGridView.CurrentCell.RowIndex];

            if (delProductSupplier != null)
            {

                DialogResult result = MessageBox.Show("Are you sure you want to delete Product Supplier with ID: " 
                                                      + delProductSupplier.ProductSupplierID
                                                       + "? ", "Confirm Delete",
                                                       MessageBoxButtons.YesNo, MessageBoxIcon.Question,
                                                       MessageBoxDefaultButton.Button2);
                if (result == DialogResult.Yes)
                {
                    try
                    {
                        ProductsSuppliersDB.DeleteProductSupplier(delProductSupplier);
                        productSuppliers = ProductsSuppliersDB.GetAllProductSuppliers();
                        productSupplierDataGridView.DataSource = productSuppliers;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, ex.GetType().ToString());
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select a product supplier to delete", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnUpdateOnEditProdSuppliersPanel_Click(object sender, EventArgs e)
        {
            ProductSupplier productSupplierToEdit = productSuppliers[productSupplierDataGridView.CurrentCell.RowIndex];
            ProductSupplier newProductSupplier = productSupplierToEdit.GetCopy();

            //Update the new copy with UI information
            Supplier selectedSupplier = suppliers[cboSupplierNameOnEditProdSupplierPanel.SelectedIndex];
            newProductSupplier.SupplierID = selectedSupplier.SupplierID;
            newProductSupplier.SupName = selectedSupplier.SupplierName;

            if (newProductSupplier.SupplierID != productSupplierToEdit.SupplierID) //Check if there was a change
            {
                try
                {
                    bool result = ProductsSuppliersDB.UpdateProductSupplier(productSupplierToEdit, newProductSupplier);

                    if (result)
                    {
                        //Update the UI
                        productSuppliers = ProductsSuppliersDB.GetAllProductSuppliers();
                        productSupplierDataGridView.DataSource = productSuppliers;
                       
                        MessageBox.Show("The new supplier has been saved.", "Product Supplier Updated",
                                         MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("The changes were not saved", "Updated Failed",
                                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                    btnCancelOnEditProdSupplierPanel_Click(null, EventArgs.Empty);
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.Message, ex.GetType().ToString(),
                                             MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
          
           
        }
    }
}
