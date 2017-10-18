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
using System.Windows.Shapes;

namespace Dataset_Analysis
{
    /// <summary>
    /// Логика взаимодействия для boolemisationWnd.xaml
    /// </summary>
    public partial class BoolemisationWnd : Window
    {
        List<Precedent> transferPrecedentsList;
        Dictionary<string, List<string>> atributesDomain;
        List<CheckBox> cBList;
        List<string> newAtributesList;
        public BoolemisationWnd(List<Precedent> precedentsList, Dictionary<string, List<string>> atributesInpDomain)
        {
            InitializeComponent();
            filePathTextBox.Text = filePathTextBox.Text = Directory.GetCurrentDirectory() + "\\Data_AlfaInsurance_170918(boolean).csv"; ;
            transferPrecedentsList = precedentsList;

            cBList = new List<CheckBox>();
            foreach (string atribute in atributesInpDomain.Keys)
            {
                CheckBox newCB = new CheckBox();
                stackPanel.Children.Add(newCB);
                newCB.Height = 22;
                newCB.Margin = new Thickness(0);
                newCB.Content = atribute;
                newCB.ToolTip = atribute;
                cBList.Add(newCB);
            }

            atributesDomain = atributesInpDomain;
        }

        private void transformButton_Click(object sender, RoutedEventArgs e)
        {
            newAtributesList = new List<string>();
            StreamWriter curFSInstance = new StreamWriter(filePathTextBox.Text, false);
            string newStr = "";
            foreach (string atribute in atributesDomain.Keys)
            {

                if (cBList.Where(p => p.IsChecked == true).Select(q => q.ToolTip).ToList().Contains(atribute))
                {
                    foreach (string atrValue in atributesDomain[atribute])
                    {
                        newStr += atribute + "->" + atrValue + ';';
                        newAtributesList.Add(atrValue);
                    }
                }
                else
                {
                    newStr += atribute + ';';
                    newAtributesList.Add(atribute);
                }
            }
            newStr.Remove(newStr.LastIndexOf(';'), 1);
//            newStr += "\n";
            curFSInstance.WriteLine(newStr);

            foreach (Precedent precInstance in transferPrecedentsList)
            {
                newStr = "";
                foreach (string key in newAtributesList)
                {
                    if (precInstance.attributes.Keys.Contains(key))
                        newStr += precInstance.attributes[key] + ';';
                    else if (precInstance.attributes.Values.Contains(key))
                        newStr += "1;";
                    else newStr += "0;";
                }
 //               newStr += "\n";
                curFSInstance.WriteLine(newStr);
            }

            curFSInstance.Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.Owner.IsEnabled = true;
        }
    }
}
