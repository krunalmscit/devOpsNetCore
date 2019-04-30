namespace MonerisTransactionBAL
{
    public class BaseClass
    {

        internal string _storeID { get; set; }
        internal string _aPIToken { get; set; }
        public string ProcessingCountry { get; set; }
        public string Environment { get; set; }

        protected string Host = "https://www3.moneris.com/gateway2/servlet/MpgRequest";

        //DR IP address
        //protected string Host = "https://192.168.192.12/gateway2/servlet/MpgRequest";

        // Tier 1 URL
        //protected string Host = "https://192.168.173.34/gateway2/servlet/MpgRequest";

        //Tier 2 URL
        // protected string Host = "https://192.168.173.35/gateway2/servlet/MpgRequest";

        //Tier 3 URL

        public BaseClass()
        {
            //StoreID = "monca02932";
            //APIToken = "CG8kYzGgzVU5z23irgMx";

            //_storeID = "monca00597";
            //_aPIToken = "hxgbcbBXNEASN6wiGLFP";

            //StoreID = "moneris";
            //APIToken = "hurgle";

            //for DR Testing
            //StoreID = "moneris";
            //APIToken = "EB6I4LlAHrSy2Y50oSXH";

            //StoreID = "monca27330";
            //APIToken = "uikY7EqRT27zIOueYcuG";


            // FOr COnv Fee
            //StoreID = "monca00392";
            //APIToken = "qYdISUhHiOdfTr1CLNpN";
            ProcessingCountry = "CA";
        }

       
        internal void setProdcutionEnvironment()
        {
            _storeID = "moneris";
            _aPIToken = "EB6I4LlAHrSy2Y50oSXH";
            Environment = "Prod";
        }

        internal void setQAEnvironment()
        {
            _storeID = "monca00597";
            _aPIToken = "7Xq0zhMcaVKBCkAV4rX5";
            Environment = "QA";
        }
    }


}
