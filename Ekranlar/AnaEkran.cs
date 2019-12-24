using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TezRota.YoneticiKatmani;

namespace TezRota
{
    public partial class AnaEkran : Form
    {
        public AnaEkran()
        {
            InitializeComponent();
        }

        private void AnaEkran_Load(object sender, EventArgs e)
        {
            //combobox'a planları yüklemek
            using (PlanYonetici py = new PlanYonetici())
            {
                py.ComboPlanYukle(ref this.cmbPlan);
            }
        }

        private void BtnPlanOlustur_Click(object sender, EventArgs e)
        {
            using (PlanYonetici py = new PlanYonetici())
            {
                py.RassalPlanUret();
            }
        }

        private void CmbPlan_SelectedIndexChanged(object sender, EventArgs e)
        {
           
                ComboBox comboBox = (ComboBox)sender;

               
                KeyValuePair<int, string> selectedEntry
                    = (KeyValuePair<int, string>)comboBox.SelectedItem;

               
                int selectedKey = selectedEntry.Key;
                //index değiştikçe datagridview dolsun
               
                using (PlanYonetici py = new PlanYonetici())
                {
                    py.DataGridePlanYukle(selectedKey, ref this.dataGridView1);
                }
          
        }

        private void BtnPlanRotala_Click(object sender, EventArgs e)
        {
            int sic = 25;
            for(int s=0; s<4; s++)
            {

                for (int j = 0; j < cmbPlan.Items.Count; ++j)
                {

                    var key = cmbPlan.Items[j].ToString().Remove(0, 1).Split(',')[0];
                    
                    int iterasyon = 250;
                    for (int i = 0; i < 4; i++)
                    {
                        txtDongu.Text = iterasyon.ToString();

                        //1-) Plan oku && Araçtiplerini Oku
                        CozumYonetici cIstek = new CozumYonetici();
                        cIstek.Dongu = int.Parse(txtDongu.Text);
                        cIstek.Sicaklik = sic;// int.Parse(txtSicaklik.Text);
                        int planId = int.Parse(key);
                        if(planId <=3038)
                        {
                            continue;
                        }
                        if(planId==4011 && iterasyon == 1000 && sic==25)
                        {
                            continue;
                        }
                        cIstek.PlanId = planId;
                        //2-) Taşıma listesini oku
                        cIstek.TasimaListesiniOku();
                        //3-) Tanımlı noktaları oku
                        cIstek.NoktaListesiniOku();
                        //4-) Araç bilgisini oku
                        cIstek.AracListesiniOku();
                        //5-) Taşımaların araç bilgisini tanımla && Taşımaların nokta bilgisini tanımla
                        cIstek.TasimaNoktaAracBilgisiEsle();
                        //6-) Başlangıç Kromozom Oluştur
                        cIstek.BaslangicKromozomOlustur();
                        //7)

                        //foreach (var b in cIstek.NakliyeListesi)
                        //{
                        //    txtBaslangicCozum.AppendText(b.NakliyeNo + " : " + string.Join(",", b.RotaList.ToList()) + " Skor : " + b.NakSkor + Environment.NewLine);
                        //}
                        //txtBaslangicCozum.AppendText("Toplam Skor : " + cIstek.NakliyeListesi.Max(a => a.NakSkor));

                        //7-) Tavlama Benzetimi Uygula
                        cIstek.TavlamaBenzetimiUygula();
                        txtEnIyiCozum.Clear();
                        foreach (var b in cIstek.EnIyiCozum)
                        {
                            txtEnIyiCozum.AppendText(b.NakliyeNo + " : " + string.Join(",", b.TasimaListesi.Select(a => a.HedefId).ToList()) + " Skor : " + b.NakSkor + Environment.NewLine);
                        }
                        if (cIstek.EnIyiCozum.Any())
                        {

                            txtEnIyiCozum.AppendText("Toplam Skor : " + cIstek.EnIyiCozum.Max(a => a.NakSkor));
                            cIstek.SonucBilginiYaz();
                        }
                        iterasyon = iterasyon + 250;
                    }


                }
                sic = sic + 25;
            }
          

          //  MessageBox.Show("","Çözüm tamamlandı", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void BtnPlanAktar_Click(object sender, EventArgs e)
        {
            var dosyaYolu = txtExcelFilePath.Text;
            using (PlanYonetici py =new PlanYonetici())
            {
                py.FilePath = dosyaYolu;
                //excel planı oku
                py.ImportExceltoDatatable();
                MessageBox.Show("Bilgilendirme", "Excel dosya planı okundu", MessageBoxButtons.OK, MessageBoxIcon.Information);
                //okunan dosya plana dönüştürülmeli
                py.DataTableToPlan();
            }
        }

        private void BtnPlanSec_Click(object sender, EventArgs e)
        {
            //labelTimer.Text = "";
            txtExcelFilePath.Clear();//yeni dosya browse edildiğinde textbox clear edilir

            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            openFileDialog1.InitialDirectory = @"C:\Users\NisanurrunasiN\Desktop\";

            openFileDialog1.Title = "Browse Excel Dosyaları";

            openFileDialog1.CheckFileExists = true;

            openFileDialog1.CheckPathExists = true;

            openFileDialog1.DefaultExt = "xlsx";

            openFileDialog1.Filter = "Excel dosyaları (*.xlsx)|*.xlsx";

            openFileDialog1.FilterIndex = 2;

            openFileDialog1.RestoreDirectory = true;

            openFileDialog1.ReadOnlyChecked = true;

            openFileDialog1.ShowReadOnly = true;

            if (openFileDialog1.ShowDialog() == DialogResult.OK) //bunu test etmem gerek !!!!!
            {
                txtExcelFilePath.Text = openFileDialog1.FileName;
               
            }
        }
    }
}
