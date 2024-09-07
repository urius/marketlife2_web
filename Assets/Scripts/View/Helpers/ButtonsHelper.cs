using Data;
using Holders;
using Infra.Instance;
using Model.People;
using Utils;
using View.UI.Common;

namespace View.Helpers
{
    public static class ButtonsHelper
    {
        public static void SetupHireButtonText(UITextButtonView buttonView, StaffCharModelBase staffCharModel, int hireStaffCost)
        {
            var playerModelHolder = Instance.Get<IPlayerModelHolder>();
            var localizationProvider = Instance.Get<ILocalizationProvider>();
        
            var playerModel = playerModelHolder.PlayerModel;
            if (staffCharModel != null)
            {
                buttonView.SetInteractable(staffCharModel.WorkSecondsLeft <=
                                           playerModel.StaffWorkTimeSeconds * 2);
            }
            else
            {
                buttonView.SetInteractable(true);
            }

            switch (hireStaffCost)
            {
                case > 0:
                {
                    var textKey = staffCharModel == null
                        ? Constants.LocalizationHireButton
                        : Constants.LocalizationProlongButton;
                    var hireText =
                        $"{localizationProvider.GetLocale(textKey)}\n{FormattingHelper.ToMoneyWithIconTextFormat(hireStaffCost)}";
                    
                    buttonView.SetText(hireText);
                    buttonView.SetOrangeSkinData();
                    break;
                }
                case 0:
                {
                    var hireText =
                        $"{Constants.TextIconAds}\n{localizationProvider.GetLocale(Constants.LocalizationHireButton)} x{Constants.HireByAdvertWorkTimeMultiplier}";
                    
                    buttonView.SetText(hireText);
                    buttonView.SetCrimsonSkinData();
                    break;
                }
                default:
                    buttonView.SetText(localizationProvider.GetLocale(Constants.LocalizationHireButton));
                    buttonView.SetOrangeSkinData();
                    break;
            }
        }
    }
}