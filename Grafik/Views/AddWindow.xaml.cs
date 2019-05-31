using Grafik.Controllers;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;

namespace Grafik.Views
{
    public partial class AddWindow : Window
    {
        List<Tuple<uint, double>> list;

        public AddWindow()
        {
            InitializeComponent();
            list = new List<Tuple<uint, double>>();
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            //this try block checks if nText and aText fields contains compatible number formats
            try
            {
                list.Add(new Tuple<uint, double>(uint.Parse(nText.Text), double.Parse(aText.Text)));
            }
            catch (Exception)
            {
                MessageBox.Show("Wprowadzono nieprawidłowe dane!\nSpróbuj wpisać liczbę w odpowiednie miejsce", "Błąd", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (list.Count != 1)
                functionLabel.Content += " +";
            functionLabel.Content += ' ' + aText.Text + "x^" + nText.Text + ' ';
            aText.Text = null;
            nText.Text = null;
        }

        private void ReadyButton_Click(object sender, RoutedEventArgs e)
        {
            if (list.Count > 0)
                Controller.Functions.Add(new Function(list));
            Close();
        }

        private void AText_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            //prevent from typing not double number
            for (int i = 0; i < e.Text.Length; i++)
            {
                if (!((e.Text[i] >= '0' && e.Text[i] <= '9') || e.Text[i] == ',' || e.Text[i] == '-'))
                    e.Handled = true;
            }
        }

        private void NText_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            //prevent from typing not uint number
            for (int i = 0; i < e.Text.Length; i++)
            {
                if (!(e.Text[i] >= '0' && e.Text[i] <= '9'))
                    e.Handled = true;
            }
        }
    }
}
