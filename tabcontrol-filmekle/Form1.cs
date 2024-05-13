using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
namespace tabcontrol_filmekle
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        string baglanti = "Server=localhost;Database=sinema_veritabani;Uid=root;Pwd=''";
        string hedefDosya;



        private void VeriGetir()
        {
            using (MySqlConnection con = new MySqlConnection(baglanti))
            {

                string sql = "SELECT *FROM filmler";
                con.Open();

                MySqlCommand cmd = new MySqlCommand(sql, con);

                MySqlDataAdapter da = new MySqlDataAdapter(cmd);

                DataTable dt = new DataTable();

                da.Fill(dt);

                dgvGListe.DataSource = dt;
                dgvGListe.Invalidate();
                dgvGListe.Refresh();

            }
        }


        private void btnGResimSec_Click(object sender, EventArgs e)
        {
            OpenFileDialog dosya = new OpenFileDialog();
            dosya.Filter = "resim dosyası |*.jpg;*.nef;*.png|video|*.avi|Tüm dosyalar|*.*";
            dosya.Title = "Dosya Seçin";

            if (dosya.ShowDialog() == DialogResult.OK)
            {
                string kaynakDosya = dosya.FileName;
                hedefDosya = Path.Combine("resimler", Guid.NewGuid() + ".jpg");


                if (!Directory.Exists("resimler"))
                {
                    Directory.CreateDirectory("resimler");
                }

                File.Copy(kaynakDosya, hedefDosya);
                pictureBox1.ImageLocation = hedefDosya;
                pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;

            }
        }

        private void btnKaydet_Click(object sender, EventArgs e)
        {

            using (MySqlConnection con = new MySqlConnection(baglanti))
            {

                string sql = "INSERT INTO filmler (filmad,tur,yil,imdb_puan,film_posteri,ozet) VALUES (@filmad,@tur,@yil,@imdb,@poster,@ozet);";

                con.Open();



                MySqlCommand cmd = new MySqlCommand(sql, con);

                cmd.Parameters.AddWithValue("@filmad", txtGFilmAd.Text);
                cmd.Parameters.AddWithValue("@tur", cmbTur.Text);
                cmd.Parameters.AddWithValue("@yil", txtEYil.Text);
                cmd.Parameters.AddWithValue("@imdb", txtGImdb.Text);
                cmd.Parameters.AddWithValue("@poster", hedefDosya);
                cmd.Parameters.AddWithValue("@ozet", txtGFilmOzet.Text);



                DialogResult result = MessageBox.Show("Film eklensin mi?", "Film Ekle", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    cmd.ExecuteNonQuery();

                    txtGFilmAd.Clear();
                    txtGFilmOzet.Clear();
                    txtGImdb.Clear();
                    txtEYil.Clear();
                    cmbTur.SelectedIndex = -1;
                }



            }
        }

        private void txtGFilmOzet_TextChanged(object sender, EventArgs e)
        {
            txtGFilmOzet.ScrollBars = ScrollBars.Vertical;
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl1.SelectedIndex == 1)
            {
                VeriGetir();
            }

            else if (tabControl1.SelectedIndex == 2)
            {
                txtGFilm.Text = lblFilm.Text;
                textBox1.Text = textBox1.Text;
                pictureBox1.Image = pictureBox1.Image;


            }




        }


        private void btnSil_Click(object sender, EventArgs e)
        {
            using (MySqlConnection con = new MySqlConnection(baglanti))
            {
                string sql = "DELETE FROM filmler WHERE id=@id";
                int id = Convert.ToInt32(txtGId.Text);

                con.Open();

                MySqlCommand cmd = new MySqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@id", id);



                DialogResult result = MessageBox.Show("Film silinsin mi?", "Film Sil", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (result == DialogResult.Yes)
                {
                    cmd.ExecuteNonQuery();
                    VeriGetir();
                }


            }
        }

        private void dgvGListe_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            txtGId.Text = dgvGListe.CurrentRow.Cells["id"].Value.ToString();
            txtGFilm.Text = dgvGListe.CurrentRow.Cells["filmad"].Value.ToString();
            txtGTur.Text = dgvGListe.CurrentRow.Cells["tur"].Value.ToString();
            txtPuan.Text = dgvGListe.CurrentRow.Cells["imdb_puan"].Value.ToString();
            txtGYil.Text = dgvGListe.CurrentRow.Cells["yil"].Value.ToString();
            txtGOzet.Text = dgvGListe.CurrentRow.Cells["ozet"].Value.ToString();
            pictureBox2.ImageLocation = dgvGListe.CurrentRow.Cells["film_posteri"].Value.ToString();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            VeriGetir();
        }

        private void btnEGuncelle_Click(object sender, EventArgs e)
        {
            using (MySqlConnection con = new MySqlConnection(baglanti))
            {
                string sql = "UPDATE filmler SET filmad=@filmad, tur=@tur,yil=@yil,imdb_puan=@imdb_puan,film_posteri=@film_posteri,ozet=@ozet WHERE id=@id;";
                int id = Convert.ToInt32(txtGId.Text);


                con.Open();

                MySqlCommand cmd = new MySqlCommand(sql, con);


                cmd.Parameters.AddWithValue("@filmad", txtGFilm.Text);
                cmd.Parameters.AddWithValue("@tur", txtGTur.Text);
                cmd.Parameters.AddWithValue("@yil", txtGYil.Text);
                cmd.Parameters.AddWithValue("@imdb_puan", txtPuan.Text);
                cmd.Parameters.AddWithValue("@film_posteri", "resimler\\1.jpg");
                cmd.Parameters.AddWithValue("@ozet", txtGOzet.Text);
                cmd.Parameters.AddWithValue("@id", txtGId.Text);




                DialogResult result = MessageBox.Show("Film güncellensin mi?", "Film Güncelle", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    cmd.ExecuteNonQuery();
                    VeriGetir();
                }




            }
        }
    }
}
