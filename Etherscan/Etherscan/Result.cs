using Newtonsoft.Json;
using System;

namespace Etherscan
{
    public class Result
    {
        [JsonProperty("blockNumber")]
        public string BlockNumber { get; set; }

        [JsonProperty("timeStamp")]
        public string TimeStamp { get; set; }

        [JsonProperty("hash")]
        public string Hash { get; set; }

        [JsonProperty("from")]
        public string From { get; set; }

        [JsonProperty("to")]
        public string To { get; set; }

        [JsonProperty("value")]
        public string Value { get; set; }

        [JsonProperty("contractAddress")]
        public string ContractAddress { get; set; }

        [JsonProperty("input")]
        public string Input { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("gas")]
        public string Gas { get; set; }

        [JsonProperty("gasUsed")]
        public string GasUsed { get; set; }

        [JsonProperty("traceId")]
        public string TraceId { get; set; }

        [JsonProperty("isError")]
        public string IsError { get; set; }

        [JsonProperty("errCode")]
        public string ErrCode { get; set; }

        [JsonIgnore]
        public DateTime DateReadFromFile { get; set; }

        [JsonIgnore]
        public decimal ValueReadFromFile { get; set; }

        [JsonIgnore]
        public DateTime TransactionDate => GetTransactionDateFromTimeStamp();

        [JsonIgnore]
        public decimal TransactionValue => GetTransactionEthValue(Value, 18); //wei 10^-18

        [JsonIgnore]
        public decimal GasEthValue => GetTransactionEthValue(Gas, 9); //gwei 10^-9

        [JsonIgnore]
        public decimal OpenseaReward { get; set; }

        private decimal GetTransactionEthValue(string value,double power)
        {
            if (string.IsNullOrEmpty(value))
                return ValueReadFromFile;

            return (decimal)(double.Parse(value) / Math.Pow(10, power));
        }

        public DateTime GetTransactionDateFromTimeStamp()
        {
            if (string.IsNullOrEmpty(TimeStamp))
                return DateReadFromFile;

            // Unix timestamp is seconds past epoch
            DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTime = dateTime.AddSeconds(double.Parse(TimeStamp)).ToLocalTime();
            return dateTime;
        }
    }
}
