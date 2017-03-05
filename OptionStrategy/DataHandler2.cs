using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using IBApi;

namespace OptionStrategy
{
    class DataHandler2
    {
        // Some Global Vars
        // Objects From Other Classes
        private int dataHandlerID; // Useful to tell them apart when sending back data to frmMain
        private frmMain parentReference; // To have a reference to send back data to frmMain
        private OSWrapper2 singleWrapper; // Need this to comunicate with IB servers
        private Contract contractDefinition; // Need this to store the info for a request
        private Dictionary<int, TickerData> tickerDataDict = new Dictionary<int, TickerData>();
        private string[] tickersList; // To store the list of tickers to be processed

        // CONSTANTS
        //************************************************
        // Formatters
        const bool parallelComputing = true;

        // Contract Definition
        const string contractExchange = "SMART";
        const string contractCurrency = "USD";
        const string contractMultiplier = "100";
        const bool contractIncludeExpired = false;

        // Connection Constants
        const string connectionHost = "";
        const int connectionPort = 4001;
        const int connectionExtraAuth = 0;

        // ConnectionID multipliers
        // They are used to build unique connectionIDs
        const int multiplierDH = 1000000; // First digit (1-8) is the DataHandler ID
        const int multiplierTK = 10000; // Second and Third digit are the ticker id. It works for up to 99 tickers per DataHandler
        const int multiplierSP = 10; // Last four digits are the strike price. Can handle strikes up to 999.5

        // ConnectionID Parts
        // These are the pieces the connectionID is made from
        int connectionDH = 0;
        int connectionTK = 0;
        double connectionSP = 0;



        // Initialization
        // ClientID tells all the DataHAndlers apart
        // parentClass to send back information
        // DateTime, Right are defined in frmMain, so easy to have them setup here
        // tickers the list of the tickers assigned to the DataHandler
        public DataHandler2(int clientID, frmMain parentClass, DateTime incomingExpirationDate, string incomingRight, string[] incomingTickers, int priceField, int optionField)
        {
            dataHandlerID = clientID;//Define the ID of the DataHandler
            connectionDH = dataHandlerID; // Setting the first

            // Storing a reference to frmMain
            parentReference = parentClass;

            // Setting up the Contract definition
            this.SetUpContract(incomingExpirationDate, incomingRight);

            // Storing the tickers list
            tickersList = incomingTickers;

            // Wrapper, connection and requests
            singleWrapper = new OSWrapper2(this, priceField, optionField);
            this.ConnectionToTheServer();
            this.requestingData();

        }

        // REQUESTING METHODS
        //********************************************************************************
        // Main sequence method
        private void requestingData()
        {
            //Starting  with all the tickers in parallel
            for (int i = 0; i <= tickersList.GetUpperBound(0); i++)
            {
                connectionTK = i; // Add index to create new ConnectionID
                contractDefinition.Symbol = tickersList[i]; // We set up the contract definition with the ticker symbol

                // Creating a TickerData and adding it to the dictionary
                TickerData tickerData = new TickerData(parentReference, tickersList[i], parallelComputing);
                tickerDataDict.Add(connectionTK, tickerData);

                // we ask for the underlying
                contractDefinition.SecType = Properties.Resources.typeStock;
                this.RequestTickerPrice(); // Asking for the price
                contractDefinition.SecType = Properties.Resources.typeOption; // Changing to Option from now on
                this.RequestContractDetails(); // Asking for contract details

            }
        }

        // Request for ticker Price
        private void RequestTickerPrice()
        {
            singleWrapper.ClientSocket.reqMktData(CalculateConnectionID() + 1, contractDefinition, "", true, null); // This is to ask for the price, ConnectionID +1, so has an ending (1) different from all others (0 or 5) and we can distinguish it
        }

        // Requests for Contracts Details
        private void RequestContractDetails()
        {
            singleWrapper.ClientSocket.reqContractDetails(CalculateConnectionID(), contractDefinition);// We ask for the list of Strike Prices
        }

        // Request for Market Data on Options, this function needs the ConnectionID to understand what Ticker is relevant 
        private void RequestMarketData(int connectionID, double strikePrice)
        {

            // Unique ID building
            connectionTK = CalculateTickerDataID(connectionID); // Need to reset this everytime otherwise, with multiple tickers, it would get messed up
            connectionSP = strikePrice;

            // Contract has to be reset as well
            contractDefinition.Strike = strikePrice; // Setup Strike Price
            contractDefinition.Symbol = tickersList[connectionTK];


            // Calling for the data
            singleWrapper.ClientSocket.reqMktData(CalculateConnectionID(), contractDefinition, "", true, null);

        }

        // RETREIVERS
        //********************************************************************************
        // These are methods called from the Wrapper
        // Incoming strike prices list
        public void SetStrikesList(int connectionID, List<double> incomingStrikesList)
        {
            TickerData activeTD = GetRelevantTickerData(connectionID);// Find the relevant TickerData
            activeTD.SetStrikePricesNumber(incomingStrikesList.Count);// Set its strike price counter (maybe a +1 is missing here)

            // Asking for all the strikes, in short sequence
            foreach(double incomingStrike in incomingStrikesList)
            {
                RequestMarketData(connectionID, incomingStrike);
            }

        }

        public void SetBidAndDelta(int connectionID, double[] bidAndDelta)
        {
            // Ser a reference to the relevant TickerData
            TickerData activeTD = GetRelevantTickerData(connectionID);

            // If it is a price request set the TickerData price
            if (CheckTickerPriceRequest(connectionID) == 1)
            {
                activeTD.SetPrice(bidAndDelta[1]); // Adding the bid part of bidAndDelta
            }
            else // Means is a strike price bid&delta
            {
                activeTD.SetStrike(CalculateStrikePriceID(connectionID), bidAndDelta); // add bidAndDelta (value) to the Dictionary, with strike price as key
            }
        }

        // UTILITY METHODS
        //********************************************************************************
        // This just sets up the Contract, just to keep the initializer tidier
        private void SetUpContract(DateTime expirationDate, string right)
        {
            // Setting Up Contract
            contractDefinition = new Contract();
            contractDefinition.SecType = Properties.Resources.typeStock;
            contractDefinition.Exchange = contractExchange;
            contractDefinition.Currency = contractCurrency;
            contractDefinition.Multiplier = contractMultiplier;
            contractDefinition.IncludeExpired = contractIncludeExpired;

            contractDefinition.LastTradeDateOrContractMonth = expirationDate.ToString(Properties.Resources.dateFormat);
            contractDefinition.Right = right;
        }

        // Conenction to the server
        private void ConnectionToTheServer()
        {
            // Connection
            singleWrapper.ClientSocket.eConnect(connectionHost, connectionPort, dataHandlerID);
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

        // Disconnect from the server
        public void DisconnectionFromTheServer()
        {
            singleWrapper.ClientSocket.eDisconnect();
        }

        // Here we generate the unique connection ID
        private int CalculateConnectionID()
        {
            int connectionIDFinal = (connectionDH * multiplierDH) + (connectionTK * multiplierTK) + (int)(connectionSP * multiplierSP); // This should create an unique connectionID for each datahandler, ticker and strikeprice
            return connectionIDFinal;
        }

        // From the ConnectionID we estrapolate the TickerDataID
        private int CalculateTickerDataID(int connectionID)
        {
            int dhPart = (int)Math.Floor((double)(connectionID / multiplierDH)) * (multiplierDH / multiplierTK); // Eliminates everything but the DH identifier
            int tkPart = (int)Math.Floor((double)(connectionID / multiplierTK)) - dhPart; // Eliminates everything but TK identifier

            return tkPart;
        }

        // From the ConnectionID we estrapolate the Strike Price value
        private double CalculateStrikePriceID(int connectionID)
        {
            int tkPart = (int)Math.Floor((double)(connectionID / multiplierTK)) * multiplierTK; // Eliminates everything but the TK identifier
            double spPart = (double)(connectionID - tkPart) / multiplierSP; // Eliminates everything but SP identifier

            return spPart;
        }

        // From the ConnectionID we estrapolate if is a Ticker Price request (return 1) or a Option detail request (return 0)
        private int CheckTickerPriceRequest(int connectionID)
        {
            int idPart = (int)Math.Floor((double)(connectionID / multiplierSP)) * multiplierSP; // turns the last digit into 0
            int result = connectionID - idPart; // if connectionID is a price request result should be 1, otherwise 0

            return result;

        }

        // Returns the relevant TickerData, given a connectionID
        private TickerData GetRelevantTickerData(int connectionID)
        {
            TickerData relevantTD;

            int tickerDataIdentifier = CalculateTickerDataID(connectionID); // Find TD key
            tickerDataDict.TryGetValue(tickerDataIdentifier, out relevantTD); // Get TD

            return relevantTD;
        }
    }
}
