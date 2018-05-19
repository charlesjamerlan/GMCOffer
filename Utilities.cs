using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GMCOffer.Common
{
    public class Utilities
    {
        public static bool IsValidAge(int YearOfBirth, int AgeLimit)
        {
            DateTime dtNow = DateTime.Today;
            int Age = dtNow.Year - YearOfBirth;
            if (Age < AgeLimit)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public static string GetImagePath()
        {
            return System.Configuration.ConfigurationManager.AppSettings["ImagePath"];
        }

        public static bool IsLive()
        {
            string environmentMode = System.Configuration.ConfigurationManager.AppSettings["Environment"];
            return environmentMode == "DEV" ? false : true;
        }

        public static string GetAddressValidationEndpoint()
        {
            string environmentMode = System.Configuration.ConfigurationManager.AppSettings["Environment"];
            return environmentMode == "DEV" ? System.Configuration.ConfigurationManager.AppSettings["AddressValidationEndPointDev"] : System.Configuration.ConfigurationManager.AppSettings["AddressValidationEndPointLive"];
        }

        public static bool CheckDomainControl(Uri Referrer)
        {
            bool redirect = false;

            //Check for domain control 
            int domainControl = int.Parse(System.Configuration.ConfigurationManager.AppSettings["DomainControl"]);
            if (domainControl == 1)
            {

                //Check mode
                string domainControlMode = System.Configuration.ConfigurationManager.AppSettings["DomainControlMode"];

                //WhiteList Mode
                if (domainControlMode == "whitelist")
                {
                    if (Referrer == null)
                    {
                        redirect = true;
                    }
                    else
                    {
                        string whiteList = System.Configuration.ConfigurationManager.AppSettings["WhiteList"];
                        if (!whiteList.Split(',').Contains(Referrer.Authority.ToString()))
                        {
                            redirect = true;
                        }
                    }
                }
                else if (domainControlMode == "blacklist")
                {
                    if (Referrer != null)
                    {
                        string blackList = System.Configuration.ConfigurationManager.AppSettings["BlackList"];
                        if (blackList.Split(',').Contains(Referrer.Authority.ToString()))
                        {
                            redirect = true;
                        }
                    }
                }
            }

            return redirect;
        }
    }
}