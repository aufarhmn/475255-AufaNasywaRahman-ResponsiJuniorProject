using System.Data;
using Npgsql;

namespace ResponsiANR
{   
    // Inheritance Form1 dari Form
    // Encapsulation ditandai dengan adanya access modifier seperti Private dan Public pada Code dibawah
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private NpgsqlConnection conn;
        string connstring = "Host=localhost;Port=5432;Username=postgres;Password=informatika;Database=Responsi";

        public DataTable dt;
        public static NpgsqlCommand cmd;
        private string sql = null;
        private DataGridViewRow r;

        // Method Overloading
        // Keuda Method memiliki nama yang sama tetapi memiliki parameter yang berbeda
        // Satu method tidak memiliki parameter
        // Satu method memiliki parameter String connstring untuk melakukan inisiasi koneksi ke Postgre
        // Method Overloading START
        private void EstablishConn(string connstring)
        {
            this.connstring = connstring;
            conn = new NpgsqlConnection(connstring);
        }
        private void EstablishConn()
        {
            MessageBox.Show("Connection to PostGre is underway", "Good", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        // Method Overloading END

        private void Form1_Load(object sender, EventArgs e)
        {
            EstablishConn();
            EstablishConn(connstring);
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            try
            {
                conn.Open();

                dgvKaryawan.DataSource = null;
                sql = "select * from select_all()";
                cmd = new NpgsqlCommand(sql, conn);
                dt = new DataTable();
                NpgsqlDataReader rd = cmd.ExecuteReader();
                dt.Load(rd);
                dgvKaryawan.DataSource = dt;

                conn.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "FAIL!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnInsert_Click(object sender, EventArgs e)
        {
            try
            {
                conn.Open();
                sql = @"select * from insert_data(:_nama, :_id_dep)";
                cmd = new NpgsqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("_nama", txtName.Text);
                cmd.Parameters.AddWithValue("_id_dep", cbDepartemen.Text);

                if ((int)cmd.ExecuteScalar() == 1)
                {
                    MessageBox.Show("Data Karyawan Berhasil Diinputkan", "Well Done!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    conn.Close();
                    btnLoad.PerformClick();
                    txtName.Text = cbDepartemen.Text = null;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Insert FAIL!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (r == null)
            {
                MessageBox.Show("Pilih baris data dahulu!", "Good!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            try
            {
                conn.Open();
                sql = @"select * from update_data(:_id_karyawan, :_nama, :_id_dep)";
                cmd = new NpgsqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("_id_karyawan", r.Cells["_id_karyawan"].Value.ToString());
                cmd.Parameters.AddWithValue("_nama", txtName.Text);
                cmd.Parameters.AddWithValue("_id_dep", cbDepartemen.Text);
                if ((int)cmd.ExecuteScalar() == 1)
                {
                    MessageBox.Show("Data Karyawan Berhasil Diupdate", "Well Done!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    conn.Close();
                    btnLoad.PerformClick();
                    txtName.Text = cbDepartemen.Text = null;
                    r = null;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Update FAIL!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (r == null)
            {
                MessageBox.Show("Pilih baris data dahulu!", "Warning!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (MessageBox.Show("Apakah Anda ingin menghapus data " + r.Cells["_nama"].Value.ToString() + " ?", "Hapus data terkonfirmasi",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Yes)

                try
                {
                    conn.Open();
                    sql = @"select * from delete_by_id(:_id_karyawan)";
                    cmd = new NpgsqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("_id_karyawan", r.Cells["_id_karyawan"].Value.ToString());
                    if ((int)cmd.ExecuteScalar() == 1)
                    {
                        MessageBox.Show("Data Karyawan Berhasil Dihapus", "Well Done!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        conn.Close();
                        btnLoad.PerformClick();
                        txtName.Text = cbDepartemen.Text = null;
                        r = null;
                    }
                }

                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message, "Delete FAIL!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
        }

        private void dgvKaryawan_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                r = dgvKaryawan.Rows[e.RowIndex];
                txtName.Text = r.Cells["_nama"].Value.ToString();
                cbDepartemen.Text = r.Cells["_id_dep"].Value.ToString();
            }
        }
    }
}
