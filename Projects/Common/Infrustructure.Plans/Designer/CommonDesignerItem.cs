﻿using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using Infrustructure.Plans.Elements;
using Infrustructure.Plans.Events;
using Infrustructure.Plans.Painters;

namespace Infrustructure.Plans.Designer
{
	public abstract class CommonDesignerItem : ContentControl, INotifyPropertyChanged
	{
		#region INotifyPropertyChanged Members

		public event PropertyChangedEventHandler PropertyChanged;

		#endregion

		public event EventHandler DesignerItemPropertyChanged;

		public static readonly DependencyProperty IsSelectedProperty = DependencyProperty.Register("IsSelected", typeof(bool), typeof(DesignerItem), new FrameworkPropertyMetadata(false));
		public virtual bool IsSelected
		{
			get { return (bool)GetValue(IsSelectedProperty); }
			set
			{
				SetValue(IsSelectedProperty, value);
				if (value)
					EventService.EventAggregator.GetEvent<ElementSelectedEvent>().Publish(Element);
			}
		}

		public static readonly DependencyProperty IsSelectableProperty = DependencyProperty.Register("IsSelectable", typeof(bool), typeof(DesignerItem), new FrameworkPropertyMetadata(false));
		public virtual bool IsSelectable
		{
			get { return (bool)GetValue(IsSelectableProperty); }
			set { SetValue(IsSelectableProperty, value); }
		}

		private bool _isVisibleLayout;
		public virtual bool IsVisibleLayout
		{
			get { return _isVisibleLayout; }
			set
			{
				_isVisibleLayout = value;
				Visibility = value ? Visibility.Visible : Visibility.Collapsed;
				if (!value)
					IsSelected = false;
			}
		}

		private bool _isSelectableLayout;
		public virtual bool IsSelectableLayout
		{
			get { return _isSelectableLayout; }
			set
			{
				_isSelectableLayout = value;
				IsSelectable = value;
				if (!value)
					IsSelected = false;
			}
		}

		public ElementBase Element { get; protected set; }
		public IPainter Painter { get; private set; }

		public event Action<CommonDesignerItem> UpdateProperties;

		private string _title;
		public string Title
		{
			get { return _title; }
			set
			{
				_title = value;
				OnPropertyChanged("Title");
			}
		}

		public CommonDesignerItem(ElementBase element)
		{
			ResetElement(element);
		}

		public virtual void ResetElement(ElementBase element)
		{
			Element = element;
			DataContext = Element;
			Painter = PainterFactory.Create(Element);
		}

		public virtual void SetLocation()
		{
			var rect = Element.GetRectangle();
			Canvas.SetLeft(this, rect.Left);
			Canvas.SetTop(this, rect.Top);
			ItemWidth = rect.Width;
			ItemHeight = rect.Height;
		}
		public virtual void Redraw()
		{
			Content = Painter == null ? null : Painter.Draw(Element);
			MinWidth = Element.BorderThickness;
			MinHeight = Element.BorderThickness;
			if (Element is ElementBaseShape)
			{
				MinWidth += 3;
				MinHeight += 3;
			}
			SetLocation();
		}
		public void SetZIndex()
		{
			int bigConstatnt = 100000;

			if (Element is IElementZIndex)
				Panel.SetZIndex(this, ((IElementZIndex)Element).ZIndex);

			if (Element is IElementZLayer)
				Panel.SetZIndex(this, ((IElementZLayer)Element).ZLayerIndex * bigConstatnt);
		}

		public virtual double ItemWidth
		{
			get { return Width - Element.BorderThickness; }
			set { Width = value + Element.BorderThickness; }
		}
		public virtual double ItemHeight
		{
			get { return Height - Element.BorderThickness; }
			set { Height = value + Element.BorderThickness; }
		}

		public virtual void UpdateElementProperties()
		{
			OnUpdateProperties();
		}
		protected void OnUpdateProperties()
		{
			if (UpdateProperties != null)
				UpdateProperties(this);
		}

		protected abstract void CreateContextMenu();

		protected void OnPropertyChanged(string propertyName)
		{
			if (PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}
		protected void OnDesignerItemPropertyChanged()
		{
			if (DesignerItemPropertyChanged != null)
				DesignerItemPropertyChanged(this, EventArgs.Empty);
		}
	}
}