using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IBApi;
using System.Threading;

namespace OptionStrategy
{
    // This class finds the data and sends it back to the main form
    class DataHandler
    {
        // Some Global Vars
        // Objects From Other Classes
        private frmMain parentReference;

        // These two counters are used to avoid duplicates in connection request ids
        private int tickerListCounter = 0;// here we keep track of which ticker we are processing
        private int strikeListCounter = 0;// here we keep track of which strike we are processint
        private string[] tickersList;

        private string tickerSymbol;// Ticker Name
        private DateTime expirationDate;// Contract Expiration Date
        private string optionRight;// Type of options to look for
        private OSWrapper singleWrapper;
        private Contract contractDefinition;

        // Strikes Table
        private double[,] strikesTable;// = { { 0.2, 2.0 }, { 0.3, 3.0 } };

        // CONSTANTS
        //************************************************
        // Contract Definition
        const string contractSectorType = "OPT";
        const string contractExchange = "SMART";
        const string contractCurrency = "USD";
        const string contractMultiplier = "100";
        const bool contractIncludeExpired = false;


        //Connection Parameters
        const string connectionHost = "";
        const int connectionPort = 4001;
        const int connectionClientID = 13;
        const int connectionExtraAuth = 0;
        
        public DataHandler(frmMain parentClass, DateTime chosenExpirationDate, string right, string[] tickers)
        {
            parentReference = parentClass;//this is to have a reference for callbacks.

            tickersList = tickers;
            expirationDate = chosenExpirationDate;// Setting the expiration date
            optionRight = right;

            // We initialize a new Wrapper
            singleWrapper = new OSWrapper(this);

            // Constants setting for the Contracts
            contractDefinition = new Contract();
            contractDefinition.SecType = contractSectorType;
            contractDefinition.Exchange = contractExchange;
            contractDefinition.Currency = contractCurrency;
            contractDefinition.Multiplier = contractMultiplier;
            contractDefinition.IncludeExpired = contractIncludeExpired;

            this.ConnectionToTheServer(); // Connection to Server

            StartStrategyProcedure(tickersList[tickerListCounter]);
        }

        // This method has the sequence for all the data collecting
        public void StartStrategyProcedure(string currentTicker)
        {
            tickerSymbol = currentTicker; // Up to now we didn't know which ticker to process
            this.RequestContractDetails(); // Asking for contract details
        }

        // Conenction to the server
        private void ConnectionToTheServer()
        {
            // Connection
            singleWrapper.ClientSocket.eConnect(connectionHost, connectionPort, connectionClientID);
            // Set up the form object in the EWrapper to send Back the Signals
            //singleWrapper.myform = (MainForm)Application.OpenForms[0];


            // ATTENTION!!!! The rest of the method was copied, I have little understanding of the workings, so don't touch as long as it works!!!
            //Create a reader to consume messages from the TWS. The EReader will consume the incoming messages and put them in a queue
            var reader = new EReader(singleWrapper.ClientSocket, singleWrapper.Signal);
            reader.Start();
            //Once the messages are in the queue, an additional thread need to fetch them
            new Thread(() => { while (singleWrapper.ClientSocket.IsConnected()) { singleWrapper.Signal.waitForSignal(); reader.processMsgs(); } }) { IsBackground = true }.Start();

            /*************************************************************************************************************************************************/
            /* One (although primitive) way of knowing if we can proceed is by monitoring the order's nextValidId reception which comes down automatically after connecting. */
            /*************************************************************************************************************************************************/
            while (singleWrapper.NextOrderId <= 0) { }
        }

        // Requests for Contracts Details
        private void RequestContractDetails()
        {
            // We set the changing contract details
            contractDefinition.LastTradeDateOrContractMonth = expirationDate.ToString("yyyyMMdd");
            contractDefinition.Strike = 0;
            contractDefinition.Symbol = tickerSymbol;
            contractDefinition.Right = optionRight;

            singleWrapper.ClientSocket.reqContractDetails(((tickerListCounter+1)*100), contractDefinition);// connectionID: +1 to avoid 0 that messes things up, *100 to wide the space between the connections
        }

        // Request Market data for specific strike
        private void RequestMarketdata(int i)
        {
            contractDefinition.Strike = strikesTable[i, 0];
            int connectionID = (strikeListCounter + 1) + ((tickerListCounter + 1)*100);// take the tickerCount as base and adds the strike count (+1 to avoid 0 which would screw things up.)
           
            singleWrapper.ClientSocket.reqMktData(i, contractDefinition, "", true, null);
        }

        //RETREIVERS
        //**************************************************************************************************
        // Strikes List retreiver
        public void SetStrikesList(double[] strikesList)
        {
            strikesTable = new double[strikesList.GetUpperBound(0), 3];// We set the dimensions for the Table
                        
            for (int i = 0; i <= strikesList.GetUpperBound(0) -1; i++)
            {
                strikesTable[i, 0] = strikesList[i];
            }
            strikeListCounter = 0;
            RequestMarketdata(strikeListCounter);
            //SetStrikesTable(strikesTable);
        }

        // Delta and Bid retreiver
        public void SetBidAndDelta(double[] bidAndDelta)
        {
            // Delta is index 0, Bid is index 2
            strikesTable[strikeListCounter, 1] = bidAndDelta[0];// Set the Delta
            strikesTable[strikeListCounter, 2] = bidAndDelta[1];// Set the Delta

            strikeListCounter++; // Proceed to next strike
            // Check if we are in the List Bounds
            if (strikeListCounter <= strikesTable.GetUpperBound(0))
            {
                RequestMarketdata(strikeListCounter);
            }
            else // If not we are done
            {
                
                SetStrikesTable(condenseStrikesTable());

                if (tickerListCounter <= tickersList.GetUpperBound(0))
                {
                    StartStrategyProcedure(tickersList[tickerListCounter]);
                }
                else
                {
                    parentReference.TickersCompleted();
                }
            }
        }

        private double[,] condenseStrikesTable()
        {
            int relevantStrikes = 0;
            int j = 0;
            for(int i =0; i <= strikesTable.GetUpperBound(0); i++)
            {
                if (strikesTable[i, 1] != 0 && strikesTable[i, 2] != 0)
                {
                    relevantStrikes++;//just counting the relevant strikes
                }
            }
            double[,] newStrikesTable = new double[relevantStrikes, 3];
            for (int i = 0; i <= strikesTable.GetUpperBound(0); i++)
            {
                if (strikesTable[i, 1] != 0 && strikesTable[i, 2] != 0)
                {
                    // Copying the relevant strikes
                    newStrikesTable[j, 0] = strikesTable[i, 0];
                    newStrikesTable[j, 1] = strikesTable[i, 1];
                    newStrikesTable[j, 2] = strikesTable[i, 2];
                    j++;
                }
            }

            return newStrikesTable;
        }

        //SENDER
        //**************************************************************************************************
        // This sends to the frmMain
        public void SetStrikesTable(double[,] finalTable)
        {
            parentReference.SetStrikes(tickerSymbol, finalTable);// Send data to frmMain
            strikeListCounter = 0;
            tickerListCounter++;
        }

    }
}
