using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TezRota.ModelKatmani
{
   public class Nakliye
    {
        public string NakliyeNo { get; set; }
        public List<int> RotaList { get; set; }//Nakliye içerisinde dolaşılacak olan nokta listesi
        public List<Tasima> TasimaListesi { get; set; }
        public List<int> AtanabilecekAraclar { get; set; }
        public Arac AtananArac { get; set; }//Nakliyeyi kullanan araç bilgisi
        public int PlanId { get; set; }
        public double NakSkor { get; set; }
        public Nakliye()
        {
            AtananArac = new Arac();
            TasimaListesi = new List<Tasima>();
            RotaList = new List<int>();
        }

    }
}
