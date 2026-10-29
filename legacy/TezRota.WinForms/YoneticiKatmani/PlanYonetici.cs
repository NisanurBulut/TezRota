using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TezRota.ModelKatmani;
using TezRota.VeriKatmani;

namespace TezRota.YoneticiKatmani
{
    public class PlanYonetici : IDisposable
    {
        private Random random = new Random();
        public int RotaSayisi { get; set; }
        public int maxSiraId { get; set; }
        public DataTable dtExcelPlan { get; set; }

        public int ELM = 65;
        public string FilePath { get; set; }
        public PlanYonetici()
        {
            
        }
        public void ComboPlanYukle(ref ComboBox combo)
        {
            using (dbTezRotaDataContext dc = new dbTezRotaDataContext())
            {
                Dictionary<int, string> veri = new Dictionary<int, string>();
                var planListesi = dc.tPlans.ToList();
                if (planListesi.Any())
                {
                    foreach (var p in planListesi)
                    {
                        veri.Add(p.Id, p.PlanIsmi);
                    }
                    combo.DataSource = new BindingSource(veri, null);
                    combo.DisplayMember = "Value";
                    combo.ValueMember = "Key";
                }

            }
        }
        public void DataGridePlanYukle(int pid, ref DataGridView dataGridView)
        {
            using (dbTezRotaDataContext dc = new dbTezRotaDataContext())
            {
                var result = dc.tTasimas.Where(a => a.PlanId == pid);
                dataGridView.DataSource = result;
            }
        }
        public void RassalPlanUret()
        {
            var seed = (int)DateTime.Now.Ticks;
            var rnd = new Random(seed);
            maxSiraId = 10;
            RotaSayisi = 20;
            List<int> tasimaTipleri = new List<int>() { 1, 2, 3, 4, 5 };
            using (dbTezRotaDataContext dc = new dbTezRotaDataContext())
            {
                List<int> NoktaIdListesi = dc.tNoktas.Where(a => a.Id != ELM).Select(a => a.Id).ToList();
                List<int> tempHedefListesi = new List<int>();
                int noktaSayisi = NoktaIdListesi.Count;
                int tasimaTipSayisi = tasimaTipleri.Count;
                tPlan plan = new tPlan();
                plan.PlanIsmi = "Plan - " + DateTime.Now.ToString();

                dc.tPlans.InsertOnSubmit(plan);
                dc.SubmitChanges();

                MessageBox.Show("Plan başarılı şekilde oluşturulmuştur.");
                int KaynakId = ELM;
                int HedefId = 0;
                //taşıma oluşturma 

                for (int i = 0; i < RotaSayisi; i++)
                {
                    random = new Random((int)DateTime.Now.Ticks);
                    int tid = random.Next(0, tasimaTipSayisi);
                    string nakliyeNo = "N-" + i;
                    for (int s = 0; s < maxSiraId; s++)
                    {
                        if (s == 0)
                        {
                            KaynakId = ELM;
                        }
                        if (s == (maxSiraId - 1))
                        {
                            HedefId = ELM;
                        }
                        else
                        {
                            var randomHedefList = NoktaIdListesi.Except(tempHedefListesi);
                            int ind = rnd.Next(0, randomHedefList.Count());
                            HedefId = randomHedefList.ElementAt(ind);
                            tempHedefListesi.Add(HedefId);
                        }

                        tTasima t = new tTasima();
                        t.PlanId = plan.Id;
                        t.KaynakId = KaynakId;
                        t.HedefId = HedefId;
                        t.SiraId = s;
                        t.NakliyeNo = nakliyeNo;
                        t.TasimaTipi = tasimaTipleri.ElementAt(tid);
                        dc.tTasimas.InsertOnSubmit(t);
                        dc.SubmitChanges();
                        KaynakId = HedefId;
                    }
                    KaynakId = ELM;
                    tempHedefListesi.Clear();
                }
                MessageBox.Show("Plana taşıma eklenmiştir.");
            }
        }
        public void ImportExceltoDatatable()
        {
            int s = 0;
            // Open the Excel file using ClosedXML.
            // Keep in mind the Excel file cannot be open when trying to read it
            string[] arr = Directory.GetFiles(@"C:\Users\NisanurrunasiN\Desktop\test", "*.xlsx");
            //Create a new DataTable.
            dtExcelPlan = new DataTable();
            foreach (string file in arr)
            {
                using (XLWorkbook workBook = new XLWorkbook(file))
                {
                    
                    foreach (IXLWorksheet w in workBook.Worksheets)
                    {
                        //Loop through the Worksheet rows.
                        bool firstRow = true;
                        foreach (IXLRow row in w.Rows())
                        {
                            //Use the first row to add columns to DataTable.
                            if (firstRow)
                            {
                                if (s == 0)
                                {
                                    foreach (IXLCell cell in row.Cells())
                                    {
                                        dtExcelPlan.Columns.Add(cell.Value.ToString());
                                        s++;
                                    }
                                    firstRow = false;
                                }
                                else
                                {
                                    firstRow = false;
                                    continue;
                                }

                            }
                            else
                            {
                                //Add rows to DataTable.
                                dtExcelPlan.Rows.Add();
                                int i = 0;

                                foreach (IXLCell cell in row.Cells(row.FirstCellUsed().Address.ColumnNumber, row.LastCellUsed().Address.ColumnNumber))
                                {
                                    dtExcelPlan.Rows[dtExcelPlan.Rows.Count - 1][i] = cell.Value.ToString();
                                    i++;
                                }
                            }
                        }
                    }
                }
            }
           
        }
        public void DataTableToPlan()
        {
            var seed = (int)DateTime.Now.Ticks;
            var rnd = new Random(seed);

            List<Tasima> Tasimalar = new List<Tasima>();
            List<tNokta> noktalar = new List<tNokta>();
            List<tNoktaKaynak> kaynakNoktalar = new List<tNoktaKaynak>();
            var araclar = new List<tArac>(); ;
            using (dbTezRotaDataContext dc = new dbTezRotaDataContext())
            {
                noktalar = dc.tNoktas.ToList();
                kaynakNoktalar = dc.tNoktaKaynaks.ToList();
                araclar = dc.tAracs.ToList();

            }
            Nokta KaynakNokta = new Nokta();
            Tasima t = new Tasima();
            var tempLokasyon = string.Empty;
            for (int i = 0; i < dtExcelPlan.Rows.Count; i++)
            {
                string Lokasyon = dtExcelPlan.Rows[i]["Lokasyon"].ToString();
                int SiraId = Convert.ToInt32(dtExcelPlan.Rows[i]["Olay Sırası"]);
                string NakliyeNo = dtExcelPlan.Rows[i]["Nakliye"].ToString();
                string dorsePlaka = dtExcelPlan.Rows[i]["Plaka"].ToString();


                if (SiraId == 1)
                {
                    t = new Tasima();
                    KaynakNokta = new Nokta();
                    var EldekiNokta = kaynakNoktalar.Where(a => a.Kodu2.Contains(Lokasyon)).FirstOrDefault();
                    if (EldekiNokta == null)
                    {
                        continue;
                    }
                    var MevcutNokta = noktalar.Where(a => a.nokta_isim == EldekiNokta.NoktaAdi).FirstOrDefault();
                    var arac = araclar.Where(a => a.DorsePlaka == dorsePlaka).FirstOrDefault();
                    if (arac == null)
                        arac = araclar.FirstOrDefault();
                    t.NakliyeNo = NakliyeNo;
                    t.KaynakId = MevcutNokta.Id;

                    t.TasimaninAraci.AracId = arac.Id;
                    t.TasimaninAraci.AracTip = arac.AracTipId;
                    tempLokasyon = Lokasyon;

                }
                if (tempLokasyon != string.Empty && tempLokasyon != Lokasyon && SiraId != 1)
                {
                    var HedefNokta = new Nokta();
                    tempLokasyon = Lokasyon;
                    if (Lokasyon.StartsWith("000"))
                        Lokasyon = Lokasyon.Remove(0, 3);
                    else if (Lokasyon.StartsWith("00"))
                        Lokasyon = Lokasyon.Remove(0, 2);
                    var EldekiNokta = kaynakNoktalar.Where(a => (a.Kodu2 ?? string.Empty).Contains(Lokasyon)).FirstOrDefault();
                    if (EldekiNokta == null)
                    {
                        continue;
                    }
                    var MevcutNokta = noktalar.Where(a => a.nokta_isim.ToLower().Contains(EldekiNokta.NoktaAdi.ToLower())).FirstOrDefault();
                    if (MevcutNokta == null)
                    {
                        continue;
                    }
                   
                    var arac = araclar.Where(a => a.DorsePlaka == dorsePlaka).FirstOrDefault();
                    if (arac == null)
                        arac = araclar.FirstOrDefault();
                    t.NakliyeNo = NakliyeNo;
                    t.TasimaninAraci.AracId = arac.Id;
                    t.TasimaninAraci.AracTip = arac.AracTipId;
                    if (t.KaynakId == 0)
                    {
                        t.KaynakId = KaynakNokta.Id;
                        t.HedefId = MevcutNokta.Id;
                        KaynakNokta.Id = MevcutNokta.Id;
                        Tasimalar.Add(t);
                        t = new Tasima();
                    }
                    else
                    {
                        t.HedefId = MevcutNokta.Id;
                        KaynakNokta.Id = MevcutNokta.Id;
                        Tasimalar.Add(t);
                        t = new Tasima();
                    }

                    using (dbTezRotaDataContext dc = new dbTezRotaDataContext())
                    {
                        var nokta = dc.tNoktas.Where(a => a.Id == MevcutNokta.Id).FirstOrDefault();
                        nokta.musteriKodu = Lokasyon;
                        dc.SubmitChanges();

                    }
                }


            }

            var dur = 0;
            using (dbTezRotaDataContext dc = new dbTezRotaDataContext())
            {
                tPlan tp = new tPlan();
                tp.PlanIsmi = FilePath;
                dc.tPlans.InsertOnSubmit(tp);
                dc.SubmitChanges();
                int s = 0;
                var t2 = string.Empty;
                var naklileyeler = Tasimalar.GroupBy(a => a.NakliyeNo).ToList();

                foreach (var n in naklileyeler)
                {
                    s = 0;
                    if (n.Where(a => a.HedefId == 65).Count() < 3)
                    {
                        if (n.Count() > 3)
                        {
                            foreach (var te in n)
                            {

                                tTasima ta = new tTasima();
                                ta.PlanId = tp.Id;
                                ta.KaynakId = te.KaynakId;
                                ta.HedefId = te.HedefId;
                                ta.NakliyeNo = te.NakliyeNo;
                                ta.SiraId = s;
                                s++;
                                dc.tTasimas.InsertOnSubmit(ta);
                                dc.SubmitChanges();
                            }
                        }

                    }


                }
            }
        }
        public void Dispose()
        {

            GC.SuppressFinalize(this);
        }
    }
}
