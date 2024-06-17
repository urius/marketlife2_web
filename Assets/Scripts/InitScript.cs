using Commands;
using Holders;
using Infra.CommandExecutor;
using Infra.EventBus;
using Infra.Instance;
using Systems;
using Tools.GameObjectsCache;
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
    [SerializeField] private SpritesHolderSo _spritesHolder;
    [SerializeField] private BuildPointsDataHolderSo _buildPointsDataHolder;
    [SerializeField] private TruckPointsSettingsProviderSo _truckPointsSettingsProvider;
    [SerializeField] private UpdatesProvider _updatesProvider;
    [SerializeField] private GameObjectsCache _gameObjectsCache;

    private PlayerModelHolder _playerModelHolder;
    private GameRootMediator _gameRootMediator;
    private UIRootMediator _uiRootMediator;
    private RootSystem _rootSystem;
    private MainCameraMediator _mainCameraMediator;

    private void Awake()
    {
        SetupInstances();
    }

    private async void Start()
    {
        var commandExecutor = Instance.Get<ICommandExecutor>();

        InitializeRootSystem();
        InitializeRootMediators();
        
        await commandExecutor.ExecuteAsync<InitPlayerModelCommand, PlayerModelHolder>(_playerModelHolder);
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
        SetupInstance.From(new MainCameraHolder(_mainCamera)).As<IMainCameraHolder>();
        SetupInstance.From(new GridCalculator(_floorGrid)).As<IGridCalculator>();
        SetupInstance.From(_updatesProvider).As<IUpdatesProvider>();
        SetupInstance.From(_defaultPlayerDataHolder).AsSelf();
        SetupInstance.From(_prefabsHolder).AsSelf();
        SetupInstance.From(_spritesHolder).AsSelf();
        SetupInstance.From(_buildPointsDataHolder).AsSelf();
        SetupInstance.From(_truckPointsSettingsProvider).AsSelf();
        SetupInstance.From(_gameObjectsCache).As<IGameObjectsCache>();
        
        SetupNewInstance<ScreenCalculator, IScreenCalculator>();
        SetupNewInstance<ShopModelHolder, IShopModelHolder>();
        SetupNewInstance<OwnedCellsDataHolder, IOwnedCellsDataHolder>();
        SetupNewInstance<ShelfSettingsProvider, IShelfSettingsProvider>();
        SetupNewInstance<PlayerCharViewSharedDataHolder, IPlayerCharViewSharedDataHolder>();
        
        _playerModelHolder = SetupNewInstance<PlayerModelHolder, IPlayerModelHolder>();
        var commandExecutor = SetupNewInstance<CommandExecutor, ICommandExecutor>();
        var eventBus = SetupNewInstance<EventBus, IEventBus>();

        MapEventsToCommands(new EventCommandMapper(eventBus, commandExecutor));
        
        SetupInstance.AllSetupsDone();
    }

    private void MapEventsToCommands(EventCommandMapper eventToCommandMapper)
    {
        //eventToCommandMapper.Map<>();
    }

    private static TInstance SetupNewInstance<TInstance, TInterface>()
        where TInstance : class, TInterface, new() 
        where TInterface : class
    {
        var instance = new TInstance();
        SetupInstance.From(instance).As<TInterface>();

        return instance;
    }
}
