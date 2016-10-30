using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using CtrlO.Properties;

namespace CtrlO.Mvvm.Commands
{
    /// <summary>
    ///     An <see cref="ICommand" /> whose delegates can be attached for <see cref="Execute" /> and <see cref="CanExecute" />
    ///     .
    /// </summary>
    public abstract class DelegateCommandBase : ICommand, IActiveAware
    {
        private readonly bool _isAutomaticRequeryDisabled;
        protected readonly Func<object, bool> CanExecuteMethod;
        protected readonly Func<object, Task> ExecuteMethod;
        private List<WeakReference> _canExecuteChangedHandlers;
        private bool _isActive;

        /// <summary>
        ///     Creates a new instance of a <see cref="DelegateCommandBase" />, specifying both the execute action and the can
        ///     execute function.
        /// </summary>
        /// <param name="executeMethod">The <see cref="Action" /> to execute when <see cref="ICommand.Execute" /> is invoked.</param>
        /// <param name="canExecuteMethod">
        ///     The <see cref="Func{Object,Bool}" /> to invoked when <see cref="ICommand.CanExecute" />
        ///     is invoked.
        /// </param>
        /// <param name="isAutomaticRequeryDisabled"></param>
        protected DelegateCommandBase(Action<object> executeMethod, Func<object, bool> canExecuteMethod,
            bool isAutomaticRequeryDisabled = false)
        {
            if (executeMethod == null || canExecuteMethod == null)
                throw new ArgumentNullException(nameof(executeMethod), Resources.DelegateCommandDelegatesCannotBeNull);

            ExecuteMethod = arg =>
            {
                executeMethod(arg);
                return Task.Delay(0);
            };
            CanExecuteMethod = canExecuteMethod;
            _isAutomaticRequeryDisabled = isAutomaticRequeryDisabled;
        }

        /// <summary>
        ///     Creates a new instance of a <see cref="DelegateCommandBase" />, specifying both the Execute action as an awaitable
        ///     Task and the CanExecute function.
        /// </summary>
        /// <param name="executeMethod">
        ///     The <see cref="Func{Object,Task}" /> to execute when <see cref="ICommand.Execute" /> is
        ///     invoked.
        /// </param>
        /// <param name="canExecuteMethod">
        ///     The <see cref="Func{Object,Bool}" /> to invoked when <see cref="ICommand.CanExecute" />
        ///     is invoked.
        /// </param>
        /// <param name="isAutomaticRequeryDisabled"></param>
        protected DelegateCommandBase(Func<object, Task> executeMethod, Func<object, bool> canExecuteMethod,
            bool isAutomaticRequeryDisabled = false)
        {
            if (executeMethod == null || canExecuteMethod == null)
                throw new ArgumentNullException(nameof(executeMethod), Resources.DelegateCommandDelegatesCannotBeNull);

            ExecuteMethod = executeMethod;
            CanExecuteMethod = canExecuteMethod;
            _isAutomaticRequeryDisabled = isAutomaticRequeryDisabled;
        }

        async void ICommand.Execute(object parameter)
        {
            await Execute(parameter);
        }

        bool ICommand.CanExecute(object parameter)
        {
            return CanExecute(parameter);
        }

        /// <summary>
        ///     Occurs when changes occur that affect whether or not the command should execute. You must keep a hard
        ///     reference to the handler to avoid garbage collection and unexpected results. See remarks for more information.
        /// </summary>
        /// <remarks>
        ///     When subscribing to the <see cref="ICommand.CanExecuteChanged" /> event using
        ///     code (not when binding using XAML) will need to keep a hard reference to the event handler. This is to prevent
        ///     garbage collection of the event handler because the command implements the Weak Event pattern so it does not have
        ///     a hard reference to this handler. An example implementation can be seen in the CompositeCommand and
        ///     CommandBehaviorBase
        ///     classes. In most scenarios, there is no reason to sign up to the CanExecuteChanged event directly, but if you do,
        ///     you
        ///     are responsible for maintaining the reference.
        /// </remarks>
        /// <example>
        ///     The following code holds a reference to the event handler. The myEventHandlerReference value should be stored
        ///     in an instance member to avoid it from being garbage collected.
        ///     <code>
        /// EventHandler myEventHandlerReference = new EventHandler(this.OnCanExecuteChanged);
        /// command.CanExecuteChanged += myEventHandlerReference;
        /// </code>
        /// </example>
        public virtual event EventHandler CanExecuteChanged
        {
            add
            {
                if (!_isAutomaticRequeryDisabled)
                {
                    CommandManager.RequerySuggested += value;
                }
                WeakEventHandlerManager.AddWeakReferenceHandler(ref _canExecuteChangedHandlers, value, 2);
            }
            remove
            {
                if (!_isAutomaticRequeryDisabled)
                {
                    CommandManager.RequerySuggested -= value;
                }
                WeakEventHandlerManager.RemoveWeakReferenceHandler(_canExecuteChangedHandlers, value);
            }
        }

        /// <summary>
        ///     Raises <see cref="ICommand.CanExecuteChanged" /> on the UI thread so every
        ///     command invoker can requery <see cref="ICommand.CanExecute" />.
        /// </summary>
        protected virtual void OnCanExecuteChanged()
        {
            WeakEventHandlerManager.CallWeakReferenceHandlers(this, _canExecuteChangedHandlers);
        }

        /// <summary>
        ///     Raises <see cref="DelegateCommandBase.CanExecuteChanged" /> on the UI thread so every command invoker
        ///     can requery to check if the command can execute.
        ///     <remarks>
        ///         Note that this will trigger the execution of <see cref="DelegateCommandBase.CanExecute" /> once for each
        ///         invoker.
        ///     </remarks>
        /// </summary>
        public void RaiseCanExecuteChanged()
        {
            OnCanExecuteChanged();
        }

        /// <summary>
        ///     Executes the command with the provided parameter by invoking the <see cref="Action{Object}" /> supplied during
        ///     construction.
        /// </summary>
        /// <param name="parameter"></param>
        protected async Task Execute(object parameter)
        {
            await ExecuteMethod(parameter);
        }

        /// <summary>
        ///     Determines if the command can execute with the provided parameter by invoking the <see cref="Func{Object,Bool}" />
        ///     supplied during construction.
        /// </summary>
        /// <param name="parameter">The parameter to use when determining if this command can execute.</param>
        /// <returns>Returns <see langword="true" /> if the command can execute.  <see langword="False" /> otherwise.</returns>
        protected bool CanExecute(object parameter)
        {
            return CanExecuteMethod == null || CanExecuteMethod(parameter);
        }

        #region IsActive

        /// <summary>
        ///     Gets or sets a value indicating whether the object is active.
        /// </summary>
        /// <value><see langword="true" /> if the object is active; otherwise <see langword="false" />.</value>
        public bool IsActive
        {
            get { return _isActive; }
            set
            {
                if (_isActive != value)
                {
                    _isActive = value;
                    OnIsActiveChanged();
                }
            }
        }

        /// <summary>
        ///     Fired if the <see cref="IsActive" /> property changes.
        /// </summary>
        public virtual event EventHandler IsActiveChanged;

        /// <summary>
        ///     This raises the <see cref="DelegateCommandBase.IsActiveChanged" /> event.
        /// </summary>
        protected virtual void OnIsActiveChanged()
        {
            var isActiveChangedHandler = IsActiveChanged;
            isActiveChangedHandler?.Invoke(this, EventArgs.Empty);
        }

        #endregion
    }
}