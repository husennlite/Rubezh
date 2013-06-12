﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common;
using System.Collections.Specialized;
using System.Collections;

namespace Infrastructure.Common.TreeList
{
	public class RootTreeNodeViewModel : TreeNodeViewModel, ITreeList
	{
		private List<ITreeList> _treeLists;
		private ObservableCollectionAdv<TreeNodeViewModel> _rows;

		public RootTreeNodeViewModel()
		{
			_treeLists = new List<ITreeList>();
			_rows = new ObservableCollectionAdv<TreeNodeViewModel>();
			IsExpanded = true;
			IsRoot = true;
			AssignToRoot(this);
		}

		public void AssignToTree(ITreeList treeList)
		{
			_treeLists.Add(treeList);
		}
		public void DisassignFromTree(ITreeList treeList)
		{
			_treeLists.Remove(treeList);
		}
		public void SetSource(IEnumerable source)
		{
			NotifyCollection = source as INotifyCollectionChanged;
			Rows.Clear();
			Nodes.Clear();
			if (source != null)
				foreach (TreeNodeViewModel item in source)
				{
					item.AssignToRoot(this);
					Nodes.Add(item);
				}
		}

		private INotifyCollectionChanged _notifyCollection;
		public INotifyCollectionChanged NotifyCollection
		{
			get { return _notifyCollection; }
			set
			{
				if (NotifyCollection != null)
					NotifyCollection.CollectionChanged -= ChildrenChanged;
				_notifyCollection = value;
				if (NotifyCollection != null)
					NotifyCollection.CollectionChanged += ChildrenChanged;
			}
		}

		private void ChildrenChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			switch (e.Action)
			{
				case NotifyCollectionChangedAction.Add:
					if (e.NewItems != null)
					{
						int index = e.NewStartingIndex;
						for (int i = 0; i < e.NewItems.Count; i++)
							Nodes.Insert(index + i, (TreeNodeViewModel)e.NewItems[i]);
					}
					break;
				case NotifyCollectionChangedAction.Remove:
					if (e.OldItems != null)
					{
						int index = e.OldStartingIndex;
						for (int i = 0; i < e.OldItems.Count; i++)
							Nodes.RemoveAt(index + i);
					}
					break;
				case NotifyCollectionChangedAction.Move:
				case NotifyCollectionChangedAction.Replace:
				case NotifyCollectionChangedAction.Reset:
					var items = Nodes.ToArray();
					Nodes.Clear();
					foreach (var item in items)
						Nodes.InsertRange(0, items);
					break;
			}
		}

		#region ITreeList Members

		public ObservableCollectionAdv<TreeNodeViewModel> Rows
		{
			get { return _rows; }
		}
		public object SelectedTreeNode
		{
			get { return _treeLists.Count > 0 ? _treeLists[0].SelectedTreeNode : null; }
			set { _treeLists.ForEach(item => item.SelectedTreeNode = value); }
		}

		public void SuspendSelection()
		{
			_treeLists.ForEach(item => item.SuspendSelection());
		}
		public void ResumeSelection()
		{
			_treeLists.ForEach(item => item.ResumeSelection());
		}

		#endregion
	}
}