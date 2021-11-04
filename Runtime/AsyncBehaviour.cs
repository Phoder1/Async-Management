using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Phoder1.AsyncManagement
{
    public abstract class AsyncBehaviour : MonoBehaviour
    {
        protected TaskGroup taskGroup = new TaskGroup();
        public CancellationToken Token => taskGroup.Token;
        public void CancelAllTasks() => taskGroup.CancelAllTasks();
        public Task StartTask(TaskFunc taskFunc, CancellationToken? cancellationToken = null)
            => taskGroup.StartTask(taskFunc, cancellationToken);
        protected virtual void OnDestroy()
        {
            taskGroup.Dispose();
        }
    }
}
