﻿using System.Windows.Controls;

namespace TaoBD10.Views
{
    /// <summary>
    /// Interaction logic for DiNgoaiView.xaml
    /// </summary>
    public partial class DiNgoaiView : UserControl
    {
        public DiNgoaiView()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            DiNgoaiMHTxt.Focus();
        }
    }
}