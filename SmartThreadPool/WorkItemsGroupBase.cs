using System;
using System.Threading;

namespace Amib.Threading.Internal
{
    public abstract class WorkItemsGroupBase : IWorkItemsGroup
    {
        #region Private Fields

        /// <summary>
        /// Contains the name of this instance of SmartThreadPool.
        /// Can be changed by the user.
        /// </summary>
        private string _name = "WorkItemsGroupBase";

        public WorkItemsGroupBase(CancellationTokenSource parentSource)
        {
            IsIdle = true;
            WorkItemCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(parentSource.Token);
        }

        public WorkItemsGroupBase() : this(new CancellationTokenSource())
        {
            IsIdle = true;
        }

        #endregion

        #region ProtectedFields
        protected CancellationTokenSource WorkItemCancellationTokenSource { get; private set; }
        #endregion

        #region IWorkItemsGroup Members

        #region Public Methods

        /// <summary>
        /// Get/Set the name of the SmartThreadPool/WorkItemsGroup instance
        /// </summary>
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        #endregion

        #region Abstract Methods

        public abstract int Concurrency { get; set; }
        public abstract int WaitingCallbacks { get; }
        public abstract int InUseThreads { get; }

        public abstract object[] GetStates();
        public abstract WIGStartInfo WIGStartInfo { get; }
        public abstract void Start();
        public abstract void Cancel(bool abortExecution);
        public abstract bool WaitForIdle(int millisecondsTimeout);
        public abstract event WorkItemsGroupIdleHandler OnIdle;

        internal abstract void Enqueue(WorkItem workItem);
        internal virtual void PreQueueWorkItem() { }

        #endregion

        #region Common Base Methods

        /// <summary>
        /// Cancel all the work items.
        /// Same as Cancel(false)
        /// </summary>
        public virtual void Cancel()
        {
            Cancel(false);
        }

        /// <summary>
        /// Wait for the SmartThreadPool/WorkItemsGroup to be idle
        /// </summary>
        public void WaitForIdle()
        {
            WaitForIdle(Timeout.Infinite);
        }

        /// <summary>
        /// Wait for the SmartThreadPool/WorkItemsGroup to be idle
        /// </summary>
        public bool WaitForIdle(TimeSpan timeout)
        {
            return WaitForIdle((int)timeout.TotalMilliseconds);
        }

        /// <summary>
        /// IsIdle is true when there are no work items running or queued.
        /// </summary>
        public bool IsIdle { get; protected set; }

        #endregion

        #region QueueWorkItem

        /// <summary>
        /// Queue a work item
        /// </summary>
        /// <param name="callback">A callback to execute</param>
        /// <returns>Returns a work item result</returns>
        public IWorkItemResult QueueWorkItem(WorkItemCallback callback)
        {
            WorkItem workItem = WorkItemFactory.CreateWorkItem(this, WIGStartInfo, callback, WorkItemCancellationTokenSource.Token);
            Enqueue(workItem);
            return workItem.GetWorkItemResult();
        }

        /// <summary>
        /// Queue a work item
        /// </summary>
        /// <param name="callback">A callback to execute</param>
        /// <param name="workItemPriority">The priority of the work item</param>
        /// <returns>Returns a work item result</returns>
        public IWorkItemResult QueueWorkItem(WorkItemCallback callback, WorkItemPriority workItemPriority)
        {
            PreQueueWorkItem();
            WorkItem workItem = WorkItemFactory.CreateWorkItem(this, WIGStartInfo, callback, workItemPriority, WorkItemCancellationTokenSource.Token);
            Enqueue(workItem);
            return workItem.GetWorkItemResult();
        }

        /// <summary>
        /// Queue a work item
        /// </summary>
        /// <param name="workItemInfo">Work item info</param>
        /// <param name="callback">A callback to execute</param>
        /// <returns>Returns a work item result</returns>
        public IWorkItemResult QueueWorkItem(WorkItemInfo workItemInfo, WorkItemCallback callback)
        {
            PreQueueWorkItem();
            WorkItem workItem = WorkItemFactory.CreateWorkItem(this, WIGStartInfo, workItemInfo, callback, WorkItemCancellationTokenSource.Token);
            Enqueue(workItem);
            return workItem.GetWorkItemResult();
        }

        /// <summary>
        /// Queue a work item
        /// </summary>
        /// <param name="callback">A callback to execute</param>
        /// <param name="state">
        /// The context object of the work item. Used for passing arguments to the work item. 
        /// </param>
        /// <returns>Returns a work item result</returns>
        public IWorkItemResult QueueWorkItem(WorkItemCallback callback, object state)
        {
            WorkItem workItem = WorkItemFactory.CreateWorkItem(this, WIGStartInfo, callback, state, WorkItemCancellationTokenSource.Token);
            Enqueue(workItem);
            return workItem.GetWorkItemResult();
        }

        /// <summary>
        /// Queue a work item
        /// </summary>
        /// <param name="callback">A callback to execute</param>
        /// <param name="state">
        /// The context object of the work item. Used for passing arguments to the work item. 
        /// </param>
        /// <param name="workItemPriority">The work item priority</param>
        /// <returns>Returns a work item result</returns>
        public IWorkItemResult QueueWorkItem(WorkItemCallback callback, object state, WorkItemPriority workItemPriority)
        {
            PreQueueWorkItem();
            WorkItem workItem = WorkItemFactory.CreateWorkItem(this, WIGStartInfo, callback, state, workItemPriority, WorkItemCancellationTokenSource.Token);
            Enqueue(workItem);
            return workItem.GetWorkItemResult();
        }

        /// <summary>
        /// Queue a work item
        /// </summary>
        /// <param name="workItemInfo">Work item information</param>
        /// <param name="callback">A callback to execute</param>
        /// <param name="state">
        /// The context object of the work item. Used for passing arguments to the work item. 
        /// </param>
        /// <returns>Returns a work item result</returns>
        public IWorkItemResult QueueWorkItem(WorkItemInfo workItemInfo, WorkItemCallback callback, object state)
        {
            PreQueueWorkItem();
            WorkItem workItem = WorkItemFactory.CreateWorkItem(this, WIGStartInfo, workItemInfo, callback, state, WorkItemCancellationTokenSource.Token);
            Enqueue(workItem);
            return workItem.GetWorkItemResult();
        }

        /// <summary>
        /// Queue a work item
        /// </summary>
        /// <param name="callback">A callback to execute</param>
        /// <param name="state">
        /// The context object of the work item. Used for passing arguments to the work item. 
        /// </param>
        /// <param name="postExecuteWorkItemCallback">
        /// A delegate to call after the callback completion
        /// </param>
        /// <returns>Returns a work item result</returns>
        public IWorkItemResult QueueWorkItem(
            WorkItemCallback callback,
            object state,
            PostExecuteWorkItemCallback postExecuteWorkItemCallback)
        {
            PreQueueWorkItem();
            WorkItem workItem = WorkItemFactory.CreateWorkItem(this, WIGStartInfo, callback, state, postExecuteWorkItemCallback, WorkItemCancellationTokenSource.Token);
            Enqueue(workItem);
            return workItem.GetWorkItemResult();
        }

        /// <summary>
        /// Queue a work item
        /// </summary>
        /// <param name="callback">A callback to execute</param>
        /// <param name="state">
        /// The context object of the work item. Used for passing arguments to the work item. 
        /// </param>
        /// <param name="postExecuteWorkItemCallback">
        /// A delegate to call after the callback completion
        /// </param>
        /// <param name="workItemPriority">The work item priority</param>
        /// <returns>Returns a work item result</returns>
        public IWorkItemResult QueueWorkItem(
            WorkItemCallback callback,
            object state,
            PostExecuteWorkItemCallback postExecuteWorkItemCallback,
            WorkItemPriority workItemPriority)
        {
            PreQueueWorkItem();
            WorkItem workItem = WorkItemFactory.CreateWorkItem(this, WIGStartInfo, callback, state, postExecuteWorkItemCallback, workItemPriority, WorkItemCancellationTokenSource.Token);
            Enqueue(workItem);
            return workItem.GetWorkItemResult();
        }

        /// <summary>
        /// Queue a work item
        /// </summary>
        /// <param name="callback">A callback to execute</param>
        /// <param name="state">
        /// The context object of the work item. Used for passing arguments to the work item. 
        /// </param>
        /// <param name="postExecuteWorkItemCallback">
        /// A delegate to call after the callback completion
        /// </param>
        /// <param name="callToPostExecute">Indicates on which cases to call to the post execute callback</param>
        /// <returns>Returns a work item result</returns>
        public IWorkItemResult QueueWorkItem(
            WorkItemCallback callback,
            object state,
            PostExecuteWorkItemCallback postExecuteWorkItemCallback,
            CallToPostExecute callToPostExecute)
        {
            PreQueueWorkItem();
            WorkItem workItem = WorkItemFactory.CreateWorkItem(this, WIGStartInfo, callback, state, postExecuteWorkItemCallback, callToPostExecute, WorkItemCancellationTokenSource.Token);
            Enqueue(workItem);
            return workItem.GetWorkItemResult();
        }

        /// <summary>
        /// Queue a work item
        /// </summary>
        /// <param name="callback">A callback to execute</param>
        /// <param name="state">
        /// The context object of the work item. Used for passing arguments to the work item. 
        /// </param>
        /// <param name="postExecuteWorkItemCallback">
        /// A delegate to call after the callback completion
        /// </param>
        /// <param name="callToPostExecute">Indicates on which cases to call to the post execute callback</param>
        /// <param name="workItemPriority">The work item priority</param>
        /// <returns>Returns a work item result</returns>
        public IWorkItemResult QueueWorkItem(
            WorkItemCallback callback,
            object state,
            PostExecuteWorkItemCallback postExecuteWorkItemCallback,
            CallToPostExecute callToPostExecute,
            WorkItemPriority workItemPriority)
        {
            PreQueueWorkItem();
            WorkItem workItem = WorkItemFactory.CreateWorkItem(this, WIGStartInfo, callback, state, postExecuteWorkItemCallback, callToPostExecute, workItemPriority, WorkItemCancellationTokenSource.Token);
            Enqueue(workItem);
            return workItem.GetWorkItemResult();
        }

        #endregion

        #region QueueWorkItem(Action<...>)

        public IWorkItemResult QueueWorkItem(Action<CancellationToken> action, WorkItemPriority priority = SmartThreadPool.DefaultWorkItemPriority)
        {
            PreQueueWorkItem ();
            WorkItem workItem = WorkItemFactory.CreateWorkItem (
                this,
                WIGStartInfo,
                (state, cancellationToken) => 
                {
                    action.Invoke (cancellationToken);
                    return null;
                }, priority, WorkItemCancellationTokenSource.Token);
            Enqueue (workItem);
            return workItem.GetWorkItemResult ();
        }

        public IWorkItemResult QueueWorkItem<T>(Action<T, CancellationToken> action, T arg, WorkItemPriority priority = SmartThreadPool.DefaultWorkItemPriority)
        {
            PreQueueWorkItem ();
            WorkItem workItem = WorkItemFactory.CreateWorkItem (
                this,
                WIGStartInfo,
                (state, cancellationToken) =>
                {
                    action.Invoke (arg, cancellationToken);
                    return null;
                },
                WIGStartInfo.FillStateWithArgs ? new object[] { arg } : null, priority, WorkItemCancellationTokenSource.Token);
            Enqueue (workItem);
            return workItem.GetWorkItemResult ();
        }

        public IWorkItemResult QueueWorkItem<T1, T2>(Action<T1, T2, CancellationToken> action, T1 arg1, T2 arg2, WorkItemPriority priority = SmartThreadPool.DefaultWorkItemPriority)
        {
            PreQueueWorkItem ();
            WorkItem workItem = WorkItemFactory.CreateWorkItem (
                this,
                WIGStartInfo,
                (state, cancellationToken) =>
                {
                    action.Invoke (arg1, arg2, cancellationToken);
                    return null;
                },
                WIGStartInfo.FillStateWithArgs ? new object[] { arg1, arg2 } : null, priority, WorkItemCancellationTokenSource.Token);
            Enqueue (workItem);
            return workItem.GetWorkItemResult ();
        }

        public IWorkItemResult QueueWorkItem<T1, T2, T3>(Action<T1, T2, T3, CancellationToken> action, T1 arg1, T2 arg2, T3 arg3, WorkItemPriority priority = SmartThreadPool.DefaultWorkItemPriority)
        {
            PreQueueWorkItem ();
            WorkItem workItem = WorkItemFactory.CreateWorkItem (
                this,
                WIGStartInfo,
                (state, cancellationToken) =>
                {
                    action.Invoke (arg1, arg2, arg3, cancellationToken);
                    return null;
                },
                WIGStartInfo.FillStateWithArgs ? new object[] { arg1, arg2, arg3 } : null, priority, WorkItemCancellationTokenSource.Token);
            Enqueue (workItem);
            return workItem.GetWorkItemResult ();
        }

        public IWorkItemResult QueueWorkItem<T1, T2, T3, T4> (
            Action<T1, T2, T3, T4, CancellationToken> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, WorkItemPriority priority = SmartThreadPool.DefaultWorkItemPriority)
        {
            PreQueueWorkItem ();
            WorkItem workItem = WorkItemFactory.CreateWorkItem (
                this,
                WIGStartInfo,
                (state, cancellationToken) =>
                {
                    action.Invoke (arg1, arg2, arg3, arg4, cancellationToken);
                    return null;
                },
                WIGStartInfo.FillStateWithArgs ? new object[] { arg1, arg2, arg3, arg4 } : null, priority, WorkItemCancellationTokenSource.Token);
            Enqueue (workItem);
            return workItem.GetWorkItemResult ();
        }

        #endregion

        #region QueueWorkItem(Func<...>)

        public IWorkItemResult<TResult> QueueWorkItem<TResult>(Func<CancellationToken, TResult> func, WorkItemPriority priority = SmartThreadPool.DefaultWorkItemPriority)
        {
            PreQueueWorkItem();
            WorkItem workItem = WorkItemFactory.CreateWorkItem(
                this,
                WIGStartInfo,
                (state, cancellationToken) =>
                {
                    return func.Invoke(cancellationToken);
                }, priority, WorkItemCancellationTokenSource.Token);
            Enqueue(workItem);
            return new WorkItemResultTWrapper<TResult>(workItem.GetWorkItemResult());
        }

        public IWorkItemResult<TResult> QueueWorkItem<T, TResult>(Func<T, CancellationToken, TResult> func, T arg, WorkItemPriority priority = SmartThreadPool.DefaultWorkItemPriority)
        {
            PreQueueWorkItem();
            WorkItem workItem = WorkItemFactory.CreateWorkItem(
                this,
                WIGStartInfo,
                (state, cancellationToken) =>
                {
                    return func.Invoke(arg, cancellationToken);
                },
                WIGStartInfo.FillStateWithArgs ? new object[] { arg } : null,
                priority, WorkItemCancellationTokenSource.Token);
            Enqueue(workItem);
            return new WorkItemResultTWrapper<TResult>(workItem.GetWorkItemResult());
        }

        public IWorkItemResult<TResult> QueueWorkItem<T1, T2, TResult>(Func<T1, T2, CancellationToken, TResult> func, T1 arg1, T2 arg2, WorkItemPriority priority = SmartThreadPool.DefaultWorkItemPriority)
        {
            PreQueueWorkItem();
            WorkItem workItem = WorkItemFactory.CreateWorkItem(
                this,
                WIGStartInfo,
                (state, cancellationToken) =>
                {
                    return func.Invoke(arg1, arg2, cancellationToken);
                },
                WIGStartInfo.FillStateWithArgs ? new object[] { arg1, arg2 } : null,
                priority, WorkItemCancellationTokenSource.Token);
            Enqueue(workItem);
            return new WorkItemResultTWrapper<TResult>(workItem.GetWorkItemResult());
        }

        public IWorkItemResult<TResult> QueueWorkItem<T1, T2, T3, TResult>(
            Func<T1, T2, T3, CancellationToken, TResult> func, T1 arg1, T2 arg2, T3 arg3, WorkItemPriority priority = SmartThreadPool.DefaultWorkItemPriority)
        {
            PreQueueWorkItem();
            WorkItem workItem = WorkItemFactory.CreateWorkItem(
                this,
                WIGStartInfo,
                (state, cancellationToken) =>
                {
                    return func.Invoke(arg1, arg2, arg3, cancellationToken);
                },
                WIGStartInfo.FillStateWithArgs ? new object[] { arg1, arg2, arg3 } : null,
                priority, WorkItemCancellationTokenSource.Token);
            Enqueue(workItem);
            return new WorkItemResultTWrapper<TResult>(workItem.GetWorkItemResult());
        }

        public IWorkItemResult<TResult> QueueWorkItem<T1, T2, T3, T4, TResult>(
            Func<T1, T2, T3, T4, CancellationToken, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, WorkItemPriority priority = SmartThreadPool.DefaultWorkItemPriority)
        {
            PreQueueWorkItem();
            WorkItem workItem = WorkItemFactory.CreateWorkItem(
                this,
                WIGStartInfo,
                (state, cancellationToken) =>
                {
                    return func.Invoke(arg1, arg2, arg3, arg4, cancellationToken);
                },
                WIGStartInfo.FillStateWithArgs ? new object[] { arg1, arg2, arg3, arg4 } : null,
                priority, WorkItemCancellationTokenSource.Token);
            Enqueue(workItem);
            return new WorkItemResultTWrapper<TResult>(workItem.GetWorkItemResult());
        }

        #endregion

        #endregion
    }
}