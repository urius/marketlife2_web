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
        private const int ShowAdsOfferCooldownSeconds = 1;
        private const int ShowAdsOfferTimeSeconds = 10;
        
        private readonly IPlayerModelHolder _playerModelHolder = Instance.Get<IPlayerModelHolder>();
        private readonly IAdsOfferViewModelsHolder _adsOfferViewModelsHolder = Instance.Get<IAdsOfferViewModelsHolder>();
        private readonly ICommonGameSettings _commonGameSettings = Instance.Get<ICommonGameSettings>();
        private readonly IUpdatesProvider _updatesProvider = Instance.Get<IUpdatesProvider>();
        private readonly IAudioPlayer _audioPlayer = Instance.Get<IAudioPlayer>();
        private readonly IEventBus _eventBus = Instance.Get<IEventBus>();
        
        private PlayerModel _playerModel;
        private int _secondsSinceLastAdsShown = ShowAdsOfferCooldownSeconds;

        private bool CanShowOffer => _adsOfferViewModelsHolder.CurrentAdsOfferViewModel == null 
                                     && _secondsSinceLastAdsShown > ShowAdsOfferCooldownSeconds
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
            
            _eventBus.Subscribe<AdsOfferClickedEvent>(OnAdsOfferClickedEvent);
        }

        private void Unsubscribe()
        {
            _playerModel.MoneyChanged -= OnMoneyChanged;
            _playerModel.InsufficientFunds -= OnInsufficientFunds;
            _updatesProvider.RealtimeSecondPassed -= OnRealtimeSecondPassed;
            
            _eventBus.Unsubscribe<AdsOfferClickedEvent>(OnAdsOfferClickedEvent);
        }

        private void OnAdsOfferClickedEvent(AdsOfferClickedEvent e)
        {
            ShowAndProcessRewardedAds(e.TargetModel).Forget();
        }

        private async UniTaskVoid ShowAndProcessRewardedAds(AdsOfferViewModelBase adsOfferViewModel)
        {
            var showRewardedAdsResult = await GamePushWrapper.ShowRewardedAds();

            _secondsSinceLastAdsShown = 0;
            _adsOfferViewModelsHolder.RemoveCurrentAdsOffer();
            
            if (showRewardedAdsResult)
            {
                switch (adsOfferViewModel.AdsOfferType)
                {
                    case AdsOfferType.AddMoney:
                        var adsOfferAddMoneyViewModel = (AdsOfferAddMoneyViewModel)adsOfferViewModel;
                        _playerModel.ChangeMoney(adsOfferAddMoneyViewModel.MoneyAmountToAdd);
                        
                        _audioPlayer.PlaySound(SoundIdKey.CashSound_2);
                        break;
                    default:
                        throw new NotSupportedException(
                            $"{nameof(ShowAndProcessRewardedAds)} unsupported AdsOfferType {adsOfferViewModel.AdsOfferType}");
                }
            }
        }

        private void OnRealtimeSecondPassed()
        {
            if (_adsOfferViewModelsHolder.CurrentAdsOfferViewModel != null)
            {
                var offer = _adsOfferViewModelsHolder.CurrentAdsOfferViewModel;
                if (offer.OfferTimeLeft < 0)
                {
                    _adsOfferViewModelsHolder.RemoveCurrentAdsOffer();
                }
                else
                {
                    offer.SetOfferTimeLeft(offer.OfferTimeLeft - 1);
                }
            }
            else
            {
                _secondsSinceLastAdsShown++;
            }
        }

        private void OnInsufficientFunds(int delta)
        {
            AddMoneyAdsOfferIfNeeded(delta > 5 ? delta : delta * 2);
        }

        private void OnMoneyChanged(int _)
        {
            if (_playerModel.MoneyAmount <= 0)
            {
                AddMoneyAdsOfferIfNeeded();
            }
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