using MelonLoader;
using S1API.UI;
using ScheduleOne.DevUtilities;
using ScheduleOne.Property;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

[assembly: MelonInfo(typeof(YahYah.Schedule1Mods.LaunderingStatus.ModMain), "LaunderingStatus", "1.0.1", "YahYah Game Studio")]
[assembly: MelonGame("TVGS", "Schedule I")]
namespace YahYah.Schedule1Mods.LaunderingStatus
{
    public class ModMain : MelonMod
    {
        private GameObject _hud;
        private Text _label;
        
        public override void OnInitializeMelon()
        {
            MelonLogger.Msg("LaunderingStatus Initialized!");
        }

        public override void OnUpdate()
        {
            // Wait until GameManager is initialized
            if (GameManager.Instance == null)
            {
                return;
            }

            // Find a reference to the HUD only if the current reference is null
            if (_hud == null)
            {
                var found = GameObject.Find("HUD");
                
                if (found == null)
                    return;

                MelonLogger.Msg("HUD found. Beginning rendering.");
                _hud = found;
                InstantiateUI();
            }
            
            // Find all active laundering operations
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
            
            // Format and print each laundering operation to the HUD
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
            _label = UIFactory.Text("Laundering Status Label", string.Empty, _hud.transform, anchor: TextAnchor.UpperRight, style: FontStyle.Bold);
            _label.rectTransform.anchorMin = Vector2.one;
            _label.rectTransform.anchorMax = Vector2.one;
            _label.rectTransform.pivot = Vector2.one;
            _label.rectTransform.anchoredPosition = -Vector2.one * 10;
            _label.horizontalOverflow = HorizontalWrapMode.Overflow;
            
            var instantiated = Object.Instantiate(_label);
            var contentSizeFitter = instantiated.gameObject.AddComponent<ContentSizeFitter>();
            contentSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            contentSizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
        }
    }
}