using Data;
using Holders;
using Infra.Instance;
using Model;
using View.Game.Shared;
using View.Helpers;

namespace View.Game.People
{
    public class PlayerCharMediator : MediatorBase
    {
        private readonly IPlayerCharViewSharedDataHolder _playerCharViewSharedDataHolder =
            Instance.Get<IPlayerCharViewSharedDataHolder>();

        private readonly SpritesHolderSo _spritesHolderSo = Instance.Get<SpritesHolderSo>();
        private readonly IPlayerModelHolder _playerModelHolder = Instance.Get<IPlayerModelHolder>();

        private ManView _playerCharView;
        private PlayerDressesModel _dressesModel;

        protected override void MediateInternal()
        {
            _dressesModel = _playerModelHolder.PlayerCharModel.DressesModel;

            _playerCharView = InstantiatePrefab<ManView>(PrefabKey.Man);

            UpdateView();

            MediateChild<PlayerCharMovementMediator>(_playerCharView.transform);
            MediateChild<PlayerCharMoneyAnimationMediator>(TargetTransform);
            MediateChild(new PlayerCharProductsMediator(_playerCharView));
            MediateChild(new PlayerCharCompassMediator(_playerCharView));

            _playerCharViewSharedDataHolder.SetView(_playerCharView);

            Subscribe();
        }

        protected override void UnmediateInternal()
        {
            Unsubscribe();

            Destroy(_playerCharView);
            _playerCharView = null;
        }

        private void Subscribe()
        {
            _dressesModel.TopDressTypeChanged += OnTopDressTypeChanged;
            _dressesModel.BottomDressTypeChanged += OnBottomDressTypeChanged;
            _dressesModel.HairTypeChanged += OnHairTypeChanged;
            _dressesModel.GlassesTypeChanged += OnGlassesTypeChanged;
        }

        private void Unsubscribe()
        {
            _dressesModel.TopDressTypeChanged -= OnTopDressTypeChanged;
            _dressesModel.BottomDressTypeChanged -= OnBottomDressTypeChanged;
            _dressesModel.HairTypeChanged -= OnHairTypeChanged;
            _dressesModel.GlassesTypeChanged -= OnGlassesTypeChanged;
        }

        private void OnTopDressTypeChanged(ManSpriteType _)
        {
            UpdateView();
        }

        private void OnBottomDressTypeChanged(ManSpriteType _)
        {
            UpdateView();
        }

        private void OnHairTypeChanged(ManSpriteType _)
        {
            UpdateView();
        }

        private void OnGlassesTypeChanged(ManSpriteType _)
        {
            UpdateView();
        }

        private void UpdateView()
        {
            var topClothes = ManSpriteTypesHelper.GetClothesByIntType((int)_dressesModel.TopDressType);
            var footClothes = _dressesModel.BottomDressType;
            var hairType = _dressesModel.HairType;
            var glassesType = _dressesModel.GlassesType;

            var bodySprite = _spritesHolderSo.GetManSpriteByKey(topClothes.BodyClothes);
            var handSprite = _spritesHolderSo.GetManSpriteByKey(topClothes.HandClothes);
            var footSprite = _spritesHolderSo.GetManSpriteByKey(footClothes);
            var hairSprite = _spritesHolderSo.GetManSpriteByKey(hairType);

            _playerCharView.SetClothesSprites(bodySprite, handSprite, footSprite);
            _playerCharView.SetHairSprite(hairSprite);

            var glassesSprite = _spritesHolderSo.GetManSpriteByKey(glassesType);
            _playerCharView.SetGlassesSprite(glassesSprite);

            if (DateTimeHelper.IsNewYearsEve())
            {
                var hatSprite = _spritesHolderSo.GetManSpriteByKey(ManSpriteType.SantaHat);
                _playerCharView.SetHatSprite(hatSprite);
            }
            else
            {
                _playerCharView.SetHatSprite(null);
            }
        }
    }
}