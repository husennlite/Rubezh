﻿using System.Windows.Media;
using FiresecAPI.Models;
using Infrastructure.Common.Windows.ViewModels;

namespace Infrastructure.Designer.ElementProperties.ViewModels
{
	public class RectanglePropertiesViewModel : SaveCancelDialogViewModel
	{
		ElementRectangle _elementRectangle;
		public ImagePropertiesViewModel ImagePropertiesViewModel { get; private set; }

		public RectanglePropertiesViewModel(ElementRectangle elementRectangle)
		{
			Title = "Свойства фигуры: Прямоугольник";
			_elementRectangle = elementRectangle;
			ImagePropertiesViewModel = new ImagePropertiesViewModel(_elementRectangle);
			CopyProperties();
		}

		void CopyProperties()
		{
			BackgroundColor = _elementRectangle.BackgroundColor;
			BorderColor = _elementRectangle.BorderColor;
			StrokeThickness = _elementRectangle.BorderThickness;
		}

		Color _backgroundColor;
		public Color BackgroundColor
		{
			get { return _backgroundColor; }
			set
			{
				_backgroundColor = value;
				OnPropertyChanged("BackgroundColor");
			}
		}

		Color _borderColor;
		public Color BorderColor
		{
			get { return _borderColor; }
			set
			{
				_borderColor = value;
				OnPropertyChanged("BorderColor");
			}
		}

		double _strokeThickness;
		public double StrokeThickness
		{
			get { return _strokeThickness; }
			set
			{
				_strokeThickness = value;
				OnPropertyChanged("StrokeThickness");
			}
		}

		protected override bool Save()
		{
			_elementRectangle.BackgroundColor = BackgroundColor;
			_elementRectangle.BorderColor = BorderColor;
			_elementRectangle.BorderThickness = StrokeThickness;
			ImagePropertiesViewModel.Save();
			return base.Save();
		}
	}
}