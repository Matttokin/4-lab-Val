using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace _4_lab_Val
{
    public partial class Form1 : Form
    {
        static String folderFileWrite;
        static String folderFileRead;
        static int keyMod = 1; // требуется для определения позиции пикселя по ключу


        public Form1()
        {
            InitializeComponent();
        }


        static int getColor(int bite, int R, int G, int B) //расчет цвета пикселя на основе бита
        {
            //расчитываем по формулам https://habr.com/ru/post/115287/
            double lam = 0.2;
            if (bite == 1)
            {
                return (int)(B + lam * (0.3 * R + 0.59 * G + 0.11 * B));
            }
            else
            {
                return (int)(B - lam * (0.3 * R + 0.59 * G + 0.11 * B));
            }
        }

        static int getBite(Bitmap image, int x, int y) //рассчет бита на основе цвета пикселя
        {

            int meanBlue;
            int sumBlue = 0;

            
                sumBlue += image.GetPixel(x + 1, y).B;
                sumBlue += image.GetPixel(x - 1, y).B;
                sumBlue += image.GetPixel(x, y + 1).B;
                sumBlue += image.GetPixel(x, y - 1).B;

            meanBlue = sumBlue / 4;

            if (image.GetPixel(x, y).B > meanBlue)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }
        static int[,] string2Array(String stringIn) //получение массива байт в виде чисел
        {
            int[,] arr = new int[stringIn.Length, 8];

            for (int i = 0; i < stringIn.Length; i++)
            {
                String str = Convert.ToString(stringIn[i], 2).PadLeft(8, '0');
                for (int j = 0; j < 8; j++)
                {
                    arr[i, j] = Convert.ToInt32(str.Substring(j, 1));
                }
            }
            return arr;
        }
        int[] getArray(int[,] dataArr,int num) //получение подмассива
        {
            int[] arr = new int[8];
            for (int i = 0; i < 8; i++)
            {
                arr[i] = dataArr[num, i];
            }
            return arr;
        }

        private void button2_Click(object sender, EventArgs e) //выбор файл для записи
        {
            try
            {
                if (openFileDialog1.ShowDialog() == DialogResult.Cancel)
                    return;
                // получаем выбранный файл
                folderFileWrite = openFileDialog1.FileName;
                textBox7.Text = folderFileWrite;
                button4.Enabled = true;
            } catch
            {
                MessageBox.Show("Произошла ошибка");
            }
        }

        private void button4_Click(object sender, EventArgs e) //встраивание данных
        {
            try
            {
                string stringIn = textBox4.Text.ToString();
                Bitmap image = new Bitmap(folderFileWrite, true);

                int[,] dataArr = string2Array(stringIn);

                for (int j = 0; j < dataArr.Length / 8; j++)
                {
                    int[] arr = getArray(dataArr, j);

                    for (int i = 0; i < arr.Length; i++)
                    {
                        int[] arrayPixel = calcPos(j * 8 + i, image.Width); 
                        Color palet = image.GetPixel(arrayPixel[0], arrayPixel[1]);
                        Color bcolor = Color.FromArgb(palet.R, palet.G, getColor(arr[i], palet.R, palet.G, palet.B)); 
                        image.SetPixel(arrayPixel[0], arrayPixel[1], bcolor); 
                    }
                }

                MessageBox.Show("Выберите файл для сохранения");
                if (saveFileDialog1.ShowDialog() == DialogResult.Cancel)
                    return;
                image.Save(saveFileDialog1.FileName);
                image.Dispose();

                MessageBox.Show("Данные записаны");
            }
            catch
            {
                MessageBox.Show("Произошла ошибка при записи");
            }
        }

        private void button6_Click(object sender, EventArgs e) //чтение встроенных данных
        {
            try
            {
                int lenght = Convert.ToInt32(textBox5.Text.ToString());
                Bitmap image = new Bitmap(@folderFileRead, true);
                String stringOut = "";
                for (int j = 0; j < lenght; j++)
                {
                    int[] arr = new int[8]; 
                    for (int i = 0; i < arr.Length; i++)
                    {
                        int[] arrayPixel = calcPos(j * 8 + i, image.Width); 
                        arr[i] = getBite(image, arrayPixel[0], arrayPixel[1]); 
                    }
                    stringOut += (char)Convert.ToInt32(array2String(arr), 2);
                }

                MessageBox.Show(stringOut.ToString(), "Считанные данные");
                image.Dispose();
            } catch
            {
                MessageBox.Show("Произошла ошибка при чтении");
            }
        }
        string array2String(int[] arr)
        {
            String str = "";
            for (int i = 0; i < arr.Length; i++)
            {
                str += arr[i].ToString();
            }
            return str;
        }
        private void button1_Click(object sender, EventArgs e) //ввод ключа
        {
            if(textBox1.Text.Length == 16)
            {
                string tmpStr  = textBox1.Text.ToString();

                int valKey = 0;
                for(int i= 0; i < 16; i++)
                {
                    valKey += (int)tmpStr[i];
                }
                keyMod = valKey % 2;

                button2.Enabled = true;
                button5.Enabled = true;
                MessageBox.Show("Ключ сохранен");
            } else
            {
                MessageBox.Show("Неверная длина!");
            }
        }
        int[] calcPos(int num, int weight) //генератор позиций пикселей
        {
            weight -= 10;

            int tmpWeight = num * 6;
            int tmpHeight = 0;
            if(keyMod == 1)
            {
                tmpWeight += 1;
                tmpHeight += 2;
            }
            if (tmpWeight > weight)
            {
                tmpHeight = tmpWeight / weight;
                tmpWeight =  (tmpWeight - tmpHeight * weight) / 6;

                return new int[2] { tmpWeight * 6 + 5, tmpHeight * 6 + 5 };
            } else
            {
                return new int[2] { tmpWeight+ 5, tmpHeight + 5 };
            }
        }

        private void button5_Click(object sender, EventArgs e) //выбор файла для чтения
        {
            try { 
            if (openFileDialog1.ShowDialog() == DialogResult.Cancel)
                return;
            // получаем выбранный файл
            folderFileRead = openFileDialog1.FileName;
            textBox6.Text = folderFileRead;
            button6.Enabled = true;
            }
            catch
            {
                MessageBox.Show("Произошла ошибка");
            }
        }
    }
}

