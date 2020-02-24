using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using CustomButtons;
using System.IO;
using Microsoft.VisualBasic;
using Microsoft.Win32;


namespace CustomButtons
{
    
    public class PrillButton : Button
    {
        public static CaselType[] Types;
        
        public ImageSource Source
        {
            get { return base.GetValue(SourceProperty) as ImageSource; }
            set { base.SetValue(SourceProperty, value); }
        }
        public static readonly DependencyProperty SourceProperty =
          DependencyProperty.Register("Source", typeof(ImageSource), typeof(PrillButton));
        public enum CaselType
        {
            niente,
            breakable,
            unbreakable, //diamond is
            battery,
            goal,
            dynBreakable,
            dynamite
        }
        public CaselType Casel; 
        protected override void OnClick()
        {
            if (CaselConvert(Casel) + 1 < Types.Length)
            {
                ChangeCasel(Types[CaselConvert(Casel) + 1]);
            }
            else
            {
                ChangeCasel(0);
            }
            
            base.OnClick();
           
        }
        public static void init()
        {
            
            int nOfTypes = 7;
            Types = new CaselType[nOfTypes];

            for (int i = 0; i < Types.Length; i++)
            {
                Types[i] = PrillButton.IntToCasel(i);
            }
        }
        public static int CaselConvert(CaselType casel)
        {
            switch (casel)
            {
                case (CaselType.niente):
                    return 0;
                case (CaselType.breakable):
                    return 1;
                case (CaselType.unbreakable):
                    return 2;
                case (CaselType.battery):
                    return 3;
                case (CaselType.dynamite):
                    return 4;
                case (CaselType.dynBreakable):
                    return 5;
                case (CaselType.goal):
                    return 6;
                default:
                    return 9;
            }
        }
        public static CaselType IntToCasel(int n)
        {
            switch (n)
            {
                case (0):
                    return CaselType.niente;
                case (1):
                    return CaselType.breakable;
                case (2):
                    return CaselType.unbreakable;
                case (3):
                    return CaselType.battery;
                case (4):
                    return CaselType.dynamite;
                case (5):
                    return CaselType.dynBreakable;
                case (6):
                    return CaselType.goal;
                default:
                    return CaselType.niente;
            }//could be more modular, now you have to modify both methods
        }
        public void CaselSetter(int casel)
        {
            ChangeCasel(IntToCasel(casel));
           
        }
        public void ChangeCasel(CaselType type)
        {
            Dictionary<CaselType, SolidColorBrush> colors = new Dictionary<CaselType, SolidColorBrush>();
            colors.Add(CaselType.breakable, Brushes.Gray);
            colors.Add(CaselType.unbreakable, Brushes.Black);
            colors.Add(CaselType.battery, Brushes.Green);
            colors.Add(CaselType.dynamite, Brushes.Red);
            colors.Add(CaselType.dynBreakable, Brushes.Yellow);
            colors.Add(CaselType.goal, Brushes.Orange);
            colors.Add(CaselType.niente, Brushes.White);

            Casel = type;
            Background = colors[type];
           
        }
        protected override void OnMouseRightButtonDown(MouseButtonEventArgs e)
        {
            switch (Casel)
            {
                case CaselType.niente:
                    ChangeCasel(CaselType.goal);
                    break;
                case CaselType.goal:
                    ChangeCasel(CaselType.dynBreakable);
                    break;
                case CaselType.dynBreakable:
                    ChangeCasel(CaselType.dynamite);
                    break;
                case CaselType.dynamite:
                    ChangeCasel(CaselType.battery);
                    break;
                case CaselType.battery:
                    ChangeCasel(CaselType.unbreakable);
                    break;
                case CaselType.unbreakable:
                    ChangeCasel(CaselType.breakable);
                    break;
                case CaselType.breakable:
                    ChangeCasel(CaselType.niente);
                    break;
            }
        }
    }
}


namespace PrillGrid3
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        static public int NumberOfColumns = 10;
        static public int NumberOfRows = 10;
        static public string ButtonPressed;
        static public int MaxRows = 100;
        static public int MaxColumns = 100;
        //public static int GridWidth = 1200;
        //public static int GridHeigth;
        public static double GridWidth = SystemParameters.PrimaryScreenWidth;
        public static double GridHeigth;
        public static PrillButton[,] buttons = new PrillButton[MaxRows, MaxColumns];
        //public static List<List<Button>> buttons = new List<List<Button>>();
        public static string SaveFile = "Livello.prill";

        public MainWindow()
        {
            InitializeComponent();
            PrillButton.init();

            try
            {
                NumberOfRows = int.Parse(Interaction.InputBox("How many rows?", "Number of Rows", "10"));
                NumberOfColumns = int.Parse(Microsoft.VisualBasic.Interaction.InputBox("How many columns? (Last column will be used for commands)", "Number of columns", "10"));
            }
            catch
            {
                MessageBox.Show("Invalid value");
                return;

            }
            if (NumberOfColumns <= 0 || NumberOfRows <= 0)
            {
                MessageBox.Show("Values cannot be lower than 0");
                return;
            }
            GridHeigth = GridWidth * NumberOfRows / NumberOfColumns;
            while (GridHeigth > SystemParameters.PrimaryScreenHeight)
            {
                GridWidth = GridWidth / 2;
                GridHeigth = GridWidth * NumberOfRows / NumberOfColumns;
            }

            // Create the Grid
            Grid DynamicGrid = new Grid();
            DynamicGrid.Width = GridWidth;
            DynamicGrid.Height = GridHeigth;
            DynamicGrid.HorizontalAlignment = HorizontalAlignment.Left;
            DynamicGrid.VerticalAlignment = VerticalAlignment.Top;
            DynamicGrid.ShowGridLines = true;
            DynamicGrid.Background = new SolidColorBrush(Colors.LightSteelBlue);
            // Create Columns

            ColumnDefinition[] gridCols = new ColumnDefinition[NumberOfColumns];
            for (int i = 0; i < NumberOfColumns; i++)
            {
                gridCols[i] = new ColumnDefinition();
                DynamicGrid.ColumnDefinitions.Add(gridCols[i]);
            }
            // Create Rows

            RowDefinition[] gridRows = new RowDefinition[NumberOfRows];
            for (int i = 0; i < NumberOfRows; i++)
            {
                gridRows[i] = new RowDefinition();
                DynamicGrid.RowDefinitions.Add(gridRows[i]);
            }

            for (int i = 0; i < NumberOfRows; i++)
            {
                for (int j = 0; j < NumberOfColumns; j++)
                {
                    if (j != NumberOfColumns - 1)
                    {
                        buttons[i, j] = new CustomButtons.PrillButton();
                        buttons[i, j].Width = GridWidth / NumberOfColumns;
                        buttons[i, j].Height = GridHeigth / NumberOfRows;
                        buttons[i, j].Margin = new Thickness(1);
                        buttons[i, j].VerticalAlignment = VerticalAlignment.Center;
                        buttons[i, j].SetValue(Grid.ColumnProperty, j);
                        buttons[i, j].SetValue(Grid.RowProperty, i);
                        buttons[i, j].Background = Brushes.White;
                        DynamicGrid.Children.Add(buttons[i, j]);
                        //buttons[i, j].Click += new RoutedEventHandler(buttonCheck);
                    }

                    else
                    {
                        buttons[i, j] = new PrillButton();
                        buttons[i, j].Background = Brushes.Gray;
                        buttons[i, j].Width = GridWidth / NumberOfColumns;
                        buttons[i, j].Height = GridHeigth / NumberOfRows;
                        buttons[i, j].Margin = new Thickness(1);
                        buttons[i, j].VerticalAlignment = VerticalAlignment.Center;
                        buttons[i, j].SetValue(Grid.ColumnProperty, j);
                        buttons[i, j].SetValue(Grid.RowProperty, i);
                        buttons[i, j].FontSize = 12;

                        //buttons[i,j].
                        DynamicGrid.Children.Add(buttons[i, j]);
                        ultimaRiga(i);
                    }
                }
            }
            // Display grid into a Window
            Content = DynamicGrid;
            WindowState = WindowState.Maximized;
            //WindowStyle = WindowStyle.None; 
        }

        static void ultimaRiga(int riga)
        {
            switch (riga)
            {
                case 0:
                    buttons[riga, NumberOfColumns - 1].Click += new RoutedEventHandler(saveButton);
                    buttons[riga, NumberOfColumns - 1].Content = "Save";
                    break;
                case 1:
                    buttons[riga, NumberOfColumns - 1].Click += new RoutedEventHandler(loadButton);
                    buttons[riga, NumberOfColumns - 1].Content = "Load";
                    break;
                case 2:
                    buttons[riga, NumberOfColumns - 1].Click += new RoutedEventHandler(upButton);
                    buttons[riga, NumberOfColumns - 1].Content = "Up";
                    break;
                case 3:
                    buttons[riga, NumberOfColumns - 1].Click += new RoutedEventHandler(rightButton);
                    buttons[riga, NumberOfColumns - 1].Content = "Right";
                    break;
                case 4:
                    buttons[riga, NumberOfColumns - 1].Click += new RoutedEventHandler(leftButton);
                    buttons[riga, NumberOfColumns - 1].Content = "Left";
                    break;
                case 5:
                    buttons[riga, NumberOfColumns - 1].Click += new RoutedEventHandler(downButton);
                    buttons[riga, NumberOfColumns - 1].Content = "Down";
                    break;
                case 6:
                    buttons[riga, NumberOfColumns - 1].Click += new RoutedEventHandler(clearButton);
                    buttons[riga, NumberOfColumns - 1].Content = "Clear";
                    break;
            }
        }

        static void saveButton(object sender, RoutedEventArgs e)
        {
            buttons[0, NumberOfColumns - 1].Background = Brushes.Gray;
            PrillButton.CaselType[,] caselOut = new PrillButton.CaselType[NumberOfRows, NumberOfColumns - 1];
            SaveFile = Interaction.InputBox("Choose a name", "Save", "Livello") + ".prill";
            if (File.Exists(SaveFile))
            {
                string risposta = "";
                risposta = Interaction.InputBox("Are you sure? Any file with the same name will be overwritten", "Save", "Yes");
                if (risposta == "Yes")
                { }
                else
                {
                    return;
                }
            }
            StreamWriter saver = new StreamWriter(SaveFile);
            PrillButton[,] prillButtons = new PrillButton[NumberOfRows, NumberOfColumns - 1];
            for (int riga = 0; riga < NumberOfRows; riga++)
            {
                for (int colonna = 0; colonna < NumberOfColumns - 1; colonna++)
                {
                    prillButtons[riga, colonna] = buttons[riga, colonna];
                    caselOut[riga, colonna] = prillButtons[riga, colonna].Casel;
                    saver.Write(PrillButton.CaselConvert(caselOut[riga, colonna]) + ",");
                }
                saver.Write("\n");
            }
            saver.Close();
            MessageBox.Show("Saved");
        }

        static void loadButton(object sender, RoutedEventArgs e)
        {
            //PrillButton.CaselType[,] loadCasel = new PrillButton.CaselType[NumberOfRows, NumberOfColumns - 1];
            SaveFile = Interaction.InputBox("Inserire il nome del file", "Load", "Livello") + ".prill";
            if (SaveFile == ".prill")
            {
                MessageBox.Show("Caricamento fallito");
                return;
            }
            if (File.Exists(SaveFile) == false)
            {
                MessageBox.Show("File not found");
                return;
            }
            StreamReader reader = new StreamReader(SaveFile);
            int riga = 0;
            int colonna = 0;
            string s;
            while (reader.EndOfStream == false)
            {
                char c = (char)reader.Read();
                s = c.ToString();
                if (s == ",")
                {
                }
                else if (s == "\n")
                {
                    riga = riga + 1;
                    colonna = 0;
                }
                else
                {
                    if (riga < NumberOfRows && colonna < NumberOfColumns)
                    {
                        buttons[riga, colonna].CaselSetter(int.Parse(s));
                        colonna = colonna + 1;
                    }
                }
            }
            reader.Close();
            buttons[1, NumberOfColumns - 1].Background = Brushes.Gray;
            MessageBox.Show("Loaded");
        }

        static void upButton(object sender, RoutedEventArgs e)
        {
            PrillButton.CaselType[,] temp = new PrillButton.CaselType[NumberOfRows, NumberOfColumns];
            for (int riga = 0; riga < NumberOfRows; riga++)
            {
                for (int colonna = 0; colonna < NumberOfColumns - 1; colonna++)
                {
                    if (riga != NumberOfRows - 1)
                    {
                        temp[riga, colonna] = buttons[riga + 1, colonna].Casel;
                    }
                    else
                    {
                        temp[riga, colonna] = buttons[0, colonna].Casel;
                    }
                }
            }
            for (int riga = 0; riga < NumberOfRows; riga++)
            {
                for (int colonna = 0; colonna < NumberOfColumns - 1; colonna++)
                {
                    buttons[riga, colonna].ChangeCasel(temp[riga, colonna]);
                }
            }
            buttons[2, NumberOfColumns - 1].Background = Brushes.Gray;
        }

        static void downButton(object sender, RoutedEventArgs e)
        {
            PrillButton.CaselType[,] temp = new PrillButton.CaselType[NumberOfRows, NumberOfColumns];
            for (int riga = 0; riga < NumberOfRows; riga++)
            {
                for (int colonna = 0; colonna < NumberOfColumns - 1; colonna++)
                {
                    if (riga != 0)
                    {
                        temp[riga, colonna] = buttons[riga - 1, colonna].Casel;
                    }
                    else
                    {
                        temp[riga, colonna] = buttons[NumberOfRows - 1, colonna].Casel;
                    }
                }
            }
            for (int riga = 0; riga < NumberOfRows; riga++)
            {
                for (int colonna = 0; colonna < NumberOfColumns - 1; colonna++)
                {
                    buttons[riga, colonna].ChangeCasel(temp[riga, colonna]);
                }
            }
            buttons[5, NumberOfColumns - 1].Background = Brushes.Gray;
        }

        static void leftButton(object sender, RoutedEventArgs e)
        {
            PrillButton.CaselType[,] temp = new PrillButton.CaselType[NumberOfRows, NumberOfColumns];
            for (int riga = 0; riga < NumberOfRows; riga++)
            {
                for (int colonna = 0; colonna < NumberOfColumns - 1; colonna++)
                {
                    if (colonna != NumberOfColumns - 2)
                    {
                        temp[riga, colonna] = buttons[riga, colonna + 1].Casel;
                    }
                    else
                    {
                        temp[riga, colonna] = buttons[riga, 0].Casel;
                    }
                }
            }
            for (int riga = 0; riga < NumberOfRows; riga++)
            {
                for (int colonna = 0; colonna < NumberOfColumns - 1; colonna++)
                {
                    buttons[riga, colonna].ChangeCasel(temp[riga, colonna]);
                }
            }
            buttons[4, NumberOfColumns - 1].Background = Brushes.Gray;
        }
        static void rightButton(object sender, RoutedEventArgs e)
        {
            PrillButton.CaselType[,] temp = new PrillButton.CaselType[NumberOfRows, NumberOfColumns];
            for (int riga = 0; riga < NumberOfRows; riga++)
            {
                for (int colonna = 0; colonna < NumberOfColumns - 1; colonna++)
                {
                    if (colonna != 0)
                    {
                        temp[riga, colonna] = buttons[riga, colonna - 1].Casel;
                    }
                    else
                    {
                        temp[riga, colonna] = buttons[riga, NumberOfColumns - 2].Casel;
                    }
                }
            }
            for (int riga = 0; riga < NumberOfRows; riga++)
            {
                for (int colonna = 0; colonna < NumberOfColumns - 1; colonna++)
                {
                    buttons[riga, colonna].ChangeCasel(temp[riga, colonna]);
                }
            }
            buttons[3, NumberOfColumns - 1].Background = Brushes.Gray;
        }

        static void clearButton(object sender, RoutedEventArgs e)
        {
            buttons[6, NumberOfColumns - 1].Background = Brushes.Gray;
            if (Interaction.InputBox("Sicuro sicuro?", "Sure?", "Yes") == "Yes")
            {
                for (int riga = 0; riga < NumberOfRows; riga++)
                {
                    for (int colonna = 0; colonna < NumberOfColumns - 1; colonna++)
                    {
                        buttons[riga, colonna].Background = Brushes.White;
                        buttons[riga, colonna].Casel = PrillButton.CaselType.niente;
                    }
                }
            }
        }
    }
}