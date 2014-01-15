using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using Microsoft.Practices.Unity;
using Settings.General;

namespace Settings {
    public class SettingsViewModel : ISettingsViewModel {
        public IEnumerable<ISettingView> Views { get; set; }

        public SettingsViewModel(IUnityContainer container) {
            this.Views = new List<ISettingView> {
                container.Resolve<IGeneralView>(),
                container.Resolve<IGeneralView>(),
                container.Resolve<IGeneralView>(),
                container.Resolve<IGeneralView>(),
            };

            this.ListenToChanges();
            this.SelectAndExpandFirstView();
        }

        private void SelectAndExpandFirstView() {
            var firstView = this.Views.FirstOrDefault();

            if(firstView != null) {
                firstView.IsSelected = true;
                firstView.IsExpanded = true;
            }
        }

        private void ListenToChanges() {
            var visitedNodes = new List<ISettingView>();

            foreach(var view in this.Views) {
                VisitChildNodesWithAction(visitedNodes, view, x => x.PropertyChanged += new PropertyChangedEventHandler(this.ListenToView));
            }
        }

        private void VisitChildNodesWithAction(List<ISettingView> visitedNodes, ISettingView currentNode, Action<ISettingView> action) {
            if(visitedNodes.Count < 100) {
                visitedNodes.Add(currentNode);
                foreach(var view in currentNode.ChildViews) {
                    if(!visitedNodes.Contains(view)) {
                        action(view);
                        this.VisitChildNodesWithAction(visitedNodes, view, action);
                    }
                }
            }
        }

        public void ListenToView(object sender, PropertyChangedEventArgs args) {
            MessageBox.Show(args.PropertyName);
        }
    }
}
