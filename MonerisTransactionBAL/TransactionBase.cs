using Moneris;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonerisTransactionBAL
{
    public class TransactionBase : BaseClass
    {
       
        internal string EcrNo;
        //internal readonly string kountTransactionId;

        //Checking repo
        public enum TransactionType
        {
            Purchase = 0,
            PreAuth = 1,
            Capture = 2,
            PurchaseCorrection = 3,
            Refund = 4,
            CardVerification = 5,           
            MPITxn = 13,
            MPIACS = 14,            
            PurchseWithRecurring = 15,           
            UpdateRecurring = 18,
            ReAuth = 19,
            CAVVPreAuth = 20,
            EncPreAuth = 21,
            IndependentRefund = 22,
            ForcePost = 23,
            ENCVaultAddCC = 24,
            VaultTempToken = 16,
            ValutTokenCardVerification = 17,
            VaultUpdateCC = 7,
            VaultTokenizeCC = 8,
            VaultDeleteCC = 9,
            VaultPurchase = 11,
            VaultPreauth = 12,
            VaultFullLookUpCC = 26,
            VaultMaskLookUpCC = 27,
            VaultAddToken = 6,
            VaultAddHPPToken = 28,
            BatchClose = 29,
            OpenTotal = 30,
            CAVVPurchase = 31,
            VaultAddCC = 32, 
            KountInquiry = 33,
            KountUpdate = 34,
            ApplePayPurchase = 35,
            GooglePayPurchase = 36,
            GooglePayPreAuth = 37
        }

        //public string StoreID { get; set; }

         public string KountTransactionId { get; set; }
        public CustInfo CustomerInfo { get; set; }
        private string StoreId { get; set; }
        private string ApiToken { get; set; }
        public string OrderId { get; set; }
        public string TxnNumber { get; set; }
        public TransactionType transactionType { get; set; }
        public string CrtpyType { get; set; }
        public string PAN { get; set; }
        public string ExpDate { get; set; }
        public string CVD { get; set; }
        public string CustId { get; set; }
        public string Amount { get; set; }
        public string DynamicDes { get; set; }
        public CofInfo cofInfo { get; set; }
        public string DataKey { get;    set; }
        public string IssuerId { get; set; }
        public Recur RecuringCycle { get; set; }
        public string ShipIndicator { get; set; }
        public AvsInfo AvsCheck { get; set; }
        public CvdInfo CvdCheck { get; set; }
        public string Duration { get; internal set; }
        public string OriginalOrderID { get;  set; }
        public string CAVV { get; set; }
        public string ENC_Track2 { get; internal set; }
        public string DeviceType { get; internal set; }
        public string AuthCode { get; internal set; }
        public ConvFeeInfo ConvFee { get; internal set; }
        public string DataKeyFormat { get; internal set; }
        public bool TestMode { get; internal set; }
        public string CmID { get; internal set; }
        public string MarketIndicator { get; internal set; }
        public string Apple_Signature { get; internal set; }
        public string Apple_Data { get; internal set; }
        public string Apple_Version { get; internal set; }
        public string Apple_PublicKeyHash { get; internal set; }
        public string Apple_EphemeralPublicKey { get; internal set; }
        public string Apple_TransactionId { get; internal set; }
        public Apple_TokenOriginator Apple_TokenOriginatore { get; internal set; }
        public string GooglePay_Signature { get; internal set; }
        public string GooglePay_protocolVersion { get; internal set; }
        public string GooglePay_signedmessage { get; internal set; }
        public GooglePay_PaymentToken googlePay_PaymentToken { get; internal set; }

        public TransactionBase()
        {
            this.StoreId = _storeID;
            this.ApiToken = _aPIToken;
            this.CustomerInfo = new CustInfo();
        }

        public void setProdcution()
        {
            base.setProdcutionEnvironment();
        }

        public new void setQAEnvironment()
        {
            base.setQAEnvironment();
        }

        //private void Init(TransactionType transactionT)
        //{
        //    transactionType = transactionT;
        //    this.storeID = StoreID;
        //    this.ApiToken = APIToken;
        //    DateTime now = DateTime.Now;
        //    OrderId = "Moneris_" + now.Year + now.ToString("MM") + now.ToString("dd") + "_" + now.Hour + now.Minute + now.Second; // make this date/time stamp
        //    TxnNumber = "";
        //    PAN = "4242424242424242";
        //    ExpDate = now.ToString("yy") + "12";
        //    //CrtpyType = "7";
        //    CVD = "123";
        //}

    }

    public class GooglePay_PaymentToken
    {
        public string signature { get; set; }
        public string protocolVersion { get; set; }
        public string signedMessage { get; set; }
    }

    public class Apple_TokenOriginator
    {
        public string originatorstoreID;
        public string originatoreAPIToken;
    }
}
