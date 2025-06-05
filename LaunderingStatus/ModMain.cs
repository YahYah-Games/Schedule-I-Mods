using System.Globalization;
using MelonLoader;
using S1API.UI;
using ScheduleOne.DevUtilities;
using ScheduleOne.Property;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

[assembly: MelonInfo(typeof(YahYah.Schedule1Mods.LaunderingStatus.ModMain), "LaunderingStatus", "1.0.0", "YahYah Game Studio")]
[assembly: MelonGame("TVGS", "Schedule I")]
namespace YahYah.Schedule1Mods.LaunderingStatus
{
    public class ModMain : MelonMod
    {
        private bool _initialized;
        private GameObject _canvas;
        private Text _label;
        
        public override void OnInitializeMelon()
        {
            MelonLogger.Msg("LaunderingStatus Initialized!");
        }

        public override void OnUpdate()
        {
            if (!_initialized && GameManager.Instance != null)
            {
                _initialized = true;
                MelonLogger.Msg("Game Manager Initialized!");
            }
            
            if (!_initialized)
                return;

            if (_canvas == null)
            {
                var found = GameObject.Find("HUD");
                
                if (found == null)
                    return;

                MelonLogger.Msg("HUD found. Beginning rendering.");
                _canvas = found;
                InstantiateUI();
            }
            
            var operations = new List<LaunderingOperation>();
            foreach (var business in Business.OwnedBusinesses)
            {
                operations.AddRange(business.LaunderingOperations);
            }

            if (operations.Count > 0)
            {
                _label.text = "Laundering Operations:\n";
            }
            else
            {
                _label.text = string.Empty;
                return;
            }
            
            foreach (var operation in operations)
            {
                var timeRemainingMinutes = operation.completionTime_Minutes - operation.minutesSinceStarted;
                var timeRemainingHours = Mathf.FloorToInt(timeRemainingMinutes / 60f);
                var remainingLabel = timeRemainingHours > 0 ?
                    $"{timeRemainingHours} hour{(timeRemainingHours > 1 ? "s" : "")} remaining" :
                    $"{timeRemainingMinutes} minute{(timeRemainingMinutes > 1 ? "s" : "")} remaining";
                _label.text += $"{operation.business.PropertyName}: ${operation.amount:N0}, {remainingLabel}\n";
            }
        }

        private void InstantiateUI()
        {
            _label = UIFactory.Text("Laundering Status Label", string.Empty, _canvas.transform, anchor: TextAnchor.UpperRight, style: FontStyle.Bold);
            _label.rectTransform.anchorMin = Vector2.one;
            _label.rectTransform.anchorMax = Vector2.one;
            _label.rectTransform.pivot = Vector2.one;
            _label.rectTransform.anchoredPosition = Vector2.zero;
            _label.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 1000);
            
            Object.Instantiate(_label);
        }
    }
}