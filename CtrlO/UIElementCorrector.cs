﻿using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CtrlO
{
    /// <summary>
    /// Class UIElementCorrector
    /// </summary>
    public class UIElementCorrector
    {

        /// <summary>
        /// The double click command property
        /// </summary>
        public static readonly DependencyProperty DoubleClickCommandProperty =
            DependencyProperty.RegisterAttached("DoubleClickCommand", typeof (ICommand), typeof (UIElementCorrector),
                                                new FrameworkPropertyMetadata(null, OnDoubleClickCommandPropertyChanged));

        /// <summary>
        /// The double click command parameter property
        /// </summary>
        public static readonly DependencyProperty DoubleClickCommandParameterProperty =
            DependencyProperty.RegisterAttached("DoubleClickCommandParameter", typeof (object),
                                                typeof (UIElementCorrector), new FrameworkPropertyMetadata(null));


        /// <summary>
        /// Gets the double click command.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns>ICommand.</returns>
        public static ICommand GetDoubleClickCommand(DependencyObject obj)
        {
            return (ICommand) obj.GetValue(DoubleClickCommandProperty);
        }

        /// <summary>
        /// Sets the double click command.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="value">The value.</param>
        public static void SetDoubleClickCommand(DependencyObject obj, ICommand value)
        {
            obj.SetValue(DoubleClickCommandProperty, value);
        }

        /// <summary>
        /// Called when [double click command property changed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs" /> instance containing the event data.</param>
        /// <exception cref="System.ArgumentException">The dependency property can only be attached to a Control;sender</exception>
        public static void OnDoubleClickCommandPropertyChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var control = sender as Control;
            if (control == null)
                throw new ArgumentException("The dependency property can only be attached to a Control", "sender");

            var command = e.NewValue as ICommand;

            if (command != null)
            {
                control.MouseDoubleClick += HandleDoubleClick;
            }
            else
            {
                control.MouseDoubleClick -= HandleDoubleClick;
            }
        }

        /// <summary>
        /// Handles the double click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="MouseButtonEventArgs" /> instance containing the event data.</param>
        private static void HandleDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var control = (DependencyObject) sender;
            GetDoubleClickCommand(control).Execute(GetDoubleClickCommandParameter(control));
        }

        /// <summary>
        /// Gets the double click command parameter.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns>System.Object.</returns>
        public static object GetDoubleClickCommandParameter(DependencyObject obj)
        {
            return obj.GetValue(DoubleClickCommandParameterProperty);
        }

        /// <summary>
        /// Sets the double click command parameter.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="value">The value.</param>
        public static void SetDoubleClickCommandParameter(DependencyObject obj, object value)
        {
            obj.SetValue(DoubleClickCommandParameterProperty, value);
        }
    }
}