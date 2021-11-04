using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Phoder1.AsyncManagement
{
    public class TaskGroup : IDisposable
    {
        private static event Action OnDisposeAll;
        private CancellationTokenSource _tokensSource = new CancellationTokenSource();
        public CancellationToken Token => _tokensSource.Token;

        bool _disposed = false;

        public TaskGroup()
        {
            OnDisposeAll += Dispose;
            Application.quitting += Application_quitting;
        }

        private static void Application_quitting()
        {
            //Must dispose all task groups! otherwise stupid async methods will keep running...
            DisposeAll();
        }
        public static void DisposeAll()
        {
            OnDisposeAll?.Invoke();
        }
        public Task StartTask(TaskFunc taskFunc, CancellationToken? cancellationToken = null)
        {
            if (taskFunc == null)
                return null;

            CancellationToken token;
            if (cancellationToken.HasValue)
            {
                cancellationToken.Value.ThrowIfCancellationRequested();
                token = Token.Combine(cancellationToken.Value);
            }
            else
            {
                token = Token;
            }

#if UNITY_EDITOR
            TaskHelper.DebugAsyncLog("Task started");
#endif
            return taskFunc.Invoke(token);
        }

        public void CancelAllTasks()
        {
            using (_tokensSource)
            {
                _tokensSource.Cancel();
            }
            _tokensSource = new CancellationTokenSource();
        }

        public void Dispose()
        {
            if (_disposed)
                return;

            Application.quitting -= Application_quitting;
            using (_tokensSource)
            {
                _tokensSource.Cancel();
            }
            _tokensSource = null;
            _disposed = true;
        }
        ~TaskGroup()
        {
            Dispose();
        }
    }
}
