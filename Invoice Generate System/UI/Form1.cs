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
using Invoice_Generate_System.DbGateway;

namespace Invoice_Generate_System
{
    public partial class Form1 : Form
    {
        private SqlConnection con;
        private SqlCommand cmd;
        private SqlDataReader rdr;
        private ConnectionString cs = new ConnectionString();
        public int refId, quotationId, sclientId, sQN, invoiceId, user_id;
        public string referenceNo;
        private delegate void ChangeFocusDelegate(Control ctl);
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            QuotationIdLoad();
            this.BeginInvoke(new ChangeFocusDelegate(changeFocus), cmbQuotation);
        }

        private void changeFocus(Control ctl)
        {
            ctl.Focus();
        }
        public void SelectSclientId()
        {
            con = new SqlConnection(cs.DBConn);
            con.Open();
            cmd = con.CreateCommand();

            cmd.CommandText = "select SClientId,RefId,QuotationId from RefNumForQuotation WHERE ReferenceNo= '" + cmbQuotation.Text + "'";

            rdr = cmd.ExecuteReader();
            if (rdr.Read())
            {
                sclientId = rdr.GetInt32(0);
                refId = rdr.GetInt32(1);
                quotationId = rdr.GetInt32(2);
            }
            if ((rdr != null))
            {
                rdr.Close();
            }
            if (con.State == ConnectionState.Open)
            {
                con.Close();
            }
        }

        private void QuotationIdLoad()
        {
            try
            {
                con=new SqlConnection(cs.DBConn);
                con.Open();
                string query = "select ReferenceNo from RefNumForQuotation left join Invoice_New on RefNumForQuotation.RefId=Invoice_New.RefId EXCEPT select ReferenceNo from RefNumForQuotation right join Invoice_New on RefNumForQuotation.RefId=Invoice_New.RefId";
                cmd=new SqlCommand(query,con);
                rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    cmbQuotation.Items.Add(rdr[0]);
                }
                //cmbQuotation.Items.Add("Not in The List");
                con.Close();
            }
                
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message,"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void cmbQuotation_SelectedIndexChanged(object sender, EventArgs e)
        {
            SelectSclientId();
        }

        private void SaveInvoice()
        {
            try
            {
                con = new SqlConnection(cs.DBConn);
                con.Open();
                String query = "insert into Invoice_New(InvoiceDate, DueDate, QuotationId, RefId, GrossReceive, NetReceive, PromisedDate, SClientId, WorkOrderNo, DeliveryNo, InvoiceTo, Address, LPhn, RP, CPhn) values (@d1,@d2,@d3,@d4,@d5,@d6,@d7,@d8,@d9,@d10,@d11,@d12,@d13,@d14,@d15)" + "SELECT CONVERT(int, SCOPE_IDENTITY())";
                cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@d1", dtpInvoiceDate.Value);
                cmd.Parameters.AddWithValue("@d2", dtpDueDate.Value);
                //cmd.Parameters.AddWithValue("@d3", user_id);
                cmd.Parameters.AddWithValue("@d3", quotationId);
                cmd.Parameters.AddWithValue("@d4", refId);

                cmd.Parameters.AddWithValue("@d5", Convert.ToDecimal(txtGrossReceivable.Text));
                cmd.Parameters.AddWithValue("@d6", Convert.ToDecimal(txtNetReceivable.Text));
                cmd.Parameters.AddWithValue("@d7", dtpPromisedDate.Value);
                cmd.Parameters.AddWithValue("@d8", sclientId);
                cmd.Parameters.AddWithValue("@d9", txtWorkOrderNo.Text);
                cmd.Parameters.AddWithValue("@d10", txtDeliveryNo.Text);
                cmd.Parameters.AddWithValue("@d11", txtInvoiceParty.Text);
                cmd.Parameters.AddWithValue("@d12", txtPayerAddress.Text);
                cmd.Parameters.AddWithValue("@d13", txtLandPhone.Text);
                cmd.Parameters.AddWithValue("@d14", txtRP.Text);
                cmd.Parameters.AddWithValue("@d15", txtCellPhone.Text);

                invoiceId = (int)cmd.ExecuteScalar();
                con.Close();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SaveReferenceNumForInvoice()
        {
            try
            {
                con = new SqlConnection(cs.DBConn);
                con.Open();                
                string qr2 = "SELECT SClientId,SQN FROM RefNumForQuotation WHERE ReferenceNo= '" + cmbQuotation.Text + "'";
                cmd = new SqlCommand(qr2, con);
                rdr = cmd.ExecuteReader();
                if (rdr.Read())
                {
                    sclientId = (rdr.GetInt32(0));
                    sQN = (rdr.GetInt32(1));

                    referenceNo = "INV-" + sclientId + "-" + sQN + "-" + quotationId + "-" + invoiceId + "";
                }
                con = new SqlConnection(cs.DBConn);
                string cb = "insert into RefNumForInvoice_New(Code,SClientId,SQN,QuotationId,InvoiceId,RefInvoiceNo) VALUES (@d1,@d2,@d3,@d4,@d5,@d6)";
                cmd = new SqlCommand(cb);
                cmd.Connection = con;
                cmd.Parameters.AddWithValue("d1", "INV");
                cmd.Parameters.AddWithValue("d2", sclientId);
                cmd.Parameters.AddWithValue("d3", sQN);
                cmd.Parameters.AddWithValue("d4", quotationId);
                cmd.Parameters.AddWithValue("d5", invoiceId);
                cmd.Parameters.AddWithValue("d6", referenceNo);
                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(cmbQuotation.Text))
            {
                MessageBox.Show("Please select Quotation Id/Ref/Number", "error", MessageBoxButtons.OK,MessageBoxIcon.Error);
                this.BeginInvoke(new ChangeFocusDelegate(changeFocus), cmbQuotation);
            }
            
            else if (string.IsNullOrWhiteSpace(txtInvoiceParty.Text))
            {
                MessageBox.Show("Please enter Invoice Party", "error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.BeginInvoke(new ChangeFocusDelegate(changeFocus), txtInvoiceParty);
            }
            
            else if (string.IsNullOrWhiteSpace(txtPayerAddress.Text))
            {
                MessageBox.Show("Please enter Payer Address","error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.BeginInvoke(new ChangeFocusDelegate(changeFocus), txtPayerAddress);
            }

            else if (string.IsNullOrWhiteSpace(txtLandPhone.Text))
            {
                MessageBox.Show("Please enter Land Phone Number", "error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.BeginInvoke(new ChangeFocusDelegate(changeFocus), txtLandPhone);
            }

            else if (string.IsNullOrWhiteSpace(txtRP.Text))
            {
                MessageBox.Show("Please enter Respondent Person name", "error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.BeginInvoke(new ChangeFocusDelegate(changeFocus), txtRP);
            }

            else if (string.IsNullOrWhiteSpace(txtCellPhone.Text))
            {
                MessageBox.Show("Please enter Cell Phone Number", "error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.BeginInvoke(new ChangeFocusDelegate(changeFocus), txtCellPhone);
            }

            else if (string.IsNullOrWhiteSpace(txtGrossReceivable.Text))
            {
                MessageBox.Show("Please  enter Gross Receivable", "error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.BeginInvoke(new ChangeFocusDelegate(changeFocus), txtGrossReceivable);

            }

            else if (string.IsNullOrWhiteSpace(txtNetReceivable.Text))
            {
                MessageBox.Show("Please  enter Net Receivable", "error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.BeginInvoke(new ChangeFocusDelegate(changeFocus), txtNetReceivable);

            }

            else
            {
                SaveInvoice();
                MessageBox.Show("Successfully Generated", "Record", MessageBoxButtons.OK, MessageBoxIcon.Information);
                SaveReferenceNumForInvoice();
                ClearData();
                QuotationIdLoad();
                cmbQuotation.ResetText();
            }
        }

        private void ClearData()
        {
            cmbQuotation.Items.Clear();
            cmbQuotation.SelectedIndex = -1;
            txtWorkOrderNo.Clear();
            txtDeliveryNo.Clear();
            txtInvoiceParty.Clear();
            txtPayerAddress.Clear();
            txtLandPhone.Clear();
            txtRP.Clear();
            txtCellPhone.Clear();
            dtpInvoiceDate.ResetText();
            dtpDueDate.ResetText();
            txtGrossReceivable.Clear();
            txtNetReceivable.Clear();
            dtpPromisedDate.ResetText();
        }

        private void txtGrossReceivable_KeyPress(object sender, KeyPressEventArgs e)
        {
            char ch = e.KeyChar;
            decimal x;
            if (ch == (char)Keys.Back)
            {
                e.Handled = false;
            }
            else if (!char.IsDigit(ch) && ch != '.' || !Decimal.TryParse(txtGrossReceivable.Text + ch, out x))
            {
                e.Handled = true;
            }
        }

        private void txtNetReceivable_KeyPress(object sender, KeyPressEventArgs e)
        {
            char ch = e.KeyChar;
            decimal x;
            if (ch==(char)Keys.Back)
            {
                e.Handled = false;
            }
            else if(!char.IsDigit(ch) && ch !='.' || !Decimal.TryParse(txtNetReceivable.Text + ch, out x))
            {
                e.Handled = true;
            }
        }

        private void dtpInvoiceDate_ValueChanged(object sender, EventArgs e)
        {
            if (dtpInvoiceDate.Value > DateTime.Now)
            {
                MessageBox.Show("Should not be exceed Date Time from today", "Warrning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                dtpInvoiceDate.ResetText();
            }
        }

        private void dtpDueDate_ValueChanged(object sender, EventArgs e)
        {
            if (dtpInvoiceDate.Value > dtpDueDate.Value)
            {
                MessageBox.Show("Due Date Should be grater than or Equal to Invoice Date", "Warrning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                dtpDueDate.ResetText();
            } 
        }

        private void dtpPromisedDate_ValueChanged(object sender, EventArgs e)
        {
            if (dtpDueDate.Value > dtpPromisedDate.Value)
            {
                MessageBox.Show("Promised Date Should be grater than or Equal to Due Date", "Warrning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                dtpPromisedDate.ResetText();
            } 
        }

        private void txtLandPhone_KeyPress(object sender, KeyPressEventArgs e)
        {
            string land_phone = txtLandPhone.Text;
            
            if (char.IsNumber(e.KeyChar))
            { }
            else
            {
                e.Handled = e.KeyChar != (char)Keys.Back;
            }
        }

        private void cmbQuotation_Leave(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(cmbQuotation.Text) && !cmbQuotation.Items.Contains(cmbQuotation.Text))
            {
                MessageBox.Show("Please Select A Valid Quotation Id/Ref/Number", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                cmbQuotation.ResetText();
                this.BeginInvoke(new ChangeFocusDelegate(changeFocus), cmbQuotation);
            }
        }
       
    }
}
