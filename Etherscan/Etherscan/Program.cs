using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Etherscan
{
    class Program
    {
        private const int SelectedMonth = 7;
        private const int BlockRange = 250;

        static void Main(string[] args)
        {
            //do not forget to set the month and blockrange variable
            Task.Run(() => DoWork()).Wait();
        }

        private static async Task DoWork()
        {
            var transcations = await GetTransactions();

            WriteToFile(transcations, "2021_07_opensea.csv");
        }

        private static void WriteToFile(List<Result> transcations, string fileName)
        {
            try
            {
                //define the columns to write to the csv file
                using (var writer = new StreamWriter(fileName))
                {
                    var firstLine = true;
                    foreach (var transaction in transcations)
                    {
                        if (firstLine)
                        {
                            writer.WriteLine("BlockNumber;Hash;Date;To;SellerReward(Eth);TraceId");
                            firstLine = false;
                        }

                        writer.WriteLine($"{transaction.BlockNumber};{transaction.Hash};{transaction.TransactionDate.ToString("yyyy-MM-dd HH:mm")};{transaction.To};{transaction.TransactionValue};{transaction.TraceId}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                Console.ReadKey();
            }

            //waiting for the user to be able to copy the rows where transaction count exceeded the 10000
            Console.ReadKey();
        }

        public static async Task<List<Result>> GetTransactions()
        {
            //12737970 the first block on 1st of July 2021
            //12935812 the first block on 1st of August 2021
            //Timestamp: 118 days 1 hr ago(Jul-31 - 2021 10:00:14 PM + UTC) GMT + 2 Aug 01 00:00:14 
            //13135896 the first block on 1st of September 2021
            var transactionList = new List<Result>();
            var stillInSelectedMonth = true;
            var startingBlockNumber = 12737970;

            //in each loop we will collect the transactions from the given number of blocks
            //probably the number of transactions for this block count will be always under 10000
            while (stillInSelectedMonth)
            {
                try
                {
                    //collect the transactions from the ${BlockRange} block range
                    var etherApiWrapper = new EtherscanApiWrapper();
                    var getTransactionsResponse = await etherApiWrapper.GetTransactions(startingBlockNumber, startingBlockNumber + BlockRange);

                    //in case of a request exceeds the maximum result record count, we will fix it manually later, so log it to the console
                    if (getTransactionsResponse.Result.Count >= 10000)
                        Console.WriteLine($"Transaction count is over 10000 between the following blocks: {startingBlockNumber}-{startingBlockNumber + BlockRange}.");

                    startingBlockNumber += BlockRange+1; //the starting block in the next loop will be the following after the last block of previous loop

                    //for example: in case any of the transactions were performed in September, we  already collected every transaction from August, so we can stop the loop
                    if (getTransactionsResponse.Result.Any(t => t.TransactionDate.Month == SelectedMonth+1))
                        stillInSelectedMonth = false;

                    transactionList.AddRange(getTransactionsResponse.Result.Where(t => t.TransactionDate.Month == SelectedMonth));

                    Console.WriteLine($"Transactions were collected from the following blocks: {startingBlockNumber}-{startingBlockNumber + BlockRange}. Transaction count was: {getTransactionsResponse.Result.Count}. Summarized count is: {transactionList.Count}.");
                    //sleep for 100 milliseconds to avoid from 429 HTTP response (too many requests)
                    Thread.Sleep(100);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error happened during querying transactions from blocks: : {startingBlockNumber}-{startingBlockNumber + BlockRange}." + ex);
                }
            }

            return transactionList;
        }
    }
}
