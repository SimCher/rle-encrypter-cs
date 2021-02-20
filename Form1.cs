using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.IO;
using System.Security;

namespace Compressor
{
    public partial class Form1 : Form
    {
        string sr;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "PNG|*.png|Icon|*.ico|Bitmap|*.bmp";
            label3.Text = "Ожидание подтверждения";
            if(openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    sr = openFileDialog1.FileName;
                    label3.Text = $"Загружен файл: {sr}";
                    using(Stream stream = openFileDialog1.OpenFile())
                    {
                        label1.Text = $"{((double)stream.Length / 1024):N2} KB";
                        stream.Close();
                    }

                }
                catch(SecurityException ex)
                {
                    label3.Text = ($"Ошибка безопасности.\n\nСообщение ошибки: {ex.Message}\n\n" +
                        $"Детали:\n\n{ex.StackTrace}");

                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            saveFileDialog1.Filter = "RLE|*.rle|Binary File|*.bin";
            saveFileDialog1.Title = "Сохранение сжатого изображения";
            label4.Text = "Запуск окна сохранения...";
            saveFileDialog1.ShowDialog();
            string des = string.Empty;

            if(!string.IsNullOrEmpty(saveFileDialog1.FileName))
            {
                des = saveFileDialog1.FileName;
                label4.Text = $"Идёт процесс сжатия...";
            }
            try
            {
                Compress(sr, des);
            }
            catch(Exception ex)
            {
                label4.Text = ($"Ошибка безопасности.\n\nСообщение ошибки: {ex.Message}\n\n" +
                        $"Детали:\n\n{ex.StackTrace}");
            }
            
            label4.Text = $"Файл сжат и сохранён как {des}";
        }

        /// <summary>
        /// Сжимает файл с использованием RLE (Run-Length Encoding)-алгоритма
        /// </summary>
        /// <param name="source">Полный путь к исходному файлу</param>
        /// <param name="destination">Полный путь файла после сжатия</param>
        public void Compress(string source, string destination)
        {
            FileStream input = new FileStream(source, FileMode.Open, FileAccess.Read);
            FileStream output = new FileStream(destination, FileMode.Create, FileAccess.Write);

            byte current = (byte)input.ReadByte();
            output.WriteByte(current);
            
            int counter = 1;

            while (input.Position != input.Length)
            {
                byte next = (byte)input.ReadByte();
                
                if (next == current)
                {
                    if (counter++ > 250)
                    {
                        counter = Flush(output, current, counter);
                        current = (byte)input.ReadByte();
                        
                        output.WriteByte(current);
                        
                    }
                    continue;
                }
                counter = Flush(output, current, counter);
                output.WriteByte(next);
                
                current = next;
                counter = 1;
            }

            counter = Flush(output, current, counter);
            label2.Text = $"{((double)output.Length / 1024):N2} KB";
            input.Close();
            output.Close();
        }

        public void Decompress(string source, string destination)
        {
            FileStream input = new FileStream(source, FileMode.Open, FileAccess.Read);
            FileStream output = new FileStream(destination, FileMode.Create, FileAccess.Write);
            FileStream output1 = new FileStream("bytes.txt", FileMode.Create, FileAccess.Write);

            byte current = (byte)input.ReadByte();
            
            output.WriteByte(current);
            

            while (input.Position < input.Length)
            {
                byte next = (byte)input.ReadByte();
                
                if (next == current)
                {
                    byte counter = (byte)input.ReadByte();
                    for (int i = 2; i <= counter; i++)
                    {
                        output.WriteByte(current);
                        
                    }
                    current = (byte)input.ReadByte();
                    
                    output.WriteByte(current);
                    
                }
                else
                {
                    output.WriteByte(next);
                    
                    current = next;
                }
            }
            label5.Text = $"{((double)output.Length / 1024):N2} KB";
            input.Close();
            output.Close();
        }

        private int Flush(FileStream output, byte current, int counter)
        {
            if (counter > 1)
            {
                output.WriteByte(current);
                
                output.WriteByte((byte)counter);
            }
            return 1;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "RLE|*.rle|Binary File|*.bin";
            label6.Text = "Ожидание подтверждения";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    sr = openFileDialog1.FileName;
                    label6.Text = $"Загружен файл: {sr}";
                    using (Stream stream = openFileDialog1.OpenFile())
                    {
                        label5.Text = $"{((double)stream.Length / 1024):N2} KB";
                        stream.Close();
                    }

                }
                catch (SecurityException ex)
                {
                    label6.Text = ($"Ошибка безопасности.\n\nСообщение ошибки: {ex.Message}\n\n" +
                        $"Детали:\n\n{ex.StackTrace}");

                }
            }
            saveFileDialog1.Filter = "PNG|*.png|Icon|*.ico|Bitmap|*.bmp"; ;
            saveFileDialog1.Title = "Сохранение разжатого изображения";
            label6.Text = "Запуск окна сохранения...";
            saveFileDialog1.ShowDialog();
            string des = string.Empty;

            if (!string.IsNullOrEmpty(saveFileDialog1.FileName))
            {
                des = saveFileDialog1.FileName;
                label6.Text = $"Идёт процесс декомпрессии...";
            }
            try
            {
                Decompress(sr, des);
            }
            catch (Exception ex)
            {
                label6.Text = ($"Ошибка безопасности.\n\nСообщение ошибки: {ex.Message}\n\n" +
                        $"Детали:\n\n{ex.StackTrace}");
            }

            label6.Text = $"Файл сохранён как {des}";
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
