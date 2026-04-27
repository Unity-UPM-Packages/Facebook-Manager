using System.Collections;
using System.Collections.Generic;
using System.Text;
using Facebook.Unity;
using TheLegends.Base.UnitySingleton;
using UnityEngine;

namespace TheLegends.Base.Facebook
{
    public class FacebookManager : PersistentMonoSingleton<FacebookManager>
    {
        public enum FacebookStatus
        {
            Initializing,
            Initialized,
        }

        public FacebookStatus Status { get; private set; }

        private readonly WaitForSeconds initDelay = new WaitForSeconds(0.1f);

        void OnApplicationPause(bool pauseStatus)
        {
#if USE_FACEBOOK
            if (!pauseStatus)
            {
                InitFacebookSDK();
            }
#endif
        }

        public IEnumerator DoInit()
        {
#if USE_FACEBOOK
            if (Status == FacebookStatus.Initializing)
            {
                yield break;
            }

            Status = FacebookStatus.Initializing;

            InitFacebookSDK();

            while (Status == FacebookStatus.Initializing)
            {
                yield return null;
            }

            yield return initDelay;
#endif
        }

        private void InitFacebookSDK()
        {
#if USE_FACEBOOK
            if (FB.IsInitialized)
            {
                FB.ActivateApp();
                Status = FacebookStatus.Initialized;
            }
            else
            {
                FB.Init(() =>
                {
                    FB.ActivateApp();
                    OnFacebookInitComplete();
                    Status = FacebookStatus.Initialized;
                });
            }
#endif
        }

        private void OnFacebookInitComplete()
        {
#if USE_FACEBOOK
            Log("FACEBOOK SDK INIT COMPLETE");
#endif
        }

        public void LogEvent(string eventName, float valueToSum, Dictionary<string, object> parameters = null)
        {
#if USE_FACEBOOK
            if (Status != FacebookStatus.Initialized)
            {
                LogError("Facebook not initialized");
                return;
            }

            FB.LogAppEvent(eventName, valueToSum, parameters);

            string paramStr = GetParamsStr(parameters);
            Log("Event Recorded: " + eventName + " | Value: " + valueToSum + " | Parameters: " + paramStr);
#endif
        }

        public void LogPurchase(float amount, string currency = "USD", Dictionary<string, object> parameters = null)
        {
#if USE_FACEBOOK
            if (Status != FacebookStatus.Initialized)
            {
                LogError("Facebook not initialized");
                return;
            }

            FB.LogPurchase(amount, currency, parameters);

            string paramStr = GetParamsStr(parameters);
            Log("Purchase Recorded: " + amount + " | Currency: " + currency + " | Parameters: " + paramStr);
#endif
        }

        public void Log(string message)
        {
            Debug.Log("FacebookManager------: " + message);
        }

        public void LogWarning(string message)
        {
            Debug.LogWarning("FacebookManager------: " + message);
        }

        public void LogError(string message)
        {
            Debug.LogError("FacebookManager------: " + message);
        }

        private string GetParamsStr(Dictionary<string, object> parameters)
        {
            if (parameters == null || parameters.Count == 0)
            {
                return "No Parameters";
            }

            var sb = new StringBuilder();

            foreach (var param in parameters)
            {
                sb.Append(param.Key)
                  .Append(": ")
                  .Append(param.Value)
                  .Append(", ");
            }

            return sb.ToString();
        }
    }
}
