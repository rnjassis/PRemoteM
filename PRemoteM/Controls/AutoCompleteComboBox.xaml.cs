﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Shawn.Utils
{
    public partial class AutoCompleteComboBox : UserControl
    {
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(AutoCompleteComboBox),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, TextPropertyChangedCallback));

        private static void TextPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is AutoCompleteComboBox c
                && e.NewValue is string v)
            {
                TextChanged(c, v);
            }
        }

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set
            {
                if (value != Text)
                {
                    SetValue(TextProperty, value);
                    TextChanged(this, value);
                }
            }
        }

        private static void TextChanged(AutoCompleteComboBox o, string newValue)
        {
            if ((o.Selections?.Count() ?? 0) == 0)
                return;
            if (string.IsNullOrWhiteSpace(newValue))
            {
                o.CbContent.IsDropDownOpen = false;
                o.Selections4Show = new ObservableCollection<string>(o.Selections);
            }
            else
            {
                o.Selections4Show = new ObservableCollection<string>(o.Selections.Where(x => x.IndexOf(newValue, StringComparison.OrdinalIgnoreCase) >= 0));
                if (o.Selections4Show?.Count() > 0)
                {
                    o.CbContent.IsDropDownOpen = true;
                    var tb = (TextBox)o.CbContent.Template.FindName("PART_EditableTextBox", o.CbContent);
                    tb?.Select(tb.Text.Length, 0);
                }
                else
                {
                    o.Selections4Show = new ObservableCollection<string>(o.Selections);
                    o.CbContent.IsDropDownOpen = false;
                }
            }
        }

        public static readonly DependencyProperty SelectionsProperty = DependencyProperty.Register(
            nameof(Selections), typeof(IEnumerable<string>), typeof(AutoCompleteComboBox),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, SelectionsPropertyChangedCallback));

        private static void SelectionsPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is AutoCompleteComboBox cb &&
                e.NewValue is IEnumerable<string> selections)
            {
                cb.Selections4Show = new ObservableCollection<string>(selections);
            }
            if (d is AutoCompleteComboBox cb2 &&
                e.NewValue == null)
            {
                cb2.Selections4Show = new ObservableCollection<string>();
            }
        }

        public IEnumerable<string> Selections
        {
            get => (IEnumerable<string>)GetValue(SelectionsProperty);
            set
            {
                SetValue(SelectionsProperty, value);
                if (Selections != null)
                    Selections4Show = new ObservableCollection<string>(Selections);
            }
        }

        public static readonly DependencyProperty Selections4ShowProperty = DependencyProperty.Register(
            nameof(Selections4Show), typeof(IEnumerable<string>), typeof(AutoCompleteComboBox),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public IEnumerable<string> Selections4Show
        {
            get => (IEnumerable<string>)GetValue(Selections4ShowProperty);
            set => SetValue(Selections4ShowProperty, value);
        }

        public static readonly DependencyProperty IsShowArrayProperty = DependencyProperty.Register(
            nameof(IsShowArray), typeof(bool), typeof(AutoCompleteComboBox),
            new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, (d, e) =>
            {
                if (d is AutoCompleteComboBox cb &&
                    e.NewValue is bool b)
                {
                    if (b)
                    {
                        cb.CbNoArray.Visibility = Visibility.Collapsed;
                        cb.CbContent.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        cb.CbNoArray.Visibility = Visibility.Visible;
                        cb.CbContent.Visibility = Visibility.Collapsed;
                    }
                }
            }));

        public bool IsShowArray
        {
            get => (bool)GetValue(IsShowArrayProperty);
            set
            {
                SetValue(IsShowArrayProperty, value);
                if (value)
                {
                    CbNoArray.Visibility = Visibility.Collapsed;
                    CbContent.Visibility = Visibility.Visible;
                }
                else
                {
                    CbNoArray.Visibility = Visibility.Visible;
                    CbContent.Visibility = Visibility.Collapsed;
                }
            }
        }

        public AutoCompleteComboBox()
        {
            InitializeComponent();
            Grid.DataContext = this;
        }

        private void CbContent_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Tab)
            {
                if (CbContent.IsDropDownOpen && Selections4Show.Any())
                {
                    var cmbTextBox = (TextBox)CbContent.Template.FindName("PART_EditableTextBox", CbContent);
                    cmbTextBox.Text = Selections4Show.First();
                    cmbTextBox.CaretIndex = cmbTextBox.Text.Length;
                    CbContent.IsDropDownOpen = false;
                }
                e.Handled = true;
            }
        }
    }
}