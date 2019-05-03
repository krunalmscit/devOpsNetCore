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
        public string DataKey { get; private set; }

        public string TestcaseVersion => "V1.0";
        public string ProjectName => ".Net Standard 2.0 v1 DLL 1020";

        public string RefundRateToken { get;  set; }

        string[] cardHolderAmount = { "500", "840" };
        string[] cardHolderAmountWithInvlidCountyrCode = { "500", "000" };
        string[] merchantSettlementAmount = { "500", "840" };
        string ExpiredPurchaseToken = "P1536674945816948";

        [SetUp]
        public void Intialize()
        {
            t.setQAEnvironment();
            t.PAN = "4242424242424242";
            t.ExpDate = now.ToString("yy") + "12";
            t.CVD = "123";
            t.CrtpyType = "7";            
            t.McpVersion = "1.00";
            t.MCPCardholderAmount = "500";
            t.MCPCardHolderCurrncy = "840";
        }

        [Test, Order(1)]
        public void GetRateMCP()
        {
            t.transactionType = TransactionBase.TransactionType.MCPGetRate;
            t.OrderId = t.OrderId = "MCPGetRateForPurchse" + now.Year + now.ToString("MM") + now.ToString("dd") + "_" + now.Hour + now.Minute + now.Second + r.Next(0, 999999);
            t.MCPRateTransactionType = "P";
            t.CardHolderAmount = cardHolderAmount;
            t.MerchantSettlementAmount = merchantSettlementAmount;
            Response = (apiTransaction.PerformTransaction(t, ProjectName, testCaseVersion, "TC1"));
            Assert.IsTrue(apiTransaction.IsValidTransaction);
            PurchaseRateToken = apiTransaction.MCPRateToken;
        }

        [Test, Order(2)]
        public void PurchaseWithCurrencyTokenMCP()
        {
            t.transactionType = TransactionBase.TransactionType.MCPPurchase;
            t.OrderId = t.OrderId = "MCPPurchase" + now.Year + now.ToString("MM") + now.ToString("dd") + "_" + now.Hour + now.Minute + now.Second + r.Next(0, 999999);
            t.MCPRateToken = PurchaseRateToken;
            t.MCPCardholderAmount = "500";
            t.MCPCardHolderCurrncy = "840";
            Response = (apiTransaction.PerformTransaction(t, ProjectName, testCaseVersion, "TC2"));
            Assert.IsTrue(apiTransaction.IsValidTransaction);
            preOrderID = apiTransaction.OrderId;
            preTxnNumber = apiTransaction.TxnNumber;
        }

        [Test, Order(3)]
        public void PurchaseCorrcetionMCP()
        {
            t.transactionType = TransactionBase.TransactionType.MCPPurchaseCorrecetion;
            t.OrderId = preOrderID;
            t.TxnNumber = preTxnNumber;
            //t.MCPRateToken = PurchaseRateToken;
            t.MCPCardholderAmount = "500";
            t.MCPCardHolderCurrncy = "840";
            Response = (apiTransaction.PerformTransaction(t, ProjectName, testCaseVersion, "TC3"));
            Assert.IsTrue(apiTransaction.IsValidTransaction);
        }

        [Test, Order(4)]
        public void PreAuthWithRateTokenMCP()
        {
            t.transactionType = TransactionBase.TransactionType.MCPPreAuth;
            t.OrderId = t.OrderId = "MCPPurchase" + now.Year + now.ToString("MM") + now.ToString("dd") + "_" + now.Hour + now.Minute + now.Second + r.Next(0, 999999);
            t.MCPRateToken = PurchaseRateToken;
            t.MCPCardholderAmount = "500";
            t.MCPCardHolderCurrncy = "840";
            t.MCPRateToken = PurchaseRateToken;
            Response = (apiTransaction.PerformTransaction(t, ProjectName, testCaseVersion, "TC4"));
            preOrderID = apiTransaction.OrderId;
            preTxnNumber = apiTransaction.TxnNumber;
            Assert.IsTrue(apiTransaction.IsValidTransaction);
        }

        // 5 - Preauth completion , completion with different token

        [Test, Order(5)]
        public void PreAuthCompletionMCP()
        {
            t.transactionType = TransactionBase.TransactionType.MCPCompletion;
            t.OrderId = preOrderID;
            t.TxnNumber = preTxnNumber;
            t.MCPRateToken = PurchaseRateToken;
            t.MCPCardholderAmount = "500";
            t.MCPCardHolderCurrncy = "840";
            Response = (apiTransaction.PerformTransaction(t, ProjectName, testCaseVersion, "TC5"));
            Assert.IsTrue(apiTransaction.IsValidTransaction);
        }

        // Refund
        [Test]
        public void RefundWithRateTokenMCP()
        {
            t.transactionType = TransactionBase.TransactionType.MCPGetRate;
            t.OrderId = t.OrderId = "MCPGetRateForPurchse" + now.Year + now.ToString("MM") + now.ToString("dd") + "_" + now.Hour + now.Minute + now.Second + r.Next(0, 999999);
            t.MCPRateTransactionType = "R";
            t.CardHolderAmount = cardHolderAmount;
            t.MerchantSettlementAmount = merchantSettlementAmount;
            Response = (apiTransaction.PerformTransaction(t, ProjectName, testCaseVersion));
            Assert.IsTrue(apiTransaction.IsValidTransaction);
            RefundRateToken = apiTransaction.MCPRateToken;

            t.transactionType = TransactionBase.TransactionType.MCPPurchase;
            t.OrderId = t.OrderId = "MCPPurchase" + now.Year + now.ToString("MM") + now.ToString("dd") + "_" + now.Hour + now.Minute + now.Second + r.Next(0, 999999);
            t.MCPRateToken = PurchaseRateToken;
            t.MCPCardholderAmount = "500";
            t.MCPCardHolderCurrncy = "840";
            Response = (apiTransaction.PerformTransaction(t, ProjectName, testCaseVersion));
            Assert.IsTrue(apiTransaction.IsValidTransaction);
            preOrderID = apiTransaction.OrderId;
            preTxnNumber = apiTransaction.TxnNumber;

            if (!string.IsNullOrEmpty(RefundRateToken))
            {
                t.transactionType = TransactionBase.TransactionType.MCPRefund;
                t.MCPCardholderAmount = "500";
                t.MCPCardHolderCurrncy = "840";
                t.MCPRateToken = RefundRateToken;
                t.OrderId = preOrderID;
                t.TxnNumber = preTxnNumber;
                Response = (apiTransaction.PerformTransaction(t, ProjectName, testCaseVersion, "TC15"));
                Assert.IsTrue(apiTransaction.IsValidTransaction);
            }
        }
        // Refund with diffrent currency then purchase

        //Ind refund 

        // Res purchase with rate token

        // 9 - Res Pre auth with rate token
        // 10 - Res refund
        // 10.1 - res refund with purchase token - Expect 400 error on  response
        // 11 - Purchase with out token 

        [Test, Order(11)]
        public void PurchaseWithoutRateTokenMCP()
        {
            t.transactionType = TransactionBase.TransactionType.MCPPurchase;
            t.OrderId = t.OrderId = "MCPPurchase" + now.Year + now.ToString("MM") + now.ToString("dd") + "_" + now.Hour + now.Minute + now.Second + r.Next(0, 999999);
            t.MCPRateToken = PurchaseRateToken;
            t.MCPCardholderAmount = "500";
            t.MCPCardHolderCurrncy = "840";
            Response = (apiTransaction.PerformTransaction(t, ProjectName, testCaseVersion, "TC11"));
            Assert.IsTrue(apiTransaction.IsValidTransaction);
        }

        // 12 - Purchase correction, as purchas correction does not require token. No need to write this test case

        // 13 - Pre auth without token 
        [Test, Order(13)]
        public void PreAuthWithOutRateTokenMCP()
        {
            t.transactionType = TransactionBase.TransactionType.MCPPreAuth;
            t.OrderId = t.OrderId = "MCPPreAuth" + now.Year + now.ToString("MM") + now.ToString("dd") + "_" + now.Hour + now.Minute + now.Second + r.Next(0, 999999);
            t.MCPCardholderAmount = "500";
            t.MCPCardHolderCurrncy = "840";            
            Response = (apiTransaction.PerformTransaction(t, ProjectName, testCaseVersion, "TC13"));
            preOrderID = apiTransaction.OrderId;
            preTxnNumber = apiTransaction.TxnNumber;
            Assert.IsTrue(apiTransaction.IsValidTransaction);
        }

        // 14 - Completion with out Token 
        [Test, Order(14)]
        public void CompletionWithOutTokenMCP()
        {
            t.transactionType = TransactionBase.TransactionType.MCPCompletion;
            t.OrderId = preOrderID;
            t.TxnNumber = preTxnNumber;
            t.MCPCardholderAmount = "500";
            t.MCPCardHolderCurrncy = "840";
            t.MCPRateToken = PurchaseRateToken;
            Response = (apiTransaction.PerformTransaction(t, ProjectName, TestcaseVersion, "TC14"));
            Assert.IsTrue(apiTransaction.IsValidTransaction);
            preOrderID = apiTransaction.OriginalOrderID;
            preTxnNumber = apiTransaction.TxnNumber;
        }


        // 15 - Refund token
        [Test, Order(15)]
        public void RefundWithOutTokenMCP()
        {   

            t.transactionType = TransactionBase.TransactionType.MCPPurchase;
            t.OrderId = t.OrderId = "MCPPurchase" + now.Year + now.ToString("MM") + now.ToString("dd") + "_" + now.Hour + now.Minute + now.Second + r.Next(0, 999999);
            t.MCPRateToken = PurchaseRateToken;
            t.MCPCardholderAmount = "500";
            t.MCPCardHolderCurrncy = "840";
            Response = (apiTransaction.PerformTransaction(t, ProjectName, testCaseVersion));
            Assert.IsTrue(apiTransaction.IsValidTransaction);
            preOrderID = apiTransaction.OrderId;
            preTxnNumber = apiTransaction.TxnNumber;

            if (!string.IsNullOrEmpty(RefundRateToken))
            {
                t.transactionType = TransactionBase.TransactionType.MCPRefund;
                t.MCPCardholderAmount = "500";
                t.MCPCardHolderCurrncy = "840";
                t.MCPRateToken = RefundRateToken;
                t.OrderId = preOrderID;
                t.TxnNumber = preTxnNumber;
                Response = (apiTransaction.PerformTransaction(t, ProjectName, testCaseVersion,"TC15"));
                Assert.IsTrue(apiTransaction.IsValidTransaction);
            }
        }

        // 16 - Ind refund with out token
        [Test, Order(16)]
        public void IndependentRefundMCP()
        {
            t.transactionType = TransactionBase.TransactionType.MCPIndependentRefund;
            t.OrderId = t.OrderId = "MCPIndRefund" + now.Year + now.ToString("MM") + now.ToString("dd") + "_" + now.Hour + now.Minute + now.Second + r.Next(0, 999999);
            t.Amount = "20.00";
            //t.MCPRateToken = RefundRateToken;
            Response = (apiTransaction.PerformTransaction(t, ProjectName, testCaseVersion, "TC16"));
            Assert.IsTrue(apiTransaction.IsValidTransaction);
        }

        // 17  - Res Purchase wot
        [Test, Order(17)]
        public void PurchaseWithVaultWithoutTokenMCP()
        {
            t.transactionType = TransactionBase.TransactionType.VaultAddCC;
            Response = (apiTransaction.PerformTransaction(t, ProjectName, testCaseVersion));
            DataKey = apiTransaction.DataKey;
            if (!string.IsNullOrEmpty(DataKey))
            {
                t.transactionType = TransactionBase.TransactionType.MCPResPurchase;
                t.DataKey = DataKey;
                t.OrderId = t.OrderId = t.OrderId = "MCPResPurchase" + now.Year + now.ToString("MM") + now.ToString("dd") + "_" + now.Hour + now.Minute + now.Second + r.Next(0, 999999);
                t.Amount = "2.00";
                Response = (apiTransaction.PerformTransaction(t, ProjectName, testCaseVersion, "TC17"));
                Assert.IsTrue(apiTransaction.IsValidTransaction);
            }
        }

        [Test, Order(18)]
        //18 - Res Pre auth wot
        public void ResPreauthMCP()
        {
            t.transactionType = TransactionBase.TransactionType.VaultAddCC;
            Response = (apiTransaction.PerformTransaction(t, ProjectName, testCaseVersion));
            DataKey = apiTransaction.DataKey;
            if (!string.IsNullOrEmpty(DataKey))
            {
                t.transactionType = TransactionBase.TransactionType.MCPResPurchase;
            }

            t.transactionType = TransactionBase.TransactionType.MCPResPreAuth;
            t.DataKey = DataKey;
            t.OrderId = t.OrderId = t.OrderId = "MCPResPreauth" + now.Year + now.ToString("MM") + now.ToString("dd") + "_" + now.Hour + now.Minute + now.Second + r.Next(0, 999999);
            t.Amount = "2.00";
            Response = (apiTransaction.PerformTransaction(t, ProjectName, testCaseVersion, "TC18"));
            Assert.IsTrue(apiTransaction.IsValidTransaction);
        }



        //19 - Res ind refund wot
        [Test, Order(19)]
        public void ResIndependentRefund()
        {
            t.transactionType = TransactionBase.TransactionType.VaultAddCC;
            Response = (apiTransaction.PerformTransaction(t, ProjectName, testCaseVersion));
            DataKey = apiTransaction.DataKey;
            if (!string.IsNullOrEmpty(DataKey))
            {
                t.transactionType = TransactionBase.TransactionType.MCPResIndependentRefund;
                t.OrderId = t.OrderId = "MCPResIndrefund" + now.Year + now.ToString("MM") + now.ToString("dd") + "_" + now.Hour + now.Minute + now.Second + r.Next(0, 999999);
                t.Amount = "2.00";
                t.DataKey = DataKey;
                Response = (apiTransaction.PerformTransaction(t, ProjectName, testCaseVersion, "TC19"));
                Assert.IsTrue(apiTransaction.IsValidTransaction, "Did not get response code < 50");
            }
        }

        //20 - Purchase without token  master card
        [Test, Order(20)]
        public void MCPurchaseWithOutTokenMCP()
        {
            t.transactionType = TransactionBase.TransactionType.MCPPurchase;
            t.OrderId = t.OrderId = "MCPPurchase" + now.Year + now.ToString("MM") + now.ToString("dd") + "_" + now.Hour + now.Minute + now.Second + r.Next(0, 999999);
            t.PAN = "5454545454545454";
            Response = (apiTransaction.PerformTransaction(t, ProjectName, testCaseVersion, "TC20"));
            Assert.IsTrue(apiTransaction.IsValidTransaction);
            preOrderID = apiTransaction.OrderId;
            preTxnNumber = apiTransaction.TxnNumber;
        }


        //21 - Purchase with token MC
        [Test, Order(21)]
        public void MCPPurchaseWithTokenMPC()
        {

            t.transactionType = TransactionBase.TransactionType.MCPGetRate;
            t.OrderId = t.OrderId = "MCPGetRateForPurchse" + now.Year + now.ToString("MM") + now.ToString("dd") + "_" + now.Hour + now.Minute + now.Second + r.Next(0, 999999);
            t.MCPRateTransactionType = "P";
            t.CardHolderAmount = cardHolderAmount;
            t.MerchantSettlementAmount = merchantSettlementAmount;
            Response = (apiTransaction.PerformTransaction(t, ProjectName, testCaseVersion));
            Assert.IsTrue(apiTransaction.IsValidTransaction);
            PurchaseRateToken = apiTransaction.MCPRateToken;
            if (!string.IsNullOrEmpty(PurchaseRateToken))
            {
                t.transactionType = TransactionBase.TransactionType.MCPPurchase;
                t.OrderId = t.OrderId = "MCPPurchase" + now.Year + now.ToString("MM") + now.ToString("dd") + "_" + now.Hour + now.Minute + now.Second + r.Next(0, 999999);
                t.PAN = "5454545454545454";
                t.MCPRateToken = PurchaseRateToken;
                Response = (apiTransaction.PerformTransaction(t, ProjectName, testCaseVersion, "TC21"));
                Assert.IsTrue(apiTransaction.IsValidTransaction);
                preOrderID = apiTransaction.OrderId;
                preTxnNumber = apiTransaction.TxnNumber;
            }
        }

        //22 - Purchase AMEX with token
        [Test, Order(22)]
        public void PurchaseWithAMEXMcp()
        {
            t.transactionType = TransactionBase.TransactionType.MCPGetRate;
            t.OrderId = t.OrderId = "MCPGetRateForPurchse" + now.Year + now.ToString("MM") + now.ToString("dd") + "_" + now.Hour + now.Minute + now.Second + r.Next(0, 999999);
            t.MCPRateTransactionType = "P";
            t.CardHolderAmount = cardHolderAmount;
            t.MerchantSettlementAmount = merchantSettlementAmount;
            Response = (apiTransaction.PerformTransaction(t, ProjectName, testCaseVersion));
            Assert.IsTrue(apiTransaction.IsValidTransaction);
            PurchaseRateToken = apiTransaction.MCPRateToken;

            if (!string.IsNullOrEmpty(PurchaseRateToken))
            {
                t.transactionType = TransactionBase.TransactionType.MCPPurchase;
                t.OrderId = t.OrderId = "MCPPurchaseAMEX" + now.Year + now.ToString("MM") + now.ToString("dd") + "_" + now.Hour + now.Minute + now.Second + r.Next(0, 999999);
                t.PAN = "373599005095005";
                t.CVD = "1234";
                t.MCPRateToken = PurchaseRateToken;
                Response = (apiTransaction.PerformTransaction(t, ProjectName, testCaseVersion, "TC22"));
                Assert.IsTrue(apiTransaction.IsValidTransaction);
                preOrderID = apiTransaction.OrderId;
                preTxnNumber = apiTransaction.TxnNumber;
            }
        }

        //23 - Purchase wuth expired token 
        [Test,Order(23)]
        public void PurchaseWithExpiredRateToken()
        {
            t.transactionType = TransactionBase.TransactionType.MCPPreAuth;
            t.OrderId = t.OrderId = "MCPPurchase" + now.Year + now.ToString("MM") + now.ToString("dd") + "_" + now.Hour + now.Minute + now.Second + r.Next(0, 999999);
            t.PAN = "4242424242424242";
            t.CVD = "123";
            t.MCPRateToken = ExpiredPurchaseToken;
            Response = (apiTransaction.PerformTransaction(t, ProjectName, testCaseVersion, "TC23"));
            preOrderID = apiTransaction.OrderId;
            preTxnNumber = apiTransaction.TxnNumber;
            Assert.IsFalse(apiTransaction.IsValidTransaction);
        }

        //24 - Purchase with customer details
        //[Test, Order(24)]
        //public void PurchaseWithCustomrDetailsMCP()
        //{
        //    t.transactionType = TransactionBase.TransactionType.MCPPurchase;
        //    t.OrderId = t.OrderId = "MCPPurchase" + now.Year + now.ToString("MM") + now.ToString("dd") + "_" + now.Hour + now.Minute + now.Second + r.Next(0, 999999);
        //    t.MCPRateToken = PurchaseRateToken;
        //    t.MCPCardholderAmount = "500";
        //    t.MCPCardHolderCurrncy = "840";
        //    t.CustomerInfo = apiTransaction.setCustomerInfo();
        //    Response = (apiTransaction.PerformTransaction(t, ProjectName, testCaseVersion, "TC24"));
        //    Assert.IsTrue(apiTransaction.IsValidTransaction);
        //    preOrderID = apiTransaction.OrderId;
        //    preTxnNumber = apiTransaction.TxnNumber;
        //}


        //25 - Purchase with refund token
        [Test, Order(25)]
        public void PurchaseWithReturnTokenMCP()
        {
            t.transactionType = TransactionBase.TransactionType.MCPGetRate;
            t.OrderId = t.OrderId = "MCPGetRateForPurchse" + now.Year + now.ToString("MM") + now.ToString("dd") + "_" + now.Hour + now.Minute + now.Second + r.Next(0, 999999);
            t.MCPRateTransactionType = "R";
            t.CardHolderAmount = cardHolderAmount;
            t.MerchantSettlementAmount = merchantSettlementAmount;
            Response = (apiTransaction.PerformTransaction(t, ProjectName, testCaseVersion));
            Assert.IsTrue(apiTransaction.IsValidTransaction);
            RefundRateToken = apiTransaction.MCPRateToken;

            if(!string.IsNullOrEmpty(RefundRateToken))
            {
                t.transactionType = TransactionBase.TransactionType.MCPPurchase;
                t.OrderId = t.OrderId = "MCPPurchase" + now.Year + now.ToString("MM") + now.ToString("dd") + "_" + now.Hour + now.Minute + now.Second + r.Next(0, 999999);
                t.MCPRateToken = RefundRateToken;
                Response = (apiTransaction.PerformTransaction(t, ProjectName, testCaseVersion, "TC25"));
                Assert.IsFalse(apiTransaction.IsValidTransaction);
                preOrderID = apiTransaction.OrderId;
                preTxnNumber = apiTransaction.TxnNumber;

            }
        }

        //26 - Refund with purchase token
        [Test, Order(26)]
        public void RefundWithPurchaseToken()
        {
            t.transactionType = TransactionBase.TransactionType.MCPPurchase;
            t.OrderId = t.OrderId = "MCPPurchase" + now.Year + now.ToString("MM") + now.ToString("dd") + "_" + now.Hour + now.Minute + now.Second + r.Next(0, 999999);
            t.MCPRateToken = PurchaseRateToken;
            Response = (apiTransaction.PerformTransaction(t, ProjectName, testCaseVersion));
            Assert.IsTrue(apiTransaction.IsValidTransaction);
            preOrderID = apiTransaction.OrderId;
            preTxnNumber = apiTransaction.TxnNumber;

            if (!string.IsNullOrEmpty(PurchaseRateToken))
            {
                t.transactionType = TransactionBase.TransactionType.MCPRefund;
                t.MCPRateToken = PurchaseRateToken;
                t.OrderId = preOrderID;
                t.TxnNumber = preTxnNumber;
                Response = (apiTransaction.PerformTransaction(t, ProjectName, testCaseVersion, "TC26"));
                Assert.IsFalse(apiTransaction.IsValidTransaction);
            }
        }

        //27 - GetToken for non supported currency
        [Test, Order(27)]
        public void GetRateForInvalidCountry()
        {
            t.transactionType = TransactionBase.TransactionType.MCPGetRate;
            t.OrderId = t.OrderId = "MCPGetRateForPurchse" + now.Year + now.ToString("MM") + now.ToString("dd") + "_" + now.Hour + now.Minute + now.Second + r.Next(0, 999999);
            t.MCPRateTransactionType = "P";
            t.CardHolderAmount = cardHolderAmountWithInvlidCountyrCode;
            Response = (apiTransaction.PerformTransaction(t, ProjectName, testCaseVersion, "TC27"));
            // AS it return 001, but in message field idt show "Unsupported Currency"
            Assert.Multiple(() =>
            {
                Assert.IsTrue(apiTransaction.IsValidTransaction); // This should be ture as it does not have issue with requets.
                Assert.IsTrue(apiTransaction.MCPErrorCodeshown);
            });
        }

        //28 - Get Rate with bad Token

        //29 - Decline purchase with invalid expdate
        [Test, Order(29)]
        public void PurchaseWithInvlidExpdateMCP()
        {
            t.transactionType = TransactionBase.TransactionType.MCPPurchase;
            t.OrderId = t.OrderId = "MCPPurchase" + now.Year + now.ToString("MM") + now.ToString("dd") + "_" + now.Hour + now.Minute + now.Second + r.Next(0, 999999);
            t.MCPCardholderAmount = "500";
            t.MCPCardHolderCurrncy = "840";
            t.ExpDate = "1010";
            Response = (apiTransaction.PerformTransaction(t, ProjectName, testCaseVersion, "TC29"));
            string sentXml = apiTransaction.SentXML;
            string s = apiTransaction.response;
            Assert.IsFalse(apiTransaction.IsValidTransaction);
        }

        //30 - Regular refund wtih MCP Purchase
        [Test, Order(30)]
        public void ReunfOnMCPPurchae() {

            // MCP Purchase
            t.transactionType = TransactionBase.TransactionType.MCPPurchase;
            t.OrderId = t.OrderId = "MCPPurchase" + now.Year + now.ToString("MM") + now.ToString("dd") + "_" + now.Hour + now.Minute + now.Second + r.Next(0, 999999);
            t.MCPRateToken = PurchaseRateToken;
            Response = (apiTransaction.PerformTransaction(t, ProjectName, testCaseVersion));
            preOrderID = apiTransaction.OrderId;
            preTxnNumber = apiTransaction.TxnNumber;

            // Regular refund
            t.transactionType = TransactionBase.TransactionType.Refund;
            t.OrderId = preOrderID;
            t.TxnNumber = preTxnNumber;
            t.Amount = "1.00";
            Response = (apiTransaction.PerformTransaction(t, ProjectName, testCaseVersion, "TC30"));
            Assert.IsTrue(apiTransaction.IsValidTransaction);
        }


        //31 - Regular completion with MCp Preath
        [Test, Order(31)]
        public void CompletiOnMCPPreAuth()
        {
            //MCP Preauth
            t.transactionType = TransactionBase.TransactionType.MCPPreAuth;
            t.OrderId = t.OrderId = "MCPPreAuth" + now.Year + now.ToString("MM") + now.ToString("dd") + "_" + now.Hour + now.Minute + now.Second + r.Next(0, 999999);  
            Response = (apiTransaction.PerformTransaction(t, ProjectName, testCaseVersion));
            preOrderID = apiTransaction.OrderId;
            preTxnNumber = apiTransaction.TxnNumber;

            //Completion
            t.transactionType = TransactionBase.TransactionType.Capture;
            t.OrderId = preOrderID;
            t.Amount = "1.00";
            Response = (apiTransaction.PerformTransaction(t, ProjectName, testCaseVersion, "TC31"));
            Assert.IsTrue(apiTransaction.IsValidTransaction);
        }

        //32 - Regualar void with MCP purchase
        [Test, Order(32)]
        public void PurchaseCorrectionOnMCPPurchase()
        {
            // MCP Purchase
            t.transactionType = TransactionBase.TransactionType.MCPPurchase;
            t.OrderId = t.OrderId = "MCPPurchase" + now.Year + now.ToString("MM") + now.ToString("dd") + "_" + now.Hour + now.Minute + now.Second + r.Next(0, 999999);
            Response = (apiTransaction.PerformTransaction(t, ProjectName, testCaseVersion));
            preOrderID = apiTransaction.OrderId;
            preTxnNumber = apiTransaction.TxnNumber;

            // Purchase Correction
            t.transactionType = TransactionBase.TransactionType.PurchaseCorrection;
            t.OrderId = preOrderID;
            t.TxnNumber = preTxnNumber;
            Response = (apiTransaction.PerformTransaction(t, ProjectName, testCaseVersion, "TC32"));
            Assert.IsTrue(apiTransaction.IsValidTransaction);
        }

        //33 - MCP Purchase with AVS
        [Test, Order(33)]
        public void PurchaseWithAVSMCP()
        {
            t.transactionType = TransactionBase.TransactionType.MCPPurchase;
            t.OrderId = t.OrderId = "MCPPurchase" + now.Year + now.ToString("MM") + now.ToString("dd") + "_" + now.Hour + now.Minute + now.Second + r.Next(0, 999999);
            t.MCPRateToken = PurchaseRateToken;
            t.SetAVSInfo = true;
            Response = (apiTransaction.PerformTransaction(t, ProjectName, testCaseVersion, "TC33"));
            Assert.IsTrue(apiTransaction.IsValidTransaction);
        }

        //34 - MCP Purchase with CVD
        [Test, Order(34)]
        public void PurchaseWithCVDMCP()
        {

        }

        //35 - MCP Purchase with AVS and CVD
        [Test, Order(35)]
        public void PurchaseWithAVSCVDMCP()
        {
        }

        //36 - MCP Purchase with 0.00 amount 
        [Test, Order(36)]
        public void PurchaseWith0AmountMCP()
        {
            t.transactionType = TransactionBase.TransactionType.MCPPurchase;
            t.OrderId = t.OrderId = "MCPPurchase" + now.Year + now.ToString("MM") + now.ToString("dd") + "_" + now.Hour + now.Minute + now.Second + r.Next(0, 999999);
            t.MCPCardholderAmount = "000";
            t.MCPCardHolderCurrncy = "840";
            Response = (apiTransaction.PerformTransaction(t, ProjectName, testCaseVersion, "TC36"));
            Assert.IsTrue(apiTransaction.IsValidTransaction);
        }

        //37 - MCP Preauth with 0.00 
        [Test, Order(37)]
        public void PreAuthWith0AmountMCP()
        {
            t.transactionType = TransactionBase.TransactionType.MCPPreAuth;
            t.OrderId = t.OrderId = "MCPPreAuth" + now.Year + now.ToString("MM") + now.ToString("dd") + "_" + now.Hour + now.Minute + now.Second + r.Next(0, 999999);
            t.MCPCardholderAmount = "000";
            t.MCPCardHolderCurrncy = "840";
            Response = (apiTransaction.PerformTransaction(t, ProjectName, testCaseVersion, "TC37"));
            string sentXML = apiTransaction.SentXML;
            string response = apiTransaction.response;
            Assert.IsFalse(apiTransaction.IsValidTransaction);
        }

        //38 - MCP Completion with 0.00
        [Test, Order(38)]
        public void CompletionWith0AmountMCP()
        {
            t.transactionType = TransactionBase.TransactionType.MCPPreAuth;
            t.OrderId = t.OrderId = "MCPPreAuth" + now.Year + now.ToString("MM") + now.ToString("dd") + "_" + now.Hour + now.Minute + now.Second + r.Next(0, 999999);
            t.MCPCardholderAmount = "500";
            t.MCPCardHolderCurrncy = "840";
            Response = (apiTransaction.PerformTransaction(t, ProjectName, testCaseVersion, "TC37"));
            string sentXML = apiTransaction.SentXML;
            string response = apiTransaction.response;
            preOrderID = apiTransaction.OrderId;
            preAmount = apiTransaction.TxnNumber;

            // Completio with 0.00 
            t.transactionType = TransactionBase.TransactionType.Capture;
            t.OrderId = preOrderID;
            t.MCPCardholderAmount = "0.00";
            Response = (apiTransaction.PerformTransaction(t, ProjectName, testCaseVersion, "TC38"));
            Assert.IsTrue(apiTransaction.IsValidTransaction);
        }

        //39 - MCP refund with 0
        [Test, Order(39)]
        public void Refund0MCP()
        {
            t.transactionType = TransactionBase.TransactionType.MCPPurchase;
            t.OrderId = t.OrderId = "MCPPurchase" + now.Year + now.ToString("MM") + now.ToString("dd") + "_" + now.Hour + now.Minute + now.Second + r.Next(0, 999999);
            t.MCPRateToken = PurchaseRateToken;
            t.MCPCardholderAmount = "500";
            t.MCPCardHolderCurrncy = "840";
            Response = (apiTransaction.PerformTransaction(t, ProjectName, testCaseVersion));
            preOrderID = apiTransaction.OrderId;
            preAmount = apiTransaction.Amount;


            t.transactionType = TransactionBase.TransactionType.Refund;
            t.OrderId = preOrderID;
            t.TxnNumber = preTxnNumber;
            t.MCPCardholderAmount = "000";
            Response = (apiTransaction.PerformTransaction(t, ProjectName, testCaseVersion, "TC39"));
            Assert.IsTrue(apiTransaction.IsValidTransaction);
        }

        //40 - Completion 0 on MCP Purchase
        [Test, Order(40)]
        public void Copletion0MCP()
        {
            t.transactionType = TransactionBase.TransactionType.MCPPreAuth;
            t.OrderId = t.OrderId = "MCPPreAuth" + now.Year + now.ToString("MM") + now.ToString("dd") + "_" + now.Hour + now.Minute + now.Second + r.Next(0, 999999);
            t.MCPCardholderAmount = "500";
            t.MCPCardHolderCurrncy = "840";
            Response = (apiTransaction.PerformTransaction(t, ProjectName, testCaseVersion));
            preOrderID = apiTransaction.OrderId;
            preTxnNumber = apiTransaction.TxnNumber;

            t.transactionType = TransactionBase.TransactionType.MCPCompletion;
            t.OrderId = preOrderID;
            t.TxnNumber = preTxnNumber;
            t.MCPCardholderAmount = "000";
            Response = (apiTransaction.PerformTransaction(t, ProjectName, testCaseVersion, "TC40"));
            Assert.IsTrue(apiTransaction.IsValidTransaction);
        }

        [Test, Order(41)]
        //41 - MCP Preauth with $9,999,999.00
        public void PreAuthMCP()
        {
            t.transactionType = TransactionBase.TransactionType.MCPPreAuth;
            t.OrderId = t.OrderId = "MCPPreAuth" + now.Year + now.ToString("MM") + now.ToString("dd") + "_" + now.Hour + now.Minute + now.Second + r.Next(0, 999999);
            t.MCPCardholderAmount = "999999900";
            t.MCPCardHolderCurrncy = "840";
            Response = (apiTransaction.PerformTransaction(t, ProjectName, testCaseVersion,"TC41"));
            Assert.IsTrue(apiTransaction.IsValidTransaction);
            preOrderID = apiTransaction.OrderId;
            preTxnNumber = apiTransaction.TxnNumber;
        }

        //42 - MCP Completion for $9,999,999.00
        [Test, Order(42)]
        public void completionMCP()
        {
            t.transactionType = TransactionBase.TransactionType.MCPPreAuth;
            t.OrderId = t.OrderId = "MCPPreAuth" + now.Year + now.ToString("MM") + now.ToString("dd") + "_" + now.Hour + now.Minute + now.Second + r.Next(0, 999999);
            t.MCPCardholderAmount = "999999900";
            t.MCPCardHolderCurrncy = "840";
            Response = (apiTransaction.PerformTransaction(t, ProjectName, testCaseVersion,"TC42"));
            Assert.IsTrue(apiTransaction.IsValidTransaction);
        }

        //43 - MCP Purchase for $9,999,999.00
        [Test, Order(43)]
        public void PurchaseMCP()
        {
            t.transactionType = TransactionBase.TransactionType.MCPPurchase;
            t.OrderId = t.OrderId = "MCPPurchase" + now.Year + now.ToString("MM") + now.ToString("dd") + "_" + now.Hour + now.Minute + now.Second + r.Next(0, 999999);
            t.MCPRateToken = PurchaseRateToken;
            t.MCPCardholderAmount = "999999900";
            t.MCPCardHolderCurrncy = "840";
            Response = (apiTransaction.PerformTransaction(t, ProjectName, testCaseVersion, "TC43"));
            Assert.IsTrue(apiTransaction.IsValidTransaction);
            preOrderID = apiTransaction.OrderId;
            preTxnNumber = apiTransaction.TxnNumber;
        }

        //44 - MCP Refund for $9,999,999.00
        [Test, Order(44)]
        public void RefundMCP()
        {
            t.transactionType = TransactionBase.TransactionType.MCPPurchase;
            t.OrderId = t.OrderId = "MCPPurchase" + now.Year + now.ToString("MM") + now.ToString("dd") + "_" + now.Hour + now.Minute + now.Second + r.Next(0, 999999);
            t.MCPRateToken = PurchaseRateToken;
            t.MCPCardholderAmount = "999999900";
            t.MCPCardHolderCurrncy = "840";
            Response = (apiTransaction.PerformTransaction(t, ProjectName, testCaseVersion));
            Assert.IsTrue(apiTransaction.IsValidTransaction);
            preOrderID = apiTransaction.OrderId;
            preTxnNumber = apiTransaction.TxnNumber;

            if (!string.IsNullOrEmpty(RefundRateToken))
            {
                t.transactionType = TransactionBase.TransactionType.MCPRefund;
                t.MCPCardholderAmount = "999999900";
                t.MCPCardHolderCurrncy = "840";
                t.MCPRateToken = RefundRateToken;
                t.OrderId = preOrderID;
                t.TxnNumber = preTxnNumber;
                Response = (apiTransaction.PerformTransaction(t, ProjectName, testCaseVersion, "TC15"));
                Assert.IsTrue(apiTransaction.IsValidTransaction);
            }
        }

        //45 - Open total with MCP transaction

        //46 - Close batch with MCP Transaction

        //47 - Open total MCP and regular transaction

        //48 - Btach Close with MCP and regular purchase




    }
}
