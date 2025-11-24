using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static System.Net.Mime.MediaTypeNames;

namespace kursach2
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public string engl = "abcdefghijklmnopqrstuvwxyz";
        public string engc = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        public string rusl = "абвгдеёжзийклмнопрстуфхцчшщъыьэюя";
        public string rusc = "АБВГДЕЁЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯ";
        public static string EnglishLayout = "`~@#$%^&*()_+-=QWERTYUIOP{}|ASDFGHJKL:\"ZXCVBNM<>?qwertyuiop[]asdfghjkl;'zxcvbnm,./ ";
        public static string RussianLayout = "ёЁ\"№;%:?*()_+-=ЙЦУКЕНГШЩЗХЪ/ФЫВАПРОЛДЖЭЯЧСМИТЬБЮ,йцукенгшщзхъфывапролджэячсмитьбю. ";
        public string alphabet1 = "";
        public string alphabet2 = "";
        public string tin = "";
        public string tout = "";
        public int len = 0;
        public int lang = -1;

        public static (string cleanedStr, List<char> symb, List<int> pos) RemoveNonAlphabetChars(string inputStr, string alphabet1)
        {
            List<char> strinput = new List<char>(inputStr);
            List<char> symb = new List<char>();
            List<int> pos = new List<int>();
            for (int i = 0; i < strinput.Count; i++)
            {
                if (!alphabet1.Contains(strinput[i].ToString()))
                {
                    symb.Add(strinput[i]);
                    pos.Add(i);
                    
                }
            }
            for (int i = 0; i < strinput.Count; i++)
            {
                if (!alphabet1.Contains(strinput[i].ToString()))
                {
                    
                    strinput.RemoveAt(i);
                    i--;
                }
            }
            string cleanedStr = new string(strinput.ToArray());
            return (cleanedStr, symb, pos);
        }

        public static string RestoreNonAlphabetChars(string cleanedStr, List<char> symb, List<int> pos)
        {
            List<char> strinput = new List<char>(cleanedStr);
            for (int i = 0; i < symb.Count; i++)
            {
                if (pos[i] <= strinput.Count)
                {
                    strinput.Insert(pos[i], symb[i]);
                }
                else
                {
                    strinput.Add(symb[i]);
                }
            }
            return new string(strinput.ToArray());
        }

        public MainWindow()
        {
            InitializeComponent();
            textinput.Text = "";
            textoutput.Text = "";
            startevent();
        }

        private int checkalphabet(string text, string alphabet)
        {
            string lowerText1 = text.ToLower();
            string lowerText2 = alphabet.ToLower();

            var allowedChars = new System.Collections.Generic.HashSet<char>(lowerText2);
            bool allCharsContained = lowerText1.All(c => allowedChars.Contains(c));
            return allCharsContained ? 1 : 0;
        }

        public static string Encrypt(string plainText, int rails)
        {
            if (rails < 2) return plainText;
            if (string.IsNullOrEmpty(plainText)) return plainText;

            var railStrings = new List<StringBuilder>();
            for (int i = 0; i < rails; i++)
                railStrings.Add(new StringBuilder());

            int currentRail = 0;
            bool directionDown = true;

            foreach (char c in plainText)
            {
                railStrings[currentRail].Append(c);

                if (directionDown)
                {
                    if (currentRail == rails - 1)
                    {
                        directionDown = false;
                        currentRail--;
                    }
                    else
                    {
                        currentRail++;
                    }
                }
                else
                {
                    if (currentRail == 0)
                    {
                        directionDown = true;
                        currentRail++;
                    }
                    else
                    {
                        currentRail--;
                    }
                }
            }

            StringBuilder result = new StringBuilder();
            foreach (var rail in railStrings)
            {
                result.Append(rail.ToString());
            }

            return result.ToString();
        }

        public static string Decrypt(string cipherText, int rails)
        {
            if (rails < 2) return cipherText;
            if (string.IsNullOrEmpty(cipherText)) return cipherText;

            int[,] railPattern = new int[rails, cipherText.Length];
            int currentRail = 0;
            bool directionDown = true;

            for (int i = 0; i < cipherText.Length; i++)
            {
                railPattern[currentRail, i] = 1;

                if (directionDown)
                {
                    if (currentRail == rails - 1)
                    {
                        directionDown = false;
                        currentRail--;
                    }
                    else
                    {
                        currentRail++;
                    }
                }
                else
                {
                    if (currentRail == 0)
                    {
                        directionDown = true;
                        currentRail++;
                    }
                    else
                    {
                        currentRail--;
                    }
                }
            }

            char[] result = new char[cipherText.Length];
            int charIndex = 0;

            for (int rail = 0; rail < rails; rail++)
            {
                for (int i = 0; i < cipherText.Length; i++)
                {
                    if (railPattern[rail, i] == 1)
                    {
                        if (charIndex < cipherText.Length)
                        {
                            result[i] = cipherText[charIndex++];
                        }
                    }
                }
            }

            return new string(result);
        }

        private void atbash()
        {
            tin = textinput.Text;
            len = tin.Length;
            tout = "";

            for (int i = 0; i < tin.Length; i++)
            {
                char c = tin[i];
                int index1 = engl.IndexOf(c);
                int index2 = engc.IndexOf(c);
                int index3 = rusl.IndexOf(c);
                int index4 = rusc.IndexOf(c);
                if (index1 >= 0)
                {
                    char mirroredChar = engl[engl.Length - 1 - index1];
                    tout += mirroredChar;
                }
                else if (index2 >= 0)
                {
                    char mirroredChar = engc[engc.Length - 1 - index2];
                    tout += mirroredChar;
                }
                else if (index3 >= 0)
                {
                    char mirroredChar = rusl[rusl.Length - 1 - index3];
                    tout += mirroredChar;
                }
                else if (index4 >= 0)
                {
                    char mirroredChar = rusc[rusc.Length - 1 - index4];
                    tout += mirroredChar;
                }
                else
                {
                    
                    tout += c;
                }
            }
            textoutput.Text = tout;
        }

        private void cesar()
        {
            if (lang == -1)
            {
                alphabet1 = rusl;
                alphabet2 = rusc;
            }else if (lang == 1)
            {
                alphabet1 = engl;
                alphabet2 = engc;
            }
            tout = "";
            tin = textinput.Text;
            string inu = "";
            for (int i = 1; i < alphabet1.Length; i++)
            {
                inu = i.ToString();
                tout += "ROT " + inu + " " + ces(tin, i) + "\n";
            }
            textoutput.Text = "";
            textoutput.Text = tout;
        }

        private string ces(string textint, int number)
        {
            string textout = "";
            string alpha1 = alphabet1 + alphabet1;
            string alpha2 = alphabet2 + alphabet2;
            for (int i = 0; i < tin.Length; i++)
            {
                char c = tin[i];
                int index1 = alpha1.IndexOf(c);
                int index2 = alpha2.IndexOf(c);

                if (index1 >= 0)
                {

                    char buffedChar = alpha1[index1 + number];
                    textout += buffedChar;
                }
                else if (index2 >= 0)
                {

                    char buffedChar = alpha2[index2 + number];
                    textout += buffedChar;
                }
                else
                {

                    textout += c;
                }
            }
            return textout;
        }

        private void vigenere()
        {
            int flag = 1;
            if (lang == -1)
            {
                alphabet1 = rusl;
            }
            else if (lang == 1)
            {
                alphabet1 = engl;
            }
            if (checkalphabet(key.Text, alphabet1) == 0 || key.Text == "")
            {
                flag = 0;
                
            }
            //Console.WriteLine(alphabet1 + "\n" + key.Text + "\n" + flag);
            tin = textinput.Text;
            tout = "";
            textoutput.Text = "";
            if (flag == 1)
            {
                ProcessVigenere();
                string answer = "";
                for (int i = 0; i < tin.Length; i++)
                {
                    char c = textinput.Text[i];
                    string s = c.ToString();
                    if (char.IsUpper(c))
                    {
                        answer += char.ToUpper(tout[i]);
                    }
                    else
                    {
                        answer += tout[i];
                    }
                }
                tout = answer;
                textoutput.Text = tout;
            }
            else
            {
                textoutput.Text = "Ошибка ввода ключа: ключ должен быть словом, составленным из выбранного алфавита";
            }
                     
        }

        private void ProcessVigenere()
        {
            
            tin = tin.ToLower();
            StringBuilder result = new StringBuilder();
            int keyIndex = 0;
            int alphabetSize = alphabet1.Length;
            key.Text = key.Text.ToLower();
            for (int i = 0; i < tin.Length; i++)
            {
                char c = tin[i];
                int charIndex = alphabet1.IndexOf(c);

                if (charIndex >= 0) 
                {
                    int keyCharIndex = alphabet1.IndexOf(key.Text[keyIndex % (key.Text).Length]);
                    if (keyCharIndex < 0)
                    {
                        throw new ArgumentException("Ключ содержит символы не из алфавита");
                        
                    }
                        



                    int newIndex;
                    if (zaradio.IsChecked==true)
                        newIndex = (charIndex + keyCharIndex) % alphabetSize;
                    else
                        newIndex = (charIndex - keyCharIndex + alphabetSize) % alphabetSize;

                    result.Append(alphabet1[newIndex]);
                    keyIndex++;
                }
                else
                {
                    result.Append(c); // Оставляем символы не из алфавита без изменений
                }
            }

            tout = result.ToString();
        }

        private void clav()
        {
            if (int.TryParse(key.Text, out int number))
            {
                string alpha1l = "йцукенгшщзхъфывапролджэячсмитьбю";
                string alpha1c = "ЙЦУКЕНГШЩЗХЪФЫВАПРОЛДЖЭЯЧСМИТЬБЮ";
                string alpha2l = "qwertyuiopasdfghjklzxcvbnm";
                string alpha2c = "QWERTYUIOPASDFGHJKLZXCVBNM";

                tin = textinput.Text;
                len = tin.Length;
                tout = "";

                for (int i = 0; i < tin.Length; i++)
                {
                    int nu = int.Parse(key.Text);
                    if (zaradio.IsChecked == true)
                    {
                        char c = tin[i];
                        int index1 = alpha1l.IndexOf(c);
                        int index2 = alpha1c.IndexOf(c);
                        int index3 = alpha2l.IndexOf(c);
                        int index4 = alpha2c.IndexOf(c);
                        if (index1 >= 0)
                        {
                            string addedchar = cl(alpha1l, index1 + nu);
                            tout += addedchar;
                        }
                        else if (index2 >= 0)
                        {
                            string addedchar = cl(alpha1c, index2 + nu);
                            tout += addedchar;
                        }
                        else if (index3 >= 0)
                        {
                            string addedchar = cl(alpha2l, index3 + nu);
                            tout += addedchar;
                        }
                        else if (index4 >= 0)
                        {
                            string addedchar = cl(alpha2c, index4 + nu);
                            tout += addedchar;
                        }
                        else
                        {
                            tout += c;
                        }
                    }
                    else if (rasradio.IsChecked == true)
                    {
                        char c = tin[i];
                        int index1 = alpha1l.IndexOf(c);
                        int index2 = alpha1c.IndexOf(c);
                        int index3 = alpha2l.IndexOf(c);
                        int index4 = alpha2c.IndexOf(c);
                        if (index1 >= 0)
                        {
                            string addedchar = cl(alpha1l, index1 - nu);
                            tout += addedchar;
                        }
                        else if (index2 >= 0)
                        {
                            string addedchar = cl(alpha1c, index2 - nu);
                            tout += addedchar;
                        }
                        else if (index3 >= 0)
                        {
                            string addedchar = cl(alpha2l, index3 - nu);
                            tout += addedchar;
                        }
                        else if (index4 >= 0)
                        {
                            string addedchar = cl(alpha2c, index4 - nu);
                            tout += addedchar;
                        }
                        else
                        {
                            tout += c;
                        }
                    }

                }
                textoutput.Text = tout;
            }
            else
            {
                textoutput.Text = "Ошибка ввода ключа: ключ должен быть натуральным числом";
            }

        }

        private string cl(string alphabet, int num)
        {
            char ans;
            if (num >= alphabet.Length)
            {
                int number = num%alphabet.Length;
                ans = alphabet[number];
                string s = "" + ans;
                return s;
            }
            else if (num < 0)
            {
                while (num < 0)
                {
                    num += alphabet.Length;
                }
                int number = num % alphabet.Length;
                ans = alphabet[number];
                string s = "" + ans;
                return s;
            }
            else
            {
                ans = alphabet[num];
                string s = "" + ans;
                return s;
            }
            
        }

        private void trit()
        {
            
            if (lang == -1)
            {
                alphabet1 = rusl;
                alphabet2 = rusc;
            }
            else if (lang == 1)
            {
                alphabet1 = engl;
                alphabet2 = engc;
            }
            tout = "";
            tin = textinput.Text;
            int iden = 1;
            if (rasradio.IsChecked == true) { iden = -1; }

            for (int i = 0; i < tin.Length; i++)
            {
                char c = tin[i];
                int index1 = alphabet1.IndexOf(c);
                int index2 = alphabet2.IndexOf(c);

                if (index1 >= 0)
                {

                    //char buffedChar = alphabet1[(index1 + iden + i*iden)%alphabet1.Length];
                    
                    tout += cl(alphabet1, (index1 + iden + i * iden));
                }
                else if (index2 >= 0)
                {

                    //char buffedChar = alphabet2[(index2 + iden + i*iden)% alphabet2.Length];
                    tout += cl(alphabet2, (index2 + iden + i * iden));
                }
                else
                {

                    tout += c;
                }
            }
            textoutput.Text = "";
            textoutput.Text = tout;
        }

        private void rails()
        {
            if (int.TryParse(key.Text, out int number))
            {
                tin = textinput.Text;
                len = tin.Length;
                tout = "";
                int ino = int.Parse(key.Text);
                if (zaradio.IsChecked == true)
                {
                    tout = Encrypt(tin, ino);
                }
                else
                {
                    tout = Decrypt(tin, ino);
                }
                textoutput.Text = tout;
            }
            else
            {
                textoutput.Text = "Ошибка ввода ключа: ключ должен быть натуральным числом";
            }
            
        }

        private void will()
        {
            int flag = 1;
            tin = textinput.Text;
            tin = tin.ToLower();
            if (lang == -1)
            {
                alphabet1 = rusl;
            }
            else if (lang == 1)
            {
                alphabet1 = engl;
            }
            var (tincl, symb, pos) = RemoveNonAlphabetChars(tin, alphabet1);
            if (tincl == "") { flag = 0; }
            if (checkalphabet(tincl, alphabet1) == 0)
            {
                flag = 0;
            }
            if (zaradio.IsChecked == true && flag == 1)
            {
                tout = "";
                List<int> listin = new List<int>();
                for (int i = 0; i < tincl.Length; i++)
                {
                    char c = tincl[i];
                    listin.Add(alphabet1.IndexOf(c) + 1);
                }
                tout += alphabet1[listin[0] - 1];
                for (int i = 1; i < tincl.Length; i++)
                {
                    if (listin[i] != 0)
                    {
                        tout += cl(alphabet1, listin[i] - listin[i - 1]);

                    }
                    else
                    {
                        tout += tincl[i];
                    }
                }
                string answer = "";
                tout = RestoreNonAlphabetChars(tout, symb, pos);
                for (int i = 0; i < tin.Length; i++)
                {
                    char c = textinput.Text[i];
                    string s = c.ToString();
                    if (char.IsUpper(c))
                    {
                        answer += char.ToUpper(tout[i]);
                    }
                    else
                    {
                        answer += tout[i];
                    }
                }
                tout = answer;
                textoutput.Text = tout;


            }
            else if (rasradio.IsChecked == true && flag == 1)
            {
                tout = "";
                
                List<int> listin = new List<int>();
                for (int i = 0; i < tincl.Length; i++)
                {
                    char c = tincl[i];
                    listin.Add(alphabet1.IndexOf(c) + 1);
                }
                tout += alphabet1[listin[0] - 1];
                for (int i = 1; i < tincl.Length; i++)
                {
                    if (listin[i] != 0)
                    {
                        tout += cl(alphabet1, listin[i] + alphabet1.IndexOf(tout[i - 1]) - 1);

                    }
                    else
                    {
                        tout += tincl[i];
                    }
                }
                string answer = "";
                tout = RestoreNonAlphabetChars(tout, symb, pos);
                for (int i = 0; i < tin.Length; i++)
                {
                    char c = textinput.Text[i];
                    string s = c.ToString();
                    if (char.IsUpper(c))
                    {
                        answer += char.ToUpper(tout[i]);
                    }
                    else
                    {
                        answer += tout[i];
                    }
                }
                tout = answer;
                textoutput.Text = tout;
            }
            else
            {
                textoutput.Text = textinput.Text;
            }
            
        }

        private void rascl()
        {
            tin = textinput.Text;
            tout = "";
            bool isEnglish = tin.Count(c => EnglishLayout.Contains(c)) > tin.Count(c => RussianLayout.Contains(c));
            for (int i = 0;i < tin.Length;i++)
            {
                char d = tin[i];
                int index;
                if (isEnglish)
                {
                    index = EnglishLayout.IndexOf(d);
                    if (index >= 0)
                    {
                        tout += RussianLayout[index];
                    }
                    else
                    {
                        tout += d;
                    }

                }
                else
                {
                    index = RussianLayout.IndexOf(d);
                    if (index >= 0)
                    {
                        tout += EnglishLayout[index];
                    }
                    else
                    {
                        tout += d;
                    }
                }
            }
            textoutput.Text = tout;
        }

     
        private void count_Click(object sender, RoutedEventArgs e)
        {
            if (textinput.Text == "")
            {
                textoutput.Text = "";
                return;
            }

            if (LB.SelectedIndex == 0)
            {
                atbash();
            }else if (LB.SelectedIndex == 1)
            {
                cesar();
            }else if (LB.SelectedIndex == 2)
            {
                vigenere();
            }else if (LB.SelectedIndex == 3)
            {
                clav();
            }else if (LB.SelectedIndex == 4)
            {
                trit();
            }else if(LB.SelectedIndex == 5)
            {
                rails();
            }
            else if (LB.SelectedIndex == 6)
            {
                will();
            }
            else if (LB.SelectedIndex == 7)
            {
                rascl();
            }
        }

        private void langbut_Click(object sender, RoutedEventArgs e)
        {
            lang = lang * -1;
            if (lang == -1)
            {
                langbut.Content = "Кириллица";
            }else if (lang == 1)
            {
                langbut.Content = "Латиница";
            }
        }
        private void descript()
        {
            if (LB.SelectedIndex == 0)
            {
                bio.Text = "Простой моноалфавитный шифр подстановки, где первая буква алфавита заменяется на последнюю, вторая — на предпоследнюю и т. д. (например, A ↔ Z, B ↔ Y).";
            }else if (LB.SelectedIndex == 1)
            {
                bio.Text = "Сдвиговый шифр, где каждая буква в тексте заменяется на букву, стоящую на фиксированное число позиций дальше в алфавите (например, сдвиг на 3: A → D, B → E).";
            }else if (LB.SelectedIndex == 2)
            {
                bio.Text = "Шифр, использующий ключевое слово для сдвига букв открытого текста. Каждая буква ключа определяет свой сдвиг (например, ключ \"KEY\": первая буква сдвигается на K (10), вторая на E (4) и т. д.).";
            }else if (LB.SelectedIndex == 3)
            {
                bio.Text = "Шифр замены, где буквы заменяются на соседние по раскладке клавиатуры (например, Q → W, A → S). Может быть одно- или многосимвольным. Не предназначен для цифр и знаков препинания. ";
            }else if(LB.SelectedIndex == 4)
            {
                bio.Text = "Автоматический полиалфавитный шифр, где каждая буква сдвигается на значение, зависящее от её позиции в тексте (например, 1-я буква +1, 2-я +2 и т. д.).";
            }else if(LB.SelectedIndex == 5)
            {
                bio.Text = "Транспозиционный шифр, где текст записывается зигзагом по \"рельсам\" (строкам), а затем считывается по горизонтали. ";
            }
            else if (LB.SelectedIndex == 6)
            {
                bio.Text = "Полиалфавитный шифр, где каждая буква заменяется на букву, равную по номеру в алфавите расстоянию между ней и предыдущей буквой. \nАвторский шифр. ";
            }
            else if (LB.SelectedIndex == 7)
            {
                bio.Text = "Смените раскладку =)";
            }
        }
        private void startevent()
        {
            textoutput.Text = "";
            
            if (LB.SelectedIndex == 0)
            {
                labl.Visibility = Visibility.Hidden;
                langbut.Visibility = Visibility.Hidden;
                zaradio.Visibility = Visibility.Hidden;
                rasradio.Visibility = Visibility.Hidden;
                labk.Visibility = Visibility.Hidden;
                key.Visibility = Visibility.Hidden;
                
            }
            else if (LB.SelectedIndex == 1)
            {
                labl.Visibility = Visibility.Visible;
                langbut.Visibility = Visibility.Visible;
                zaradio.Visibility = Visibility.Hidden;
                rasradio.Visibility = Visibility.Hidden;
                labk.Visibility = Visibility.Hidden;
                key.Visibility = Visibility.Hidden;
                

            }
            else if (LB.SelectedIndex == 2)
            {
                labl.Visibility = Visibility.Visible;
                langbut.Visibility = Visibility.Visible;
                zaradio.Visibility = Visibility.Visible;
                rasradio.Visibility = Visibility.Visible;
                labk.Visibility = Visibility.Visible;
                key.Visibility = Visibility.Visible;
                

                key.Text = "";
            }
            else if (LB.SelectedIndex == 3)
            {
                labl.Visibility = Visibility.Hidden;
                langbut.Visibility = Visibility.Hidden;
                zaradio.Visibility = Visibility.Visible;
                rasradio.Visibility = Visibility.Visible;
                labk.Visibility = Visibility.Visible;
                key.Visibility = Visibility.Visible;
                

                key.Text = "1";
            }else if (LB.SelectedIndex == 4)
            {
                labl.Visibility = Visibility.Visible;
                langbut.Visibility = Visibility.Visible;
                zaradio.Visibility = Visibility.Visible;
                rasradio.Visibility = Visibility.Visible;
                labk.Visibility = Visibility.Hidden;
                key.Visibility = Visibility.Hidden;
                

            }
            else if(LB.SelectedIndex == 5)
            {
                labl.Visibility = Visibility.Hidden;
                langbut.Visibility = Visibility.Hidden;
                zaradio.Visibility = Visibility.Visible;
                rasradio.Visibility = Visibility.Visible;
                labk.Visibility = Visibility.Visible;
                key.Visibility = Visibility.Visible;
                key.Text = "1";
            }
            else if (LB.SelectedIndex == 6)
            {
                labl.Visibility = Visibility.Visible;
                langbut.Visibility = Visibility.Visible;
                zaradio.Visibility = Visibility.Visible;
                rasradio.Visibility = Visibility.Visible;
                labk.Visibility = Visibility.Hidden;
                key.Visibility = Visibility.Hidden;
                key.Text = "0";
            }
            else if (LB.SelectedIndex == 7)
            {
                labl.Visibility = Visibility.Hidden;
                langbut.Visibility = Visibility.Hidden;
                zaradio.Visibility = Visibility.Hidden;
                rasradio.Visibility = Visibility.Hidden;
                labk.Visibility = Visibility.Hidden;
                key.Visibility = Visibility.Hidden;
            }
        }
        private void LB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            startevent();
            descript();
        }

     

        private void pastebut(object sender, RoutedEventArgs e)
        {
            if (Clipboard.ContainsText())
            {
                textinput.Text = Clipboard.GetText();
            }
        }

        private void clearbut(object sender, RoutedEventArgs e)
        {
            textinput.Text = "";
        }

        private void copybut(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(textoutput.Text);

        }

        
    }
}
