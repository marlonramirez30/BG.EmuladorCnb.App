using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BG.EmuladorCnb.App.App_Code
{
    public class DataFormat
    {
        private const short indexHeader = 6;
        private const short indexNone0 = 5;
        private const short indexDateTimeClient = 17;
        private const short indexTransactionType = 4;
        private const short indexErrorCodeTcpClient = 5;
        private const short indexNone1 = 8;
        private const short indexDateTimeServer = 14;
        private const short indexTerminalId = 10;
        private const short indexSequential = 10;
        private const short indexTransactionState = 4;

        public static string getTrxLog(
          DataFormat.TransactionType type,
          short errorCodeTcpClient,
          string dateTimeServer,
          string terminalId,
          string sequential,
          DataFormat.TransactionStateType stateType)
        {
            string empty1 = string.Empty;
            string empty2 = string.Empty;
            return empty1 + "NBCLOG".PadRight(6, ' ') + empty2.PadRight(5, '0') + DateTime.Now.ToString("yyyyMMddHHmmssfff").PadLeft(17, '0') + ((int)type).ToString().PadLeft(4, '0') + errorCodeTcpClient.ToString().PadLeft(5, '0') + empty2.PadLeft(8, '0') + dateTimeServer.PadLeft(14, '0') + terminalId.PadLeft(10, '0') + sequential.PadLeft(10, '0') + ((int)stateType).ToString().PadLeft(4, '0');
        }

        public enum TransactionType
        {
            ResponseServer = 1,
        }

        public enum TransactionStateType
        {
            Ok = 2,
            Reverse = 3,
        }
    }
}
