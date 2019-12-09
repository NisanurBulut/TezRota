using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TezRota.VeriKatmani;

namespace TezRota.YoneticiKatmani
{
    public class PlanYonetici:IDisposable
    {
        public int RotaSayisi { get; set; }
        public int maxSiraId { get; set; }

        public int ELM = 65;
        public  PlanYonetici()
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
            maxSiraId = 3;
            RotaSayisi = 10;
            List<int> tasimaTipleri = new List<int>() { 1, 2, 3 };
            using (dbTezRotaDataContext dc=new dbTezRotaDataContext())
            {
                List<int> NoktaIdListesi = dc.tNoktas.Where(a=>a.Id!=ELM).Select(a => a.Id).ToList();
                List<int> tempHedefListesi = new List<int>();
                int noktaSayisi = NoktaIdListesi.Count;
                int tasimaTipSayisi = tasimaTipleri.Count;
                tPlan plan=new tPlan();
                plan.PlanIsmi = "Plan - " + DateTime.Now.ToString();
                dc.tPlans.InsertOnSubmit(plan);
                dc.SubmitChanges();
                MessageBox.Show("Plan başarılı şekilde oluşturulmuştur.");
                int KaynakId = ELM;
                int HedefId = 0;
                //taşıma oluşturma 
                for (int i=0; i<RotaSayisi; i++)
                {
                    string nakliyeNo = "N-" + i;
                    for (int s=0; s<maxSiraId; s++)
                    {
                        if(s==0)
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
                            int ind = new Random().Next(0, randomHedefList.Count());
                            HedefId = randomHedefList.ElementAt(ind);
                            tempHedefListesi.Add(HedefId);
                        }
                       

                        int tid = new Random().Next(0, tasimaTipSayisi);
                        
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

        public void Dispose()
        {
            
            GC.SuppressFinalize(this);
        }
    }
}
