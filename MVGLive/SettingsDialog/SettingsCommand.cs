// Copyright (c) Sergei Grigorev. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Windows.Input;

namespace MVGLive
{
    /// <summary>
    /// 
    /// </summary>
    public class SettingsCommand : ICommand
    {

        private readonly Action<object> _executemethod;
        private readonly Func<object, bool> _canexecutemethod;

        /// ************************************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <param name="executemethod"></param>
        /// <param name="canexecutemethod"></param>
        public SettingsCommand(Action<object> executemethod, Func<object, bool> canexecutemethod)
        {
            _executemethod = executemethod;
            _canexecutemethod = canexecutemethod;
        }

        /// ************************************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public bool CanExecute(object parameter)
        {
            if (_canexecutemethod != null)
            {
                return _canexecutemethod(parameter);
            }
            else
            {
                return false;
            }
        }

        /// ************************************************************************************************
        /// <summary>
        /// 
        /// </summary>
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        /// ************************************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameter"></param>
        public void Execute(object parameter)
        {
            _executemethod(parameter);
        }
    }
}