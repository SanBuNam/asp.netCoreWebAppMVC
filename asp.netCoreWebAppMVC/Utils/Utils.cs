using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Security;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.Office.Core;
using PowerPoint = Microsoft.Office.Interop.PowerPoint;

using ClosedXML.Excel;
using DocumentFormat.OpenXml.Packaging;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using UAParser;

using WebPortal.Utils.Models;
using System.Web.Mvc;
using WebPortal.Utils.Models.API.v1.Interfaces;

namespace WebPortal.Utils
{
    public static class Utils
    {
        public const string SumitErrorMessage = "Sorry, an unexpected error occurred while processing your request. Please try again later.";
        public const string ServerBusyMessage = "Sorry, the action cannot be completed because the other session is busy. Please try again later.";
        public const string TemporaryProbelmMessage = "This is a temporary problem. Please refresh the page in a few minutes and try again.";
        public const string NameRegex = @"^[a-zA-Z]+(([\'\,\.\- ][a-zA-Z ])?[a-zA-Z]*)*$";
        public const string ErrorFirstnameValidationMsg = "First name may contain letters only";
        public const string ErrorMiddlenameValidationMsg = "First name may contain letters only";
        public const string ErrorLastnameValidationMsg = "Last name may contain letters only";

        public const string EmailRegex = @"^([\w-\.\+]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$";
        public const string ErrorEmailalidationMsg = "That doesn't look like an email.";
        public const string CorrectAndSubmitMsg = "Please correct and submit again.";

        public const string PasswordPolicy = @"^(?=.{8,32})(?=.*\d)(?=.*[a-zA-Z])([a-zA-Z0-9@#$!%^&*()\-_+;,]+)$";
        public const string ErrorPasswordPolicyMsg = "Please read through the requirements above to create your new password";

        public const string StreetAddressRegex = @"^[A-Za-z0-9.,;: áéíóúñÁÉÍÓÚÑâêîôûÂÊÎÔÛ&#-\/]+$";
        public const string ErrorStreeAddressValidationMsg = "That doesn’t look like an address";

        public const string SSNRegex = @"^\d{9}|\d{3}-\d{2}-\d{4}$";
        public const string ErrorSSNValidationMsg = "Invalid Social Security Number";

        public const string CityRegex = @"^[ A-Za-záéíóúñÁÉÍÓÚÑâêîôûÂÊÎÔÛ]*$";
        public const string ErrorCityValidationMsg = "That doesn’t look like a city";
        public const string ZipCodeRegex = @"\(?\d{5}\)?-? *\d{4}";
        public const string PhoneRegex = @"\(?\d{3}\)?-? *\d{3}-? *-?\d{4}";
        public const string ErrorNumericOnlyMsg = "Numeric only";

        public enum JsonFormat
        {
            None,
            Indented
        }

        #region Generator
        private class Generator
        {
            #region Properties and Fields
            public int MinUpperCaseChars { get; set; }
            public int MinLowerCaseChars { get; set; }
            public int MinNumericChars { get; set; }
            public int MinSpecialChars { get; set; }
            public System.Char FillRest { get; set; }
            #endregion

            #region Public Methods
            public string GeneratePassword(int length)
            {
                int sum = this.MinUpperCaseChars + this.MinLowerCaseChars +
                          this.MinNumericChars + this.MinSpecialChars;

                if (length < sum)
                    throw new ArgumentException("length parameter must be valid!");

                List<char> chars = new List<char>();

                if (this.MinUpperCaseChars > 0)
                    chars.AddRange(GetUpperCasePasswordChars(this.MinUpperCaseChars));

                if (this.MinLowerCaseChars > 0)
                    chars.AddRange(GetLowerCasePasswordChars(this.MinLowerCaseChars));

                if (this.MinNumericChars > 0)
                    chars.AddRange(GetNumericPasswordChars(this.MinNumericChars));

                if (this.MinSpecialChars > 0)
                    chars.AddRange(GetSpecialPasswordChars(this.MinSpecialChars));

                int restLength = length - chars.Count;

                if (System.Char.IsUpper(this.FillRest))
                    chars.AddRange(GetUpperCasePasswordChars(restLength));
                else if (System.Char.IsLower(this.FillRest))
                    chars.AddRange(GetLowerCasePasswordChars(restLength));
                else if (System.Char.IsNumber(this.FillRest))
                    chars.AddRange(GetNumericPasswordChars(restLength));
                else if (System.Char.IsPunctuation(this.FillRest))
                    chars.AddRange(GetSpecialPasswordChars(restLength));
                else
                    chars.AddRange(GetLowerCasePasswordChars(restLength));

                return GeneratePasswordFromList(chars);
            }
            #endregion

            #region Private Methods
            private List<char> GetUpperCasePasswordChars(int count)
            {
                List<char> result = new List<char>();
                Random random = new Random();
                for (int index = 0; index < count; index++)
                    result.Add(Char.ToUpper(Convert.ToChar(random.Next(65, 90))));
                return result;
            }

            private List<char> GetLowerCasePasswordChars(int count)
            {
                List<char> result = new List<char>();
                Random random = new Random();
                for (int index = 0; index < count; index++)
                    result.Add(Char.ToLower(Convert.ToChar(random.Next(97, 122))));
                return result;
            }

            private List<char> GetNumericPasswordChars(int count)
            {
                List<char> result = new List<char>();
                Random random = new Random();
                for (int index = 0; index < count; index++)
                    result.Add(Convert.ToChar(random.Next(0, 9).ToString()));
                return result;
            }

            private List<char> GetSpecialPasswordChars(int count)
            {
                List<char> result = new List<char>();
                Random random = new Random();
                for (int index = 0; index < count; index++)
                    result.Add(Char.ToLower(Convert.ToChar(random.Next(35, 38))));
                return result;
            }

            private string GeneratePasswordFromList(List<char> chars)
            {
                string result = string.Empty;
                Random random = new Random();
                while (chars.Count > 0)
                {
                    int randomIndex = random.Next(0, chars.Count);
                    result += chars[randomIndex];
                    chars.RemoveAt(randomIndex);
                }
                return result;
            }
            #endregion
        }
        #endregion

        private static void ProcessException(Exception ex, int referenceNo, int stackNumber = 3, string content = null)
        {
            (new SessionManager()).CheckException(ex, HttpContext.Current);

            string userName = HttpContext.Current.User.Identity.IsAuthenticated ? HttpContext.Current.User.Identity.Name : null;
            string functionName = HttpContext.Current.Request.Url.ToString();

            Log.WriteToDb(Log.LogType.ProgramError, ex.Message, referenceNo.ToString() + (string.IsNullOrEmpty(content) ? "" : ": " + content), functionName, userName);
        }

        #region General 
        public static bool ValidationEmail(string email)
        {
            Match match = Regex.Match(email, EmailRegex, RegexOptions.IgnoreCase);

            return match.Success;
        }

        public static string GetGeolocation(string ipAddress)
        {
            bool geolocationAPIOn = bool.Parse(GetAppSetting("IPAddressGeolocationAPIOn"));
            string result = string.Empty, returnValue = string.Empty;
            string url = string.Format(GetAppSetting("IPAddressGeolocationAPI"), ipAddress);

            Geolocation geolocation = new Geolocation();

            try
            {
                if (geolocationAPIOn)
                {
                    WebClient client = new WebClient();
                    result = client.DownloadString(url);
                    geolocation = Deserialize<Geolocation>(result);
                    returnValue = geolocation.city + ", "
                                        + geolocation.region_name + " "
                                        + geolocation.zip_code + " "
                                        + geolocation.country_name;
                }
            }
            catch { }

            return returnValue;
        }

        public static string CaptchaErrorMessage(string error)
        {
            string errorMessage = string.Empty;
            switch (error)
            {
                case ("missing-input-secret"):
                    errorMessage = "The secret parameter is missing.";
                    break;
                case ("invalid-input-secret"):
                    errorMessage = "The secret parameter is invalid or malformed.";
                    break;
                case ("missing-input-response"):
                    errorMessage = "The response parameter is missing.";
                    break;
                case ("invalid-input-response"):
                    errorMessage = "The response parameter is invalid or malformed.";
                    break;
                default:
                    errorMessage = "Error occured. Please try again";
                    break;
            }

            return errorMessage;
        }

        public static bool IsHumanUser(string response, ref string errorMessage)
        {
            bool isSuccess = true;

            if (!HttpContext.Current.Request.IsLocal && Convert.ToBoolean(GetAppSetting("reCAPTCHA.On")))
            {
                string reply = string.Empty, secretKey = string.Empty;
                string webSiteName = GetAppSetting("WebSiteName");
                string currenDomain = HttpContext.Current.Request.Url.ToString().ToLower();

                if (webSiteName == "WholesaleSite")
                    secretKey = GetAppSetting("reCAPTCHA." + (currenDomain.Contains("retail.") ? "Retail" : "WholeSale") + ".SecretKey");
                else
                    secretKey = GetAppSetting("reCAPTCHA.SecretKey");

                string url = string.Format(GetAppSetting("reCAPTCHA.SiteVerifyUrl"), secretKey, response);
                WebClient client = new WebClient();
                reply = client.DownloadString(url);
                var captchaResponse = Utils.Deserialize<CaptchaResponse>(reply);
                isSuccess = captchaResponse.Success;

                if (!isSuccess)
                {
                    if (captchaResponse.ErrorCodes == null)
                        errorMessage = "The secret parameter is missing. Please check the reCAPTCHA.";
                    else
                        errorMessage = CaptchaErrorMessage(captchaResponse.ErrorCodes[0].ToLower());
                }
            }

            return isSuccess;
        }

        public static string GenerateRandomPassword(int length, int numericRequired, int specialRequired, int minLowerRequired, int minUpperRequired)
        {
            bool passwordnotset = true;
            string password = string.Empty;
            while (passwordnotset)
            {
                Generator pwdGenerator = new Generator();
                pwdGenerator.MinLowerCaseChars = minLowerRequired;
                pwdGenerator.MinUpperCaseChars = minUpperRequired;
                pwdGenerator.MinNumericChars = numericRequired;
                pwdGenerator.MinSpecialChars = specialRequired;
                password = pwdGenerator.GeneratePassword(length);
                if (Regex.IsMatch(password, PasswordPolicy, RegexOptions.IgnoreCase))
                    passwordnotset = false;
            }

            return password;
        }

        public static string GetUserOS(string userAgent)
        {
            // get a parser with the embedded regex patterns
            var uaParser = Parser.GetDefault();
            ClientInfo c = uaParser.Parse(userAgent);
            return c.OS.Family;
        }

        public static double GetFirstMonthPayment(decimal loanAmount, int loanTermMonth, decimal interestPerYear, bool interestOnly)
        {
            double a, b, x;
            double monthlyPayment;

            try
            {
                a = (1 + Math.Round(Convert.ToDouble(interestPerYear), 3) / 1200);

                b = Convert.ToDouble(loanTermMonth);
                x = Math.Pow(a, b);
                x = 1 / x;
                x = 1 - x;

                if (interestOnly)
                    monthlyPayment = Convert.ToDouble(loanAmount) * Convert.ToDouble(Math.Round(Convert.ToDouble(interestPerYear), 3) / 1200);
                else
                    monthlyPayment = Convert.ToDouble(loanAmount) * Convert.ToDouble(Math.Round(Convert.ToDouble(interestPerYear), 3) / 1200 / x);
            }
            catch (Exception)
            {
                monthlyPayment = 0;
            }

            return monthlyPayment;
        }

        public static bool IsTLSSupportBrowser()
        {
            bool isTLSSupportBrowser = false;
            string userAgent = HttpContext.Current.Request.UserAgent;
            string OSVersion = GetUserOS(userAgent);
            var browser = HttpContext.Current.Request.Browser;
            string browserType = browser.Browser;
            int majorVersion = browser.MajorVersion;
            double minorVersion = browser.MinorVersion;

            userAgent = userAgent.ToLower();
            browserType = userAgent.Contains("edge") ? "Edge" : browserType;

            switch (browserType)
            {
                case "Chrome":
                    isTLSSupportBrowser = majorVersion >= 38;
                    break;
                case "Firefox":
                    isTLSSupportBrowser = majorVersion >= 27;
                    break;
                case "InternetExplorer":
                    isTLSSupportBrowser = majorVersion >= 11;
                    break;
                case "Edge":
                    isTLSSupportBrowser = true;
                    break;
                case "Opera":
                    isTLSSupportBrowser = majorVersion > 12 || (majorVersion == 12 && minorVersion >= 18);
                    break;
                case "Safari":
                    if ((OSVersion == "Mac OS X" && majorVersion >= 7) || (OSVersion == "iOS" && majorVersion >= 5))
                    {
                        isTLSSupportBrowser = true;
                    }
                    else if (OSVersion == "iOS" && (userAgent.IndexOf("crios/") > -1 || userAgent.IndexOf("applewebkit/") > -1))  //Chrome on the iPad
                    {
                        isTLSSupportBrowser = true;
                    }
                    break;
                default:
                    isTLSSupportBrowser = false;
                    break;
            }

            if (!isTLSSupportBrowser)
                ClearAllCacheAndCookie();

            return isTLSSupportBrowser;
        }

        public static string GetAppSetting(string key, string defaultValue = "")
        {
            string value = defaultValue;

            try
            {
                value = ConfigurationManager.AppSettings[key];
            }
            catch { }

            return value;
        }

        public static NetworkCredential SetFileServerAndCredential(string serverType, ref string fileServer)
        {
            if (string.IsNullOrEmpty(fileServer)) fileServer = "Documents";
            NetworkCredential credentials = new NetworkCredential();
            fileServer = Utils.GetAppSetting(serverType + ".FileServer");
            string credentialType = serverType == "Upload" ? "Upload" : "Documents";
            credentials.UserName = GetAppSetting(credentialType + ".FileServer.ID");
            credentials.Password = DecryptString(GetAppSetting(credentialType + ".FileServer.PW"));

            return credentials;
        }

        public static bool IsWholesaleSite()
        {
            bool returnValue = false;
            try
            {
                string currentUrl = HttpContext.Current.Request.Url.ToString().ToLower();
                returnValue = !currentUrl.Contains("test") && currentUrl.Contains("wholesale");
            }
            catch { }

            return returnValue;
        }

        public static bool IsRetailSite()
        {
            bool returnValue = false;
            try
            {
                string currentUrl = HttpContext.Current.Request.Url.ToString().ToLower();
                returnValue = currentUrl.Contains("retail.") || Convert.ToBoolean(Utils.GetAppSetting("OnRetailSite"));
            }
            catch { }

            return returnValue;
        }

        public static bool IsCorrespondentSite()
        {
            bool returnValue = false;
            string webSiteName = GetAppSetting("WebSiteName");

            try
            {
                string currentUrl = HttpContext.Current.Request.Url.ToString().ToLower();
                returnValue = currentUrl.Contains("correspondent") || webSiteName == "CorrespondentSite";
            }
            catch { }

            return returnValue;
        }

        public static bool IsPrivateLendingSite()
        {
            bool returnValue = false;
            try
            {
                string currentUrl = HttpContext.Current.Request.Url.ToString().ToLower();
                returnValue = currentUrl.Contains("private") || Convert.ToBoolean(Utils.GetAppSetting("OnPrivateLendingSite"));
            }
            catch { }

            return returnValue;
        }

        public static bool IsTestEnvironment()
        {
            bool returnValue = false;
            try
            {
                string currentUrl = HttpContext.Current.Request.Url.ToString().ToLower();
                returnValue = currentUrl.Contains("test");
            }
            catch { }

            return returnValue;
        }

        public static bool IsNumeric(string value)
        {
            return value.All(char.IsNumber);
        }

        public static bool IsURLExist(string url)
        {
            Uri urlToCheck = new Uri(url);
            WebRequest request = WebRequest.Create(urlToCheck);
            request.Timeout = 5000;

            WebResponse response;
            try
            {
                response = request.GetResponse();
                if (request.RequestUri != response.ResponseUri)
                    return false; //url doesn't exist

                return true; // url exist
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("denied"))
                    return true; // url exist but access denied
                else
                    return false; // url doesn't exist
            }
        }

        public static bool DoNotReadButDownload(string tmpFile, bool useMapPath = true)
        {
            bool returnValue = false;
            string fileName = useMapPath ? HttpContext.Current.Server.MapPath(tmpFile) : tmpFile;
            string extensionType = MimeTypes.MimeTypeMap.GetExtension(MimeMapping.GetMimeMapping(fileName));
            Regex r = new Regex(GetAppSetting("Documents.OfficeFileType"), RegexOptions.IgnoreCase);
            returnValue = r.IsMatch(extensionType);

            return returnValue;
        }

        public static bool RedoUploadCondition(int errorCode)
        {
            Regex r = new Regex(GetAppSetting("ConditionUpload.RetryCode"), RegexOptions.IgnoreCase);
            return r.IsMatch("" + errorCode);
        }

        public static string AddBrToDescription(string text)
        {
            try
            {
                string patterns = File.ReadAllText(HttpContext.Current.Server.MapPath(GetAppSetting("Documents.Condition.JsonTemplate")));
                PatternAndReplacementModel PatternAndReplacemen = Utils.Deserialize<PatternAndReplacementModel>(patterns);

                foreach (var pr in PatternAndReplacemen.PatternAndReplacements)
                {
                    text = text.Replace(pr.pattern, pr.replacement);
                }
            }
            catch { }

            return text;
        }

        public static bool CheckAndConvertErrorMessage(ref string errMessage)
        {
            bool returnValue = false;

            if (errMessage.Contains("[BIZ]") || errMessage.Contains("[INF]") || errMessage.Contains("[MSG]") || errMessage.Contains("[USR]"))
            {
                errMessage = errMessage.Replace("[BIZ]<br />", "").Replace("[BIZ]", "");
                errMessage = errMessage.Replace("[INF]<br />", "").Replace("[INF]", "");
                errMessage = errMessage.Replace("[MSG]<br />", "").Replace("[MSG]", "");
                errMessage = errMessage.Replace("[USR]<br />", "").Replace("[USR]", "").Replace("[MESSAGE]", "");
                if (errMessage.IndexOf("Ticket:") > 0)
                    errMessage = errMessage.Substring(0, errMessage.IndexOf("Ticket:") - 1);
                errMessage = "<span class='text-primary convert'>" + errMessage + "</span>";
                returnValue = true;
            }

            return returnValue;
        }

        public static bool IsPrivateIPAddress(string ipAddress)
        {
            if (HttpContext.Current.Request.IsLocal) return true;

            int[] ipParts = ipAddress.Split(new String[] { "." }, StringSplitOptions.RemoveEmptyEntries)
                                     .Select(s => int.Parse(s)).ToArray();
            // in private ip range
            if (ipParts[0] == 10 ||
                (ipParts[0] == 192 && ipParts[1] == 168) ||
                (ipParts[0] == 172 && (ipParts[1] >= 16 && ipParts[1] <= 31)))
            {
                return true;
            }

            // IP Address is probably public.
            // This doesn't catch some VPN ranges like OpenVPN and Hamachi.
            return false;
        }
        #endregion

        #region JSON Serialization and Deserialization
        /// <summary>
        /// JSON Serialization
        /// </summary>
        public static string Serialize<T>(T t)
        {
            return Serialize<T>(t, JsonFormat.None);
        }

        /// <summary>
        /// JSON Serialization
        /// </summary>
        public static string Serialize<T>(T t, JsonFormat formatting)
        {
            Newtonsoft.Json.Formatting format = Newtonsoft.Json.Formatting.None;
            switch (formatting)
            {
                case JsonFormat.Indented:
                    format = Newtonsoft.Json.Formatting.Indented;
                    break;
                default:
                    format = Newtonsoft.Json.Formatting.None;
                    break;
            }

            return Newtonsoft.Json.JsonConvert.SerializeObject(t, format);
        }

        public static string Serialize(object instance, JsonFormat formatting)
        {
            Newtonsoft.Json.Formatting format = Newtonsoft.Json.Formatting.None;
            switch (formatting)
            {
                case JsonFormat.Indented:
                    format = Newtonsoft.Json.Formatting.Indented;
                    break;
                default:
                    format = Newtonsoft.Json.Formatting.None;
                    break;
            }

            return Newtonsoft.Json.JsonConvert.SerializeObject(instance, format);
        }

        /// <summary>
        /// JSON Deserialization
        /// </summary>
        public static T Deserialize<T>(string jsonString)
        {
            return Deserialize<T>(jsonString, false);
        }

        public static T Deserialize<T>(string jsonString, bool isResolve)
        {
            if (isResolve)
            {
                jsonString = Regex.Replace(jsonString, @"^\s*\{[""'][^""']+[""']\s*:\s*", "", RegexOptions.IgnoreCase);
                jsonString = Regex.Replace(jsonString, @"\}\s*$", "", RegexOptions.IgnoreCase);
            }

            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(jsonString);
        }
        #endregion

        #region Check Device 
        public static bool IsTabletDevice()
        {
            bool returnValue = false;

            try
            {
                string userAgent = HttpContext.Current.Request.UserAgent.ToLower();
                Regex r = new Regex(GetAppSetting("TabletDevices"), RegexOptions.IgnoreCase);
                returnValue = !userAgent.Contains("windows") && r.IsMatch(userAgent);
            }
            catch { }

            return returnValue;
        }

        public static bool IsWinTablet()
        {
            bool returnValue = false;

            try
            {
                string userAgent = HttpContext.Current.Request.UserAgent.ToLower();
                bool isMobile = HttpContext.Current.Request.Browser.IsMobileDevice;
                returnValue = isMobile && userAgent.Contains("windows") && userAgent.Contains("touch");
            }
            catch { }

            return returnValue;
        }

        public static bool IsMobilephone()
        {
            bool returnValue = false;

            try
            {
                bool isMobile = HttpContext.Current.Request.Browser.IsMobileDevice;
                returnValue = isMobile && !IsTabletDevice() && !IsWinTablet();
            }
            catch { }

            return returnValue;
        }
        #endregion

        #region Soap Request
        public static void SaveSoapRequest(object lstCntct, string fileName)
        {
            try
            {
                XmlSerializer x = new XmlSerializer(lstCntct.GetType());
                StreamWriter writer = new StreamWriter(fileName, true);
                x.Serialize(writer, lstCntct);
                writer.Close();
            }
            catch { }
        }

        public static string SerializeSoapRequest(object lstCntct)
        {
            string returnValue = string.Empty;
            try
            {
                XmlSerializer x = new XmlSerializer(lstCntct.GetType());
                XmlWriterSettings settings = new XmlWriterSettings();
                settings.Indent = false;
                settings.NewLineOnAttributes = false;

                using (var writer = new StringWriter())
                {
                    using (var xw = XmlWriter.Create(writer, settings))
                    {
                        x.Serialize(writer, lstCntct);
                        returnValue = writer.ToString().Replace("\r\n", "");

                    }
                }
            }
            catch { }

            return returnValue;
        }
        #endregion

        #region Convert Format
        public static Dictionary<string, string> GetParseXmlValues(string xmlcontent, IList<string> keys)
        {
            Dictionary<string, string> returnValue = null;

            try
            {
                XmlDocument xml = new XmlDocument();
                xml.LoadXml(xmlcontent);
                foreach (string key in keys)
                {
                    if (returnValue == null) returnValue = new Dictionary<string, string>();
                    XmlNodeList xnl = xml.GetElementsByTagName(key);
                    returnValue[key] = xnl != null && xnl.Count > 0 ? xnl[0].InnerText : null;
                }
            }
            catch { }

            return returnValue;
        }

        public static string ReplaceCharacter(string strInput, string strSign, string strChar)
        {
            string returnValue = !string.IsNullOrEmpty(strInput) && strInput.Contains(strSign) ? strInput.Replace(strSign, strChar) : strInput;

            return returnValue;
        }

        public static string GetPhoneNumberFormat(string phoneNumber, bool forTextBox = false)
        {
            string returnValue = string.Empty;

            try
            {
                if (!string.IsNullOrEmpty(phoneNumber))
                {
                    Regex rgx = new Regex("[^0-9]");
                    string strPhoneNumber = rgx.Replace(phoneNumber, string.Empty);
                    if (strPhoneNumber.Length > 10)
                        strPhoneNumber = strPhoneNumber.Substring(strPhoneNumber.Length - 10);

                    returnValue = string.Format("({0}) {1}-{2}", strPhoneNumber.Substring(0, 3), strPhoneNumber.Substring(3, 3), strPhoneNumber.Substring(6));
                    if (forTextBox)
                        returnValue = string.Format("{0}-{1}-{2}", strPhoneNumber.Substring(0, 3), strPhoneNumber.Substring(3, 3), strPhoneNumber.Substring(6));
                }
            }
            catch
            {
                returnValue = "Invalid Phone Number: " + phoneNumber;
            }

            return returnValue;
        }

        public static string GetPhoneNumberFormat2(string phoneNumber)
        {
            string returnValue = string.Empty;

            if (!string.IsNullOrEmpty(phoneNumber))
            {
                Regex rgx = new Regex("[^0-9]");
                string strPhoneNumber = rgx.Replace(phoneNumber, string.Empty);

                returnValue = string.Format("{0}.{1}.{2}", strPhoneNumber.Substring(0, 3), strPhoneNumber.Substring(3, 3), strPhoneNumber.Substring(6));
            }

            return returnValue;
        }

        public static string GetSSNFormat(string SSN, bool isMask = true)
        {
            if (!string.IsNullOrEmpty(SSN) && SSN.Length > 8)
            {
                Regex rgx = new Regex("[^0-9]");
                string strSSN = rgx.Replace(SSN, string.Empty);

                if (isMask)
                    return string.Format("***-**-{0}", strSSN.Substring(strSSN.Length - 4));
                else
                    return string.Format("{0}-{1}-{2}", strSSN.Substring(0, 3), strSSN.Substring(3, 2), strSSN.Substring(5, 4));
            }
            else
            {
                return string.Empty;
            }
        }

        public static string GetCurrencyFormat(string currency)
        {
            if (!string.IsNullOrEmpty(currency))
            {
                Regex rgx = new Regex(@"[^0-9\\.]");
                string strCurrency = rgx.Replace(currency, string.Empty);

                return string.Format("{0:c2}", Convert.ToDecimal(strCurrency));
            }
            else
            {
                return string.Empty;
            }
        }

        public static string GetCurrencyFormat2(decimal currency)
        {
            return currency < 0 ? "(" + string.Format("{0:c2}", Convert.ToDecimal(currency)) + ")" : string.Format("{0:c2}", Convert.ToDecimal(currency));
        }

        public static string GetDecimalFormat(decimal dec)
        {
            return dec < 0 ? "(" + (dec * -1).ToString("0.000") + ")" : string.Format("{0:n3}", dec);
        }

        public static string GetPercentFormat(decimal dec)
        {
            return dec < 0 ? "(" + (dec * -1).ToString("0.000 %") + ")" : string.Format("{0:p3}", dec);
        }
        public static string SerializeObjectToXml(object obj)
        {
            string xmlStr = string.Empty;
            using (var stringWriter = new StringWriter())
            {

                var serializer = new XmlSerializer(obj.GetType());
                serializer.Serialize(stringWriter, obj);
                xmlStr = stringWriter.ToString();
            }
            return xmlStr;
        }

        public static T DeserializeXmlToObject<T>(string xml)
        {
            T result;
            using (var strReader = new StringReader(xml))
            {
                var serializer = new XmlSerializer(typeof(T));
                var xmlReader = new XmlTextReader(strReader);
                result = (T)serializer.Deserialize(xmlReader);
            }

            return result;
        }

        public static string GetFullAddress(IAddress address)
        {
            StringBuilder fullAddress = new StringBuilder();
            fullAddress.Append(address.AddressLine1);

            if (!string.IsNullOrEmpty(address.AddressLine2))
            {
                fullAddress.Append(" ");
                fullAddress.Append(address.AddressLine2);
            }
            fullAddress.Append(", " + address.City + ", ");
            fullAddress.Append(address.StateProvince + " " + address.PostalCode);

            return fullAddress.ToString();
        }
        #endregion

        #region Encryp and Decrypt
        public static string EncryptString(string valueToEncrypt)
        {
            CryptoGraphy.CryptoGraphy oCryptoGraphy = new CryptoGraphy.CryptoGraphy();
            var encryptedValue = oCryptoGraphy.Encrypt(valueToEncrypt);
            return encryptedValue;
        }

        public static string DecryptString(string valueToDecrypt)
        {
            var decryptedValue = string.Empty;
            if (!string.IsNullOrEmpty(valueToDecrypt))
            {
                CryptoGraphy.CryptoGraphy oCryptoGraphy = new CryptoGraphy.CryptoGraphy();
                decryptedValue = oCryptoGraphy.Decrypt(valueToDecrypt);
            }

            return decryptedValue;
        }

        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }
        #endregion

        #region Safety Parameter
        public static string SafeSqlParameter(object theValue)
        {
            return SafeSqlParameter(theValue, 2, false);
        }

        public static string SafeSqlParameter(object theValue, bool justLevel2)
        {
            return SafeSqlParameter(theValue, 2, justLevel2);
        }

        public static string SafeSqlParameter(object theValue, int theLevel, bool justLevel2)
        {
            // Written by user CWA, CoolWebAwards.com Forums. 2 February 2010
            // http://forum.coolwebawards.com/threads/12-Preventing-SQL-injection-attacks-using-C-NET

            /* intLevel represent how thorough the value will be checked for dangerous code
            // intLevel (1) - Do just the basic. This level will already counter most of the SQL injection attacks
            // intLevel (2) -   (non breaking space) will be added to most words used in SQL queries to prevent unauthorized access to the database. 
            // Safe to be printed back into HTML code. Don't use for usernames or passwords
            */
            string returnValue = (string)theValue;
            int intLevel = theLevel;

            if (!string.IsNullOrEmpty(returnValue))
            {
                if (intLevel > 0 && !justLevel2)
                {
                    returnValue = returnValue.TrimStart(' ', '#');
                    returnValue = returnValue.Replace("'", string.Empty); // Most important one! This line alone can prevent most injection attacks
                    returnValue = returnValue.Replace("--", string.Empty);
                    returnValue = returnValue.Replace("%", string.Empty);
                    returnValue = returnValue.Replace("@", string.Empty);
                    returnValue = returnValue.Replace("|", string.Empty);
                    returnValue = returnValue.Replace("&", string.Empty);
                    returnValue = returnValue.Replace(";", string.Empty);
                    returnValue = returnValue.Replace("$", string.Empty);
                    returnValue = returnValue.Replace('"', ' ');
                    returnValue = returnValue.Replace(@"\", string.Empty);
                    returnValue = returnValue.Replace("<>", string.Empty);
                    returnValue = returnValue.Replace("(", " ");
                    returnValue = returnValue.Replace(")", " ");
                    returnValue = returnValue.Replace("+", string.Empty);
                    returnValue = returnValue.Replace(",", string.Empty);
                    returnValue = returnValue.Replace("*", string.Empty);
                    returnValue = returnValue.Trim();
                }

                if (intLevel > 1)
                {
                    returnValue = returnValue.Replace("%20", " ");
                    string[] myArray = new string[]
                    {
                        "xp_", "update", "insert", "select", "drop", "alter", "create", "rename", "delete", "replace",
                        "declare", "nchar", "char", "nvarchar", "varchar", "begin", "cast", "drop", "end", "exec", "execute", "fetch", "kill",
                        "open", "sysobjects", "syscolumns", "sys", "table"
                    };
                    int i = 0;
                    int i2 = 0;
                    int intLenghtLeft = 0;
                    for (i = 0; i < myArray.Length; i++)
                    {
                        string strWord = myArray[i];
                        if (strWord != "xp_")
                        {
                            strWord = @"\b" + strWord + @"\b";
                        }

                        Regex rx = new Regex(strWord, RegexOptions.Compiled | RegexOptions.IgnoreCase);
                        MatchCollection matches = rx.Matches(returnValue);
                        i2 = 0;
                        foreach (Match match in matches)
                        {
                            GroupCollection groups = match.Groups;
                            intLenghtLeft = groups[0].Index + myArray[i].Length + i2;
                            returnValue = returnValue.Substring(0, intLenghtLeft - 1) + " " + returnValue.Substring(returnValue.Length - (returnValue.Length - intLenghtLeft), returnValue.Length - intLenghtLeft);
                            i2 += 5;
                        }
                    }
                }

                return returnValue;
            }
            else
            {
                return returnValue;
            }
        }
        #endregion

        #region Convert Phrase
        public static string ToCamelCase(string input)
        {
            string[] words = input.Split(' ');

            StringBuilder sb = new StringBuilder();

            foreach (string s in words)
            {
                string firstLetter = s.Substring(0, 1);
                string rest = s.Substring(1, s.Length - 1);
                sb.Append(firstLetter.ToUpper() + rest);
                sb.Append(" ");
            }

            return sb.ToString().Substring(0, sb.ToString().Length - 1);
        }

        public static string UppercaseFirst(string s)
        {
            if (string.IsNullOrEmpty(s))
                return string.Empty;

            char[] a = s.ToLower().ToCharArray();
            a[0] = char.ToUpper(a[0]);
            return new string(a);
        }

        public static string AddSpacesToSentence(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return "";
            StringBuilder newText = new StringBuilder(text.Length * 2);
            newText.Append(text[0]);
            for (int idx = 1; idx < text.Length; idx++)
            {
                if (!char.IsUpper(text[idx - 1]) && char.IsUpper(text[idx]) && text[idx - 1] != ' ')
                    newText.Append(' ');
                newText.Append(text[idx]);
            }

            return newText.ToString();
        }

        public static int OrginationChannel(string marketPlace)
        {
            int orginationChannel = 3;

            switch (marketPlace.Trim().ToUpper())
            {
                case ("WHOLESALE"):
                    orginationChannel = 3;
                    break;
                case ("RETAIL"):
                    orginationChannel = 1;
                    break;
                case ("CORRESPONDENT"):
                    orginationChannel = 4;
                    break;
                case ("MINICORR"):
                    orginationChannel = 5;
                    break;
                case ("PRIVATELENDING"):
                    orginationChannel = 6;
                    break;
                default:
                    orginationChannel = 3;
                    break;
            }

            return orginationChannel;
        }
        #endregion

        #region Cookies
        public static void ClearAllCacheAndCookie()
        {
            // replace with username if this is the wrong cookie name
            HttpContext.Current.Response.Cookies.Remove(FormsAuthentication.FormsCookieName);

            // Session.Clear();
            (new SessionManager()).LogoutUserSession(HttpContext.Current.Session.SessionID);
            HttpContext.Current.Session.Abandon();

            HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            HttpContext.Current.Response.Cache.SetExpires(DateTime.Now.AddSeconds(-1));
            HttpContext.Current.Response.Cache.SetNoStore();
            HttpContext.Current.Response.AppendHeader("Pragma", "no-cache");

            // clear authentication cookie
            HttpCookie cookie1 = new HttpCookie(FormsAuthentication.FormsCookieName, "");
            cookie1.Expires = DateTime.Now.AddYears(-1);
            HttpContext.Current.Response.Cookies.Add(cookie1);

            // clear session cookie
            HttpCookie cookie2 = new HttpCookie("ASP.NET_SessionId", "");
            cookie2.Expires = DateTime.Now.AddYears(-1);
            HttpContext.Current.Response.Cookies.Add(cookie2); DataCacher.Clear("WebPortal_CurrentUser");

            string[] myCookies = HttpContext.Current.Request.Cookies.AllKeys;
            foreach (string c in myCookies)
                if (c != "AmWest_MovingAnnouncement" && c != "KeepMeLoggedIn" && !c.Contains("RememberMe") && c != "AmWest_WebsiteMaintenance"
                    && !c.Contains("AlreadyViewedExpirationNotice") && c != "HelpDesk_TicketOption" && !c.Contains("OriginationChannel"))
                    HttpContext.Current.Response.Cookies[c].Expires = DateTime.Now.AddDays(-1);
        }

        public static void SetCookieDomain(HttpCookie ck)
        {
            string webSiteName = GetAppSetting("WebSiteName");
            if (GetAppSetting(webSiteName + ".CookieDomain") != null
                            && !string.IsNullOrEmpty(GetAppSetting(webSiteName + ".CookieDomain")))
                ck.Domain = GetAppSetting(webSiteName + ".CookieDomain");
            if (HttpContext.Current.Request.Url.ToString().ToLower().Contains("retail."))
                ck.Domain = GetAppSetting("Retail.CookieDomain");
        }

        public static void CreateCookie(string key, string value)
        {
            CreateCookie(key, value, null, false);
        }

        public static void CreateCookie(string key, string value, DateTime expires)
        {
            CreateCookie(key, value, expires, false);
        }

        public static void CreateCookie(string key, string value, DateTime? expires, bool httpOnly)
        {
            HttpCookie ck = new HttpCookie(key);
            ck.Value = value;
            SetCookieDomain(ck);

            if (expires.HasValue)
                ck.Expires = expires.Value;
            ck.HttpOnly = httpOnly;
            if (HttpContext.Current.Request.IsSecureConnection && httpOnly)
                ck.Secure = true;
            SetCookieDomain(ck);

            HttpContext.Current.Response.Cookies.Add(ck);
        }

        public static void RemoveCookie(string key)
        {
            HttpCookie ck = new HttpCookie(key);
            SetCookieDomain(ck); //since we are now adding domains to the cookies, need to add to the cookie when removing
            ck.Expires = DateTime.Now.AddDays(-1);

            HttpContext.Current.Response.Cookies.Add(ck);
        }

        public static string GetCookie(string name, bool clearCookie = false)
        {
            string ret = null;
            HttpCookie cookie = HttpContext.Current.Request.Cookies[name];
            ret = cookie != null ? cookie.Value : null;
            if (!string.IsNullOrEmpty(ret))
            {
                if (clearCookie)
                {
                    cookie = HttpContext.Current.Response.Cookies[name];
                    if (cookie == null)
                    {
                        cookie = new HttpCookie(name, null);
                        cookie.Path = "/";
                        cookie.HttpOnly = true;
                        if (HttpContext.Current.Request.IsSecureConnection)
                            cookie.Secure = true;
                    }
                    else
                        HttpContext.Current.Response.Cookies.Remove(name);
                    cookie.Expires = DateTime.Now.AddDays(-1d);
                    SetCookieDomain(cookie);
                    HttpContext.Current.Response.Cookies.Add(cookie);
                }
            }

            return ret;
        }

        public static void SetNotification(string notification, bool isWarning = false)
        {
            HttpCookie cookie = new HttpCookie(isWarning ? "WebPortalWarning" : "WebPortalNotification");
            cookie.Value = string.Format(notification);
            cookie.Expires = DateTime.Now.AddMinutes(20);
            cookie.Path = "/";
            HttpContext.Current.Response.Cookies.Add(cookie);
        }
        #endregion

        #region Data Control
        public static string StringFormatWithNameParameter(string input, object p)
        {
            foreach (PropertyDescriptor prop in TypeDescriptor.GetProperties(p))
            {
                input = input.Replace("{" + prop.Name + "}", (prop.GetValue(p) ?? "(null)").ToString());
            }

            return input;
        }

        public static DataTable ToDataTable<T>(List<T> items)
        {
            DataTable dataTable = new DataTable(typeof(T).Name);

            //Get all the properties
            PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prop in Props)
            {
                //Defining type of data column gives proper data table 
                //var type = (prop.PropertyType.IsGenericType && prop.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>) ? typeof(System.String) : prop.PropertyType);
                //Setting column names as Property names
                var propName = AddSpacesToSentence(prop.Name);
                propName = (propName.Contains("UW") ? propName.Replace("UW", " UW ") : propName.Contains("LO") ? propName.Replace("LO", " LO ") : propName).Trim();
                if (prop.PropertyType.IsGenericType && prop.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                    dataTable.Columns.Add(propName);
                else
                    dataTable.Columns.Add(propName, prop.PropertyType);
            }
            foreach (T item in items)
            {
                var values = new object[Props.Length];
                for (int i = 0; i < Props.Length; i++)
                {
                    //inserting property values to datatable rows
                    values[i] = Props[i].GetValue(item, null);
                    if (values[i] != null)
                        values[i] = values[i].ToString().Replace(" 12:00:00 AM", "");
                }
                dataTable.Rows.Add(values);
            }
            //put a breakpoint here and check datatable
            return dataTable;
        }

        public static List<T> ConvertToList<T>(DataTable dt)
        {

            var columnNames = dt.Columns.Cast<DataColumn>().Select(c => c.ColumnName.ToLower()).ToList();
            var properties = typeof(T).GetProperties();
            var result = dt.AsEnumerable().Select(row =>
            {
                var objT = Activator.CreateInstance<T>();
                //take long time around 2 rows / sec
                foreach (var pro in properties)
                {
                    if (columnNames.Contains(pro.Name.ToLower()))
                    {
                        try
                        {
                            if (pro.PropertyType.Name == "Int32")
                            {
                                pro.SetValue(objT, Int32.Parse(row[pro.Name].ToString()));
                            }
                            else if (pro.PropertyType.Name == "DateTime")
                            {
                                pro.SetValue(objT, DateTime.Parse(row[pro.Name].ToString()));
                            }
                            else if (pro.PropertyType.Name == "Boolean")
                            {
                                pro.SetValue(objT, Boolean.Parse(row[pro.Name].ToString()));
                            }
                            else
                            {
                                pro.SetValue(objT, !string.IsNullOrEmpty(row[pro.Name].ToString()) ? row[pro.Name] : DBNull.Value);
                            }
                        }
                        catch { }
                    }
                }
                return objT;
            }).ToList();

            return result;
        }

        private static string ChangeColumnNames(string columnNames)
        {
            columnNames = columnNames.Replace("Description", "").Replace("Code", "").Replace(" ", "").Trim();
            if (columnNames.Contains("Reports")) columnNames = "ReportsTo";
            else if (columnNames.Contains("Department")) columnNames = "Department";
            else if (columnNames.Contains("Email")) columnNames = "Email";
            else if (columnNames.Contains("PrimaryAddress")) columnNames = "Territory";

            return columnNames;
        }

        public static DataTable GetDataTableFromExcel(Stream excelFileStream, ref string errMeesage, bool removeSpaceInFirstRow = false, bool firstRow = true)
        {
            //Create a new DataTable.
            DataTable dt = new DataTable();

            try
            {
                // Open the Excel file using ClosedXML.
                // Keep in mind the Excel file cannot be open when trying to read it
                using (XLWorkbook workBook = new XLWorkbook(excelFileStream))
                {
                    //Read the first Sheet from Excel file.
                    IXLWorksheet workSheet = workBook.Worksheet(1);

                    foreach (IXLRow row in workSheet.Rows())
                    {
                        //Use the first row to add columns to DataTable.
                        if (firstRow)
                        {
                            foreach (IXLCell cell in row.Cells())
                            {
                                string cellValue = "" + cell.Value;
                                if (removeSpaceInFirstRow) cellValue = ChangeColumnNames(cellValue);
                                dt.Columns.Add(cellValue);
                            }
                            firstRow = false;
                        }
                        else
                        {
                            //Add rows to DataTable.
                            dt.Rows.Add();
                            int i = 0;
                            foreach (IXLCell cell in row.Cells())
                            {
                                dt.Rows[dt.Rows.Count - 1][i] = ("" + cell.Value).Trim();
                                i++;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                errMeesage = ex.Message;
            }

            return dt;
        }
        #endregion

        #region File Control
        public static string GetFileTypeForIcon(string fileExtension)
        {
            string returnValue;
            fileExtension = fileExtension.ToLower();
            switch (fileExtension)
            {
                case ("xls"):
                case ("xlsx"):
                    returnValue = "excel";
                    break;
                case ("doc"):
                case ("docx"):
                    returnValue = "word";
                    break;
                case ("jpg"):
                case ("png"):
                case ("gif"):
                case ("tif"):
                case ("bmp"):
                    returnValue = "image";
                    break;
                case ("htm"):
                    returnValue = "code";
                    break;
                case ("ppt"):
                    returnValue = "powerpoint";
                    break;
                case ("avi"):
                case ("mkv"):
                case ("mp4"):
                    returnValue = "video";
                    break;
                default:
                    returnValue = fileExtension;
                    break;
            }

            return returnValue;
        }

        public static string ConvertFileSize(double len)
        {
            string[] sizes = { "B", "KB", "MB", "GB" };
            int order = 0;
            while (len >= 1024 && ++order < sizes.Length)
            {
                len = len / 1024;
            }

            string result = string.Format("{0:0.##} {1}", len, sizes[order]);

            return result;
        }

        public static string GetContentType(byte[] fileByte)
        {
            string contentType = "application/pdf";

            string data_as_hex = BitConverter.ToString(fileByte);
            string ft = data_as_hex.Substring(0, 11);

            switch (ft) //https://en.wikipedia.org/wiki/List_of_file_signatures, https://asecuritysite.com/forensics/magic
            {
                case "25-50-44-46":
                    contentType = "application/pdf";
                    break;
                case "47-49-46-38":
                    contentType = "image/gif";
                    break;
                case "49-49-2A-00":
                case "4D-4D-00-2A":
                    contentType = "image/tiff";
                    break;
                case "FF-D8-FF-DB":
                case "FF-D8-FF-E0":
                case "4A-46-49-46":
                case "FF-D8-FF-E1":
                case "45-78-69-66":
                    contentType = "image/jpeg";
                    break;
                case "89-50-4E-47":
                    contentType = "image/png";
                    break;
                case "42-4D-F8-A9":
                case "42-4D-62-25":
                case "42-4D-76-03":
                    contentType = "image/bmp";
                    break;
                case "50-4B-03-04":
                    contentType = "application/zip";
                    break;
                case "38-42-50-53":
                    contentType = "application/psd";
                    break;
                case "D0-CF-11-E0":
                    contentType = "application/doc";
                    break;
                default:
                    contentType = "text/*";
                    break;
            }

            return contentType;
        }

        public static byte[] GetBytesFromFile(string fullFilePath)
        {
            // this method is limited to 2^32 byte files (4.2 GB)
            FileStream fs = null;
            try
            {
                fs = System.IO.File.OpenRead(fullFilePath);
                byte[] bytes = new byte[fs.Length];
                fs.Read(bytes, 0, Convert.ToInt32(fs.Length));
                return bytes;
            }
            finally
            {
                if (fs != null)
                {
                    fs.Close();
                    fs.Dispose();
                }
            }
        }

        public static ZipArchive GetZippedPDF(string filePath)
        {
            string fileText = string.Empty;
            ZipArchive zipArchive = null;
            try
            {
                fileText = System.IO.File.ReadAllText(filePath);
                Stream memoryStream = new MemoryStream(Convert.FromBase64String(fileText));
                zipArchive = new ZipArchive(memoryStream, ZipArchiveMode.Read);
            }
            catch { }

            return zipArchive;
        }

        public static byte[] GetBytesFromZippedPDF(ZipArchiveEntry tempFile)
        {
            byte[] fileByte = null;
            int pages;
            PdfImportedPage page;
            PdfReader reader;
            Document document = new Document();
            using (var memoryStream = new MemoryStream())
            {
                PdfWriter pdfWriter = PdfWriter.GetInstance(document, memoryStream);
                document.SetPageSize(PageSize.LETTER);
                document.Open();
                PdfContentByte pdfContentByte = pdfWriter.DirectContent;

                if (tempFile != null)
                {
                    using (Stream zipEntryStream = tempFile.Open())
                    {
                        reader = new PdfReader(zipEntryStream);
                        pages = reader.NumberOfPages;
                    }

                    for (int i = 1; i <= pages; i++)
                    {
                        document.SetPageSize(PageSize.LETTER);
                        document.NewPage();
                        page = pdfWriter.GetImportedPage(reader, i);
                        pdfContentByte.AddTemplate(page, 0, 0);
                    }
                    document.Close();
                    fileByte = memoryStream.GetBuffer();

                    memoryStream.Flush();
                    memoryStream.Dispose();
                }
            }

            return fileByte;
        }

        public static string RemovingIllegalCharactersFromString(string unCleanString)
        {
            string cleanString = Regex.Replace(unCleanString, @"[^a-zA-Z 0-9'.@]", "_").Trim();

            return cleanString;
        }

        #endregion

        #region Search Text From File
        public static bool SearchTextFromPdf(string path, string searchText)
        {
            bool returnValue = false;

            using (PdfReader reader = new PdfReader(path))
            {
                StringBuilder text = new StringBuilder();

                for (int i = 1; i <= reader.NumberOfPages; i++)
                {
                    text.Append(PdfTextExtractor.GetTextFromPage(reader, i));
                }

                returnValue = text.ToString().ToLower().Contains(searchText);
            }

            return returnValue;
        }

        public static bool SearchTextFromExcel(string path, string searchText)
        {
            bool returnValue = false;

            try
            {
                using (XLWorkbook workBook = new XLWorkbook(path))
                {
                    //Read the first Sheet from Excel file.
                    IXLWorksheet workSheet = workBook.Worksheet(1);

                    foreach (IXLRow row in workSheet.Rows())
                    {
                        foreach (IXLCell cell in row.Cells())
                        {
                            returnValue = cell.Value.ToString().ToLower().Contains(searchText);
                            if (returnValue) return returnValue;
                        }
                    }
                }
            }
            catch { }

            return returnValue;
        }

        public static bool SearchTextFromWord(string path, string searchText)
        {
            bool returnValue = false;
            string docText = string.Empty;

            try
            {
                using (Stream str = File.OpenRead(path))
                {
                    using (WordprocessingDocument wpd = WordprocessingDocument.Open(str, false))
                    {
                        using (StreamReader sr = new StreamReader(wpd.MainDocumentPart.GetStream()))
                        {
                            docText = sr.ReadToEnd();
                        }
                    }
                }

                returnValue = docText.ToString().ToLower().Contains(searchText);
            }
            catch { }

            return returnValue;
        }

        public static bool SearchTextFromPowerPoint(string path, string searchText)
        {
            bool returnValue = false;

            try
            {
                PowerPoint.Application PowerPoint_App = new PowerPoint.Application();
                PowerPoint.Presentations multi_presentations = PowerPoint_App.Presentations;
                PowerPoint.Presentation presentation = multi_presentations.Open(path);

                for (int i = 0; i < presentation.Slides.Count; i++)
                {
                    foreach (var item in presentation.Slides[i + 1].Shapes)
                    {
                        var shape = (PowerPoint.Shape)item;
                        if (shape.HasTextFrame == MsoTriState.msoTrue)
                        {
                            if (shape.TextFrame.HasText == MsoTriState.msoTrue)
                            {
                                var textRange = shape.TextFrame.TextRange;
                                var pptText = textRange.Text;
                                returnValue = pptText.ToLower().Contains(searchText);
                                if (returnValue)
                                {
                                    PowerPoint_App.Quit();
                                    return returnValue;
                                }
                            }
                        }
                    }
                }

                PowerPoint_App.Quit();
            }
            catch { }

            return returnValue;
        }

        public static bool SearchTextFromRTF(string path, string searchText)
        {
            bool returnValue = false;

            System.Windows.Forms.RichTextBox rtBox = new System.Windows.Forms.RichTextBox();
            string str = File.ReadAllText(path);
            rtBox.Rtf = str;
            string plainText = rtBox.Text;
            returnValue = plainText.ToLower().Contains(searchText);

            return returnValue;
        }

        public static bool SearchTextFromText(string path, string searchText)
        {
            bool returnValue = false;
            string plainText;

            using (StreamReader streamReader = File.OpenText(path))
            {
                plainText = streamReader.ReadLine();

                while (plainText != null)
                {
                    returnValue = plainText.ToLower().Contains(searchText);
                    if (returnValue) return returnValue;
                    plainText = streamReader.ReadLine();
                }
            }

            return returnValue;
        }
        #endregion

        #region Code Convert
        public class ToStringOption
        {
            public const int EmptyToNull = 1;
            public const int EmptyToBlank = 2;
        }

        public class ConvertNumOption
        {
            public const int EmptyToZero = 1;
            public const int EmptyToNull = 2;
        }

        static public string String(object value)
        {
            return String(value, ToStringOption.EmptyToBlank);
        }
        static public string String(object value, int option)
        {
            string ret = null;
            try
            {
                if (value == null || value == DBNull.Value)
                {
                    if (option == ToStringOption.EmptyToNull)
                        return null;
                    else if (option == ToStringOption.EmptyToBlank)
                        return string.Empty;
                }

                if (Convert.ToString(value).Trim() == "")
                {
                    if (option == ToStringOption.EmptyToNull)
                        ret = null;
                    else
                        ret = Convert.ToString(value).Trim();
                }
                else
                    ret = Convert.ToString(value).Trim();
            }
            catch
            {
                throw;
            }
            return ret;
        }

        public static string ConvertToCodeList(object x)
        {
            if (x.GetType() == typeof(Int64)) return ConvertToCodeList64((Int64)x);
            else return ConvertToCodeList32((int)x);
        }

        public static string ConvertToCodeList32(int x)
        {
            string retCodeList = "";
            List<char> bitList = new List<char>();

            while (x != 0)
            {
                bitList.Add((x & 1) == 1 ? '1' : '0');
                x >>= 1;
            }

            for (int idx = 0; idx < bitList.Count; idx++)
            {
                if (bitList[idx] == '1') retCodeList += Math.Pow(2, idx).ToString() + "|";
            }
            return retCodeList;
        }

        public static string ConvertToCodeList64(Int64 x)
        {
            string retCodeList = "";
            List<char> bitList = new List<char>();

            while (x != 0)
            {
                bitList.Add((x & 1) == 1 ? '1' : '0');
                x >>= 1;
            }

            for (int idx = 0; idx < bitList.Count; idx++)
            {
                if (bitList[idx] == '1') retCodeList += Math.Pow(2, idx).ToString() + "|";
            }
            return retCodeList;
        }

        public static List<string> ConvertToCodeStringList(object x)
        {
            if (x.GetType() == typeof(Int64)) return ConvertToCodeStringList64((Int64)x);
            else return ConvertToCodeStringList32((int)x);
        }

        public static List<string> ConvertToCodeStringList32(int x)
        {
            List<string> retCodeList = new List<string>();
            List<char> bitList = new List<char>();
            while (x != 0)
            {
                bitList.Add((x & 1) == 1 ? '1' : '0');
                x >>= 1;
            }

            for (int idx = 0; idx < bitList.Count; idx++)
            {
                if (bitList[idx] == '1') retCodeList.Add(Math.Pow(2, idx).ToString());
            }
            return retCodeList;
        }

        public static List<string> ConvertToCodeStringList64(Int64 x)
        {
            List<string> retCodeList = new List<string>();
            List<char> bitList = new List<char>();
            while (x != 0)
            {
                bitList.Add((x & 1) == 1 ? '1' : '0');
                x >>= 1;
            }

            for (int idx = 0; idx < bitList.Count; idx++)
            {
                if (bitList[idx] == '1') retCodeList.Add(Math.Pow(2, idx).ToString());
            }
            return retCodeList;
        }

        public static List<int> ConvertToCodeIntList(int x)
        {
            List<int> retCodeList = new List<int>();
            List<char> bitList = new List<char>();

            while (x != 0)
            {
                bitList.Add((x & 1) == 1 ? '1' : '0');
                x >>= 1;
            }

            for (int idx = 0; idx < bitList.Count; idx++)
            {
                if (bitList[idx] == '1') retCodeList.Add(Convert.ToInt32(Math.Pow(2, idx)));
            }
            return retCodeList;
        }

        public static List<Int64> ConvertToCodeInt64List(Int64 x)
        {
            List<Int64> retCodeList = new List<Int64>();
            List<char> bitList = new List<char>();

            while (x != 0)
            {
                bitList.Add((x & 1) == 1 ? '1' : '0');
                x >>= 1;
            }

            for (int idx = 0; idx < bitList.Count; idx++)
            {
                if (bitList[idx] == '1') retCodeList.Add(Convert.ToInt64(Math.Pow(2, idx)));
            }
            return retCodeList;
        }

        public static int ConvertToValue(string codeList)
        {
            int iValue = 0;
            string[] arrCodeList = codeList.Split(',');
            foreach (string value in arrCodeList)
            {
                iValue += Convert.ToInt32(value);
            }
            return iValue;
        }

        public static Int64 ConvertToValue64(string codeList)
        {
            Int64 iValue = 0;
            string[] arrCodeList = codeList.Split(',');
            foreach (string value in arrCodeList)
            {
                iValue += Convert.ToInt64(value);
            }
            return iValue;
        }

        public static int ConvertToValue(List<string> codeList)
        {
            int iValue = 0;

            foreach (string value in codeList)
            {
                iValue += Convert.ToInt32(value);
            }
            return iValue;
        }

        public static Int64 ConvertToValue64(List<string> codeList)
        {
            Int64 iValue = 0;

            foreach (string value in codeList)
            {
                iValue += Convert.ToInt64(value);
            }
            return iValue;
        }

        public static int ConvertToValue(List<int> codeList)
        {
            int iValue = 0;

            foreach (int value in codeList)
            {
                iValue += value;
            }
            return iValue;
        }

        public static Int64 ConvertToValue(List<Int64> codeList)
        {
            Int64 iValue = 0;

            foreach (Int64 value in codeList)
            {
                iValue += value;
            }
            return iValue;
        }

        public static bool CompareCode(int compareValue1, int compareValue2)
        {
            byte byreValue1 = (byte)compareValue1;
            byte byreValue2 = (byte)compareValue2;
            return Convert.ToInt32(byreValue1 & byreValue2) != 0 ? true : false;
        }

        public static bool CompareCode(Int64 compareValue1, Int64 compareValue2)
        {
            byte byreValue1 = (byte)compareValue1;
            byte byreValue2 = (byte)compareValue2;
            return Convert.ToInt64(byreValue1 & byreValue2) != 0 ? true : false;
        }

        public static List<string> GetCodeList(string binaryString)
        {
            var list = new List<string>();
            for (int idx = 0; idx < binaryString.Length; idx++)
            {
                list.Add(binaryString.Substring(idx, 1));
            }
            return list;
        }

        public static Byte[] GetBytesFromBinaryString(String binary)
        {
            var list = new List<Byte>();

            for (int i = 0; i < binary.Length; i += 8)
            {
                String t = binary.Substring(i, 8);

                list.Add(Convert.ToByte(t, 2));
            }

            return list.ToArray();
        }

        public static string ConvertIntToBinaryString(int x)
        {
            char[] bits = new char[32];
            int i = 0;

            while (x != 0)
            {
                bits[i++] = (x & 1) == 1 ? '1' : '0';
                x >>= 1;
            }

            Array.Reverse(bits, 0, i);
            return new string(bits);
        }

        public static string ConvertIntToBinaryString(Int64 x)
        {
            char[] bits = new char[32];
            int i = 0;

            while (x != 0)
            {
                bits[i++] = (x & 1) == 1 ? '1' : '0';
                x >>= 1;
            }

            Array.Reverse(bits, 0, i);
            return new string(bits);
        }

        public static string ListNumberToString(List<string> listNumber)
        {
            string sNumberList = "";
            foreach (string buff in listNumber.ToArray())
            {
                sNumberList += buff + ",";
            }

            if (sNumberList.Trim() == "") return "NULL";
            else return sNumberList.TrimEnd(',');
        }

        public static string ListNumberToString(List<int> listNumber)
        {
            string sNumberList = "";
            foreach (int buff in listNumber.ToArray())
            {
                sNumberList += buff.ToString() + ",";
            }

            if (sNumberList.Trim() == "") return "NULL";
            else return sNumberList.TrimEnd(',');
        }

        public static Decimal? ToNullableDecimal(object value)
        {
            return ToNullableDecimal(value, ConvertNumOption.EmptyToZero);
        }
        public static Decimal? ToNullableDecimal(object value, int option)
        {
            Decimal? ret = (Decimal?)null;
            try
            {
                if (value == null || value == DBNull.Value) return option == ConvertNumOption.EmptyToZero ? 0m : (Decimal?)null;
                Decimal n;
                if (Decimal.TryParse(Convert.ToString(value).Replace("(", "-").Replace(")", "").Replace("$", "").Replace(",", ""), out n) == true)
                    ret = Convert.ToDecimal(Convert.ToString(value).Replace("(", "-").Replace(")", "").Replace("$", "").Replace(",", ""));
                else
                    ret = option == ConvertNumOption.EmptyToZero ? 0m : (Decimal?)null;
            }
            catch
            {
                throw;
            }
            return ret;
        }

        public static DateTime ToDateTime(object value)
        {
            try
            {
                if (value == null || value == DBNull.Value)
                {
                    return DateTime.MinValue;
                }
                DateTime dt;

                if (!DateTime.TryParse(String(value), out dt))
                {
                    return DateTime.MinValue;
                }
                return dt;
            }
            catch
            {
                return DateTime.MinValue;
            }
        }

        public static DateTime? ToNullableDateTime(object value)
        {
            DateTime? ret = (DateTime?)null;
            try
            {
                if (value == null || value == DBNull.Value) return ret;

                DateTime dt;

                if (!DateTime.TryParse(String(value), out dt)) return (DateTime?)null;

                ret = (DateTime?)dt;
            }
            catch
            {
                throw;
            }
            return ret;
        }

        public static string DataBaseString(object value)
        {
            if (value == null || value == DBNull.Value) return "NULL";

            if (value.GetType() == typeof(Boolean) || value.GetType() == typeof(bool))
            {
                if ((Boolean)value == false) return "'N'";
                else return "'Y'";
            }
            else if (value.ToString().Trim() == "") return "NULL";
            return "'" + value.ToString().Trim().Replace("'", "''") + "'";
        }

        public static string toDateString(object value)
        {
            try
            {
                DateTime dt = ToDateTime(value);

                if (dt == DateTime.MinValue) return null;
                else return dt.ToShortDateString();
            }
            catch
            {
                return null;
            }
        }

        public static string toDateTimeString(object value)
        {
            try
            {
                DateTime dt = ToDateTime(value);

                if (dt == DateTime.MinValue) return null;
                else return dt.ToShortDateString() + " " + dt.ToShortTimeString();
            }
            catch
            {
                return null;
            }
        }

        public static Int16 ToInt16(object value)
        {
            return ToInt16(value, ConvertNumOption.EmptyToZero);
        }

        public static Int16 ToInt16(object value, int argOption)
        {
            if (value == null || value == DBNull.Value) return 0;

            if (String(value).IndexOf(".") >= 0) value = String(value).Substring(0, String(value).IndexOf("."));

            Int16 n;
            if (Int16.TryParse(String(value).Replace("(", "-").Replace(")", ""), out n) == true) return Convert.ToInt16(String(value).Replace("(", "-").Replace(")", ""));
            else return 0;
        }

        public static int ToInt32(object value)
        {
            return ToInt32(value, ConvertNumOption.EmptyToZero);
        }

        public static int ToInt32(object value, int option)
        {
            if (value == null || value == DBNull.Value) return option == ConvertNumOption.EmptyToZero ? 0 : -999;
            if (Convert.ToString(value).IndexOf(".") >= 0) value = Convert.ToString(value).Substring(0, Convert.ToString(value).IndexOf("."));

            Int32 n;
            if (Int32.TryParse(Convert.ToString(value).Replace("(", "-").Replace(")", ""), out n) == true) return Convert.ToInt32(Convert.ToString(value).Replace("(", "-").Replace(")", ""));
            else return option == ConvertNumOption.EmptyToZero ? 0 : -999;
        }

        public static Int64 ToInt64(object value)
        {
            return ToInt64(value, ConvertNumOption.EmptyToZero);
        }

        public static Int64 ToInt64(object value, int argOption)
        {
            if (value == null || value == DBNull.Value) return argOption == ConvertNumOption.EmptyToZero ? 0 : -999;
            if (String(value).IndexOf(".") >= 0) value = String(value).Substring(0, String(value).IndexOf("."));
            Int64 n;
            if (Int64.TryParse(String(value).Replace("(", "-").Replace(")", ""), out n) == true) return Convert.ToInt64(String(value).Replace("(", "-").Replace(")", ""));
            else return argOption == ConvertNumOption.EmptyToZero ? 0 : -999;
        }

        public static Double ToDouble(object value)
        {
            return ToDouble(value, ConvertNumOption.EmptyToZero);
        }

        public static Double ToDouble(object value, int argOption)
        {
            if (value == null || value == DBNull.Value) return argOption == ConvertNumOption.EmptyToZero ? 0 : -999;
            Double n;
            if (Double.TryParse(String(value).Replace("(", "-").Replace(")", ""), out n) == true) return Convert.ToDouble(String(value).Replace("(", "-").Replace(")", ""));
            else return argOption == ConvertNumOption.EmptyToZero ? 0 : -999;
        }

        public static Decimal ToDecimal(object value)
        {
            return ToDecimal(value, ConvertNumOption.EmptyToZero);
        }

        public static Decimal ToDecimal(object value, int option)
        {
            if (value == null || value == DBNull.Value) return option == ConvertNumOption.EmptyToZero ? 0m : -999m;
            Decimal n;
            if (Decimal.TryParse(Convert.ToString(value).Replace("(", "-").Replace(")", ""), out n) == true) return Convert.ToDecimal(Convert.ToString(value).Replace("(", "-").Replace(")", ""));
            else return option == ConvertNumOption.EmptyToZero ? 0m : -999m;
        }

        public static decimal ToFloor(decimal value, int decimalPlaces)
        {
            decimal adjustment = Convert.ToDecimal(Math.Pow(10, decimalPlaces));
            return Math.Floor(value * adjustment) / adjustment;
        }

        public static string ToDateString(object value)
        {
            try
            {
                DateTime dt = ToDateTime(value);

                if (dt == DateTime.MinValue) return null;
                else return dt.ToShortDateString();
            }
            catch
            {
                return null;
            }
        }
        #endregion

        #region Web API Features
        public static string GetBasicAuth()
        {
            string username = GetAppSetting("WebApiUserName");
            string password = GetAppSetting("WebApiKey");
            string apiKey = username + ":" + password;
            byte[] binaryApiKey = Encoding.UTF8.GetBytes(apiKey);
            string base64ApiKey = Convert.ToBase64String(binaryApiKey);
            string authorizationValue = "Basic " + base64ApiKey;

            return authorizationValue;
        }

        public static HttpWebRequest SetHttpWebRequest(string requestUrl, string method, string contentType = null, bool setBasicAuth = true, int requestTimeout = 150 * 1000)
        {
            HttpWebRequest webRequest = WebRequest.Create(requestUrl) as HttpWebRequest;
            webRequest.Method = method;
            if (!string.IsNullOrEmpty(contentType)) webRequest.ContentType = contentType;
            if (setBasicAuth) webRequest.Headers.Add("Authorization", GetBasicAuth());
            webRequest.Timeout = requestTimeout; // default: 150 sec
            webRequest.ServicePoint.Expect100Continue = false;
            webRequest.KeepAlive = false;

            return webRequest;
        }

        public static string ExcuteWebAPI(HttpWebRequest request, string jsonData)
        {
            string responseText = string.Empty;
            HttpContext.Current.Session["WebAPI_ErrorMessage"] = null;

            try
            {
                byte[] bytes = Encoding.ASCII.GetBytes(jsonData);
                request.ContentLength = bytes.Length;

                using (Stream reqStream = request.GetRequestStream())
                {
                    reqStream.Write(bytes, 0, bytes.Length);
                }

                using (WebResponse response = request.GetResponse())
                {
                    Stream resStream = response.GetResponseStream();
                    using (var sr = new StreamReader(resStream))
                    {
                        responseText = sr.ReadToEnd();
                    }
                }
            }
            catch (Exception ex)
            {
                HttpContext.Current.Session["WebAPI_ErrorMessage"] = ex.Message;
                ProcessException(ex, ErrorConstants.ErrCommon3001, 3, "" + request.RequestUri.OriginalString);
            }

            return responseText;
        }

        public static void ExtractErrorMessageFromResponce(string responseText)
        {
            int start = responseText.IndexOf("ErrorMessage");
            int end = responseText.IndexOf("ResultMessage");
            if (end > start)
                HttpContext.Current.Session["WebAPI_ErrorMessage"] = responseText.Substring(start + 15, end - 3);
        }

        public static string GetMismoVersion(MismoVersion version)
        {
            string versionStr = string.Empty;

            switch (version)
            {
                case MismoVersion.V2_3_1:
                    versionStr = "2.3.1";
                    break;
                case MismoVersion.V2_4_1_1:
                    versionStr = "2.4.1.1";
                    break;
                case MismoVersion.V3_4:
                    versionStr = "3.4";
                    break;
                default:
                    versionStr = "2.3.1";
                    break;
            }

            return versionStr;
        }
        #endregion

        #region Enums
        public enum MismoVersion
        {
            V2_3_1,
            V2_4_1_1,
            V3_4
        }
        #endregion
    }
}