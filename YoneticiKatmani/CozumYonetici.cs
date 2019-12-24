using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TezRota.ModelKatmani;
using TezRota.VeriKatmani;
using TezRota.YardimciKatmani;

namespace TezRota.YoneticiKatmani
{
    public class CozumYonetici : IDisposable
    {
        public int Sicaklik { get; set; }
        public int Dongu { get; set; }
        public double SicaklikAzaltmaOrani { get; set; }
        public int PlanId { get; set; }
        public List<Nokta> NoktaListesi { get; set; }
        public List<int> AracTipleri { get; set; }//Mevcutta bulunan araç tipleri
        public List<Arac> AracListesi { get; set; }
        public List<Nakliye> NakliyeListesi { get; set; }
      
        public List<Tasima> TasimaListesi { get; set; }
        public double ELMyeOlanUZaklik { get; set; }
        public Random rastgele { get; set; }
        public List<Nakliye> EnIyiCozum = new List<Nakliye>();

        public CozumYonetici()
        {
            NoktaListesi = new List<Nokta>();
            AracTipleri = new List<int>();
            AracListesi = new List<Arac>();
            NakliyeListesi = new List<Nakliye>();
            ELMyeOlanUZaklik = 0;
            rastgele = new Random();
            Sicaklik = 25;
            Dongu = 100;
            SicaklikAzaltmaOrani = 0.99;
        }
        public void TasimaListesiniOku()
        {
            using (dbTezRotaDataContext dc = new dbTezRotaDataContext())
            {
                AracTipleri = dc.tAracTipis.Select(a => a.Id).ToList();
                TasimaListesi = dc.tTasimas.Where(a => a.PlanId==this.PlanId).Select(a => a.Tasima()).ToList();
                //TasimaListesi = dc.tTasimas.Where(a => this.AracTipleri.Contains(a.TasimaTipi)).Select(a => a.Tasima()).ToList();
            }
        }
        public void AracListesiniOku()
        {
            using (dbTezRotaDataContext dc = new dbTezRotaDataContext())
            {
                AracListesi = dc.tAracs.Where(a => this.AracTipleri.Contains(a.AracTipId)).Select(a => a.Arac()).ToList();
            }
        }

        public void NoktaListesiniOku()
        {
            using (dbTezRotaDataContext dc = new dbTezRotaDataContext())
            {
                var result = dc.tNoktas.ToList();
                foreach (var item in result)
                {
                    var tnokta = dc.tNoktas.Where(a => a.Id == item.Id).FirstOrDefault();
                    var kordinate = tnokta.kordinat.Split(',');
                    double enlem = double.Parse(kordinate.ElementAt(0).Trim(), System.Globalization.NumberStyles.AllowDecimalPoint);
                    double boylam = double.Parse(kordinate.ElementAt(1).Trim(), System.Globalization.NumberStyles.AllowDecimalPoint);
                    Nokta n = new Nokta();
                    n.Enlem = enlem;
                    n.Boylam = boylam;
                    n.Id = tnokta.Id;
                    NoktaListesi.Add(n);
                }
            }
        }
     
        public void TasimaNoktaAracBilgisiEsle()
        {
            List<Arac> secimTorbasi = new List<Arac>();
            //taşımaya aracını ata
            foreach (var item in TasimaListesi)
            {
                var kn = NoktaGetir(item.KaynakId);
                var hn = NoktaGetir(item.HedefId);
                item.KaynakNokta = kn;
                item.HedefNokta = hn;
                Arac tempArac = AracListesi.Except(secimTorbasi).FirstOrDefault(a => a.AracTip == item.TasimaTipiId);
                item.TasimaninAraci = tempArac;
                secimTorbasi.Add(tempArac);
            }
        }
       
        public void BaslangicKromozomOlustur()
        {
            var result = this.TasimaListesi.GroupBy(u => u.NakliyeNo) .Select(grp => grp.ToList());
           
            foreach (var item in result)
            {
                var ilkTasima = item.FirstOrDefault();     
                var nakliye = new Nakliye();
                nakliye.NakliyeNo = ilkTasima.NakliyeNo;
                nakliye.TasimaListesi.AddRange(item);
                nakliye.RotaList = item.Where(a => a.KaynakNokta.Id != 65 && a.HedefNokta.Id != 65).Select(a => a.HedefId).ToList();
                nakliye.PlanId = this.PlanId;
                nakliye.NakSkor = RotaSkorHesapla(nakliye.RotaList);
                nakliye.AtananArac = ilkTasima.TasimaninAraci;
                this.NakliyeListesi.Add(nakliye);

            }
           
        }
        private double IkiNoktaArasiUzaklikHesapla(double lat1, double lon1, double lat2, double lon2)
        {
            if ((lat1 == lat2) && (lon1 == lon2))
            {
                return 0;
            }
            else
            {
                double theta = lon1 - lon2;
                double dist = Math.Sin(deg2rad(lat1)) * Math.Sin(deg2rad(lat2)) + Math.Cos(deg2rad(lat1)) * Math.Cos(deg2rad(lat2)) * Math.Cos(deg2rad(theta));
                dist = Math.Acos(dist);
                dist = rad2deg(dist);
                dist = dist * 60 * 1.1515;

                dist = dist * 1.609344;

                return (dist);
            }
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //::  This function converts decimal degrees to radians             :::
        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        private double deg2rad(double deg)
        {
            return (deg * Math.PI / 180.0);
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //::  This function converts radians to decimal degrees             :::
        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        private double rad2deg(double rad)
        {
            return (rad / Math.PI * 180.0);
        }


        public double RotaSkorHesapla(List<int> _rotaListesi)
        {
            double nakSkor = 0;
            var kaynakNokta = new Nokta() { Id = 65, Enlem = 39.752254700000265,Boylam= 30.662165899999618 };
            var sonHedefNokta = new Nokta() { Id = 65, Enlem = 39.752254700000265, Boylam = 30.662165899999618 };
            Nokta tempHedefNokta = kaynakNokta;
            for (int i = 0; i < _rotaListesi.Count; i++)
            {
                //Her zaman ilk rota ELM
                var hedefNokta = NoktaListesi.Where(a => a.Id == _rotaListesi.ElementAt(i)).FirstOrDefault();
                var uzaklik = IkiNoktaArasiUzaklikHesapla(kaynakNokta.Enlem, kaynakNokta.Boylam, hedefNokta.Enlem, hedefNokta.Boylam);
                nakSkor = nakSkor + uzaklik;
                kaynakNokta = hedefNokta;
            }
            var _uzaklik = IkiNoktaArasiUzaklikHesapla(kaynakNokta.Enlem, kaynakNokta.Boylam, sonHedefNokta.Enlem, sonHedefNokta.Boylam);
            nakSkor = nakSkor + _uzaklik;
            return nakSkor;
        }
        public Nokta NoktaGetir(int id)
        {
            return NoktaListesi.Where(a => a.Id == id).FirstOrDefault();

        }
        public double NakliyeSkorHesapla(List<Nakliye> _nakliyeListesi)
        {
            double nakSkor = 0;
            double maxSkor = double.MaxValue;
            foreach(var item in _nakliyeListesi)
            {
                
                nakSkor = RotaSkorHesapla(item.RotaList);
                item.NakSkor = nakSkor;
                //System.Diagnostics.Debug.WriteLine(item.NakliyeNo+" : " + item.NakSkor + " : 65," + string.Join(",", item.RotaList));
            }
            maxSkor= _nakliyeListesi.Max(a => nakSkor);
            return maxSkor;
        }
        public void TavlamaBenzetimiUygula()
        {
            //her aracın bulunduğu konumdan ELM ye olan uzaklıkları şimdilik 0
            //sicaklik 0
            var seed = (int)DateTime.Now.Ticks;
            var rnd = new Random(seed);
            double sicaklik = this.Sicaklik ;
            double sao = this.SicaklikAzaltmaOrani;
            double MaxSkor = double.MaxValue;
            double KomsuSkor1 = 0;
            double KomsuSkor2 = 0;
            double BestSkor = 0;
            double Delta = 0;
            int Iteration = this.Dongu;
            
            List<Nakliye> KomsulukCozum1 = new List<Nakliye>();
            List<Nakliye> KomsulukCozum2 = new List<Nakliye>();
            List<int> SecilenAracTorbasi = new List<int>();
            KomsulukCozum1.AddRange(NakliyeListesi);//komşuluk1

            List<int> KRota1 = new List<int>();
            List<int> KRota2 = new List<int>();
 
            while (sicaklik > 1)
            {
                for (int k = 0; k < Iteration; k++)
                {
                    //System.Diagnostics.Debug.WriteLine("Sıcaklık : "+sicaklik+" : "+ "KomşuSkor1 : "+ KomsuSkor1);
                    //her nakliye için her iterasyonda bir swap yapılır_> Komşuluk 2
                    KomsulukCozum2.Clear();
                    foreach (var n in KomsulukCozum1)
                    {
                        var nakl = new Nakliye();
                           
                        KRota1.Clear();
                        KRota1.AddRange(n.RotaList);//Komşuluk1

                        //İç rotadan rassal iki taşıma seç
                        int ind1 = rnd.Next(0, KRota1.Count);
                        int ind2 = rnd.Next(0, KRota1.Count);

                        //seçilen taşımaları bul
                        var r1 = KRota1.ElementAt(ind1);
                        var r2 = KRota1.ElementAt(ind2);
                        //Debug.WriteLine(string.Join(",", n.RotaList));

                        //swap işlemini yap
                        n.RotaList[ind1] = r2;
                        n.RotaList[ind2] = r1;
                        //Debug.WriteLine(string.Join(",",n.RotaList));

                        KRota2.Clear();
                        KRota2.AddRange(n.RotaList); //Komşuluk2

                        nakl.NakliyeNo = n.NakliyeNo;
                        nakl.RotaList.AddRange(KRota2);
                        nakl.RotaList.Clear();
                        nakl.RotaList.AddRange(n.RotaList);
                        nakl.NakSkor = RotaSkorHesapla(n.RotaList);
                        KomsulukCozum2.Add(nakl); 
                    }

                    Debug.WriteLine(Environment.NewLine);
                    KomsuSkor1 = KomsulukCozum1.Max(a => a.NakSkor);
                    Debug.WriteLine("Sıcaklık : "+sicaklik+" KomşuSkor1 : " + KomsuSkor1);

                    KomsuSkor2 = KomsulukCozum2.Max(a => a.NakSkor);
                    Debug.WriteLine("KomşuSkor2 : " + KomsuSkor2);
                    Delta = KomsuSkor2 - KomsuSkor1;

                    if (Delta <= 0)
                    {
                        KomsulukCozum1.Clear();
                        for (int i = 0; i < KomsulukCozum2.Count; i++)
                        {
                            KomsulukCozum1.Add(KomsulukCozum2[i]);
                        }        
                    }
                    
                    else if (rnd.NextDouble() < (Math.Exp(-Delta / sicaklik)))
                    {
                        KomsulukCozum1.Clear();
                        for (int i = 0; i < KomsulukCozum2.Count; i++)
                        {
                            KomsulukCozum1.Add(KomsulukCozum2[i]);
                        }
                    }
                    else
                    {
                        KomsulukCozum2.Clear();
                        for (int i = 0; i < KomsulukCozum1.Count; i++)
                        {
                            KomsulukCozum2.Add(KomsulukCozum1[i]);
                        }
                    }
                    if (KomsuSkor2 < MaxSkor)
                    {

                        MaxSkor = KomsuSkor2;
                        BestSkor = KomsuSkor2;
                        EnIyiCozum.Clear();
                        for (int i = 0; i < KomsulukCozum2.Count; i++)
                        {
                            EnIyiCozum.Add(KomsulukCozum2[i]);
                        }
                        System.Diagnostics.Debug.WriteLine("Sıcaklık :"+sicaklik+" "+ MaxSkor);
                    }
                }
                sicaklik = sicaklik * sao;
            }
        }

        public void SonucBilginiYaz()
        {
            using (dbTezRotaDataContext dc = new dbTezRotaDataContext())
            {
                tOptimizasyonEmir tOp = new tOptimizasyonEmir();
                tOp.BaslangicSkor = NakliyeListesi.Max(a => a.NakSkor).ToString();
                tOp.PlanId = this.PlanId;
                tOp.Sicaklik = this.Sicaklik;
                tOp.SicaklikAzaltmaOrani = this.SicaklikAzaltmaOrani;
                tOp.Dongu = this.Dongu;
                tOp.SonucSkor = this.EnIyiCozum.Max(a => a.NakSkor).ToString();
                tOp.EmirZamani = DateTime.Now;
                dc.tOptimizasyonEmirs.InsertOnSubmit(tOp);
                dc.SubmitChanges();

                tOptimizasyonSonuc tos = new tOptimizasyonSonuc();

                int i = 0;
                foreach (var item in this.EnIyiCozum) 
                {
                  
                    foreach(var n in item.RotaList)
                    {
                        tos = new tOptimizasyonSonuc();
                        tos.EmirId = tOp.Id;
                        tos.NakliyeNo = item.NakliyeNo;
                        var tasimaListesi = this.NakliyeListesi.Where(a => a.NakliyeNo == item.NakliyeNo && a.PlanId==PlanId)
                                                      .FirstOrDefault().TasimaListesi;
                        Tasima tasima;
                        if (i == 0)
                        {
                            tasima = tasimaListesi.Where(a => a.KaynakId == 65).FirstOrDefault();
                            i = 99;

                        }
                        tasima = tasimaListesi.Where(a => a.HedefId == n).FirstOrDefault();

                                     
                        tos.TasimaId = tasima.Id;
                        dc.tOptimizasyonSonucs.InsertOnSubmit(tos);
                        dc.SubmitChanges();
                    }
                    
                }
            }
            
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}




/* //System.Diagnostics.Debug.WriteLine("------------------------------------------------------------");
                       //System.Diagnostics.Debug.WriteLine("Komşuluk 1 : 65," + string.Join(",", n.TasimaListesi.Select(a => a.HedefId).ToList()));
                        rnd.Next();
                        var nakl = new Nakliye();
                        nakl.TasimaListesi.AddRange(n.TasimaListesi);
                        //başlangıç ve bitiş noktası dışındakileri al
                        var rList = n.TasimaListesi.Where(a => a.KaynakNokta.Id != 65 && a.HedefNokta.Id != 65).ToList();
                       
                        //İç rotadan rassal iki taşıma seç
                        int ind1 = rnd.Next(0, rList.Count);
                        int ind2= rnd.Next(0, rList.Count);

                        //seçilen taşımaları bul
                        var r1 = n.TasimaListesi.ElementAt(ind1);
                        var r2 = n.TasimaListesi.ElementAt(ind2);

                        //seçilen taşımaları belirle Çünkü başlangıç ve bitiş taşımalarını en başta dışladın
                        var ran1 = n.TasimaListesi.Where(a => a.Id == r1.Id).FirstOrDefault();
                        var ran2 = n.TasimaListesi.Where(a => a.Id == r2.Id).FirstOrDefault();
                        //System.Diagnostics.Debug.WriteLine("N: "+n.NakliyeNo+" Swap :" + ran2.HedefNokta.Id + " => " + ran1.HedefNokta.Id);

                        //taşımayı belirle
                        var a1 = n.TasimaListesi.IndexOf(ran1);
                        var a2 = n.TasimaListesi.IndexOf(ran2);
                        var s1= ran1.SiraId;
                        var s2 = ran2.SiraId;

                        nakl.TasimaListesi[a1].SiraId = s2;

                        nakl.TasimaListesi[a2].SiraId = s1;

                        //swap işlemini yap
                        nakl.TasimaListesi[a2] = ran1;
                        nakl.TasimaListesi[a1] = ran2;

                        

                        //System.Diagnostics.Debug.WriteLine("Komşuluk 2 : 65," + string.Join(",", nakl.TasimaListesi.Select(a => a.HedefId).ToList()));
                        //swapten dolayı kaynak-hedef guncelleme işlemi yapılabilmeli.
                        //System.Diagnostics.Debug.WriteLine("Komşuluk 2 : " + string.Join(",", nakl.TasimaListesi.Select(a => a.SiraId).ToList()));
                        nakl.TasimaListesi = nakl.TasimaListesi.OrderBy(a=>a.SiraId).ToList();
                        //System.Diagnostics.Debug.WriteLine("Komşuluk 2 : " + string.Join(",", nakl.TasimaListesi.Select(a => a.SiraId).ToList()));
                        //System.Diagnostics.Debug.WriteLine("Komşuluk 2 : 65," + string.Join(",", nakl.TasimaListesi.Select(a => a.HedefId).ToList()));
                        
                        nakl.RotaList = nakl.TasimaListesi.Select(a => a.HedefId).ToList();
                        nakl.NakliyeNo = n.NakliyeNo;
                       
                        KomsulukCozum2.Add(nakl);*/
