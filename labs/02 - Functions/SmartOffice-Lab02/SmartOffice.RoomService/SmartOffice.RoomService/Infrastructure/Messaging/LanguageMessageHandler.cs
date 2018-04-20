using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace Neudesic.SmartOffice.RoomService.Infrastructure.Messaging
{
    /// <summary>
    /// 
    /// </summary>
    public class LanguageMessageHandler : DelegatingHandler
    {
        #region Private Members

        private const string LanguageBahasaIndonesia = "id";
        private const string LanguageBahasaMalaysia = "ms";
        private const string LanguageChinese = "zh";
        private const string LanguageChineseHongKong = "zh-HK";
        private const string LanguageChineseSingapore = "zh-SG";
        private const string LanguageChineseTaiwan = "zh-TW";
        private const string LanguageEnglish = "en";
        private const string LanguageEnglishAustralia = "en-AU";
        private const string LanguageEnglishCanada = "en-CA";
        private const string LanguageEnglishNewZealand = "en-NZ";
        private const string LanguageEnglishUnitedKingdom = "en-GB";
        private const string LanguageEnglishUnitedStates = "en-US";
        private const string LanguageFrenchCanada = "fr-CA";
        private const string LanguageSpanish = "es";
        private const string LanguageSpanishColumbia = "es-CO";
        private const string LanguageSpanishMexico = "es-MX";
        private const string LanguageSpanishPuertoRico = "es-PR";
        private const string LanguageSpanishUnitedStates = "es-US";

        private readonly List<string> _supportedLanguages = new List<string>
        {
            LanguageBahasaIndonesia,
            LanguageBahasaMalaysia,
            LanguageChinese,
            LanguageChineseHongKong,
            LanguageChineseSingapore,
            LanguageChineseTaiwan,
            LanguageEnglish,
            LanguageEnglishAustralia,
            LanguageEnglishCanada,
            LanguageEnglishNewZealand,
            LanguageEnglishUnitedKingdom,
            LanguageEnglishUnitedStates,
            LanguageFrenchCanada,
            LanguageSpanish,
            LanguageSpanishColumbia,
            LanguageSpanishMexico,
            LanguageSpanishPuertoRico,
            LanguageSpanishUnitedStates
        };

        #endregion

        #region Private Methods

        private void SetCulture(HttpRequestMessage request, string language)
        {
            request.Headers.AcceptLanguage.Clear();
            request.Headers.AcceptLanguage.Add(new StringWithQualityHeaderValue(language));

            Thread.CurrentThread.CurrentCulture = new CultureInfo(language);
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(language);
        }

        private bool SetHeaderIfAcceptLanguageMatchesSupportedLanguage(HttpRequestMessage request)
        {
            try
            {
                // Check for a full language identifier match (primary-sublanguage)
                foreach (var language in request.Headers.AcceptLanguage)
                {
                    if (_supportedLanguages.Contains(language.Value))
                    {
                        SetCulture(request, language.Value);

                        return true;
                    }
                }

                // Check for a default language identifier match (primary)
                foreach (var language in request.Headers.AcceptLanguage)
                {
                    var primaryLanguageIdentifier = language.Value.Substring(0, 2);

                    // Check for exact match on primary language identifier
                    if (_supportedLanguages.Contains(primaryLanguageIdentifier))
                    {
                        SetCulture(request, primaryLanguageIdentifier);

                        return true;
                    }

                    // Check for single match on primary language identifier
                    if (_supportedLanguages.Count(l => l.StartsWith(primaryLanguageIdentifier)).Equals(1))
                    {
                        var supportedLanguage = _supportedLanguages.First(l => l.StartsWith(primaryLanguageIdentifier));

                        SetCulture(request, supportedLanguage);

                        return true;
                    }
                }

                return false;
            }
            catch
            {
                return false;
            }
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (!SetHeaderIfAcceptLanguageMatchesSupportedLanguage(request))
            {
                // No localization found - use English US
                SetCulture(request, LanguageEnglishUnitedStates);
            }

            var response = await base.SendAsync(request, cancellationToken);

            return response;
        }
    }
}