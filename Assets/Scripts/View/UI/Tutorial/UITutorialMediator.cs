using System.Collections.Generic;
using Data;
using Holders;
using Infra.Instance;
using Model;
using UnityEngine;
using View.UI.Tutorial.Steps;

namespace View.UI.Tutorial
{
    public class UITutorialMediator : MediatorBase
    {
        private readonly IPlayerModelHolder _playerModelHolder = Instance.Get<IPlayerModelHolder>();

        private readonly Dictionary<TutorialStep, UITutorialStepMediatorBase> _tutorialStepMediators = new();
        
        private UITutorialRootCanvasView _tutorialCanvasView;
        private PlayerModel _playerModel;

        protected override void MediateInternal()
        {
            _playerModel = _playerModelHolder.PlayerModel;
            
            _tutorialCanvasView = TargetTransform.GetComponent<UITutorialRootCanvasView>();

            InitStepMediators();

            Subscribe();
        }

        protected override void UnmediateInternal()
        {
            Unsubscribe();
            
            _tutorialCanvasView = null;
        }

        private void Subscribe()
        {
            _playerModel.OpenTutorialStepAdded += OnOpenTutorialStepAdded;
            _playerModel.OpenTutorialStepRemoved += OnOpenTutorialStepRemoved;
        }

        private void Unsubscribe()
        {
            _playerModel.OpenTutorialStepAdded -= OnOpenTutorialStepAdded;
            _playerModel.OpenTutorialStepRemoved -= OnOpenTutorialStepRemoved;
        }

        private void InitStepMediators()
        {
            foreach (var openTutorialStep in _playerModel.OpenTutorialSteps)
            {
                MediateStep(openTutorialStep);
            }
        }

        private void OnOpenTutorialStepAdded(TutorialStep step)
        {
            MediateStep(step);
        }

        private void OnOpenTutorialStepRemoved(TutorialStep step)
        {
            UnmediateStep(step);
        }

        private void MediateStep(TutorialStep tutorialStep)
        {
            switch (tutorialStep)
            {
                case TutorialStep.HowToMove:
                    MediateStepInternal<UITutorialHowToMoveStepMediator>(tutorialStep);
                    break;
                default:
                    Debug.LogError($"tutorial step {tutorialStep} not implemented");
                    break;
            }
        }

        private void MediateStepInternal<T>(TutorialStep tutorialStep) 
            where T : UITutorialStepMediatorBase, new()
        {
            var mediator = new T();
            
            mediator.Mediate(TargetTransform, tutorialStep);
            
            AddChildMediator(mediator);
            _tutorialStepMediators.Add(tutorialStep, mediator);
        }

        private void UnmediateStep(TutorialStep step)
        {
            if (_tutorialStepMediators.TryGetValue(step, out var stepMediator))
            {
                UnmediateChild(stepMediator);
                
                _tutorialStepMediators.Remove(step);
            }
        }
    }
}