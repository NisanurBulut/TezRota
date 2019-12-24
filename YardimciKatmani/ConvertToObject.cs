using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TezRota.ModelKatmani;
using TezRota.VeriKatmani;

namespace TezRota.YardimciKatmani
{
   public static class ConvertToObject
    {
        public static Arac Arac(this tArac arac)
        {
            return new Arac()
            {
                AracId=arac.Id,
                AracTip=arac.AracTipId
            };
        }
        public static Tasima Tasima(this tTasima item)
        {
            return new Tasima()
            {
                KaynakHedefArasiMesafe = 0,
                TasimaTipiId = item.TasimaTipi,
                KaynakNokta = new Nokta(),
                HedefNokta = new Nokta(),
                Id = item.Id,
                NakliyeNo = item.NakliyeNo,
                KaynakId = item.KaynakId,
                HedefId = item.HedefId,
                SiraId = item.SiraId
            };
        }
    }
}
