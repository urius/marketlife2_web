using Commands;
using Holders;
using Infra.CommandExecutor;
using Infra.EventBus;
using Infra.Instance;
using Providers;
using UnityEngine;
using Utils;
using View.Game;
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

    private PlayerModelHolder _playerModelHolder;
    private GameRootMediator _gameRootMediator;
    private UIRootMediator _uiRootMediator;

    private void Awake()
    {
        SetupInstances();
    }

    private async void Start()
    {
        var commandExecutor = Instance.Get<ICommandExecutor>();

        InitializeRootMediators();
        
        await commandExecutor.ExecuteAsync<InitPlayerModelCommand, PlayerModelHolder>(_playerModelHolder);
    }

    private void InitializeRootMediators()
    {
        _gameRootMediator = new GameRootMediator();
        _gameRootMediator.Mediate(_gameRootView.transform);

        _uiRootMediator = new UIRootMediator();
        _uiRootMediator.Mediate(_uiRootView.transform);
    }

    private void SetupInstances()
    {
        SetupInstance.From(new MainCameraHolder(_mainCamera)).As<IMainCameraHolder>();
        SetupInstance.From(new GridCalculator(_floorGrid)).As<IGridCalculator>();
        SetupInstance.From(_defaultPlayerDataHolder).AsSelf();
        SetupInstance.From(_prefabsHolder).AsSelf();
        SetupInstance.From(_spritesHolder).AsSelf();
        
        SetupNewInstance<ScreenCalculator, IScreenCalculator>();
        
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
