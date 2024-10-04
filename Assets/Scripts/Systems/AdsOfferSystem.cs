using System;
using Cysharp.Threading.Tasks;
using Data;
using Events;
using Extensions;
using Holders;
using Infra.EventBus;
using Infra.Instance;
using Model;
using Model.AdsOffer;
using Tools;
using Tools.AudioManager;
using Random = UnityEngine.Random;

namespace Systems
{
    public class AdsOfferSystem : ISystem
    {
        private const int ShowAdsOfferCooldownSeconds = 50;
        private const int ShowAdsOfferTimeSeconds = 10;
        
        private readonly IPlayerModelHolder _playerModelHolder = Instance.Get<IPlayerModelHolder>();
        private readonly IAdsOfferViewModelsHolder _adsOfferViewModelsHolder = Instance.Get<IAdsOfferViewModelsHolder>();
        private readonly ICommonGameSettings _commonGameSettings = Instance.Get<ICommonGameSettings>();
        private readonly IUpdatesProvider _updatesProvider = Instance.Get<IUpdatesProvider>();
        private readonly IAudioPlayer _audioPlayer = Instance.Get<IAudioPlayer>();
        private readonly IEventBus _eventBus = Instance.Get<IEventBus>();
        
        private PlayerModel _playerModel;
        private int _secondsSinceLastOfferShown = int.MaxValue;

        private bool CanShowOffer => _adsOfferViewModelsHolder.CurrentAdsOfferViewModel == null
                                     && GamePushWrapper.CanShowRewardedAds();

        public void Start()
        {
            _playerModel = _playerModelHolder.PlayerModel;

            Subscribe();
        }

        public void Stop()
        {
            Unsubscribe();
        }

        private void Subscribe()
        {
            _playerModel.MoneyChanged += OnMoneyChanged;
            _playerModel.InsufficientFunds += OnInsufficientFunds;
            _updatesProvider.RealtimeSecondPassed += OnRealtimeSecondPassed;
            _updatesProvider.GameplaySecondPassed += OnGameplaySecondPassed;
            
            _eventBus.Subscribe<AdsOfferClickedEvent>(OnAdsOfferClickedEvent);
        }

        private void Unsubscribe()
        {
            _playerModel.MoneyChanged -= OnMoneyChanged;
            _playerModel.InsufficientFunds -= OnInsufficientFunds;
            _updatesProvider.RealtimeSecondPassed -= OnRealtimeSecondPassed;
            _updatesProvider.GameplaySecondPassed -= OnGameplaySecondPassed;
            
            _eventBus.Unsubscribe<AdsOfferClickedEvent>(OnAdsOfferClickedEvent);
        }

        private void OnAdsOfferClickedEvent(AdsOfferClickedEvent e)
        {
            ShowAndProcessRewardedAds(e.TargetModel).Forget();
        }

        private async UniTaskVoid ShowAndProcessRewardedAds(AdsOfferViewModelBase adsOfferViewModel)
        {
            var showRewardedAdsResult = await GamePushWrapper.ShowRewardedAds();

            _adsOfferViewModelsHolder.RemoveCurrentAdsOffer();
            
            if (showRewardedAdsResult)
            {
                switch (adsOfferViewModel.AdsOfferType)
                {
                    case AdsOfferType.AddMoney:
                        var adsOfferAddMoneyViewModel = (AdsOfferAddMoneyViewModel)adsOfferViewModel;
                        _playerModel.ChangeMoney(adsOfferAddMoneyViewModel.MoneyAmountToAdd);
                        
                        PlayAdsOfferApplySound();
                        break;
                    case AdsOfferType.MoneyMultiplier:
                        var adsOfferMoneyMultiplierViewModel = (AdsOfferMoneyMultiplierViewModel)adsOfferViewModel;
                        var modifier = new PlayerMoneyEarnModifierModel(
                            adsOfferMoneyMultiplierViewModel.Multiplier,
                            adsOfferMoneyMultiplierViewModel.RewardTimeMin * Constants.SecondsInMinute);
                        _playerModel.AddMoneyEarnModifier(modifier);

                        PlayAdsOfferApplySound();
                        break;
                    default:
                        throw new NotSupportedException(
                            $"{nameof(ShowAndProcessRewardedAds)} unsupported AdsOfferType {adsOfferViewModel.AdsOfferType}");
                }
            }
        }

        private void PlayAdsOfferApplySound()
        {
            _audioPlayer.PlaySound(SoundIdKey.CashSound_2);
        }

        private void OnGameplaySecondPassed()
        {
            if (_secondsSinceLastOfferShown < int.MaxValue)
            {
                _secondsSinceLastOfferShown++;
            }
        }

        private void OnRealtimeSecondPassed()
        {
            ProcessCurrentOfferTimeLeft();
        }

        private void ProcessCurrentOfferTimeLeft()
        {
            if (_adsOfferViewModelsHolder.CurrentAdsOfferViewModel == null) return;
            
            var offer = _adsOfferViewModelsHolder.CurrentAdsOfferViewModel;
            if (offer.OfferTimeLeft < 0)
            {
                RemoveCurrentOffer();
            }
            else
            {
                offer.SetOfferTimeLeft(offer.OfferTimeLeft - 1);
            }
        }

        private void RemoveCurrentOffer()
        {
            _adsOfferViewModelsHolder.RemoveCurrentAdsOffer();

            _secondsSinceLastOfferShown = 0;
        }

        private void OnInsufficientFunds(int delta)
        {
            AddMoneyAdsOfferIfNeeded(delta > 5 ? delta : delta * 2);
        }

        private void OnMoneyChanged(int deltaMoney)
        {
            if (_playerModel.MoneyAmount <= 0)
            {
                AddMoneyAdsOfferIfNeeded();
            }
            else if (deltaMoney > 0
                     && _playerModelHolder.PlayerCharModel.NearCashDesk != null)
            {
                AddMoneyMultiplierAdsOfferIfNeeded();
            }
        }

        private void AddMoneyMultiplierAdsOfferIfNeeded()
        {
            if (CanShowOffer == false) return;
            
            if (_secondsSinceLastOfferShown < ShowAdsOfferCooldownSeconds 
                || _playerModel.MoneyEarnModifier != null) return;

            var multiplier = _playerModel.Level < 4 ? 2 : Random.Range(2, 4);
            var rewardTimeMin = Random.Range(1, Math.Min(_playerModel.Level + 1, 6));
            var offerViewModel =
                new AdsOfferMoneyMultiplierViewModel(ShowAdsOfferTimeSeconds, rewardTimeMin, multiplier);
            _adsOfferViewModelsHolder.SetAdsOffer(offerViewModel);

            _secondsSinceLastOfferShown = 0;
        }

        private void AddMoneyAdsOfferIfNeeded(int requestedAmount = -1)
        {
            if (CanShowOffer == false) return;
            
            var moneyAmountToAdd = requestedAmount;
            if (moneyAmountToAdd <= 0)
            {
                var levelTargetMoney = _commonGameSettings.GetLevelTargetMoney(_playerModel.LevelIndex + 1);
                moneyAmountToAdd = (int)Math.Ceiling(levelTargetMoney * Random.Range(0.25f, 0.45f));
            }

            var offerViewModel = new AdsOfferAddMoneyViewModel(ShowAdsOfferTimeSeconds, moneyAmountToAdd);
            _adsOfferViewModelsHolder.SetAdsOffer(offerViewModel);
        }
    }
}