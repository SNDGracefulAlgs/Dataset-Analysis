using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.IO;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Dataset_Analysis
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public List<string> atributesList;
        public Dictionary<string, List<string>> atributesDomain;
        public List<Precedent> precedentsList;
        public MainWindow()
        {
            InitializeComponent();
            filePathTextBox.Text = Directory.GetCurrentDirectory() + "\\Data_AlfaInsurance_170918.csv";
        }

        private void connectToCSVButton_Click(object sender, RoutedEventArgs e)
        {
            string filePath = filePathTextBox.Text;
            Dictionary<string, List<string>> atributesNewDomain = new Dictionary<string, List<string>>();
            StreamReader sr = new StreamReader(filePath);
            atributesList = new List<string>();
            string headerStr;
            headerStr = sr.ReadLine();
            while (headerStr != "")
            {
                if (headerStr.IndexOf(';') > -1)
                {
                    atributesList.Add(headerStr.Substring(0, headerStr.IndexOf(';')));
                    headerStr = headerStr.Remove(0, headerStr.IndexOf(';') + 1);
                }
                else
                {
                    atributesList.Add(headerStr);
                    headerStr = "";
                }
            }
            string curStr;
            precedentsList = new List<Precedent>();

            while (!sr.EndOfStream)
            {
                curStr = sr.ReadLine();
                Precedent precedentNewInstance = new Precedent();
                while (curStr != "")
                {
                    if (curStr.IndexOf(';') > -1)
                    {
                        precedentNewInstance.attributes.Add(atributesList[precedentNewInstance.attributes.Count], curStr.Substring(0, curStr.IndexOf(';')));
                        curStr = curStr.Remove(0, curStr.IndexOf(';') + 1);
                    }
                    else
                    {
                        precedentNewInstance.attributes.Add(atributesList[precedentNewInstance.attributes.Count], curStr);
                        curStr = "";
                    }
                }
                precedentsList.Add(precedentNewInstance);
            }

            foreach(string atribute in atributesList)
            {
                atributesNewDomain.Add(atribute, (precedentsList.Select(p => p.attributes[atribute]).Distinct()).ToList());
            }
            atributesDomain = atributesNewDomain;
            infoTextBox.Text = "Precedents count= " + precedentsList.Count;

            atributesComboBox1.ItemsSource = atributesList;
            atributesComboBox1.SelectedIndex = 29;
            atributesComboBox2.ItemsSource = atributesList;
            atributesComboBox2.SelectedIndex = 1;

            makeDistributionButton.Content = "Output distribution of values of attribute <" + atributesComboBox1.SelectedItem + "> on values of attribute <" + atributesComboBox2.SelectedItem + ">";

            makeDistributionButton.IsEnabled = true;
            atributesComboBox1.IsEnabled = true;
            atributesComboBox2.IsEnabled = true;

            bolemisationMenuItem.IsEnabled = true;
        }




        private void atributesComboBox1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            makeDistributionButton.Content = "Output distribution of values of attribute <" + atributesComboBox1.SelectedItem + "> on values of attribute <" + atributesComboBox2.SelectedItem + ">";
        }

        private void atributesComboBox2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            makeDistributionButton.Content = "Output distribution of values of attribute <" + atributesComboBox1.SelectedItem + "> on values of attribute <" + atributesComboBox2.SelectedItem + ">";
        }

        private void makeDistributionButton_Click(object sender, RoutedEventArgs e)
        {
            List<string> atr2ValuesList = (precedentsList.Select(p => p.attributes[atributesComboBox2.Text]).Distinct()).ToList();
            List<string> atr1ValuesList = (precedentsList.Select(p => p.attributes[atributesComboBox1.Text]).Distinct()).ToList();
            atr2ValuesList.Sort();
            infoTextBox.Text = "";
            foreach (string groupValue in atr2ValuesList)
            {
                infoTextBox.Text += groupValue + ":\n";
                Dictionary<string, float> valuesDict = new Dictionary<string, float>();
                int s = 0;
                foreach (string atr in atr1ValuesList)
                {
                    int val = (((precedentsList.Where(p => p.attributes[atributesComboBox2.Text] == groupValue)).Where(q => q.attributes[atributesComboBox1.Text] == atr)).ToList()).Count;
                    s += val;
                    valuesDict.Add(atr, val);
                }
                foreach (string key in valuesDict.Keys)
                {
                    infoTextBox.Text += "    " + key + "   (abs): " + (float)(valuesDict[key]) / (float)(precedentsList.Count) + "   (rel): " + (float)(valuesDict[key]) / s + "\n";
                }
                valuesDict.Clear();
            }
            atr1ValuesList.Clear();
            atr2ValuesList.Clear();
        }

        private void bolemisationMenuItem_Click(object sender, RoutedEventArgs e)
        {
            BoolemisationWnd boolemisationWndInstance = new BoolemisationWnd(precedentsList,atributesDomain);
            boolemisationWndInstance.Owner = this;
            this.IsEnabled = false;
            boolemisationWndInstance.Show();
        }
    }
}
