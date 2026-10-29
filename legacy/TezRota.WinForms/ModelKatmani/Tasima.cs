using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TezRota.ModelKatmani
{
    public class Tasima
    {
        public Tasima()
        {
            TasimaninAraci = new Arac();
            KaynakNokta = new Nokta();
            HedefNokta = new Nokta();
        }
        public int Id { get; set; }
        public string NakliyeNo { get; set; }
        public int TasimaTipiId { get; set; }
        public Nokta KaynakNokta { get; set; }
        public Nokta HedefNokta { get; set; }
        public double KaynakHedefArasiMesafe { get; set; }
        public int KaynakId { get; set; }
        public int HedefId { get; set; }
        public Arac TasimaninAraci { get; set; }
        public int SiraId { get; set; }
    }
}
