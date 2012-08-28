﻿using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using FiresecAPI.Models;
using Infrastructure.Common.Windows.ViewModels;
using Infrustructure.Plans;
using Infrustructure.Plans.Designer;
using Infrustructure.Plans.Elements;
using PlansModule.Designer;

namespace PlansModule.ViewModels
{
	public partial class PlansViewModel : ViewPartViewModel
	{
		private List<IPlanExtension<Plan>> _planExtensions;

		public void RegisterExtension(IPlanExtension<Plan> planExtension)
		{
			_planExtensions.Add(planExtension);
			planExtension.ExtensionRegistered(DesignerCanvas);
			ElementsViewModel.Update();
			if (planExtension.TabPage != null)
			{
				TabPages.Insert(planExtension.Index + 1, new TabItem()
				{
					Header = planExtension.Title,
					Content = planExtension.TabPage
				});
				OnPropertyChanged("TabPages");
			}
			if (planExtension.Instruments != null)
				foreach (IInstrument instrument in planExtension.Instruments)
					DesignerCanvas.Toolbox.Instruments.Add(instrument);
		}
		public void ElementAdded(ElementBase element)
		{
			foreach (var planExtension in _planExtensions)
				if (planExtension.ElementAdded(SelectedPlan.Plan, element))
					break;
		}
		public void ElementRemoved(ElementBase element)
		{
			foreach (var planExtension in _planExtensions)
				if (planExtension.ElementRemoved(SelectedPlan.Plan, element))
					break;
		}
		public void RegisterDesignerItem(DesignerItem designerItem)
		{
			foreach (var planExtension in _planExtensions)
				planExtension.RegisterDesignerItem(designerItem);
		}
		public IEnumerable<ElementBase> LoadPlan(Plan plan)
		{
			foreach (var planExtension in _planExtensions)
				foreach (var element in planExtension.LoadPlan(plan))
					yield return element;
		}

		private List<TabItem> _tabPages;
		public List<TabItem> TabPages
		{
			get { return _tabPages; }
			set
			{
				_tabPages = value;
				OnPropertyChanged("TabPages");
			}
		}

		private void CreatePages()
		{
			var layers = new TabItem()
				{
					Header = "Слои",
					Content = ElementsViewModel
				};
			Binding visibilityBinding = new Binding("SelectedPlan");
			visibilityBinding.Source = this;
			visibilityBinding.Converter = (IValueConverter)Application.Current.FindResource("NullToVisibilityConverter");
			layers.SetBinding(TabItem.VisibilityProperty, visibilityBinding);

			TabPages = new List<TabItem>()
			{
				new TabItem()
				{
					Header = "Планы",
					Content = PlansTreeViewModel
				},
				layers
			};
		}
	}
}