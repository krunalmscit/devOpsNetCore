using MonerisTransactionBAL;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace TransactionTest
{
    [TestFixture]
    class TestMCPTransactions
    {
        TransactionBase t = new TransactionBase();
        APITransacton apiTransaction = new APITransacton();
        string testCaseVersion = "DLL v1020 -1 ";
        Random r = new Random();
        DateTime now = DateTime.Now;

        public string PurchaseRateToken { get; set; }

        string preOrderID, preTxnNumber, preAmount;
        public string Response { get; set; }
        public string TestcaseVersion => "V1.0";
        public string ProjectName => ".Net Standard 2.0 v1 DLL 1020";
        string[] cardHolderAmount = { "500", "840" };
        string[] merchantSettlementAmount = { "500", "840" };

        [SetUp]
        public void Intialize()
        {
            t.setQAEnvironment();
            t.PAN = "4242424242424242";
            t.ExpDate = now.ToString("yy") + "12";
            t.CVD = "123";
            t.CrtpyType = "7";
           
            //t.SetMCPRate(cardHolderAmount, merchantSettlementAmount);
            t.McpVersion = "1.00";
        }

        [Test, Order(1)]
        public void GetRate()
        {
            t.transactionType = TransactionBase.TransactionType.MCPGetRate;
            t.OrderId = t.OrderId = "MCPGetRateForPurchse" + now.Year + now.ToString("MM") + now.ToString("dd") + "_" + now.Hour + now.Minute + now.Second + r.Next(0, 999999);
            t.MCPRateTransactionType = "P";
            t.CardHolderAmount = cardHolderAmount;
            t.MerchantSettlementAmount = merchantSettlementAmount;
            Response = (apiTransaction.PerformTransaction(t, ProjectName, testCaseVersion, "TC1"));
            Assert.IsTrue(apiTransaction.IsValidTransaction);
            PurchaseRateToken = apiTransaction.MCPRateToken;
            //preTxnNumber = apiTransaction.TxnNumber;
            //preOrderID = apiTransaction.OrderId;
            //preAmount = apiTransaction.Amount;
            Console.Write("Purchase");
        }

        [Test, Order(2)]
        public void MultiCurrencyPurchaseWithCurrencyToken()
        {
            t.transactionType = TransactionBase.TransactionType.MCPPurchase;
            t.OrderId = t.OrderId = "MCPPurchase" + now.Year + now.ToString("MM") + now.ToString("dd") + "_" + now.Hour + now.Minute + now.Second + r.Next(0, 999999);
            t.MCPRateToken = PurchaseRateToken;
            t.MCPCardholderAmount = "500";
            t.MCPCardHolderCurrncy = "840";
            t.MCPRateToken = PurchaseRateToken;
            Response = (apiTransaction.PerformTransaction(t, ProjectName, testCaseVersion, "TC2"));
            Assert.IsTrue(apiTransaction.IsValidTransaction);

        }
    }
}
