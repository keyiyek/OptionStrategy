using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IBApi;

namespace OptionStrategy
{
    // Here we Override what we need from the Wrapper
    class OSWrapper2 : AbstractIBWrapper
    {
        //GLOBAL VARIABLES
        //*********************************************************************************
        // The creator DataHandler2
        DataHandler2 parentDataHandler2;
        // The data coming from the servers
        private Dictionary<int, List<double>> contractsStrike = new Dictionary<int, List<double>>();
        private double incomingBid;
        private double incomingDelta;

        private int tickPriceField; //This to set the kind of data you want: 1=Bid, 2=Ask, 4=Last, 9=Closed
        private int tickOptionComputationField;// 10=Bid, 11=Ask, 12=Last, 13=Model

        public OSWrapper2(DataHandler2 parentClass, int priceField, int optionField)
        {
            parentDataHandler2 = parentClass;
            tickPriceField = priceField; //This to set the kind of data you want: 1=Bid, 2=Ask, 4=Last, 9=Closed
            tickOptionComputationField = optionField;// 10=Bid, 11=Ask, 12=Last, 13=Model

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
        }

        // Finished getting Contracts Details
        public override void contractDetailsEnd(int reqId)
        {
            //base.contractDetailsEnd(reqId);
            parentDataHandler2.SetStrikesList(reqId, contractsStrike[reqId]);

        }

        // Tick Price
        public override void tickPrice(int tickerId, int field, double price, int canAutoExecute)
        {
            //base.tickPrice(tickerId, field, price, canAutoExecute);
            // If bid price
            if (field == tickPriceField)// 1=Bid, 2=Ask, 4=Last, 9=Closed
            {
                incomingBid = price;
            }

        }

        // Delta
        public override void tickOptionComputation(int tickerId, int field, double impliedVolatility, double delta, double optPrice, double pvDividend, double gamma, double vega, double theta, double undPrice)
        {
            //base.tickOptionComputation(tickerId, field, impliedVolatility, delta, optPrice, pvDividend, gamma, vega, theta, undPrice);

            if (field == tickOptionComputationField)// 10=Bid, 11=Ask, 12=Last, 13=Model
            {
                incomingDelta = delta;
            }
        }

        // Finished getting tick data
        public override void tickSnapshotEnd(int tickerId)
        {
            //base.tickSnapshotEnd(tickerId);
            double[] bidAndDelta = new double[] { incomingDelta, incomingBid };// Actually should be DeltaAndBid since they come the other way around
            parentDataHandler2.SetBidAndDelta(tickerId, bidAndDelta);
            incomingBid = 0; // Reset the data, in case next one isn't found
            incomingDelta = 0; // Reset the data, in case next one isn't found
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
