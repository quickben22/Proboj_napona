using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Interop;
using System.ComponentModel;
using System.Threading;
using System.Globalization;
using System.Collections.ObjectModel;
using System.IO;
using System.Data;
using System.Security.Principal;
using System.Text.RegularExpressions;

namespace Citac2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        SQLiteDatabase db;
        bool spremit = false;
        List<string> popis = new List<string>();
        List<string> popis2 = new List<string>();
        string dir = "";
        List<TextBox> Tbox = new List<TextBox>();
        List<TextBox> Tbox2 = new List<TextBox>();
        //DataSet ds;
        public MainWindow()
        {
            InitializeComponent();
            RichT.AppendText("Breakdown voltage of transformer oil fulfills requirements according to IEC 60422 tab. 3. \"Mineral insulating oils after filling in new electrical equipment prior to energization\":");

        }

        private double pretvori(object b)
        {
            double rezultat;
            string a = b.ToString();
            bool isNum = double.TryParse(a, out rezultat);

            if (isNum)
                rezultat = double.Parse(a.Replace(",", "."), System.Globalization.NumberStyles.Any, CultureInfo.CreateSpecificCulture("en-us"));
            else
                rezultat = 0;

            return rezultat;
        }


        private void button1_Click(object sender, RoutedEventArgs e)
        {
            grid1.Width = 1307;
            Print(grid1);
            grid1.Width = double.NaN;
        }

        private void vadi()
        {
            string line;

            //AllocConsole(); komentar

            // Read the file and display it line by line.
            System.IO.StreamReader file =
               new System.IO.StreamReader(FileNameTextBox.Text);

            int i = 0;
            List<int> lista = new List<int>();

            //Console.WriteLine(dtDateTime);
            while ((line = file.ReadLine()) != null)
            {
                if (line.Length > 1)
                {
                    if (i != 0)
                    {
                        lista.Add(i);
                        string sve = "";
                        //string sve2 = "";
                        //Console.WriteLine(line);
                        string[] w = line.Split(',');
                        System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);
                        dtDateTime = dtDateTime.AddSeconds(pretvori(w[6])).ToLocalTime();
                        string datum = "";
                        string datum2 = "";
                        if (dtDateTime.Minute > 9)
                        {
                            if (dtDateTime.Hour > 9)
                            {
                                if (dtDateTime.Month < 10)
                                    datum = dtDateTime.Day + "/" + "0" + dtDateTime.Month + "/" + dtDateTime.Year;
                                else
                                    datum = dtDateTime.Day + "/" + dtDateTime.Month + "/" + dtDateTime.Year;
                                datum2 = dtDateTime.Hour + ":" + dtDateTime.Minute;
                            }
                            else
                            {
                                if (dtDateTime.Month < 10)
                                    datum = dtDateTime.Day + "/" + "0" + dtDateTime.Month + "/" + dtDateTime.Year;
                                else
                                    datum = dtDateTime.Day + "/" + dtDateTime.Month + "/" + dtDateTime.Year;
                                datum2 = "0" + dtDateTime.Hour + ":" + dtDateTime.Minute;
                            }

                        }
                        else
                        {
                            if (dtDateTime.Hour > 9)
                            {
                                if (dtDateTime.Month < 10)
                                    datum = dtDateTime.Day + "/" + "0" + dtDateTime.Month + "/" + dtDateTime.Year;
                                else
                                    datum = dtDateTime.Day + "/" + dtDateTime.Month + "/" + dtDateTime.Year;
                                datum2 = dtDateTime.Hour + ":" + "0" + dtDateTime.Minute;

                            }
                            else
                            {
                                if (dtDateTime.Month < 10)
                                    datum = dtDateTime.Day + "/" + "0" + dtDateTime.Month + "/" + dtDateTime.Year;
                                else
                                    datum = dtDateTime.Day + "/" + dtDateTime.Month + "/" + dtDateTime.Year;
                                datum2 = "0" + dtDateTime.Hour + ":" + "0" + dtDateTime.Minute;
                            }
                        }
                        if (dtDateTime.Day < 10)
                            datum = "0" + datum;

                        if (textBox3.Text.Length > 0)
                            datum = textBox3.Text;

                        List<string> lines = new List<string> 
                            {
                                //"         KONČAR - DISTRIBUTIVNI I SPECIJALNI TRANSFORMATORI d.d.",
                                //"         Address:  Josipa Mokrovića 8, P.O. Box 100, HR-10090 Zagreb, Croatia",
                                //"         Telephone: (+385 1)  3783 777,  Fax: (+ 385 1)  3794 051",
                                //"         E-mail:  info@koncar-dst.hr,  Internet:  www.koncar-dst.hr",
                                "",
                                "",
                                "Megger",
                                "OTS100AF",
                               w[2].Replace("\"",""),
                                ""
                            };
                        List<string> lines2 = new List<string>();
                        lines.Add("F/W Version:        " + w[3]);
                        lines.Add("Std. Lib. Version:  " + w[4]);
                        lines.Add("");
                        string idd = w[5].Replace("\"", "");
                        lines.Add("Test Id:            " + idd);
                        if (i == comboBox1.SelectedIndex + 1)
                            T1.Text = idd;
                        lines.Add("");
                        lines.Add("Date:   " + datum);
                        lines.Add("Time:   " + datum2);
                        lines.Add("");
                        lines.Add(w[8].Replace("\"", ""));
                        lines.Add("");
                        lines.Add("Oil Type:");
                        lines.Add(vrsta_ulja(w[13]));
                        lines.Add("");
                        lines.Add("Electrodes: " + vrsta_elek(w[9]));
                        lines.Add("Elec. Gap:  " + pretvori(w[10]).ToString("F2").Replace(",", ".") + w[11].Replace("\"", ""));
                        lines.Add("Stirrer:    " + vrsta_stir(w[12]));
                        lines.Add("Test Freq:  " + "61.8Hz");
                        lines.Add("Max. Volt:  " + (pretvori(w[20]) / 10).ToString("F1").Replace(",", ".") + "kV");
                        lines.Add("dV/dt Rate: " + (pretvori(w[18]) / 1000).ToString("F1").Replace(",", ".") + "kV/s");

                        lines2.Add("Results:  ");
                        lines2.Add("Oil Temp:         " + w[16] + "°" + w[17].Replace("\"", ""));
                        lines2.Add("");
                        for (int j = 0; j < Convert.ToInt32(w[19]); j++)
                        {
                            lines2.Add("Test" + (j + 1) + ":            " + w[21 + j] + "kV");
                        }
                        lines2.Add("Avg. Voltage:     ");
                        lines2.Add("");
                        List<string> lines3 = new List<string>();
                        lines3.Add("                  " + w[33] + "kV");
                        lines3.Add("");
                        lines3.Add("Dispersion s/x:   " + w[34]);
                        lines3.Add("Std. Deviation:   " + w[35] + "kV");
                        lines3.Add("");
                        lines3.Add("");

                        sve = string.Join("\n", lines.ToArray());
                        //sve2 = string.Join("\n", lines2.ToArray());

                        if (i == Convert.ToInt32(comboBox1.Text))
                            ubaci(sve, lines2, lines3);
                    }
                    i++;

                }
            }
            comboBox1.ItemsSource = new ObservableCollection<int>(lista);
            //flowDocument.Blocks.Add(b);
            file.Close();
            file.Dispose();
        }


        private void ubaci(string sve, List<string> lines2, List<string> lines3)
        {
            Ubacit.Text = sve;
            Ubacit2.Inlines.Add((new string('\n', Math.Max(0, 24 - lines2.Count)) + string.Join("\n", lines2.ToArray())));
            Bold b = new Bold(new Run(lines3[0]));
            b.FontSize = 24;
            Ubacit2.Inlines.Add(b);
            lines3[0] = "";
            Ubacit2.Inlines.Add((string.Join("\n", lines3.ToArray())));
        }

        private void Print(Visual v)
        {

            button1.Visibility = Visibility.Hidden;
            button2.Visibility = Visibility.Hidden;
            button3.Visibility = Visibility.Hidden;
            button4.Visibility = Visibility.Hidden;
            button5.Visibility = Visibility.Hidden;
            button4a.Visibility = Visibility.Hidden;
            button5a.Visibility = Visibility.Hidden;
            textBox4.Visibility = Visibility.Hidden;
            comboBox2.Visibility = Visibility.Hidden;
            comboBox2a.Visibility = Visibility.Hidden;
            checkBox1.Visibility = Visibility.Hidden;
            comboBox1.Visibility = Visibility.Hidden;
            FileNameTextBox.Visibility = Visibility.Hidden;
            textBlock21.Visibility = Visibility.Hidden;
            textBox3.Visibility = Visibility.Hidden;
            C1.Visibility = Visibility.Hidden;
            
            System.Windows.FrameworkElement e = v as System.Windows.FrameworkElement;
            if (e == null)
                return;

            PrintDialog pd = new PrintDialog();
            if (pd.ShowDialog() == true)
            {
                //store original scale
                Transform originalScale = e.LayoutTransform;
                //get selected printer capabilities
                System.Printing.PrintCapabilities capabilities = pd.PrintQueue.GetPrintCapabilities(pd.PrintTicket);

                //get scale of the print wrt to screen of WPF visual
                double scale = Math.Min(capabilities.PageImageableArea.ExtentWidth / e.ActualWidth, capabilities.PageImageableArea.ExtentHeight /
                               e.ActualHeight);

                //Transform the Visual to scale
                e.LayoutTransform = new ScaleTransform(scale, scale);

                //get the size of the printer page
                System.Windows.Size sz = new System.Windows.Size(capabilities.PageImageableArea.ExtentWidth, capabilities.PageImageableArea.ExtentHeight);

                //update the layout of the visual to the printer page size.
                e.Measure(sz);
                e.Arrange(new System.Windows.Rect(new System.Windows.Point(capabilities.PageImageableArea.OriginWidth, capabilities.PageImageableArea.OriginHeight), sz));

                //now print the visual to printer to fit on the one page.

                pd.PrintVisual(v, "Izv_" + MakeValidFileName(T1.Text));

                //apply the original transform.
                e.LayoutTransform = originalScale;
            }



            button1.Visibility = Visibility.Visible;
            button2.Visibility = Visibility.Visible;
            button3.Visibility = Visibility.Visible;
            button4.Visibility = Visibility.Visible;
            button5.Visibility = Visibility.Visible;
            button4a.Visibility = Visibility.Visible;
            button5a.Visibility = Visibility.Visible;
            comboBox1.Visibility = Visibility.Visible;
            textBox4.Visibility = Visibility.Visible;
            comboBox2.Visibility = Visibility.Visible;
            comboBox2a.Visibility = Visibility.Visible;
            checkBox1.Visibility = Visibility.Visible;
            FileNameTextBox.Visibility = Visibility.Visible;
            textBlock21.Visibility = Visibility.Visible;
            textBox3.Visibility = Visibility.Visible;
            C1.Visibility = Visibility.Visible;

            //Close();
        }

        public string MakeValidFileName(string name)
        {
            var builder = new StringBuilder();
            var invalid = System.IO.Path.GetInvalidFileNameChars();
            foreach (var cur in name)
            {
                if (!invalid.Contains(cur))
                {
                    builder.Append(cur);
                }
                else
                    builder.Append("_");
            }
            return builder.ToString();
        }

        private string vrsta_ulja(string s)
        {
            if (s == "1") return "Mineral/Ester";
            if (s == "2") return "Silicon";
            return s;
        }
        private string vrsta_stir(string s)
        {
            if (s == "4") return "None";
            if (s == "1") return "Impeller";
            return s;
        }
        private string vrsta_elek(string s)
        {
            if (s == "1") return "Mushroom";
            return s;
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            // Set filter for file extension and default file extension
            dlg.DefaultExt = ".csv";
            dlg.Filter = "CSV documents (.csv)|*.csv";

            dlg.RestoreDirectory = true;
            // Display OpenFileDialog by calling ShowDialog method
            Nullable<bool> result = dlg.ShowDialog();

            // Get the selected file name and display in a TextBox
            if (result == true)
            {
                //Open document
                string filename = dlg.FileName;
                FileNameTextBox.Text = filename;
                try
                {
                    vadi();
                }
                catch
                {
                    MessageBox.Show("Došlo je do greške!");
                }
            }



        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //Console.WriteLine(this.FontFamily);
            //Console.WriteLine(this.FontSize);
            //Console.WriteLine(this.FontStretch);
            //Console.WriteLine(this.FontStyle);
            //Console.WriteLine(this.FontWeight);
            try
            {
                db = new SQLiteDatabase();
                DataTable recipe;
                String query = "select Projekt,Jezik from Tablica;";
                recipe = db.GetDataTable(query);
                // The results can be directly applied to a DataGridView control
                //recipeDataGrid.DataSource = recipe;

                // Or looped through for some other reason
                foreach (DataRow r in recipe.Rows)
                {
                    //MessageBox.Show(r["Projekt"].ToString());

                    string zzz = r["Jezik"].ToString();
                    if (r["Jezik"].ToString() == "1")
                        popis2.Add(r["Projekt"].ToString());
                    else
                        popis.Add(r["Projekt"].ToString());

                }


            }
            catch (Exception fail)
            {
                String error = "The following error has occurred:\n\n";
                error += fail.Message.ToString() + "\n\n";
                MessageBox.Show(error);
                this.Close();
            }




            dir = Directory.GetCurrentDirectory();
            //ds = new DataSet();
            //ds.ReadXml(dir + @"\" + "bazica.xml");

            comboBox1.ItemsSource = new ObservableCollection<int>(new int[] { 1 });
            comboBox1.Text = "1";


            System.DateTime dtDateTime = DateTime.Now;

            string datum = "";
            string datum2 = "";
            if (dtDateTime.Minute > 9)
            {
                if (dtDateTime.Hour > 9)
                {
                    if (dtDateTime.Month < 10)
                        datum = dtDateTime.Day + "." + "0" + dtDateTime.Month + "." + dtDateTime.Year;
                    else
                        datum = dtDateTime.Day + "." + dtDateTime.Month + "." + dtDateTime.Year;
                    datum2 = dtDateTime.Hour + ":" + dtDateTime.Minute;
                }
                else
                {
                    if (dtDateTime.Month < 10)
                        datum = dtDateTime.Day + "." + "0" + dtDateTime.Month + "." + dtDateTime.Year;
                    else
                        datum = dtDateTime.Day + "." + dtDateTime.Month + "." + dtDateTime.Year;
                    datum2 = "0" + dtDateTime.Hour + ":" + dtDateTime.Minute;
                }

            }
            else
            {
                if (dtDateTime.Hour > 9)
                {
                    if (dtDateTime.Month < 10)
                        datum = dtDateTime.Day + "." + "0" + dtDateTime.Month + "." + dtDateTime.Year;
                    else
                        datum = dtDateTime.Day + "." + dtDateTime.Month + "." + dtDateTime.Year;
                    datum2 = dtDateTime.Hour + ":" + "0" + dtDateTime.Minute;

                }
                else
                {
                    if (dtDateTime.Month < 10)
                        datum = dtDateTime.Day + "." + "0" + dtDateTime.Month + "." + dtDateTime.Year;
                    else
                        datum = dtDateTime.Day + "." + dtDateTime.Month + "." + dtDateTime.Year;
                    datum2 = "0" + dtDateTime.Hour + ":" + "0" + dtDateTime.Minute;
                }
            }
            if (dtDateTime.Day < 10)
                datum = "0" + datum;



            T14.Text = datum + ".";


            comboBox2.ItemsSource = new ObservableCollection<string>(popis);
            comboBox2a.ItemsSource = new ObservableCollection<string>(popis2);

            Tbox = new List<TextBox> {T1, T2, T3, T4, T5, T6, T7, T8,
              T9, T10, T11, T12, T13, T14, T15, T16,T17,T18,T19,T20,T21,T22, TT1, TT2, TT3, TT4, TT5, TT6,TT6a, TT7, TT8,
              TT9, TT10, TT11, TT13, TT14, TT15, TT16,TT17,TT18,TT19,TT20,TT21,
              TT22,TT23,TT24,TT25,TT26};

        }

        private void textBox1_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (FileNameTextBox.Text.Length > 1)
            {
                try
                {
                    vadi();
                    spremit = true;
                }
                catch
                {
                    MessageBox.Show("Došlo je do greške!");
                }
            }
        }

        private void textBox2_TextChanged(object sender, TextChangedEventArgs e)
        {

        }


        private void Spremi_click(object sender, RoutedEventArgs e)
        {
            if (textBox4.Text.Length > 0)
            {
                int b = -1;
                if (checkBox1.IsChecked.Value) b = postoji2(textBox4.Text);
                else b = postoji(textBox4.Text);
                if (b != -1)
                {
                    spremi2b(textBox4.Text);
                }
                else
                {
                    spremib(textBox4.Text);
                }
            }
        }
        /*
        private void spremi(string s)
        {
                    DataRow dr = ds.Tables["Tablica"].NewRow();
                    foreach (TextBox t in Tbox)
                    {
                        dr[t.Name.ToString()] = t.Text;
                    }
                    dr["Projekt"] = s;
                    dr["Date"] = DateTime.Now;
                    dr["Korisnik"] = WindowsIdentity.GetCurrent().Name.ToString();
                    dr["Ubacit"] = Ubacit.Text;
                    dr["Ubacit2"] = Ubacit2.Text;
                    TextRange txt = new TextRange(RichT.Document.ContentStart, RichT.Document.ContentEnd);
                    dr["RichT"] = txt.Text;
                    ds.Tables["Tablica"].Rows.Add(dr);
                    ds.WriteXml(dir + @"\" + "bazica.xml");
                    popis.Add(s);
                    comboBox2.ItemsSource = new ObservableCollection<string>(popis); 
        }

        private void spremi2(int i)
        {
           
            foreach (TextBox t in Tbox)
            {
                ds.Tables["Tablica"].Rows[i][t.Name.ToString()] = t.Text;
            }
            ds.Tables["Tablica"].Rows[i]["Date"] = DateTime.Now;
            ds.Tables["Tablica"].Rows[i]["Korisnik"] = WindowsIdentity.GetCurrent().Name.ToString();
            ds.Tables["Tablica"].Rows[i]["Ubacit"] = Ubacit.Text;
            ds.Tables["Tablica"].Rows[i]["Ubacit2"] = Ubacit2.Text;
            TextRange txt = new TextRange(RichT.Document.ContentStart, RichT.Document.ContentEnd);
            ds.Tables["Tablica"].Rows[i]["RichT"] = txt.Text;
            ds.WriteXml(dir + @"\" + "bazica.xml");
   
        }
          */

        private void spremi2b(string i)
        {



            Dictionary<String, String> data = new Dictionary<String, String>();
            //DataTable rows;
            data.Add("Date", DateTime.Now.ToString());
            data.Add("Korisnik", WindowsIdentity.GetCurrent().Name.ToString());
            data.Add("Ubacit", Ubacit.Text.Replace("'", "''"));
            data.Add("Ubacit2", Ubacit2.Text.Replace("'", "''"));
            TextRange txt = new TextRange(RichT.Document.ContentStart, RichT.Document.ContentEnd);
            data.Add("RichT", txt.Text.Replace("'", "''"));
            if(checkBox1.IsChecked.Value)
            data.Add("Jezik", "1");
            else
            data.Add("Jezik", "0");
            if (C1.IsChecked == true) data.Add("C1", "1");
            else data.Add("C1", "0");

            foreach (TextBox t in Tbox)
            {
                data.Add(t.Name.ToString(), t.Text.Replace("'", "''"));
            }

            try
            {
                db.Update("Tablica", data, String.Format("Projekt = {0}", "\"" + i + "\""));
            }
            catch (Exception crap)
            {
                MessageBox.Show(crap.Message);
            }







        }

        private void spremib(string s)
        {
            //SQLiteDatabase db = new SQLiteDatabase();
            Dictionary<String, String> data = new Dictionary<String, String>();
            foreach (TextBox t in Tbox)
            {
                data.Add(t.Name.ToString(), t.Text.Replace("'", "''"));
            }
            data.Add("Projekt", s);
            data.Add("Date", DateTime.Now.ToString());
            data.Add("Korisnik", WindowsIdentity.GetCurrent().Name.ToString());
            data.Add("Ubacit", Ubacit.Text.Replace("'", "''"));
            data.Add("Ubacit2", Ubacit2.Text.Replace("'", "''"));
            TextRange txt = new TextRange(RichT.Document.ContentStart, RichT.Document.ContentEnd);
            data.Add("RichT", txt.Text.Replace("'", "''"));
            if (checkBox1.IsChecked.Value)
                data.Add("Jezik", "1");
            else
                data.Add("Jezik", "0");
            if (C1.IsChecked == true) data.Add("C1", "1");
            else data.Add("C1", "0");

            try
            {
                db.Insert("Tablica", data);
                if (checkBox1.IsChecked.Value)
                {
                    popis2.Add(s);
                    comboBox2a.ItemsSource = new ObservableCollection<string>(popis2);
                    if (popis.Contains(s)) { popis.Remove(s); comboBox2.ItemsSource = new ObservableCollection<string>(popis); }
                }
                else
                {
                    popis.Add(s);
                    comboBox2.ItemsSource = new ObservableCollection<string>(popis);
                    if (popis2.Contains(s)) { popis2.Remove(s); comboBox2a.ItemsSource = new ObservableCollection<string>(popis2); }
                }
            }
            catch (Exception crap)
            {
                MessageBox.Show(crap.Message);
            }



        }

        private int postoji(string a)
        {
            int i = 0;
            foreach (string s in popis)
            {
                if (s == a)
                {

                    return i;

                }
                i++;

            }

            return -1;
        }

        private int postoji2(string a)
        {
            int i = 0;
            foreach (string s in popis2)
            {
                if (s == a)
                {

                    return i;

                }
                i++;

            }

            return -1;
        }



        private void Otvori_Click(object sender, RoutedEventArgs e)
        {
            ComboBox cbx = comboBox2;
            if (button4a.Name == ((Button)sender).Name)
                cbx = comboBox2a;

            if (cbx.Text != "id" && cbx.SelectedIndex != -1)
            {

                try
                {

                    DataTable recipe;
                    String query = "select * from Tablica WHERE (Projekt = \"" + cbx.Text + "\")";
                    recipe = db.GetDataTable(query);
                    // The results can be directly applied to a DataGridView control
                    //recipeDataGrid.DataSource = recipe;

                    // Or looped through for some other reason
                    foreach (DataRow r in recipe.Rows)
                    {


                        foreach (TextBox t in Tbox)
                        {
                            t.Text = r[t.Name.ToString()].ToString();
                        }
                        if (r["Jezik"].ToString() == "1")
                            checkBox1.IsChecked = true;
                        else
                            checkBox1.IsChecked = false;
                        textBox4.Text = r["Projekt"].ToString();
                        Ubacit.Text = r["Ubacit"].ToString();
                        string ubac = r["Ubacit2"].ToString();
                        if (ubac.Contains("Avg. Voltage:"))
                        {
                            Ubacit2.Inlines.Add(ubac.Substring(0, ubac.IndexOf("Avg. Voltage:") + "Avg. Voltage:".Length));
                            Bold b = new Bold(new Run(ubac.Substring(ubac.IndexOf("Avg. Voltage:") + "Avg. Voltage:".Length, ubac.IndexOf("Dispersion") - ubac.IndexOf("Avg. Voltage:") - "Avg. Voltage:".Length)));
                            b.FontSize = 24;
                            Ubacit2.Inlines.Add(b);
                            //lines3[0] = "";
                            Ubacit2.Inlines.Add(ubac.Substring(ubac.IndexOf("Dispersion")));
                        }
                        else
                            Ubacit2.Text = r["Ubacit2"].ToString();
                        RichT.Document.Blocks.Clear();
                        RichT.AppendText(r["RichT"].ToString());
                        if (r["C1"].ToString() == "1") C1.IsChecked = true;
                        else C1.IsChecked = false;
                        checki();
                        spremit = false;


                    }


                }
                catch (Exception fail)
                {
                    String error = "The following error has occurred:\n\n";
                    error += fail.Message.ToString() + "\n\n";
                    MessageBox.Show(error);
                    this.Close();
                }


            }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (spremit)
            {
                string messageBoxText = "Želite li spremiti promjene";
                DateTime d = new DateTime();
                d = DateTime.Now;
                string datumcic = d.Day + "_" + d.Month + "_" + d.Year + "_" + d.Hour + "_" + d.Minute + "_" + d.Second;

                if (textBox4.Text.Length > 0)
                {
                    messageBoxText += " na projektu:" + textBox4.Text + "?";
                }
                else
                {
                    messageBoxText += " na projektu:" + datumcic + "?";
                }
                string caption = "Probojni Napon";
                MessageBoxButton button = MessageBoxButton.YesNoCancel;
                MessageBoxImage icon = MessageBoxImage.Warning;
                MessageBoxResult result = MessageBox.Show(messageBoxText, caption, button, icon);

                // Process message box results
                switch (result)
                {
                    case MessageBoxResult.Yes:
                        {
                            try
                            {
                                if (textBox4.Text.Length > 0)
                                {

                                    int b = -1;
                                    if (checkBox1.IsChecked.Value) b = postoji2(textBox4.Text);
                                    else b = postoji(textBox4.Text);

                                    if (b != -1)
                                    {
                                        spremi2b(textBox4.Text);
                                    }
                                    else
                                    {
                                        spremib(textBox4.Text);
                                    }
                                }
                                else
                                {

                                    spremib(datumcic);
                                }
                            }
                            catch
                            {
                                MessageBox.Show("Neuspjeh");
                            }
                            spremit = false;
                            break;
                        }
                    case MessageBoxResult.No:
                        {
                            break;
                        }

                    case MessageBoxResult.Cancel:
                        e.Cancel = true;
                        break;
                }
            }
        }

        private void Obrisi_Click(object sender, RoutedEventArgs e)
        {
            ComboBox cbx = comboBox2;
            if (button5a.Name == ((Button)sender).Name)
                cbx = comboBox2a;

            if (cbx.Text.Length > 0 && cbx.SelectedIndex != -1 && cbx.Text != "id")
            {
                string messageBoxText = "Sigurno želite izbrisati projekt:" + cbx.Text + "?";

                string caption = "Probojni Napon";
                MessageBoxButton button = MessageBoxButton.YesNo;
                MessageBoxImage icon = MessageBoxImage.Warning;
                MessageBoxResult result = MessageBox.Show(messageBoxText, caption, button, icon);

                // Process message box results
                switch (result)
                {
                    case MessageBoxResult.Yes:
                        {
                            try
                            {
                                if (button5a.Name == ((Button)sender).Name)
                                {
                                    int b = postoji2(cbx.Text);
                                    brisi2(b);
                                }
                                else
                                {
                                    int b = postoji(cbx.Text);
                                    brisi(b);
                                }
                            }
                            catch
                            {
                                MessageBox.Show("Neuspjeh");
                            }
                            break;
                        }
                    case MessageBoxResult.No:
                        {
                            break;
                        }
                }

            }
        }

        private void brisi(int a)
        {
            //ds.Tables["Tablica"].Rows[a].Delete();
            //ds.WriteXml(dir + @"\" + "bazica.xml");

            db.Delete("Tablica", String.Format("Projekt = {0}", "\"" + popis[a] + "\""));




            popis.RemoveAt(a);
            comboBox2.ItemsSource = new ObservableCollection<string>(popis);

        }

        private void brisi2(int a)
        {
            //ds.Tables["Tablica"].Rows[a].Delete();
            //ds.WriteXml(dir + @"\" + "bazica.xml");

            db.Delete("Tablica", String.Format("Projekt = {0}", "\"" + popis2[a] + "\""));




            popis2.RemoveAt(a);
            comboBox2a.ItemsSource = new ObservableCollection<string>(popis2);

        }


        private void T1_KeyDown(object sender, KeyEventArgs e)
        {
            spremit = true;
        }

        private void C1_Click(object sender, RoutedEventArgs e)
        {
            checki();
        }

        private void checki()
        {
            if (C1.IsChecked == true)
            {
                T18.Visibility = Visibility.Visible;
                T18.Height = double.NaN;
                T18.Margin = new Thickness(4);
                T19.Visibility = Visibility.Visible;
                T19.Height = double.NaN;
                T19.Margin = new Thickness(4);
                T20.Visibility = Visibility.Visible;
                T20.Height = double.NaN;
                T20.Margin = new Thickness(4);
                T21.Visibility = Visibility.Visible;
                T21.Height = double.NaN;
                T21.Margin = new Thickness(4);

            }
            else
            {
                T18.Visibility = Visibility.Hidden;
                T18.Height = 0;
                T18.Margin = new Thickness(0);
                T19.Visibility = Visibility.Hidden;
                T19.Height = 0;
                T19.Margin = new Thickness(0);
                T20.Visibility = Visibility.Hidden;
                T20.Height = 0;
                T20.Margin = new Thickness(0);
                T21.Visibility = Visibility.Hidden;
                T21.Height = 0;
                T21.Margin = new Thickness(0);
            }
        }
        //private Element current = new Element(); 

        //private void Canvas_MouseDown(object sender, MouseButtonEventArgs e)
        //{

        //    this.current.X = Mouse.GetPosition((IInputElement)sender).X;

        //    this.current.Y = Mouse.GetPosition((IInputElement)sender).Y;



        //    // Ensure object receives all mouse events.

        //    this.current.InputElement.CaptureMouse();

        //}



        //private void Canvas_MouseUp(object sender, MouseButtonEventArgs e)
        //{

        //    if (this.current.InputElement != null)

        //        this.current.InputElement.ReleaseMouseCapture();

        //}



        //private void Canvas_MouseMove(object sender, MouseEventArgs e)
        //{

        //    // if mouse is down when its moving, then it's dragging current

        //    if (e.LeftButton == MouseButtonState.Pressed)

        //        this.current.IsDragging = true;



        //    if (this.current.IsDragging && current.InputElement != null)
        //    {

        //        // Retrieve the current position of the mouse.

        //        var newX = Mouse.GetPosition((IInputElement)sender).X;

        //        var newY = Mouse.GetPosition((IInputElement)sender).Y;


        //        // Reset the location of the object (add to sender's renderTransform

        //        // newPosition minus currentElement's position

        //        var rt = ((UIElement)this.current.InputElement).RenderTransform;

        //        var offsetX = rt.Value.OffsetX;

        //        var offsetY = rt.Value.OffsetY;

        //        rt.SetValue(TranslateTransform.XProperty, offsetX + newX - current.X);

        //        rt.SetValue(TranslateTransform.YProperty, offsetY + newY - current.Y);



        //        // Update position of the mouse

        //        current.X = newX;

        //        current.Y = newY;

        //    }

        //}



        //private void rect1_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        //{

        //    this.current.InputElement = (IInputElement)sender;

        //}

        //private void rect2_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        //{

        //    this.current.InputElement = (IInputElement)sender;

    }



}

//public class Element
//{



//    #region Fields

//    bool isDragging = false;

//    IInputElement inputElement = null;

//    double x, y = 0;

//    #endregion



//    #region Constructor

//    public Element() { }

//    #endregion



//    #region Properties

//    public IInputElement InputElement
//    {

//        get { return this.inputElement; }

//        set
//        {

//            this.inputElement = value;

//            /* every time inputElement resets, the draggin stops (you actually don't even need to track it, but it made things easier in the begining, I'll change it next time I get to play with it. */

//            this.isDragging = false;

//        }

//    }

//    public double X
//    {

//        get { return this.x; }

//        set { this.x = value; }

//    }

//    public double Y
//    {

//        get { return this.y; }

//        set { this.y = value; }

//    }

//    public bool IsDragging
//    {

//        get { return this.isDragging; }

//        set { this.isDragging = value; }

//    }

//    #endregion

//    }

//}
