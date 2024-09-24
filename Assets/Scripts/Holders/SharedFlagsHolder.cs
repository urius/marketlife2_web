using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Data;

namespace Holders
{
    public class SharedFlagsHolder
    {
        public static readonly SharedFlagsHolder Instance = new SharedFlagsHolder();
        
        private readonly Dictionary<SharedFlagKey, bool> _sharedFlagValues = new();
        private readonly Dictionary<SharedFlagKey, Action<bool>> _flagUpdateOnceActions = new();
        
        public bool Get(SharedFlagKey flagKey)
        {
            return _sharedFlagValues.TryGetValue(flagKey, out var value) ? value : default;
        }
        
        public void Set(SharedFlagKey flagKey, bool flagValue)
        {
            if (_sharedFlagValues.TryGetValue(flagKey, out var value)
                && value == flagValue)
            {
                return;
            }

            _sharedFlagValues[flagKey] = flagValue;

            if (_flagUpdateOnceActions.TryGetValue(flagKey, out var action))
            {
                action?.Invoke(flagValue);
                
                _flagUpdateOnceActions.Remove(flagKey);
            }
        }

        public void SubscribeOnce(SharedFlagKey flagKey, Action<bool> newAction)
        {
            Action<bool> actionsChain;
            
            if (_flagUpdateOnceActions.TryGetValue(flagKey, out var existingAction))
            {
                actionsChain = existingAction + newAction;
            }
            else
            {
                actionsChain = newAction;
            }

            _flagUpdateOnceActions[flagKey] = actionsChain;
        }
        
        public void UnsubscribeOnce(SharedFlagKey flagKey, Action<bool> newAction)
        {
            if (_flagUpdateOnceActions.TryGetValue(flagKey, out var existingAction))
            {
                _flagUpdateOnceActions[flagKey] = existingAction - newAction;
            }
        }
        
        public UniTask WaitForFlagValue(SharedFlagKey flagKey, bool expectedValue)
        {
            return WaitForFlagValue(flagKey, expectedValue, CancellationToken.None);
        }

        public async UniTask WaitForFlagValue(SharedFlagKey flagKey, bool expectedValue, CancellationToken token)
        {
            if (Get(flagKey) == expectedValue)
            {
                return;
            }
            
            var tcs = new UniTaskCompletionSource();

            var tokenRegistration = token.Register(OnCancel);
            SubscribeOnce(flagKey, OnFlagUpdate);

            await tcs.Task;
            
            UnsubscribeOnce(flagKey, OnFlagUpdate);
            
            await tokenRegistration.DisposeAsync();
            
            return;

            void OnFlagUpdate(bool value)
            {
                tcs.TrySetResult();
            }

            void OnCancel()
            {
                tcs.TrySetCanceled();
            }
        }
    }
}