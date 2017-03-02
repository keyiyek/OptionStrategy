using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OptionStrategy
{
    class TickerData
    {
        private string tickerSymbol;
        private double tickerPrice;
        private List<double[]> tickerStrikes = new List<double[]>();
        
        public TickerData(string tickerName)
        {
            tickerSymbol = tickerName;
        }

        public void SetPrice(double price)
        {
            tickerPrice = price;
        }

        public void SetStrike(double[] strikeScenario)
        {
            tickerStrikes.Add(strikeScenario);
        }

        public void dataComplete(frmMain parent)
        {

        }

    }
}
