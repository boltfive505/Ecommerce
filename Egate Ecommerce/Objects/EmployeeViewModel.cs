using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommerce.Model;

namespace Egate_Ecommerce.Objects
{
    public class EmployeeViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public long Id { get; set; }
        public string EmployeeName { get; set; }
        public bool IsActive { get; set; } = true;

        public EmployeeViewModel()
        { }

        public EmployeeViewModel(employee entity)
        {
            this.Id = entity.Id;
            this.EmployeeName = entity.EmployeeName;
            this.IsActive = entity.IsActive.ToBool();
        }

        public override bool Equals(object obj)
        {
            if (obj is EmployeeViewModel)
            {
                var o = obj as EmployeeViewModel;
                return this.Id == o.Id;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return this.Id.GetHashCode();
        }
    }
}
