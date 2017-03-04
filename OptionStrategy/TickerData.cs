using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OptionStrategy
{
    class TickerData
    {
        // Global Variables
        // Some are public because I had to send them back to frmMain, and couldn't send the whole TickerData
        private frmMain originalForm;
        public string tickerSymbol; // name
        public double tickerPrice; // Price
        private int tickerStrikePricesNumber; // how many strike prices are there
        public Dictionary<double, double[]> tickerStrikes = new Dictionary<double, double[]>();// The list of the strike Prices as Key, delta and bid as content
        
        // Init giving it the relevant ticker
        public TickerData(frmMain mainForm, string tickerName)
        {
            originalForm = mainForm;
            tickerSymbol = tickerName;
        }

        // SETTERS
        //******************************************************************
        // setting how many strike prices to expect
        public void SetStrikePricesNumber(int strikericesNumber)
        {
            tickerStrikePricesNumber = strikericesNumber;
        }

        // Setting the price
        public void SetPrice(double price)
        {
            tickerPrice = price;// Price
            if (CheckIfCompleted()) { this.dataComplete(); } // Check if all the data is here, if so call Completed method
        }

        public void SetStrike(double strike, double[] strikeScenario)
        {
            // Check for repetition in strike prices
            if (!tickerStrikes.ContainsKey(strike))
            {
                tickerStrikes.Add(strike, strikeScenario);// Strike with bid and delta
                if (CheckIfCompleted()) { this.dataComplete(); } // Check if all the data is here, if so call Completed method
            }
        }


        // UTILITY METHODS
        //**********************************************************************
        // Check if price is present and all the strikes have come back
        private bool CheckIfCompleted()
        {
            return (tickerPrice != 0 && tickerStrikes.Count == tickerStrikePricesNumber && tickerStrikePricesNumber != 0);
        }

        // SENDERS
        //*************************************************************************
        // Send data were relevant
        public void dataComplete()
        {
            originalForm.SetStrikes(this.tickerSymbol, this.tickerPrice, this.tickerStrikes);
        }

    }
}
