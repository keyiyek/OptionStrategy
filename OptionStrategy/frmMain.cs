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
        DataHandler2 dataHandler2;
        List<DataHandler> dataHandlerList = new List<DataHandler>();
        List<DataHandler2> dataHandlerList2 = new List<DataHandler2>();

        // Delegates to handle thread safeness in modifying the form
        public delegate void DataHandlerPageAdd(TabPage resultPage);
        public delegate void DataHandlerResultsAdd(ListViewItem TickersResults);
        private readonly object syncLock = new object();

        int tickerCount; // We need to know how many tickers we are expecting back
        const int numberOfClients = 8; // The number of parallel clients we can connect

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
            txbAddTickers.Text = "";
            txbAddTickers.Select();
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

        // UI Other Methods
        //*************************************************************************************************
        // Set the focus on Add Ticker when the txb is selected
        private void txbAddTickers_Enter(object sender, EventArgs e)
        {
            this.AcceptButton = btAddTicker;
        }

        // Set the focus on Connect when the DateTimePicker is selected
        private void dtpExirationDate_Enter(object sender, EventArgs e)
        {
            this.AcceptButton = btConnect;
        }

        // Set the focus on Connect when the Combobox is selected
        private void cbxRight_Enter(object sender, EventArgs e)
        {
            this.AcceptButton = btConnect;
        }
       

        // DATAHANDLER Methods
        //****************************************************************************************//
        // Here we create a DataHandler, it will give us back, somehow, the data about the Ticker
        private void btConnect_Click(object sender, EventArgs e)
        {
            // Setting the market status variables
            int priceField = 4;
            int optionField = 12;
            if(cbxMarketStatus.Text == Properties.Resources.marketStatusOpen)
            {
                priceField = 1;
                optionField = 10;
            }

            if (cbxMarketStatus.Text == Properties.Resources.marketStatusClosed)
            {
                priceField = 4;
                optionField = 12;
            }

            // Setting how many tickers we are expecting back
            tickerCount = lsbTickers.Items.Count;

            // Getting the tickers displayed in the listbox into 8 different arrays (8 is the max number of clients we can connect at the same time)
            // Finding the arrays dimensions
            int bigArraysDimension = (int)Math.Floor((double)lsbTickers.Items.Count / numberOfClients); // First seven arrays dimention 
            int lastArrayDimension = lsbTickers.Items.Count - (bigArraysDimension * (numberOfClients - 1)); // This shoud give the odd array dimension, 0 if it has the same dimension as the others

            // If bigArrayDimension is 0 it means that there is a list of fewer tickets than numberOfClients, so skip the big arrays and jump to the last one 
            if (bigArraysDimension > 0)
            {
                for (int i = 1; i < numberOfClients; i++)// Creating 7 arrays of bigArrayDimension dimension
                {
                    // Creating 7 mini arrays with different tickers lists and feading them to 7 DataHandlers
                    string[] miniTickerList = CreateMiniTickerList(i, bigArraysDimension);

                    // Deciding wether to create a DataHandler for serial computing, or a DataHandler2 for parallel computing
                    if (chbxParallelComputing.Checked)
                    {
                        dataHandler2 = new DataHandler2(i, this, dtpExirationDate.Value.Date, cbxRight.Text, miniTickerList, priceField, optionField);
                    }
                    else
                    {
                        dataHandler = new DataHandler(i, this, dtpExirationDate.Value.Date, cbxRight.Text, miniTickerList, priceField, optionField);

                    }
                }
            }
                if (lastArrayDimension == 0)//If there is no last array means the 8th array has the same dimentions of the first 7
                {
                    lastArrayDimension = bigArraysDimension;
                }

                // We create the last array and dataHandler
                string[] lastArray = CreateMiniTickerList(numberOfClients, lastArrayDimension, bigArraysDimension);

                // Deciding wether to create a DataHandler for serial computing, or a DataHandler2 for parallel computing
                // This is the second time, not very elegant
                if (chbxParallelComputing.Checked)
                {
                    dataHandler2 = new DataHandler2(numberOfClients, this, dtpExirationDate.Value.Date, cbxRight.Text, lastArray, priceField, optionField);
                }
                else
                {
                    dataHandler = new DataHandler(numberOfClients, this, dtpExirationDate.Value.Date, cbxRight.Text, lastArray, priceField, optionField);
                }

        }

        // Here we generate the shorter ticker lists
        private string[] CreateMiniTickerList(int arrayNumber, int arrayDimension)
        {
            string[] miniTickerList = new string[arrayDimension];
            for (int j = 0; j < arrayDimension; j++)
            {
                miniTickerList[j] = lsbTickers.Items[j + ((arrayNumber - 1) * arrayDimension)].ToString();
            }
            return miniTickerList;
        }

        // Like the one above, but last array needs to know the dimensions of the otherones
        private string[] CreateMiniTickerList(int arrayNumber, int arrayDimension, int previousArrayDimention)
        {
            string[] miniTickerList = new string[arrayDimension];
            for (int j = 0; j < arrayDimension; j++)
            {
                miniTickerList[j] = lsbTickers.Items[j + ((arrayNumber - 1) * previousArrayDimention)].ToString();
            }
            return miniTickerList;
        }

        public void SetStrikes(string tickerSymbol, double tickerPrice, Dictionary<double, double[]> scenarios)
        {
            // This is to make sure we do one TabPage at a time
            lock (syncLock)
            {
                // We count down each time we are receiving back
                tickerCount--;

                // Here we check that there are at least two scenarios, otherwise Formatter will break.
                if (scenarios.Count > 1)
                {
                    // This is not very elegant, transforming a Dictionary into a 2d Array
                    // Problem is, Formatter works with 2d arrays and I don't want to touch it for now
                    int i = 0;
                    double[,] scenariosList = new double[scenarios.Count, 3];// Create the 2D Array
                    foreach (double key in scenarios.Keys)
                    {
                        scenariosList[i, 0] = key;// First is the strike
                        scenariosList[i, 1] = scenarios[key][0]; // Then the delta
                        scenariosList[i, 2] = scenarios[key][1]; // Then the bid
                        i++;
                    }
                    Array.Sort(scenariosList);
                    // Let's try to build the TabPage
                    this.TabPageSetup(tickerSymbol, tickerPrice, scenariosList);
                }
                // Here I would like to log which one I lost
                else
                {
                    int j = 0;
                }
                // If we processed last ticker
                if(tickerCount==0)
                {
                    foreach(DataHandler dataHandler in dataHandlerList)
                    {
                        dataHandler.DisconnectionFromTheServer();
                    }
                    foreach (DataHandler2 dataHandler in dataHandlerList2)
                    {
                        dataHandler.DisconnectionFromTheServer();
                    }
                }
            }
        }


        // AUTOMATIC INTERFACE METHODS
        //****************************************************************************************//
        // These set up the different TabPages with all the data
        // Main Tab Page SetUp
        private void TabPageSetup(string ticker, double price, double[,] strikes)
        {
            const int offSet = 10; // General Offset for display

            // Setting the TabPage
            TabPage newTabPage = new TabPage();// Create new Page
            newTabPage.Text = ticker;// Set TabPage Title
            newTabPage.AutoScroll = true;// Set Scrollbar in case there are too many Strike Prices

            // Setting the Labels
            Label tickerLabel = new Label();// Create Label for Ticker Name and Price
            Label MaxProfitLabel = new Label();// Create Label for Max Profit Strike Price
            Label RocLabel = new Label();// Create Label for ROC calculation

            // Ticker Labels
            tickerLabel.Location = new Point(offSet, offSet); // Place the first Label
            tickerLabel.AutoSize = true;
            tickerLabel.Text = ticker + ": " + price.ToString(Properties.Resources.doubleFormat); // Label with ticker and price
            tickerLabel.Parent = newTabPage; // Add to TabPage
            // Best Strategy Label
            MaxProfitLabel.Location = new Point(tickerLabel.Width + tickerLabel.Location.X + offSet, offSet);// Place the Profit Label beside the Ticker one
            MaxProfitLabel.AutoSize = true;
            // ROC Label
            RocLabel.Location = new Point(MaxProfitLabel.Width + MaxProfitLabel.Location.X + offSet, offSet);// Place the ROC Label beside the Profit one 
            RocLabel.AutoSize = true;

            // Here we call the Formatter to do the heavy lifting of setting the data in lines
            // Here is when we know the Price and Strike Prices Count, so we pass them to the Init method
            Formatter getAllInLine = new Formatter(price, strikes.GetUpperBound(0) + 1);// Initiate
            AddScenariosList(getAllInLine.ScenarioLine(strikes), newTabPage, 0);// First Line with Strike Prices
            AddScenariosList(getAllInLine.ProbabilityLine(strikes), newTabPage, 1);// Second Line with deltas
            if (cbxRight.Text == Properties.Resources.typeCall)
            {
                AddScenariosList(getAllInLine.ProbabilityDiffLineDeltaCall(strikes), newTabPage, 2);// Third line with differential Deltas
            }
            if (cbxRight.Text == Properties.Resources.typePut)
            {
                AddScenariosList(getAllInLine.ProbabilityDiffLineDeltaPut(strikes), newTabPage, 2);// Third line with differential Deltas
            }
            // Then add a line for each strike
            for (int i = 0; i <= strikes.GetUpperBound(0); i++)
            {
                AddScenariosList(getAllInLine.NormalLine(strikes, i), newTabPage, i + 3);// Add a line with calculations
            }

            // Completing the page with last data about Best Strategy and ROC
            if (getAllInLine.MaxIndex() >= 0)// This means Formatter worked
            {
                MaxProfitLabel.Text = "Max: " + strikes[getAllInLine.MaxIndex(), 0].ToString(Properties.Resources.doubleFormat);// Display Best Strategy

                double roc = getAllInLine.Roc();// Get the ROC calculated by the Formatter
                RocLabel.Text = "ROC: " + roc.ToString(Properties.Resources.doubleFormat);// Display ROC
            }
            else // This means Formatter didn't work
            {
                MaxProfitLabel.Text = "Max: ???";// Display Best Strategy
                RocLabel.Text = "ROC: ???";// Display ROC
            }

            // Add to tab Page
            MaxProfitLabel.Parent = newTabPage;
            RocLabel.Parent = newTabPage;

            // add the TabPage to the TabControl
            AddingTickerPage(newTabPage);// We need a new function so it is easier to set the delegate for the safe thread pattern

            var result = new ListViewItem(new String[] { RocLabel.Text, ticker }); // Create  a ListView Item with ROC and Ticker Name
            // Add Item to ListView
            AddingTickerResults(result);// We need a new function so it is easier to set the delegate for the safe thread pattern
        }

        // This Adds a line of TextBoxes with the given data
        // Numbers[] is an array with a number for each TextBox
        // Parent is the TabPage the TextBoxes must land on
        // lineNumber is used to lower the TextBoxes by one line each time
        private void AddScenariosList(double[] numbers, TabPage parent, int lineNumber)
        {
            int txbWidth = 40;// TextBox Width

            // Repeat for all the given numbers
            for (int i = 0; i <= numbers.GetUpperBound(0); i++)
            {
                TextBox newTexBox = new TextBox(); // Create TextBox
                newTexBox.Width = txbWidth;
                newTexBox.Location = new Point(60 + (i * txbWidth), 60 + (lineNumber * txbWidth));// Automatic placement
                newTexBox.Text = numbers[i].ToString(Properties.Resources.doubleFormat); // Set the Text

                newTexBox.Parent = parent;// Add to TabPage
            }
        }

        // Here we actualy add the tabpage to the tabpage controller. We are coming from a different thread so we need to be safe to modify the form
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

        // Here we actualy add the results to the listview. We are coming from a different thread so we need to be safe to modify the form
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


    }
}
