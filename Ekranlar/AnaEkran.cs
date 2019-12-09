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
    }
}
