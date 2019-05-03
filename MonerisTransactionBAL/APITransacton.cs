using Moneris;
using System;
using System.Collections;
using System.Data.SqlClient;
using System.Text;

namespace MonerisTransactionBAL
{
    public class APITransacton : TransactionBase
    {
        public APITransacton()
        {
            this.CvdCheck = new CvdInfo();
            this.AvsCheck = new AvsInfo();
        }

        private string storeId { get; set; }
        public string SentXML { get; set; }
        public string Message { get; private set; }
        public string ProjectName { get; internal set; }
        public string apiToken { get; set; }
        public bool IsValidTransaction { get; set; }
        public string connectionStringAWS => "Data Source=52.14.31.135;Initial Catalog=APITransaction;User ID=KrunalP2;Password=Shiva@1984";
        public string connectionStringLocalDB => "Data Source=T450-PC06FQ83;Initial Catalog=APITransaction;Integrated Security=True";
        public string kountTransactionsId { get; set; }
        public bool? MCPErrorCodeshown { get; set; }
        public string response { get; set; }
        //public string MCPRateToken { get; set; }

        // public new string TxnNumber { get; set; }
        //public string OrderId { get; private set; }

        //public string DataKey { get; set; }
        public string PerformTransaction(TransactionBase t, string appName = "Test", string testVersion = "default", string TestCaseNumber = null)
        {

            HttpsPostRequest mpgReq = new HttpsPostRequest();
            this.ProjectName = appName;
            storeId = t._storeID;
            apiToken = t._aPIToken;

            switch (t.transactionType)
            {
                case TransactionType.Purchase:
                    Purchase purchase = doPurchase(t);
                    mpgReq.SetTransaction(purchase);
                    break;

                case TransactionType.CAVVPurchase:
                    CavvPurchase cavvPurchase = doCavvPurchase(t);
                    mpgReq.SetTransaction(cavvPurchase);
                    break;
                case TransactionType.Refund:
                    Refund refund = doRefund(t);
                    mpgReq.SetTransaction(refund);
                    break;

                case TransactionType.PurchaseCorrection:
                    PurchaseCorrection perCorrection = doPurchaseCorrection(t);
                    mpgReq.SetTransaction(perCorrection);
                    break;

                case TransactionType.IndependentRefund:
                    IndependentRefund indRefund = doIndRefund(t);
                    mpgReq.SetTransaction(indRefund);
                    break;
                case TransactionType.PreAuth:
                    PreAuth preAuth = doPreAuth(t);
                    mpgReq.SetTransaction(preAuth);
                    break;
                case TransactionType.CAVVPreAuth:
                    CavvPreAuth cavvpreAuth = doCavvPreAuth(t);
                    //cavvpreAuth.SetCmId(t.CmID);
                    mpgReq.SetTransaction(cavvpreAuth);
                    break;
                case TransactionType.EncPreAuth:
                    EncPreauth encpreauth = new EncPreauth();
                    encpreauth.SetOrderId(t.OrderId);
                    encpreauth.SetAmount(t.Amount);
                    encpreauth.SetEncTrack2(t.ENC_Track2);
                    encpreauth.SetDeviceType(t.DeviceType);
                    encpreauth.SetCryptType(t.CrtpyType);
                    mpgReq.SetTransaction(encpreauth);
                    break;
                case TransactionType.Capture:
                    Completion com = doCompletion(t);
                    mpgReq.SetTransaction(com);
                    break;

                case TransactionType.ForcePost:
                    ForcePost forcePost = doForcePost(t);
                    mpgReq.SetTransaction(forcePost);
                    break;


                case TransactionType.VaultAddCC:
                    ResAddCC resaddcc = new ResAddCC();
                    resaddcc.SetPan(t.PAN);
                    resaddcc.SetExpDate(t.ExpDate);
                    resaddcc.SetCryptType(t.CrtpyType);
                    //resaddcc.SetCofInfo(setCardOnFile("C","0",this.IssuerId));
                    resaddcc.SetCofInfo(t.cInfo);
                    if (t.cInfo != null)
                    {
                        AvsInfo avsCheck2 = new AvsInfo();
                        avsCheck2.SetAvsStreetNumber("212");
                        avsCheck2.SetAvsStreetName("Payton Street");
                        avsCheck2.SetAvsZipCode("M1M1M1");
                        resaddcc.SetAvsInfo(avsCheck2);
                    }
                    resaddcc.SetDataKeyFormat(t.DataKeyFormat);
                    mpgReq.SetTransaction(resaddcc);
                    break;

                case TransactionType.VaultAddToken:
                    ResAddToken rAddToken = new ResAddToken();
                    rAddToken.SetDataKey(t.DataKey);
                    rAddToken.SetCryptType(t.CrtpyType);
                    rAddToken.SetExpDate(t.ExpDate);
                    if (t.AvsCheck != null)
                        rAddToken.SetAvsInfo(t.AvsCheck);
                    mpgReq.SetTransaction(rAddToken);
                    break;

                case TransactionType.VaultAddHPPToken:
                    ResAddToken resAddToken = new ResAddToken();
                    resAddToken.SetDataKey(t.DataKey);
                    resAddToken.SetCryptType(t.CrtpyType);
                    resAddToken.SetExpDate(t.ExpDate);
                    if (t.AvsCheck != null)
                        resAddToken.SetAvsInfo(t.AvsCheck);
                    mpgReq.SetTransaction(resAddToken);
                    break;

                case TransactionType.VaultDeleteCC:
                    ResDelete resDelete = new ResDelete(t.DataKey);
                    mpgReq.SetTransaction(resDelete);
                    break;
                case TransactionType.VaultFullLookUpCC:
                    ResLookupFull resLookupFull = new ResLookupFull(t.DataKey);
                    mpgReq.SetTransaction(resLookupFull);
                    break;
                case TransactionType.VaultMaskLookUpCC:
                    ResLookupMasked resLookupMasked = new ResLookupMasked();
                    resLookupMasked.SetDataKey(t.DataKey);
                    mpgReq.SetTransaction(resLookupMasked);
                    break;

                case TransactionType.VaultPurchase:
                    ResPurchaseCC resPurchaseCC = doResPurchase(t);
                    resPurchaseCC.SetCryptType(t.CrtpyType);
                    mpgReq.SetTransaction(resPurchaseCC);
                    break;


                case TransactionType.VaultPreauth:
                    ResPreauthCC resPreauthCC = doResPreAuth(t);
                    mpgReq.SetTransaction(resPreauthCC);
                    break;

                case TransactionType.VaultUpdateCC:
                    ResUpdateCC resUpdateCC = doResUpdateCC(t);
                    mpgReq.SetTransaction(resUpdateCC);
                    break;

                case TransactionType.CardVerification:
                    CardVerification cardVerification = doCardverification(t);
                    mpgReq.SetTransaction(cardVerification);
                    break;

                case TransactionType.VaultTempToken:
                    ResTempAdd resTempAdd = new ResTempAdd();
                    resTempAdd.SetPan(t.PAN);
                    resTempAdd.SetExpDate(t.ExpDate);
                    resTempAdd.SetDuration(t.Duration);
                    resTempAdd.SetCryptType(t.CrtpyType);
                    mpgReq.SetTransaction(resTempAdd);
                    break;

                case TransactionType.ValutTokenCardVerification:
                    ResCardVerificationCC resCardVerificationCC = new ResCardVerificationCC();
                    resCardVerificationCC.SetDataKey(t.DataKey);
                    resCardVerificationCC.SetOrderId(t.OrderId);
                    resCardVerificationCC.SetCryptType(t.CrtpyType);
                    resCardVerificationCC.SetCofInfo(t.cofInfo);
                    resCardVerificationCC.SetAvsInfo(t.AvsCheck);
                    resCardVerificationCC.SetCvdInfo(t.CvdCheck);
                    mpgReq.SetTransaction(resCardVerificationCC);
                    // if it is temp token then you  can send an exp date in it.
                    break;

                case TransactionType.VaultTokenizeCC:
                    ResTokenizeCC resTokenizeCC = new ResTokenizeCC();
                    resTokenizeCC.SetOrderId(t.OrderId);
                    resTokenizeCC.SetTxnNumber(t.TxnNumber);
                    resTokenizeCC.SetCofInfo(t.cofInfo);
                    resTokenizeCC.SetDataKeyFormat(t.DataKeyFormat);
                    if (t.AvsCheck != null)
                    {
                        resTokenizeCC.SetAvsInfo(t.AvsCheck);
                    }
                    mpgReq.SetTransaction(resTokenizeCC);
                    break;
                case TransactionType.UpdateRecurring:
                    RecurUpdate recurUpdate = new RecurUpdate();
                    recurUpdate.SetOrderId(t.OrderId);
                    recurUpdate.SetRecurAmount(t.Amount);
                    if (t.PAN != null)
                    {
                        recurUpdate.SetPan(t.PAN);
                    }
                    recurUpdate.SetExpDate(t.ExpDate);
                    recurUpdate.SetCofInfo(t.cofInfo);
                    mpgReq.SetTransaction(recurUpdate);
                    break;
                case TransactionType.ReAuth:
                    ReAuth reauth = new ReAuth();
                    reauth.SetOrderId(t.OrderId);
                    reauth.SetCustId(t.CustId);
                    reauth.SetOrigOrderId(t.OriginalOrderID);
                    reauth.SetTxnNumber(t.TxnNumber);
                    reauth.SetAmount(t.Amount);
                    reauth.SetCryptType(t.CrtpyType);
                    reauth.SetDynamicDescriptor(t.DynamicDes);
                    reauth.SetCvdInfo(t.CvdCheck);
                    mpgReq.SetTransaction(reauth);
                    break;
                case TransactionType.ENCVaultAddCC:
                    EncResAddCC encresaddcc = new EncResAddCC();
                    encresaddcc.SetEncTrack2(t.ENC_Track2);
                    encresaddcc.SetDeviceType("idtech_bdk");
                    encresaddcc.SetCryptType(t.CrtpyType);
                    encresaddcc.SetCustId(t.CustId);
                    AvsInfo avsCheck4 = new AvsInfo();
                    avsCheck4.SetAvsStreetNumber("212");
                    avsCheck4.SetAvsStreetName("Payton Street");
                    avsCheck4.SetAvsZipCode("M1M1M1");
                    encresaddcc.SetAvsInfo(avsCheck4);
                    encresaddcc.SetDataKeyFormat(t.DataKeyFormat);
                    mpgReq.SetTransaction(encresaddcc);
                    break;
                case TransactionType.BatchClose:
                    BatchClose batchClose = new BatchClose();
                    batchClose.SetEcrno(t.EcrNo);
                    mpgReq.SetTransaction(batchClose);
                    break;
                case TransactionType.OpenTotal:
                    OpenTotals openTotal = new OpenTotals();
                    openTotal.SetEcrno(t.EcrNo);
                    mpgReq.SetTransaction(openTotal);
                    break;
                case TransactionType.KountInquiry:
                    KountInquiry kount_inquiry = new KountInquiry();
                    kount_inquiry.SetOrderId(t.OrderId);
                    mpgReq.SetTransaction(setKountInquiry(kount_inquiry));
                    break;
                case TransactionType.KountUpdate:
                    KountUpdate kount_Update = new KountUpdate();
                    kount_Update.SetOrderId(t.OrderId);
                    kount_Update.SetKountTransactionId(t.KountTransactionId);
                    kount_Update.SetFinancialOrderId(t.OriginalOrderID);
                    mpgReq.SetTransaction(setKountUpdate(kount_Update));
                    break;
                case TransactionType.ApplePayPurchase:
                    ApplePayTokenPurchase applePurchase = new ApplePayTokenPurchase();
                    applePurchase.SetOrderId(t.OrderId);
                    applePurchase.SetSignature(t.Apple_Signature);
                    applePurchase.SetData(t.Apple_Data);
                    applePurchase.SetVersion(t.Apple_Version);
                    applePurchase.SetHeader(t.Apple_PublicKeyHash, t.Apple_EphemeralPublicKey, t.Apple_TransactionId);
                    mpgReq.SetTransaction(applePurchase);
                    break;
                case TransactionType.GooglePayPurchase:
                    GooglePayPurchase googlePurchase = new GooglePayPurchase();
                    googlePurchase.SetCustId(t.CustId);
                    googlePurchase.SetNetwork("");
                    googlePurchase.SetOrderId(t.OrderId);
                    googlePurchase.SetPaymentToken(t.googlePay_PaymentToken.signature, t.googlePay_PaymentToken.protocolVersion, t.googlePay_PaymentToken.signedMessage);
                    mpgReq.SetTransaction(googlePurchase);
                    break;
                case TransactionType.GooglePayPreAuth:
                    GooglePayPurchase googlePreauth = new GooglePayPurchase();
                    googlePreauth.SetCustId(t.CustId);
                    googlePreauth.SetNetwork("Rogers");
                    googlePreauth.SetOrderId(t.OrderId);
                    googlePreauth.SetPaymentToken(t.GooglePay_Signature, t.GooglePay_protocolVersion, t.GooglePay_signedmessage);
                    mpgReq.SetTransaction(googlePreauth);
                    break;

                case TransactionType.MCPGetRate:
                    MCPGetRate getRate = new MCPGetRate();
                    getRate.SetMCPVersion("1.00");
                    getRate.SetMCPRateTxnType(t.MCPRateTransactionType);

                    MCPRate rate = new MCPRate();
                    rate.AddCardholderAmount(t.CardHolderAmount[0], t.CardHolderAmount[1]);
                    if (t.MerchantSettlementAmount != null)
                    {
                        rate.AddMerchantSettlementAmount(t.MerchantSettlementAmount[0], t.MerchantSettlementAmount[1]);
                    }
                    getRate.SetMCPRateInfo(rate);
                    mpgReq.SetTransaction(getRate);
                    break;

                case TransactionType.MCPPurchase:
                    MCPPurchase mcpPurchase = new MCPPurchase();
                    mcpPurchase.SetAmount(t.Amount);
                    mcpPurchase.SetOrderId(t.OrderId);
                    mcpPurchase.SetPan(t.PAN);
                    mcpPurchase.SetExpDate(t.ExpDate);
                    mcpPurchase.SetCryptType(t.CrtpyType);
                    mcpPurchase.SetMCPRateToken(t.MCPRateToken);
                    mcpPurchase.SetCardholderAmount(t.MCPCardholderAmount);
                    mcpPurchase.SetCardholderCurrencyCode(t.MCPCardHolderCurrncy);
                    mcpPurchase.SetMCPVersion(t.McpVersion);
                    mcpPurchase.SetCustInfo(t.CustomerInfo);
                    if (t.SetAVSInfo == true)
                    {   
                        mcpPurchase.SetAvsInfo(setValidAVSInfo());
                    }
                    if (t.SetCustInfo == true)
                    {
                        mcpPurchase.SetCustInfo(setCustomerInfo());
                    }
            
                    mpgReq.SetTransaction(mcpPurchase);
                    break;

                case TransactionType.MCPPurchaseCorrecetion:
                    MCPPurchaseCorrection purchaseCorrection = new MCPPurchaseCorrection();
                    purchaseCorrection.SetOrderId(t.OrderId);
                    purchaseCorrection.SetTxnNumber(t.TxnNumber);
                    purchaseCorrection.SetCustomerID(t.CustId);
                    purchaseCorrection.SetDynamicDescriptor(t.DynamicDes);
                    purchaseCorrection.SetCryptType(t.CrtpyType);
                    mpgReq.SetTransaction(purchaseCorrection);
                    break;

                case TransactionType.MCPPreAuth:
                    MCPPreAuth mcpPreAut = new MCPPreAuth();
                    mcpPreAut.SetAmount(t.Amount);
                    mcpPreAut.SetOrderId(t.OrderId);
                    mcpPreAut.SetPan(t.PAN);
                    mcpPreAut.SetExpDate(t.ExpDate);
                    mcpPreAut.SetCryptType(t.CrtpyType);
                    mcpPreAut.SetMCPRateToken(t.MCPRateToken);
                    mcpPreAut.SetCardholderAmount(t.MCPCardholderAmount);
                    mcpPreAut.SetCardholderCurrencyCode(t.MCPCardHolderCurrncy);
                    mcpPreAut.SetMCPVersion(t.McpVersion);                    
                    mpgReq.SetTransaction(mcpPreAut);
                    break;

                case TransactionType.MCPCompletion:
                    MCPCompletion completion = new MCPCompletion();
                    completion.SetAmount(t.Amount);
                    completion.SetCardholderAmount(t.MCPCardholderAmount);
                    completion.SetCardholderCurrencyCode(t.MCPCardHolderCurrncy);
                    completion.SetOrderId(t.OrderId);
                    completion.SetTxnNumber(t.TxnNumber);
                    completion.SetCustId(t.CustId);
                    completion.SetDynamicDescriptor(t.DynamicDes);
                    completion.SetMCPRateToken(t.MCPRateToken);
                    completion.SetCryptType(t.CrtpyType);
                    mpgReq.SetTransaction(completion);
                    break;

                case TransactionType.MCPRefund:
                    MCPRefund refundMcp = new MCPRefund();
                    refundMcp.SetOrderId(t.OrderId);
                    refundMcp.SetTxnNumber(t.TxnNumber);
                    refundMcp.SetDynamicDescriptor(t.DynamicDes);
                    refundMcp.SetCustId(t.CustId);
                    refundMcp.SetCardholderAmount(t.MCPCardholderAmount);
                    refundMcp.SetCardholderCurrencyCode(t.MCPCardHolderCurrncy);
                    refundMcp.SetCryptType(t.CrtpyType);
                    refundMcp.SetMCPRateToken(t.MCPRateToken);
                    mpgReq.SetTransaction(refundMcp);
                    break;

                case TransactionType.MCPIndependentRefund:
                    MCPIndependentRefund indRefundMCP = new MCPIndependentRefund();
                    indRefundMCP.SetOrderId(t.OrderId);
                    indRefundMCP.SetAmount(t.Amount);
                    indRefundMCP.SetCardholderAmount(t.MCPCardholderAmount);
                    indRefundMCP.SetCardholderCurrencyCode(t.MCPCardHolderCurrncy);
                    indRefundMCP.SetMCPRateToken(t.MCPRateToken);
                    indRefundMCP.SetCustId(t.CustId);
                    indRefundMCP.SetDynamicDescriptor(t.DynamicDes);
                    mpgReq.SetTransaction(indRefundMCP);
                    break;

                case TransactionType.MCPResPurchase:
                    MCPResPurchaseCC resPurchase = new MCPResPurchaseCC();
                    resPurchase.SetCryptType(t.CrtpyType);
                    resPurchase.SetDataKey(t.DataKey);
                    resPurchase.SetOrderId(t.OrderId);
                    resPurchase.SetAmount(t.Amount);
                    resPurchase.SetDynamicDescriptor(t.DynamicDes);
                    resPurchase.SetMCPRateToken(t.MCPRateToken);
                    resPurchase.SetCardholderAmount(t.MCPCardholderAmount);
                    resPurchase.SetCardholderCurrencyCode(t.MCPCardHolderCurrncy);
                    mpgReq.SetTransaction(resPurchase);
                    break;
                case TransactionType.MCPResPreAuth:
                    MCPResPreauthCC resPreAuth = new MCPResPreauthCC();
                    resPreAuth.SetCryptType(t.CrtpyType);
                    resPreAuth.SetDataKey(t.DataKey);
                    resPreAuth.SetOrderId(t.OrderId);
                    resPreAuth.SetAmount(t.Amount);
                    resPreAuth.SetDynamicDescriptor(t.DynamicDes);
                    resPreAuth.SetMCPRateToken(t.MCPRateToken);
                    resPreAuth.SetCardholderAmount(t.MCPCardholderAmount);
                    resPreAuth.SetCardholderCurrencyCode(t.MCPCardHolderCurrncy);
                    mpgReq.SetTransaction(resPreAuth);
                    break;
                case TransactionType.MCPResIndependentRefund:
                    MCPResIndRefundCC mcpResIndRefundCC = new MCPResIndRefundCC();
                    mcpResIndRefundCC.SetOrderId(t.OrderId);
                    mcpResIndRefundCC.SetCustId(t.CustId);
                    mcpResIndRefundCC.SetAmount(t.Amount);
                    mcpResIndRefundCC.SetCryptType(t.CrtpyType);
                    mcpResIndRefundCC.SetDataKey(t.DataKey);

                    //MCP Fields
                    mcpResIndRefundCC.SetMCPVersion(t.McpVersion);
                    mcpResIndRefundCC.SetCardholderAmount(t.MCPCardholderAmount);
                    mcpResIndRefundCC.SetCardholderCurrencyCode(t.MCPCardHolderCurrncy);
                    mpgReq.SetTransaction(mcpResIndRefundCC);
                    break;
            }
            try
            {

                response = sendRequestToMonerisGateWay(t, mpgReq);

                if (!string.IsNullOrEmpty(response))
                    InsertDateToDB(SentXML, response, t.OrderId, t.transactionType.ToString(), Message, testVersion, TestCaseNumber);
            }
            catch (Exception)
            {
                return "There is some issue with Code.";
            }
            return response;
        }

        private KountInquiry setKountInquiry(KountInquiry kount_inquiry)
        {
            kount_inquiry.SetKountMerchantId("760000"); //6 digit - This is a UNIQUE local identifier used by the merchant to identify the kount inquiry request
            kount_inquiry.SetKountApiKey("eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJpc3MiOiI3NjAwMDAiLCJhdWQiOiJLb3VudC4xIiwiaWF0IjoxNDg3MTcwMjQ4LCJzY3AiOnsia2EiOm51bGwsImtjIjpudWxsLCJhcGkiOnRydWUsInJpcyI6dHJ1ZX19.LaNqPR2GPYRbiZV0-0KArp325fvFaTBDMVZkwWp1I-U"); //214 character max - This is a UNIQUE local identifier used by the merchant to identify the kount inquiry request
                                                                                                                                                                                                                                                                   // kount_inquiry.SetOrderId("nqa-orderidkount-1"); //64 characters max - This is a UNIQUE local identifier used by the merchant to identify the transaction e.g. purchase order number.
            kount_inquiry.SetCallCenterInd("N"); //Y or N - Risk Inquiry originating from call center environment
            kount_inquiry.SetCurrency("CAD"); //country of currency submitted on order
            kount_inquiry.SetEmail("test@gmail.com"); //email address submitted by the customer
            kount_inquiry.SetDataKey("ak84nklcma9Khdg1"); //token from moneris vault service to represent pan if previously tokenized
            kount_inquiry.SetCustomerId("NQA"); //Merchant assigned account number for consumer
            kount_inquiry.SetAutoNumberId("NQA-X1"); //Automatic Number Identification (ANI) submitted with order
            kount_inquiry.SetFinancialOrderId("nqa-fin-orderid-1"); //64 characters max - This is a local identifier used by the merchant to identify the transaction e.g. purchase order number.
            kount_inquiry.SetPaymentToken("4242424242424242"); //payment 

            kount_inquiry.SetPaymentType("CARD"); //payment type submitted by merchant
            kount_inquiry.SetIpAddress("192.168.2.1"); //Dotted Decimal IPv4 address that the merchant sees coming from the customer
            kount_inquiry.SetSessionId("xjudq804i1049jkjakdad"); //unique session id.  Must be unique over a 30-day span
            kount_inquiry.SetWebsiteId("DEFAULT");
            kount_inquiry.SetAmount("100"); //Transaction amount This must contain at least 3 digits, two of which are penny values
            kount_inquiry.SetPaymentResponse("A"); //A - Authorized, D - Declined - payment transaction response
            kount_inquiry.SetAvsResponse("M"); //M - Match, N - No Match - avs verification response returned from payment request. This can be provided should kount_inquiry be performed after the transaction is complete
            kount_inquiry.SetCvdResponse("M"); //M - Match, N - No Match, X - Unsupported/Unavailable - cvd response returned to merchant from processor. This can be provided should kount_inquiry be performed after the transaction is complete
            kount_inquiry.SetBillStreet1("3300 Bloor Street"); //billing street address line 1
            kount_inquiry.SetBillStreet2("West Tower"); //billing street address line 2
            kount_inquiry.SetBillCountry("CA"); //2 character - billing country code
            kount_inquiry.SetBillCity("Toronto"); //billing address city
            kount_inquiry.SetBillPostalCode("M8X2X2"); //billing address postal code
            kount_inquiry.SetBillPhone("4167341000"); //billing phone number
            kount_inquiry.SetBillProvince("ON"); //billing address province
            kount_inquiry.SetDob("1950-11-12"); //YYYY-MM-DD
            kount_inquiry.SetEpoc("1491783223"); //timestamp expressed as seconds from epoch
            kount_inquiry.SetGender("M"); //M - Male or F - Female
            kount_inquiry.SetLast4("4242"); //last 4 digits of credit card value
            kount_inquiry.SetCustomerName("Moneris Test"); //customer name submitted with the order
            kount_inquiry.SetShipStreet1("3200 Bloor Street"); //shipping street address line 1
            kount_inquiry.SetShipStreet2("East Tower"); //shipping street address line 2
            kount_inquiry.SetShipCountry("CA"); //2 digit - shipping country code
            kount_inquiry.SetShipCity("Toronto"); //shipping address city
            kount_inquiry.SetShipEmail("test@gmail.com"); //email of recipient
            kount_inquiry.SetShipName("Moneris Test"); //name of recipient
            kount_inquiry.SetShipPostalCode("M8X2X3"); //shipping address postal code
            kount_inquiry.SetShipPhone("4167341001"); //ship-to phone number
            kount_inquiry.SetShipProvince("ON"); //shipping address province
            kount_inquiry.SetShipType("ST"); //Same Day = SD, Next Day = ND, Second Day = 2D, Standard = ST

            //Product Details - item number, product_type, product_item (SKU), product_description, product quatity, product_price
            //1-25 products can be added - must be in sequence starting with 1
            kount_inquiry.SetProduct(1, "Phone", "XM9731S", "iPhone 7", "1", "100");
            kount_inquiry.SetProduct(2, "Phone", "YM9731R", "iPhone 6", "1", "100");

            //Local Attributes - 255 character max each,These attributes can be used to pass custom attribute data. These are used if you wish to correlate some data with the returned response via kount
            //1-25 of these can be submitted in one request - must be in sequence starting with 1
            kount_inquiry.SetUdfField("local_custom_1", "iPhone 7");
            kount_inquiry.SetUdf();
            return kount_inquiry;
        }

        private KountUpdate setKountUpdate(KountUpdate kount_update)
        {
            //KountUpdate kount_update = new KountUpdate();

            //kount_update.SetKountTransactionId("PHKH0QSYJN50"); //kount transaction ID number that is returned in the response of a kount_inquiry request
            kount_update.SetKountMerchantId("760000"); //6 digit - This is a UNIQUE local identifier used by the merchant to identify the kount inquiry request
            kount_update.SetKountApiKey("eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJpc3MiOiI3NjAwMDAiLCJhdWQiOiJLb3VudC4xIiwiaWF0IjoxNDg3MTcwMjQ4LCJzY3AiOnsia2EiOm51bGwsImtjIjpudWxsLCJhcGkiOnRydWUsInJpcyI6dHJ1ZX19.LaNqPR2GPYRbiZV0-0KArp325fvFaTBDMVZkwWp1I-U"); //214 character max - This is a UNIQUE local identifier used by the merchant to identify the kount inquiry request
            //kount_update.SetOrderId("nqa-orderidkount-5"); //64 characters max - This is a UNIQUE local identifier used by the merchant to identify the transaction e.g. purchase order number.
            //kount_update.SetFinancialOrderId(); //64 characters max - This is a local identifier used by the merchant to identify the transaction e.g. purchase order number
            kount_update.SetPaymentToken("4242424242424242"); //payment token submitted by merchant (ie: credit card, payer ID)
                                                              /*	Payment Type Must be one of the following values:
                                                              APAY-­Apple Pay
                                                              CARD-­Credit Card
                                                              PYPL-­PayPal
                                                              NONE-­None
                                                              GOOG-­Google Checkout
                                                              GIFT-­Gift Card
                                                              INTERAC-­Interac
                                                              CHEK - Check
                                                              GDMP - Green Dot Money Pack
                                                              BLML - Bill Me Later
                                                              BPAY - BPAY
                                                              NETELLER - Neteller
                                                              GIROPAY - GiroPay
                                                              ELV - ELV
                                                              MERCADE_PAGO - Mercade Pago
                                                              SEPA - Single Euro Payments Area
                                                              CARTE_BLEUE - Carte Bleue
                                                              POLI - POLi
                                                              Skrill/Moneybookers - SKRILL
                                                              SOFORT - Sofort
                                                              */
            kount_update.SetPaymentType("CARD"); //payment type submitted by merchant
            kount_update.SetSessionId("xjudq804i1049jkjakdad");  //unique session id.  Must be unique over a 30-day spa
            kount_update.SetPaymentResponse("A"); //A - Authorized, D - Declined - payment transaction response
            kount_update.SetAvsResponse("M"); //M - Match, N - No Match - avs verification response returned from payment request. This can be provided should kount_inquiry be performed after the transaction is complete
            kount_update.SetCvdResponse("N"); //M - Match, N - No Match, X - Unsupported/Unavailable - cvd response returned to merchant from processor. This can be provided should kount_inquiry be performed after the transaction is complete
            kount_update.SetLast4("4242"); ////last 4 digits of credit card value
            kount_update.SetEvaluate("Y"); //Y or N - If set to Y, full re-evaluation will be performed with Kount.  If unset, default value is N
            kount_update.SetRefundStatus("C"); //R = Refund, C = Chargeback		

            HttpsPostRequest mpgReq = new HttpsPostRequest();
            mpgReq.SetStoreId("intuit_sped");
            mpgReq.SetApiToken("spedguy");
            mpgReq.SetTestMode(true);
            return kount_update;
            //mpgReq.SetTransaction(kount_update);
            //mpgReq.Send();

            //try
            //{
            //    Receipt receipt = mpgReq.GetReceipt();
            //    Console.WriteLine("Response Code = " + receipt.GetResponseCode());
            //    Console.WriteLine("Receipt Id = " + receipt.GetReceiptId());
            //    Console.WriteLine("Message = " + receipt.GetMessage());
            //    Console.WriteLine("Kount Transaction Id = " + receipt.GetKountTransactionId());
            //    Console.WriteLine("Kount Result = " + receipt.GetKountResult());
            //    Console.WriteLine("Kount Score = " + receipt.GetKountScore());
            //    Console.WriteLine("Kount info = " + receipt.GetKountInfo());
            //}
            //catch (Exception e)
            //{
            //    Console.WriteLine(e);
            //}
        }

        private string sendRequestToMonerisGateWay(TransactionBase t, HttpsPostRequest mpgReq)
        {
            try
            {
                string response;
                mpgReq.SetStoreId(t._storeID);
                mpgReq.SetApiToken(t._aPIToken);
                mpgReq.SetProcCountryCode(ProcessingCountry);
                // FOr DR Testing
                // Uncomment HOST property from Base class and uncomment DR store ID
                if (t.Environment == "Prod")
                { mpgReq.SetHost(base.Host); }
                if (t.Environment == "QA")
                {
                    // FOR QA
                    mpgReq.SetTestMode(true);
                }
                if (mpgReq == null)
                {
                    string s = "MPGReq is null";
                    //return;
                }
                //ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
                mpgReq?.Send();
                string s1 = mpgReq.getResponseXML();
                SentXML = mpgReq.getXML();
                Receipt res = mpgReq.GetReceipt();
                response = generateReceipt(mpgReq.GetReceipt());
                return response;
            }
            catch (Exception erre)
            {
                IsValidTransaction = false;
                return string.Empty;
            }
        }

        private static CardVerification doCardverification(TransactionBase t)
        {
            CardVerification cardVerification = new CardVerification();
            cardVerification.SetOrderId(t.OrderId);
            cardVerification.SetPan(t.PAN);
            cardVerification.SetExpDate(t.ExpDate);
            cardVerification.SetCofInfo(t.cofInfo);

            //AvsInfo avsCheckCV = new AvsInfo();
            //avsCheckCV.SetAvsStreetNumber("212");
            //avsCheckCV.SetAvsStreetName("Payton Street");
            //avsCheckCV.SetAvsZipCode("M1M1M1");
            //cardVerification.SetAvsInfo(avsCheckCV);
            CvdInfo cvdCheckCV = new CvdInfo();
            cvdCheckCV.SetCvdIndicator("1");
            cvdCheckCV.SetCvdValue("658");
            cardVerification.SetCvdInfo(cvdCheckCV);
            cardVerification.SetCryptType(t.CrtpyType);
            return cardVerification;
        }

        private static ResUpdateCC doResUpdateCC(TransactionBase t)
        {
            ResUpdateCC resUpdateCC = new ResUpdateCC();
            resUpdateCC.SetDataKey(t.DataKey);
            //resUpdateCC.SetAvsInfo(t.AvsCheck);
            resUpdateCC.SetPan(t.PAN);
            resUpdateCC.SetExpDate(t.ExpDate);
            resUpdateCC.SetCryptType(t.CrtpyType);
            resUpdateCC.SetNote("Update CC Note");
            resUpdateCC.SetCofInfo(t.cofInfo);
            AvsInfo avsCheck = SetAvsInfo("230", "Test Street", "M1M1M1");
            resUpdateCC.SetAvsInfo(avsCheck);
            //resPreauthCC.setExpDate(expdate); //Temp Tokens only
            //Mandatory - Credential on File details
            //resPreauthCC.SetCofInfo(t.cInfo);
            return resUpdateCC;
        }

        private ResPreauthCC doResPreAuth(TransactionBase t)
        {
            ResPreauthCC resPreauthCC = new ResPreauthCC();
            resPreauthCC.SetDataKey(this.DataKey);
            resPreauthCC.SetOrderId(t.OrderId);
            resPreauthCC.SetAmount(t.Amount);
            resPreauthCC.SetCryptType(t.CrtpyType);
            resPreauthCC.SetDynamicDescriptor(t.DynamicDes);
            resPreauthCC.SetCustId(t.CustId);
            resPreauthCC.SetCustInfo(t.CustomerInfo == null ? null : t.CustomerInfo);
            //resPreauthCC.SetMarketIndicator(t.MarketIndicator);
            resPreauthCC.SetCvdInfo(t.CvdCheck);
            resPreauthCC.SetAvsInfo(t.AvsCheck);
            //resPreauthCC.setExpDate(expdate); //Temp Tokens only
            //Mandatory - Credential on File details
            resPreauthCC.SetCofInfo(t.cofInfo);
            return resPreauthCC;
        }

        private static ResPurchaseCC doResPurchase(TransactionBase t)
        {
            ResPurchaseCC resPurchaseCC = new ResPurchaseCC();
            resPurchaseCC.SetDataKey(t.DataKey);
            resPurchaseCC.SetOrderId(t.OrderId);
            resPurchaseCC.SetAmount(t.Amount);
            resPurchaseCC.SetCryptType(t.CrtpyType);
            resPurchaseCC.SetExpDate(t.ExpDate); //Temp Tokens only
            resPurchaseCC.SetCustId(t.CustId);
            //resPurchaseCC.SetMarketIndicator(t.MarketIndicator);
            resPurchaseCC.SetCvdInfo(t.CvdCheck);
            resPurchaseCC.SetAvsInfo(t.AvsCheck);
            resPurchaseCC.SetCustInfo(t.CustomerInfo);
            resPurchaseCC.SetCofInfo(t.cofInfo);
            //if (t.cInfo != null)
            //{
            //    CvdInfo cvdCheck = new CvdInfo();
            //    cvdCheck.SetCvdIndicator("1");
            //    cvdCheck.SetCvdValue("099");
            //    resPurchaseCC.SetCvdInfo(cvdCheck);
            //}
            if (t.RecuringCycle != null)
            {
                resPurchaseCC.SetRecur(t.RecuringCycle);
            }

            return resPurchaseCC;
        }

        private static ResAddCC doResAddCC(TransactionBase t)
        {
            ResAddCC resaddcc = new ResAddCC();
            resaddcc.SetPan(t.PAN);
            resaddcc.SetExpDate(t.ExpDate);
            resaddcc.SetCryptType(t.CrtpyType);
            resaddcc.SetCofInfo(t.cofInfo);
            if (t.cofInfo != null)
            {
                //AvsInfo avsCheck2 = new AvsInfo();
                //avsCheck2.SetAvsStreetNumber("212");
                //avsCheck2.SetAvsStreetName("Payton Street");
                //avsCheck2.SetAvsZipCode("M1M1M1");
                if (t.AvsCheck != null)
                    resaddcc.SetAvsInfo(t.AvsCheck);
            }
            resaddcc.SetDataKeyFormat(t.DataKeyFormat);
            return resaddcc;
        }

        private static ForcePost doForcePost(TransactionBase t)
        {
            ForcePost forcePost = new ForcePost();
            forcePost.SetAmount(t.Amount);
            forcePost.SetOrderId(t.OrderId);
            forcePost.SetPan(t.PAN);
            forcePost.SetExpDate(t.ExpDate);
            forcePost.SetAuthCode(t.AuthCode);
            forcePost.SetCryptType(t.CrtpyType);
            return forcePost;
        }

        private Completion doCompletion(TransactionBase t)
        {
            Completion com = new Completion();
            com.SetOrderId(this.OrderId);
            if (t.ShipIndicator == "P" || t.ShipIndicator == "F")
            {
                com.SetTxnNumber(t.TxnNumber);
            }
            else
            {
                com.SetTxnNumber(this.TxnNumber);
            }
            com.SetAmount(t.Amount);
            com.SetCryptType(t.CrtpyType);
            com.SetShipIndicator(t.ShipIndicator);
            return com;
        }

        private static CavvPreAuth doCavvPreAuth(TransactionBase t)
        {
            CavvPreAuth cavvpreAuth = new CavvPreAuth();
            cavvpreAuth.SetOrderId(t.OrderId);
            cavvpreAuth.SetAmount(t.Amount);
            cavvpreAuth.SetPan(t.PAN);
            cavvpreAuth.SetExpDate(t.ExpDate);
            cavvpreAuth.SetCryptType(t.CrtpyType);
            cavvpreAuth.SetCofInfo(t.cofInfo);
            cavvpreAuth.SetCustInfo(t.CustomerInfo);
            cavvpreAuth.SetDynamicDescriptor(t.DynamicDes);
            cavvpreAuth.SetCavv(t.CAVV);
            return cavvpreAuth;
        }

        private static PreAuth doPreAuth(TransactionBase t)
        {
            PreAuth preAuth = new PreAuth();
            preAuth.SetOrderId(t.OrderId);
            preAuth.SetAmount(t.Amount);
            preAuth.SetPan(t.PAN);
            preAuth.SetExpDate(t.ExpDate);
            preAuth.SetCryptType(t.CrtpyType);
            //preAuth.SetCmId(t.CmID);
            if (t.cofInfo != null)
            { preAuth.SetCofInfo(t.cofInfo); }
            preAuth.SetCustId(t.CustId);
            preAuth.SetCustInfo(t.CustomerInfo == null ? null : t.CustomerInfo);
            preAuth.SetDynamicDescriptor(t.DynamicDes);
            preAuth.SetAvsInfo(t.AvsCheck);
            preAuth.SetMarketIndicator(t.MarketIndicator);
            return preAuth;
        }

        private static IndependentRefund doIndRefund(TransactionBase t)
        {
            IndependentRefund indRefund = new IndependentRefund();
            indRefund.SetOrderId(t.OrderId);
            indRefund.SetAmount(t.Amount);
            indRefund.SetCryptType(t.CrtpyType);
            indRefund.SetPan(t.PAN);
            indRefund.SetExpDate(t.ExpDate);
            return indRefund;
        }

        private static PurchaseCorrection doPurchaseCorrection(TransactionBase t)
        {
            PurchaseCorrection perCorrection = new PurchaseCorrection();
            perCorrection.SetOrderId(t.OrderId);
            perCorrection.SetTxnNumber(t.TxnNumber);
            perCorrection.SetCryptType(t.CrtpyType);
            return perCorrection;
        }

        private static Refund doRefund(TransactionBase t)
        {
            Refund refund = new Refund();
            refund.SetOrderId(t.OrderId);
            refund.SetAmount(t.Amount);
            refund.SetTxnNumber(t.TxnNumber);
            refund.SetCryptType(t.CrtpyType);
            return refund;
        }

        private static CavvPurchase doCavvPurchase(TransactionBase t)
        {
            CavvPurchase cavvPurchase = new CavvPurchase();
            cavvPurchase.SetOrderId(t.OrderId);
            cavvPurchase.SetCustId(t.CustId);
            cavvPurchase.SetAmount(t.Amount);
            cavvPurchase.SetPan(t.PAN);
            cavvPurchase.SetExpDate(t.ExpDate);
            cavvPurchase.SetCavv(t.CAVV);
            cavvPurchase.SetCryptType(t.CrtpyType); //Mandatory for AMEX cards only
            cavvPurchase.SetDynamicDescriptor(t.DynamicDes);
            //cavvPurchase.SetCmId(t.CmID);
            return cavvPurchase;
        }

        private static Purchase doPurchase(TransactionBase t)
        {
            Moneris.Purchase purchase = new Moneris.Purchase();
            purchase.SetOrderId(t.OrderId);
            purchase.SetAmount(t.Amount);
            purchase.SetPan(t.PAN);
            purchase.SetExpDate(t.ExpDate);
            purchase.SetCryptType(t.CrtpyType);
            purchase.SetDynamicDescriptor(t.DynamicDes);
            purchase.SetCofInfo(t.cofInfo);
            purchase.SetCustId(t.CustId);
            //purchase.SetCmId(t.CmID);
            purchase.SetMarketIndicator(t.MarketIndicator);
            purchase.SetCustInfo(t.CustomerInfo);
            purchase.SetCvdInfo(t.CvdCheck);
            purchase.SetAvsInfo(t.AvsCheck);
            if (t.RecuringCycle != null)
            {
                purchase.SetRecur(t.RecuringCycle);
            }
            if (t.IssuerId != null)
            {
                //purchase.setIsis
            }
            if (t.ConvFee != null)
            {
                purchase.SetConvFeeInfo(t.ConvFee);
            }

            return purchase;
        }

        internal ConvFeeInfo setConvFee(string v)
        {
            ConvFeeInfo convFeeInfo = new ConvFeeInfo();
            convFeeInfo.SetConvenienceFee(v);
            return convFeeInfo;
        }

        private static AvsInfo SetAvsInfo(string StreetNumber, string StreetName, string ZipCode)
        {
            AvsInfo avsCheck = new AvsInfo();
            avsCheck.SetAvsStreetNumber(StreetNumber);
            avsCheck.SetAvsStreetName(StreetNumber);
            avsCheck.SetAvsZipCode(ZipCode);
            return avsCheck;
        }

        private void InsertDateToDB(string sentXML, string response, string orderId, string transactionType, string message, string ver, string testcaseName)
        {
            try
            {
                {
                    using (SqlConnection conn = new SqlConnection(connectionStringAWS))
                    {
                        conn.Open();
                        using (SqlCommand cmd = new SqlCommand("INSERT INTO APIReqRes(OrderId,Request, Response, StoreID,APIToken,TransactionTime, TransactionType, Message, ProjectName, Version,  TestCaseNumber) VALUES (@orderID,@request,@response,@storeID,@apiToken,@tt,@transactionType,@message,@projectName, @version, @testcaseName)", conn))
                        {
                            cmd.Parameters.AddWithValue("@orderID", orderId.Trim());
                            cmd.Parameters.AddWithValue("@request", sentXML.Trim());
                            cmd.Parameters.AddWithValue("@response", response.Trim());
                            cmd.Parameters.AddWithValue("@storeID", storeId);
                            cmd.Parameters.AddWithValue("@apiToken", apiToken);
                            cmd.Parameters.AddWithValue("@transactionType", transactionType);
                            cmd.Parameters.AddWithValue("@message", message ?? (object)DBNull.Value);
                            cmd.Parameters.AddWithValue("@tt", System.DateTime.Now);
                            cmd.Parameters.AddWithValue("@projectName", ProjectName ?? (object)DBNull.Value);
                            cmd.Parameters.AddWithValue("@version", ver ?? (object)DBNull.Value);
                            cmd.Parameters.AddWithValue("@testcaseName", testcaseName ?? (object)DBNull.Value);
                            int rows = cmd.ExecuteNonQuery();

                        }
                    }
                }
            }
            catch (Exception rr)
            { }
            finally
            {

            }

        }

        private void InsertDateToDB(string sentXML, string response, string orderId, string transactionType, string message)
        {
            try
            {
                {
                    using (SqlConnection conn = new SqlConnection(connectionStringAWS))
                    {
                        conn.Open();
                        using (SqlCommand cmd = new SqlCommand("INSERT INTO APIReqRes(OrderId,Request, Response, StoreID,APIToken,TransactionTime, TransactionType, Message, ProjectName) VALUES (@orderID,@request,@response,@storeID,@apiToken,@tt,@transactionType,@message,@projectName)", conn))
                        {
                            cmd.Parameters.AddWithValue("@orderID", orderId.Trim());
                            cmd.Parameters.AddWithValue("@request", sentXML.Trim());
                            cmd.Parameters.AddWithValue("@response", response.Trim());
                            cmd.Parameters.AddWithValue("@storeID", storeId);
                            cmd.Parameters.AddWithValue("@apiToken", apiToken);
                            cmd.Parameters.AddWithValue("@transactionType", transactionType);
                            cmd.Parameters.AddWithValue("@message", message);
                            cmd.Parameters.AddWithValue("@tt", System.DateTime.Now);
                            cmd.Parameters.AddWithValue("@projectName", ProjectName ?? (object)DBNull.Value);
                            int rows = cmd.ExecuteNonQuery();
                            //rows number of record got inserted
                        }
                    }
                }
            }
            catch (Exception rr)
            { }
            finally
            {

            }

        }

        public Recur setRecurrInfo(string numberOfRecur, string recurPeriod, string recurAmount, string recureUnit = "month", string startNow = "true", string startDate = "2018/07/28")
        {

            string recur_unit = recureUnit; //eom = end of month
            string start_now = startNow;
            string start_date = startDate;
            string num_recurs = numberOfRecur;
            string period = recurPeriod;
            string recur_amount = recurAmount;
            //string processing_country_code = "CA";
            //bool status_check = false;

            /************************* Recur Object Option1 ******************************/

            Hashtable recur_hash = new Hashtable();
            recur_hash.Add("recur_unit", recur_unit);
            recur_hash.Add("start_now", start_now);
            recur_hash.Add("start_date", start_date);
            recur_hash.Add("num_recurs", num_recurs);
            recur_hash.Add("period", period);
            recur_hash.Add("recur_amount", recur_amount);
            Recur recurring_cycle = new Recur(recur_hash);
            return recurring_cycle;
        }

        public string generateReceipt(Receipt receipt)
        {
            string rCode = receipt.GetResponseCode();
            StringBuilder response = new StringBuilder();

            if (cofInfo != null && !string.IsNullOrEmpty(receipt.GetIssuerId()))
            {
                IsValidTransaction = true;
            }
            else if (!string.IsNullOrEmpty(receipt.GetResponseCode()))
            {
                if (cofInfo == null && (!string.IsNullOrEmpty(receipt.GetResponseCode())))
                {
                    if (receipt.GetResponseCode().Trim() != "null")
                    {
                        IsValidTransaction = (Convert.ToInt32(receipt.GetResponseCode()) < 50);
                    }
                    
                }
            }
            else if (string.IsNullOrEmpty(receipt.GetResponseCode()))
            {
                IsValidTransaction = false;
            }


            int rates = receipt.GetRatesCount();
            if (rates > 0)
            {

                response.Append("RateTxnType = " + receipt.GetRateTxnType());
                response.Append("MCPRateToken = " + receipt.GetMCPRateToken());
                response.Append("RateInqStartTime = " + receipt.GetRateInqStartTime());  //The time (unix UTC) of when the rate is requested
                response.Append("RateInqEndTime = " + receipt.GetRateInqEndTime());  //The time (unix UTC) of when the rate is returned
                response.Append("RateValidityStartTime = " + receipt.GetRateValidityStartTime());    //The time (unix UTC) of when the rate is valid from
                response.Append("RateValidityEndTime = " + receipt.GetRateValidityEndTime());    //The time (unix UTC) of when the rate is valid until
                response.Append("RateValidityPeriod = " + receipt.GetRateValidityPeriod());  //The time in minutes this rate is valid for	            
                response.Append("ResponseCode = " + receipt.GetResponseCode());
                response.Append("Message = " + receipt.GetMessage());
                response.Append("Complete = " + receipt.GetComplete());
                response.Append("TransDate = " + receipt.GetTransDate());
                response.Append("TransTime = " + receipt.GetTransTime());
                response.Append("TimedOut = " + receipt.GetTimedOut());
                MCPRateToken = receipt.GetMCPRateToken();
                //RateData
                for (int index = 0; index < receipt.GetRatesCount(); index++)
                {
                    response.Append("MCPRate = " + receipt.GetMCPRate(index));
                    response.Append("MerchantSettlementCurrency = " + receipt.GetMerchantSettlementCurrency(index));
                    response.Append("MerchantSettlementAmount = " + receipt.GetMerchantSettlementAmount(index));    //Domestic(CAD) amount
                    response.Append("CardholderCurrencyCode = " + receipt.GetCardholderCurrencyCode(index));
                    response.Append("CardholderAmount = " + receipt.GetCardholderAmount(index));    //Foreign amount
                    response.Append("MCPErrorStatusCode = " + receipt.GetMCPErrorStatusCode(index));
                    response.Append("MCPErrorMessage = " + receipt.GetMCPErrorMessage(index));
                    if (string.IsNullOrEmpty(receipt.GetMCPErrorStatusCode(index))) {
                        MCPErrorCodeshown = true;
                    }
                    else { MCPErrorCodeshown = false; }
                }

            }



            string[] ecr2 = receipt.GetTerminalIDs();
            if (ecr2.Length == 0 && rates == 0)
            {
                response.Append("==================== Response ================== \n");
                response.Append("\nCardType = " + receipt.GetCardType());
                response.Append("\nTransAmount = " + receipt.GetTransAmount());
                response.Append("\nTxnNumber = " + receipt.GetTxnNumber());
                response.Append("\nReceiptId = " + receipt.GetReceiptId());
                response.Append("\nTransType = " + receipt.GetTransType());
                response.Append("\nReferenceNum = " + receipt.GetReferenceNum());
                response.Append("\nResponseCode = " + receipt.GetResponseCode());
                response.Append("\nISO = " + receipt.GetISO());
                response.Append("\nBankTotals = " + receipt.GetBankTotals());
                response.Append("\nMessage = " + receipt.GetMessage());
                response.Append("\nAuthCode = " + receipt.GetAuthCode());
                response.Append("\nComplete = " + receipt.GetComplete());
                response.Append("\nTransDate = " + receipt.GetTransDate());
                response.Append("\nTransTime = " + receipt.GetTransTime());
                response.Append("\nTicket = " + receipt.GetTicket());
                response.Append("\nTimedOut = " + receipt.GetTimedOut());
                response.Append("\nIsVisaDebit = " + receipt.GetIsVisaDebit());
                response.Append("\nHostId = " + receipt.GetHostId());
                response.Append("\nData Key = " + receipt.GetDataKey());
                response.Append("\nIssuerId = " + receipt.GetIssuerId());
                response.Append("\nCVDReposneCode = " + receipt.GetCvvResponseCode());
                response.Append("\nCVDResultCode = " + receipt.GetCvdResultCode());
                response.Append("\nAVSResponseCode = " + receipt.GetAvsResponseCode());
                response.Append("\nAVSResultCode = " + receipt.GetAvsResultCode());
                response.Append("\nMasked PAN = " + receipt.GetResDataMaskedPan());
                response.Append("\nFull PAN = " + receipt.GetResDataPan());
                response.Append("\nFee Amount = " + receipt.GetFeeAmount());
                response.Append("\nFee Type = " + receipt.GetFeeType());
                response.Append("\nFee Rate = " + receipt.GetFeeRate());
                response.Append("MCPErrorStatusCode = " + receipt.GetMCPErrorStatusCode());
                response.Append("MCPErrorMessage = " + receipt.GetMCPErrorMessage());
                //response.Append("\nTerminal = " + receipt.GetTerminalIDs());
                DataKey = receipt.GetDataKey();
                IssuerId = receipt.GetIssuerId();
                TxnNumber = receipt.GetTxnNumber();
                Message = receipt.GetMessage();
                OrderId = receipt.GetReceiptId();
                if (!string.IsNullOrEmpty(receipt.GetKountTransactionId()))
                {
                    this.kountTransactionsId = receipt.GetKountTransactionId();
                    response.Append("Kount Transaction ID =" + receipt.GetKountTransactionId());
                    response.Append("Kount Result = " + receipt.GetKountResult());
                    response.Append("Kount Score = " + receipt.GetKountScore());
                    response.Append("Kount info = " + receipt.GetKountInfo());
                }
                this.OrderId = receipt.GetReceiptId();
            }
            foreach (string ecr in receipt.GetTerminalIDs())
            {
                response.Append("ECR: " + ecr);
                foreach (string cardType in receipt.GetCreditCards(ecr))
                {
                    response.Append("\tCard Type: " + cardType);

                    response.Append("\t\tPurchase: Count = "
                                        + receipt.GetPurchaseCount(ecr, cardType)
                                        + " Amount = "
                                        + receipt.GetPurchaseAmount(ecr,
                                                                      cardType));

                    response.Append("\t\tRefund: Count = "
                                        + receipt.GetRefundCount(ecr, cardType)
                                        + " Amount = "
                                        + receipt.GetRefundAmount(ecr, cardType));

                    response.Append("\t\tCorrection: Count = "
                                        + receipt.GetCorrectionCount(ecr, cardType)
                                        + " Amount = "
                                        + receipt.GetCorrectionAmount(ecr,
                                                                       cardType));

                }
            }
            return response.ToString();
        }

        public CofInfo setCardOnFile(string indicator, string info, string IssuerId = null)
        {
            CofInfo cofIntial = new CofInfo();
            cofIntial.SetPaymentIndicator(indicator);
            cofIntial.SetPaymentInformation(info);
            cofIntial.SetIssuerId(IssuerId);
            return cofIntial;
        }

        public  AvsInfo setValidAVSInfo()
        {
            AvsInfo avsCheck = new AvsInfo();
            avsCheck.SetAvsStreetNumber("212");
            avsCheck.SetAvsStreetName("Payton Street");
            avsCheck.SetAvsZipCode("M1M1M1");
            return avsCheck;
        }

        public CvdInfo setValidCVDInfo()
        {
            CvdInfo cvdCheck = new CvdInfo();
            cvdCheck.SetCvdIndicator("1");
            cvdCheck.SetCvdValue("123");
            return cvdCheck;
        }

        public string getissuerId()
        {
            return IssuerId;
        }

        public string setPAN(string PAN)
        {
            if (PAN == "A")
            {
                return "4242424242424242";
            }
            else if (PAN == "B")
            {
                return "4111111111111111";
            }
            else if (PAN == "C")
            {
                return "373599005095005";
            }
            else if (PAN == "D")
            {
                return "4242424254545447";
            }
            else if (PAN == "E")
            {
                return "5454545442545459";
            }
            else if (PAN == "F")
            {
                return "5454545442545442";
            }
            else { return "4242424242424242"; }
        }

        public CustInfo setCustomerInfo()
        {
            CustInfo customer = new CustInfo();

            /********************* Billing/Shipping Variables ****************************/

            String first_name = "Bob";
            String last_name = "Smith";
            String company_name = "ProLine Inc.";
            String address = "623 Bears Ave";
            String city = "Chicago";
            String province = "Illinois";
            String postal_code = "M1M2M1";
            String country = "Canada";
            String phone = "777-999-7777";
            String fax = "777-999-7778";
            String tax1 = "10.00";
            String tax2 = "5.78";
            String tax3 = "4.56";
            String shipping_cost = "10.00";

            /********************* Order Line Item Variables *****************************/

            String[] item_description = new String[] { "Chicago Bears Helmet", "Soldier Field Poster" };
            String[] item_quantity = new String[] { "1", "1" };
            String[] item_product_code = new String[] { "CB3450", "SF998S" };
            String[] item_extended_amount = new String[] { "150.00", "19.79" };


            /********************** Set Customer Billing Information **********************/

            customer.SetBilling(first_name, last_name, company_name, address, city,
                    province, postal_code, country, phone, fax, tax1, tax2,
                    tax3, shipping_cost);

            /******************** Set Customer Shipping Information ***********************/

            customer.SetShipping(first_name, last_name, company_name, address, city,
                    province, postal_code, country, phone, fax, tax1, tax2,
                    tax3, shipping_cost);

            /***************************** Order Line Items  ******************************/

            customer.SetItem(item_description[0], item_quantity[0], item_product_code[0], item_extended_amount[0]);

            customer.SetItem(item_description[1], item_quantity[1], item_product_code[1], item_extended_amount[1]);
            customer.SetEmail("test@moneris.com");
            customer.SetInstructions("Please drop it at front door");
            return customer;
        }
    }
}




//public void SetStoreID(string storeId)
//{
//    if (storeId == "A")
//    {
//        this.storeId = "monca00923";
//        this.APIToken = "FM2wzcAADmRVhSoxGPen";
//    }
//    else if (storeId == "B")
//    {
//        this.StoreID = "monmpg0342";
//        this.APIToken = "6f7dbSFCJfL30tt1JsCM";
//    }
//    else if (storeId == "C")
//    {
//        this.StoreID = "monca00002";
//        this.APIToken = "giftguy";
//    }
//    else if (storeId == "D")
//    {
//        this.StoreID = "monca00597";
//        this.APIToken = "hxgbcbBXNEASN6wiGLFP";
//    }

//}

//public Recur setRecurrInfo(string numberOfRecur, string recurPeriod, string recurAmount, string recureUnit = "month", string startNow = "true", string startDate = "2018/07/28")
//{

//    string recur_unit = recureUnit; //eom = end of month
//    string start_now = startNow;
//    string start_date = startDate;
//    string num_recurs = numberOfRecur;
//    string period = recurPeriod;
//    string recur_amount = recurAmount;
//    //string processing_country_code = "CA";
//    //bool status_check = false;

//    /************************* Recur Object Option1 ******************************/

//    Hashtable recur_hash = new Hashtable();
//    recur_hash.Add("recur_unit", recur_unit);
//    recur_hash.Add("start_now", start_now);
//    recur_hash.Add("start_date", start_date);
//    recur_hash.Add("num_recurs", num_recurs);
//    recur_hash.Add("period", period);
//    recur_hash.Add("recur_amount", recur_amount);
//    Recur recurring_cycle = new Recur(recur_hash);
//    return recurring_cycle;
//}


//public String performPurchase(string orderId, string amount, string pan, string expDate, string cryptType, string dynamicDescripotr, bool statucCheck, CofInfo cof)
//{

//    Moneris.Purchase purchase = new Moneris.Purchase();
//    //purchase.SetOrderId(orderId);
//    //purchase.SetAmount(amount);
//    //purchase.SetPan(pan);
//    //purchase.SetExpDate(expDate);
//    //purchase.SetCryptType(cryptType);
//    //purchase.SetDynamicDescriptor(dynamicDescripotr);
//    //purchase.SetWalletIndicator(""); //Refer to documentation for details
//    // purchase.SetCofInfo(cof);
//    Transaction t = new Transaction();

//    HttpsPostRequest mpgReq = new HttpsPostRequest();
//    mpgReq.SetProcCountryCode(ProcessingCountry);
//    mpgReq.SetTestMode(true); //false or comment out this line for production transactions
//    mpgReq.SetStoreId(StoreID);
//    mpgReq.SetApiToken(APIToken);
//    mpgReq.SetTransaction(purchase);
//    mpgReq.SetStatusCheck(statucCheck);
//    mpgReq.Send();
//    mpgReq.getResponseXML();

//    string s = mpgReq.getResponseXML();
//    try
//    {
//        Receipt receipt = mpgReq.GetReceipt();
//        IssuerId = receipt.GetIssuerId();
//        return generateReceipt(receipt);
//    }
//    catch
//    {
//        return "Can not process a Transaction from Code";
//    }
//}