namespace TezRota
{
    partial class AnaEkran
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tableLayoutPanel0 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.cmbPlan = new System.Windows.Forms.ComboBox();
            this.btnPlanOlustur = new System.Windows.Forms.Button();
            this.btnPlanRotala = new System.Windows.Forms.Button();
            this.tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtSicaklik = new System.Windows.Forms.TextBox();
            this.txtDongu = new System.Windows.Forms.TextBox();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.txtEnIyiCozum = new System.Windows.Forms.TextBox();
            this.txtBaslangicCozum = new System.Windows.Forms.TextBox();
            this.tableLayoutPanel5 = new System.Windows.Forms.TableLayoutPanel();
            this.btnPlanAktar = new System.Windows.Forms.Button();
            this.txtExcelFilePath = new System.Windows.Forms.TextBox();
            this.btnPlanSec = new System.Windows.Forms.Button();
            this.tableLayoutPanel0.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel4.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.tableLayoutPanel3.SuspendLayout();
            this.tableLayoutPanel5.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel0
            // 
            this.tableLayoutPanel0.ColumnCount = 1;
            this.tableLayoutPanel0.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel0.Controls.Add(this.tableLayoutPanel1, 0, 0);
            this.tableLayoutPanel0.Controls.Add(this.tableLayoutPanel2, 0, 1);
            this.tableLayoutPanel0.Controls.Add(this.tableLayoutPanel5, 0, 2);
            this.tableLayoutPanel0.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel0.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel0.Name = "tableLayoutPanel0";
            this.tableLayoutPanel0.RowCount = 3;
            this.tableLayoutPanel0.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 6F));
            this.tableLayoutPanel0.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 88F));
            this.tableLayoutPanel0.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 6F));
            this.tableLayoutPanel0.Size = new System.Drawing.Size(1220, 614);
            this.tableLayoutPanel0.TabIndex = 0;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 4;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 40F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel1.Controls.Add(this.cmbPlan, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.btnPlanOlustur, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.btnPlanRotala, 3, 0);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel4, 2, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1214, 30);
            this.tableLayoutPanel1.TabIndex = 1;
            // 
            // cmbPlan
            // 
            this.cmbPlan.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.cmbPlan.FormattingEnabled = true;
            this.cmbPlan.Location = new System.Drawing.Point(25, 4);
            this.cmbPlan.Name = "cmbPlan";
            this.cmbPlan.Size = new System.Drawing.Size(192, 21);
            this.cmbPlan.TabIndex = 0;
            this.cmbPlan.SelectedIndexChanged += new System.EventHandler(this.CmbPlan_SelectedIndexChanged);
            // 
            // btnPlanOlustur
            // 
            this.btnPlanOlustur.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnPlanOlustur.Location = new System.Drawing.Point(245, 3);
            this.btnPlanOlustur.Name = "btnPlanOlustur";
            this.btnPlanOlustur.Size = new System.Drawing.Size(236, 24);
            this.btnPlanOlustur.TabIndex = 2;
            this.btnPlanOlustur.Text = "Plan Üret";
            this.btnPlanOlustur.UseVisualStyleBackColor = true;
            this.btnPlanOlustur.Click += new System.EventHandler(this.BtnPlanOlustur_Click);
            // 
            // btnPlanRotala
            // 
            this.btnPlanRotala.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnPlanRotala.Location = new System.Drawing.Point(972, 3);
            this.btnPlanRotala.Name = "btnPlanRotala";
            this.btnPlanRotala.Size = new System.Drawing.Size(239, 24);
            this.btnPlanRotala.TabIndex = 3;
            this.btnPlanRotala.Text = "Planı Rotala";
            this.btnPlanRotala.UseVisualStyleBackColor = true;
            this.btnPlanRotala.Click += new System.EventHandler(this.BtnPlanRotala_Click);
            // 
            // tableLayoutPanel4
            // 
            this.tableLayoutPanel4.ColumnCount = 4;
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel4.Controls.Add(this.label1, 2, 0);
            this.tableLayoutPanel4.Controls.Add(this.label2, 0, 0);
            this.tableLayoutPanel4.Controls.Add(this.txtSicaklik, 1, 0);
            this.tableLayoutPanel4.Controls.Add(this.txtDongu, 3, 0);
            this.tableLayoutPanel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel4.Location = new System.Drawing.Point(487, 3);
            this.tableLayoutPanel4.Name = "tableLayoutPanel4";
            this.tableLayoutPanel4.RowCount = 1;
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel4.Size = new System.Drawing.Size(479, 24);
            this.tableLayoutPanel4.TabIndex = 4;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(241, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(39, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Döngü";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(44, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Sıcaklık";
            // 
            // txtSicaklik
            // 
            this.txtSicaklik.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtSicaklik.Location = new System.Drawing.Point(122, 3);
            this.txtSicaklik.Name = "txtSicaklik";
            this.txtSicaklik.Size = new System.Drawing.Size(113, 20);
            this.txtSicaklik.TabIndex = 0;
            // 
            // txtDongu
            // 
            this.txtDongu.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtDongu.Location = new System.Drawing.Point(360, 3);
            this.txtDongu.Name = "txtDongu";
            this.txtDongu.Size = new System.Drawing.Size(116, 20);
            this.txtDongu.TabIndex = 2;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 1;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Controls.Add(this.dataGridView1, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.tableLayoutPanel3, 0, 1);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 39);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 2;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 60F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 40F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(1214, 534);
            this.tableLayoutPanel2.TabIndex = 2;
            // 
            // dataGridView1
            // 
            this.dataGridView1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridView1.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView1.Location = new System.Drawing.Point(3, 3);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.Size = new System.Drawing.Size(1208, 314);
            this.dataGridView1.TabIndex = 3;
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.ColumnCount = 2;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel3.Controls.Add(this.txtEnIyiCozum, 1, 0);
            this.tableLayoutPanel3.Controls.Add(this.txtBaslangicCozum, 0, 0);
            this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel3.Location = new System.Drawing.Point(3, 323);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 1;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(1208, 208);
            this.tableLayoutPanel3.TabIndex = 4;
            // 
            // txtEnIyiCozum
            // 
            this.txtEnIyiCozum.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtEnIyiCozum.Location = new System.Drawing.Point(607, 3);
            this.txtEnIyiCozum.Multiline = true;
            this.txtEnIyiCozum.Name = "txtEnIyiCozum";
            this.txtEnIyiCozum.Size = new System.Drawing.Size(598, 202);
            this.txtEnIyiCozum.TabIndex = 1;
            // 
            // txtBaslangicCozum
            // 
            this.txtBaslangicCozum.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtBaslangicCozum.Location = new System.Drawing.Point(3, 3);
            this.txtBaslangicCozum.Multiline = true;
            this.txtBaslangicCozum.Name = "txtBaslangicCozum";
            this.txtBaslangicCozum.Size = new System.Drawing.Size(598, 202);
            this.txtBaslangicCozum.TabIndex = 0;
            // 
            // tableLayoutPanel5
            // 
            this.tableLayoutPanel5.ColumnCount = 3;
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 60F));
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel5.Controls.Add(this.btnPlanAktar, 2, 0);
            this.tableLayoutPanel5.Controls.Add(this.txtExcelFilePath, 0, 0);
            this.tableLayoutPanel5.Controls.Add(this.btnPlanSec, 1, 0);
            this.tableLayoutPanel5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel5.Location = new System.Drawing.Point(3, 579);
            this.tableLayoutPanel5.Name = "tableLayoutPanel5";
            this.tableLayoutPanel5.RowCount = 1;
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel5.Size = new System.Drawing.Size(1214, 32);
            this.tableLayoutPanel5.TabIndex = 3;
            // 
            // btnPlanAktar
            // 
            this.btnPlanAktar.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnPlanAktar.Location = new System.Drawing.Point(973, 3);
            this.btnPlanAktar.Name = "btnPlanAktar";
            this.btnPlanAktar.Size = new System.Drawing.Size(238, 26);
            this.btnPlanAktar.TabIndex = 0;
            this.btnPlanAktar.Text = "Plan Aktar";
            this.btnPlanAktar.UseVisualStyleBackColor = true;
            this.btnPlanAktar.Click += new System.EventHandler(this.BtnPlanAktar_Click);
            // 
            // txtExcelFilePath
            // 
            this.txtExcelFilePath.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtExcelFilePath.Location = new System.Drawing.Point(3, 3);
            this.txtExcelFilePath.Name = "txtExcelFilePath";
            this.txtExcelFilePath.Size = new System.Drawing.Size(722, 20);
            this.txtExcelFilePath.TabIndex = 1;
            // 
            // btnPlanSec
            // 
            this.btnPlanSec.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnPlanSec.Location = new System.Drawing.Point(731, 3);
            this.btnPlanSec.Name = "btnPlanSec";
            this.btnPlanSec.Size = new System.Drawing.Size(236, 26);
            this.btnPlanSec.TabIndex = 2;
            this.btnPlanSec.Text = "Plan Seç";
            this.btnPlanSec.UseVisualStyleBackColor = true;
            this.btnPlanSec.Click += new System.EventHandler(this.BtnPlanSec_Click);
            // 
            // AnaEkran
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1220, 614);
            this.Controls.Add(this.tableLayoutPanel0);
            this.Name = "AnaEkran";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.AnaEkran_Load);
            this.tableLayoutPanel0.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel4.ResumeLayout(false);
            this.tableLayoutPanel4.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel3.PerformLayout();
            this.tableLayoutPanel5.ResumeLayout(false);
            this.tableLayoutPanel5.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel0;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.ComboBox cmbPlan;
        private System.Windows.Forms.Button btnPlanOlustur;
        private System.Windows.Forms.Button btnPlanRotala;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.TextBox txtEnIyiCozum;
        private System.Windows.Forms.TextBox txtBaslangicCozum;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel4;
        private System.Windows.Forms.TextBox txtSicaklik;
        private System.Windows.Forms.TextBox txtDongu;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel5;
        private System.Windows.Forms.Button btnPlanAktar;
        private System.Windows.Forms.TextBox txtExcelFilePath;
        private System.Windows.Forms.Button btnPlanSec;
    }
}

