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
        private OSWrapper singleWrapper;
        private Contract contractDefinition;

        // These two counters are used to avoid duplicates in connection request ids
        private int tickerListCounter = 0;// here we keep track of which ticker we are processing
        private int strikeListCounter = 0;// here we keep track of which strike we are processint
        private string[] tickersList;
        private Dictionary<int, TickerData> tickerDataDict = new Dictionary<int, TickerData>();

        private string tickerSymbol;// Ticker Name
        private DateTime expirationDate;// Contract Expiration Date
        private string optionRight;// Type of options to look for
        
        // Strikes Table

        private double[] strikesList;// = { { 0.2, 2.0 }, { 0.3, 3.0 } };

        // CONSTANTS
        //************************************************
        // Contract Definition
        const string typeStock = "STK";
        const string typeOption = "OPT";
        const string contractExchange = "SMART";
        const string contractCurrency = "USD";
        const string contractMultiplier = "100";
        const bool contractIncludeExpired = false;

        // These multipliers are used to set the unique connection id for each request
        // They are also used to understand to wich ticker the returning data belongs
        const int multiplierDH = 1000000; // the first number identifies the DH, not necessary but you never know...
        const int multiplierTK = 10000;// next two numbers identify the ticker, you can have only 99 tickers for each DataHandler
        const int multiplierSP = 10;// the other numbers are for the ticket strike price, you can use only tickers with strike prices up to 3 digits

        // Connection Parameters
        int connectionClientID;
        // ConnectionID parts
        int connectionDH = 0; // main datahandler connectionID root
        int connectionTK = 0; // ticker connectionID
        int connectionSP = 0; // Strike Price connectionID

        const string connectionHost = "";
        const int connectionPort = 4001;
        const int connectionExtraAuth = 0;
        
        public DataHandler(int clientID, frmMain parentClass, DateTime chosenExpirationDate, string right, string[] tickers, int priceField, int optionField)
        {
            parentReference = parentClass;//this is to have a reference for callbacks.

            connectionClientID = clientID;
            connectionDH = connectionClientID;

            tickersList = tickers;
            expirationDate = chosenExpirationDate;// Setting the expiration date
            optionRight = right;

            // We initialize a new Wrapper
            singleWrapper = new OSWrapper(this, priceField, optionField);

            // Constants setting for the Contracts
            contractDefinition = new Contract();
            contractDefinition.SecType = typeStock;
            contractDefinition.Exchange = contractExchange;
            contractDefinition.Currency = contractCurrency;
            contractDefinition.Multiplier = contractMultiplier;
            contractDefinition.IncludeExpired = contractIncludeExpired;

            // We set the changing contract details
            contractDefinition.LastTradeDateOrContractMonth = expirationDate.ToString("yyyyMMdd");
            contractDefinition.Right = optionRight;

            this.ConnectionToTheServer(); // Connection to Server

            StartStrategyProcedure(tickersList[tickerListCounter]);
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

        // This method has the sequence for all the data collecting
        public void StartStrategyProcedure(string currentTicker)
        {
            connectionTK = tickerListCounter; // Add index to create new ConnectionID
            tickerSymbol = currentTicker; // Up to now we didn't know which ticker to process
            contractDefinition.Symbol = tickerSymbol; // We set up the contract definition
            contractDefinition.Strike = 0; // We need to reset the strike to 0

            // Creating a TickerData and adding it to the dictionary
            TickerData tickerData = new TickerData(parentReference, tickerSymbol);
            tickerDataDict.Add(connectionTK, tickerData);

            // we ask for the underlying
            contractDefinition.SecType = typeStock;
            this.RequestTickerPrice(); // Asking for the price
            contractDefinition.SecType = typeOption; // Changing to Option from now on
            this.RequestContractDetails(); // Asking for contract details
        }

        // Request for ticker Price
        private void RequestTickerPrice()
        {
           singleWrapper.ClientSocket.reqMktData(CalculateConnectionID() +1, contractDefinition, "", true, null); // This is to ask for the price, ConnectionID +1, so has an ending different from all others and we can distinguish it
        }

        // Requests for Contracts Details
        private void RequestContractDetails()
        {
            singleWrapper.ClientSocket.reqContractDetails(CalculateConnectionID(), contractDefinition);// We ask for the list of Strike Prices
        }

        // Request Market data for specific strike
        private void RequestMarketdata(int i)
        {
            contractDefinition.Strike = strikesList[i]; // Setup Strike Price
            connectionSP = (int)(strikesList[i] * multiplierSP); // Unique connectionID, need to multiply here instead the formula or I'm going to miss the decimal point
            singleWrapper.ClientSocket.reqMktData(CalculateConnectionID(), contractDefinition, "", true, null); // Request Market Data
        }

        // UTILITY METHODS
        //*************************************************************************************************
        // Method to calculate the current connectionID
        private int CalculateConnectionID()
        {
            int connectionIDFinal = (connectionDH * multiplierDH) + (connectionTK * multiplierTK) + (connectionSP); // This should create an unique connectionID for each datahandler, ticker and strikeprice
            return connectionIDFinal;
        }
        
        // Calculate the TickerID from the ConnectionID
        private int CalculateTickerDataID(int connectionID)
        {
            int dhPart = (int)Math.Floor((double)(connectionID / multiplierDH)) * (multiplierDH / multiplierTK); // Eliminates everything but the DH identifier
            int tkPart = (int)Math.Floor((double)(connectionID / multiplierTK)) - dhPart; // Eliminates everything but TK identifier

            return tkPart;
        }

        // Calculate the StrikePrice from the ConnectionID
        private double CalculateStrikePriceID(int connectionID)
        {
            int tkPart = (int)Math.Floor((double)(connectionID / multiplierTK)) * multiplierTK; // Eliminates everything but the TK identifier
            double spPart = (double)(connectionID - tkPart) / multiplierSP; // Eliminates everything but SP identifier

            return spPart;
        }

        // Check if the request is fr a ticker price or an option
        private int CheckTickerPriceRequest(int connectionID)
        {
            int idPart = (int)Math.Floor((double)(connectionID / multiplierSP)) * multiplierSP; // turns the last digit into 0
            int result = connectionID - idPart; // if connectionID is a price request result should be 1, otherwise 0

            return result;

        }

        //RETREIVERS
        //**************************************************************************************************
        // Strikes List retreiver
        public void SetStrikesList(int connectionID, double[] incomingStrikesList)
        {
            strikesList = incomingStrikesList;

            TickerData activeTD;
            int tickerDataIdentifier = CalculateTickerDataID(connectionID); // Find TD key
            tickerDataDict.TryGetValue(tickerDataIdentifier, out activeTD); // Get TD

            activeTD.SetStrikePricesNumber(incomingStrikesList.GetUpperBound(0) + 1);
            strikeListCounter = 0;
            RequestMarketdata(strikeListCounter);
            strikeListCounter++;
        }

        // Delta and Bid retreiver
        public void SetBidAndDelta(int connectionID, double[] bidAndDelta)
        {
            // Find the relevant TickerData
            TickerData activeTD;
            int tickerDataIdentifier = CalculateTickerDataID(connectionID); // Find TD key
            tickerDataDict.TryGetValue(tickerDataIdentifier, out activeTD); // Get TD
            if (CheckTickerPriceRequest(connectionID) == 1)// If is a price request set the TickerData price
            {
                activeTD.SetPrice(bidAndDelta[1]); // Adding the bid
            }
            else // Means is a strike price bid&delta
            {
                activeTD.SetStrike(CalculateStrikePriceID(connectionID), bidAndDelta); // add to the list

                // Check if it was the last strike price
                if (strikeListCounter <= strikesList.GetUpperBound(0))
                {
                    RequestMarketdata(strikeListCounter);
                    strikeListCounter++;
                }
                else // If last we are done
                {
                    // Send the TickerData back to the main form
                    SetStrikesTable(activeTD);
                    
                }
            }
        }

        private double[,] condenseStrikesTable()
        {
            double[,] strikesTable = { { 0,1} , {0,1 } };
            int relevantStrikes = 0;
            int j = 0;
            for(int i =0; i <= strikesList.GetUpperBound(0); i++)
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
        public void SetStrikesTable(TickerData incomingTickerData)
        {
            // We need to send the TickerData in pieces because we cant send it whole to frmMain (accessibility conflic)
            parentReference.SetStrikes(incomingTickerData.tickerSymbol, incomingTickerData.tickerPrice, incomingTickerData.tickerStrikes);// Send data to frmMain
            strikeListCounter = 0;
            if(tickerListCounter < tickersList.GetUpperBound(0))
            {
                tickerListCounter++;
                StartStrategyProcedure(tickersList[tickerListCounter]);
            }
            
        }

    }
}
