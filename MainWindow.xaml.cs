using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Disjunctive_Normal_Form
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public DataTable dt = new DataTable();
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void ButtonGenerate(object sender, RoutedEventArgs e)
        {
            if (!int.TryParse(TBNumberOfFunctions.Text, out int NumberOfFunctions))
            {
                if (TBNumberOfFunctions.Text.Length == 0)
                {
                    await this.ShowMessageAsync("Error", "Number of functions cannot be empty");
                }
                else
                {
                    await this.ShowMessageAsync("Error", "Wrong format in number of functions TextBox");
                }

                return;
            }
            else if (!int.TryParse(TBNumberOfLogicValues.Text, out int NumberOfLogicValues))
            {
                if (TBNumberOfLogicValues.Text.Length == 0)
                {
                    await this.ShowMessageAsync("Error", "Number of logic values cannot be empty");
                }
                else
                {
                    await this.ShowMessageAsync("Error", "Wrong format in number of logic values TextBox");
                }

                return;
            }
            else
            {
                CreateDataGrid(NumberOfFunctions, NumberOfLogicValues);
            }
        }

        private void ButtonCalculate(object sender, RoutedEventArgs e)
        {
            DataTable data = new DataTable();
            data = ((DataView)DGFunctionsTable.ItemsSource).ToTable();
            List<List<int>> truthTable = new List<List<int>>();
            for (int i = 0; i < data.Rows.Count; i++)
            {
                List<int> row = new List<int>();
                for (int j = 0; j < data.Columns.Count - 1; j++)
                {
                    row.Add((int)data.Rows[i]["f" + j]);
                }
                row.Add((int)data.Rows[i]["y"]);
                truthTable.Add(row);
            }
            DNF dnf = new DNF
            {
                Data = truthTable
            };
            Result.Text = dnf.Calculate();
        }

        private async void ButtonLoad(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            List<string> file = new List<string>();
            openFileDialog.Filter = "Text files (*.txt)|*.txt";
            if (openFileDialog.ShowDialog() == true)
            {
                file = File.ReadAllLines(openFileDialog.FileName).ToList();
            }
            else
            {
                await this.ShowMessageAsync("Error", "Select file");
                return;
            }
            List<List<int>> data = new List<List<int>>();
            for (int i = 0; i < file.Count; i++)
            {
                string[] valInRows = file[i].Split(' ');
                List<int> row = new List<int>();
                foreach (string val in valInRows)
                {
                    try
                    {
                        row.Add(int.Parse(val));
                    }
                    catch
                    {
                        await this.ShowMessageAsync("Error", "Could not parse file");
                    }
                }
                data.Add(row);
            }
            CreateDataGridFromFile(data);
        }

        private void CreateDataGridFromFile(List<List<int>> list)
        {
            dt = new DataTable();

            for (int i = 0; i < list[0].Count - 1; i++)
            {
                dt.Columns.Add("f" + i, Type.GetType("System.Int32"));
            }
            dt.Columns.Add("y", Type.GetType("System.Int32"));

            for (int i = 0; i < list.Count; i++)
            {
                DataRow row = dt.NewRow();
                for (int j = 0; j < list[0].Count - 1; j++)
                {
                    row["f" + j] = list[i][j];
                }
                row["y"] = list[i].Last();
                dt.Rows.Add(row);
            }

            DGFunctionsTable.DataContext = dt.DefaultView;
            Style style = new Style()
            {
                TargetType = typeof(DataGridCell),
            };
            style.Setters.Add(new Setter(BackgroundProperty, FindResource("AccentColorBrush4")));
            DGFunctionsTable.Columns[list[0].Count - 1].CellStyle = style;

            BCalculate.IsEnabled = true;

        }

        private void CreateDataGrid(int x, int y)
        {
            Random random = new Random();
            dt = new DataTable();

            for (int i = 0; i < x; i++)
            {
                dt.Columns.Add("f" + i, Type.GetType("System.Int32"));
            }

            dt.Columns.Add("y", Type.GetType("System.Int32"));

            for (int i = 0; i < y; i++)
            {

                DataRow row = dt.NewRow();
                for (int j = 0; j < x; j++)
                {
                    row["f" + j] = random.Next(0, 2);
                }
                row["y"] = random.Next(0, 2);
                dt.Rows.Add(row);
            }

            DGFunctionsTable.DataContext = dt.DefaultView;
            Style style = new Style()
            {
                TargetType = typeof(DataGridCell),
            };
            style.Setters.Add(new Setter(BackgroundProperty, FindResource("AccentColorBrush4")));
            DGFunctionsTable.Columns[x].CellStyle = style;

            BCalculate.IsEnabled = true;
        }

    }
}
