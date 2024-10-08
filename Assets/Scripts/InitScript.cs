using Commands;
using Events;
using Holders;
using Infra.CommandExecutor;
using Infra.EventBus;
using Infra.Instance;
using Systems;
using Tools;
using Tools.AssetBundles;
using Tools.AudioManager;
using Tools.GameObjectsCache;
using Tools.StreamingAudioPlayer;
using UnityEngine;
using Utils;
using View.Camera;
using View.Game;
using View.Game.Shared;
using View.UI;

public class InitScript : MonoBehaviour
{
    [SerializeField] private UIRootView _uiRootView;
    [SerializeField] private GameRootView _gameRootView;
    [SerializeField] private Grid _floorGrid;
    [SerializeField] private Camera _mainCamera;
    [SerializeField] private DefaultPlayerDataHolderSo _defaultPlayerDataHolder;
    [SerializeField] private PrefabsHolderSo _prefabsHolder;
    [SerializeField] private CommonGameSettingsSo _commonGameSettings;
    [SerializeField] private LocalizationsHolderSo _localizationsHolder;
    [SerializeField] private SpritesHolderSo _spritesHolder;
    [SerializeField] private BuildPointsDataHolderSo _buildPointsDataHolder;
    [SerializeField] private TruckPointsSettingsProviderSo _truckPointsSettingsProvider;
    [SerializeField] private InteriorDataProviderSo _interiorDataProviderSo;
    [SerializeField] private UpdatesProvider _updatesProvider;
    [SerializeField] private GameObjectsCache _gameObjectsCache;
    [SerializeField] private AudioManager _audioManager;
    [SerializeField] private SoundsHolderSo _soundsHolder;
    [SerializeField] private StreamingAudioLoader _streamingAudioLoader;
    [SerializeField] private AssetBundlesLoader _assetBundlesLoader;

    private PlayerModelHolder _playerModelHolder;
    private GameRootMediator _gameRootMediator;
    private UIRootMediator _uiRootMediator;
    private RootSystem _rootSystem;
    private MainCameraMediator _mainCameraMediator;
    private EventCommandMapper _eventCommandMapper;

    private void Awake()
    {
        QualitySettings.vSyncCount = 1;
        Application.targetFrameRate = 50;

        SetupInstances();
    }

    private async void Start()
    {
        var commandExecutor = Instance.Get<ICommandExecutor>();

        InitAudioManager();

        InitializeRootSystem();
        InitializeRootMediators();

        await GamePushWrapper.Init();
        
        _localizationsHolder.SetLocaleLang(GamePushWrapper.GetLanguage());
        
        await commandExecutor.ExecuteAsync<InitPlayerModelCommand, PlayerModelHolder>(_playerModelHolder);
    }

    private void InitAudioManager()
    {
        foreach (var soundData in _soundsHolder.SoundsCollection)
        {
            _audioManager.SetupSound((int)soundData.SoundIdKey, soundData.AudioClip);
        }
    }

    private void InitializeRootSystem()
    {
        _rootSystem = new RootSystem();
        _rootSystem.Start();
    }

    private void InitializeRootMediators()
    {
        _gameRootMediator = new GameRootMediator();
        _gameRootMediator.Mediate(_gameRootView.transform);

        _uiRootMediator = new UIRootMediator();
        _uiRootMediator.Mediate(_uiRootView.transform);

        _mainCameraMediator = new MainCameraMediator();
        _mainCameraMediator.Mediate(_mainCamera.transform);
    }

    private void SetupInstances()
    {
        SetupInstance.From(new MainCameraHolder(_mainCamera))
            .As<IMainCameraHolder>()
            .As<IPlayerFocusProvider>()
            .As<IPlayerFocusSetter>();
        SetupInstance.From(new GridCalculator(_floorGrid)).As<IGridCalculator>();
        SetupInstance.From(_updatesProvider).As<IUpdatesProvider>();
        SetupInstance.From(_defaultPlayerDataHolder).AsSelf();
        SetupInstance.From(_prefabsHolder).AsSelf();
        SetupInstance.From(_spritesHolder).AsSelf();
        SetupInstance.From(_buildPointsDataHolder).AsSelf();
        SetupInstance.From(_truckPointsSettingsProvider).AsSelf();
        SetupInstance.From(_interiorDataProviderSo).As<IInteriorDataProvider>();
        SetupInstance.From(_gameObjectsCache).As<IGameObjectsCache>();
        SetupInstance.From(_commonGameSettings).As<ICommonGameSettings>();
        SetupInstance.From(_localizationsHolder).As<ILocalizationProvider>();
        SetupInstance.From(_audioManager).As<IAudioPlayer>();
        SetupInstance.From(_streamingAudioLoader).As<IStreamingAudioLoader>();
        SetupInstance.From(_assetBundlesLoader).As<IAssetBundlesLoader>();
        
        var commandExecutor = SetupNewInstance<CommandExecutor, ICommandExecutor>();
        var eventBus = SetupNewInstance<EventBus, IEventBus>();
        _eventCommandMapper = new EventCommandMapper(eventBus, commandExecutor);

        MapEventsToCommands();
        
        SetupNewInstance<ScreenCalculator, IScreenCalculator>();
        SetupNewInstance<ShopModelHolder, IShopModelHolder>();
        SetupNewInstance<OwnedCellsDataHolder, IOwnedCellsDataHolder>();
        SetupNewInstance<ShelfUpgradeSettingsProvider, IShelfUpgradeSettingsProvider>();
        SetupNewInstance<PlayerCharViewSharedDataHolder, IPlayerCharViewSharedDataHolder>();
        SetupNewInstance<SharedViewsDataHolder, ISharedViewsDataHolder>();
        SetupNewInstance<UpgradeCostProvider, IUpgradeCostProvider>();
        SetupNewInstance<HireStaffCostProvider, IHireStaffCostProvider>();
        SetupNewInstance<ViewModelsHolder, IPopupViewModelsHolder, IAdsOfferViewModelsHolder>();
        SetupNewInstance<SharedFlagsHolder, ISharedFlagsHolder>();
        
        _playerModelHolder = SetupNewInstance<PlayerModelHolder, IPlayerModelHolder>();
        
        SetupInstance.AllSetupsDone();
    }

    private void MapEventsToCommands()
    {
        Map<UIShelfUpgradeClickedEvent, UpgradeShelfCommand>();
        Map<UIInteriorButtonClickedEvent, ShowInteriorPopupCommand>();
        Map<UIInteriorPopupItemClickedEvent, ProcessInteriorPopupItemClickedCommand>();
        Map<UIRequestClosePopupEvent, ProcessClosePopupCommand>();
        Map<UIInteriorPopupTabShownEvent, ProcessInteriorPopupTabShownCommand>();
        Map<UISettingsButtonClickedEvent, ProcessSettingsButtonClickedCommand>();
    }

    private void Map<TEvent, TCommand>()
        where TCommand : ICommand<TEvent>, new()
    {
        _eventCommandMapper.Map<TEvent, TCommand>();
    }

    private static TInstance SetupNewInstance<TInstance, TInterface>()
        where TInstance : class, TInterface, new() 
        where TInterface : class
    {
        var instance = new TInstance();
        SetupInstance.From(instance).As<TInterface>();

        return instance;
    }
    
    private static TInstance SetupNewInstance<TInstance, TInterface1, TInterface2>()
        where TInstance : class, TInterface1 , TInterface2, new() 
        where TInterface1 : class
        where TInterface2 : class
    {
        var instance = new TInstance();
        SetupInstance.From(instance)
            .As<TInterface1>()
            .As<TInterface2>();

        return instance;
    }
}
