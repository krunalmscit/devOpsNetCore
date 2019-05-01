using MonerisTransactionBAL;
using NUnit.Framework;
using System;

namespace Tests
{
    public class TestPurchase
    {

        TransactionBase t = new TransactionBase();
        APITransacton apiTransaction = new APITransacton();
        string testCaseVersion = "DLL v1020 -1 ";
        Random r = new Random();
        DateTime now = DateTime.Now;
        
        string preOrderID, preTxnNumber, preAmount;
        public string Response { get; set; }
        public string TestcaseVersion => "V1.0";
        public string ProjectName => ".Net Standard 2.0 v1";


        [SetUp]
        public void Intialize()
        {
            t.setQAEnvironment();
            t.PAN = "4242424242424242";
            t.ExpDate = now.ToString("yy") + "12";
            t.CVD = "123";
            t.CrtpyType = "7";
            
        }

        [Test, Order(1)]
        public void Purchase()
        {
            t.transactionType = TransactionBase.TransactionType.Purchase;
            t.OrderId = t.OrderId = "Moneris_Purchase_VISA" + now.Year + now.ToString("MM") + now.ToString("dd") + "_" + now.Hour + now.Minute + now.Second + r.Next(0, 999999);
            t.Amount = "1.00";
            Response = (apiTransaction.PerformTransaction(t, ProjectName, testCaseVersion, "TC1"));
            Assert.IsTrue(apiTransaction.IsValidTransaction);
            
            preTxnNumber = apiTransaction.TxnNumber;
            preOrderID = apiTransaction.OrderId;
            preAmount = apiTransaction.Amount;
            Console.Write("Purchase");
        }
    }
}