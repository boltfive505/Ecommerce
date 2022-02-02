using System;
using System.Collections;
using System.ComponentModel;
using System.Windows.Data;

namespace Egate_Ecommerce.Classes
{
    public class NotifiableCollectionView : ListCollectionView
    {
        private Func<string, bool> onPropertyChangedCallback;

        public NotifiableCollectionView(IList source, object model, Func<string, bool> onPropertyChangedCallback) : this(source, model)
        {
            this.onPropertyChangedCallback = onPropertyChangedCallback;
        }

        public NotifiableCollectionView(IList source, object model) : base(source)
        {
            if (model is INotifyPropertyChanged)
                (model as INotifyPropertyChanged).PropertyChanged += NotifiableCollectionView_PropertyChanged;
        }

        private void NotifiableCollectionView_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (onPropertyChangedCallback == null) return;
            if (this.IsRefreshDeferred) return;
            if (onPropertyChangedCallback(e.PropertyName))
                this.Refresh();
        }
    }
}
