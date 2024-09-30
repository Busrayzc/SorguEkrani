using SorguEkran;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Windows.Forms;
using static System.Data.SqlClient.SqlException;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace SorguEkran
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            dataGridView1.CellDoubleClick += DataGridView1_CellDoubleClick;
            txtTckn.KeyPress += txtTckn_KeyPress;
        }

        SqlConnection baglanti = new SqlConnection("data source=localhost;initial catalog=SorguEkranDb;integrated security=True;trustservercertificate=True;");
        SorguEkranDbEntities db = new SorguEkranDbEntities();

        private void btnAra_Click(object sender, EventArgs e)
        {
            var gunFark = dtEnd.Value.Day - dtStart.Value.Day;
            if (gunFark <= 7)
            {

            }  
            else
            {
                MessageBox.Show("Tarih aralığı 1 hafta ile kısıtlanmalıdır.");
            }
            
            // TCKN kontrolü
                if (!string.IsNullOrEmpty(txtTckn.Text) && txtTckn.Text.Length != 11)
            {
                MessageBox.Show("TCKN 11 haneli olmalıdır.");
                return;
            }

            // Kullanıcının TCKN girip girmediğini kontrol et
            if (!string.IsNullOrEmpty(txtTckn.Text))
            {
                if (dtStart.Value.Date != DateTime.Now.Date && dtEnd.Value.Date != DateTime.Now.Date)
                {
                    var odemeListesi1 = db.TblOdeme.Where(x => x.TCKN==txtTckn.Text && x.Başlangıç_Tarihi>= dtStart.Value.Date && x.Bitiş_Tarihi<= dtEnd.Value.Date).ToList();  
                    if (odemeListesi1.Count != 0)
                    {
                        dataGridView1.DataSource = odemeListesi1;
                        dataGridView1.Columns[13].Visible = false;
                        dataGridView1.Columns[14].Visible = false;
                        MessageBox.Show("Sonuçlar listeleniyor.");
                    }
                }
                else
                {
                    // TCKN'ye ait tüm kayıtları al
                    var odemeListesi = db.TblOdeme
                        .Where(x => x.TCKN == txtTckn.Text)
                        .ToList();

                    if (odemeListesi.Count != 0)
                    {
                        dataGridView1.DataSource = odemeListesi;
                        dataGridView1.Columns[13].Visible = false;
                        dataGridView1.Columns[14].Visible = false;
                        MessageBox.Show("Sonuçlar listeleniyor.");
                    }
                    else
                    {
                        MessageBox.Show("Bu TCKN'ye ait kayıt bulunmamaktadır.");
                    }
                }
               
                    
            }
            else
            {
                var odemeListesi2 = db.TblOdeme.Where(x=> x.Başlangıç_Tarihi == dtStart.Value.Date && x.Bitiş_Tarihi == dtEnd.Value.Date).ToList(); 
                if(odemeListesi2.Count != 0)
                {
                    dataGridView1.DataSource= odemeListesi2;
                    dataGridView1.Columns[13].Visible = false;
                    dataGridView1.Columns[14].Visible = false;
                    MessageBox.Show("Sonuçlar listeleniyor.");
                }
                
            }
        }

        private void btnTemizle_Click(object sender, EventArgs e)
        {
            txtTckn.Text = string.Empty;
            dtStart.Value = DateTime.Now;
            dtEnd.Value = DateTime.Now;
            dataGridView1.DataSource = null;
            dataGridView1.Rows.Clear();
            MessageBox.Show("Alanlar temizlendi.");
        }

        private void txtTckn_KeyPress(object sender, KeyPressEventArgs e)
        {
           
            if (!char.IsDigit(e.KeyChar) && e.KeyChar != (char)Keys.Back)
            {
                MessageBox.Show("Lütfen sadece rakam giriniz.", "Geçersiz Giriş", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                e.Handled = true; 
            }
        }
        private void DataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            
            if (e.RowIndex >= 0)
            {
                
                int durumKodSutunu = 9; 

                
                if (e.ColumnIndex == durumKodSutunu)
                {
                    var paymentDetails = dataGridView1.Rows[e.RowIndex].DataBoundItem as TblOdeme; 
                    if (paymentDetails != null)
                    {
                        
                        MessageBox.Show($"Ödeme Bilgileri:\n\n" +
                                        $"TCKN: {paymentDetails.TCKN}\n" +
                                        $"Başlangıç Tarihi: {paymentDetails.Başlangıç_Tarihi}\n" +
                                        $"Bitiş Tarihi: {paymentDetails.Bitiş_Tarihi}\n" +
                                        $"Ödeme Tarihi: {paymentDetails.Ödeme_Tarihi}\n" +
                                        $"Alacaklı IBAN: {paymentDetails.Alacaklı_Iban}\n" +
                                        $"Borçlu IBAN: {paymentDetails.Borçlu_İban}\n" +
                                        $"Döviz Kodu: {paymentDetails.Döviz_Kodu}\n" +
                                        $"Tutar: {paymentDetails.Tutar}\n" +
                                        $"Açıklama: {paymentDetails.Açıklama}\n",
                                        "Ödeme Detayları", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
        }
    }
}