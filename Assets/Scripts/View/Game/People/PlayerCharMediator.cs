using Data;
using Holders;
using Infra.Instance;
using View.Game.Shared;
using View.Helpers;

namespace View.Game.People
{
    public class PlayerCharMediator : MediatorBase
    {
        private readonly IPlayerCharViewSharedDataHolder _playerCharViewSharedDataHolder = Instance.Get<IPlayerCharViewSharedDataHolder>();
        private readonly SpritesHolderSo _spritesHolderSo = Instance.Get<SpritesHolderSo>();
        
        private ManView _playerCharView;

        protected override void MediateInternal()
        {
            _playerCharView = InstantiatePrefab<ManView>(PrefabKey.Man);

            SetupView();
            
            MediateChild<PlayerCharMovementMediator>(_playerCharView.transform);
            MediateChild(new PlayerCharMoneyAnimationMediator(_playerCharView));
            MediateChild(new PlayerCharProductsMediator(_playerCharView));

            _playerCharViewSharedDataHolder.SetView(_playerCharView);
        }

        protected override void UnmediateInternal()
        {
            Destroy(_playerCharView);
            _playerCharView = null;
        }

        private void SetupView()
        {
            var clothes = ManSpriteTypesHelper.GetClothesByIntType((int)ManSpriteType.Clothes3);
            var hairType = ManSpriteType.Hair2;
            var glassesType = ManSpriteType.Glasses1;
            
            var bodySprite = _spritesHolderSo.GetManSpriteByKey(clothes.BodyClothes);
            var handSprite = _spritesHolderSo.GetManSpriteByKey(clothes.HandClothes);
            var footSprite = _spritesHolderSo.GetManSpriteByKey(clothes.FootClothes);
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