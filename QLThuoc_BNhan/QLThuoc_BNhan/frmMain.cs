using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Services;
using DataModels;
namespace QLThuoc_BNhan
{
    public partial class frmMain : Form
    {
        private readonly ManagerService _managerService = new ManagerService();
        public frmMain()
        {
            InitializeComponent();
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            // dgvBenhNhan.AutoGenerateColumns = true;
            this.dgvDetail.MultiSelect = false;
            List<string> days = new List<string>();
            days.Add("All");
            for (int i = 1; i <= 31; i++)
            {
                days.Add(i.ToString());
            }
            cbDay.DataSource = days;
            List<int> months = new List<int>();
            for (int i = 1; i <= 12; i++)
            {
                months.Add(i);
            }
            cbMonth.DataSource = months;

            List<int> years = new List<int>();
            for (int i = 2000; i <= DateTime.Now.Year; i++)
            {
                years.Add(i);
            }
            cbYear.DataSource = years;
            setDatetimeforCombobox();
        }

        private void frmMain_KeyPress(object sender, KeyPressEventArgs e)
        {
            
        }

        private void txtMaso_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Escape)
            {
                clear();
            }
            else if (e.KeyChar == (char)Keys.Enter)
            {
                List<BenhNhan> benhnhans = new List<BenhNhan>();
                try
                {
                    string id = txtCode.Text.Trim();

                    if (!string.IsNullOrEmpty(id) && !string.IsNullOrWhiteSpace(id))
                    {
                        benhnhans = _managerService.GetBenhNhanById(id);
                    }
                }
                catch (Exception ex)
                { }

                dgvBenhNhan.DataSource = benhnhans;
            }
        }
        private void clear()
        {
            txtCode.Text = string.Empty;
            txtName.Text = string.Empty;
        }

        private void btnToday_Click(object sender, EventArgs e)
        {
            clearForToday();
            setDatetimeforCombobox();
            loadBenhNhanByDate();
        }
        private void setDatetimeforCombobox()
        {
            cbDay.SelectedIndex = DateTime.Now.Day;
            cbMonth.SelectedIndex = DateTime.Now.Month - 1;
            cbYear.SelectedIndex = DateTime.Now.Year - 2000;
        }
        private void txtHoten_TextChanged(object sender, EventArgs e)
        {
            
            //if (benhnhans.Count == 1)
            //{
            //    loadBenhNhan(benhnhans[0].ID);
            //}
        }

        private void txtHoten_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Escape)
            {
                clear();
            }
            else if (e.KeyChar == (char)Keys.Enter)
            {
                string[] names = txtName.Text.Trim().Split(' ').Where(p => !string.IsNullOrEmpty(p)).ToArray();
                List<BenhNhan> benhnhans = new List<BenhNhan>();
                if (names.Length > 0)
                {
                    benhnhans = _managerService.GetBenhNhanByName(names);
                }
                dgvBenhNhan.DataSource = benhnhans;
            }
        }

        private void dtpBirth_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Escape)
            {
                
            }
        }

        private void dgvDatetime_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvDatetime.SelectedRows.Count > 0)
            {
                int benhnhanid = int.Parse(this.dgvDatetime.SelectedRows[0].Cells[0].Value.ToString());
                DateTime datetime = DateTime.Parse(this.dgvDatetime.SelectedRows[0].Cells[1].Value.ToString());
                loadDrugHistories(benhnhanid, datetime);
            }
        }

        private void dgvBenhNhan_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (this.dgvBenhNhan.Columns[e.ColumnIndex].Name == "clBirthDay")
            {
                if (e.Value != null)
                {
                    // Check for the string "pink" in the cell.
                    DateTime stringValue = (DateTime)e.Value;
                    e.Value = stringValue.ToString("dd / MM / yyyy");

                }
            }
        }

        private void dgvBenhNhan_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvBenhNhan.SelectedRows.Count > 0)
            {
                BenhNhan bn = (BenhNhan)this.dgvBenhNhan.SelectedRows[0].DataBoundItem;
                loadBenhNhan(bn.ID);
                loadDrugHistoryDatetime(bn.ID);
            }
        }

        private void dgvBenhNhan_SelectionChanged(object sender, EventArgs e)
        {
            
        }
        private void loadBenhNhan(int id)
        {
            BenhNhan bn = _managerService.GetBenhNhanByID(id);
            if (bn != null)
            {
                txtCode.Text = bn.ID.ToString();
                txtName.Text = bn.Name;
                txtPhone.Text = bn.Phone;
                dtpBirth.Value = bn.BirthDay.Value;
            }
            else
            {
                clearAll();
            }
            
        }
        private void loadDrugHistoryDatetime(int benhnhanid)
        {
            var obj = _managerService.GetDrugHistoryDatetimeByBenhNhan(benhnhanid);
            var list = obj.Group;
            dgvDatetime.DataSource = list;

        }
        private void clearAll()
        {
            txtCode.Text = string.Empty;
            txtName.Text = string.Empty;
            txtPhone.Text = string.Empty;
            dtpBirth.Value = DateTime.Now;
            dgvBenhNhan.DataSource = new List<BenhNhan>();
            dgvDatetime.DataSource = new List<object>();


            BindingSource src = new BindingSource();
            src.DataSource = new List<DrugHistory>();
            dgvDetail.DataSource = src;
        }
        private void clearForToday()
        {            


            BindingSource src = new BindingSource();
            src.DataSource = new List<DrugHistory>();
            dgvDetail.DataSource = src;
        }
       

        private void dgvDetail_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {

            try
            {
                int dhid = int.Parse(this.dgvDetail.Rows[e.RowIndex].Cells["clDHID"].Value.ToString());
                DrugHistory dh = (DrugHistory)this.dgvDetail.Rows[e.RowIndex].DataBoundItem;
                
                if (
                    string.IsNullOrEmpty(txtCode.Text.Trim())
                    || dh.Price == 0 || dh.Count == 0 
                    || string.IsNullOrEmpty(dh.DrugName.Trim())
                    || string.IsNullOrEmpty(dh.Unit.Trim()))
                {
                    return;
                }
                if (dh.ID == 0)
                {
                    if (dh.Date.HasValue == false)
                    {
                        dh.Date = DateTime.Now;
                    }
                }
                else
                {
                    if (dh.Date.HasValue == false)
                    {
                        dh.Date = DateTime.Now;
                    }
                }
                //if (string.IsNullOrEmpty(dh.DrugName) || string.IsNullOrWhiteSpace(dh.DrugName))
                //{
                //    MessageBox.Show("Nhập tên thuốc");
                //    setFocusForDataGridViewCell(this.dgvDetail, e.RowIndex,1);                    
                //}
                dh.BenhNhanID = int.Parse(txtCode.Text);
                int id = _managerService.Update(dh);
                this.dgvDetail.Rows[e.RowIndex].Cells["clDHID"].Value = id;
                this.dgvDetail.Rows[e.RowIndex].Cells["clTotal"].Value = dh.Count * dh.Price;
                calcTotal();



            }
            catch (Exception ex)
            {

            }

        }

       
        private void setFocusForDataGridViewCell(DataGridView dgv, int rowindex, int cellIndex)
        {
            dgv.CurrentCell = dgv.Rows[rowindex].Cells[cellIndex];
            dgv.CurrentCell.Selected = true;

            dgv.BeginEdit(true);
        }

        private void dgvDatetime_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            
        }
        private void loadDrugHistories(int benhnhanid, DateTime datetime)
        {
            var list = _managerService.GetDrugHistories(benhnhanid, datetime);
            BindingSource src = new BindingSource();
            src.DataSource = list;
            dgvDetail.DataSource = src;


            //dgvDetail.DataSource = list;
        }

        private void txtCode_TextChanged(object sender, EventArgs e)
        {
            
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            string code = txtCode.Text.Trim();
            if (string.IsNullOrEmpty(code))
            {
                int id = _managerService.create(txtName.Text.Trim(), txtPhone.Text.Trim(), dtpBirth.Value);
                txtCode.Text = id.ToString();
            }
            else
            {
                MessageBox.Show("Bạn phải xoá dữ liệu trước khi tạo mới");
                clearAll();
                MessageBox.Show("Mời bạn nhập lại");
            }
        }

        

        private void cbDay_SelectedIndexChanged(object sender, EventArgs e)
        {
            loadBenhNhanByDate();
        }

        private void loadBenhNhanByDate()
        {
            if (string.IsNullOrEmpty(cbDay.SelectedItem.ToString())
                || cbMonth.SelectedItem == null
                || cbYear.SelectedItem == null)
            {
                return;
            }
            loadDrugHistoryDatetime(0);
            dgvDetail.DataSource = null;
            int day = cbDay.SelectedItem.ToString() == "All" ? 0 : int.Parse(cbDay.SelectedItem.ToString());
            int month = int.Parse(cbMonth.SelectedItem.ToString());
            int year = int.Parse(cbYear.SelectedItem.ToString());
            var list = _managerService.GetBenhNhanByDate(day, month, year);
            dgvBenhNhan.DataSource = list;
        }

        private void cbMonth_SelectedIndexChanged(object sender, EventArgs e)
        {
            loadBenhNhanByDate();
        }

        private void cbYear_SelectedIndexChanged(object sender, EventArgs e)
        {
            loadBenhNhanByDate();
        }

        private void dgvDetail_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == dgvDetail.Columns["clDelete"].Index && e.RowIndex >= 0)
            {
                string name = dgvDetail.Rows[e.RowIndex].Cells[1].Value.ToString();
                int id = int.Parse(dgvDetail.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString());
                DialogResult dr = MessageBox.Show("Bạn có muốn xoá " + name + " không ?", "Title", MessageBoxButtons.YesNo,MessageBoxIcon.Information);

                if (dr == DialogResult.Yes)
                {
                    _managerService.DeleteDrugHistory(id);
                    dgvDetail.Rows.RemoveAt(e.RowIndex);
                }
            }
        }

        

        private void txtName_MouseClick(object sender, MouseEventArgs e)
        {
            txtName.SelectAll();
        }

        private void txtCode_MouseClick(object sender, MouseEventArgs e)
        {
            txtCode.SelectAll();
        }

        private void txtPhone_MouseClick(object sender, MouseEventArgs e)
        {
            txtPhone.SelectAll();
        }

        private void txtPhone_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Escape)
            {
                clear();
            }
            else if (e.KeyChar == (char)Keys.Enter)
            {
                string phone = txtPhone.Text.ToString().Trim();
                List<BenhNhan> benhnhans = new List<BenhNhan>();
                if (!string.IsNullOrEmpty(phone))
                {
                    benhnhans = _managerService.GetBenhNhanByPhone(phone);
                }
                dgvBenhNhan.DataSource = benhnhans;
            }
        }

        
        private void dgvDetail_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            int row = e.RowIndex;
            int col = e.ColumnIndex;
            if(row % 2 == 1)
            {
                dgvDetail.Rows[row].DefaultCellStyle.BackColor = Color.FromArgb(236, 236, 236);
            }
            if (this.dgvDetail.Columns[e.ColumnIndex].Name == "clTotal")
            {

                int count = dgvDetail.Columns["clCount"].Index;
                int price = dgvDetail.Columns["clPrice"].Index;
                try
                {
                    int c = int.Parse(dgvDetail.Rows[e.RowIndex].Cells[count].Value.ToString());
                    int p = int.Parse(dgvDetail.Rows[e.RowIndex].Cells[price].Value.ToString());
                    // Check for the string "pink" in the cell.
                    //DateTime stringValue = (DateTime)e.Value;
                    e.Value = (c * p).ToString("#,##0");

                }
                catch (Exception ex) { }


            }
            else if (this.dgvDetail.Columns[e.ColumnIndex].Name == "clPrice")
            {
                if (e.Value != null)
                {
                    e.Value = int.Parse(e.Value.ToString()).ToString("#,##0");
                }
            }
            else if (this.dgvDetail.Columns[e.ColumnIndex].Name == "clDelete")
            {
                DataGridViewButtonCell btn = (DataGridViewButtonCell)dgvDetail.Rows[row].Cells["clDelete"];
                if (btn != null)
                {
                    if (row % 2 == 1)
                    {
                        btn.FlatStyle = FlatStyle.Standard;
                    }
                    else
                    {
                        btn.FlatStyle = FlatStyle.Flat;

                    }
                }
            }
            calcTotal();

        }
        private void calcTotal()
        {
            int sum = dgvDetail.Rows.Cast<DataGridViewRow>()
                                   .AsEnumerable()
                                   .Where(x => x.Cells["clCount"].Value != null && x.Cells["clPrice"].Value != null)
                                   .Sum(x => int.Parse(x.Cells["clCount"].Value.ToString())
                                            * int.Parse(x.Cells["clPrice"].Value.ToString()))
                                   ;


            lblTongTien.Text = sum.ToString("#,##0");
        }

      
    }
}
