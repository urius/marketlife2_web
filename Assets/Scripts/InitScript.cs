using Commands;
using Holders;
using Infra.CommandExecutor;
using Infra.EventBus;
using Infra.Instance;
using Providers;
using UnityEngine;
using Utils;
using View.Game;

public class InitScript : MonoBehaviour
{
    [SerializeField] private GameRootView _gameRootView;
    [SerializeField] private Grid _floorGrid;
    [SerializeField] private DefaultPlayerDataHolderSo _defaultPlayerDataHolder;
    [SerializeField] private PrefabsHolderSo _prefabsHolder;

    private PlayerModelHolder _playerModelHolder;
    private GameRootMediator _gameRootMediator;

    private void Awake()
    {
        SetupInstances();
    }

    private async void Start()
    {
        var commandExecutor = Instance.Get<ICommandExecutor>();

        InitializeRootMediator();
        
        await commandExecutor.ExecuteAsync<InitPlayerModelCommand, PlayerModelHolder>(_playerModelHolder);
    }

    private void InitializeRootMediator()
    {
        _gameRootMediator = new GameRootMediator();
        _gameRootMediator.Mediate(_gameRootView.transform);
    }

    private void SetupInstances()
    {
        SetupInstance.From(new GridCalculator(_floorGrid)).As<IGridCalculator>();
        SetupInstance.From(_defaultPlayerDataHolder).AsSelf();
        SetupInstance.From(_prefabsHolder).AsSelf();
        
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
