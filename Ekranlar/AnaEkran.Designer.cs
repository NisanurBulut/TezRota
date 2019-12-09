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
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.cmbPlan = new System.Windows.Forms.ComboBox();
            this.btnPlanOlustur = new System.Windows.Forms.Button();
            this.btnPlanRotala = new System.Windows.Forms.Button();
            this.tableLayoutPanel0.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel0
            // 
            this.tableLayoutPanel0.ColumnCount = 1;
            this.tableLayoutPanel0.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel0.Controls.Add(this.dataGridView1, 0, 1);
            this.tableLayoutPanel0.Controls.Add(this.tableLayoutPanel1, 0, 0);
            this.tableLayoutPanel0.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel0.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel0.Name = "tableLayoutPanel0";
            this.tableLayoutPanel0.RowCount = 2;
            this.tableLayoutPanel0.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 8F));
            this.tableLayoutPanel0.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 92F));
            this.tableLayoutPanel0.Size = new System.Drawing.Size(800, 450);
            this.tableLayoutPanel0.TabIndex = 0;
            // 
            // dataGridView1
            // 
            this.dataGridView1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridView1.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView1.Location = new System.Drawing.Point(3, 39);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.Size = new System.Drawing.Size(794, 408);
            this.dataGridView1.TabIndex = 0;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 4;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.Controls.Add(this.cmbPlan, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.btnPlanOlustur, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.btnPlanRotala, 2, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(794, 30);
            this.tableLayoutPanel1.TabIndex = 1;
            // 
            // cmbPlan
            // 
            this.cmbPlan.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.cmbPlan.FormattingEnabled = true;
            this.cmbPlan.Location = new System.Drawing.Point(3, 4);
            this.cmbPlan.Name = "cmbPlan";
            this.cmbPlan.Size = new System.Drawing.Size(192, 21);
            this.cmbPlan.TabIndex = 0;
            this.cmbPlan.SelectedIndexChanged += new System.EventHandler(this.CmbPlan_SelectedIndexChanged);
            // 
            // btnPlanOlustur
            // 
            this.btnPlanOlustur.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnPlanOlustur.Location = new System.Drawing.Point(201, 3);
            this.btnPlanOlustur.Name = "btnPlanOlustur";
            this.btnPlanOlustur.Size = new System.Drawing.Size(192, 24);
            this.btnPlanOlustur.TabIndex = 2;
            this.btnPlanOlustur.Text = "Plan Üret";
            this.btnPlanOlustur.UseVisualStyleBackColor = true;
            this.btnPlanOlustur.Click += new System.EventHandler(this.BtnPlanOlustur_Click);
            // 
            // btnPlanRotala
            // 
            this.btnPlanRotala.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnPlanRotala.Location = new System.Drawing.Point(399, 3);
            this.btnPlanRotala.Name = "btnPlanRotala";
            this.btnPlanRotala.Size = new System.Drawing.Size(192, 24);
            this.btnPlanRotala.TabIndex = 3;
            this.btnPlanRotala.Text = "Planı Rotala";
            this.btnPlanRotala.UseVisualStyleBackColor = true;
            // 
            // AnaEkran
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.tableLayoutPanel0);
            this.Name = "AnaEkran";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.AnaEkran_Load);
            this.tableLayoutPanel0.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel0;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.ComboBox cmbPlan;
        private System.Windows.Forms.Button btnPlanOlustur;
        private System.Windows.Forms.Button btnPlanRotala;
    }
}

