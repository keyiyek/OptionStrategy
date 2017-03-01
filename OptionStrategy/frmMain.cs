using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Collections.Generic;

namespace OptionStrategy
{
    public partial class frmMain : Form
    {
        // We need a DataHandler and some Delegates
        DataHandler dataHandler;
        Dictionary<string, double[,]> database = new Dictionary<string, double[,]>();
        public delegate void SetAddCallback(string ticker, double price, double[,] strikes);

        // Delegates
        public delegate void DataHandlerPageAdd(TabPage resultPage);
        public delegate void DataHandlerResultsAdd(ListViewItem TickersResults);

        public frmMain()
        {
            InitializeComponent();
        }

        // UI BUTTONS METHODS
        //*****************************************************************************************//
        // Load Tickers
        private void btLoadTickers_Click(object sender, EventArgs e)
        {
            DialogResult loadTickers = ofdLoadTickers.ShowDialog();
            if (loadTickers == DialogResult.OK)
            {
                string tickersListFile = ofdLoadTickers.FileName;
                try
                {
                    string[] lines = File.ReadAllLines(tickersListFile);
                    foreach (string tickerSymbol in lines)
                    {
                        lsbTickers.Items.Add(tickerSymbol);
                    }
                }
                catch (IOException) { }
            }
        }

        // Add Ticker to list
        private void btAddTicker_Click(object sender, EventArgs e)
        {
            lsbTickers.Items.Add(txbAddTickers.Text);
        }

        // Remove ticker from list
        private void btRemoveTickers_Click(object sender, EventArgs e)
        {
            // Can't iterate with foreach, need to go backwards otherwise selected items will change while elimination is in progress
            var selectedItems = new ListBox.SelectedObjectCollection(lsbTickers);
            selectedItems = lsbTickers.SelectedItems;

            if (lsbTickers.SelectedItems.Count != -1)
            {
                for (int i = lsbTickers.SelectedItems.Count - 1; i >= 0; i--)
                {
                    lsbTickers.Items.Remove(selectedItems[i]);
                }
            }

        }

        // Save ticker list
        private void btSaveTickers_Click(object sender, EventArgs e)
        {
            DialogResult saveTickers = sfdSaveTickers.ShowDialog();// Call the dialog
        }
        // Save File Dialog
        private void sfdSaveTickers_FileOk(object sender, CancelEventArgs e)
        {
            string[] fileLines = new string[lsbTickers.Items.Count];
            string fileName = sfdSaveTickers.FileName;
            for (int i = 0; i < lsbTickers.Items.Count; i++)
            {
                fileLines[i] = lsbTickers.Items[i].ToString();
            }
            File.WriteAllLines(fileName, fileLines);
        }
        //**********************************************************

        // DATAHANDLER Methods
        //****************************************************************************************//
        // Here we create a DataHandler, it will give us back, somehow, the data about the Ticker
        private void btConnect_Click(object sender, EventArgs e)
        {
            
            string[] listOfTickers = new string[lsbTickers.Items.Count];
            for(int i = 0; i <= listOfTickers.GetUpperBound(0); i++)
            {
                listOfTickers[i] = lsbTickers.Items[i].ToString();
            }
            dataHandler = new DataHandler(this, dtpExirationDate.Value.Date, cbxRight.Text, listOfTickers);
        }

        // This is to know DataHandler has finished
        public void TickersCompleted()
        {
            MessageBox.Show("Connection Completed!");
        }

        public void SetStrikes(string tickerSymbol, double[,] dataTable)
        {
            database.Add(tickerSymbol, dataTable);
            //this.DisplayPages();

        }


        // AUTOMATIC INTERFACE METHODS
        //****************************************************************************************//
        // These set up the different TabPages with all the data
        // Main Tab Page SetUp
        private void TabPageSetup(string ticker, double price, double[,] strikes)
        {

            TabPage newTabPage = new TabPage();// Create new Page
            newTabPage.Text = ticker;// Set TabPage TitleTitle
            newTabPage.AutoScroll = true;// Set Scrollbar in case there are too many Strike Prices

            Label tickerLabel = new Label();// Create Label for Ticker Name and Price
            Label MaxProfitLabel = new Label();// Create Label for Max Profit Strike Price
            Label RocLabel = new Label();// Create Label for ROC calculation

            const int offSet = 10; // General Offset for display

            tickerLabel.Location = new Point(offSet, offSet); // Place the first Label
            tickerLabel.AutoSize = true;
            tickerLabel.Text = ticker + ": " + price.ToString("0.00"); // Label with ticker and price
            tickerLabel.Parent = newTabPage; // Add to TabPage

            // Here we call the Formatter to do the heavy lifting of setting the data in lines
            // Here is when we know the Price and Strike Prices Count, so we pass them to the Init method
            Formatter getAllInLine = new Formatter(price, strikes.GetUpperBound(0) + 1);// Initiate
            AddScenariosList(getAllInLine.ScenarioLine(strikes), newTabPage, 0);// First Line with Strike Prices
            AddScenariosList(getAllInLine.ProbabilityLine(strikes), newTabPage, 1);// Second Line with deltas
            AddScenariosList(getAllInLine.ProbabilityDiffLine(strikes), newTabPage, 2);// Third line with differential Deltas

            // Then add a line for each strike
            for (int i = 0; i <= strikes.GetUpperBound(0); i++)
            {
                AddScenariosList(getAllInLine.NormalLine(strikes, i), newTabPage, i + 3);// Add a line with calculations
            }

            MaxProfitLabel.Location = new Point(tickerLabel.Width + tickerLabel.Location.X + offSet, offSet);// Place the Profit Label beside the Ticker one
            MaxProfitLabel.AutoSize = true;
            if(getAllInLine.MaxIndex()>=0)// This means Formatter worked
            {
                MaxProfitLabel.Text = "Max: " + strikes[getAllInLine.MaxIndex(), 0].ToString("0.00");
            }
            else
            {
                MaxProfitLabel.Text = "Max: ???";
            }
            MaxProfitLabel.Parent = newTabPage;// Add to TabPage

            double roc = getAllInLine.Roc();// Get the ROC calculated by the Formatter
            RocLabel.Location = new Point(MaxProfitLabel.Width + MaxProfitLabel.Location.X + offSet, offSet);// Place the ROC Label beside the Profit one 
            RocLabel.AutoSize = true;
            RocLabel.Text = "ROC: " + roc.ToString("0.00");
            RocLabel.Parent = newTabPage;// add to TabPage

            AddingTickerPage(newTabPage);// add the TabPage to the TabControl

            var result = new ListViewItem(new String[] { roc.ToString("0.00"), ticker }); // Create  a ListView Item with ROC and Ticker Name
            AddingTickerResults(result);// Add Item to ListView
        }

        // This Adds a line of TextBoxes with the given data
        // Numbers[] is an array with a number for each TextBox
        // Parent is the TabPage the TextBox must land on
        // lineNumber is used to lower the TextBoxes by one line each time
        private void AddScenariosList(double[] numbers, TabPage parent, int lineNumber)
        {

            int txbWidth = 40;// TextBox Width

            // Repeat for all the given numbers
            for (int i = 0; i <= numbers.GetUpperBound(0); i++)
            {
                TextBox newTexBox = new TextBox();
                newTexBox.Width = txbWidth;
                newTexBox.Location = new Point(60 + (i * txbWidth), 60 + (lineNumber * txbWidth));// Automatic placement
                newTexBox.Text = numbers[i].ToString("0.00");

                newTexBox.Parent = parent;// Add to TabPage
            }
        }



        private void AddingTickerPage(TabPage resultsPage)
        {
            if (this.tbcMain.InvokeRequired)
            {
                DataHandlerPageAdd d = new DataHandlerPageAdd(AddingTickerPage);
                this.Invoke(d, new object[] { resultsPage });
            }
            else
            {
                tbcMain.TabPages.Add(resultsPage);
            }
        }

        private void AddingTickerResults(ListViewItem tickersResults)
        {
            if (this.tbcMain.InvokeRequired)
            {
                DataHandlerResultsAdd d = new DataHandlerResultsAdd(AddingTickerResults);
                this.Invoke(d, new object[] { tickersResults });
            }
            else
            {
                lsvResults.Items.Add(tickersResults);
            }
        }

        private void btDisplay_Click(object sender, EventArgs e)
        {
            foreach(var item in database)
            {
                TabPageSetup(item.Key, 50.00, item.Value);
            }
        }

        private void DisplayPages()
        {
            foreach (var item in database)
            {
                TabPageSetup(item.Key, 50.00, item.Value);
            }
        }
    }

}
