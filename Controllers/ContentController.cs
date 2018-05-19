using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Caching;
using GMCOffer.Models;

namespace GMCOffer.Controllers
{
    public class ContentController : Controller
    {
        public string _InventoryID = System.Configuration.ConfigurationManager.AppSettings["InventoryID"];

        public string GetVariantDescription(int ccgsku)
        {
            string description = System.Configuration.ConfigurationManager.AppSettings["Description_" + ccgsku.ToString()];
            if (String.IsNullOrEmpty(description))
            {
                description = System.Configuration.ConfigurationManager.AppSettings["Description_Default"];
            }
            return description;
        }

        public int GetSortOrder(int ccgsku)
        {
            string sort = System.Configuration.ConfigurationManager.AppSettings["Sort_" + ccgsku.ToString()];
            if (String.IsNullOrEmpty(sort))
            {
                sort = "0";
            }
            return int.Parse(sort);
        }

        public ActionResult Index()
        {
            //Check for domain control 
            if (Common.Utilities.CheckDomainControl(Request.UrlReferrer))
            {
                string domainControlRedirect = System.Configuration.ConfigurationManager.AppSettings["DomainControlRedirect"];
                return Redirect(domainControlRedirect);
           } 

            //Consume API to get inventory
            //First check cache to see if already exists 
            IList<Offer> offers = new List<Offer>();

            if (System.Web.HttpContext.Current.Cache["OfferList"] == null)
            {
                int secondsExpiration = int.Parse(System.Configuration.ConfigurationManager.AppSettings["SecondsExpiration"]);
                SampleMetrics.piiSoapClient pii = new SampleMetrics.piiSoapClient();
                SampleMetrics.SampleSKU[] ccgskus = pii.GetCurrentSKUInventory(_InventoryID, string.Empty);

                foreach (SampleMetrics.SampleSKU ccgsku in ccgskus)
                {
                    Offer offer = new Offer();
                    offer.ccgsku = int.Parse(ccgsku.SKU);
                    offer.title = ccgsku.SampleDescription.Replace("®", "<sup>&reg;</sup>");
                    offer.description = GetVariantDescription(offer.ccgsku);
                    offer.boh = int.Parse(ccgsku.BalanceOnHand);
                    offer.active = ccgsku.Active;
                    offer.sort = GetSortOrder(offer.ccgsku);
                    offers.Add(offer);
                }
                HttpContext.Cache.Add("OfferList", offers, null, Cache.NoAbsoluteExpiration, new TimeSpan(0, 0, secondsExpiration), CacheItemPriority.Normal, null);
            }
            else
            {
                //get offerlist from Cache
                offers = (List<Offer>)System.Web.HttpContext.Current.Cache["OfferList"];
            }

            //lets check if all variants are depleted
            int totalInventory = offers.Sum(x => x.boh);
            if (totalInventory <= 0)
            {
                //Redirect to expired
                return Redirect("/expired");
            }

            return View("sample", offers);
        }

        [HttpPost]
        public ActionResult Index(FormCollection form)
        {
            //CCG Information
            string inventoryID = _InventoryID;
            string source = System.Configuration.ConfigurationManager.AppSettings["Source"];
            string accesskey = System.Configuration.ConfigurationManager.AppSettings["AccessKey"];

            //collect data
            string trafficsource = String.IsNullOrEmpty(Request.QueryString[System.Configuration.ConfigurationManager.AppSettings["SourceTag"]]) ? "" : Request.QueryString[System.Configuration.ConfigurationManager.AppSettings["SourceTag"]];
            string registrationsource = "";
            string firstname = form["firstname"];
            string lastname = form["lastname"]; 
            string streetaddress1 = form["streetaddress1"];
            string streetaddress2 = form["streetaddress2"];
            string city = form["city"];
            string state = form["state"];
            string zip = form["zip"];
            string country = "US";
            string emailaddress = form["emailaddress"];
            string emailconfirmation = form["emailconfirmation"];
            string gender = "";
            string birthdayyear = form["birthdayyear"];
            string birthdate = "1/1/" + birthdayyear;
            string ccgsku = form["variants"];
            string optin1 = String.IsNullOrEmpty(form["optin1"]) ? "N" : "Y";
            string optin2 = "";
            string optin3 = "";
            string optin4 = "";
            string useragent = Request.UserAgent;
            string isRequiredBirthDate = "Y";
            string isRequiredEmail = "Y";
            string isRequiredAddressValidation = form["overrideValidation"] == "on" ? "N" : "Y";

            //additional strings 1-25           
            string question1 = form["question-1"];
            string question2 = form["question-2"];
            string question3 = form["question-3"];
            string question310other = form["question-3-10-other"];
            string question26other = form["question-2-6-other"];
            string cm_mmc = String.IsNullOrEmpty(Request.QueryString["cm_mmc"]) ? "" : Request.QueryString["cm_mmc"];

            string additional1 = cm_mmc;
            string additional2 = string.Empty;
            string additional3 = string.Empty;
            string additional4 = string.Empty;
            string additional5 = string.Empty;
            string additional6 = string.Empty;
            string additional7 = string.Empty;
            string additional8 = string.Empty;
            string additional9 = string.Empty;

            //as per CCG: Additional question information must be passed from text containers 10 and above
            string additional10 = question1;
            string additional11 = question2;
            string additional12 = question26other;
            string additional13 = question3;
            string additional14 = question310other;
            string additional15 = string.Empty;
            string additional16 = string.Empty;
            string additional17 = string.Empty;
            string additional18 = string.Empty;
            string additional19 = string.Empty;
            string additional20 = string.Empty;
            string additional21 = string.Empty;
            string additional22 = string.Empty;
            string additional23 = string.Empty;
            string additional24 = string.Empty;
            string additional25 = string.Empty;
            
            //Invoke API
            SampleMetrics.piiSoapClient pii = new SampleMetrics.piiSoapClient();
            SampleMetrics.Results result = pii.Submit(inventoryID, source, accesskey, trafficsource, registrationsource, firstname, lastname, streetaddress1, streetaddress2, city, state, zip, country, emailaddress, gender, birthdate, ccgsku,
                       optin1, optin2, optin3, optin4, additional1, additional2, additional3, additional4, additional5, additional6, additional7, additional8, additional9, additional10, additional11, additional12,
                       additional13, additional14, additional15, additional16, additional17, additional18, additional19, additional20, additional21, additional22, additional23, additional24, additional25, useragent,
                       isRequiredBirthDate, isRequiredEmail, isRequiredAddressValidation);

            //Implement action depending on return code
            if (result.Successful)
            {
                //Collect Velocity Information

                return Redirect("/thankyou?id=" + result.OrderNumber.ToString());
            }
            else
            {
                //check for error codes
                string resultCode = string.Empty;
               
                foreach (SampleMetrics.WebServiceError error in result.Errors)
                {                    
                    resultCode = error.Code;

                    if (resultCode == "F12")
                    {
                        return Redirect("/get?error=multipleaddress");
                    }
                    else if (resultCode == "F08")
                    {
                        return Redirect("/get?error=invalidemail");
                    }
                    else if (resultCode == "F11")
                    {
                        return Redirect("/eligibility");
                    }
                    else if (resultCode == "S11")
                    {
                        return Redirect("/expired");
                    }
                    else if (resultCode == "S22")
                    {
                        return Redirect("/limit");
                    }
                    else if (resultCode == "S23")
                    {
                        //this should never occur as the address is prevalidated before submission
                        return Redirect("/get?error=invalidaddress");
                    }
                    else if (resultCode == "S24")
                    {
                        return Redirect("/limit");
                    }           
                }
            }


            return View("sample");
        }


        public ActionResult Eligibility()
        {
            return View("eligibility");
        }   

        public ActionResult ThankYou()
        {
            return View("thankyou");
        }

        public ActionResult Limit()
        {
            return View("limit");
        }

        public ActionResult Error()
        {
            return View("error");
        }

        public ActionResult Expired()
        {
            return View("expired");
        }

    }
}
