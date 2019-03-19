using MonerisTransactionBAL;
using System;

namespace NetCoreSampleAPP
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            GoooglePurchase();
        }
        private static void GoooglePurchase()
        {
            string signedMessage = @"{""encryptedMessage"":""vXNLIj5wAZLiwno4 + S15beZiXqPiy5fcdElZC40s0kiVfMLAbb4pd0HPo79F + KO8wVwmicVoktih + YVQCn0EiDZpVKRSeoUFF3N9ZqHWVY8Rf6hZ8skcn3uAM8o4KdcQJEsXFm6AvohYjPwyTk7jlo3pDZtqnHbs + RnzAIQ3Zg7X6 + JaM / YlyVcHSMtUmcV87G1OgRjpBDPwavI9aF1JiVGnjOk9KBwEm24jQJuzswv9reyRlS6g49WE7WBZHjSkG2eLEo + dUZORo3pO4kTHwitXEPxXoeeLd / JJsSi4cBO22b2CHSltttaNnFOoicL5QJNKG7r2U2HxQT6LQ3LYD4ivH / 1sING61KH5Kwid9bgChtqGxjMB7reBNfrozOFOONklPe7DNmW + cfQ6NmoTdVsvkDH11yxf0SIOWEfxfWGoO + c20rj07bXCZdBVANN4YQw\u003d"",""ephemeralPublicKey"":""BGUnGL0ncAi0ZDX1Sqg6Wx25hOpqdRSJSB2guaeIJXYUX0N + LtU + 5KMxKKB1TPba7DsLcB8QOBxmqjI1Hm7bTB8\u003d"",""tag"":""TxPBUbgL / sjC1QnM7qdOzeUmYzy / 1NGB1vy8HsZmgJ8\u003d""}";
            APITransacton apiT = new APITransacton();
            TransactionBase t = new TransactionBase();
            t.setQAEnvironment();
            //t._aPIToken = "hurgle";
            t.transactionType = TransactionBase.TransactionType.GooglePayPurchase;
            t.OrderId = "Purchase_15518123378jdxZ4EyQYwAEFF";
            t.Environment = "QA";
           
            t.googlePay_PaymentToken = new TransactionBase.GooglePay_PaymentToken()
            {
                signature = "MEUCIEMGWvKHXKhypHI3YQxbAO+mK0vA08ZmWg5RJ/zzT6SwAiEA1pH07JBaRcEMiEjvTYqLLTxopMpiJxXro30Pz5EUsic=",
                protocolVersion = "ECv1",
                signedMessage = signedMessage.Replace(" ", string.Empty)
            };
            string Response = (apiT.PerformTransaction(t, "", "", "TC1"));
            Console.WriteLine("Google Purchase" + Response);
        }
    }
}
