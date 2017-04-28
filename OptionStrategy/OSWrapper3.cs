using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IBApi;
using System.Diagnostics;

namespace OptionStrategy
{
    // Here we Override what we need from the Wrapper
    class OSWrapper3 : AbstractIBWrapper
    {
        //GLOBAL VARIABLES
        //*********************************************************************************
        // The creator DataHandler2
        frmMain parentDataHandler3;
        // The data coming from the servers
        private Dictionary<int, List<double>> contractsStrike = new Dictionary<int, List<double>>();

        public int tickPriceField;//This to set the kind of data you want: 1=Bid, 2=Ask, 4=Last, 9=Closed
        private int tickOptionComputationField;// 10=Bid, 11=Ask, 12=Last, 13=Model

        public OSWrapper3(frmMain parentClass)
        {
            parentDataHandler3 = parentClass;
            tickPriceField = 66;// priceField; //This to set the kind of data you want: 1=Bid, 2=Ask, 4=Last, 9=Closed, Delayed data 66 Bid, 67 Ask, 68 Last, 72 Highest, 73 Lowest
            tickOptionComputationField = 83;// optionField;// 10=Bid, 11=Ask, 12=Last, 13=Model, Delayed 80 Bid, 81 Ask, 82 Last, 83 Model

            Console.WriteLine("Wrapper: Tick Field - " + tickPriceField + "Tick Option - " + tickOptionComputationField);

        }

        //OVERRIDEN METHODS ARE ALSO DELEGATES FROM DataHandler2
        //*********************************************************************************
        // Contract Details
        public override void contractDetails(int reqId, ContractDetails contractDetails)
        {
            //base.contractDetails(reqId, contractDetails);
            // If the key already exists just add the strike
            if(contractsStrike.ContainsKey(reqId))
            {
                contractsStrike[reqId].Add(contractDetails.Summary.Strike);
            }
            else // If not add the key too
            {
                List<double> tempStrikeList = new List<double>();
                tempStrikeList.Add(contractDetails.Summary.Strike);
                contractsStrike.Add(reqId, tempStrikeList);
            }
            Console.WriteLine("Contract Details reqId: " + reqId + " Contract Details: " + contractDetails);
        }

        // Finished getting Contracts Details
        public override void contractDetailsEnd(int reqId)
        {
            //base.contractDetailsEnd(reqId);
            //parentDataHandler3.SetStrikesList(reqId, contractsStrike[reqId]);

            Console.WriteLine("Contracts End: reqId - " + reqId);
        }

        // Tick Price
        public override void tickPrice(int tickerId, int field, double price, int canAutoExecute)
        {
            //base.tickPrice(tickerId, field, price, canAutoExecute);
            // If bid price
            if (field == tickPriceField)// 1=Bid, 2=Ask, 4=Last, 9=Closed, Delayed data 66 Bid, 67 Ask, 68 Last, 72 Highest, 73 Lowest
            {
                Console.WriteLine("field " + field + " price " + price);
            }

            Console.WriteLine("Ticker Price: tickerId - " + tickerId + " Field - " + field + " Price - " + price);

        }

        // Delta
        public override void tickOptionComputation(int tickerId, int field, double impliedVolatility, double delta, double optPrice, double pvDividend, double gamma, double vega, double theta, double undPrice)
        {
            //base.tickOptionComputation(tickerId, field, impliedVolatility, delta, optPrice, pvDividend, gamma, vega, theta, undPrice);

            if (field == tickOptionComputationField)// 10=Bid, 11=Ask, 12=Last, 13=Model, Delayed 80 Bid, 81 Ask, 82 Last, 83 Model
            {
                //parentDataHandler2.SetBidOrDelta(tickerId, 0, field, Math.Abs(delta)); // we need the absolute value because PUT's deltas are negative
            }
            Console.WriteLine("Option Computation: tickerId - " + tickerId + " Field - " + field + " delta - " + delta + " Option Price - " + optPrice + " Underlying Price - " + undPrice);

        }

        // Error Handling
        public override void error(int id, int errorCode, string errorMsg)
        {
            // If no contract is found just skip over
            if (errorCode == 200)
            {
                tickSnapshotEnd(id);
            }
        }

    }
}
