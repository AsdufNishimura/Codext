namespace Codext
{
    partial class FrmCodext
    {
        /// <summary>
        /// Variable del diseñador necesaria.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Limpiar los recursos que se estén usando.
        /// </summary>
        /// <param name="disposing">true si los recursos administrados se deben desechar; false en caso contrario.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código generado por el Diseñador de Windows Forms

        /// <summary>
        /// Método necesario para admitir el Diseñador. No se puede modificar
        /// el contenido de este método con el editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmCodext));
            this.txtCodigo = new System.Windows.Forms.RichTextBox();
            this.btnCargar = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.txtRenglones = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtSubcadena = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtEvaluacion = new System.Windows.Forms.TextBox();
            this.txtTokens = new System.Windows.Forms.RichTextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.btnComenzar = new System.Windows.Forms.Button();
            this.txtConsola = new System.Windows.Forms.RichTextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.dgvTSConstantesNumericas = new System.Windows.Forms.DataGridView();
            this.dgvTSOperadores = new System.Windows.Forms.DataGridView();
            this.dgvTSIdentificadores = new System.Windows.Forms.DataGridView();
            this.label7 = new System.Windows.Forms.Label();
            this.txtRenglonActual = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.btnSiguientePaso = new System.Windows.Forms.Button();
            this.txtGramatica = new System.Windows.Forms.RichTextBox();
            this.txtSintaxis = new System.Windows.Forms.RichTextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.txtSemantica = new System.Windows.Forms.RichTextBox();
            this.txtReglasSem = new System.Windows.Forms.RichTextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.txtTipos = new System.Windows.Forms.RichTextBox();
            this.label14 = new System.Windows.Forms.Label();
            this.txtPostFijo = new System.Windows.Forms.RichTextBox();
            this.dgvTripleta = new System.Windows.Forms.DataGridView();
            this.label15 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dgvTSConstantesNumericas)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvTSOperadores)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvTSIdentificadores)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvTripleta)).BeginInit();
            this.SuspendLayout();
            // 
            // txtCodigo
            // 
            this.txtCodigo.Location = new System.Drawing.Point(12, 12);
            this.txtCodigo.Name = "txtCodigo";
            this.txtCodigo.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.ForcedBoth;
            this.txtCodigo.Size = new System.Drawing.Size(297, 164);
            this.txtCodigo.TabIndex = 0;
            this.txtCodigo.Text = "";
            this.txtCodigo.WordWrap = false;
            // 
            // btnCargar
            // 
            this.btnCargar.BackColor = System.Drawing.Color.Silver;
            this.btnCargar.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(82)))), ((int)(((byte)(226)))));
            this.btnCargar.FlatAppearance.MouseOverBackColor = System.Drawing.Color.White;
            this.btnCargar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCargar.Location = new System.Drawing.Point(344, 10);
            this.btnCargar.Name = "btnCargar";
            this.btnCargar.Size = new System.Drawing.Size(100, 23);
            this.btnCargar.TabIndex = 1;
            this.btnCargar.Text = "Cargar";
            this.btnCargar.UseVisualStyleBackColor = false;
            this.btnCargar.Click += new System.EventHandler(this.btnCargar_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(341, 71);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(83, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "# de Renglones";
            // 
            // txtRenglones
            // 
            this.txtRenglones.BackColor = System.Drawing.SystemColors.Control;
            this.txtRenglones.Location = new System.Drawing.Point(344, 87);
            this.txtRenglones.Name = "txtRenglones";
            this.txtRenglones.ReadOnly = true;
            this.txtRenglones.Size = new System.Drawing.Size(100, 20);
            this.txtRenglones.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(26, 179);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(110, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Subcadena a Evaluar";
            // 
            // txtSubcadena
            // 
            this.txtSubcadena.Location = new System.Drawing.Point(12, 195);
            this.txtSubcadena.Name = "txtSubcadena";
            this.txtSubcadena.ReadOnly = true;
            this.txtSubcadena.Size = new System.Drawing.Size(297, 20);
            this.txtSubcadena.TabIndex = 5;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.ForeColor = System.Drawing.Color.White;
            this.label3.Location = new System.Drawing.Point(26, 232);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(103, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Evaluación o Token";
            // 
            // txtEvaluacion
            // 
            this.txtEvaluacion.Location = new System.Drawing.Point(12, 248);
            this.txtEvaluacion.Name = "txtEvaluacion";
            this.txtEvaluacion.ReadOnly = true;
            this.txtEvaluacion.Size = new System.Drawing.Size(297, 20);
            this.txtEvaluacion.TabIndex = 7;
            // 
            // txtTokens
            // 
            this.txtTokens.Location = new System.Drawing.Point(12, 300);
            this.txtTokens.Name = "txtTokens";
            this.txtTokens.ReadOnly = true;
            this.txtTokens.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.ForcedBoth;
            this.txtTokens.Size = new System.Drawing.Size(297, 167);
            this.txtTokens.TabIndex = 8;
            this.txtTokens.Text = "";
            this.txtTokens.WordWrap = false;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.ForeColor = System.Drawing.Color.White;
            this.label4.Location = new System.Drawing.Point(26, 284);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(97, 13);
            this.label4.TabIndex = 9;
            this.label4.Text = "Archivo de Tokens";
            // 
            // btnComenzar
            // 
            this.btnComenzar.BackColor = System.Drawing.Color.Silver;
            this.btnComenzar.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(82)))), ((int)(((byte)(226)))));
            this.btnComenzar.FlatAppearance.MouseOverBackColor = System.Drawing.Color.White;
            this.btnComenzar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnComenzar.Location = new System.Drawing.Point(344, 39);
            this.btnComenzar.Name = "btnComenzar";
            this.btnComenzar.Size = new System.Drawing.Size(100, 23);
            this.btnComenzar.TabIndex = 10;
            this.btnComenzar.Text = "Comenzar";
            this.btnComenzar.UseVisualStyleBackColor = false;
            this.btnComenzar.Click += new System.EventHandler(this.btnComenzar_Click);
            // 
            // txtConsola
            // 
            this.txtConsola.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.txtConsola.Location = new System.Drawing.Point(0, 633);
            this.txtConsola.Name = "txtConsola";
            this.txtConsola.ReadOnly = true;
            this.txtConsola.Size = new System.Drawing.Size(1292, 38);
            this.txtConsola.TabIndex = 11;
            this.txtConsola.Text = "";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.ForeColor = System.Drawing.Color.White;
            this.label5.Location = new System.Drawing.Point(324, 484);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(94, 26);
            this.label5.TabIndex = 12;
            this.label5.Text = "Tabla de símbolos\r\nConst. numéricas";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.ForeColor = System.Drawing.Color.White;
            this.label6.Location = new System.Drawing.Point(521, 484);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(94, 26);
            this.label6.TabIndex = 13;
            this.label6.Text = "Tabla de símbolos\r\nOperadores";
            // 
            // dgvTSConstantesNumericas
            // 
            this.dgvTSConstantesNumericas.AllowUserToAddRows = false;
            this.dgvTSConstantesNumericas.AllowUserToDeleteRows = false;
            this.dgvTSConstantesNumericas.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvTSConstantesNumericas.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.dgvTSConstantesNumericas.Location = new System.Drawing.Point(327, 513);
            this.dgvTSConstantesNumericas.Name = "dgvTSConstantesNumericas";
            this.dgvTSConstantesNumericas.Size = new System.Drawing.Size(181, 114);
            this.dgvTSConstantesNumericas.TabIndex = 14;
            // 
            // dgvTSOperadores
            // 
            this.dgvTSOperadores.AllowUserToAddRows = false;
            this.dgvTSOperadores.AllowUserToDeleteRows = false;
            this.dgvTSOperadores.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvTSOperadores.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.dgvTSOperadores.Location = new System.Drawing.Point(524, 513);
            this.dgvTSOperadores.Name = "dgvTSOperadores";
            this.dgvTSOperadores.Size = new System.Drawing.Size(132, 114);
            this.dgvTSOperadores.TabIndex = 15;
            // 
            // dgvTSIdentificadores
            // 
            this.dgvTSIdentificadores.AllowUserToAddRows = false;
            this.dgvTSIdentificadores.AllowUserToDeleteRows = false;
            this.dgvTSIdentificadores.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvTSIdentificadores.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.dgvTSIdentificadores.Location = new System.Drawing.Point(12, 513);
            this.dgvTSIdentificadores.Name = "dgvTSIdentificadores";
            this.dgvTSIdentificadores.Size = new System.Drawing.Size(297, 114);
            this.dgvTSIdentificadores.TabIndex = 16;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.ForeColor = System.Drawing.Color.White;
            this.label7.Location = new System.Drawing.Point(9, 484);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(94, 26);
            this.label7.TabIndex = 17;
            this.label7.Text = "Tabla de símbolos\r\nIdentificadores";
            // 
            // txtRenglonActual
            // 
            this.txtRenglonActual.BackColor = System.Drawing.SystemColors.Control;
            this.txtRenglonActual.Location = new System.Drawing.Point(344, 137);
            this.txtRenglonActual.Name = "txtRenglonActual";
            this.txtRenglonActual.ReadOnly = true;
            this.txtRenglonActual.Size = new System.Drawing.Size(100, 20);
            this.txtRenglonActual.TabIndex = 19;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.ForeColor = System.Drawing.Color.White;
            this.label8.Location = new System.Drawing.Point(341, 121);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(79, 13);
            this.label8.TabIndex = 18;
            this.label8.Text = "Renglón actual";
            // 
            // btnSiguientePaso
            // 
            this.btnSiguientePaso.BackColor = System.Drawing.Color.Silver;
            this.btnSiguientePaso.Enabled = false;
            this.btnSiguientePaso.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(82)))), ((int)(((byte)(226)))));
            this.btnSiguientePaso.FlatAppearance.MouseOverBackColor = System.Drawing.Color.White;
            this.btnSiguientePaso.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSiguientePaso.Location = new System.Drawing.Point(344, 179);
            this.btnSiguientePaso.Name = "btnSiguientePaso";
            this.btnSiguientePaso.Size = new System.Drawing.Size(100, 36);
            this.btnSiguientePaso.TabIndex = 20;
            this.btnSiguientePaso.Text = "Siguiente paso";
            this.btnSiguientePaso.UseVisualStyleBackColor = false;
            this.btnSiguientePaso.Click += new System.EventHandler(this.btnSiguientePaso_Click);
            // 
            // txtGramatica
            // 
            this.txtGramatica.Location = new System.Drawing.Point(657, 32);
            this.txtGramatica.Name = "txtGramatica";
            this.txtGramatica.ReadOnly = true;
            this.txtGramatica.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.ForcedBoth;
            this.txtGramatica.Size = new System.Drawing.Size(297, 160);
            this.txtGramatica.TabIndex = 21;
            this.txtGramatica.Text = "";
            this.txtGramatica.WordWrap = false;
            // 
            // txtSintaxis
            // 
            this.txtSintaxis.Location = new System.Drawing.Point(657, 218);
            this.txtSintaxis.Name = "txtSintaxis";
            this.txtSintaxis.ReadOnly = true;
            this.txtSintaxis.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.ForcedBoth;
            this.txtSintaxis.Size = new System.Drawing.Size(297, 167);
            this.txtSintaxis.TabIndex = 22;
            this.txtSintaxis.Text = "";
            this.txtSintaxis.WordWrap = false;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.ForeColor = System.Drawing.Color.White;
            this.label9.Location = new System.Drawing.Point(669, 202);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(43, 13);
            this.label9.TabIndex = 23;
            this.label9.Text = "Sintaxis";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.ForeColor = System.Drawing.Color.White;
            this.label10.Location = new System.Drawing.Point(669, 15);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(55, 13);
            this.label10.TabIndex = 24;
            this.label10.Text = "Gramatica";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.ForeColor = System.Drawing.Color.White;
            this.label11.Location = new System.Drawing.Point(994, 15);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(96, 13);
            this.label11.TabIndex = 28;
            this.label11.Text = "Reglas semánticas";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.ForeColor = System.Drawing.Color.White;
            this.label12.Location = new System.Drawing.Point(994, 202);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(57, 13);
            this.label12.TabIndex = 27;
            this.label12.Text = "Semántica";
            // 
            // txtSemantica
            // 
            this.txtSemantica.Location = new System.Drawing.Point(982, 218);
            this.txtSemantica.Name = "txtSemantica";
            this.txtSemantica.ReadOnly = true;
            this.txtSemantica.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.ForcedBoth;
            this.txtSemantica.Size = new System.Drawing.Size(297, 167);
            this.txtSemantica.TabIndex = 26;
            this.txtSemantica.Text = "";
            this.txtSemantica.WordWrap = false;
            // 
            // txtReglasSem
            // 
            this.txtReglasSem.Location = new System.Drawing.Point(982, 32);
            this.txtReglasSem.Name = "txtReglasSem";
            this.txtReglasSem.ReadOnly = true;
            this.txtReglasSem.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.ForcedBoth;
            this.txtReglasSem.Size = new System.Drawing.Size(297, 160);
            this.txtReglasSem.TabIndex = 25;
            this.txtReglasSem.Text = "";
            this.txtReglasSem.WordWrap = false;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.ForeColor = System.Drawing.Color.White;
            this.label13.Location = new System.Drawing.Point(341, 284);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(87, 13);
            this.label13.TabIndex = 30;
            this.label13.Text = "Archivo de Tipos";
            // 
            // txtTipos
            // 
            this.txtTipos.Location = new System.Drawing.Point(327, 300);
            this.txtTipos.Name = "txtTipos";
            this.txtTipos.ReadOnly = true;
            this.txtTipos.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.ForcedBoth;
            this.txtTipos.Size = new System.Drawing.Size(297, 167);
            this.txtTipos.TabIndex = 29;
            this.txtTipos.Text = "";
            this.txtTipos.WordWrap = false;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.ForeColor = System.Drawing.Color.White;
            this.label14.Location = new System.Drawing.Point(681, 428);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(149, 13);
            this.label14.TabIndex = 32;
            this.label14.Text = "Conversion a notación postfija";
            // 
            // txtPostFijo
            // 
            this.txtPostFijo.Location = new System.Drawing.Point(669, 444);
            this.txtPostFijo.Name = "txtPostFijo";
            this.txtPostFijo.ReadOnly = true;
            this.txtPostFijo.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.ForcedBoth;
            this.txtPostFijo.Size = new System.Drawing.Size(297, 167);
            this.txtPostFijo.TabIndex = 31;
            this.txtPostFijo.Text = "";
            this.txtPostFijo.WordWrap = false;
            // 
            // dgvTripleta
            // 
            this.dgvTripleta.AllowUserToAddRows = false;
            this.dgvTripleta.AllowUserToDeleteRows = false;
            this.dgvTripleta.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvTripleta.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.dgvTripleta.Location = new System.Drawing.Point(971, 444);
            this.dgvTripleta.Name = "dgvTripleta";
            this.dgvTripleta.Size = new System.Drawing.Size(308, 167);
            this.dgvTripleta.TabIndex = 33;
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.ForeColor = System.Drawing.Color.White;
            this.label15.Location = new System.Drawing.Point(985, 428);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(42, 13);
            this.label15.TabIndex = 34;
            this.label15.Text = "Tripleta";
            // 
            // FrmCodext
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(77)))), ((int)(((byte)(77)))), ((int)(((byte)(77)))));
            this.ClientSize = new System.Drawing.Size(1292, 671);
            this.Controls.Add(this.label15);
            this.Controls.Add(this.dgvTripleta);
            this.Controls.Add(this.label14);
            this.Controls.Add(this.txtPostFijo);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.txtTipos);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.txtSemantica);
            this.Controls.Add(this.txtReglasSem);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.txtSintaxis);
            this.Controls.Add(this.txtGramatica);
            this.Controls.Add(this.btnSiguientePaso);
            this.Controls.Add(this.txtRenglonActual);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.dgvTSIdentificadores);
            this.Controls.Add(this.dgvTSOperadores);
            this.Controls.Add(this.dgvTSConstantesNumericas);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.txtConsola);
            this.Controls.Add(this.btnComenzar);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.txtTokens);
            this.Controls.Add(this.txtEvaluacion);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtSubcadena);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtRenglones);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnCargar);
            this.Controls.Add(this.txtCodigo);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.Name = "FrmCodext";
            this.Text = "Codext - Compilador";
            this.Load += new System.EventHandler(this.FrmCodext_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frmCodext_KeyDown);
            ((System.ComponentModel.ISupportInitialize)(this.dgvTSConstantesNumericas)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvTSOperadores)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvTSIdentificadores)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvTripleta)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RichTextBox txtCodigo;
        private System.Windows.Forms.Button btnCargar;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtRenglones;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtSubcadena;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtEvaluacion;
        private System.Windows.Forms.RichTextBox txtTokens;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnComenzar;
        private System.Windows.Forms.RichTextBox txtConsola;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.DataGridView dgvTSConstantesNumericas;
        private System.Windows.Forms.DataGridView dgvTSOperadores;
        private System.Windows.Forms.DataGridView dgvTSIdentificadores;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txtRenglonActual;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Button btnSiguientePaso;
        private System.Windows.Forms.RichTextBox txtGramatica;
        private System.Windows.Forms.RichTextBox txtSintaxis;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.RichTextBox txtSemantica;
        private System.Windows.Forms.RichTextBox txtReglasSem;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.RichTextBox txtTipos;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.RichTextBox txtPostFijo;
        private System.Windows.Forms.DataGridView dgvTripleta;
        private System.Windows.Forms.Label label15;
    }
}

