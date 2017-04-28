using System;

namespace OptionStrategy
{
    /// <summary>
    /// Description of Formatter.
    /// </summary>

    // This class calculats and orders the data given a list of Strike Prices, Deltas and Bids
    public class Formatter  
    {
        // Some general storage Variable
        double[] probabilityDiffLine; // We need to remember the diff line to calculate the normal lines
        double[] averageList; // To calculate the averages we need to know the number of strikes
        double currentPrice; // To calculate ROC we need the price

        // When the Formatter is istantiated Price and number of Strike Prices are needed to setup some global information
        public Formatter(double price, int strikePricesCount)
        {
            currentPrice = price;
            averageList = new double[strikePricesCount];
        }

        // LINE BUILDERS
        //*************************************************************************************//
        // Next Section builds the lines, probably I'll make them Private and call them with a switch so I just need to call 1 method
        // Line = 0 scenarios list (strikes list), line = 1 Probability line (deltas)
        // Build the first line, the one with the Strike Prices
        // Build the second line, the one with the Deltas
        public double[] StrikeAndDeltaLines(double[,] resultTable, int line)
        {
            // Set the array lenght, +4 is because we need 3 blank column and 1 for the 0 from the Upper Bound
            double[] scenario = new double[resultTable.GetUpperBound(0) + 4];

            // Skip the first three columns that are blank
            scenario[0] = 0;
            scenario[1] = 0;
            scenario[2] = 0;

            // Repeat for all the Strike Prices
            for (int i = 3; i <= scenario.GetUpperBound(0); i++)
            {
                scenario[i] = resultTable[i - 3, line];// Add Strike Price
            }

            return scenario;
        }

        // Build the second line, the one with the deltas, it is like the previous one, could be merged
        // Deprecated
        public double[] ScenarioLine(double[,] resultTable)
        {
            // Set the array lenght, +4 is because we need 3 blank column and 1 for the 0 from the Upper Bound
            double[] scenario = new double[resultTable.GetUpperBound(0) + 4];

            // Skip the first three columns that are blank
            scenario[0] = 0;
            scenario[1] = 0;
            scenario[2] = 0;

            // Repeat for all the Strike Prices
            for (int i = 3; i <= scenario.GetUpperBound(0); i++)
            {
                scenario[i] = resultTable[i - 3, 0];// Add Strike Price
            }

            return scenario;
        }
        public double[] ProbabilityLine(double[,] resultTable)
        {
            // Set the array lenght, +4 is because we need 3 blank column and 1 for the 0 from the Upper Bound
            double[] delta = new double[resultTable.GetUpperBound(0) + 4];

            // Skip the first three columns
            delta[0] = 0;
            delta[1] = 0;
            delta[2] = 0;

            // Repeat for all the Strike Prices
            for (int i = 3; i <= delta.GetUpperBound(0); i++)
            {
                delta[i] = resultTable[i - 3, 1];
            }

            return delta;
        }

        // This is used with the Delta and Calls
        public double[] ProbabilityDiffLineDeltaCall(double[,] resultTable)
        {
            // Deltadiff are between and outside the strikes, so there is one more of them
            double[] deltaDiff = new double[resultTable.GetUpperBound(0) + 5]; // +1 for the UpperBound-Count discrepancy, +3 for the first three clolumn, + 1 for the "one more" from the above line

            // Skip the first three columns
            deltaDiff[0] = 0;
            deltaDiff[1] = 0;
            deltaDiff[2] = 0;

            // First one has no calculation because is the lower bound
            // It is set manualy otherwise the calculation would fire a "Index out of Range" Exception
            deltaDiff[3] = 1- resultTable[0, 1]; // Delta is the probability to stay ITM, we want the prob of staying lower

            for (int i = 4; i <= (deltaDiff.GetUpperBound(0) -1); i++)
            {
                deltaDiff[i] = (resultTable[i - 4, 1]) - (resultTable[i - 3, 1]); // Delta is decreasing so Lower Index - Higher Index
            }

            // Last number has no calculation cause is the upper bound
            // It is set manualy otherwise the calculation would fire a "Index out of Range" Exception
            deltaDiff[deltaDiff.GetUpperBound(0)] = resultTable[resultTable.GetUpperBound(0), 1];
            
            // Store the results also in a global Array, so thay can used in the calculations of the NormalLine method
            probabilityDiffLine = deltaDiff;

            return deltaDiff;
        }

        // This is used with the delta and Puts
        // For the put we want the probability to get lower, so is 1 - Delta
        public double[] ProbabilityDiffLineDeltaPut(double[,] resultTable)
        {
            // Deltadiff are between and outside the strikes, so there is one more of them
            double[] deltaDiff = new double[resultTable.GetUpperBound(0) + 5]; // +1 for the UpperBound-Count discrepancy, +3 for the first three clolumn, + 1 for the "one more" from the above line

            // Skip the first three columns
            deltaDiff[0] = 0;
            deltaDiff[1] = 0;
            deltaDiff[2] = 0;

            deltaDiff[3] = resultTable[0, 1]; // Delta is the probability to stay ITM, we want the prob of staying lower

            for (int i = 4; i < (deltaDiff.GetUpperBound(0) -1); i++)
            {
                deltaDiff[i] = resultTable[i - 3, 1] - resultTable[i - 4, 1]; // Delta is increasing so Higher Index - Lower Index
            }

            // Last number has no calculation cause is the upper bound
            // It is set manualy otherwise the calculation would fire a "Index out of Range" Exception
            deltaDiff[deltaDiff.GetUpperBound(0)] = (1 - resultTable[resultTable.GetUpperBound(0), 1]);

            // Store the results also in a global Array, so thay can used in the calculations of the NormalLine method
            probabilityDiffLine = deltaDiff;

            return deltaDiff;
        }

 
        // All the other lines
        public double[] NormalLineCall(double[,] resultTable, int line)
        {
            // Set up some local variables
            double lineAverage = 0; // Average that goes at the end
            // Deltadiff are between and outside the strikes, so there is one more of them
            double[] resultsLine = new double[resultTable.GetUpperBound(0) + 6]; // +1 for the UpperBound-Count discrepancy, +3 for the first three clolumn, + 1 for the "one more" from the above line, +1 because we have one more element at the end: the Average 

            // First two columns are data from the result Table
            resultsLine[0] = resultTable[line, 0]; // Set Strike Price
            resultsLine[1] = resultTable[line, 2]; // Set the bid
            resultsLine[2] = resultTable[line, 0] + resultTable[line, 2]; // Third column is the BE Price for Calls (Strike + Bid)

            // for all the Strike DeltaDiff
            for (int i = 3; i <= (resultTable.GetUpperBound(0)); i++) // -1 because the last one is the average
            {
                // Here is the main calculation
                // If Scenario Price is Higher than Strike Price the Option is Exersised
                // Money = ((Probable Scenario - Strike) + Bid) * Probability of Scenario
                if (resultTable[i - 3, 0] >= resultTable[line, 0])
                {
                    resultsLine[i] = (((resultTable[i - 3, 0] - ((resultTable[i -2, 0] - resultTable[i - 3, 0]) / 2)) - resultTable[line, 0]) + resultsLine[1]) * probabilityDiffLine[i];

                }
                // If Scenario is Higher then Strike Price the Option is NOT Exercised
                // Money = Bid * Probability of Scenario
                else
                {
                    resultsLine[i] = resultsLine[1] * probabilityDiffLine[i];
                }

                // Summ all the results to get the Average
                // You don't need to divide by number of Strike Prices because is a weighted Average, and the weights are the Probability of Scenario
                lineAverage += resultsLine[i];
            }

            // Last element is the Average
            resultsLine[resultsLine.GetUpperBound(0)] = lineAverage;
            averageList[line] = lineAverage; // Building a global array of averages, is used to find the MAX

            return resultsLine;


        }

        // All the other lines
        public double[] NormalLinePut(double[,] resultTable, int line)
        {
            // Set up some local variables
            double lineAverage = 0; // Average that goes at the end
            // Deltadiff are between and outside the strikes, so there is one more of them
            double[] resultsLine = new double[resultTable.GetUpperBound(0) + 6]; // +1 for the UpperBound-Count discrepancy, +3 for the first three clolumn, + 1 for the "one more" from the above line, +1 because we have one more element at the end: the Average 

            // First two columns are data from the result Table
            resultsLine[0] = resultTable[line, 0]; // Set Strike Price
            resultsLine[1] = resultTable[line, 2]; // Set the bid
            resultsLine[2] = resultTable[line, 0] - resultTable[line, 2]; // Third column is the BE Price for Puts (Strike - Bid)
            // TODO
            //resultsLine[2] = resultTable[line, 0] + resultTable[line, 2]; // Third column is the BE Price for Calls (Strike + Bid)

            // for all the Strike DeltaDiff
            for (int i = 3; i <= (resultsLine.GetUpperBound(0) - 1); i++) // -1 because the last one is the average
            {
                // Here is the main calculation
                // If Scenario Price is Lower than Strike Price the Option is Exersised
                // Money = ((Probable Scenario - Strike) + Bid) * Probability of Scenario
                if (resultTable[i - 3, 0] <= resultTable[line, 0])
                {
                    resultsLine[i] = ((resultTable[line, 0] - (resultTable[i - 2, 0] - ((resultTable[i - 2, 1] - resultTable[i - 3, 1]) / 2))) + resultsLine[1]) * probabilityDiffLine[i];

                }
                // If Scenario is Higher then Strike Price the Option is NOT Exercised
                // Money = Bid * Probability of Scenario
                else
                {
                    resultsLine[i] = resultsLine[1] * probabilityDiffLine[i];
                }

                // Summ all the results to get the Average
                // You don't need to divide by number of Strike Prices because is a weighted Average, and the weights are the Probability of Scenario
                lineAverage += resultsLine[i];
            }

            // Last element is the Average
            resultsLine[resultsLine.GetUpperBound(0)] = lineAverage;
            averageList[line] = lineAverage; // Building a global array of averages, is used to find the MAX

            return resultsLine;


        }

        //*************************************************************************************//

        // GENERAL CALCULATIONS
        //*************************************************************************************//
        // Here we find the best strategy value
        // This is needed to calculate the ROC so it is private
        private double MaxAverage()
        {
            // Set the Value to 0
            double maxValue = 0;
            // Search the whole Average array
            for (int i = 0; i < averageList.GetUpperBound(0); i++)
            {
                // If Current is higher than previous change value
                if (averageList[i] > maxValue)
                {
                    maxValue = averageList[i];
                }
            }
            return maxValue;
        }

        // Here we want the index of the best strategy, so we can find the relative Strike Price
        public int MaxIndex()
        {
            // Set the value to -1
            int maxIndex = -1;// this to be sure to ??? I can't remember why I thought it was a good idea.
            double maxValue = -1;// -1 otherwise if you have only 0s you will end with a -1 index
            // Search on array for max value
            for (int i = 0; i < averageList.GetUpperBound(0); i++)
            {
                // Iv current value is higher then take update value and index
                if (averageList[i] > maxValue)
                {
                    maxValue = averageList[i];
                    maxIndex = i;
                }
            }
            return maxIndex;
        }

        // Calculation of ROC
        public double Roc()
        {
            // Take the Max Average, divide by price, you get ROC
            double roc;
            return roc = MaxAverage() / currentPrice;
        }
        //*************************************************************************************//

    }
}
