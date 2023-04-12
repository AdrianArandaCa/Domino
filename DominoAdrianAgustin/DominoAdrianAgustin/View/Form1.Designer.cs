namespace DominoAdrianAgustin.View
{
    partial class Form1
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
            this.labelTablero = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.nomTB = new System.Windows.Forms.TextBox();
            this.servidorTB = new System.Windows.Forms.TextBox();
            this.conectarBtn = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.missatgeLB = new System.Windows.Forms.ListBox();
            this.enviarBtn = new System.Windows.Forms.Button();
            this.missatgeTB = new System.Windows.Forms.TextBox();
            this.tornLBL = new System.Windows.Forms.Label();
            this.pasarBtn = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // labelTablero
            // 
            this.labelTablero.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.labelTablero.Location = new System.Drawing.Point(12, 251);
            this.labelTablero.Name = "labelTablero";
            this.labelTablero.Size = new System.Drawing.Size(1072, 170);
            this.labelTablero.TabIndex = 0;
            this.labelTablero.Click += new System.EventHandler(this.label1_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(23, 40);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(49, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Servidor:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(218, 40);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(32, 13);
            this.label3.TabIndex = 3;
            this.label3.Text = "Nom:";
            // 
            // nomTB
            // 
            this.nomTB.Location = new System.Drawing.Point(221, 57);
            this.nomTB.Name = "nomTB";
            this.nomTB.Size = new System.Drawing.Size(102, 20);
            this.nomTB.TabIndex = 4;
            this.nomTB.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // servidorTB
            // 
            this.servidorTB.Location = new System.Drawing.Point(26, 57);
            this.servidorTB.Name = "servidorTB";
            this.servidorTB.Size = new System.Drawing.Size(138, 20);
            this.servidorTB.TabIndex = 5;
            // 
            // conectarBtn
            // 
            this.conectarBtn.Location = new System.Drawing.Point(341, 54);
            this.conectarBtn.Name = "conectarBtn";
            this.conectarBtn.Size = new System.Drawing.Size(75, 23);
            this.conectarBtn.TabIndex = 6;
            this.conectarBtn.Text = "Conectar";
            this.conectarBtn.UseVisualStyleBackColor = true;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(23, 104);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(52, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "Missatge:";
            this.label4.Click += new System.EventHandler(this.label4_Click);
            // 
            // missatgeLB
            // 
            this.missatgeLB.FormattingEnabled = true;
            this.missatgeLB.Location = new System.Drawing.Point(26, 126);
            this.missatgeLB.Name = "missatgeLB";
            this.missatgeLB.Size = new System.Drawing.Size(573, 95);
            this.missatgeLB.TabIndex = 8;
            // 
            // enviarBtn
            // 
            this.enviarBtn.Location = new System.Drawing.Point(524, 72);
            this.enviarBtn.Name = "enviarBtn";
            this.enviarBtn.Size = new System.Drawing.Size(75, 23);
            this.enviarBtn.TabIndex = 9;
            this.enviarBtn.Text = "Enviar";
            this.enviarBtn.UseVisualStyleBackColor = true;
            // 
            // missatgeTB
            // 
            this.missatgeTB.Location = new System.Drawing.Point(81, 101);
            this.missatgeTB.Name = "missatgeTB";
            this.missatgeTB.Size = new System.Drawing.Size(518, 20);
            this.missatgeTB.TabIndex = 10;
            this.missatgeTB.TextChanged += new System.EventHandler(this.textBox3_TextChanged);
            // 
            // tornLBL
            // 
            this.tornLBL.BackColor = System.Drawing.Color.LawnGreen;
            this.tornLBL.Font = new System.Drawing.Font("Microsoft Sans Serif", 22F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tornLBL.Location = new System.Drawing.Point(708, 31);
            this.tornLBL.Name = "tornLBL";
            this.tornLBL.Size = new System.Drawing.Size(299, 46);
            this.tornLBL.TabIndex = 11;
            this.tornLBL.Text = "És el teu TORN";
            this.tornLBL.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.tornLBL.Visible = false;
            // 
            // pasarBtn
            // 
            this.pasarBtn.Location = new System.Drawing.Point(714, 171);
            this.pasarBtn.Name = "pasarBtn";
            this.pasarBtn.Size = new System.Drawing.Size(149, 50);
            this.pasarBtn.TabIndex = 12;
            this.pasarBtn.Text = "Pasar torn";
            this.pasarBtn.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1103, 602);
            this.Controls.Add(this.pasarBtn);
            this.Controls.Add(this.tornLBL);
            this.Controls.Add(this.missatgeTB);
            this.Controls.Add(this.enviarBtn);
            this.Controls.Add(this.missatgeLB);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.conectarBtn);
            this.Controls.Add(this.servidorTB);
            this.Controls.Add(this.nomTB);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.labelTablero);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        public System.Windows.Forms.Label labelTablero;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        public System.Windows.Forms.Label label2;
        public System.Windows.Forms.TextBox nomTB;
        public System.Windows.Forms.TextBox servidorTB;
        public System.Windows.Forms.Button conectarBtn;
        public System.Windows.Forms.ListBox missatgeLB;
        public System.Windows.Forms.Button enviarBtn;
        public System.Windows.Forms.TextBox missatgeTB;
        public System.Windows.Forms.Label tornLBL;
        public System.Windows.Forms.Button pasarBtn;
    }
}

