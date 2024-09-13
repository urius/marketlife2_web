using Data;
using Holders;
using Infra.Instance;

namespace View.UI.Tutorial.Steps
{
    public class UITutorialStepTakeProductsFromTruckPointSecondTimeMediator : UITutorialStepTakeProductsFromTruckPointMediator
    {
        private readonly ILocalizationProvider _localizationProvider = Instance.Get<ILocalizationProvider>();
        
        protected override string MessageText =>
            _localizationProvider.GetLocale(Constants.LocalizationTutorialTakeProductsFromTruckPointSecondTimeMessageKey);
    }
}