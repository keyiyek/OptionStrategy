using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

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
        private int tickersCompleted;
        public SortedDictionary<double, double[]> tickerStrikes = new SortedDictionary<double, double[]>();// The list of the strike Prices as Key, delta and bid as content

        private bool parallelComputing;
        
        // Init giving it the relevant ticker
        public TickerData(frmMain mainForm, string tickerName, bool computingInParallel)
        {
            originalForm = mainForm;
            tickerSymbol = tickerName;
            parallelComputing = computingInParallel;
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
            //this.dataComplete();

            if (CheckIfCompleted()) { this.dataComplete(); } // Check if all the data is here, if so call Completed method
        }

        public void SetStrike(double strike, double[] incomingScenario)
        {
            // If the strike is present we already have bid or delta, now we add the other
            if (tickerStrikes.ContainsKey(strike))
            {
                tickerStrikes[strike][0] += incomingScenario[0];
                tickerStrikes[strike][1] += incomingScenario[1];
                tickersCompleted++; // We increment the numbers of the completed strikes
            }
            else // If not we add the new strike
            {
                tickerStrikes.Add(strike, incomingScenario);// Strike with bid and delta
            }
            if (CheckIfCompleted()) { this.dataComplete(); } // Check if all the data is here, if so call Completed method
            
        }


        // UTILITY METHODS
        //**********************************************************************
        // Check if price is present and all the strikes have come back
        private bool CheckIfCompleted()
        {
            // We chect to have: price, all the tickers, that the number of tickers has been set (otherwise it could be that first strike gets true), that we are in parallel computing and we nee to call the sender function
            Console.WriteLine("Tickers Completed: " + tickersCompleted + " of " + tickerStrikePricesNumber);
            return (tickerPrice != 0 && tickersCompleted == tickerStrikePricesNumber && tickerStrikePricesNumber != 0 && parallelComputing);


            // This is for testing when markets are closed
            //return (tickersCompleted == tickerStrikePricesNumber);
        }

        // Erase strike prices with bid or delta = 0
        public void DictionaryCleanUp()
        {
            // Create a list
            List<double> listOfKeysToRemove = new List<double>();

            // Check which entry has to be purged
            foreach (double key in tickerStrikes.Keys)
            {
                // if bid or delta are = 0
                if(tickerStrikes[key][0] == 0 || tickerStrikes[key][1] == 0)
                {
                    // Take note of the key, can't remove straightaway, because we are iterating on the dictionary
                    listOfKeysToRemove.Add(key);
                }
            }
            // Iterate on the list and remove the entry from dictionary
            foreach(double entry in listOfKeysToRemove)
            {
                tickerStrikes.Remove(entry);
            }
        }

        // SENDERS
        //*************************************************************************
        // Send data were relevant
        public void dataComplete()
        {
            //Thread.Sleep(300000);
            // First we clean the Dictionary

            this.DictionaryCleanUp();
            originalForm.SetStrikes(this.tickerSymbol, this.tickerPrice, this.tickerStrikes);
        }

    }
}
