using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Timers;
using System.Diagnostics;
using System.Linq;

namespace CheatGTA
{
    public partial class form1 : Form
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool ReadProcessMemory(IntPtr process, IntPtr baseAddress, byte[] Buffer, 
            int Size, out IntPtr BytesRead);


        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer,
            Int32 nSize, out IntPtr lpNumberOfBytesWritten);

        System.Timers.Timer gameCheck;
        IntPtr foundAdrress = IntPtr.Zero;
        Process gameProcess;

        private TextBox textBox1;

        public static long ReadInt64(IntPtr process, IntPtr baseAddress)
        {
            var buffer = new byte[8];
            IntPtr bytesRead;
            ReadProcessMemory(process, baseAddress, buffer, 8, out bytesRead);
            return BitConverter.ToInt64(buffer, 0);
        }

        public static long ReadInt32(IntPtr process, IntPtr baseAddress)
        {
            var buffer = new byte[8];
            IntPtr bytesRead;
            ReadProcessMemory(process, baseAddress, buffer, 4, out bytesRead);
            return BitConverter.ToInt64(buffer, 0);
        }

        private void OnTimerGameCheck(object source, ElapsedEventArgs e)
        {
            gameProcess = Process.GetProcessesByName("GTAIV").FirstOrDefault();

            if(gameProcess != null)
            {
                // Jogo iniciado! Procurando o ponteiro
                gameCheck.Stop();
                var gameModule = gameProcess.MainModule;
                var baseAddress = gameModule.BaseAddress.ToInt64() + 0x01050A88;
                var offsets = new[] { 0x14, 0x1C, 0x1C, 0x10 };

                var realAddress = GetRealAdrress(gameProcess.Handle, (IntPtr)baseAddress, offsets);
                foundAdrress = (IntPtr)realAddress;

                string Address = realAddress.ToString("X");

                if(Address.Length > 1)
                {
                    textBox1.Text = "Dinheiro alterado! :)";
                    System.Timers.Timer moneyTimer = new System.Timers.Timer();
                    moneyTimer.Elapsed += new ElapsedEventHandler(onTimerSetAmmo);
                    moneyTimer.Interval = 1;
                    moneyTimer.Enabled = true;
                }
                else
                {
                    textBox1.Text = "Endereco do Dinheiro nao encontrado :(";
                }
            }
            else
            {
                textBox1.Text = "Jogo nao iniciado :(";
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
        }

        private void onTimerSetAmmo(object sender, ElapsedEventArgs e)
        {
            var array = BitConverter.GetBytes(1000000);
            IntPtr bytesWritten;
            WriteProcessMemory(gameProcess.Handle, foundAdrress, array, (int)array.Length, out bytesWritten);
        }

        public static long GetRealAdrress(IntPtr process, IntPtr baseAddress, int[] offsets)
        {
            var address = baseAddress.ToInt64();
            foreach(var offset in offsets)
            {
                address = ReadInt32(process, (IntPtr)address) + offset;
            }
            return address;
        }


        /// <summary>
        /// Variável de designer necessária.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Limpar os recursos que estão sendo usados.
        /// </summary>
        /// <param name="disposing">true se for necessário descartar os recursos gerenciados; caso contrário, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código gerado pelo Windows Form Designer

        /// <summary>
        /// Método necessário para suporte ao Designer - não modifique 
        /// o conteúdo deste método com o editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(form1));
            this.gameCheck = new System.Timers.Timer();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.gameCheck)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // gameCheck
            // 
            this.gameCheck.Enabled = true;
            this.gameCheck.Interval = 1D;
            this.gameCheck.SynchronizingObject = this;
            this.gameCheck.Elapsed += new System.Timers.ElapsedEventHandler(this.OnTimerGameCheck);
            // 
            // textBox1
            // 
            this.textBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 26.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox1.ForeColor = System.Drawing.Color.Red;
            this.textBox1.Location = new System.Drawing.Point(12, 299);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(849, 122);
            this.textBox1.TabIndex = 0;
            this.textBox1.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.textBox1.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.pictureBox1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("pictureBox1.BackgroundImage")));
            this.pictureBox1.InitialImage = ((System.Drawing.Image)(resources.GetObject("pictureBox1.InitialImage")));
            this.pictureBox1.Location = new System.Drawing.Point(292, 12);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(326, 266);
            this.pictureBox1.TabIndex = 1;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.WaitOnLoad = true;
            // 
            // form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.ClientSize = new System.Drawing.Size(873, 433);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.textBox1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "form1";
            this.Text = "Cheat GTA IV";
            ((System.ComponentModel.ISupportInitialize)(this.gameCheck)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }



        #endregion

        private PictureBox pictureBox1;
    }
}

